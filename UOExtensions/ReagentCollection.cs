using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;

namespace CalExtension.UOExtensions
{
  public class ReagentCollection : List<Reagent>
  {
    //---------------------------------------------------------------------------------------------

    public static ReagentCollection Reagents
    {
      get
      {
        ReagentCollection reagents = new ReagentCollection() 
        { 
          Reagent.BloodMoss, Reagent.Garlic, Reagent.SulphurousAsh, Reagent.Ginseng, Reagent.SpidersSilk, Reagent.Nightshade,
          Reagent.BlackPearl, Reagent.MandrakeRoot, Reagent.WyrmsHeart, Reagent.EyeofNewt, 
          Reagent.Brimstone, Reagent.Bloodspawn, Reagent.DaemonBlood, Reagent.Blackmoor, 
          Reagent.Pumice, Reagent.Obsidian, Reagent.Bones, Reagent.DaemonBones, 
          Reagent.BlueEye, Reagent.SerpentsScales, Reagent.VulcanicAsh,  Reagent.Batwings,
          Reagent.ExecutionersCap, Reagent.FertileDirt, Reagent.DragonBlood, Reagent.DaemonBlood
        };

        return reagents;
      }
    }

    //---------------------------------------------------------------------------------------------

    public static ReagentCollection MagReagents
    {
      get
      {
        ReagentCollection reagents = new ReagentCollection() 
        { 
          Reagent.BloodMoss, Reagent.Garlic, Reagent.SulphurousAsh, Reagent.Ginseng, Reagent.SpidersSilk, Reagent.Nightshade,
          Reagent.BlackPearl, Reagent.MandrakeRoot
        };

        return reagents;
      }
    }

    //---------------------------------------------------------------------------------------------

    public static ReagentCollection NecroReagents
    {
      get
      {
        ReagentCollection reagents = new ReagentCollection() 
        { 
          Reagent.WyrmsHeart, Reagent.EyeofNewt, 
          Reagent.Brimstone, Reagent.Bloodspawn, Reagent.DaemonBlood, Reagent.Blackmoor, 
          Reagent.Pumice, Reagent.Obsidian, Reagent.Bones, Reagent.DaemonBones, 
          Reagent.SerpentsScales, Reagent.VulcanicAsh,  Reagent.Batwings,
          Reagent.ExecutionersCap, Reagent.FertileDirt, Reagent.DragonBlood
        };

        return reagents;
      }
    }

    //---------------------------------------------------------------------------------------------

    public UOItemTypeBaseCollection ToItemTypeCollection()
    {
      UOItemTypeBaseCollection items = new UOItemTypeBaseCollection();

      foreach(Reagent r in this)
      {
        items.Add(new UOItemTypeBase(r.Graphic, r.Color) { });
      }
      return items;
    }

    //---------------------------------------------------------------------------------------------

    public bool Contains(UOItem item)
    {
      return this.Contains(item.Graphic, item.Color);
    }

    //---------------------------------------------------------------------------------------------

    public bool Contains(Graphic graphic, UOColor color)
    {
      foreach (Reagent r in this)
        if (r.Graphic == graphic && r.Color == color) return true;

      return false;
    }

    //---------------------------------------------------------------------------------------------

    public Reagent FindReagent(UOItem item)
    {
      foreach (Reagent r in this)
        if (r.Graphic == item.Graphic && r.Color == item.Color) return r;

      return null;
    }

    //---------------------------------------------------------------------------------------------
  }

  //---------------------------------------------------------------------------------------------
}
