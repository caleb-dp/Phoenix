using Phoenix;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CalExtension;
using CalExtension.UOExtensions;
using Caleb.Library;
using CalExtension.Skills;

namespace CalExtension.Abilities
{
  public static class GolemMaster
  {
    public static IUOItemType IronIngot { get { return new UOItemTypeBase(0x1BEF, 0x0000);  } }
    public static IUOItemType IronWall { get { return new UOItemTypeBase(0x2206, 0x0835); } }//Todo color

    //---------------------------------------------------------------------------------------------

    public static UOItem AdaHammer
    {
      get { return World.Player.FindType(0x1438, 0x044C); }
    }

    //---------------------------------------------------------------------------------------------

    //[Executable]
    //public static void CastFromEbony(string target)
    //{

    //}

    ////---------------------------------------------------------------------------------------------

    //[Executable]
    //public static void CastFromEbony(string target)
    //{

    //}

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void CastFromIron(string target, int ingotAmount)
    {
      UOItem ingots = World.Player.Backpack.AllItems.FindType(IronIngot.Graphic, IronIngot.Color);

      if (!AdaHammer.Exist)
      {
        World.Player.PrintMessage("[Neni adahammer..]", MessageType.Error);
        return;
      }

      if (!ingots.Exist || ingots.Amount < ingotAmount)
      {
        Game.PrintMessage(String.Format("Nemas dostatecny pocet Ironu ({0})", ingots.Amount), MessageType.Error);
        return;
      }

      string msg = "1x1";
      if (ingotAmount >= 25)
        msg = "3x3";

      World.Player.PrintMessage("[Wall " + msg + "...]", Game.Val_GreenBlue);

      TargetInfo targetInfo = Targeting.GetTarget(target);
      if (targetInfo.Success)
      {
        ushort distance = targetInfo.Position.Distance();
        if (distance <= 3)
        {
          if (ingots.Move((ushort)ingotAmount, targetInfo.StaticTarget.X, targetInfo.StaticTarget.Y, targetInfo.StaticTarget.Z))
          {
            UO.WaitTargetObject(ingots);
            AdaHammer.Use();
          }
        }
        else
        {
          World.Player.PrintMessage("[3 policka max..]", Game.Val_LightPurple);

          Game.PrintMessage("Vzdalenost: " + distance, Game.Val_LightPurple);
        }
      }

    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void CastWallTest()
    {
      TargetInfo targetInfo = Targeting.GetTarget(null);

      if (targetInfo.Success)
      {
        Game.PrintMessage(String.Format("[" + targetInfo.StaticTarget.X + ":" + targetInfo.StaticTarget.Y + "] {0:N4}", Robot.GetAngle(World.Player.GetPosition(), targetInfo.Position)) + "");

        UO.WaitTargetTile(targetInfo.StaticTarget.X, targetInfo.StaticTarget.Y, targetInfo.StaticTarget.Z, targetInfo.StaticTarget.Graphic);
        UO.Cast(StandardSpell.WallofStone);

      }
    }

    //    3344.357
    //02:59 Phoenix: [3344:357] -1,0000 WE - SELF

    //02:56 Phoenix: [3344:360] 0,0000 WE
    //02:56 Phoenix: [3345:360] 18,4349 WE
    //02:56 Phoenix: [3346:360] 33,6901 WE
    //02:57 Phoenix: [3346:359] 45,0000 WE
    //02:57 Phoenix: [3347:359] 56,3099 SN
    //02:58 Phoenix: [3347:358] 71,5651 SN
    //02:58 Phoenix: [3347:357] 90,0000 SN
    //02:58 Phoenix: [3347:356] 108,4349 SN
    //02:58 Phoenix: [3347:355] 123,6901 SN
    //02:58 Phoenix: [3347:354] 135,0000 WE
    //02:59 Phoenix: [3346:354] 146,3099 WE
    //02:59 Phoenix: [3345:354] 161,5651 WE
    //02:59 Phoenix: [3344:354] 180,0000 WE

    //03:01 Phoenix: [3343:354] 198,4349 WE
    //03:01 Phoenix: [3342:354] 213,6901 WE
    //03:01 Phoenix: [3341:354] 225,0000 WE
    //03:01 Phoenix: [3341:355] 236,3099 SN
    //03:02 Phoenix: [3341:356] 251,5651 SN
    //03:02 Phoenix: [3341:357] 270,0000 SN
    //03:02 Phoenix: [3341:358] 288,4349 SN
    //03:02 Phoenix: [3341:359] 303,6901 SN
    //03:02 Phoenix: [3342:359] 315,0000 WE
    //03:04 Phoenix: [3342:360] 326,3099 WE
    //03:03 Phoenix: [3343:359] 333,4349 WE
    //03:04 Phoenix: [3343:360] 341,5651 WE

    //WE-L X-
    //SN-L Y+

    //---------------------------------------------------------------------------------------------
 //   Serial: 0x402BAB77  Name: "Zelezny crystal"  Position: 3347.358.4  Flags: 0x0000  Color: 0x0835  Graphic: 0x2206  Amount: 0  Layer: None Container: 0x00000000

    [Executable]
    public static void CastWallOfIron(string target, int sizeL, int sizeR, string direction)
    {
      //if (!AdaHammer.Exist)
      //{
      //  World.Player.PrintMessage("[Neni adahammer..]", MessageType.Error);
      //  return;
      //}

      World.Player.PrintMessage("[WoI " + (sizeL + sizeR + 1) + "...]", Game.Val_GreenBlue);
      TargetInfo targetInfo = Targeting.GetTarget(target);
      
      if (targetInfo.Success)
      {
        IUOPosition startPostion = targetInfo.Position;
        //direction
        //SN
        //WE
        List<IUOPosition> positions = new List<IUOPosition>();
        positions.Add(startPostion); ;

        double angle = Robot.GetAngle(World.Player.GetPosition(), startPostion);
        if (String.IsNullOrEmpty(direction))
        {
          if (angle <= 45)
            direction = "WE";
          else if (angle < 135)
            direction = "SN";
          else if (angle <= 225)
            direction = "WE";
          else if (angle < 315)
            direction = "SN";
          else
            direction = "WE";
        }

        Game.PrintMessage(String.Format(direction + " [" + targetInfo.StaticTarget.X + ":" + targetInfo.StaticTarget.Y + "] {0:N4}", angle) + "");

        if (direction == "WE")
        {
          for (int i = 1; i <= sizeL; i++)
            positions.Add(new UOPositionBase((ushort)(startPostion.X - 1), (ushort)(startPostion.Y), startPostion.Z.Value));

          for (int i = 1; i <= sizeR; i++)
            positions.Add(new UOPositionBase((ushort)(startPostion.X + 1), (ushort)(startPostion.Y), startPostion.Z.Value));
        }
        else
        {
          for (int i = 1; i <= sizeL; i++)
            positions.Add(new UOPositionBase((ushort)(startPostion.X), (ushort)(startPostion.Y + 1), startPostion.Z.Value));

          for (int i = 1; i <= sizeR; i++)
            positions.Add(new UOPositionBase((ushort)(startPostion.X), (ushort)(startPostion.Y - 1), startPostion.Z.Value));
        }

        positions = positions.Where(p => p.Distance() <= 3).OrderByDescending(p => p.Distance()).ToList();

        DateTime start = DateTime.Now;
        World.FindDistance = 6;
        List<UOItem> ground = World.Ground.Where(i => i.Distance <= 5).ToList();
        List<UOItem> groundIngots = new List<UOItem>();

        foreach (IUOPosition p in positions)
        {
          bool done = false;
         // UOItem ingot = null;
          foreach (UOItem g in ground)
          {
            if (g.X == p.X && g.Y == p.Y)
            {
              if (g.Graphic == IronIngot.Graphic && g.Color == IronIngot.Color && g.Amount == 10)
              {
                //ingot = g;
                groundIngots.Add(g);
                done = true;
                break;
              }

              if (g.Graphic == IronWall.Graphic && g.Color == IronWall.Color)//todo color
              {
                done = true;
                break;
              }
            }
          }

          if (!done)
          {
            UOItem ingots = World.Player.Backpack.AllItems.FindType(IronIngot.Graphic, IronIngot.Color);

            if (ingots.Amount >= 10 && ingots.Move((ushort)10, p.X.Value, p.Y.Value, (sbyte)p.Z.Value))
            {
              //ingot = ingots;
              groundIngots.Add(ingots);
              Game.Wait(50);
            }
          }
        }

        foreach (UOItem ingot in groundIngots)
        {
          if (ingot != null && AdaHammer.Exist)
          {
            UO.WaitTargetObject(ingot);
            AdaHammer.Use();
            Game.Wait(125);
          }
        }

        World.Player.PrintMessage("Move: " + (DateTime.Now - start).TotalMilliseconds);
        start = DateTime.Now;
      }
    }

    //---------------------------------------------------------------------------------------------
  }
}
