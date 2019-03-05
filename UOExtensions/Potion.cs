using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;

namespace CalExtension.UOExtensions
{
  public class Potion
  {
    public static readonly Graphic KadGraphic = 0x1843;

    public string Name
    {
      get { return this.name; }
      set { this.name = value; }
    }

    public string name = "";
    public string Shortcut = "";
    public Reagent Reagent;
    private Dictionary<PotionQuality, PotionQualityDefinition> qualities;
    public Dictionary<PotionQuality, PotionQualityDefinition> Qualities
    {
      get
      {
        if (this.qualities == null)
          this.qualities = new Dictionary<PotionQuality, PotionQualityDefinition>();
        return this.qualities;
      }
      private set { this.qualities = value; }
    }


    //---------------------------------------------------------------------------------------------

    public Graphic DefaultGraphic
    {
      get
      {
        foreach (KeyValuePair<PotionQuality, PotionQualityDefinition> kvp in this.Qualities)
          return kvp.Value.Graphic;

        return Graphic.Invariant;
      }
    }

    //---------------------------------------------------------------------------------------------

    public UOColor DefaultColor
    {
      get
      {
        foreach (KeyValuePair<PotionQuality, PotionQualityDefinition> kvp in this.Qualities)
          return kvp.Value.Color;

        return UOColor.Invariant;
      }
    }

    //---------------------------------------------------------------------------------------------

    public UOColor TopKadColor
    {
      get
      {
        if (this.Qualities.ContainsKey(PotionQuality.Deadly))
          return this.Qualities[PotionQuality.Deadly].KadColor;
        else if (this.Qualities.ContainsKey(PotionQuality.Greater))
          return this.Qualities[PotionQuality.Greater].KadColor;
        else if (this.Qualities.ContainsKey(PotionQuality.Total))
          return this.Qualities[PotionQuality.Total].KadColor;
        else if (this.Qualities.ContainsKey(PotionQuality.None))
          return this.Qualities[PotionQuality.None].KadColor;
        else if (this.Qualities.ContainsKey(PotionQuality.Lesser))
          return this.Qualities[PotionQuality.Lesser].KadColor;

        return UOColor.Invariant;
      }
    }

    //---------------------------------------------------------------------------------------------

    public PotionQualityDefinition TopQualityDefinition 
    {
      get
      {
        if (this.Qualities.ContainsKey(PotionQuality.Deadly))
          return this.Qualities[PotionQuality.Deadly];
        else if (this.Qualities.ContainsKey(PotionQuality.Greater))
          return this.Qualities[PotionQuality.Greater];
        else if (this.Qualities.ContainsKey(PotionQuality.Total))
          return this.Qualities[PotionQuality.Total];
        else if (this.Qualities.ContainsKey(PotionQuality.None))
          return this.Qualities[PotionQuality.None];
        else if (this.Qualities.ContainsKey(PotionQuality.Lesser))
          return this.Qualities[PotionQuality.Lesser];

        return null;
      }
    }

    //---------------------------------------------------------------------------------------------

    public PotionQualityDefinition GetQualityDefinition(PotionQuality quality)
    {
      if (this.Qualities.ContainsKey(quality))
        return this.Qualities[quality];

      return TopQualityDefinition;
    }

    //---------------------------------------------------------------------------------------------

    public int GetAmountOrDefault(PotionQuality pq)
    {
      if (this.Qualities.ContainsKey(pq))
        return this.Qualities[pq].Amount;

      return 0;
    }

    //---------------------------------------------------------------------------------------------

    public static PotionQuality ParsePQOrDefault(string name)
    {
      return ParsePQOrDefault(name, PotionQuality.None);
    }

    //---------------------------------------------------------------------------------------------

    public static PotionQuality ParsePQOrDefault(string name, PotionQuality df)
    {
      return ParsePQOrDefault(name, df, null);
    }

    //---------------------------------------------------------------------------------------------

    public static PotionQuality ParsePQOrDefault(string name, PotionQuality df, Potion potion)
    {
      PotionQuality result = df;
      try { result = (PotionQuality)Enum.Parse(typeof(PotionQuality), name, true); }
      catch { }
      if (potion != null && !potion.Qualities.ContainsKey(result))
      {
        result = potion.Qualities.GetEnumerator().Current.Key;
      }

      return result;
    }

    //---------------------------------------------------------------------------------------------

    public bool ContainsTopKad(UOItem container)
    {
      return container.Items.FindType(KadGraphic, this.TopKadColor).Exist;
    }

    //---------------------------------------------------------------------------------------------

    public static bool HasEmptyPotion
    {
      get { return GetEmptyPotion().Exist; }
    }

    //---------------------------------------------------------------------------------------------

    public static UOItem GetEmptyPotion()
    {
      UOItem empty = UO.Backpack.AllItems.FindType(0x0F0E);
      if (!empty.Exist) 
        Game.PrintMessage("Nemas prazdne lahve!");
      return empty;
    }

    public static Graphic Empty = 0x0F0E;

    //---------------------------------------------------------------------------------------------
    public static Potion Agility
    {
      get
      {
        Dictionary<PotionQuality, PotionQualityDefinition> dict = new Dictionary<PotionQuality, PotionQualityDefinition>();
        dict.Add(PotionQuality.None, new PotionQualityDefinition() { Amount = 2, Color = 0x0000, Graphic = 0x0F08, KadColor = 0x00BF, MenuName = "Agility Potion" });
        dict.Add(PotionQuality.Greater, new PotionQualityDefinition() { Amount = 3, Color = 0x0000, Graphic = 0x0F08, KadColor = 0x00BD, MenuName = "Greater Agility" });
        return new Potion() { Name = "Agility", Reagent = Reagent.BloodMoss, Qualities = dict, Shortcut = "Agility" };
      }
    }

    public static Potion Cure
    {
      get
      {
        Dictionary<PotionQuality, PotionQualityDefinition> dict = new Dictionary<PotionQuality, PotionQualityDefinition>();
        dict.Add(PotionQuality.Lesser, new PotionQualityDefinition() { Amount = 2, Color = 0x0000, Graphic = 0x0F07, KadColor = 0x0091, MenuName = "Lesser Cure Potion" });
        dict.Add(PotionQuality.None, new PotionQualityDefinition() { Amount = 3, Color = 0x0000, Graphic = 0x0F07, KadColor = 0x0091, MenuName = "Cure Potion" });//??KadBarva
        dict.Add(PotionQuality.Greater, new PotionQualityDefinition() { Amount = 6, Color = 0x0000, Graphic = 0x0F07, KadColor = 0x0842, MenuName = "Greater Cure" });
        return new Potion() { Name = "Cure", Reagent = Reagent.Garlic, Qualities = dict, Shortcut = "Cure" };
      }
    }

    public static Potion Explosion
    {
      get
      {
        Dictionary<PotionQuality, PotionQualityDefinition> dict = new Dictionary<PotionQuality, PotionQualityDefinition>();
        dict.Add(PotionQuality.Lesser, new PotionQualityDefinition() { Amount = 3, Color = 0x0000, Graphic = 0x0F0D, KadColor = 0x001A, MenuName = "Lesser Explosion potion" });
        dict.Add(PotionQuality.None, new PotionQualityDefinition() { Amount = 5, Color = 0x0000, Graphic = 0x0F0D, KadColor = 0x001A, MenuName = "Explosion potion" });//??KadBarva
        dict.Add(PotionQuality.Greater, new PotionQualityDefinition() { Amount = 10, Color = 0x0000, Graphic = 0x0F0D, KadColor = 0x001A, MenuName = "Greater Explosion" });//??KadBarva
        return new Potion() { Name = "Explosion", Reagent = Reagent.SulphurousAsh, Qualities = dict, Shortcut = "Explosion" };

        //Serial: 0x402C5936  Name: "Nadoba s Lesser Explosion (2 "  Position: 66.86.0  Flags: 0x0020  Color: 0x001A  Graphic: 0x1843  Amount: 1  Layer: None Container: 0x402CBC83

        //Serial: 0x402B2ED0  Position: 85.106.0  Flags: 0x0000  Color: 0x0000  Graphic: 0x0F0D  Amount: 1  Layer: None Container: 0x402CBC83


      }
    }

    public static Potion Heal
    {
      get
      {
        Dictionary<PotionQuality, PotionQualityDefinition> dict = new Dictionary<PotionQuality, PotionQualityDefinition>();
        dict.Add(PotionQuality.Lesser, new PotionQualityDefinition() { Amount = 2, Color = 0x0000, Graphic = 0x0F0C, KadColor = 0x08A7, MenuName = "Lesser Heal Potion" });//??KadBarva
        dict.Add(PotionQuality.None, new PotionQualityDefinition() { Amount = 3, Color = 0x0000, Graphic = 0x0F0C, KadColor = 0x08A8, MenuName = "Heal Potion" });//??KadBarva
        dict.Add(PotionQuality.Greater, new PotionQualityDefinition() { Amount = 7, Color = 0x0000, Graphic = 0x0F0C, KadColor = 0x08A7, MenuName = "Greater Heal" });
        return new Potion() { Name = "Heal", Reagent = Reagent.Ginseng, Qualities = dict, Shortcut = "Heal" };
      }
    }

    public static Potion Nightsight
    {
      get
      {
        Dictionary<PotionQuality, PotionQualityDefinition> dict = new Dictionary<PotionQuality, PotionQualityDefinition>();
        dict.Add(PotionQuality.None, new PotionQualityDefinition() { Amount = 2, Color = 0x0000, Graphic = 0x0F06, KadColor = 0x03C4, MenuName = "Nightsight" });//??KadBarva
        return new Potion() { Name = "Nightsight", Reagent = Reagent.SpidersSilk, Qualities = dict, Shortcut = "NS" };
      }
    }

    public static Potion Poison
    {
      get
      {
        Dictionary<PotionQuality, PotionQualityDefinition> dict = new Dictionary<PotionQuality, PotionQualityDefinition>();
        dict.Add(PotionQuality.Lesser, new PotionQualityDefinition() { Amount = 2, Color = 0x0000, Graphic = 0x0F0A, KadColor = 0x089F, MenuName = "Lesser Poison" });//??KadBarva
        dict.Add(PotionQuality.None, new PotionQualityDefinition() { Amount = 4, Color = 0x0000, Graphic = 0x0F0A, KadColor = 0x08A2, MenuName = "Poison Potion" });//??KadBarva
        dict.Add(PotionQuality.Greater, new PotionQualityDefinition() { Amount = 7, Color = 0x0000, Graphic = 0x0F0A, KadColor = 0x08A2, MenuName = "Grater Poison" });//??KadBarva
        dict.Add(PotionQuality.Deadly, new PotionQualityDefinition() { Amount = 10, Color = 0x0000, Graphic = 0x0F0A, KadColor = 0x08A2, MenuName = "Deadly Poison" });
        return new Potion() { Name = "Poison", Reagent = Reagent.Nightshade, Qualities = dict, Shortcut = "Pois" };
      }
    }

    public static Potion Refresh
    {
      get
      {
        Dictionary<PotionQuality, PotionQualityDefinition> dict = new Dictionary<PotionQuality, PotionQualityDefinition>();
        dict.Add(PotionQuality.None, new PotionQualityDefinition() { Amount = 2, Color = 0x0000, Graphic = 0x0F0B, KadColor = 0x014D, MenuName = "Refresh Potion" });//??KadBarva
        dict.Add(PotionQuality.Total, new PotionQualityDefinition() { Amount = 5, Color = 0x0000, Graphic = 0x0F0B, KadColor = 0x014D, MenuName = "Total Refresh" });
        return new Potion() { Name = "Refresh", Reagent = Reagent.BlackPearl, Qualities = dict, Shortcut = "Refresh" };
      }
    }

    public static Potion Strength
    {
      get
      {
        Dictionary<PotionQuality, PotionQualityDefinition> dict = new Dictionary<PotionQuality, PotionQualityDefinition>();
        dict.Add(PotionQuality.None, new PotionQualityDefinition() { Amount = 2, Color = 0x0000, Graphic = 0x0F09, KadColor = 0x0481, MenuName = "Strength Potion" });//??KadBarva
        dict.Add(PotionQuality.Greater, new PotionQualityDefinition() { Amount = 6, Color = 0x0000, Graphic = 0x0F09, KadColor = 0x0481, MenuName = "Greater Strength" });
        return new Potion() { Name = "Strength", Reagent = Reagent.MandrakeRoot, Qualities = dict, Shortcut = "Strength" };
      }
    }

    public static Potion Shrink
    {
      get
      {
        Dictionary<PotionQuality, PotionQualityDefinition> dict = new Dictionary<PotionQuality, PotionQualityDefinition>();
        dict.Add(PotionQuality.None, new PotionQualityDefinition() { Amount = 3, Color = 0x045E, Graphic = 0x0F09, KadColor = 0x0724, MenuName = "Shrink" });
        return new Potion() { Name = "Shrink", Reagent = Reagent.Batwings, Qualities = dict, Shortcut = "Shrink" };
      }
    }

    public static Potion Invisibility
    {
      get
      {
        Dictionary<PotionQuality, PotionQualityDefinition> dict = new Dictionary<PotionQuality, PotionQualityDefinition>();
        dict.Add(PotionQuality.None, new PotionQualityDefinition() { Amount = 4, Color = 0x0B77, Graphic = 0x0F09, KadColor = 0x0B77, MenuName = "Invisibility" });
        return new Potion() { Name = "Invisibility", Reagent = Reagent.WyrmsHeart, Qualities = dict, Shortcut = "Invis" };
      }
    }

    public static Potion ManaRefresh//TODO jediny potion ktery ma ruzne regy pro qulity
    {
      get
      {
        Dictionary<PotionQuality, PotionQualityDefinition> dict = new Dictionary<PotionQuality, PotionQualityDefinition>();
        dict.Add(PotionQuality.None, new PotionQualityDefinition() { Amount = 3, Color = 0x0005, Graphic = 0x0F09, KadColor = 0x0005, MenuName = "Mana Refresh Potion" });//??KadBarva
        return new Potion() { Name = "Mana Refresh", Reagent = Reagent.EyeofNewt, Qualities = dict, Shortcut = "MR" };
      }
    }

    public static Potion TotalManaRefresh//TODO jediny potion ktery ma ruzne regy pro qulity
    {
      get
      {
        Dictionary<PotionQuality, PotionQualityDefinition> dict = new Dictionary<PotionQuality, PotionQualityDefinition>();
        dict.Add(PotionQuality.None, new PotionQualityDefinition() { Amount = 6, Color = 0x0003, Graphic = 0x0F09, KadColor = 0x0003, MenuName = "Total Mana Refresh" });//??KadBarva
        return new Potion() { Name = "Total Mana Refresh", Reagent = Reagent.EyeofNewt, Qualities = dict, Shortcut = "TMR" };
      }
    }

    public static Potion FlaskOfHallucination
    {
      get
      {
        Dictionary<PotionQuality, PotionQualityDefinition> dict = new Dictionary<PotionQuality, PotionQualityDefinition>();
        dict.Add(PotionQuality.None, new PotionQualityDefinition() { Amount = 6, Color = 0x0B90, Graphic = 0x0F06, KadColor = 0x0B90, MenuName = "Hallucination" });//??KadBarva
        return new Potion() { Name = "Hallucination", Reagent = Reagent.SerpentsScales, Qualities = dict, Shortcut = "Hallu" };
      }
    }

    public static Potion LavaBomb
    {
      get
      {
        Dictionary<PotionQuality, PotionQualityDefinition> dict = new Dictionary<PotionQuality, PotionQualityDefinition>();
        dict.Add(PotionQuality.None, new PotionQualityDefinition() { Amount = 6, Color = 0x000E, Graphic = 0x0F0D, KadColor = 0x000E, MenuName = "Lava Bomb" });//??KadBarva
        return new Potion() { Name = "Lava Bomb", Reagent = Reagent.VulcanicAsh, Qualities = dict, Shortcut = "Lava" };
      }
    }

    public static Potion Blood
    {
      get
      {
        Dictionary<PotionQuality, PotionQualityDefinition> dict = new Dictionary<PotionQuality, PotionQualityDefinition>();
        dict.Add(PotionQuality.None, new PotionQualityDefinition() { Amount = 0, Color = 0x0025, Graphic = 0x0F0C, KadColor = 0x0025 });//
        dict.Add(PotionQuality.Greater, new PotionQualityDefinition() { Amount = 0, Color = 0x0025, Graphic = 0x0F0C, KadColor = 0x0025 });//??KadBarva
        return new Potion() { Name = "Blood", Qualities = dict, Shortcut= "Blood" };
      }
    }
  }

  //---------------------------------------------------------------------------------------------

  public class PotionQualityDefinition
  {
    public int Amount = 0;
    public Graphic Graphic = Graphic.Invariant;
    public UOColor Color = UOColor.Invariant;
    public Graphic KadGraphic = Potion.KadGraphic;
    public UOColor KadColor = UOColor.Invariant;
    public string MenuName = String.Empty;

    public UOItemTypeBase ToItemType()
    {
      return new UOItemTypeBase(this.Graphic, this.Color);
    }
  }

  //---------------------------------------------------------------------------------------------

  public enum PotionQuality
  {
    None = 0,
    Lesser = 1,
    Greater = 2,
    Deadly = 4,
    Total = 8,
  }

  //---------------------------------------------------------------------------------------------
}
