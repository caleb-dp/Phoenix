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
  public static class Bishop
  {
    //---------------------------------------------------------------------------------------------
    //*You activated KVM*
    //18:36 System: You can't cast this spell yet

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static CastResultInfo CastBishopGreaterHeal(string target)
    {
      World.Player.PrintMessage("GHeal [Bsp]", Game.Val_Green);
      return CastBishopGreaterHeal(true, Targeting.GetTarget(target), true);
    }

    //---------------------------------------------------------------------------------------------

    public static CastResultInfo CastBishopGreaterHeal(bool useScrool, Serial target, bool silence)
    {
      CastResultInfo info = new CastResultInfo();
      UOItem scrools = World.Player.Backpack.AllItems.FindType(Magery.SpellScrool[StandardSpell.GreaterHeal]);

      info.Spell = StandardSpell.GreaterHeal;

      if (useScrool && scrools.Exist)
      {
        Journal.Clear();
        scrools.Use();
        if (Journal.WaitForText(true, 250, "You activated KVM", "You can't cast this spell yet"))
        {
          if (!Journal.Contains(true, "You activated KVM"))
          {
            UOCharacter ch = new UOCharacter(target);
            double dmg = ch.MaxHits - ch.Hits;

            if (ch.ExistCust() && dmg < 25)
              info.Spell = StandardSpell.Heal;
          }
        }
      }

      info = Magery.Current.CastSpell(info.Spell, target, false, false, silence, false);
      return info;
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void CastBishopKVM(string target)
    {
      UOItem scrools = World.Player.Backpack.AllItems.FindType(Magery.SpellScrool[StandardSpell.GreaterHeal]);

      if (scrools.ExistCust())
      {
        World.Player.PrintMessage("[KVM]");

        TargetInfo t = Targeting.GetTarget(target);

        Journal.Clear();
        scrools.Use();
        if (!Journal.WaitForText(true, 250, "You can't cast this spell yet"))
        { 
          t.Character.PrintMessage("[KVM...]", Game.GetAlieColorByHits(t));
          Magery.Current.CastSpell(StandardSpell.GreaterHeal, t, false, false, true, false);
        }
      }
      else
        World.Player.PrintMessage("[Nemas scrooly...]", MessageType.Error);
    }

    //  res by byshop s healem


    //Serial: 0x402D4741  Name: "Scepter of Peaceful Mind"  Position: 0.0.0  Flags: 0x0000  Color: 0x04A6  Graphic: 0x26BC  Amount: 0  Layer: RightHand Container: 0x00316E6E
    //Serial: 0x4035C0AB  Name: "Angelic Robe"  Position: 44.102.0  Flags: 0x0000  Color: 0x04A6  Graphic: 0x1F03  Amount: 1  Layer: None Container: 0x4033DBE6

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void ScepterOfPeacefulMind()
    {
      UOItem scepter = World.Player.FindType(0x26BC, 0x0000);

      if (scepter.Exist && scepter.Layer == Layer.RightHand)
      {
        World.Player.PrintMessage("[One Step VIM..]");
        scepter.Use();
      }
      else
        World.Player.PrintMessage("[Nemas Scepter..]", MessageType.Error);

    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void AngelicRobe()
    {
      UOItem robe = World.Player.FindType(0x1F03, 0x04A6);

      if (robe.Exist && robe.Layer == Layer.OuterTorso)
      {
        World.Player.PrintMessage("[Angelic IJS..]");
        robe.Use();
      }
      else
        World.Player.PrintMessage("[Nemas Angelic..]", MessageType.Error);

    }

    //---------------------------------------------------------------------------------------------
  }
}
