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
  public class Cartography : Skill
  {
    public static UOItemType AtlasType { get { return new UOItemType() { Graphic = 0x0FBE, Color = 0x0B98 }; } }

    //---------------------------------------------------------------------------------------------

    public UOItem Atlas
    {
      get
      {
        if (World.Player.Backpack.AllItems.FindType(AtlasType.Graphic, AtlasType.Color).Exist)
          return World.Player.Backpack.AllItems.FindType(AtlasType.Graphic, AtlasType.Color);
        return new UOItem(Serial.Invalid);
      }
    }

    //---------------------------------------------------------------------------------------------


    public void TrainCarth()
    {
      Game.CurrentGame.Mode = GameMode.Working;
      if (this.Atlas.Exist)
      {
        bool prevOk = true;
        while (this.Atlas.Amount > 0)
        {
          Journal.Clear();

          if (prevOk)
          {
            UO.WaitTargetSelf();
            this.Atlas.Use();
            Game.Wait(350);
          }

          UO.WaitMenu("What sort of map do you want to draw ?", "Detail Map");
          UO.UseType(0x14EB, 0x0000);
          JournalEventWaiter jew = new JournalEventWaiter(true, "You put the map", "Thy trembling hand results in an unusable map");
          jew.Wait(4000);

          //if (Journal.Contains(true, "Thy trembling hand results in an unusable map"))
          //{
          //  prevOk = false;
          //}
          //else
          //{
            prevOk = true;
            UO.WaitTargetType(0x14EB, 0x0000);
            this.Atlas.Use();
            Game.Wait();
          //}
          //Game.PrintMessage("Pocet map: " + this.Atlas.Amount);
          Journal.Clear();
        }
      }
      else
        Game.PrintMessage("Nemas u sebe atlas!");
    }

    #region exec

    [Executable("TrainCarth")]
    [BlockMultipleExecutions]
    public static void ExecTrainCarth()
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Cartography>().TrainCarth();
    }

    #endregion  
  }
}
