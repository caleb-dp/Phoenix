using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;
using Caleb.Library;
using Caleb.Library.CAL.Business;
using System.Collections;

namespace CalExtension.UOExtensions
{
  public class ItemLibrary
  {
    //---------------------------------------------------------------------------------------------
    #region Items

    public static UOItemType DeathsServantBardiche { get { return new UOItemType() { Graphic = 0x0F4D, Color = 0x0B1B, Name = "Death's Servant Bardiche" }; } }
    public static UOItemType MytherilScimitar { get { return new UOItemType() { Graphic = 0x13B5, Color = 0x052D, Name = "Mytheril Scimitar" }; } }
    public static UOItemType OakwoodQuarterStaff { get { return new UOItemType() { Graphic = 0x0E89, Color = 0x05A6, Name = "Oak-wood Quarter Staff" }; } }
    public static UOItemType KnightCloakGold { get { return new UOItemType() { Graphic = 0x1515, Color = 0x0BAC, Name = "Knight Cloak - Gold" }; } }
    public static UOItemType KnightCloakBlack { get { return new UOItemType() { Graphic = 0x1515, Color = 0x0BB8, Name = "Knight Cloak - Black" }; } }
    public static UOItemType GoldenscaleShield { get { return new UOItemType() { Graphic = 0x1B72, Color = 0x049B, Name = "Goldenscale Shield" }; } }

    #endregion
    //---------------------------------------------------------------------------------------------

    public static UOItemTypeCollection Items
    {
      get 
      {
        UOItemTypeCollection items = new UOItemTypeCollection();
        items.Add(new UOItemType() { Graphic = 0x0190, Color = 0x0000, Name = "Human Model" });
        return items;
      }
    }

    //---------------------------------------------------------------------------------------------

    public static UOItemType GetMonsterConflictSummon(UOCharacter ch)
    {
      UOItemTypeCollection items = new UOItemTypeCollection();
      items.Add(new UOItemType() { Graphic = 0x000E, Color = 0x0000, Name = "earthelemental" });//eartelemental
      items.Add(new UOItemType() { Graphic = 0x003A, Color = 0x0000, Name = "wisp" });//guardian spirit
      items.Add(new UOItemType() { Graphic = 0x0032, Color = 0x0000, Name = "skeleton" });
      items.Add(new UOItemType() { Graphic = 0x0018, Color = 0x0000, Name = "liche", Names = new List<string>() { "liche", "summoner" } });
      items.Add(new UOItemType() { Graphic = 0x0003, Color = 0x0000, Name = "zombie" });
      items.Add(new UOItemType() { Graphic = 0x000D, Color = 0x0000, Name = "airelemental" });
      //  summi.Add(new UOItemType() { Graphic = 0x000D, Color = 0x0000, Name = "air elemental" });//airelemental
      //summi.Add(new UOItemType() { Graphic = 0x0032, Color = 0x0000, Name = "Skeleton" });//Skeleton
      //summi.Add(new UOItemType() { Graphic = 0x0018, Color = 0x0000, Name = "Liche" });//Liche
      //summi.Add(new UOItemType() { Graphic = 0x0003, Color = 0x0000, Name = "Zombie" });//Zombie

      return items.FindItem(ch.Model, ch.Color);
    }

    //---------------------------------------------------------------------------------------------

    public static bool IsMonsterConflictSummon(UOCharacter ch)
    {
      return GetMonsterConflictSummon(ch) != null;
    }


    //---------------------------------------------------------------------------------------------

    public static bool IsMostCommonPlayerSummon(UOCharacter ch)
    {
      return ch.Model == 0x000D && ch.Color == 0x0B77 ||//death vortex 
                      ch.Model == 0x0015 && ch.Color == 0x0757 ||//giant viper
                      ch.Model == 0x00D4 && ch.Color == 0x0712 ||//grizlly
                      ch.Model == 0x003C && ch.Color == 0x0751 ||//dragon
                      ch.Model == 0x0039 && ch.Color == 0x0835 ||//skeleton w
                      ch.Model == 0x0090 && ch.Color == 0x4001 ||//ethernalspirit
                       ch.Model == 0x003A && ch.Color == 0x0B87 || //angel spirit
                       ch.Model == 0x003A && ch.Color == 0x084C || //awaken spirit
      ch.Model == 0x00D3 && ch.Color == 0x0712 || //brown bear
      ch.Model == 0x00E1 && ch.Color == 0x0712; //wolf
    }

    //---------------------------------------------------------------------------------------------

    public static List<IUOItemType> PlayerSummons
    {
      get
      {
        List<IUOItemType> summi = new List<IUOItemType>();
     
        
        //summi.Add(new UOItemType() { Graphic = 0x0005, Color = 0x0BA4, Name = "phoenix" });//phoenix
        summi.Add(new UOItemType() { Graphic = 0x00D4, Color = 0x0712, Name = "grizzly bear" });//grizzly
        summi.Add(new UOItemType() { Graphic = 0x00D3, Color = 0x0712, Name = "brown bear" });//brown bear
        summi.Add(new UOItemType() { Graphic = 0x00E1, Color = 0x0712, Name = "wolf" });//wolf
        summi.Add(new UOItemType() { Graphic = 0x0005, Color = 0x0712, Name = "hawk" });//orel
        summi.Add(new UOItemType() { Graphic = 0x0015, Color = 0x0757, Name = "giant viper" });//giant viper
        summi.Add(new UOItemType() { Graphic = 0x0030, Color = 0x0751, Name = "giant scorpion" });//giant scorpion
        summi.Add(new UOItemType() { Graphic = 0x001C, Color = 0x0751, Name = "giant spider" });//giant spider
        summi.Add(new UOItemType() { Graphic = 0x00CC, Color = 0x0000, Name = "horse" });//giant spider
        summi.Add(new UOItemType() { Graphic = 0x003C, Color = 0x0751, Name = "magic drake" });//dragon
        summi.Add(new UOItemType() { Graphic = 0x003A, Color = 0x0B87, Name = "angel spirit" });//angel spirit
        summi.Add(new UOItemType() { Graphic = 0x003A, Color = 0x0000, Name = "guardian spirit" });//guardian spirit
        summi.Add(new UOItemType() { Graphic = 0x003A, Color = 0x084C, Name = "awaken spirit" });//awaken spirit
        summi.Add(new UOItemType() { Graphic = 0x003A, Color = 0x0849, Name = "temple spirit" });//temple spirit
        summi.Add(new UOItemType() { Graphic = 0x0018, Color = 0x0835, Name = "liche" });//liche
        summi.Add(new UOItemType() { Graphic = 0x00EE, Color = 0x01BB, Name = "rat" });//krysa
        summi.Add(new UOItemType() { Graphic = 0x00D2, Color = 0x0966, Name = "dark oclock" });//dark oclock
                                                                                               //Serial: 0x0030FC84  Name: "bne4i8C5"  Position: 5279.1116.0  Flags: 0x0040  Color: 0x0835  Model: 0x0038  Renamable: True  Notoriety: Murderer  HP: 115/115, Distance: 4

        summi.Add(new UOItemType() { Graphic = 0x0038, Color = 0x0835, Name = "skeleton (axe)" });//skeleton warrior
        summi.Add(new UOItemType() { Graphic = 0x0039, Color = 0x0835, Name = "skeleton warrior" });//skeleton warrior
        summi.Add(new UOItemType() { Graphic = 0x001A, Color = 0x0835, Name = "wraith" });//wraith TOTO spatne
        summi.Add(new UOItemType() { Graphic = 0x0003, Color = 0x0835, Name = "mummy" });//mummy TOTO spatne
        summi.Add(new UOItemType() { Graphic = 0x0003, Color = 0x058D, Name = "ghoul" });//ghoul TOTO spatne
        summi.Add(new UOItemType() { Graphic = 0x0003, Color = 0x049C, Name = "magic golem" });//ghoul TOTO spatne

        summi.Add(new UOItemType() { Graphic = 0x000D, Color = 0x0B77, Name = "death vortex" });//death vortex
        summi.Add(new UOItemType() { Graphic = 0x0018, Color = 0x0837, Name = "demi liche" });//demiliche
        summi.Add(new UOItemType() { Graphic = 0x0018, Color = 0x058D, Name = "demi liche" });//demiliche

        summi.Add(new UOItemType() { Graphic = 0x000D, Color = 0x0000, Name = "air elemental" });//airelemental
        summi.Add(new UOItemType() { Graphic = 0x000E, Color = 0x0000, Name = "earth elemental" });//eartelemental
        summi.Add(new UOItemType() { Graphic = 0x0090, Color = 0x4001, Name = "ethernal spirit" });//
        summi.Add(new UOItemType() { Graphic = 0x0027, Color = 0x0966, Name = "Dark Vampire" });//Dark Vampire

        //Necro summy
        summi.Add(new UOItemType() { Graphic = 0x0032, Color = 0x0000, Name = "Skeleton" });//Skeleton
        summi.Add(new UOItemType() { Graphic = 0x0018, Color = 0x0000, Name = "Liche" });//Liche
        summi.Add(new UOItemType() { Graphic = 0x0003, Color = 0x0000, Name = "Zombie" });//Zombie

        return summi;
      }
    }

    //---------------------------------------------------------------------------------------------

    public static List<IUOItemType> AttackKlamaci
    {
      get
      {
        List<IUOItemType> summi = new List<IUOItemType>();
        summi.Add(new UOItemTypeBase(0x00D6, 0x0972));// Leopard
        summi.Add(new UOItemTypeBase(0x00D5, 0x08FD));// Polar bear
        summi.Add(new UOItemTypeBase(0x00D6, 0x0901));// Panther
        summi.Add(new UOItemTypeBase(0x00D6, 0x0000));// Cougar
        summi.Add(new UOItemTypeBase(0x00E1, 0x0000));// Wolf
        summi.Add(new UOItemTypeBase(0x00D4, 0x0000));// Grilzy
        summi.Add(new UOItemTypeBase(0x0005, 0x0847));// Skyhawk

        return summi;
      }
    }


    //      }

    //---------------------------------------------------------------------------------------------

    public static UOItemTypeCollection PlayerKlamaci
    {
      get
      {
        UOItemTypeCollection items = new UOItemTypeCollection();
        items.Add(new UOItemType() { Graphic = 0x00DF, Color = 0x0388, Name = "Lamb" });
        items.Add(new UOItemType() { Graphic = 0x00EE, Color = 0x01BB, Name = "Sewer Rat" });
        items.Add(new UOItemType() { Graphic = 0x00F5, Color = 0x0000, Name = "Squirrel" });
        items.Add(new UOItemType() { Graphic = 0x0034, Color = 0x07DB, Name = "Snake" });
        items.Add(new UOItemType() { Graphic = 0x00D7, Color = 0x0000, Name = "Giant Rat" });
        items.Add(new UOItemType() { Graphic = 0x00CD, Color = 0x0900, Name = "Rabbit" });
        items.Add(new UOItemType() { Graphic = 0x00D1, Color = 0x0000, Name = "Goat" });
        items.Add(new UOItemType() { Graphic = 0x00CD, Color = 0x01BB, Name = "Jackrabbit" });
        items.Add(new UOItemType() { Graphic = 0x0051, Color = 0x0000, Name = "Bull Frog" });
        items.Add(new UOItemType() { Graphic = 0x00E7, Color = 0x0000, Name = "Cow" });
        items.Add(new UOItemType() { Graphic = 0x00CA, Color = 0x0000, Name = "Alligator" });
        items.Add(new UOItemType() { Graphic = 0x00D8, Color = 0x0000, Name = "Cow" });
        items.Add(new UOItemType() { Graphic = 0x00CF, Color = 0x08FD, Name = "Sheep" });
        items.Add(new UOItemType() { Graphic = 0x00ED, Color = 0x0000, Name = "Hind" });
        items.Add(new UOItemType() { Graphic = 0x0122, Color = 0x0000, Name = "Boar" });
        items.Add(new UOItemType() { Graphic = 0x001D, Color = 0x0000, Name = "Gorilla" });
        items.Add(new UOItemType() { Graphic = 0x00E1, Color = 0x0000, Name = "Timber Wolf" });
        items.Add(new UOItemType() { Graphic = 0x00D0, Color = 0x0000, Name = "Chicken" });
        items.Add(new UOItemType() { Graphic = 0x00D3, Color = 0x01BB, Name = "Brown Bear" });
        items.Add(new UOItemType() { Graphic = 0x00E8, Color = 0x01BB, Name = "Bull" });
        items.Add(new UOItemType() { Graphic = 0x00C9, Color = 0x0000, Name = "Cat" });
        
        return items;
      }
    }

    //---------------------------------------------------------------------------------------------

    public static UOItemTypeCollection MountTypes
    {
      get
      {
        UOItemTypeCollection items = new UOItemTypeCollection();
        items.Add(new UOItemType() { Graphic = 0x00DF, Color = 0x0000, Name = "Lama" });
        items.Add(new UOItemType() { Graphic = 0x00DC, Color = 0x0000, Name = "Lama" });
        
        items.Add(new UOItemType() { Graphic = 0x00DA, Color = 0x0000, Name = "Zost" });
        items.Add(new UOItemType() { Graphic = 0x00E2, Color = 0x0000, Name = "Must" });
        items.Add(new UOItemType() { Graphic = 0x00CC, Color = 0x0000, Name = "Kun" });
        items.Add(new UOItemType() { Graphic = 0x00E4, Color = 0x0000, Name = "Kun 2" });

        items.Add(new UOItemType() { Graphic = 0x00D2, Color = 0x0000, Name = "Ock" });
        items.Add(new UOItemType() { Graphic = 0x00DB, Color = 0x0000, Name = "Ock" });

        return items;
      }
    }

    //---------------------------------------------------------------------------------------------

    public static UOItemTypeCollection ShrinkMountTypes
    {
      get
      {
        UOItemTypeCollection items = new UOItemTypeCollection();
        items.Add(new UOItemType() { Graphic = 0x211F, Color = 0x0000, Name = "???" });
        items.Add(new UOItemType() { Graphic = 0x2121, Color = 0x0000, Name = "???" });
        items.Add(new UOItemType() { Graphic = 0x2124, Color = 0x0000, Name = "???" });
        items.Add(new UOItemType() { Graphic = 0x20F6, Color = 0x0000, Name = "???" });

        items.Add(new UOItemType() { Graphic = 0x2135, Color = 0x0000, Name = "???" });
        items.Add(new UOItemType() { Graphic = 0x2136, Color = 0x0000, Name = "???" });
        items.Add(new UOItemType() { Graphic = 0x2137, Color = 0x0000, Name = "???" });

        return items;
      }
    }

    //---------------------------------------------------------------------------------------------

    public static UOItemTypeCollection ShrinkPackTypes
    {
      get
      {
        UOItemTypeCollection items = new UOItemTypeCollection();
        items.Add(new UOItemType() { Graphic = 0x2127, Color = 0x0000, Name = "Pack Lama" });
        items.Add(new UOItemType() { Graphic = 0x2126, Color = 0x0000, Name = "Pack Kun" });
        return items;
      }
    }



    //---------------------------------------------------------------------------------------------

    public static UOItemTypeCollection ShrinkKlamaci
    {
      get
      {
        UOItemTypeCollection items = new UOItemTypeCollection();
        items.Add(new UOItemType() { Graphic = 0x20E6, Color = 0x0000, Name = "Lamb" });
        items.Add(new UOItemType() { Graphic = 0x20EB, Color = 0x0000, Name = "Cow" });
        items.Add(new UOItemType() { Graphic = 0x20EB, Color = 0x0000, Name = "Sheep" });
        items.Add(new UOItemType() { Graphic = 0x2108, Color = 0x0000, Name = "Goat" });

        items.Add(new UOItemType() { Graphic = 0x20D4, Color = 0x0000, Name = "Hind" });
        items.Add(new UOItemType() { Graphic = 0x20D4, Color = 0x0000, Name = "Hart" });
        items.Add(new UOItemType() { Graphic = 0x211B, Color = 0x0000, Name = "Cat" });
        items.Add(new UOItemType() { Graphic = 0x20F5, Color = 0x0000, Name = "Gorrila" });
        items.Add(new UOItemType() { Graphic = 0x2103, Color = 0x0000, Name = "Cow" });
        items.Add(new UOItemType() { Graphic = 0x20D1, Color = 0x0000, Name = "Chicken" });
        items.Add(new UOItemType() { Graphic = 0x20EF, Color = 0x0000, Name = "Bull" });
        items.Add(new UOItemType() { Graphic = 0x20CF, Color = 0x01BB, Name = "Brown Bear" });
        items.Add(new UOItemType() { Graphic = 0x2101, Color = 0x0000, Name = "Pig" });
        items.Add(new UOItemType() { Graphic = 0x2125, Color = 0x090C, Name = "Rabbit" });
        items.Add(new UOItemType() { Graphic = 0x20E2, Color = 0x0000, Name = "Rabbit" });
        items.Add(new UOItemType() { Graphic = 0x211C, Color = 0x0000, Name = "Dog" });
        items.Add(new UOItemType() { Graphic = 0x2D97, Color = 0x0000, Name = "Squirrel" });
        items.Add(new UOItemType() { Graphic = 0x20EA, Color = 0x03B2, Name = "Gray Wolf" });
        items.Add(new UOItemType() { Graphic = 0x2130, Color = 0x0000, Name = "Bull Frog" });
        items.Add(new UOItemType() { Graphic = 0x2119, Color = 0x0901, Name = "Panther" });
        items.Add(new UOItemType() { Graphic = 0x2119, Color = 0x0000, Name = "Cougar" });
        items.Add(new UOItemType() { Graphic = 0x2119, Color = 0x0972, Name = "Leopard" });
        items.Add(new UOItemType() { Graphic = 0x211E, Color = 0x0000, Name = "Grizzly bear" });
        items.Add(new UOItemType() { Graphic = 0x211D, Color = 0x0847, Name = "Skyhawk" });

        items.Add(new UOItemType() { Graphic = 0x2108, Color = 0x0481, Name = "Mountain Goat" });
        items.Add(new UOItemType() { Graphic = 0x211B, Color = 0x0001, Name = "Black Cat" });
        items.Add(new UOItemType() { Graphic = 0x2101, Color = 0x0000, Name = "Boar" });
        items.Add(new UOItemType() { Graphic = 0x20FC, Color = 0x0000, Name = "Snake" });
        items.Add(new UOItemType() { Graphic = 0x211D, Color = 0x0000, Name = "Eagle" });
        items.Add(new UOItemType() { Graphic = 0x20E1, Color = 0x0000, Name = "Polar Bear" });
        items.Add(new UOItemType() { Graphic = 0x20D0, Color = 0x0000, Name = "Giant Rat" });
        items.Add(new UOItemType() { Graphic = 0x2131, Color = 0x0000, Name = "Alligator" });
        items.Add(new UOItemType() { Graphic = 0x20E2, Color = 0x01BB, Name = "Jackrabbit" });
        items.Add(new UOItemType() { Graphic = 0x20EA, Color = 0x01BB, Name = "Timber Wolf" });

        items.Add(new UOItemType() { Graphic = 0x2125, Color = 0x0000, Name = "Rabbit" });





        return items;
      }
    }

    public static Graphic[] ShrinkKlamaciArray
    {
      get
      {
        Graphic[] lootItem = new Graphic[39];
        lootItem[0] = 0x2121;
        lootItem[1] = 0x2120;
        lootItem[2] = 0x211F;
        lootItem[3] = 0x2124;
        lootItem[4] = 0x20F6;
        lootItem[5] = 0x2137;
        lootItem[6] = 0x2126;
        lootItem[7] = 0x2127;
        lootItem[8] = 0x2135;
        lootItem[9] = 0x20E1; // polar bear
        lootItem[10] = 0x20F7; // panther, walrus
        lootItem[11] = 0x2119; // leopard/snow leopard1
        lootItem[12] = 0x2102; // leopard/snow leopard2
        lootItem[13] = 0x211D; // eagle
        lootItem[14] = 0x2136; // zostrich
        lootItem[15] = 0x2123; //  Rat
        lootItem[16] = 0x20D1; // Chicken
        lootItem[17] = 0x211B; // Cat
        lootItem[18] = 0x2D97; // Veverka
        lootItem[19] = 0x20EE; // Papousek
        lootItem[20] = 0x20D4; // Hind
        lootItem[21] = 0x2130; // Frog
        lootItem[22] = 0x20EA; // Timber wolf
        lootItem[23] = 0x20FC; // Snake
        lootItem[24] = 0x20CF; // Brown bear
        lootItem[25] = 0x2118; // Black bear
        lootItem[26] = 0x2108; // Goat
        lootItem[27] = 0x2101; // Pig
        lootItem[28] = 0x211C; // Dog
        lootItem[29] = 0x20D0; // Giant Rat
        lootItem[30] = 0x20F5; // Gorila
        lootItem[31] = 0x211E; // Grizzli bear
        lootItem[32] = 0x20E6; // Lamb
        lootItem[33] = 0x20EF; // Bull
        lootItem[34] = 0x2103; // Cow
        lootItem[35] = 0x20E2; // Jackrabit
        lootItem[36] = 0x20EB; // Sheep
        lootItem[37] = 0x2131; // Aligator
        lootItem[38] = 0x2125;//rabid

        return lootItem;
      }
    }

    //---------------------------------------------------------------------------------------------

    public static bool IsGraphicFromList(Graphic graphic, IEnumerable list)
    {
      IEnumerator enumerator = list.GetEnumerator();
      while (enumerator.MoveNext())
      {
        Graphic current = (Graphic)enumerator.Current;
        if (graphic == current)
          return true;
      }
      return false;
    }

    //---------------------------------------------------------------------------------------------

    public static UOItemTypeCollection RingmailArmor
    {
      get
      {
        UOItemTypeCollection items = new UOItemTypeCollection();
        items.Add(new UOItemType() { Graphic = 0x13EC, Color = 0x0000, Name = "Ringmail Tunic" });
        items.Add(new UOItemType() { Graphic = 0x13E5, Color = 0x0000, Name = "Ringmail Leggings" });
        items.Add(new UOItemType() { Graphic = 0x13BB, Color = 0x0000, Name = "Ringmail Coif" });
        items.Add(new UOItemType() { Graphic = 0x13EE, Color = 0x0000, Name = "Ringmail Sleeves" });
        items.Add(new UOItemType() { Graphic = 0x13EB, Color = 0x0000, Name = "Ringmail Gloves" });
        items.Add(new UOItemType() { Graphic = 0x2B76, Color = 0x0000, Name = "Ringmail Gorget" });
        items.Add(new UOItemType() { Graphic = 0x13EF, Color = 0x0000, Name = "+20 Ringmail Sleeves" });
        items.Add(new UOItemType() { Graphic = 0x13F1, Color = 0x0000, Name = "+20 Ringmail Leggings" });
        return items;
      }
    }

    //---------------------------------------------------------------------------------------------

    public static UOItemTypeCollection PlatemailArmor
    {
      get
      {
        UOItemTypeCollection items = new UOItemTypeCollection();
        items.Add(new UOItemType() { Graphic = 0x1415, Color = 0x0000, Name = "Platemail" });
        items.Add(new UOItemType() { Graphic = 0x1411, Color = 0x0000, Name = "Platemail Legs" });
        items.Add(new UOItemType() { Graphic = 0x1412, Color = 0x0000, Name = "Platemail Helm" });
        items.Add(new UOItemType() { Graphic = 0x1410, Color = 0x0000, Name = "Platemail Arms" });
        items.Add(new UOItemType() { Graphic = 0x1414, Color = 0x0000, Name = "Platemail Gauntlets" });
        items.Add(new UOItemType() { Graphic = 0x1413, Color = 0x0000, Name = "Platemail Gorget" });
        return items;
      }
    }

    //---------------------------------------------------------------------------------------------

    public static UOItemTypeCollection ChainmailArmor
    {
      get
      {
        UOItemTypeCollection items = new UOItemTypeCollection();
        items.Add(new UOItemType() { Graphic = 0x13C4, Color = 0x0000, Name = "Chainmail Tunic" });
        items.Add(new UOItemType() { Graphic = 0x2792, Color = 0x0000, Name = "Chainmail Gloves" });
        items.Add(new UOItemType() { Graphic = 0x2FC7, Color = 0x0000, Name = "Chainmail Gorget" });
        items.Add(new UOItemType() { Graphic = 0x2B77, Color = 0x0000, Name = "Chainmail Sleeves" });
        items.Add(new UOItemType() { Graphic = 0x13C3, Color = 0x0000, Name = "Chainmail Leggings" });
        items.Add(new UOItemType() { Graphic = 0x2774, Color = 0x0000, Name = "Chainmail Coif" });
        return items;
      }
    }

    //---------------------------------------------------------------------------------------------

    public static UOItemTypeCollection BoneArmor
    {
      get
      {
        UOItemTypeCollection items = new UOItemTypeCollection();
        items.Add(new UOItemType() { Graphic = 0x144F, Color = 0x0000, Name = "Bone Chest" });
        items.Add(new UOItemType() { Graphic = 0x1452, Color = 0x0000, Name = "Bone Leggins" });
        items.Add(new UOItemType() { Graphic = 0x144E, Color = 0x0000, Name = "Bone Arms" });
        items.Add(new UOItemType() { Graphic = 0x1451, Color = 0x0000, Name = "Bone Helmet" });
        items.Add(new UOItemType() { Graphic = 0x1450, Color = 0x0000, Name = "Bone Gloves" });
        return items;
      }
    }

    //---------------------------------------------------------------------------------------------

    public static UOItemTypeCollection LeatherArmor
    {
      get
      {
        UOItemTypeCollection items = new UOItemTypeCollection();
        items.Add(new UOItemType() { Graphic = 0x13CA, Color = 0x0000, Name = "Leather Tunic" });
        items.Add(new UOItemType() { Graphic = 0x13C9, Color = 0x0000, Name = "Leather Leggins" });
        items.Add(new UOItemType() { Graphic = 0x13C5, Color = 0x0000, Name = "Leather Arms" });
        items.Add(new UOItemType() { Graphic = 0x13C7, Color = 0x0000, Name = "Leather Gorget" });
        items.Add(new UOItemType() { Graphic = 0x1DB9, Color = 0x0000, Name = "Leather Cap" });
        items.Add(new UOItemType() { Graphic = 0x13C6, Color = 0x0000, Name = "Leather Gloves" });
        return items;
      }
    }

    //---------------------------------------------------------------------------------------------

    public static UOItemTypeCollection StuddedArmor
    {
      get
      {
        UOItemTypeCollection items = new UOItemTypeCollection();
        items.Add(new UOItemType() { Graphic = 0x13D9, Color = 0x0000, Name = "Studded Tunic" });
        items.Add(new UOItemType() { Graphic = 0x13D8, Color = 0x0000, Name = "Studded Leggins" });
        items.Add(new UOItemType() { Graphic = 0x13D4, Color = 0x0000, Name = "Studded Sleeves" });
        items.Add(new UOItemType() { Graphic = 0x13D6, Color = 0x0000, Name = "Studded Gorget" });
        items.Add(new UOItemType() { Graphic = 0x1DB9, Color = 0x0000, Name = "Studded Cap" });
        items.Add(new UOItemType() { Graphic = 0x13D5, Color = 0x0000, Name = "Studded Gloves" });
        return items;
      }
    }

    //---------------------------------------------------------------------------------------------

    public static UOItemTypeCollection Shields
    {
      get
      {
        UOItemTypeCollection items = new UOItemTypeCollection();
        items.Add(new UOItemType() { Graphic = 0x1B74, Color = 0x0000, Name = "Metal Kite Shield" });
        items.Add(new UOItemType() { Graphic = 0x1B7B, Color = 0x0000, Name = "Metal Shield" });
        items.Add(new UOItemType() { Graphic = 0x1B76, Color = 0x0000, Name = "Heater Shield" });
        items.Add(new UOItemType() { Graphic = 0x1B73, Color = 0x0000, Name = "Buckler" });
        items.Add(new UOItemType() { Graphic = 0x1B78, Color = 0x0000, Name = "Wooden Kite Shield" });
        items.Add(new UOItemType() { Graphic = 0x1B7A, Color = 0x0000, Name = "Wooden Shield" });
        items.Add(new UOItemType() { Graphic = 0x1B72, Color = 0x0000, Name = "Bronze" });
        items.Add(new UOItemType() { Graphic = 0x0A15, Color = 0x0000, Name = "Lucerna" });
        return items;
      }
    }

    //---------------------------------------------------------------------------------------------
    //0x0F4F  

    public static UOItemTypeCollection WeaponsArch
    {
      get
      {
        UOItemTypeCollection items = new UOItemTypeCollection();
        items.Add(new UOItemType() { Graphic = 0x26C3, Color = 0x0000, Name = "Repeating Crossbow" });
        items.Add(new UOItemType() { Graphic = 0x0F4F, Color = 0x0000, Name = "CrossBow" });
        items.Add(new UOItemType() { Graphic = 0x13FC, Color = 0x0000, Name = "Heavy" });
        items.Add(new UOItemType() { Graphic = 0x13B1, Color = 0x0000, Name = "Bow" });
        // 0x0EC4  
        //0x08A1  
        return items;
      }
    }

    public static UOItemTypeCollection WeaponsFenc
    {
      get
      {
        
        UOItemTypeCollection items = new UOItemTypeCollection();
        items.Add(new UOItemType() { Graphic = 0x1404, Color = 0x0000, Name = "War Fork" });
        items.Add(new UOItemType() { Graphic = 0x0E87, Color = 0x0000, Name = "Pitchfork", TwoHanded = true });
        items.Add(new UOItemType() { Graphic = 0x0F62, Color = 0x0000, Name = "Spear", TwoHanded = true });
        items.Add(new UOItemType() { Graphic = 0x0F62, Color = 0x0030, Name = "War Pike", TwoHanded = true });
        items.Add(new UOItemType() { Graphic = 0x1400, Color = 0x0000, Name = "Kryss"});
        items.Add(new UOItemType() { Graphic = 0x1401, Color = 0x0845, Name = "Kryss of Vampire Kiss" });
        items.Add(new UOItemType() { Graphic = 0x1400, Color = 0x0B87, Name = "Kryss - Silver" });
        items.Add(new UOItemType() { Graphic = 0x1400, Color = 0x0770, Name = "Kryss - Shadow" });
        items.Add(new UOItemType() { Graphic = 0x0F51, Color = 0x0000, Name = "Dagger" });
        items.Add(new UOItemType() { Graphic = 0x1402, Color = 0x0000, Name = "Short Spear" });
        items.Add(new UOItemType() { Graphic = 0x1404, Color = 0x0563, Name = "+14 War Fork" });
        items.Add(new UOItemType() { Graphic = 0x1404, Color = 0x054B, Name = "+15 War Fork" });
        items.Add(new UOItemType() { Graphic = 0x1404, Color = 0x0B87, Name = "Silver War Fork" });
        items.Add(new UOItemType() { Graphic = 0x1404, Color = 0x0770, Name = "Shadow War Fork" });
        items.Add(new UOItemType() { Graphic = 0x0F62, Color = 0x0000, Name = "Spear", TwoHanded = true });
        items.Add(new UOItemType() { Graphic = 0x0F62, Color = 0x0B88, Name = "War Pike - Blood Stone", TwoHanded = true });
        items.Add(new UOItemType() { Graphic = 0x0F62, Color = 0x0B60, Name = "War Pike - Night Stone", TwoHanded = true });
        items.Add(new UOItemType() { Graphic = 0x0EC4, Color = 0x0BAF, Name = "Dyka ukryteho vraha" });
        items.Add(new UOItemType() { Graphic = 0x26C5, Color = 0x0000, Name = "Guardian's Lungbreaker" });
        items.Add(new UOItemType() { Graphic = 0x27AB, Color = 0x0000, Name = "Drapy" });
        //  
        //   0x26C5
        // 0x0EC4  
        //0x08A1  
        return items;
      }
    }

    //---------------------------------------------------------------------------------------------

    public static UOItemTypeCollection WeaponsMace
    {
      get
      {
        UOItemTypeCollection items = new UOItemTypeCollection();
        items.Add(new UOItemType() { Graphic = 0x0F5C, Color = 0x0000, Name = "Mace" });
        items.Add(new UOItemType() { Graphic = 0x143A, Color = 0x0000, Name = "Maul", TwoHanded = true });
        items.Add(new UOItemType() { Graphic = 0x143C, Color = 0x0000, Name = "Hammer Pick", TwoHanded = true });
        items.Add(new UOItemType() { Graphic = 0x13B3, Color = 0x0000, Name = "Club" });
        items.Add(new UOItemType() { Graphic = 0x1406, Color = 0x0000, Name = "War Mace" });
        items.Add(new UOItemType() { Graphic = 0x1438, Color = 0x0000, Name = "War hammer", TwoHanded = true });
        items.Add(new UOItemType() { Graphic = 0x1438, Color = 0x0B88, Name = "Rune Hammer - Blood Stone", TwoHanded = true });
        items.Add(new UOItemType() { Graphic = 0x1438, Color = 0x0BA4, Name = "Rune Hammer - Fire Stone", TwoHanded = true });
        items.Add(new UOItemType() { Graphic = 0x1438, Color = 0x0030, Name = "Rune Hammer", TwoHanded = true });
        items.Add(new UOItemType() { Graphic = 0x143A, Color = 0x0BA4, Name = "Maul - Great", TwoHanded = true });
        items.Add(new UOItemType() { Graphic = 0x143A, Color = 0x044D, Name = "Maul - +17", TwoHanded = true });
        items.Add(new UOItemType() { Graphic = 0x0DF0, Color = 0x0000, Name = "Black Staff", TwoHanded = true });
        return items;
      }
    }

            //zbrane[10] = 0x13FC; // heavy crossbow
            //zbrane[11] = 0x13B1; // bow
            //zbrane[20] = 0x0F4F; // crossbow

    //---------------------------------------------------------------------------------------------

    public static UOItemTypeCollection WeaponsSword
    {
      get
      {
        UOItemTypeCollection items = new UOItemTypeCollection();
        items.Add(new UOItemType() { Graphic = 0x143E, Color = 0x0000, Name = "Halbert", TwoHanded = true });
        items.Add(new UOItemType() { Graphic = 0x0F4D, Color = 0x0000, Name = "Bardiche", TwoHanded = true });
        items.Add(new UOItemType() { Graphic = 0x13FE, Color = 0x0000, Name = "Katana" });
        items.Add(new UOItemType() { Graphic = 0x1440, Color = 0x0000, Name = "Cutlass" });
        items.Add(new UOItemType() { Graphic = 0x13B9, Color = 0x0000, Name = "Viking Sword" });
        items.Add(new UOItemType() { Graphic = 0x13FA, Color = 0x0000, Name = "Large Battle Axe", TwoHanded = true });
        items.Add(new UOItemType() { Graphic = 0x0F47, Color = 0x0000, Name = "Battle Axe", TwoHanded = true });
        items.Add(new UOItemType() { Graphic = 0x0F60, Color = 0x0000, Name = "Longsword" });
        items.Add(new UOItemType() { Graphic = 0x0F45, Color = 0x0000, Name = "Executioner's Axe", TwoHanded = true });
        items.Add(new UOItemType() { Graphic = 0x13B5, Color = 0x0000, Name = "Scimitar" });
        items.Add(new UOItemType() { Graphic = 0x0F5E, Color = 0x0000, Name = "Broadsword" });
        items.Add(new UOItemType() { Graphic = 0x0F49, Color = 0x0000, Name = "Axe", TwoHanded = true });
        items.Add(new UOItemType() { Graphic = 0x0F4B, Color = 0x0000, Name = "Double Axe", TwoHanded = true });
        items.Add(new UOItemType() { Graphic = 0x143E, Color = 0x0000, Name = "Halberd", TwoHanded = true });
        items.Add(new UOItemType() { Graphic = 0x1442, Color = 0x0000, Name = "Two Handed Axe", TwoHanded = true });
        items.Add(new UOItemType() { Graphic = 0x13B9, Color = 0x0B88, Name = "Bastard Sword - Blood Stone", TwoHanded = true });
        items.Add(new UOItemType() { Graphic = 0x13B9, Color = 0x0BA4, Name = "Bastard Sword - Fire Stone", TwoHanded = true });
        items.Add(new UOItemType() { Graphic = 0x13B9, Color = 0x0B60, Name = "Bastard Sword - Night Stone", TwoHanded = true });
        items.Add(new UOItemType() { Graphic = 0x13B9, Color = 0x0BB3, Name = "Bastard Sword - Moon Stone", TwoHanded = true });
        items.Add(new UOItemType() { Graphic = 0x13B9, Color = 0x0B87, Name = "Bastard Sword - Light Stone", TwoHanded = true });
        items.Add(new UOItemType() { Graphic = 0x13B9, Color = 0x0030, Name = "Bastard Sword", TwoHanded = true });
        items.Add(new UOItemType() { Graphic = 0x26CE, Color = 0x0000, Name = "Paladins Sword" });
        items.Add(new UOItemType() { Graphic = 0x0EC2, Color = 0x0000, Name = "Cleaver" });
        items.Add(new UOItemType() { Graphic = 0x2D23, Color = 0x0000, Name = "Slicer" });
        items.Add(new UOItemType() { Graphic = 0x0F43, Color = 0x0000, Name = "Hatchet" });//hatchet
        items.Add(new UOItemType() { Graphic = 0x2D28, Color = 0x0000, Name = "Daemon Bane" });
        items.Add(new UOItemType() { Graphic = 0x13AF, Color = 0x0000, Name = "War Axe" });


        //items.Add(new UOItemType() { Graphic = 0x1400, Color = 0x0000, Name = "Kryss" });

        return items;
      }
    }

    //---------------------------------------------------------------------------------------------

    public static UOItemTypeCollection Robes
    {
      get
      {
        UOItemTypeCollection items = new UOItemTypeCollection();
        items.Add(new UOItemType() { Graphic = 0x1F03, Color = 0x0000, Name = "Robe" });
        return items;
      }
    }

    //---------------------------------------------------------------------------------------------

    public static UOItemTypeCollection Clothes
    {
      get
      {
        UOItemTypeCollection items = new UOItemTypeCollection();
        items.Add(new UOItemType() { Graphic = 0x1711, Color = 0x0000, Name = "Thigh Boots" });
        items.Add(new UOItemType() { Graphic = 0x1FA1, Color = 0x06BA, Name = "Bear furcoat" });
        items.Add(new UOItemType() { Graphic = 0x1515, Color = 0x0000, Name = "Cloak" });
        return items;
      }
    }

    //---------------------------------------------------------------------------------------------

    public static UOItemTypeCollection Hats
    {
      get
      {
        UOItemTypeCollection items = new UOItemTypeCollection();
        items.Add(new UOItemType() { Graphic = 0x1718, Color = 0x0000, Name = "Hat" });
        return items;
      }
    }

    //---------------------------------------------------------------------------------------------

    public static UOItemTypeCollection MagicFish
    {
      get
      {
        UOItemTypeCollection items = new UOItemTypeCollection();
        items.Add(new UOItemType() { Graphic = 0x09CD, Color = 0x0850, Name = "Magic fish" });
        items.Add(new UOItemType() { Graphic = 0x09CD, Color = 0x0482, Name = "Magic fish" });
        items.Add(new UOItemType() { Graphic = 0x09CD, Color = 0x084C, Name = "Magic fish" });
        items.Add(new UOItemType() { Graphic = 0x099B, Color = 0x0000, Name = "Magic fish" });
        items.Add(new UOItemType() { Graphic = 0x097A, Color = 0x0000, Name = "Magic fish" });
        items.Add(new UOItemType() { Graphic = 0x09CD, Color = 0x0B7C, Name = "Magic fish" });
        items.Add(new UOItemType() { Graphic = 0x09CD, Color = 0x0799, Name = "Magic fish" });
        items.Add(new UOItemType() { Graphic = 0x099B, Color = 0x08A4, Name = "Magic fish" });
        items.Add(new UOItemType() { Graphic = 0x099D, Color = 0x0481, Name = "Magic fish" });
        items.Add(new UOItemType() { Graphic = 0x09CD, Color = 0x0481, Name = "Magic fish" });

        return items;
      }
    }

    //---------------------------------------------------------------------------------------------

    public static UOItemTypeCollection Fish
    {
      get
      {
        UOItemTypeCollection items = new UOItemTypeCollection();
        items.Add(new UOItemType() { Graphic = 0x09CC, Color = 0x0000, Name = "fish" });
        items.Add(new UOItemType() { Graphic = 0x09CD, Color = 0x0000, Name = "fish" });
        items.Add(new UOItemType() { Graphic = 0x09CE, Color = 0x0000, Name = "fish" });
        items.Add(new UOItemType() { Graphic = 0x09CF, Color = 0x0000, Name = "fish" });


        return items;
      }
    }

    //---------------------------------------------------------------------------------------------

    public static UOItemTypeCollection MysticComponents
    {
      get
      {
        UOItemTypeCollection items = new UOItemTypeCollection();
        items.Add(new UOItemType() { Graphic = 0x108B, Color = 0x0BB5, Name = "Mystical Beeds" });
        items.Add(new UOItemType() { Graphic = 0x1A9D, Color = 0x0481, Name = "Mystical Stick" });
        items.Add(new UOItemType() { Graphic = 0x0F5A, Color = 0x0044, Name = "Mystic Crystal" });
        items.Add(new UOItemType() { Graphic = 0x0DC3, Color = 0x005B, Name = "Mystical Flower" });
        items.Add(new UOItemType() { Graphic = 0x136C, Color = 0x0B94, Name = "Mystical Stone" });
        items.Add(new UOItemType() { Graphic = 0x0DBD, Color = 0x0B9F, Name = "Mystical Leaf" });
        items.Add(new UOItemType() { Graphic = 0x0CB0, Color = 0x0899, Name = "Mystical Plant" });
        items.Add(new UOItemType() { Graphic = 0x0E73, Color = 0x0B9F, Name = "Mystical Ball" });
        return items;
      }
    }

    //---------------------------------------------------------------------------------------------

    public static UOItemType BlankMagicKey { get { return new UOItemType() { Graphic = 0x1012, Color = 0x0000 }; } }
    public static UOItemType KeyRing0 { get { return new UOItemType() { Graphic = 0x1011, Color = 0x0000 }; } }
    public static UOItemType KeyRing1 { get { return new UOItemType() { Graphic = 0x1769, Color = 0x0000 }; } }
    public static UOItemType KeyRing2 { get { return new UOItemType() { Graphic = 0x176A, Color = 0x0000 }; } }
    public static UOItemType KeyRing3 { get { return new UOItemType() { Graphic = 0x176B, Color = 0x0000 }; } }

    //---------------------------------------------------------------------------------------------

    //---------------------------------------------------------------------------------------------
    public static UOItemType RuneBook { get { return new UOItemType() { Graphic = 0x0FF0, Color = 0x08A5, Name = "Rune Book" }; } }
    public static UOItemType TravelBook { get { return new UOItemType() { Graphic = 0x0FEF, Color = 0x0482, Name = "Travel Book" }; } }
    public static UOItemType KeyRing { get { return new UOItemType() { Graphic = 0x176B, Color = 0x0000, Name = "Key Ring" }; } }
    public static UOItemType Mortar { get { return new UOItemType() { Graphic = 0x0E9B, Color = 0x0000, Name = "Mortar" }; } }
    public static UOItemType SpellBook { get { return new UOItemType() { Graphic = 0x0EFA, Color = 0x0000, Name = "Spell Book" }; } }
    public static UOItemType DwarfKnife { get { return new UOItemType() { Graphic = 0x10E4, Color = 0x0000, Name = "Dwarf Knife" }; } }
    public static UOItemType PoisonKit { get { return new UOItemType() { Graphic = 0x1837, Color = 0x0000, Name = "Poison Kit" }; } }
    public static UOItemType Backpack { get { return new UOItemType() { Graphic = 0x0E75, Color = 0x0000, Name = "Backpack" }; } }
    public static UOItemType Serpa { get { return new UOItemType() { Graphic = 0x1541, Color = 0x0000, Name = "Serpa" }; } }
    public static UOItemType KlanKniha { get { return new UOItemType() { Graphic = 0x0FBD, Color = 0x0000, Name = "KlanKniha" }; } }
    public static UOItemType KlanKad { get { return new UOItemType() { Graphic = 0x1940, Color = 0x0000, Name = "KlanKad" }; } }
    public static UOItemType CestovniKniha { get { return new UOItemType() { Graphic = 0x22C5, Color = 0x0000, Name = "CestovniKniha" }; } }
    public static UOItemType BoltQuiver { get { return new UOItemType() { Graphic = 0x1EA0, Color = 0x083A, Name = "BoltQuiver" }; } }
    public static UOItemType ArrowQuiver { get { return new UOItemType() { Graphic = 0x1EA0, Color = 0x0747, Name = "ArrowQuiver" }; } }
    public static UOItemType Scissors { get { return new UOItemType() { Graphic = 0x0F9E, Color = 0x0000, Name = "Scissors" }; } }
    public static UOItemType LockNaramek { get { return new UOItemType() { Graphic = 0x1F06, Color = 0x0B18, Name = "LockNaramek" }; } }
    public static UOItemType Voditko { get { return new UOItemType() { Graphic = 0x1374, Color = 0x0B4C, Name = "Voditko" }; } }

    public static UOItemType NbRuna { get { return new UOItemType() { Graphic = 0x1F14, Color = 0x0B1D, Name = "NbRuna" }; } }

    public static UOItemType Panvicka { get { return new UOItemType() { Graphic = 0x097F, Color = 0x0000, Name = "Panvicka" }; } }
    public static UOItemType RawFishSteak { get { return new UOItemType() { Graphic = 0x097A, Color = 0x0000, Name = "Raw fish steak" }; } }

    public static UOItemType DuchStepi { get { return new UOItemType() { Graphic = 0x0FC4, Color = 0x0B83, Name = "DuchStepik" }; } }
    public static UOItemType DuchPralesa { get { return new UOItemType() { Graphic = 0x0E26, Color = 0x0B83, Name = "DuchPralesa" }; } }

    public static UOItemType BeltPouch  { get { return new UOItemType() { Graphic = 0x0E79, Color = 0x0000, Name = "BeltPouch" }; } }

    public static UOItemType EmptyBottle { get { return new UOItemType() { Graphic = 0x0F0E, Color = 0x0000, Name = "EmptyBottle" }; } }
    public static UOItemType DungeonScrool { get { return new UOItemType() { Graphic = 0x227A, Color = 0x0498, Name = "DungeonScrool" }; } }
    public static UOItemType MagickyBrk { get { return new UOItemType() { Graphic = 0x0FBF, Color = 0x0000, Name = "MagickyBrk" }; } }
    public static UOItemType CleanBandages { get { return new UOItemType() { Graphic = 0x0E21, Color = 0x0000, Name = "CleanBandages" }; } }


    public static UOItemType VampKrystal { get { return new UOItemType() { Graphic = 0x1F19, Color = 0x0000, Name = "VampKrystal" }; } }
    public static UOItemType StoneVampKrystal { get { return new UOItemType() { Graphic = 0x1F19, Color = 0x0B87, Name = "StoneVampKrystal" }; } }
    public static UOItemType StoneKad { get { return new UOItemType() { Graphic = 0x1843, Color = 0x04CC, Name = "StoneKad" }; } }
    public static UOItemType GreeziArtefakt { get { return new UOItemType() { Graphic = 0x1B17, Color = 0x0493, Name = "GreeziArtefakt" }; } }

    //public static UOItemType HumanModel { get { return new UOItemType() { Graphic = 0x0190 }; } }
    //public static UOItemTemplate GinsengSalad { get { return new UOItemTemplate() { Graphic = 0x09EC, Color = 0x06AB, Name = "GinsengSalad" }; } }
  }
}
