using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;
using Caleb.Library;
using System.Threading;
using CalExtension.Skills;
using CalExtension.UOExtensions;
using Caleb.Library.CAL.Business;
using CalExtension.Abilities;

namespace CalExtension.Skills
{
  public class Healing : Skill
  {
    public static UOItemType VAL_CleanBandage_1 { get { return new UOItemType() { Graphic = 0x0E21, Color = 0x0000, Name = "clean bandages" }; } }
    public static UOItemType VAL_BloodyBandage_1 { get { return new UOItemType() { Graphic = 0x0E20, Color = 0x0000, Name = "bloody bandages" }; } }
    public static UOItemType VAL_BloodyBandage_2 { get { return new UOItemType() { Graphic = 0x0E22, Color = 0x0000, Name = "bloody bandages" }; } }
    public static UOItemType VAL_Salat { get { return new UOItemType() { Graphic = 0x09EC, Color = 0x06AB, Name = "Ginseng Salad" }; } }
    public static UOItemType VAL_WashBasin { get { return new UOItemType() { Graphic = 0x1008, Color = 0x0000, Name = "Wash basin" }; } }

    //---------------------------------------------------------------------------------------------

    private static UOCharacter lastCharacter;
    public static UOCharacter LastCharacter
    {
      get { return lastCharacter; }
      set { lastCharacter = value; }
    }

    //---------------------------------------------------------------------------------------------

    public static UOItem CleanBandage
    {
      get
      {
        UOItem bandage = World.Player.FindType(VAL_CleanBandage_1.Graphic);
        if (bandage.Exist)
          return bandage;
        else
        {
          bandage = World.Ground.FindType(VAL_CleanBandage_1.Graphic);
          if (bandage.Exist && bandage.Distance <= 3)
            return bandage;
        }

        return new UOItem(Serial.Invalid);
      }
    }

    //---------------------------------------------------------------------------------------------

    public static UOItem BloodyBandage
    {
      get
      {
        List<UOItem> bloodbandages = new List<UOItem>();

        bloodbandages.Add(World.Player.FindType(VAL_BloodyBandage_1.Graphic));
        bloodbandages.Add(World.Player.FindType(VAL_BloodyBandage_2.Graphic));
        bloodbandages.Add(World.Ground.FindType(VAL_BloodyBandage_1.Graphic));
        bloodbandages.Add(World.Ground.FindType(VAL_BloodyBandage_2.Graphic));

        bloodbandages = bloodbandages.Where(i => i.Exist && i.Distance <= 3).OrderByDescending(i => i.Amount).ToList();

        if (bloodbandages.Count > 0)
          return bloodbandages[0];

        return new UOItem(Serial.Invalid);
      }
    }

    //---------------------------------------------------------------------------------------------

    public static List<CharHealPriority> GetCharHealPriorityList(double maxdistance, bool poisoned, List<UOCharacter> charsSource)
    {
      List<UOCharacter> chars = new List<UOCharacter>();
      chars.Add(World.Player);

      if (charsSource != null)
      {
        chars.AddRange(charsSource);
        if (Healing.LastCharacter != null)
        {
          if (Healing.LastCharacter.Hits >= Healing.LastCharacter.MaxHits && !Healing.LastCharacter.Poisoned)
            Healing.LastCharacter = null;
          else if (chars.Where(c => c.Serial == Healing.LastCharacter).ToArray().Length == 0)
            chars.Add(LastCharacter);
        }

        UOCharacter hover = new UOCharacter(Serial.Invalid);
        if (Game.CurrentGame.CurrentHoverStatus != null)
          hover = new UOCharacter(Game.CurrentGame.CurrentHoverStatus);

        if (hover.ExistCust() && 
          (
          Game.CurrentGame.IsHealAlie(hover) ||
          Game.CurrentGame.IsAlie(hover) || 
          hover.Notoriety == Notoriety.Guild ||
          hover.Notoriety == Notoriety.Neutral || 
          hover.Notoriety == Notoriety.Innocent
          ) &&
    chars.Where(c => c.Serial == Game.CurrentGame.CurrentHoverStatus).ToArray().Length == 0
    )
        {
          chars.Add(new UOCharacter(Game.CurrentGame.CurrentHoverStatus));
        }
      }

      List<CharHealPriority> chhpList = new List<CharHealPriority>();
      //nastaveni priorit pro sezaeni
      foreach (UOCharacter ch in chars)
      {
        if (ch.Distance <= maxdistance && (ch.Hits < ch.MaxHits || (poisoned && ch.Poisoned)) && ch.Hits > 0 && !ch.Dead)
        {
          CharHealPriority chhp = new CharHealPriority();
          chhp.Char = ch;
          chhp.Damage = (ch.MaxHits - ch.Hits);
          chhp.Perc = (((decimal)(ch.MaxHits - ch.Hits) / (decimal)ch.MaxHits) * 100.0m) / (decimal)ch.MaxHits;
          chhp.DamagePerc = ((decimal)(ch.MaxHits - ch.Hits) / (decimal)ch.MaxHits) * 100.0m;
          chhp.HitsPercRelative = (decimal)ch.Hits / (decimal)World.Player.MaxHits;


          if (Healing.LastCharacter != null && Healing.LastCharacter.Serial == ch.Serial)
            chhp.Priority = 1000;

          if (Game.CurrentGame.CurrentHoverStatus != null && Game.CurrentGame.CurrentHoverStatus == ch.Serial)
            chhp.Priority = 900;

          if (ch.Serial == World.Player.Serial)
            chhp.Priority += 500;

          chhpList.Add(chhp);
        }
      }

      var sortedList = (from chhp in chhpList
                        orderby chhp.Priority descending, chhp.Perc descending
                        select chhp).ToList();

      return sortedList;
    }

    private List<Serial> skip = new List<Serial>();
    //---------------------------------------------------------------------------------------------

    [Executable]
    public UOCharacter IVMAuto(int maxdistance)
    {
      return IVMAuto(maxdistance, true);
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public UOCharacter IVMAuto(int maxdistance, bool useInMani)
    {
      return IVMAuto(maxdistance, useInMani, StandardSpell.GreaterHeal);
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public UOCharacter IVMAuto(int maxdistance, bool useInMani, StandardSpell secondarySpell)
    {
      return IVMAuto(maxdistance, useInMani, secondarySpell, true);
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public UOCharacter IVMAuto(int maxdistance, bool useInMani, StandardSpell secondarySpell, bool useScroolByHits)
    {
      return IVMAuto(maxdistance, useInMani, secondarySpell, true, StandardSpell.GreaterHeal);
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public UOCharacter IVMAuto(int maxdistance, bool useInMani, StandardSpell secondarySpell, bool useScroolByHits, StandardSpell primarySpell)
    {
      UOCharacter result = null;
      List<CharHealPriority> chhpList = GetCharHealPriorityList(maxdistance, false, Game.MergeLists<UOCharacter>(Game.CurrentGame.Alies, Game.CurrentGame.HealAlies));
      var sortedList = chhpList.Where(c => !skip.Contains(c.Char.Serial)).ToList();

      if (sortedList.Count == 0)
      {
        skip.Clear();
        sortedList = chhpList;
      }

      if (sortedList.Count > 0)
      {
        bool useScrool = false;
 
        foreach (CharHealPriority c in sortedList)
        {
          UOCharacter ch = result = c.Char;
          //if (ch.Serial != World.Player.Serial)
          //  LastCharacter = ch;

          if (String.IsNullOrEmpty(ch.Name))
          {
            ch.Click();
            Game.Wait(150);
          }

          CastResultInfo info = new CastResultInfo();
          string spellShortCut = "";
          if (ch.MaxHits - ch.Hits <= 25 && useInMani)
          {
            spellShortCut = "IM";
            info = Magery.Current.CastSpell(StandardSpell.Heal, ch, true, false, true);
          }
          else
          {
            if (useScroolByHits && (c.DamagePerc >= 35 || c.Char.MaxHits <= 90 && c.Damage >= 30 || ch.Serial == World.Player.Serial))
              useScrool = true;

            //TODO predelat
            info = Magery.Current.CastSpell(primarySpell, ch, useScrool, false, true, true);
            if (!info.Success)
            {
              if (secondarySpell == StandardSpell.Heal)
              {
                spellShortCut = "IM";
                info = Magery.Current.CastSpell(StandardSpell.Heal, ch, useScrool, false, true, false);
              }
              else if (secondarySpell == StandardSpell.GreaterHeal)
              {
                spellShortCut = "IVM";
                info = Magery.Current.CastSpell(StandardSpell.GreaterHeal, ch, false, false, true, false);
              }
            }
            else
              spellShortCut = primarySpell == StandardSpell.GreaterHeal ? "IVM" : "IM";
          }

          if (info.NoInLineOfSight)
          {
            skip.Add(ch.Serial);
          }
          else
          {
            ushort color = CalStatusMessage.Val_InfoColor;
            if (info.Usage == CastUsage.Scrool)
              color = 0x0048;

            ch.PrintMessage("[" + spellShortCut + "...]" + (useScrool ? " (S)" : ""), Game.GetAlieColorByHits(ch.Serial));
 
            bool reset = sortedList.Count == 1;

            if (!reset)
            {
              int skipCount = 0;
              foreach (CharHealPriority chp in sortedList)
              {
                if (skip.Contains(chp.Char.Serial))
                  skipCount++;
              }

              reset = skipCount == sortedList.Count;
            }

            if (reset)
              skip = new List<Serial>();
          }

          break;
        }
      }
      else
      {
        Game.PrintMessage("IVMA: " + Game.MergeLists<UOCharacter>(Game.CurrentGame.Alies, Game.CurrentGame.HealAlies).Count + " OK");
      }

      return result;
    }



    //---------------------------------------------------------------------------------------------

    [Executable]
    public UOCharacter IVMAutoB(int maxdistance, int hits)
    {
      UOCharacter result = null;
      List<CharHealPriority> chhpList = GetCharHealPriorityList(maxdistance, false, Game.MergeLists<UOCharacter>(Game.CurrentGame.Alies, Game.CurrentGame.HealAlies));
      var sortedList = chhpList.Where(c => !skip.Contains(c.Char.Serial)).ToList();

      if (sortedList.Count == 0)
      {
        skip.Clear();
        sortedList = chhpList;
      }

      if (sortedList.Count > 0)
      {
        foreach (CharHealPriority c in sortedList)
        {
          UOCharacter ch = result = c.Char;
          //if (ch.Serial != World.Player.Serial)
          //  LastCharacter = ch;

          if (String.IsNullOrEmpty(ch.Name))
          {
            ch.Click();
            Game.Wait(150);
          }

          CastResultInfo info = new CastResultInfo();
          string spellShortCut = "";

          double dhits = (double)hits;
          double dmg = ch.MaxHits - ch.Hits;
          double dmgRel = dmg / dhits;
          double mHitsRel = (double)ch.MaxHits / dhits;

          double dmgHitsRel = mHitsRel - dmgRel;

          UOItem scrools = World.Player.Backpack.AllItems.FindType(Magery.SpellScrool[StandardSpell.GreaterHeal]);

          if (dmgRel > 0.75 &&  dmgHitsRel <= 2.25 && scrools.Exist)
          {
            spellShortCut = "IVM";

            info = Bishop.CastBishopGreaterHeal(true, ch.Serial, true);
            if (info.Spell == StandardSpell.Heal)
              spellShortCut = "IM";
          }
          else if (dmg > 30)
          {
            spellShortCut = "IVM";
            info = Magery.Current.CastSpell(StandardSpell.GreaterHeal, ch, true, false, true);
          }
          else
          {
            spellShortCut = "IM";
            info = Magery.Current.CastSpell(StandardSpell.Heal, ch, true, false, true);
          }

          if (info.NoInLineOfSight)
          {
            skip.Add(ch.Serial);
          }
          else
          {
            ushort color = CalStatusMessage.Val_InfoColor;
            if (info.Usage == CastUsage.Scrool)
              color = Game.Val_Green;

            ch.PrintMessage("[" + spellShortCut + "...]", Game.GetAlieColorByHits(ch.Serial));
            
            bool reset = sortedList.Count == 1;

            if (!reset)
            {
              int skipCount = 0;
              foreach (CharHealPriority chp in sortedList)
              {
                if (skip.Contains(chp.Char.Serial))
                  skipCount++;
              }

              reset = skipCount == sortedList.Count;
            }

            if (reset)
              skip = new List<Serial>();
          }

          break;
        }
      }
      else
      {
        Game.PrintMessage("IVMA: " + Game.MergeLists<UOCharacter>(Game.CurrentGame.Alies, Game.CurrentGame.HealAlies).Count + " OK");
      }

      return result;
    }

    //---------------------------------------------------------------------------------------------

    public static List<UOCharacter> GetGhosts()
    {
      return World.Characters.Where(i => i.Serial != World.Player.Serial && i.Distance < 30 && (i.Model == 0x0192 || i.Model == 0x0193)).ToList();
    }

    //---------------------------------------------------------------------------------------------

    public static int GhostCount()
    {
      return GetGhosts().Count();
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static UOCharacter BandRessQuick()
    {
      return BandRessQuick("nearestghost");
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static  UOCharacter BandRessQuick(string target)
    {
      Game.PrintMessage("BandRessQuick - Ghosts: " + GhostCount());

      if (Healing.CleanBandage.Exist)
      {
        TargetInfo tInfo = Targeting.GetTarget(target);
        if (tInfo.Success)
        {
          if (tInfo.Character.Distance <= 1)
          {
            tInfo.Character.Print(Game.Val_GreenBlue, "[Ressuji..]");
            Healing.CleanBandage.Use();
            UO.WaitTargetObject(tInfo);
            LastCharacter = tInfo.Character;

            if (tInfo.Character.Distance <= 1)
              World.Player.PrintMessage("[Ress done " + Healing.CleanBandage.Amount + "..]");

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
        World.Player.PrintMessage("[Nemas bandage..]", MessageType.Error);

      return LastCharacter;
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static UOCharacter BandHealRess()
    {
      return BandHealRess("nearestghost");
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static UOCharacter BandHealRess(string target)
    {
      Game.PrintMessage("BandHealRess - Ghosts: " + GhostCount());

      if (Healing.CleanBandage.Exist)
      {
        TargetInfo tInfo = Targeting.GetTarget(target);
        if (tInfo.Success)
        {
          if (tInfo.Character.Distance <= 1)
          {
            tInfo.Character.Print(Game.Val_GreenBlue, "[Ressuji..]");

            CastResultInfo info = Magery.Current.CastSpell(StandardSpell.GreaterHeal, tInfo.Character, true, false);
            if (info.NotInSpellBook)
              info = Magery.Current.CastSpell(StandardSpell.Heal, tInfo.Character, false, false);

            Game.Wait(250);

            Healing.CleanBandage.Use();
            UO.WaitTargetObject(tInfo);
            LastCharacter = tInfo.Character;

            if (tInfo.Character.Distance <= 1)
              World.Player.PrintMessage("[Ress done " + Healing.CleanBandage.Amount + "..]");

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
        World.Player.PrintMessage("[Nemas bandage..]", MessageType.Error);

      return LastCharacter;
    }

    //---------------------------------------------------------------------------------------------

    public static HealInfo BandSafe(Serial serial)
    {
      return BandSafe(serial, false);
    }

    //---------------------------------------------------------------------------------------------

    public static HealInfo BandSafe(Serial serial, bool isAutoHeal)
    {
      HealInfo hInfo = new HealInfo();
      hInfo.HasBandages = Healing.CleanBandage.Exist;

      if (hInfo.HasBandages)
      {
        if (isAutoHeal && Game.CheckRunning())
          return hInfo;

        UOCharacter ch = new UOCharacter(serial);

        ch.PrintHitsMessage("[Banding...]");
        int ohits = new UOCharacter(serial).Hits;

        Game.RunScriptCheck(250);
        Journal.Clear();
        Targeting.ResetTarget();
        Healing.CleanBandage.Use();
        UO.WaitTargetObject(serial);

        DateTime start = DateTime.Now;
        //Vylecil jsi otravu!
        hInfo.Used = Journal.WaitForText(true, 3500, "You must be able to reach the target", "Chces vytvorit mumii?", "You put the bloody bandagess in your pack.", "You apply the bandages, but they barely help.", "Your target is already fully healed", "Vylecil jsi otravu!");
        int time = Convert.ToInt32((DateTime.Now - start).TotalMilliseconds);
        int wait = 2550 - time;

        if (Journal.Contains(true, "Nemuzes pouzit bandy na summona!"))
          hInfo.TryHealSummon = true;
        if (Journal.Contains(true, "You must be able to reach the target"))
          hInfo.CantReach = true;
        if (Journal.Contains(true, "Chces vytvorit mumii?") || Journal.Contains(true, "Your target is already fully healed"))
          hInfo.FullHealed = true;
        if (Journal.Contains(true, "Vylecil jsi otravu!"))
          hInfo.CuredPoison = true;

        hInfo.Success = Journal.Contains(true, "You put the bloody bandagess in your pack");
        hInfo.Failed = Journal.Contains(true, "You apply the bandages, but they barely help");

        if (hInfo.CuredPoison)
          wait = 250;

        wait += 1;

        if (wait > 0)
        {
          Game.Wait(wait);
        }

        if (hInfo.CantReach)
          ch.PrintMessage("[Can't reach Band]" + (serial == World.Player.Serial ? " Self" : ""), Game.Val_LightGreen);

        int hits = new UOCharacter(serial).Hits;
        int incr = hits - ohits;

        if (hInfo.Success && incr > 0)
          hInfo.Gain = incr;
      }

      return hInfo;
    }




    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void SnezSalat()
    {
      UOItem salat = new UOItem(Serial.Invalid);

      salat = World.Player.Backpack.AllItems.FindType(VAL_Salat.Graphic, VAL_Salat.Color);

      if ((World.Player.MaxHits - World.Player.Hits) > 35 || World.Player.Hits < 60 && World.Player.Hits < World.Player.MaxHits)
      {
        if (Guardian.ShieldOfAncients())
          Game.Wait(250);

        if ((World.Player.MaxHits - World.Player.Hits) > 35 || World.Player.Hits < 60 && World.Player.Hits < World.Player.MaxHits)
        {
          if (salat.Serial.IsValid && salat.Exist)
          {
            int startHits = World.Player.Hits;
            salat.Use();
            Game.Wait(500, true);
            int endHits = World.Player.Hits;

            World.Player.PrintMessage("[Salat " + (endHits - startHits) + "hp..]", endHits < World.Player.MaxHits ? MessageType.Warning : MessageType.Info);
          }
          else
          {
            World.Player.PrintMessage("[Neni Salat..]");
          }
        }
      }
      else
        World.Player.PrintMessage("Nemusis jist salat "  + salat.Amount);
    }

    //---------------------------------------------------------------------------------------------

  }


  //---------------------------------------------------------------------------------------------

  public class CharHealPriority
  {
    public UOCharacter Char;
    public decimal Perc;
    public int Zone = 0;
    public int Priority = 0;
    public int Damage = 0;
    public decimal DamagePerc = 0;
    public decimal HitsPercRelative = 0;
  }


  public class HealInfo
  {
    public bool Success;
    public bool HasBandages;
    public bool TryHealSummon = false;
    public bool CantReach = false;
    public bool FullHealed = false;
    public bool Used = false;
    public bool Failed = false;
    public int Gain = 0;
    public bool CuredPoison = false;
  }
}
