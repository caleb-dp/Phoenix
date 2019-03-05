using Phoenix;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CalExtension;
using CalExtension.UOExtensions;


namespace CalExtension.Abilities
{
  public static class Ranger
  {
    //---------------------------------------------------------------------------------------------
    //Serial: 0x40069137  Name: "Arrow quiver (1400) sipu"  Position: 135.78.0  Flags: 0x0000  Color: 0x0747  Graphic: 0x1EA0  Amount: 1  Layer: None  Container: 0x4029398B

    [Executable]
    public static void EnsureArcherAmmo()
    {
      UOItem bolts = World.Player.Backpack.AllItems.FindType(0x1BFB);
      UOItem quiver = World.Player.Backpack.AllItems.FindType(0x1EA0, 0x083A);
      bool requipBolts = false;
      bool requipArrows = false;

      if (!bolts.Exist)
      {

        if (quiver.Exist)
        {
          quiver.Use();
          Game.Wait(100);
          bolts = World.Player.Backpack.AllItems.FindType(0x1BFB);
          requipBolts = true;
        }
      }


      UOItem arrows = World.Player.Backpack.AllItems.FindType(0x0F3F);
      UOItem quiverArrow = World.Player.Backpack.AllItems.FindType(0x1EA0, 0x0747);
      if (!arrows.Exist)
      {

        if (quiverArrow.Exist)
        {
          quiverArrow.Use();
          Game.Wait(100);
          arrows = World.Player.Backpack.AllItems.FindType(0x0F3F);
          requipArrows = true;
        }
      }

      if (quiver.Exist)
      {
        if (requipBolts)
        {
          UOItemExtInfo info = ItemHelper.GetItemExtInfo(quiver);
          World.Player.PrintMessage("[" + info.Charges + " " + info.ChargesName + "]", Game.Val_LightGreen);
        }
        else
          Game.PrintMessage("Bolts - " + bolts.Amount);
      }

      if (quiverArrow.Exist)
      {
        if (requipArrows)
        {
          UOItemExtInfo info = ItemHelper.GetItemExtInfo(quiverArrow);
          World.Player.PrintMessage("[" + info.Charges + " " + info.ChargesName + "]", Game.Val_LightGreen);
        }
        else
          Game.PrintMessage("Arrows - " + arrows.Amount);
      }
    }

    //---------------------------------------------------------------------------------------------
  }
}
