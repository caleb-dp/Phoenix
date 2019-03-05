using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;
using Caleb.Library;
using CalExtension.UOExtensions;
using CalExtension.PlayerRoles;
using Caleb.Library.CAL.Business;
using System.Collections;
using System.Timers;
using System.Threading;

namespace CalExtension.Skills
{
  [RuntimeObject]
  public class Magery : Skill
  {
    //0x3956  EnergyField
    public static Magery Current;

    public static CastSpellInfo CastingSpellInfo;

    protected static bool casting = false;
    public static bool Casting
    {
      get { return casting; }
      set
      {
        if (casting != value)
        {
          casting = value;

          if (casting)
          {
            Game.CheckStopBanding();
            if (Game.Debug)
              Game.PrintMessage("Casting ON");

            if (CastingSpellInfo != null)
            {

              if (CastingSpellInfo.IsScrool)
                Game.RunScript(3000);
              else
                Game.RunScript(Magery.GetCircleRunscriptTime(Magery.GetSpellCircle(CastingSpellInfo.Spell)));

              if (!CastingSpellInfo.Silence)
              {
                UOColor color = CastingSpellInfo.IsScrool ? Game.Val_Green : Game.Val_PureWhite;
                string message = String.Empty;

                if (CastingSpellInfo.Spell == StandardSpell.SummonCreature)
                {
                  if (!String.IsNullOrEmpty(Magery.CurrentCastingSummon))
                    message = Magery.CurrentCastingSummon + String.Empty;
                  else
                    message = GetSpellShorcut(CastingSpellInfo.Spell) + String.Empty;

                  Magery.CurrentCastingSummon = String.Empty;
                }
                else
                  message = GetSpellShorcut(CastingSpellInfo.Spell) + String.Empty + (CastingSpellInfo.IsScrool ? " " + World.Player.FindType(Magery.SpellScrool[CastingSpellInfo.Spell]).Amount + "" : "");


                if (CalebConfig.CastMessageType != MessagePrintType.None)
                {
                  if (CalebConfig.CastMessageType == MessagePrintType.Default)
                    World.Player.PrintMessage(message, color);
                  else
                    Game.PrintMessage(message, color);
                }
              }
            }
          }
          else
          {
            if (CastingSpellInfo != null)
            {
              if (!CastingSpellInfo.HasError)
              {
                OnCastingSuccess(CastingSpellInfo);
              }
              else
                OnCastingFailed(CastingSpellInfo);
            }

            CastingSpellInfo = null;
            if (Game.Debug)
              Game.PrintMessage("Casting OFF");
          }
        }
      }
    }

    public Magery()
    {
      this.OnInit();

    }

    //---------------------------------------------------------------------------------------------

    protected static void OnCastingSuccess(CastSpellInfo info)
    {
      if (info.Spell == StandardSpell.WallofStone || info.Spell == StandardSpell.EnergyField)//PF?
      {
        if (Game.Debug)
          Game.PrintMessage("Casting WallofStone SUCCESS");

        Game.Wait(250);

        if (CalebConfig.UseWallTime)
          WallTimeKeeper.TryFoundAddWall();
      }

      if (Game.Debug)
        Game.PrintMessage("Casting SUCCESS");
    }

    //---------------------------------------------------------------------------------------------

    protected static void OnCastingFailed(CastSpellInfo info)
    {
      if (Game.Debug)
        Game.PrintMessage("Casting FAILED");
    }

    //---------------------------------------------------------------------------------------------
    public static StandardSpell? LastCastedSpell = null;

    public static void TrySetCastingSpell(CastSpellInfo spellInfo)
    {
      try
      {
        if (CastingSpellInfo == null || CastingSpellInfo.CastRunDuration > 150)
          casting = false;

        if (!Casting)
        {
          LastCastedSpell = spellInfo.Spell;
          CastingSpellInfo = spellInfo;
          Casting = true;
        }
      }
      catch
      {
        Game.PrintMessage("TrySetCastingSpell - fail!", MessageType.Error);
      }
    }

    //---------------------------------------------------------------------------------------------

    public object SyncRoot;
    protected string selectedSummon;
    protected int selectedSummonIndex = -1;
    public string SelectedSummonSet
    {
      set
      {
        this.selectedSummon = value;

        UOColor color = 0x2bb;

        switch (selectedSummonIndex)
        {
          case 0:
            color = 0x0194;
            break;
          case 1:
            color = 0x01d0;
            break;
          case 2:
            color = 0x258;
            break;
          case 3:
            color = 0x01a8;
            break;
          case 4:
            color = 0x258;
            break;
          case 5:
            color = 0x01c6;
            break;
          case 6:
            color = 0x01e4;
            break;
          case 7:
            color = 0x30;
            break;
          default:
            break;
        }

        World.Player.PrintMessage(value, color);
      }
    }
    //---------------------------------------------------------------------------------------------

    protected string selectedSpell;
    protected string SelectedSpell
    {
      get { return this.selectedSpell; }
    }

    public string SelectedSpellSet
    {
      set
      {
        this.selectedSpell = value;
        World.Player.PrintMessage(value, Magery.GetSpellPrintColor(value));
      }
    }

    //---------------------------------------------------------------------------------------------

    protected void OnInit()
    {
      if (Current == null)
      {
        Current = this;
      }
      this.SyncRoot = new object();
    }

    //---------------------------------------------------------------------------------------------

    public void MoveSpellNext(int direction)
    {
      MoveSpellNext(direction, new List<StandardSpell>());
    }

    //---------------------------------------------------------------------------------------------

    public void MoveSpellNext(int direction, params string[] spells)
    {
      this.MoveSpellNext(direction, Magery.GetStandardSpells(spells));
    }

    //---------------------------------------------------------------------------------------------

    public void MoveSpellNext(int direction, List<StandardSpell> switchCol)
    {
      if (switchCol == null) switchCol = Magery.GetStandardSpells();//Nebo all
      int index = 0;
      try
      {
        StandardSpell sp;
        if (Magery.ParseSpell(this.SelectedSpell, out sp))
          index = UOExtensions.Utils.GetSwitchIndex(switchCol.IndexOf(sp), direction, switchCol.Count);
      }
      catch { index = 0; }
      this.SelectedSpellSet = switchCol[index].ToString();
    }

    //---------------------------------------------------------------------------------------------

    public void MoveSummonNext(int direction, params string[] summons)
    {
      List<string> list = new List<string>(summons);

      int index = 0;
      try
      {
        if (!String.IsNullOrEmpty(selectedSummon))
          index = UOExtensions.Utils.GetSwitchIndex(list.IndexOf(selectedSummon), direction, list.Count);
      }
      catch { index = 0; }
      selectedSummonIndex = index;
      this.SelectedSummonSet = list[index];
    }



    //---------------------------------------------------------------------------------------------

    public static int GetSpellCircle(StandardSpell spell)
    {
      int result = -1;

      if (spell == StandardSpell.CreateFood || spell == StandardSpell.MagicArrow || spell == StandardSpell.NightSight || spell == StandardSpell.Weaken || spell == StandardSpell.Clumsy || spell == StandardSpell.Feeblemind || spell == StandardSpell.Heal || spell == StandardSpell.ReactiveArmor)
        result = 1;
      else if (spell == StandardSpell.Agility || spell == StandardSpell.Cunning || spell == StandardSpell.Cure || spell == StandardSpell.Harm || spell == StandardSpell.MagicTrap || spell == StandardSpell.MagicUntrap || spell == StandardSpell.Protection || spell == StandardSpell.Strength)
        result = 2;
      else if (spell == StandardSpell.Bless || spell == StandardSpell.Fireball || spell == StandardSpell.MagicLock || spell == StandardSpell.Poison || spell == StandardSpell.Teleport || spell == StandardSpell.Unlock || spell == StandardSpell.WallofStone)
        result = 3;
      else if (spell == StandardSpell.ArchCure || spell == StandardSpell.ArchProtection || spell == StandardSpell.Curse || spell == StandardSpell.FireField || spell == StandardSpell.GreaterHeal || spell == StandardSpell.Lightning || spell == StandardSpell.ManaDrain || spell == StandardSpell.Recall)
        result = 4;
      else if (spell == StandardSpell.BladeSpirit || spell == StandardSpell.DispelField || spell == StandardSpell.Incognito || spell == StandardSpell.Reflection || spell == StandardSpell.MindBlast || spell == StandardSpell.Paralyze || spell == StandardSpell.PoisonField || spell == StandardSpell.SummonCreature)
        result = 5;
      else if (spell == StandardSpell.Dispel || spell == StandardSpell.EnergyBolt || spell == StandardSpell.Explosion || spell == StandardSpell.Invisibility || spell == StandardSpell.Mark || spell == StandardSpell.MassCurse || spell == StandardSpell.ParalyzeField || spell == StandardSpell.Reveal)
        result = 6;
      else if (spell == StandardSpell.ChainLightning || spell == StandardSpell.EnergyField || spell == StandardSpell.FlameStrike || spell == StandardSpell.GateTravel || spell == StandardSpell.ManaVampire || spell == StandardSpell.MassDispel || spell == StandardSpell.MeteorShower || spell == StandardSpell.Polymorph)
        result = 7;
      else if (spell == StandardSpell.EnergyVortex || spell == StandardSpell.Ressurection || spell == StandardSpell.SummonAirElemental || spell == StandardSpell.SummonDaemon || spell == StandardSpell.SummonEarthElemental || spell == StandardSpell.SummonFireElemental || spell == StandardSpell.SummonWaterElemental)
        result = 8;

      return result;
    }


    //---------------------------------------------------------------------------------------------

    public static int GetCircleRunscriptTime(int circle)
    {
      int result = 5500;

      if (circle <= 2)
        result = 2400;
      else if (circle <= 4)
        result = 3700;
      else if (circle <= 7)
        result = 4800;

      return result;
    }

    //---------------------------------------------------------------------------------------------

    protected double GetScroolTimeout()
    {
      SkillValue skv = SkillsHelper.GetSkillValue("Magery");
      if (skv.RealValue > 960)
        return 2.000;
      else if (skv.Value > 910)
        return 4.000;
      else if (skv.RealValue > 860)
        return 7.000;
      else if (skv.RealValue > 810)
        return 10.000;
      else if (skv.RealValue > 750)
        return 13.000;
      else if (skv.RealValue > 710)
        return 16.000;
      else if (skv.RealValue > 660)
        return 19.000;
      else if (skv.RealValue > 610)
        return 22.000;
      else
        return 25.000;
    }

    //---------------------------------------------------------------------------------------------

    public void SetCurrentSpellUsage(StandardSpell spell, CastUsage usage)
    {
      if (htSwitch == null)
        htSwitch = new Hashtable();

      htSwitch[spell] = usage;
    }

    //---------------------------------------------------------------------------------------------

    private static DateTime? lastScrool;
    private Hashtable htSwitch;

    //---------------------------------------------------------------------------------------------

    [Executable]
    public void CastLastSpell(string target)
    {
      if (LastCastedSpell.HasValue)
        this.CastSpell(LastCastedSpell.Value.ToString(), target, false, false);
      else
        World.Player.PrintMessage("[Not LastSpell...]", MessageType.Warning);
    }


    //---------------------------------------------------------------------------------------------

    public void CastSpell()
    {
      this.CastSpell(null);
    }

    //---------------------------------------------------------------------------------------------

    public void CastSpell(string target)
    {
      this.CastSpell(this.SelectedSpell, target);
    }

    //---------------------------------------------------------------------------------------------

    public void CastSpell(string spellName, string target)
    {
      this.CastSpell(spellName, target, false);
    }

    public void CastSpell(string spellName, string target, bool useScrool)
    {
      CastSpell(spellName, target, useScrool, false);
    }

    //---------------------------------------------------------------------------------------------

    public void CastSpell(string spellName, string target, bool useScrool, bool useSwitchHeadScrool)
    {
      CastSpell(spellName, target, useScrool, useSwitchHeadScrool, false, false);
    }

    //---------------------------------------------------------------------------------------------

    public void CastSpell(string spellName, string target, bool useScrool, bool useSwitchHeadScrool, bool silence)
    {
      CastSpell(spellName, target, useScrool, useSwitchHeadScrool, silence, false);
    }


    //---------------------------------------------------------------------------------------------

    public void CastSpell(string spellName, string target, bool useScrool, bool useSwitchHeadScrool, bool silence, bool forceScrool)
    {
      if (String.IsNullOrEmpty(spellName))
      {
        World.Player.PrintMessage("[Zvol kouzlo..]", MessageType.Error);
      }
      else
      {
        if (Necromancy.IsNecroSpell(spellName))
        {
          Necromancy.Current.CastNecroSpell(spellName, target);
        }
        else
        {
          StandardSpell sp;
          if (Magery.ParseSpell(spellName, out sp))
          {
            TargetAliasResult tr = Targeting.ParseTargets((target == null ? "" : target).Split(','));
            this.CastSpell(sp, tr, useScrool, useSwitchHeadScrool, silence, forceScrool);
          }
          else
            Game.PrintMessage("Cast: " + spellName + " - Neexistuje!!!");
        }
      }
    }

    //---------------------------------------------------------------------------------------------

    public void CastSpell(StandardSpell spell, Serial target)
    {
      this.CastSpell(spell, target, false, false);
    }

    //---------------------------------------------------------------------------------------------

    public CastResultInfo CastSpell(StandardSpell spell, TargetAliasResult target, bool useScrool, bool useSwitchHeadScrool)
    {
      return CastSpell(spell, target, useScrool, useSwitchHeadScrool, false);
    }

    //---------------------------------------------------------------------------------------------

    public CastResultInfo CastSpell(StandardSpell spell, TargetAliasResult target, bool useScrool, bool useSwitchHeadScrool, bool silence)
    {
      return CastSpell(spell, target, useScrool, useSwitchHeadScrool, silence, false);
    }

    //---------------------------------------------------------------------------------------------

    /// <summary>
    /// Obecne kouzleni hlava + svitky, svitky omezena podpora na to co je SpellScrool, co chybi napsat dodela. Nefunguje na nekro kouzla. Pokud je kouzlo vypsano nad hlavou zelene = SVITEK
    /// </summary>
    /// <param name="spell">Kouzlo</param>
    /// <param name="target">Cil = pokud Serial.Invalid resp. null tak vyhodi tercik</param>
    /// <param name="useScrool">Pouzij svitek, pokud je, jinak hlava</param>
    /// <param name="useSwitchHeadScrool">Stridej svitek a hlavu, tj. uchovava si ke kazdemu kouzlo co bylo kouzleno naposled</param>
    /// <param name="silence">Nevypise nad hlavu jmeno kouzla + lvl, pouziva se v kodu kde chcete si vypadt neco extra</param>
    /// <param name="forceScrool">Vynutit svite, pokud neni svitek tak nekouzli a v CastResiltInfu vrat FALSe</param>
    /// <returns></returns>
    public CastResultInfo CastSpell(StandardSpell spell, TargetAliasResult target, bool useScrool, bool useSwitchHeadScrool, bool silence, bool forceScrool)
    {
      Game.CheckStopBanding();
      Targeting.ResetTarget();
      Journal.Clear();

      CastResultInfo info = new CastResultInfo();
      info.Usage = useScrool ? CastUsage.Scrool : CastUsage.Head;

      if (htSwitch == null)
        htSwitch = new Hashtable();

      int circle = GetSpellCircle(spell);

      UOCharacter ch = new UOCharacter(target);
      if (ch.Exist && String.IsNullOrEmpty(ch.Name))
      {
        ch.Click();
        Game.Wait(50, true);
      }

      if (spell == StandardSpell.SummonCreature && !String.IsNullOrEmpty(this.selectedSummon))
      {
        Game.RunScript(5000);
        this.CastSummonCreature(this.selectedSummon, target);
      }
      else
      {

        if (htSwitch[spell] == null)
          htSwitch[spell] = info.Usage;
        else if (useSwitchHeadScrool && !forceScrool)
        {
          CastUsage currentUsage = (CastUsage)htSwitch[spell];
          if (currentUsage == CastUsage.Head)
            info.Usage = CastUsage.Scrool;
          else
            info.Usage = CastUsage.Head;
        }

        bool waitForScrool = false;
        if (info.Usage == CastUsage.Scrool && lastScrool.HasValue)
        {
          double currentTimeout = GetScroolTimeout();
          double currentTime = (DateTime.Now - lastScrool.Value).TotalSeconds;

          if (currentTime < currentTimeout)
          {
            waitForScrool = true;
            if (!forceScrool)
              info.Usage = CastUsage.Head;
            Game.PrintMessage(String.Format("Scroll za! {0:N1}s", currentTimeout - currentTime));
          }
        }

        //TODO zjistit Flag na jsem parnutej
        if (info.Usage == CastUsage.Scrool && !waitForScrool)
        {
          if (SpellScrool.ContainsKey(spell) && World.Player.Backpack.AllItems.FindType(SpellScrool[spell]).Exist)
          {
            if (!target.IsStatic && (!target.IsValid || !new UOObject(target).Exist))
            {
              Magery.TrySetCastingSpell(new CastSpellInfo(spell, true, silence));
              World.Player.Backpack.AllItems.FindType(SpellScrool[spell]).Use();

              if (!Journal.WaitForText(true, 250, "You can't reach that", "You can't cast", "You can't read that"))
              {
                lastScrool = DateTime.Now.AddMilliseconds(-250);
                info.Usage = CastUsage.Scrool;
                info.Success = true;
              }
              else if (!forceScrool)
                info.Usage = CastUsage.Head;
            }
            else
            {
              Magery.TrySetCastingSpell(new CastSpellInfo(spell, true, silence));

              target.WaitTarget();
              //UO.WaitTargetObject(target);
              World.Player.Backpack.AllItems.FindType(SpellScrool[spell]).Use();

              if (!Journal.WaitForText(true, 250, "You can't reach that", "You can't cast", "You can't read that"))
              {
                lastScrool = DateTime.Now.AddMilliseconds(-250);
                info.Usage = CastUsage.Scrool;
                info.Success = true;
              }
              else if (!forceScrool)
                info.Usage = CastUsage.Head;
            }
          }
          else if (!forceScrool)
            info.Usage = CastUsage.Head;
        }

        ushort color = CalStatusMessage.Val_InfoColor;
        //if (info.Usage == CastUsage.Scrool)
        //{
        //  color = Game.Val_Green;
        //  if (!silence)
        //    World.Player.PrintMessage(spell + " [" + World.Player.Backpack.AllItems.FindType(SpellScrool[spell]).Amount + "ks]", color);
        //}

        htSwitch[spell] = info.Usage;

        if (info.Usage == CastUsage.Head)
        {
          //Game.RunScript(Magery.GetCircleRunscriptTime(Magery.GetSpellCircle(spell)));

          if (!target.IsStatic && (!target.IsValid || !new UOObject(target).Exist || new UOObject(target).Distance > 30))
          {
            Magery.TrySetCastingSpell(new CastSpellInfo(spell, false, silence));
            UO.Cast(spell);
          }
          else
          {
            Magery.TrySetCastingSpell(new CastSpellInfo(spell, false, silence));
            target.WaitTarget();
            UO.Cast(spell);//, target);

            if (Journal.WaitForText(true, 150, "You can't see the target", "Target is not in line of sight"))
            {
              if (Journal.Contains(true, "You can't see the target"))
              {
                info.CantSee = true;

                Game.PrintMessage("Cant SEE: " + target + " / " + new UOCharacter(target).Exist + " / " + new UOCharacter(target).Name);

                info.Success = CastSpell(spell, Serial.Invalid, useScrool, useSwitchHeadScrool, silence, forceScrool).Success;
              }
              else if (Journal.Contains(true, "Target is not in line of sight"))
              {
                info.NoInLineOfSight = true;

                new UOObject(target).PrintMessage("[Not in sight]", Game.Val_LightPurple);

              }
            }
            else
            {
              if (ch.Exist && !silence &&
                (spell == StandardSpell.Harm ||
                spell == StandardSpell.FlameStrike ||
                spell == StandardSpell.MagicArrow ||
                spell == StandardSpell.Lightning ||
                spell == StandardSpell.Clumsy ||
                spell == StandardSpell.Curse ||
                spell == StandardSpell.EnergyBolt ||
                spell == StandardSpell.Feeblemind ||
                spell == StandardSpell.MindBlast ||
                spell == StandardSpell.Paralyze)
                )

                if (target != World.Player.Serial && target.IsValid)
                  new UOObject(target).PrintMessage(String.Format("[{0}/{1}]", ch.Hits, ch.MaxHits), ch.Notoriety == Notoriety.Murderer || ch.Notoriety == Notoriety.Enemy ? Game.GetEnemyColorByHits(target) : Game.GetAlieColorByHits(target));
            }
          }

          if (Journal.WaitForText(true, 100, "The spell is not in your spellbook"))
          {
            info.Success = false;
            info.NotInSpellBook = true;
            Targeting.ResetTarget();
          }
          else if (!info.CantReach)
          {
            info.Success = true;
          }
        }
        else
        {
          Game.PrintMessage(spell + ": " + World.Player.Backpack.AllItems.FindType(SpellScrool[spell]).Amount + "");
        }
      }


      return info;
    }

    //---------------------------------------------------------------------------------------------

    public void SetCastSummon(string defaultSummon, string target)
    {
      if (String.IsNullOrEmpty(this.selectedSummon))
        this.selectedSummon = defaultSummon;

      this.CastSummonCreature(this.selectedSummon, Targeting.ParseTargets(target));
    }

    //---------------------------------------------------------------------------------------------

    public void CastSummonCreature(string target)
    {
      this.CastSummonCreature(this.selectedSummon, Targeting.ParseTargets(target));
    }

    //---------------------------------------------------------------------------------------------

    public void CastSummonCreature(string summonName, string target)
    {
      this.CastSummonCreature(summonName, Targeting.ParseTargets(target));
    }

    //---------------------------------------------------------------------------------------------

    public static string CurrentCastingSummon = String.Empty;
    public void CastSummonCreature(string summonName, TargetAliasResult target)
    {
      Game.RunScript(5000);
      Targeting.ResetTarget();

      CurrentCastingSummon = summonName;
      //World.Player.PrintMessage(summonName);
      Journal.Clear();

      if (!target.IsValid || !new UOObject(target).Exist)
        target = Game.CurrentGame.CurrentHoverStatus;

      if (!target.IsStatic && (!target.IsValid || !new UOObject(target).Exist || new UOObject(target).Distance > 25))
        UO.SummonCreature(summonName);
      else
      {
        UIManager.WaitForMenu(new MenuSelection("What do you want to summon ?", summonName));

        target.WaitTarget();

        UO.Cast("Summ. Creature");

        //target.WaitTarget();
        //UO.SummonCreature(summonName);//, target);

        if (Journal.WaitForText(true, 150, "You can't see the target", "Target is not in line of sight"))
        {
          if (Journal.Contains(true, "You can't see the target"))
          {
            Game.PrintMessage("Cant SEE: " + target + " / " + new UOCharacter(target).Exist + " / " + new UOCharacter(target).Name);
          }
          else if (Journal.Contains(true, "Target is not in line of sight"))
          {
            UO.PrintObject(target, Game.Val_LightPurple, "[Not in sight]");
          }
        }
      }
    }

    //---------------------------------------------------------------------------------------------

    public static List<StandardSpell> GetStandardSpells(params string[] spellNames)
    {
      List<StandardSpell> list = new List<StandardSpell>();

      if (spellNames.Length == 0 || (spellNames.Length == 1 && spellNames[0].ToLower() == "all"))
      {
        foreach (int val in Enum.GetValues(typeof(StandardSpell)))
          list.Add((StandardSpell)val);
      }
      else 
      {
        foreach (string name in spellNames)
        {
          StandardSpell spell;
          if (Magery.ParseSpell(name.Trim(), out spell) && !list.Contains(spell))
            list.Add(spell);
        }
      }
      return list;
    }

    //---------------------------------------------------------------------------------------------

    public static bool ParseSpell(string spellName, out StandardSpell resultSpell)
    {
      resultSpell = StandardSpell.Unlock;//Misto INVALID

      try
      {
        resultSpell = (StandardSpell)Enum.Parse(typeof(StandardSpell), spellName);
      }
      catch { resultSpell = StandardSpell.Unlock; }

      if (resultSpell == StandardSpell.Unlock)
        return false;
      else
        return true;
    }

    //---------------------------------------------------------------------------------------------

    public static UOColor GetSpellPrintColor(string spellName)
    {
      StandardSpell sp;
      if (Magery.ParseSpell(spellName, out sp) && Magery.SpellPrinColor.ContainsKey(sp))
        return Magery.SpellPrinColor[sp];

      return new UOColor(0x096);
    }

    //---------------------------------------------------------------------------------------------

    private static Dictionary<StandardSpell, UOColor> SpellPrinColor//TODO vyrobit barvy pro vsechny spelly
    {
      get
      {
        Dictionary<StandardSpell, UOColor> dict = new Dictionary<StandardSpell, UOColor>();
        dict.Add(StandardSpell.Teleport, new UOColor(0x0194));
        dict.Add(StandardSpell.Dispel, new UOColor(0x0199));
        dict.Add(StandardSpell.DispelField, new UOColor(0x019e));
        dict.Add(StandardSpell.EnergyBolt, new UOColor(0x01a3));
        dict.Add(StandardSpell.EnergyVortex, new UOColor(0x01a8));
        dict.Add(StandardSpell.Harm, new UOColor(0x258));
        dict.Add(StandardSpell.ChainLightning, new UOColor(0x01b2));
        dict.Add(StandardSpell.Lightning, new UOColor(0x21));
        dict.Add(StandardSpell.MagicArrow, new UOColor(0x01b7));
        dict.Add(StandardSpell.MassCurse, new UOColor(0x01bc));
        dict.Add(StandardSpell.MassDispel, new UOColor(0x01c1));
        dict.Add(StandardSpell.MindBlast, new UOColor(0x01c6));
        dict.Add(StandardSpell.NightSight, new UOColor(0x01cb));
        dict.Add(StandardSpell.Paralyze, new UOColor(0x01d0));
        dict.Add(StandardSpell.ParalyzeField, new UOColor(0x01d5));
        dict.Add(StandardSpell.Reflection, new UOColor(0x01da));
        dict.Add(StandardSpell.Ressurection, new UOColor(0x01df));
        dict.Add(StandardSpell.Reveal, new UOColor(0x01e4));
        dict.Add(StandardSpell.WallofStone, new UOColor(0x01e9));
        dict.Add(StandardSpell.EnergyField, new UOColor(0x03e5));
        dict.Add(StandardSpell.FlameStrike, new UOColor(0x30));

        return dict;
      }
    }

    //---------------------------------------------------------------------------------------------

    public static Dictionary<StandardSpell, Graphic> SpellScrool
    {
      get
      {
        Dictionary<StandardSpell, Graphic> dict = new Dictionary<StandardSpell, Graphic>();
        dict.Add(StandardSpell.WallofStone, 0x1F44);
        dict.Add(StandardSpell.EnergyField, 0x1F5E);
        dict.Add(StandardSpell.Teleport, 0x1F42);
        dict.Add(StandardSpell.Lightning, 0x1F4A);
        dict.Add(StandardSpell.SummonEarthElemental, 0x1F6A);
        dict.Add(StandardSpell.Reflection, 0x1F50);
        dict.Add(StandardSpell.Paralyze, 0x1F52);
        dict.Add(StandardSpell.Protection, 0x1F3B);
        dict.Add(StandardSpell.Strength, 0x1F3C);
        dict.Add(StandardSpell.Dispel, 0x1F55);
        dict.Add(StandardSpell.BladeSpirit, 0x1F4D);
        dict.Add(StandardSpell.Ressurection, 0x1F67);
        dict.Add(StandardSpell.GreaterHeal, 0x1F49);
        dict.Add(StandardSpell.Heal, 0x1F31);
        dict.Add(StandardSpell.FlameStrike, 0x1F5F);
        return dict;
      }
    }


    //---------------------------------------------------------------------------------------------

    public static StandardSpell GetSpellFromScroll(UOItem item)
    {
      return GetSpellFromScroll(item.Graphic);
    }

    //---------------------------------------------------------------------------------------------

    public static StandardSpell GetSpellFromScroll(Graphic graphic)
    {
      foreach (KeyValuePair<StandardSpell, Graphic> kvp in SpellScrool)
      {
        if (kvp.Value == graphic && kvp.Key != StandardSpell.Unlock)
          return kvp.Key;
      }
      return StandardSpell.Unlock;
    }

    //---------------------------------------------------------------------------------------------

    public static bool IsSpellScroll(UOItem item)
    {
      return IsSpellScroll(item.Graphic);
    }

    //---------------------------------------------------------------------------------------------

    public static bool IsSpellScroll(Graphic graphic)
    {
      return GetSpellFromScroll(graphic) != StandardSpell.Unlock;
    }

    //---------------------------------------------------------------------------------------------

    public void TrainMagery()
    {
      Game.PrintMessage("TrainMagery");
      Game.CurrentGame.Mode = GameMode.Working;
      while (!UO.Dead &&
        World.Player.Backpack.AllItems.FindType(Reagent.Nightshade.Graphic, Reagent.Nightshade.Color).Exist && 
        World.Player.Backpack.AllItems.FindType(Reagent.BlackPearl.Graphic, Reagent.BlackPearl.Color).Exist 
        )
      {
        while (World.Player.Mana > 10)
        {
          if (World.Player.Hits < 40)
          {
            while (World.Player.Hits < World.Player.Strenght)
            {
              UOItem banda = World.Ground.FindType(0x0E21);//bandy
              if (!banda.Exist)
                return;

              UO.WaitTargetSelf();
              banda.Use();
              Game.Wait(2500);
            }
          }

          UO.Cast(StandardSpell.MagicArrow, Aliases.Self);
          Game.Wait(1500);
        }

        while (World.Player.Hits < World.Player.Strenght)
        {
          UOItem banda = World.Ground.FindType(0x0E21);//bandy
          if (!banda.Exist)
            break;

          UO.WaitTargetSelf();
          banda.Use();
          Game.Wait(2500);
        }


        UOItem mrKad = new UOItem(World.Player.Backpack.Items.FindType(Potion.KadGraphic, Potion.ManaRefresh.TopKadColor));
        if (!mrKad.Exist)
          mrKad = World.Ground.FindType(Potion.KadGraphic, Potion.ManaRefresh.TopKadColor);

        if (!mrKad.Exist)
          mrKad = World.Ground.FindType(Potion.KadGraphic, Potion.TotalManaRefresh.TopKadColor);
        //       UOItem tmrKad = new UOItem(World.Player.Backpack.Items.FindType(Potion.KadGraphic, Potion.TotalManaRefresh.TopKadColor));
        if (mrKad.Exist)
        {
          Game.CurrentGame.CurrentPlayer.GetSkillInstance<Alchemy>().DrinkPotion(Potion.ManaRefresh);
          Game.Wait();
        }
        else
        {
          while (World.Player.Mana < World.Player.Intelligence)
          {
            UO.UseSkill(StandardSkill.Meditation);
            Game.Wait(2500);

          }
        }
      }
      Game.PrintMessage("TrainMagery END");
    }

    //---------------------------------------------------------------------------------------------

    public static bool WaitForSpell(int timeout)
    {
      int counter = 0;
      bool isCasting = Magery.Casting;

      while (counter <= timeout)
      {
        Thread.Sleep(5);
        counter += 5;

        if (isCasting != Magery.Casting)
          return true;
      }

      return false;
    }


    //---------------------------------------------------------------------------------------------

    #region exec

    [Executable("TrainMagery")]
    [BlockMultipleExecutions]
    public static void ExecTrainMagery()
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Magery>().TrainMagery();
    }

    //---------------------------------------------------------------------------------------------

    [Executable("CastSummonCreature")]
    [BlockMultipleExecutions]
    public static void ExecCastSummonCreature(string summonName, string target)
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Magery>().CastSummonCreature(summonName, target);
    }

    //---------------------------------------------------------------------------------------------

    [Executable("SetCastSummon")]
    [BlockMultipleExecutions]
    public static void ExecSetCastSummon(string defaultSummon)
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Magery>().SetCastSummon(defaultSummon, null);
    }

    //---------------------------------------------------------------------------------------------

    [Executable("SetCastSummon")]
    [BlockMultipleExecutions]
    public static void ExecSetCastSummon(string defaultSummon, string target)
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Magery>().SetCastSummon(defaultSummon, target);
    }

    //---------------------------------------------------------------------------------------------

    [Executable("CastSpell")]
    public static void ExecCastSpell(string summonName, string target)
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Magery>().CastSpell(summonName, target);
    }


    //---------------------------------------------------------------------------------------------

    [Executable("CastSpell")]
    public static void ExecCastSpell(string summonName, string target, bool useScrool)
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Magery>().CastSpell(summonName, target, useScrool);
    }

    //---------------------------------------------------------------------------------------------

    [Executable("CastSpell")]
    public static void ExecCastSpell(string summonName, string target, bool useScrool, bool useSwitchHeadScrool)
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Magery>().CastSpell(summonName, target, useScrool, useSwitchHeadScrool);
    }


    //---------------------------------------------------------------------------------------------

    [Executable("CastSpell")]
    public static void ExecCastSpell(string summonName, string target, bool useScrool, bool useSwitchHeadScrool, bool forceScrool)
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Magery>().CastSpell(summonName, target, useScrool, useSwitchHeadScrool, false, forceScrool);
    }

    //---------------------------------------------------------------------------------------------

    [Executable("CastSpell")]
    public static void ExecCastSpell(string target)
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Magery>().CastSpell(target);
    }

    //---------------------------------------------------------------------------------------------

    [Executable("CastSpell")]
    public static void ExecCastSpell()
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Magery>().CastSpell();
    }

    //---------------------------------------------------------------------------------------------

    [Executable("MoveSpellNext")]
    public static void ExecMoveSpellNext(params string[] spells) 
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Magery>().MoveSpellNext(1, spells);
    }

    //---------------------------------------------------------------------------------------------

    [Executable("MoveSpellBack")]
    public static void ExecMoveSpellBack(params string[] spells) 
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Magery>().MoveSpellNext(-1, spells);
    }

    //---------------------------------------------------------------------------------------------

    [Executable("MoveSummonNextManual")]
    public static void ExecMoveSummonNext(params string[] summons)
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Magery>().MoveSummonNext(1, summons);
    }

    //---------------------------------------------------------------------------------------------

    [Executable("MoveSummonBackManual")]
    public static void ExecMoveSummonBack(params string[] summons)
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Magery>().MoveSummonNext(-1, summons);
    }


    #endregion

    //---------------------------------------------------------------------------------------------


    public static string GetSpellShorcut(StandardSpell spell)
    {
      if (spell == StandardSpell.WallofStone)
        return "WoS";
      else if (spell == StandardSpell.EnergyField)
        return "EF";
      else if (spell == StandardSpell.DispelField)
        return "DisFld";
      else if (spell == StandardSpell.EnergyVortex)
        return "EngVort";
      else if (spell == StandardSpell.ChainLightning)
        return "ChainLight";
      else if (spell == StandardSpell.SummonAirElemental)
        return "AirElm";
      else if (spell == StandardSpell.SummonDaemon)
        return "Daemon";
      else if (spell == StandardSpell.SummonEarthElemental)
        return "EarthElm";
      else if (spell == StandardSpell.SummonFireElemental)
        return "FireElm";
      else if (spell == StandardSpell.SummonWaterElemental)
        return "WaterElm";
      else if (spell == StandardSpell.ParalyzeField)
        return "ParaFld";
      else if (spell == StandardSpell.GreaterHeal)
        return "GHeal";
      else if (spell == StandardSpell.Paralyze)
        return "Para";
      else if (spell == StandardSpell.FlameStrike)
        return "KVF";
      else if (spell == StandardSpell.Lightning)
        return "POG";

      return spell.ToString();
    }

  }

  public enum CastUsage
  {
    Head = 1,
    Scrool = 2
  }



  public class CastResultInfo
  {
    public CastUsage Usage;
    public bool Success = false;
    public bool NotInSpellBook = false;
    public bool CantSee = false;
    public bool NoInLineOfSight = false;
    public bool CantReach { get { return CantSee || NoInLineOfSight;  } }
    public StandardSpell Spell;

  }

}
