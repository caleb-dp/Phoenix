using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;

namespace CalExtension.UOExtensions
{
  //---------------------------------------------------------------------------------------------

  public class Reagent 
  {
    protected Graphic graphic = Graphic.Invariant;
    public Graphic Graphic { get { return this.graphic; } set { this.graphic = value; } }
    
    protected UOColor color = UOColor.Invariant;
    public UOColor Color { get { return this.color; } set { this.color = value; } }

    protected string name = "";
    public string Name { get { return this.name; } set { this.name = value; } }

    protected string shortName = "";
    public string ShortName { get { return this.shortName; } set { this.shortName = value; } }

    public bool IsValid { get { return true; /*!this.Graphic.IsInvariant && !this.Color.IsConstant;*/ } }

    public Reagent(Graphic gra, UOColor col, string name, string shortName)
    {
      this.graphic = gra;
      this.color = col;
      this.name = name;
      this.shortName = shortName;
    }

    //---------------------------------------------------------------------------------------------

    public static UOItem FindReagent(Serial container, Graphic graphic, UOColor color)
    {
      return FindReagent(container, graphic, color, null);
    }

    //---------------------------------------------------------------------------------------------

    public static UOItem FindReagent(Serial container, Graphic graphic, UOColor color, int? amount)
    {
      UOItem cont = new UOItem(container);
      if (cont.Exist)
      {
        foreach (UOItem item in cont.Items)
        {
          if (item.Graphic == graphic && item.Color == color && (!amount.HasValue || (item.Amount == amount.Value || item.Amount > amount.Value)))
            return item;
        }
        //return cont.AllItems.FindType(graphic, color);
      }
      return new UOItem(Serial.Invalid);
    }

    //---------------------------------------------------------------------------------------------

    public static UOItem FindReagentAll(Serial container, Graphic graphic, UOColor color)
    {
      return FindReagentAll(container, graphic, color, null);
    }

    //---------------------------------------------------------------------------------------------

    public static UOItem FindReagentAll(Serial container, Graphic graphic, UOColor color, int? amount)
    {
      UOItem cont = new UOItem(container);
      if (cont.Exist)
      {
        foreach (UOItem item in cont.AllItems)
        {
          if (item.Graphic == graphic && item.Color == color && (!amount.HasValue || (item.Amount == amount.Value || item.Amount > amount.Value)))
            return item;
        }
        //return cont.AllItems.FindType(graphic, color);
      }
      return new UOItem(Serial.Invalid);
    }

    //---------------------------------------------------------------------------------------------

    public static Reagent BloodMoss       { get { return new Reagent(0x0F7B, 0x0000, "Blood Moss", "Bm"); } }
    public static Reagent Garlic          { get { return new Reagent(0x0F84, 0x0000, "Garlic", "Ga"); } }
    public static Reagent SulphurousAsh   { get { return new Reagent(0x0F8C, 0x0000, "Sulfurous Ash", "Sa"); } }
    public static Reagent Ginseng         { get { return new Reagent(0x0F85, 0x0000, "Ginseng", "Gi"); } }
    public static Reagent SpidersSilk     { get { return new Reagent(0x0F8D, 0x0000, "Spider's Silk", "Ss"); } }
    public static Reagent Nightshade      { get { return new Reagent(0x0F88, 0x0000, "Nightshade", "Ns"); } }
    public static Reagent BlackPearl      { get { return new Reagent(0x0F7A, 0x0000, "Black Pearls", "Bp"); } }
    public static Reagent MandrakeRoot    { get { return new Reagent(0x0F86, 0x0000, "Mandrake Roots", "Mr"); } }

    public static Reagent WyrmsHeart      { get { return new Reagent(0x0F91, 0x0000, "Wyrm's Hearts", ""); } }
    public static Reagent EyeofNewt       { get { return new Reagent(0x0F87, 0x0000, "Eyes of Newt", "Eon"); } }
    public static Reagent BlueEye         { get { return new Reagent(0x0F87, 0x0005, "Blue Eye", "Be"); } }
    public static Reagent SerpentsScales { get { return new Reagent(0x0F8E, 0x0000, "Serpents Scales", ""); } }
    public static Reagent VulcanicAsh     { get { return new Reagent(0x0F8F, 0x0000, "Volcanic Ash", ""); } }
    public static Reagent Batwings        { get { return new Reagent(0x0F78, 0x0000, "Batwings", "Bw"); } }
    public static Reagent Brimstone       { get { return new Reagent(0x0F7F, 0x0000, "Brimstone", ""); } }
    public static Reagent Bloodspawn      { get { return new Reagent(0x0F7C, 0x0000, "Bloodspawn", ""); } }
    public static Reagent DaemonBlood     { get { return new Reagent(0x0F7D, 0x0000, "Daemon Blood", ""); } }
    public static Reagent Blackmoor { get { return new Reagent(0x0F79, 0x0000, "Blackmoor", ""); } }
    public static Reagent Pumice { get { return new Reagent(0x0F8B, 0x0000, "Pumice", ""); } }
    public static Reagent Obsidian { get { return new Reagent(0x0F89, 0x0000, "Obsidian", ""); } }
    public static Reagent Bones { get { return new Reagent(0x0F7E, 0x0000, "Bones", ""); } }
    public static Reagent DaemonBones { get { return new Reagent(0x0F80, 0x0000, "Daemon Bones", ""); } }
    public static Reagent ExecutionersCap { get { return new Reagent(0x0F83, 0x0000, "Executioner's Cap", ""); } }
    public static Reagent FertileDirt { get { return new Reagent(0x0F81, 0x0000, "FertileDirt", ""); } }
    public static Reagent DragonBlood { get { return new Reagent(0x0F82, 0x0000, "DragonBlood", ""); } }
    public static Reagent DarkBlood { get { return new Reagent(0x0F7D, 0x031D, "Dark Blood", ""); } }
  }
}
