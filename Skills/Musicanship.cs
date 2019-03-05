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

namespace CalExtension.Skills
{
  public class Musicanship : Skill
  {
    //---------------------------------------------------------------------------------------------

    public void TrainMusic()
    {
      Game.CurrentGame.CurrentPlayer.SwitchWarmode();
      if (UO.Backpack.AllItems.FindType(0x0E9D, 0x0000).Exist)
      {
        
        Game.PrintMessage("Startuji Musican Train");
        while (!UO.Dead)
        {
          Journal.Clear();
          UO.UseType(0x0E9D, 0x0000);
          if (Journal.WaitForText(true, 2500, "You play poorly, and there is no effects")) { }

          if (Journal.Contains(true, " You are preoccupied with thoughts of battle"))
          {
            Game.CurrentGame.CurrentPlayer.SwitchWarmode();
            Game.Wait();
          }
          Game.Wait(50);
        }

      }
      else
        Game.PrintMessage("Nemas TAmburinu");
    }


    //---------------------------------------------------------------------------------------------

    #region exec

    [Executable("TrainMusic")]
    public static void ExecTrainMusic()
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Musicanship>().TrainMusic();
    }


    #endregion
  }
}
