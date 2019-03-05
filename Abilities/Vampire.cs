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
  public static class Vampire
  {
    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void SnezBandu()
    {
      UOItem banda = new UOItem(Serial.Invalid);

      banda = Healing.BloodyBandage;

      if (banda.Serial.IsValid)
      {
        if (World.Player.Hits >= World.Player.MaxHits)
        {
          World.Player.PrintMessage("[Plne HP " + World.Player.Hits + "/" + World.Player.MaxHits + "hp..]");
          return;
        }

        int startHits = World.Player.Hits;
        banda.Use();
        Game.Wait(true);
        int endHits = World.Player.Hits;

        World.Player.PrintMessage("[Krev banda " + (endHits - startHits) + "hp..]", endHits < World.Player.MaxHits ? MessageType.Warning : MessageType.Info);
      }
      else
      {
        World.Player.PrintMessage("[Neni Krev banda..]");
      }
    }

    //---------------------------------------------------------------------------------------------
  }
}
