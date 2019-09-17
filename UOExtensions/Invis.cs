using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;
using System.Threading;

namespace CalExtension.UOExtensions
{
  //---------------------------------------------------------------------------------------------

  public class Invis
  {
    private static int time = 10000;
    private static int counter = 0;
    private static LastLocation lastLocation;
    private static int eatFish = 0;


    private class LastLocation
    {
      public LastLocation(ushort x, ushort y)
      {
        X = x;
        Y = y;
      }
      public ushort X;
      public ushort Y;
    }


    //---------------------------------------------------------------------------------------------

    private static System.Timers.Timer _t;
    private static System.Timers.Timer t
    {
      get
      {
        if (_t == null)
        {
          _t = new System.Timers.Timer();
          _t.Elapsed += TimerElapsed;
          _t.Enabled = true;
          _t.Interval = 50;
        }
        return _t;
      }
    }


    //---------------------------------------------------------------------------------------------

    private static void Stop()
    {
      t.Stop();
      lastLocation = null;
      counter = 0;
      UO.Print(0x0032, "Invis - konec");
    }

    //---------------------------------------------------------------------------------------------

    public static double GetVectorLength(int x, int y)
    {
      double vLength = Math.Pow((Math.Pow(x, 2) + Math.Pow(y, 2)), 0.5);
      return vLength;
    }

    //---------------------------------------------------------------------------------------------

    public static void GetRelativeCoordinates(ushort startX, ushort startY, ushort originalX, ushort originalY, out int x, out int y)
    {
      x = (originalX - startX);
      y = (originalY - startY);
    }

    //---------------------------------------------------------------------------------------------

    public static double GetRelativeVectorLength(ushort startX, ushort startY, ushort originalX, ushort originalY)
    {
      int x;
      int y;
      GetRelativeCoordinates(startX, startY, originalX, originalY, out x, out y);

      return GetVectorLength(x, y);
    }


    //---------------------------------------------------------------------------------------------
    // Color: 0x0B77  Graphic: 0x0F09

    public static void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
      counter += 50;

      if (lastLocation != null && GetRelativeVectorLength(World.Player.X, World.Player.Y, lastLocation.X, lastLocation.Y) > 2)
      {
        UO.Print(0x0032, "Kop detect! Hide");
        Stop();

        UO.Wait(Core.CurrentLatency + 75);
        UO.UseType(0x0F09, 0x0B77);

        for (int i = 0; i < eatFish; i++)
        {
          if (World.Player.Backpack.AllItems.FindType(0x09CD, 0x084C).Exist)
          {
            UO.Wait(400);
            UO.UseType(0x09CD, 0x084C);
          }
          else
            break;
        }

        eatFish = 0;
        return;
      }


      if (counter % 1000 == 0)
      {
        UO.Print(0x0032, String.Format("Invis ETA: {0:N1}", ((time - counter) / 1000.0m)));
      }

      if (counter >= time)
      {
        Stop();
        eatFish = 0;
        return;
      }

      lastLocation = new LastLocation(World.Player.X, World.Player.Y);
    }

    [Executable]
    public static void RunInvis1()
    {
      RunInvis(1);
    }

    //---------------------------------------------------------------------------------------------
    //0x084C  Graphic: 0x09CD
    [Executable]
    public static void RunInvis()
    {
      RunInvis(0);
    }

    [Executable]
    public static void RunInvis(int eat)
    {
      eatFish = eat;
      t.Stop();
      lastLocation = null;
      //counter = 0;

      if (UO.Backpack.AllItems.FindType(0x0F09, 0x0B77).Exist)
      {
        UO.Print(0x0032, "Invis - Start");
        t.Start();
      }
      else
        UO.Print(0x0032, "Invis - Nemas invisku!!");
    }


    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void RunWaitForInvis(int time)
    {
      if (World.Player.Backpack.AllItems.Count(0x0F09, 0x0B77) == 0)
      {
        World.Player.PrintMessage("Nemas Invisku!", MessageType.Warning);
        return;
      }

      bool hidden = World.Player.Hidden;

      int counter = 0;
      while (counter < time)
      {
        counter += 25;
        UO.Wait(25);

        if (!hidden && World.Player.Hidden)
          hidden = true;

        if (hidden && !World.Player.Hidden)
        {
          UO.UseType(0x0F09, 0x0B77);


        }

        if (counter % 2000 == 0)
          UO.Print(0x0032, String.Format("Wait Invis ETA: {0:N1}", ((time - counter) / 1000.0m)));
      }
    }

    //---------------------------------------------------------------------------------------------
  }
}


