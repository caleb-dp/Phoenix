using System;
using System.Linq;
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

namespace CalExtension.Skills
{
  [RuntimeObject]
  public class Necromancy : Skill
  {
    public static Necromancy Current;
    public object SyncRoot;


    //---------------------------------------------------------------------------------------------

    public static UOItem BookOfDead
    {
      get { return World.Player.FindType(0x0EFA, 0x0455); }
    }

    //---------------------------------------------------------------------------------------------

    public Necromancy()
    {
      this.OnInit();
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

    public void CastNecroSpell(string spellName, string target)
    {
      NecromancySpell sp;
      if (Necromancy.ParseSpell(spellName, out sp))
      {
        CastNecroSpell(sp, target);
      }
      else
        World.Player.PrintMessage("[Neexistuje " + spellName + "]", MessageType.Warning);
    }

    //---------------------------------------------------------------------------------------------

    //TODO
    public CastResultInfo CastNecroSpell(NecromancySpell spell, string target)
    {
      Journal.Clear();
      CastResultInfo info = new CastResultInfo();
      info.Usage = CastUsage.Scrool;
      UOItem scroll = EnsureNecroScroll(spell);
      if (scroll.Exist)
      {
        World.Player.PrintMessage(spell + "" /*+  " [" + World.Player.FindType(SpellScrool[spell]).Amount + "ks]"*/);//spis regy
        Game.RunScriptCheck(5000);

        TargetInfo tInfo = Targeting.GetTarget(target);

        if (tInfo.Success)
        {
          scroll.Use();
          if (tInfo.Object.Exist)
            UO.WaitTargetObject(tInfo);
          else
            UO.WaitTargetTile(tInfo.StaticTarget.X, tInfo.StaticTarget.Y, tInfo.StaticTarget.Z, tInfo.StaticTarget.Graphic);

          if (Journal.WaitForText(true, 150, "You can't see the target", "Target is not in line of sight"))
          {
            if (Journal.Contains(true, "You can't see the target"))
            {
              info.CantSee = true;
              Game.PrintMessage("Cant SEE: " + tInfo.StaticTarget.Serial + " / " + tInfo.Object.Exist + " / " + tInfo.Character.Name);
            }
            else if (Journal.Contains(true, "Target is not in line of sight"))
            {
              info.NoInLineOfSight = true;
              tInfo.Object.PrintMessage("[Not in sight]", Game.Val_LightPurple);
            }
          }
        }
      }

      return info;
    }

    //Serial: 0x40213311  Name: "Fire Bolt scroll"  Position: 66.145.0  Flags: 0x0020  Color: 0x0070  Graphic: 0x0E35  Amount: 100  Layer: None Container: 0x4038402B
    //Serial: 0x40211101  Name: "Clumsy scroll"  Position: 66.68.0  Flags: 0x0020  Color: 0x0079  Graphic: 0x0E35  Amount: 100  Layer: None Container: 0x4038402B
    //Serial: 0x40218569  Name: "Necromancer's Armor scroll"  Position: 75.65.0  Flags: 0x0000  Color: 0x0040  Graphic: 0x0E35  Amount: 100  Layer: None Container: 0x4038402B
    //Serial: 0x40000C72  Name: "Curse scroll"  Position: 97.89.0  Flags: 0x0000  Color: 0x0099  Graphic: 0x0E35  Amount: 100  Layer: None Container: 0x4038402B
    //Serial: 0x4020FEF9  Name: "Hallucination scroll"  Position: 101.70.0  Flags: 0x0020  Color: 0x0010  Graphic: 0x0E35  Amount: 100  Layer: None Container: 0x4038402B
    //Serial: 0x4021E770  Name: "Dark scroll"  Position: 138.90.0  Flags: 0x0020  Color: 0x0050  Graphic: 0x0E35  Amount: 100  Layer: None Container: 0x4038402B
    //Serial: 0x40349B3E  Name: "Summon Undead scroll"  Position: 137.74.0  Flags: 0x0000  Color: 0x0005  Graphic: 0x0E35  Amount: 274  Layer: None Container: 0x4038402B
    //Serial: 0x402730C5  Position: 137.65.0  Flags: 0x0000  Color: 0x0020  Graphic: 0x0E35  Amount: 248  Layer: None Container: 0x4038402B

    //---------------------------------------------------------------------------------------------

    private static UOItem EnsureNecroScroll(NecromancySpell spell)
    {
      UOItem result = World.Player.FindType(SpellScrool[spell]);

      if (!result.Exist)
      {
        List<UOItem> scrools = World.Player.Backpack.Items.Where(i => i.Graphic == 0x0E35).ToList();//new List<UOItem>();
        List<UOItem> exactScrool =
          scrools.Where(i => !String.IsNullOrEmpty(i.Name)
          && i.Name.ToLower().Replace(" ", "").StartsWith(Enum.GetName(typeof(NecromancySpell), spell).ToLower())).ToList();

        if (exactScrool.Count > 0)
        {
          result = exactScrool[0];
        }
        else
        {
          foreach (UOItem possibleItem in scrools.Where(i => String.IsNullOrEmpty(i.Name)).ToArray())
          {
            possibleItem.Click();
            Game.Wait(100);

            if (!String.IsNullOrEmpty(possibleItem.Name))
            {
              if (possibleItem.Name.ToLower().Replace(" ", "").StartsWith(Enum.GetName(typeof(NecromancySpell), spell).ToLower()))//Nefunguje na nekro armor
              {
                result = possibleItem;
                break;
              }
            }
          }
        }
      }

      if (!result.Exist)
      {
        if (BookOfDead.Exist)
        {
          Kniha.Current.DeadBookUse((uint)spell);
          Game.Wait(250);
          result = World.Player.FindType(SpellScrool[spell]);
          
          if (result.Exist)
          {
            result.Move(result.Amount, World.Player.Backpack.Serial);
            Game.Wait(250); 
          }
        }
        else
          World.Player.PrintMessage("[Nemas Book of Dead..]", MessageType.Warning);
      }

      return result;
    }

    //---------------------------------------------------------------------------------------------

    public static Dictionary<NecromancySpell, IUOItemType> SpellScrool
    {
      get
      {
        Dictionary<NecromancySpell, IUOItemType> dict = new Dictionary<NecromancySpell, IUOItemType>();
        dict.Add(NecromancySpell.SummonUndead, new UOItemTypeBase(0x0E35, 0x0005));
        dict.Add(NecromancySpell.AnimateDead, new UOItemTypeBase(0x0E35, 0x0020));
        dict.Add(NecromancySpell.NecroArmor, new UOItemTypeBase(0x0E35, 0x0040));
        dict.Add(NecromancySpell.Dark, new UOItemTypeBase(0x0E35, 0x0050));
        dict.Add(NecromancySpell.FireBolt, new UOItemTypeBase(0x0E35, 0x0070));
        dict.Add(NecromancySpell.Hallucination, new UOItemTypeBase(0x0E35, 0x0010));
        dict.Add(NecromancySpell.Clumsy, new UOItemTypeBase(0x0E35, 0x0079));
        dict.Add(NecromancySpell.Curse, new UOItemTypeBase(0x0E35, 0x0099));
        dict.Add(NecromancySpell.Invalid, new UOItemTypeBase(0x0000, 0x0000));

        return dict;
      }
    }

    //---------------------------------------------------------------------------------------------

    public static NecromancySpell GetNecromancySpellFromScroll(UOItem item)
    {
      return GetNecromancySpellFromScroll(item.Graphic, item.Color);
    }


    //---------------------------------------------------------------------------------------------

    public static NecromancySpell GetNecromancySpellFromScroll(Graphic graphic, UOColor color)
    {
      foreach (KeyValuePair<NecromancySpell, IUOItemType> kvp in SpellScrool)
      {
        if (kvp.Value.Graphic == graphic && kvp.Value.Color == color && kvp.Key != NecromancySpell.Invalid)
          return kvp.Key;
      }
      return NecromancySpell.Invalid;
    }

    //---------------------------------------------------------------------------------------------

    public static bool IsNecroSpellScroll(UOItem item)
    {
      return IsNecroSpellScroll(item.Graphic, item.Color);
    }

    //---------------------------------------------------------------------------------------------

    public static bool IsNecroSpellScroll(Graphic graphic, UOColor color)
    {
      return GetNecromancySpellFromScroll(graphic, color) != NecromancySpell.Invalid;
    }

    //---------------------------------------------------------------------------------------------

    public static bool IsNecroSpell(string spellName)
    {
      NecromancySpell dummy;
      return ParseSpell(spellName, out dummy);
    }

    //---------------------------------------------------------------------------------------------

    public static bool ParseSpell(string spellName, out NecromancySpell resultSpell)
    {
      resultSpell = NecromancySpell.Invalid;

      spellName.Replace("Necro_", "");

      try
      {
        resultSpell = (NecromancySpell)Enum.Parse(typeof(NecromancySpell), spellName);
      }
      catch { resultSpell = NecromancySpell.Invalid; }

      if (resultSpell == NecromancySpell.Invalid)
        return false;
      else
        return true;
    }

    //---------------------------------------------------------------------------------------------

    [Executable("CastNecroSpell")]
    public static void ExecCastNecroSpell(string spellName, string target)
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Necromancy>().CastNecroSpell(spellName, target);
    }

    //---------------------------------------------------------------------------------------------


  }

  public enum NecromancySpell
  {
    Invalid = 0,
    SummonUndead = 1,
    AnimateDead = 2,
    NecroArmor = 3,
    Dark = 4,
    FireBolt = 5,
    Hallucination = 6,
    Clumsy = 7,
    Curse = 8
  }


}
