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
  public class Bowcraft : Skill
  {

    public static UOItemType Log { get { return new UOItemType() { Graphic = 0x1BDD, Color = 0x0000 }; } }
    public static UOItemType Kudla { get { return new UOItemType() { Graphic = 0x0F51, Color = 0x0000 }; } }
    public static UOItemType Kindling { get { return new UOItemType() { Graphic = 0x0DE1, Color = 0x0000 }; } }

    //Color: 0x03B9  Graphic:  
      
    //---------------------------------------------------------------------------------------------

    public void BowTrain()
    {
      Game.PrintMessage("Zvol kontejner s materialem > (esc) = ground");
      UOItem containerFrom = new UOItem(UIManager.TargetObject());

      Journal.Clear();

      while (!UO.Dead)
      {
        if (!World.Player.Backpack.Items.FindType(Log.Graphic).Exist)
        {
          Game.PrintMessage("Doplnuji Log");
          UOItem materials = new UOItem(Serial.Invalid);
          if (containerFrom.Exist)
            materials = containerFrom.Items.FindType(Log.Graphic);

          if (materials.Exist)
          {
            materials.Move(2, World.Player.Backpack);
            Game.Wait();
            World.Player.Backpack.Items.FindType(Log.Graphic).Move(1, World.Player.Backpack, 10, 10);
          }
          else
          {
            Game.PrintMessage("Neni Log");
            break;
          }
          Game.Wait();

          UOItem items = World.Player.Backpack.Items.FindType(Kindling.Graphic);
          if (items.Exist && items.Amount > 100)
          {
            items.Move(100, containerFrom);
            Game.Wait();
          }
        }

        UO.WaitTargetType(Log.Graphic);
        UO.WaitMenu("Bowcraft", "Kindling");
        UO.UseType(Kudla.Graphic);

        Journal.WaitForText(true, 5000, "You have failed", "You can't make anything", "You put");//, "You fail to");

        if (Journal.Contains("You can't make anything"))
        {
          Game.PrintMessage("Nemas suroviny");
        }

        Journal.Clear();
      }
      Game.PrintMessage("Konec treninku bowcraft");
    }

    //---------------------------------------------------------------------------------------------

    #region exec

    //---------------------------------------------------------------------------------------------

    [Executable("BowTrain")]
    [BlockMultipleExecutions]
    public static void ExecBowTrain()
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Bowcraft>().BowTrain();
    }


    #endregion
  }
}
