using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;
using System.Threading;
using Caleb.Library.CAL.Business;
using Caleb.Library;
using CalExtension.Skills;

namespace CalExtension.UOExtensions
{
  //---------------------------------------------------------------------------------------------

  public class Bandana
  {
    private static int time = 10000;
    private static int counter = 0;
    private static LastLocation lastLocation;
    public static UOItemType CraftBandana { get { return new UOItemType() { Graphic = 0x153F, Color = 0x0485 }; } }
    //Color: 0x0485  Graphic: 0x153F
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

      Game.PrintMessage("Bandana - Konec");
    }

    //---------------------------------------------------------------------------------------------
    // Color: 0x0B77  Graphic: 0x0F09

    public static void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
      counter += 50;

      if (lastLocation != null && Invis.GetRelativeVectorLength(World.Player.X, World.Player.Y, lastLocation.X, lastLocation.Y) > 4)
      {
        World.Player.PrintMessage("[Detect.. Hide!]", Game.Val_GreenBlue);
        Stop();

        Game.Wait(75);
        Hiding.CraftBandana();

        return;
      }


      if (counter % 1000 == 0)
      {
        Game.PrintMessage(String.Format("Bandana ETA: {0:N1}", ((time - counter) / 1000.0m)), Game.Val_LightPurple);
      }

      if (counter >= time)
      {
        Stop();
        return;
      }

      lastLocation = new LastLocation(World.Player.X, World.Player.Y);
    }

    //---------------------------------------------------------------------------------------------
    //0x084C  Graphic: 0x09CD
    [Executable]
    public static void RunBandana()
    {
      t.Stop();
      lastLocation = null;

      UOItem bandana = World.Player.FindType(CraftBandana);
      if (bandana.Exist)//UO.Backpack.AllItems.FindType(0x0F09, 0x0B77).Exist)
      {
        if (bandana.Layer == Layer.Hat)
        {
          if (bandana.Move(1, World.Player.Backpack))
            Game.Wait();
          else
          {
            Game.PrintMessage("Nepodarilo se sundat bandanu", MessageType.Error);
            return;
          }
        }

        Game.PrintMessage("Bandana - Start");
        t.Start();
      }
      else
        Game.PrintMessage("Nemas bandanu", MessageType.Error);
    }

    //---------------------------------------------------------------------------------------------
  }
}
