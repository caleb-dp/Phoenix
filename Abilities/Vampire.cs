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
  public static class Vampire
  {
    //0x0B7D  Graphic: 0x1088 - Talisman Tonnyho Kalnera
    //Talisman slabe zazaril...
    //---------------------------------------------------------------------------------------------
    //You cannot eat any more!
    public static UOItemType TalismanTonnyhoKalnera { get { return new UOItemType() { Graphic = 0x1088, Color = 0x0B7D }; } }

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
        Journal.Clear();
        if (Journal.WaitForText(true, 500, "You cannot eat any more"))
        {
          Targeting.ResetTarget();
          UO.Press(System.Windows.Forms.Keys.Escape);
          Game.Wait(100);

          UOItem neck = World.Player.Layers[Layer.Neck];
          UOItem talisman = World.Player.Backpack.Items.FindType(0x1088, 0x0B7D);

          if (neck.IsItemType(TalismanTonnyhoKalnera))
          {
            talisman = neck;
            neck.Move(1, World.Player.Backpack.Serial);
            Game.Wait(250);
          }
        
          if (talisman.Exist)
          {
            talisman.Use();
            if (Journal.WaitForText(true, 500, "....."))
            {
              banda.Use();

            }
          }
        }
        
        //Game.Wait(true);
        int endHits = World.Player.Hits;

        World.Player.PrintMessage("[Krev banda " + (endHits - startHits) + "hp..]", endHits < World.Player.MaxHits ? MessageType.Warning : MessageType.Info);
      }
      else
      {
        World.Player.PrintMessage("[Neni Krev banda..]");
      }
    }

    //---------------------------------------------------------------------------------------------

//    19:11 System: Targeting Cancelled
//19:12 You see: a Greezi Artefakt
//19:12 : [Use]
//19:12 Portia Labiata: [Odhidnut! ]
//19:12 System: You have been revealed
//19:12 System: Vitej doma.
//19:12 : [Use]
//19:12 Portia Labiata: *You used Rewind*
//19:12 System: Za 5 sekund budes vracen v case!
//19:12 Portia Labiata: *You was ported back in time*
//19:12 Portia Labiata: Artefakt jeste neni pripraven k pouziti!
  }
}
