using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;
using System.Threading;
using Caleb.Library.CAL.Business;
using Caleb.Library;
using CalExtension.Skills;

namespace CalExtension.UOExtensions
{
  //---------------------------------------------------------------------------------------------

  public class GoldenMedailon
  {
    //---------------------------------------------------------------------------------------------

    public static GoldenMedailon Current;
    
    public GoldenMedailon()
    {
      if (Current == null)
        Current = this;
    }

    //---------------------------------------------------------------------------------------------

    List<ItemOriginalLayer> armor = new List<ItemOriginalLayer>();
    private DateTime? lastBersekerStart = null;
    //---------------------------------------------------------------------------------------------

    public static UOItem Medailon
    {
      get
      {
        return World.Player.FindType(0x0FC7, 0x044C);
      }
    }

    //---------------------------------------------------------------------------------------------

    //[Executable]
    //public void UseGoldenMedailon()
    //{
    //  UseGoldenMedailon(3);
    //}

    //---------------------------------------------------------------------------------------------

    [Executable]
    public void UseGoldenMedailon()
    {
      List<ItemOriginalLayer> currentArmor = new List<ItemOriginalLayer>();

      if (World.Player.Layers[Layer.InnerTorso].Exist)
        currentArmor.Add(new ItemOriginalLayer() { Item = World.Player.Layers[Layer.InnerTorso], OriginalLater = Layer.InnerTorso });

      if (World.Player.Layers[Layer.InnerLegs].Exist)
        currentArmor.Add(new ItemOriginalLayer() { Item = World.Player.Layers[Layer.InnerLegs], OriginalLater = Layer.InnerLegs });

      if (World.Player.Layers[Layer.Hat].Exist)
        currentArmor.Add(new ItemOriginalLayer() { Item = World.Player.Layers[Layer.Hat], OriginalLater = Layer.Hat });

      if (World.Player.Layers[Layer.Pants].Exist)
        currentArmor.Add(new ItemOriginalLayer() { Item = World.Player.Layers[Layer.Pants], OriginalLater = Layer.Pants });

      if (World.Player.Layers[Layer.Waist].Exist)
        currentArmor.Add(new ItemOriginalLayer() { Item = World.Player.Layers[Layer.Waist], OriginalLater = Layer.Waist });

      if (World.Player.Layers[Layer.Shirt].Exist)
        currentArmor.Add(new ItemOriginalLayer() { Item = World.Player.Layers[Layer.Shirt], OriginalLater = Layer.Shirt });

      if (World.Player.Layers[Layer.Arms].Exist)
        currentArmor.Add(new ItemOriginalLayer() { Item = World.Player.Layers[Layer.Arms], OriginalLater = Layer.Arms });

      if (World.Player.Layers[Layer.Gloves].Exist)
        currentArmor.Add(new ItemOriginalLayer() { Item = World.Player.Layers[Layer.Gloves], OriginalLater = Layer.InnerTorso });

      if (World.Player.Layers[Layer.Neck].Exist)
        currentArmor.Add(new ItemOriginalLayer() { Item = World.Player.Layers[Layer.Neck], OriginalLater = Layer.Neck });

      if (World.Player.Layers[Layer.Shoes].Exist)
        currentArmor.Add(new ItemOriginalLayer() { Item = World.Player.Layers[Layer.Shoes], OriginalLater = Layer.Shoes });

      if (World.Player.Layers[Layer.OuterTorso].Exist)
        currentArmor.Add(new ItemOriginalLayer() { Item = World.Player.Layers[Layer.OuterTorso], OriginalLater = Layer.OuterTorso });

      bool rearmor = false;

      if (Medailon.Exist)
      {
        Journal.Clear();
        Medailon.Use();
        bool bersekerSwitch = false;
        bool end = false;


        if (Journal.WaitForText(true, 500, "Citis se mnohem silnejsi!", "Berserk zrusen", "Jsi stale prilis unaven od minuleho pouziti"))
        {
          if (Journal.Contains(true, "Citis se mnohem silnejsi!", "Berserk zrusen"))
          {
            bersekerSwitch = true;
            end = Journal.Contains(true, "Berserk zrusen");
          }

          if (bersekerSwitch)
          {
            World.Player.PrintMessage("[Berseker " + (end ? "OFF" : "ON") + "]");
            if (!end)
            {
              lastBersekerStart = DateTime.Now;
              armor = currentArmor;

              for (int i = 0; i < armor.Count; i++)//each (//UOItem ar in armor)
              {
                if (armor[i].OriginalLater == Layer.OuterTorso ||
                  armor[i].OriginalLater == Layer.Shoes ||
                  armor[i].OriginalLater == Layer.Waist ||
                   armor[i].OriginalLater == Layer.Shirt ||
                    armor[i].OriginalLater == Layer.Pants)
                {

                  UOItem ar = armor[i].Item;
                  ar.Use();
                  if (Journal.WaitForText(true, 450, "The item should be equipped to use"))
                  {
                    Game.Wait(400);
                  }
                }
              }

            }
            else
            {
//              Game.Wait(350);
              //for (int i = 0; i < Math.Min(armor.Count, maxRearmorOnEnd); i++)//each (//UOItem ar in armor)
              //{
              //  UOItem ar = armor[i].Item;
              //  ar.Use();
              //  if (Journal.WaitForText(true, 450, "The item should be equipped to use"))
              //  {
              //    Game.Wait(400);
              //    //The item should be equipped to use
              //    //neco ze nejde use
              //    //break;
              //  }
              //}
              //armor.Clear();
            }
          }
          else if (lastBersekerStart != null)
          {
            rearmor = true;
            TimeSpan span = DateTime.Now - lastBersekerStart.Value;
            World.Player.PrintMessage(String.Format("[Last {0:0#}:{1:0#}:{2:0#}]", span.Hours, span.Minutes, span.Seconds));
          }

          //Berserk zrusen
          //Jsi stale prilis unaven od minuleho pouziti!
        }
        else
          rearmor = true;
      }
      else
      {
        rearmor = true;
        Game.PrintMessage("[Nemas medailon..]", MessageType.Warning);

        Game.PrintMessage("Armor SAVE " + currentArmor.Count);

        if (currentArmor.Count > this.armor.Count)
          this.armor = currentArmor;
      }

      if (rearmor)
      {
        List<UOItem> toRearm = new List<UOItem>();

        for (int i = 0; i < armor.Count; i++)// (ItemOriginalLayer iol in armor)
        {
          ItemOriginalLayer iol = armor[i];

          if (iol.Item.Layer == Layer.None)
          {
            toRearm.Add(iol.Item);

          }
        }

        Game.PrintMessage("Rearmor " + toRearm.Count);

        for (int i = 0; i < toRearm.Count; i++)
        {
          UOItem item = toRearm[i];
          item.Use();
          Game.Wait(360 + i * 35);
        }
      }
    }


    //---------------------------------------------------------------------------------------------
  }

  public class ItemOriginalLayer
  {
    public UOItem Item;
    public Layer OriginalLater;
  }

  //Serial: 0x401B2934  Name: "Golden Medallion (3 pouziti)"  Position: 139.97.0  Flags: 0x0000  Color: 0x044C  Graphic: 0x0FC7  Amount: 1  Layer: None  Container: 0x403174BC


  //    Serial: 0x401A9932  Name: "robe"  Position: 0.0.0  Flags: 0x0000  Color: 0x0027  Graphic: 0x1F03  Amount: 0  Layer: OuterTorso Container: 0x00332ACF

  //Serial: 0x4008A6B7  Position: 0.0.0  Flags: 0x0000  Color: 0x0021  Graphic: 0x1711  Amount: 0  Layer: Shoes Container: 0x00332ACF

  //Serial: 0x401084CC  Position: 0.0.0  Flags: 0x0000  Color: 0x0084  Graphic: 0x13C3  Amount: 0  Layer: Pants Container: 0x00332ACF

  //Serial: 0x4029BD56  Name: "half apron"  Position: 0.0.0  Flags: 0x0000  Color: 0x0021  Graphic: 0x153B  Amount: 0  Layer: Waist Container: 0x00332ACF

  //Serial: 0x400914EE  Position: 0.0.0  Flags: 0x0000  Color: 0x0084  Graphic: 0x13C4  Amount: 0  Layer: InnerTorso Container: 0x00332ACF

  //Serial: 0x402501DD  Position: 0.0.0  Flags: 0x0000  Color: 0x0084  Graphic: 0x13EF  Amount: 0  Layer: Arms Container: 0x00332ACF

  //Serial: 0x40377F89  Position: 0.0.0  Flags: 0x0000  Color: 0x0084  Graphic: 0x13EB  Amount: 0  Layer: Gloves Container: 0x00332ACF

  //Serial: 0x4036F78A  Name: "Warrior's Coif"  Position: 0.0.0  Flags: 0x0000  Color: 0x0084  Graphic: 0x13BB  Amount: 0  Layer: Hat Container: 0x00332ACF

  //Serial: 0x4031B398  Position: 0.0.0  Flags: 0x0000  Color: 0x0084  Graphic: 0x1413  Amount: 0  Layer: Neck Container: 0x00332ACF

  //Serial: 0x403648F7  Position: 44.115.0  Flags: 0x0000  Color: 0x0000  Graphic: 0x1411  Amount: 1  Layer: InnerLegs Container: 0x00332ACF
}
