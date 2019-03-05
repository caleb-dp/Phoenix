using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;
using Caleb.Library;
using System.Threading;
using CalExtension.Skills;
using CalExtension.UOExtensions;
using Caleb.Library.CAL.Business;

namespace CalExtension.Skills
{
  public class Fighting : Skill
  {
    //Serial: 0x40363D29  Name: "clean bandages"  Position: 3167.30.26  Flags: 0x0020  Color: 0x0000  Graphic: 0x0E21  Amount: 29339  Layer: None  Container: 0x00000000
    

    //---------------------------------------------------------------------------------------------

    public void TrainMob()
    {
      Game.CurrentGame.Mode = GameMode.Working;
      Game.PrintMessage("Vyber mob...");
      UOCharacter mob = new UOCharacter(UIManager.TargetObject());
      if (mob.Exist)
      {
        UO.Warmode(true);
        Game.Wait();
        UO.Attack(mob);
        Game.Wait();
        UO.WaitTargetSelf();
        UO.Say("all kill");

        while (!UO.Dead)
        {
          if ((mob.MaxHits > 0 && (decimal)mob.Hits / (decimal)mob.MaxHits <= 0.65m) || (World.Player.MaxHits > 0 && (decimal)World.Player.Hits / (decimal)World.Player.MaxHits <= 0.65m))
            {
              UO.Say("all stop");
              Game.Wait();
              UO.Warmode(false);
              Game.Wait();

              while (mob.Hits > 0 && mob.Hits < mob.MaxHits)
              {
                UOItem banda = World.Ground.FindType(0x0E21);
                if (banda.Exist)
                {
                  banda.Use();
                  UO.WaitTargetObject(mob);
                  Game.Wait(2500);
                }
                else
                {
                  Game.PrintMessage("Neni banda...");
                  return;
                }
              }

              while (World.Player.Hits < World.Player.MaxHits)
              {
                UOItem banda = World.Ground.FindType(0x0E21);
                if (banda.Exist)
                {
                  banda.Use();
                  UO.WaitTargetSelf();
                  Game.Wait(2500);
                }
                else
                {

                  Game.PrintMessage("Neni banda...");
                  return;
                }
              }

              if (!World.Player.Backpack.Items.FindType(0x0F3F).Exist)
              {
                foreach (UOItem item in World.Player.Backpack.AllItems)
                {
                  if (item.Graphic == 0x1EA0)
                  {
                    item.Use();
                    Game.Wait();

                    if (World.Player.Backpack.Items.FindType(0x0F3F).Exist)
                      break;
                  }
                }
              }


              UO.Warmode(true);
              Game.Wait();
              UO.Attack(mob);
              Game.Wait();
              UO.WaitTargetSelf();
              UO.Say("all kill");
            }
            else
            {


              Game.Wait(1000);
              UO.Attack(mob);
            }
        }
      }
      else
        Game.PrintMessage("Mob neexistuje...");

    }

    //---------------------------------------------------------------------------------------------

    #region exec

    [Executable("FightTrain")]
    public static void FightTrainExec()
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Fighting>().TrainMob();
    }

    //---------------------------------------------------------------------------------------------

     [Executable("PrintCurrenSkillStatus")]
    public static void PrintCurrenSkillStatus()
    {
      SkillValue mf = SkillsHelper.GetSkillValue("MaceFighting");
      SkillValue fe = SkillsHelper.GetSkillValue("Fencing");
      SkillValue sw = SkillsHelper.GetSkillValue("Swordsmanship");
      SkillValue wr = SkillsHelper.GetSkillValue("Wrestling");

      Game.PrintMessage(String.Format("{0} - {1} / {2}", "MaceFighting", mf.RealValue, mf.MaxValue));
      Game.PrintMessage(String.Format("{0} - {1} / {2}", "Wrestling", wr.RealValue, wr.MaxValue));
      Game.PrintMessage(String.Format("{0} - {1} / {2}", "Fencing", fe.RealValue, fe.MaxValue));
      Game.PrintMessage(String.Format("{0} - {1} / {2}", "Swordsmanship", sw.RealValue, sw.MaxValue));
    }


    #endregion
  }
}
