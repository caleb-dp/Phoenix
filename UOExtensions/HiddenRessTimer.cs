using Phoenix;
using Phoenix.Communication.Packets;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Xml;

namespace CalExtension.UOExtensions
{
  [RuntimeObject]
  public class HiddenRessTimer
  {
    //---------------------------------------------------------------------------------------------
    public static HiddenRessTimer Current;
    private Thread TimerThread;

    public HiddenRessTimer()
    {
      if (Current == null)
      {
        Current = this;
        IngameConsoleOverride.AsciiSpeechBeforeOverride += IngameConsoleOverride_AsciiSpeechBeforeOverride;
      }
    }

    //---------------------------------------------------------------------------------------------

    private void IngameConsoleOverride_AsciiSpeechBeforeOverride(object sender, EventArgs e)
    {
      AsciiSpeech packet = (AsciiSpeech)sender;
      string textSafe = packet.Text + String.Empty;

      bool hideRess = textSafe.Contains("Resurrect narusil celistvost tveho ukryti v stinech");

      if (hideRess && World.Player.Hidden)
      {
        if (TimerThread != null)
          TimerThread.Abort();

        timerStop = 35;
        timerCounter = 0;
        TimerThread = new Thread(new ThreadStart(StartTimer));
        TimerThread.Start();
      }
      
      if (TimerThread != null)
      {
        if (textSafe.Contains("You have been revealed") || textSafe.Contains("You have hidden yourself well"))
        {
          TimerThread.Abort();

          int remain = timerStop - timerCounter;
          if (remain > 1)
            Game.PrintMessage(String.Format("Hid reset v {0}", timerStop - timerCounter));

        }
      }
  }

    //---------------------------------------------------------------------------------------------
    private int timerStop = 35;
    private int timerCounter = 0;
    private void StartTimer()
    {
      while (timerCounter < timerStop)
      {
        Thread.Sleep(1000);
        timerCounter += 1;

        if (timerCounter >= timerStop)
        {
          Thread.CurrentThread.Abort();
          return;
        }

        int remain = timerStop - timerCounter;

        if ((remain > 5 && remain % 5 == 0 || remain <= 10))
          Game.PrintMessage(String.Format("Odhid za {0}", remain));

      }
      Thread.CurrentThread.Abort();
    }

    //---------------------------------------------------------------------------------------------
  }

}