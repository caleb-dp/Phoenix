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

namespace CalExtension.Abilities
{
  public static class Medic
  {
    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void UseKPZ(StandardSpell spell)
    {
      UseKPZ(spell, "hover");
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void UseKPZ(StandardSpell spell, string target)
    {
      UOItem kpz = World.Player.Backpack.AllItems.FindType(0x09B0, 0x0493);
      if (!kpz.Exist)
        kpz = World.Player.Backpack.AllItems.FindType(0x09B0, 0x0494);
      else
      {
        kpz.Use();
        Game.Wait(150);
      }

      if (kpz.Exist)
      {
        if (spell == StandardSpell.Heal || spell == StandardSpell.GreaterHeal)
          World.Player.PrintMessage("[KPZ << k sobe ]", Game.Val_LightGreen);
        else if (spell == StandardSpell.Protection)
          World.Player.PrintMessage("[KPZ >> k nemu]", Game.Val_LightGreen);
        else if (spell == StandardSpell.ReactiveArmor)
          World.Player.PrintMessage("[KPZ <> vymena]", Game.Val_LightGreen);
        else
        {
          Game.PrintMessage("KPZ invalid spell!", MessageType.Error);
          return;
        }

        TargetInfo info = Targeting.GetTarget(target);
        if (info.Success && info.TargetType == TargetType.Object && info.Character.Exist)
        {
          info.Object.PrintMessage("[> KPZ <]", Game.Val_LightGreen);
          CastResultInfo castInfo = Magery.Current.CastSpell(spell, info, false, false);
          if (castInfo.Success)
          {
            Healing.LastCharacter = info.Character;
            Game.Wait(250);
            info.Character.RequestStatus(250);
          }
        }
      }
      else
      {
        Game.PrintMessage("Nemas u sebe KPZ");
      }
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static UOCharacter BloodyBandRessQuick()
    {
      return BloodyBandRessQuick("onestepghost");
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static UOCharacter BloodyBandRessQuick(string target)
    {
      Game.PrintMessage("BloodyBandRessQuick - Ghosts: " + Healing.GhostCount());

      if (Healing.BloodyBandage.Exist && Healing.BloodyBandage.Amount >= 50)
      {
        TargetInfo tInfo = Targeting.GetTarget(target);
        if (tInfo.Success)
        {
          if (tInfo.Character.Distance <= 1)
          {
            tInfo.Character.Print(Game.Val_GreenBlue, "[Ressuji Full..]");
            Healing.BloodyBandage.Use();
            UO.WaitTargetObject(tInfo);
            Healing.LastCharacter = tInfo.Character;

            if (tInfo.Character.Distance <= 1)
              World.Player.PrintMessage("[Ress done " + Healing.BloodyBandage.Amount + "..]");

            Game.Wait(50);

            if (tInfo.Character.RequestStatus(125) && tInfo.Character.Hits > 0)
              tInfo.Character.Print(Game.Val_GreenBlue, "[Ress OK " + tInfo.Character.Hits + "hp]");
          }
          else
            tInfo.Character.Print(Game.Val_LightPurple, "[Daleko " + tInfo.Character.Distance + "...]");
        }
        else
          World.Player.PrintMessage("[Zadny duch..]", MessageType.Warning);
      }
      else
      {
        Game.PrintMessage("Nemas dost kvravych BAND! " + Healing.BloodyBandage.Amount);
        return Healing.BandRessQuick();
      }

      return Healing.LastCharacter;
    }

    //---------------------------------------------------------------------------------------------
  }
}
