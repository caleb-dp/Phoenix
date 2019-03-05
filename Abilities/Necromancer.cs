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
  public static class Necromancer
  {
    public static UOItemType VendetaCharged { get { return new UOItemType() { Graphic = 0x27AB, Color = 0x0B4F, Name = "VendetaCharged" }; } }
    public static UOItemType Vendeta { get { return new UOItemType() { Graphic = 0x27AB, Color = 0x0B2D, Name = "Vendeta" }; } }
    public static UOItemType DarkSkull { get { return new UOItemType() { Graphic = 0x1F0B, Color = 0x0485, Name = "DarkSkull" }; } }
    public static UOItemType DarkOclock { get { return new UOItemType() { Graphic = 0x00D2, Color = 0x0966, Name = "DarkOclock" }; } }
    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void EquipVendeta()
    {
      UOItem itmCharged = World.Player.FindType(VendetaCharged);
      UOItem itm = World.Player.FindType(Vendeta);
      UOItem deadlyKad = World.Player.FindType(Potion.Poison.Qualities[PotionQuality.Deadly].KadGraphic, Potion.Poison.Qualities[PotionQuality.Deadly].KadColor);


      if (itmCharged.Exist)
      {
        if (itmCharged.Layer == Layer.None)
          Game.UseTypeCust(itmCharged.Graphic, itmCharged.Color, "none", "", "[Vendeta...]");
        else
          World.Player.PrintMessage("[Vendeta...]");

        return;
      }

      if (!itm.Exist)
      {
        World.Player.PrintMessage("[Nemas vendetu..]", MessageType.Warning);
        return;
      }

      if (!deadlyKad.Exist)
      {
        World.Player.PrintMessage("[Nemas deadly..]", MessageType.Warning);
        return;
      }

      World.Player.PrintMessage("[Vendeta]");
      UO.WaitTargetObject(deadlyKad);
      UO.UseObject(itm);
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void UseDarkSkull()
    {
      UOItem itm = World.Player.FindType(DarkSkull);
      UOItem hat = World.Player.Layers[Layer.Hat];
      UOItem mount = World.Player.Layers[Layer.Mount];
      bool mountDie = false;// mount.Exist && mount.Color == DarkOclock.Color; - dakroclock neda manu asi zadny sum ;]

      if (itm.Exist)
      {
        if (World.Player.MaxMana - World.Player.Mana > 10)
        {
          if (World.Player.Hits >= 50)// || mountDie)
          {
            if (itm.Layer == Layer.Hat)
            {
              if (!itm.Move(1, World.Player.Backpack))
              {
                World.Player.PrintMessage("[ err move skull ]", MessageType.Error);
                return;
              }
              else
                Game.Wait(200);
            }

            bool dismount = false;
            if (mount.Exist && !mountDie)
            {
              if (Mount.Current.UseMount())
              {
                if (World.Player.Layers[Layer.Mount].ExistCust())
                {
                  World.Player.PrintMessage("[ err unmout ]", MessageType.Error);
                  return;
                }
                else
                  dismount = true;
              }
            }

            itm.Use();
            Game.Wait(200);
            if (hat.Exist && hat.Serial != itm.Serial)
            {
              hat.Use();
              Game.Wait(200);
            }

            if (dismount)
            {
              Mount.Current.UseMount();
            }
          }
          else
            World.Player.PrintMessage("[ malo HP ]", MessageType.Warning);
        }
        else
          World.Player.PrintMessage("[ full mana ]", MessageType.Info);
      }
      else
        World.Player.PrintMessage("[ neni darkskull ]", MessageType.Warning);
    }

    //---------------------------------------------------------------------------------------------
  }
}
