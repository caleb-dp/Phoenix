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
  public class Forensic : Skill
  {
//Serial: 0x40363D29  Name: "clean bandages"  Position: 3167.30.26  Flags: 0x0020  Color: 0x0000  Graphic: 0x0E21  Amount: 29339  Layer: None  Container: 0x00000000

    //---------------------------------------------------------------------------------------------

    public void TrainForensic()
    {
      UOItem body = new UOItem(UIManager.TargetObject());
      body.Click();
      Game.Wait();
      Game.PrintMessage("Train na tele: " + body.Name);

      while (!UO.Dead && body.Exist)
      {
        Journal.Clear();

        UO.WaitTargetObject(body);
        UO.UseSkill(StandardSkill.ForensicEvaluation);
        Journal.WaitForText(true, 2500, "this is body of", "You can tell nothing about the corpse.");// Game.Wait(1500);
        Game.Wait(150);
      }
    }

    //---------------------------------------------------------------------------------------------

    #region exec

    [Executable("TrainForensic")]
    public static void TrainForensicExec()
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Forensic>().TrainForensic();
    }


    #endregion
  }
}
