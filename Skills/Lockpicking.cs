using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;
using Caleb.Library;
using System.Threading;
using CalExtension.Skills;
using Caleb.Library.CAL.Business;

namespace CalExtension.Skills
{
  public class Lockpicking : Skill
  {
    public static UOItemType Lockpicks { get { return new UOItemType() { Graphic = 0x14FB, Color = 0x0000, Name = "Lockpicks" }; } }

    //---------------------------------------------------------------------------------------------

    public void Unlock()
    {
      Journal.Clear();
      Game.PrintMessage("Truhla >");
      UOItem chest = new UOItem(UIManager.TargetObject());

      while (!UO.Dead && chest.Exist)
      {
        if (!World.Player.Backpack.Items.FindType(Lockpicks.Graphic).Exist)
        {
          Game.PrintMessage("Nejsou locky!");
          break;  
        }

        UO.UseType(Lockpicks.Graphic);
        UO.WaitTargetObject(chest.Serial);

//        Journal.WaitForText(false, 2500, "You fail to unlock the item", "This item cannot be lockpicked");
        Game.Wait(1500);
        if (Journal.Contains(true, "This item cannot be lockpicked"))
        {
          Game.PrintMessage("Odemknuto.");
          break;
        }
        Journal.Clear();
      }
      Game.PrintMessage("Unlock konec.");
    }

    //---------------------------------------------------------------------------------------------

    public void TrainLock()
    {
      Game.CurrentGame.Mode = GameMode.Working;
      Journal.Clear();
      Game.PrintMessage("Truhla >");
      UOItem chest = new UOItem(UIManager.TargetObject());
      Game.PrintMessage("Klic >");
      UOItem key = new UOItem(UIManager.TargetObject());
      Game.PrintMessage("Kontejner s locky >");
      UOItem container = new UOItem(UIManager.TargetObject());

      while (!UO.Dead && chest.Exist)
      {
        if (!World.Player.Backpack.Items.FindType(Lockpicks.Graphic).Exist)
        {
          UOItem locky = container.Items.FindType(Lockpicks.Graphic);
          if (locky.Exist)
          {
            locky.Move(100, World.Player.Backpack);
            Game.Wait();
          }
          else
          {
            Game.PrintMessage("Nejsou locky!");
            break;
          }
        }

        UO.UseType(Lockpicks.Graphic);
        UO.WaitTargetObject(chest.Serial);

        Game.Wait(1500);
        if (Journal.Contains(true, "This item cannot be lockpicked"))
        {
          Game.PrintMessage("Odemknuto.");


          UO.WaitTargetObject(chest.Serial);
          key.Use();
          Game.Wait();
        }
        Journal.Clear();
      }
      Game.PrintMessage("Unlock konec.");
    }



    //---------------------------------------------------------------------------------------------

    #region exec

    [Executable("Unlock")]
    [BlockMultipleExecutions]
    public static void ExecUnlock()
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Lockpicking>().Unlock();
    }


    [Executable("TrainLock")]
    [BlockMultipleExecutions]
    public static void ExecTrainLock()
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Lockpicking>().TrainLock();
    }

    #endregion
  }
}
