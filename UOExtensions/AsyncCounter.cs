

using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;
using Caleb.Library;
using System.Threading;

namespace CalExtension.UOExtensions
{
  //---------------------------------------------------------------------------------------------

  public class AsyncCounter
  {
    public static AsyncCounter Current;

    public AsyncCounter()
    {
       
    }

    public int MaxTries = 20;
    public int Step = 250;
    public int HighlightTime = 0;
    public ushort HighlightColor = 0x0020;
    public string StopMessage = "";
    public string PrefixText = "Time: ";
    public delegate bool AsyncCounterEndMethod();
    public AsyncCounterEndMethod StopMethod;
    private Thread spearateTh;
    public void Run()
    {
      if (Current != null)
      {
        if (Current.spearateTh != null && Current.spearateTh.IsAlive)
          Current.spearateTh.Abort();
      }

      spearateTh = new Thread(new ThreadStart(Start));
      spearateTh.Start();
      Current = this;
    }

    protected void Start()
    {
      //World.Player.PrintMessage(MessageType.Info, String.Format(this.PrefixText + ": {0:00.00}", 0));

      DateTime start = DateTime.Now;
      decimal counter = 0;
      Journal.Clear();
      int time = 0;
      for (int i = 1; i <= MaxTries; i++)
      {

        Thread.Sleep(Step);
        time += Step;
        counter = counter + (decimal)(Step / 1000.0m);

        if (Skills.Hiding.HideRunning)
        {
          if (HighlightTime > 0 && time >= HighlightTime)
            World.Player.PrintMessage(String.Format(PrefixText + "{0:#0.00}", counter), HighlightColor);
          else
            World.Player.PrintMessage(String.Format(PrefixText + "{0:#0.00}", counter));
        }
        if (!String.IsNullOrEmpty(StopMessage) && ContainMessages())// || StopMethod.Invoke()/* || (StopMethod != null && StopMethod())*/)
          break;
      }
    }

    //---------------------------------------------------------------------------------------------

    protected bool ContainMessages()
    {
      foreach (string message in this.StopMessage.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
      {
        if (Journal.Contains(true, message))
          return true;
      }

      return false;
    }

    //---------------------------------------------------------------------------------------------
  }
}
