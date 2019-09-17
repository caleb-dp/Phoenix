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
using Caleb.Library.CAL.Business;
using CalExtension.PlayerRoles;

namespace CalExtension.Abilities
{
//  Serial: 0x40382A1B  Name: "Machette of Lightshield craft"  Position: 105.138.0  Flags: 0x0000  Color: 0x042C  Graphic: 0x2D29  Amount: 1  Layer: None Container: 0x403174BC
//Serial: 0x4038B261  Name: "Ornate Axe of Garrou Strength"  Position: 100.81.0  Flags: 0x0000  Color: 0x06F4  Graphic: 0x2D28  Amount: 1  Layer: None Container: 0x403174BC


  public static class Warrior
  {
    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void MachetteofLightshield()
    {
      UOItem machette = World.Player.FindType(0x2D29, 0x042C);
      if (!machette.Exist)
        machette = World.Player.FindType(0x2D29);

      if (machette.Exist)
      {
        if (machette.Color == 0x042C)
        {
          Targeting.CancelClientTarget();
          Targeting.ResetTarget();
          machette.Use();
          UO.WaitTargetSelf();
          World.Player.PrintMessage("[ Maceta IJS ]");

          Game.Wait(250);
          if (!World.Player.Layers[Layer.LeftHand].Exist)
          {
            UOItem shield = Fighter.Current.GetSlotItem("__LastShield");
            if (!shield.Exist)
            {
              Fighter.Current.SwitchShield();
            }
            else
              shield.Use();
          }
        }
        else 
          World.Player.PrintMessage("[ Maceta neni nabita ]", MessageType.Warning);
      }
      else
      {
        World.Player.PrintMessage("[ Neni Maceta ]", MessageType.Error);
      }
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void OrnateAxeofGarrouStrength()
    {
      UOItem axe = World.Player.FindType(0x2D28, 0x06F4);

      if (!axe.Exist)
        axe = World.Player.FindType(0x2D28);

      if (axe.Exist)
      {
        if (axe.Color == 0x06F4)
        {
          Targeting.CancelClientTarget();
          Targeting.ResetTarget();
          axe.Use();
          UO.WaitTargetSelf();
          World.Player.PrintMessage("[ Sekyra STR ]");
        }
        else
          World.Player.PrintMessage("[ Sekyra neni nabita ]", MessageType.Warning);
      }
      else
      {
        World.Player.PrintMessage("[ Neni Sekyra ]", MessageType.Error);
      }
    }

    //---------------------------------------------------------------------------------------------
  }
}
