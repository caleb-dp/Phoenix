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
  public static class Guardian
  {
    //---------------------------------------------------------------------------------------------

    public static UOItemTypeBase VAL_GuardianScale { get { return new UOItemTypeBase(0x0E2A, 0x0000); } }
    public static UOItemTypeBase VAL_GuardianShieldOfAncients { get { return new UOItemTypeBase(0x1B76, 0x0B77); } }
    public static UOItemTypeBase VAL_GuardianShieldOfAncientsCharged { get { return new UOItemTypeBase(0x1B76, 0x049B); } }
    public static UOItemTypeBase VAL_GuardianShieldOfLastStand { get { return new UOItemTypeBase(0x1B72, 0x0B98); } }
    public static UOItemTypeBase VAL_GuardianShieldOfLastStandCharged { get { return new UOItemTypeBase(0x1B76, 0x0B77); } }

    //---------------------------------------------------------------------------------------------

    //TODO poradi podle Serialu >>>> 
    [Executable]
    public static bool ShieldOfLastStand()
    {
      bool result = false;
      UOItem scale = World.Player.FindType(VAL_GuardianScale);
      if (scale.Exist)
      {
        UOItem shield = World.Player.Backpack.AllItems.FindType(VAL_GuardianShieldOfLastStand);
        UOItem originalShield = World.Player.Layers[Layer.LeftHand];
        if (shield.Exist)
        {
          shield.Use();
          Game.Wait(250);
        }

        UOItem shieldCharged = World.Player.FindType(VAL_GuardianShieldOfLastStandCharged);
        if (shieldCharged.Exist)
        {
          UO.WaitMenu("Vyber Bonus", "Shield of Last Stand");
          scale.Use();

          if (Journal.WaitForText(true, 200, "You used"))
          {
            shieldCharged.Move(1, World.Player.Serial);
            Game.Wait(250);
          }
          result = true;
        }

        if (originalShield.Exist && originalShield.Serial != World.Player.Layers[Layer.LeftHand].Serial)
        {
          originalShield.Use();
        }
      }
      return result;
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static bool ShieldOfAncients()
    {
      bool result = false;
      UOItem scale = World.Player.FindType(VAL_GuardianScale);
      if (scale.Exist)
      {
        UOItem shield = World.Player.Backpack.AllItems.FindType(VAL_GuardianShieldOfAncients);
        UOItem originalShield = World.Player.Layers[Layer.LeftHand];
        if (shield.Exist)
        {
          shield.Use();
          Game.Wait(250);
        }

        UOItem shieldCharged = World.Player.FindType(VAL_GuardianShieldOfAncientsCharged);
        if (shieldCharged.Exist)
        {
          UO.WaitMenu("Vyber Bonus", "Shield of Ancients");
          scale.Use();

          if (Journal.WaitForText(true, 200, "You used"))
          {
            shieldCharged.Move(1, World.Player.Serial);
            Game.Wait(250);
          }
          result = true;
        }

        if (originalShield.Exist && originalShield.Serial != World.Player.Layers[Layer.LeftHand].Serial)
        {
          originalShield.Use();
        }
      }
      return result;
    }


    //---------------------------------------------------------------------------------------------
  }
}
