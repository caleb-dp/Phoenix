using Caleb.Library;
using Caleb.Library.CAL.Business;
using CalExtension.Abilities;
using CalExtension.PlayerRoles;
using CalExtension.Skills;
using CalExtension.UI.Status;
using CalExtension.UOExtensions;
using Phoenix;
using Phoenix.Communication;
using Phoenix.Communication.Packets;
using Phoenix.Macros;
using Phoenix.Runtime;
using Phoenix.WorldData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;

//Serial: 0x402B9CFC  Name: "Bezpecna Truhla"  Position: 3349.344.10  Flags: 0x0000  Color: 0x0B1C  Graphic: 0x0E41  Amount: 0  Layer: None Container: 0x00000000
namespace CalExtension
{
  [RuntimeObject]
  public class Game
  {
    public static UOColor Val_LightGreen { get { return CalebConfig.GetUOColorOrDefault("Color_LightGreen", 0x0053); } } //= 0x0053;
    public static UOColor Val_Green  { get { return CalebConfig.GetUOColorOrDefault("Color_Green",  0x0048); } }
    public static UOColor Val_GreenBlue  { get { return CalebConfig.GetUOColorOrDefault("Color_GreenBlue",  0x00B3); } }
    public static UOColor Val_UltraGreen  { get { return CalebConfig.GetUOColorOrDefault("Color_UltraGreen",  0x055A); } }
    public static UOColor Val_SuperLightYellow  { get { return CalebConfig.GetUOColorOrDefault("Color_SuperLightYellow",  0x022c); } }
    public static UOColor Val_LightYellow  { get { return CalebConfig.GetUOColorOrDefault("Colorl_LightYellow",  0x0037); } }
    public static UOColor Val_LightPurple  { get { return CalebConfig.GetUOColorOrDefault("Color_LightPurple",  0x00dd); } }
    public static UOColor Val_Red  { get { return CalebConfig.GetUOColorOrDefault("Color_Red",  0x0027); } }
    public static UOColor Val_LightRed  { get { return CalebConfig.GetUOColorOrDefault("Color_LightRed",  0x00ef); } }
    public static UOColor Val_LightOrange  { get { return CalebConfig.GetUOColorOrDefault("Color_LightOrange",  0x002c); } }
    public static UOColor Val_LightBlue  { get { return CalebConfig.GetUOColorOrDefault("Color_LightBlue",  0x005d); } }
    public static UOColor Val_Blue  { get { return CalebConfig.GetUOColorOrDefault("Color_Blue",  0x0063); } }
    public static UOColor Val_PureWhite  { get { return CalebConfig.GetUOColorOrDefault("Color_PureWhite",  0x0B1D); } }
    public static UOColor Val_PureOrange  { get { return CalebConfig.GetUOColorOrDefault("Color_PureOrange",  0x0B3F); } }
    public static UOColor Val_Radiation  { get { return CalebConfig.GetUOColorOrDefault("Color_Radiation",  0x0B42); } }

    private readonly object syncRoot = new object();
    //---------------------------------------------------------------------------------------------

    public Game()
    {
      this.Mode = GameMode.Dungeon;
      this.OnInit();
    }



    //---------------------------------------------------------------------------------------------


    public DateTime? LastWorldSaveTime
    {
      get
      {
        if (this.WorldSaves.Count > 0)
          return WorldSaves[WorldSaves.Count - 1];

        return null;
      }
    }

    private DateTime? lastTryFromWeb;
    private List<DateTime> worldSaves;
    public List<DateTime> WorldSaves
    {
      get
      {
        if (this.worldSaves == null)
        {
          this.worldSaves = new List<DateTime>();
          //TODO jak vyresit kdyz lognu po save
          DateTime webTime = WebWorldSaveTime.GetTimeFromUrl();
          if (WebWorldSaveTime.IsValid(webTime))
            this.worldSaves.Add(webTime);

        }
        else if (this.worldSaves.Count == 1)
        {
          DateTime wsTime = this.worldSaves[0];
          if ((DateTime.Now - wsTime).TotalMinutes > WebWorldSaveTime.WsPeriodMinutes + 5 && (!lastTryFromWeb.HasValue || (DateTime.Now - lastTryFromWeb.Value).TotalSeconds > 60))
          {
            lastTryFromWeb = DateTime.Now;
            DateTime webTime = WebWorldSaveTime.GetTimeFromUrl();
            if (WebWorldSaveTime.IsValid(webTime))
            {
              this.worldSaves.Clear();
              this.worldSaves.Add(webTime);
            }
          }
        }

        return this.worldSaves;
      }
    }

    //---------------------------------------------------------------------------------------------

    private List<DateTime> resyncs;
    public List<DateTime> Resyncs
    {
      get
      {
        if (this.resyncs == null)
          this.resyncs = new List<DateTime>();
        return this.resyncs;
      }
    }

    //---------------------------------------------------------------------------------------------

    public static bool KlamakAround
    {
      get
      {
        return World.Characters.FirstOrDefault(klamak => klamak.Distance <= 3 && klamak.Hits > 0 && klamak.Renamable) != null;
      }
    }

    //---------------------------------------------------------------------------------------------

    private DateTime initStart;
    private static long counter = 0;
    private static long totalCounter = 0;
    private Thread mainThread;

    internal void OnInit()
    {
      Game.currentGame = this;
      /// Cal.DataBasePath = "\\";
      Cal.InitEngine();//TODO nastavit z konfigu Cal.DataBasePath = "";

      UO.Print("DB path: " + Cal.Engine.DataBaseFullPath);

      this.currentPlayer = new PlayerExtended();
      Core.Window.FormClosing += Window_FormClosing;
      Journal.EntryAdded += Journal_EntryAdded;
      UIManager.StateChanged += UIManager_StateChanged;
     // World.CharacterAppeared += World_CharacterAppeared;
      World.ItemAdded += World_ItemAdded;

      initStart = DateTime.Now;

      Phoenix.Core.LoginComplete += Core_LoginComplete1;
      Phoenix.Core.Disconnected += Core_Disconnected;

      SyncRootLoot = new object();
      toDispatchItems = new List<Serial>();

      lock (syncRoot)
      {
        foreach (byte id in PacketTranslator.GetKnownPackets())
        {
          Core.RegisterClientMessageCallback(id, new MessageCallback(PacketHandler));
        }
      }
    }

    //---------------------------------------------------------------------------------------------

    //private CastSpellInfo LastCastSpell = null;

    private CallbackResult PacketHandler(byte[] data, CallbackResult prevResult)
    {
      bool handled = false;
      bool useDefault = true;
      //lock (syncRoot)
      //{
      IMacroCommand cmd = PacketTranslator.Translate(data);
      if (cmd != null)
      {
        if (cmd as CastMacroCommand != null)
        {
          CastMacroCommand cmc = (CastMacroCommand)cmd;
          StandardSpell spell = (StandardSpell)cmc.Spell;
          Magery.TrySetCastingSpell(new CastSpellInfo(spell));
          handled = true;
        }
        else if (cmd as UseSkillMacroCommand != null)
        {
          UseSkillMacroCommand usmc = (UseSkillMacroCommand)cmd;
          Game.PrintMessage("Skill: " + (StandardSkill)usmc.Skill);

          if ((StandardSkill)usmc.Skill == StandardSkill.Hiding)
          {
            Hiding.HideRunning = true;
          }

          Game.RunScriptCheck(3500);
          handled = true;
        }
        else if (cmd as WaitMenuMacroCommand != null)
        {
          //WaitMenuMacroCommand wmmc = (WaitMenuMacroCommand)cmd;

          //if (LastCastSpell != null && LastCastSpell.IsRunning)
          //{
          //  RunScript(LastCastSpell.Timeout);
          //  LastCastSpell = null;
          //  handled = true;
          //}
        }
        else if (cmd as SpeechMacroCommand != null)
        {
          useDefault = false;
        }
        else if (data.Length > 0)
        {
          if (data[0] == 0x06)//OnDoubleClick
          {
            Targeting.ResetTarget();

            UOItem item = new UOItem(ByteConverter.BigEndian.ToUInt32(data, 1));

            if (Debug)
              Game.PrintMessage("OnDoubleClick " + item.Serial);

            if (item.Exist)
            {
              string hanledText = "";

              foreach (KeyValuePair<StandardSpell, Graphic> kvp in Magery.SpellScrool)
              {
                if (item.Graphic == kvp.Value)
                {
                  Magery.TrySetCastingSpell(new CastSpellInfo(kvp.Key, true));
                  hanledText = " Scrool";
                  handled = true;
                  break;
                }

              }

              if (!handled)
              {
                if (item.Graphic == ItemLibrary.RuneBook.Graphic
                    || item.Graphic == ItemLibrary.TravelBook.Graphic
                    || item.Graphic == ItemLibrary.SpellBook.Graphic
                    || item.Graphic == ItemLibrary.CestovniKniha.Graphic
                    || item.Graphic == ItemLibrary.NbRuna.Graphic
                  )
                {
                  handled = true;
                  hanledText = " Book";
                  Game.RunScript(5000);
                  //World.Player.PrintMessage("OnDoubleClick knizky: " + item.Serial + " / " + item.Graphic + "|" + item.Color);
                }
              }

              if (!handled)
              {

                foreach (Potion potion in PotionCollection.Potions)
                {
                  if (item.Graphic == potion.DefaultGraphic)
                  {
                    if (Journal.WaitForText(true, 250, "You can't drink another potion yet"))
                    {
                      if (LastDrinkTime != null && (17.0 - (DateTime.Now - LastDrinkTime.Value).TotalSeconds) >= 0)
                        World.Player.PrintMessage(String.Format("Lahev za {0:N1}s", 17.0 - (DateTime.Now - LastDrinkTime.Value).TotalSeconds));
                    }
                    else
                      LastDrinkTime = DateTime.Now;

                    handled = true;
                    hanledText = " Potion " + potion.Name;

                    break;
                  }
                }

              }


              if (!handled)
              {
                if (item.Graphic == 0x0E26 && item.Color == 0x0B83)//lahvicka - Duch prasteraho pralesa 
                {
                  this.CurrentPlayer.Player.PrintMessage("[1. Forma...]", Game.Val_LightPurple);
                  handled = true;
                  hanledText = "Forma";
                }
                else if (item.Graphic == 0x0FC4 && item.Color == 0x0B83)//soul - Duch prastarych stepi 
                {
                  this.CurrentPlayer.Player.PrintMessage("[2. Forma...]", Game.Val_LightPurple);
                  handled = true;
                  hanledText = "Forma";
                }
                else if (item.Graphic == 0x227A && item.Color == 0x0000)
                {
                  this.CurrentPlayer.Player.PrintMessage("[Isk Ress...]", Game.Val_GreenBlue);
                  handled = true;
                  hanledText = "IskRess";
                  RunScript(6000);
                }
                else if (Necromancy.IsNecroSpellScroll(item))
                {
                  NecromancySpell spell = Necromancy.GetNecromancySpellFromScroll(item);
                  World.Player.PrintMessage(spell + " [" + item.Amount + "ks]");
                  RunScriptCheck(5000);
                  hanledText = "NekroSpell";
                }

                if (handled && Debug)
                  Game.PrintMessage("DoubleClick handled" + hanledText);
              }

              if (!handled)
              {
                if (ItemLibrary.ShrinkKlamaci.Count(s => s.Graphic == item.Graphic) > 0 || ItemLibrary.ShrinkMountTypes.Count(s=>s.Graphic == item.Graphic) > 0)
                {
                  UOItemType d = ItemLibrary.ShrinkKlamaci.FirstOrDefault(s => s.Graphic == item.Graphic);
                  if (d == null)
                    d = ItemLibrary.ShrinkMountTypes.FirstOrDefault(s => s.Graphic == item.Graphic);

                  MobMaster.LasTimeUseKlamak = DateTime.Now;
                  Game.PrintMessage("Klamak used: " + (d != null ? d.Name : " unknown"));
                }

              }

            }
          }
          else if (data[0] == 0x05)//OnAtack
          {
            atackCounterEnabled = true;
            Game.RunScriptCheck(Targeting.DynamicAttackDelay);
          }
        }

        if (!handled && useDefault)
        {
          RunScriptCheck(4500);
        }
      }
      // }

      return CallbackResult.Normal;
    }

    //---------------------------------------------------------------------------------------------

    public bool CanDrink
    {
      get
      {
        return lastDrinkTime == null || (DateTime.Now - LastDrinkTime.Value).TotalSeconds > 17;
      }
    }
  

    //---------------------------------------------------------------------------------------------

    private DateTime? lastDrinkTime;
    internal DateTime? LastDrinkTime
    {
      get { return this.lastDrinkTime; }
      set
      {
        this.lastDrinkTime = value;
        drinkTimer.Stop();
        drinkTimer.Interval = 3000;
        drinkTimer.Start();

      }
    }

    //---------------------------------------------------------------------------------------------

    private System.Timers.Timer _drinkTimer;
    private System.Timers.Timer drinkTimer
    {
      get
      {
        if (_drinkTimer == null)
        {
          _drinkTimer = new System.Timers.Timer();
          _drinkTimer.Elapsed += DrinkTimerTimerElapsed;
          _drinkTimer.Enabled = true;

        }
        return _drinkTimer;
      }
    }

    //---------------------------------------------------------------------------------------------


    private void DrinkTimerTimerElapsed(object sender, ElapsedEventArgs e)
    {
      if (LastDrinkTime == null || (DateTime.Now - LastDrinkTime.Value).TotalSeconds > 17)
      {
        drinkTimer.Stop();
        World.Player.PrintMessage("[Muzes pit...]", Val_GreenBlue);
      }
      else
      {
        if (LastDrinkTime != null)
          Game.PrintMessage(String.Format("Lahev za! {0:N1}s", 17.0 - (DateTime.Now - LastDrinkTime.Value).TotalSeconds), Val_GreenBlue);
      }
    }


    //---------------------------------------------------------------------------------------------

    private void Core_Disconnected(object sender, EventArgs e)
    {
      Journal.EntryAdded -= Journal_EntryAdded;
      UIManager.StateChanged -= UIManager_StateChanged;
      //World.CharacterAppeared -= World_CharacterAppeared;
      Core.Window.FormClosing -= Window_FormClosing;
      player.Changed -= new ObjectChangedEventHandler(Player_Changed);
    }

    //---------------------------------------------------------------------------------------------

    public DateTime? loginStartTime;
    private void Core_LoginComplete1(object sender, EventArgs e)
    {
      loginStartTime = DateTime.Now;
      player = World.Player;
      player.Changed += new ObjectChangedEventHandler(Player_Changed);

      runmt();
    }

    //---------------------------------------------------------------------------------------------

    private static bool mtStarted = false;
    //---------------------------------------------------------------------------------------------

    [Command]
    public static void runmt()
    {
      if (Game.CurrentGame.mainThread == null)
      {
        UO.Print("mt Start");
        Game.CurrentGame.mainThread = new Thread(new ThreadStart(Main));
        Game.CurrentGame.mainThread.Start();

        Thread secondary = new Thread(new ThreadStart(Secondary));
        secondary.Start();
      }
      else
      {
        if (!mtStarted)
          UO.Print("mt Alredy running");
      }
      mtStarted = true;
    }

    //---------------------------------------------------------------------------------------------

    [Command]
    public static void pmts()
    {
      UO.Print(String.Format("mainThread: {0}, {1}", Game.CurrentGame.mainThread != null, (Game.CurrentGame.mainThread != null ? Game.CurrentGame.mainThread.IsAlive : false)));
    }

    //---------------------------------------------------------------------------------------------

    public Serial CurrentHoverStatus
    {
      get { return Aliases.GetObject("CurrentHoverStatus"); }
    }

    //---------------------------------------------------------------------------------------------

    public bool HasAliveAlie
    {
      get
      {
        List<UOCharacter> alies = new List<UOCharacter>();
        alies.AddRange(Alies.ToArray());

        return alies.Where(i => i.Exist && i.Hits > 0).Count() > 0;
      }
    }

    //---------------------------------------------------------------------------------------------

    public bool IsAlie(Serial s)
    {
      lock (syncRoot)
      {
        foreach (UOCharacter ch in Alies)
        {
          if (ch.Serial == s)
            return true;
        }

        return false;
      }
    }

    //---------------------------------------------------------------------------------------------

    public bool IsHealAlie(Serial s)
    {
      lock (syncRoot)
      {
        foreach (UOCharacter ch in HealAlies)
        {
          if (ch.Serial == s)
            return true;
        }

        return false;
      }
    }
    //---------------------------------------------------------------------------------------------

    private List<UOCharacter> healAlies;
    public List<UOCharacter> HealAlies
    {
      get
      {
        if (this.healAlies == null)
        {
          this.healAlies = new List<UOCharacter>();
        }
        return this.healAlies;
      }
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void RefreshAllAlies()
    {
      DateTime start = DateTime.Now;
      List<UOCharacter> alies = Game.CurrentGame.Alies;
      foreach (UOCharacter alie in alies)
      {
        alie.RequestStatus(100);
      }

      Game.PrintMessage("Refresh duration: " + String.Format("{0:n1}", (DateTime.Now - start).TotalMilliseconds));
    }  

    //---------------------------------------------------------------------------------------------

    private List<UOCharacter> alies;
    public List<UOCharacter> Alies
    {
      get
      {
        if (this.alies == null)
        {
          this.alies = new List<UOCharacter>();
        }
        return this.alies;
      }


      
    }


    //---------------------------------------------------------------------------------------------

    public static bool IsPossibleMob(UOCharacter ch)
    {
      bool typeCheck = 
        UOItemTypeBase.ListContains(ch.Model, ch.Color, ItemLibrary.PlayerSummons) || 
        UOItemTypeBase.ListContains(ch.Model, ch.Color, ItemLibrary.AttackKlamaci) || 
        ItemLibrary.PlayerKlamaci.Count(k => k.Graphic == ch.Model) > 0 ||
        ItemLibrary.MountTypes.Count(k => k.Graphic == ch.Model) > 0;
      return /*ch.Notoriety != Notoriety.Guild && ch.Notoriety != Notoriety.Innocent && ch.Notoriety != Notoriety.Invulnerable &&*/ (typeCheck || ch.Renamable);
    }

    //---------------------------------------------------------------------------------------------

    //private DateTime lastKlick = DateTime.MinValue;
    //private void World_CharacterAppeared(object sender, CharacterAppearedEventArgs e)
    //{
    //  if (CalebConfig.Rename == RenameType.OnAppeared || CalebConfig.Rename == RenameType.Booth)
    //  {
    //    UOCharacter ch = new UOCharacter(e.Serial);
    //    string msg = String.Format("Appear: {0}, {1}", IsPossibleMob(ch), !IsMobRenamed(e.Serial));

    //    bool result = false;
    //    if (IsPossibleMob(ch) && !IsMobRenamed(e.Serial))
    //      result = (bool)Rename(e.Serial);

    //    msg += ", Renamed: " + result;

    //    if (!result && ch.Distance <= 3 && UOItemTypeBase.ListContains(ch.Model, ItemLibrary.AttackKlamaci))
    //      new StatusBar().Show(ch.Serial);

    //    if (Debug)
    //      Notepad.WriteLine("World_CharacterAppeared: " + msg + " ch: " + ch.Description);
    //  }
    //}

//    World_CharacterAppeared: Appear: True, True, Renamed: True ch: Serial: 0x00189CFF  Name: "anebit5J"  Position: 3349.358.4  Flags: 0x0000  Color: 0x0972  Model: 0x00D6  Renamable: True Notoriety: Neutral HP: 176/176, Distance: 1
//World_CharacterAppeared: Appear: True, True, Renamed: True ch: Serial: 0x0019FB09  Name: "ane5wuE5"  Position: 3344.358.4  Flags: 0x0000  Color: 0x0972  Model: 0x00D6  Renamable: True Notoriety: Neutral HP: 166/166, Distance: 0
//World_CharacterAppeared: Appear: True, True, Renamed: True ch: Serial: 0x0025213A  Name: "aneg9o77"  Position: 3347.357.4  Flags: 0x0000  Color: 0x0972  Model: 0x00D6  Renamable: True Notoriety: Neutral HP: 170/170, Distance: 0
//World_CharacterAppeared: Appear: False, True, Renamed: False ch: Serial: 0x0031E94B  Name: "Cat"  Position: 3341.360.4  Flags: 0x0000  Color: 0x0904  Model: 0x00C9  Renamable: False Notoriety: Neutral HP: -1/-1, Distance: 0


    //---------------------------------------------------------------------------------------------

    public static bool IsMobActive(Serial e)
    {
      UOCharacter ch = new UOCharacter(e);
      return ch.ExistCust() && ch.Distance < 30;
    }

    //---------------------------------------------------------------------------------------------

    public static bool IsMyMob(Serial e)
    {
      UOCharacter ch = new UOCharacter(e);

      return(ch.Renamable || Rename.IsMobRenamed(e)); //IsMobRenamed(e); //;
    }

    //---------------------------------------------------------------------------------------------

    public static bool IsMob(Serial e)
    {
      UOCharacter ch = new UOCharacter(e);
      
      return IsMobActive(e) && IsMyMob(e); //IsMobRenamed(e); //;
    }

    //---------------------------------------------------------------------------------------------
    private Hashtable renameTryHt;
    public List<UOCharacter> Mobs
    {
      get
      {
        List<UOCharacter> mobs = new List<UOCharacter>();
        if (renameTryHt == null)
          renameTryHt = new Hashtable();

        mobs.AddRange(World.Characters.Where(i => i.Distance < 22 && IsMob(i.Serial)).ToArray());

        var possibleMobs = World.Characters.Where(
          i => 
          !Rename.IsMobRenamed(i.Serial) &&
          IsPossibleMob(i) &&
          renameTryHt[i.GetUniqueKey()] == null);

        if (Game.Debug)
          Game.PrintMessage("possibleMobs: " + possibleMobs.Count());

        foreach (UOCharacter ch in possibleMobs)
        {
          if (((int?)renameTryHt[ch.GetUniqueKey()]).GetValueOrDefault() > 2)
            continue;

          renameTryHt[ch.GetUniqueKey()] = ((int?)renameTryHt[ch.GetUniqueKey()]).GetValueOrDefault() + 1;

          MobRenameInfo info = Rename.RenameCharacter(ch.Serial, 3);
          if (info)
            mobs.Add(ch);

          if (Game.Debug)
            Game.PrintMessage("[Try Rename... " + (info ? "OK" : "") + "] (" + info.Tries + ")", Val_LightPurple);

          break;
        }

        return mobs;
      }
    }

    //---------------------------------------------------------------------------------------------

    public static UOItem LootBag
    {
      get
      {
        if (World.Player.Backpack.Items.FindType(0x0E76).Exist)
          return World.Player.Backpack.Items.FindType(0x0E76);

        return World.Player.Backpack;
      }
    }

    //---------------------------------------------------------------------------------------------

    public static UOItem DwarfKnife
    {
      get
      {
        if (World.Player.Backpack.Items.FindType(0x10E4).Exist)
          return World.Player.Backpack.Items.FindType(0x10E4);

        return new UOItem(Serial.Invalid);
      }
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void PrintConfig()
    {
      UO.Print("CalebConfig.AutoHeal: " + CalebConfig.AutoHeal);
      UO.Print("CalebConfig.Rename: " + CalebConfig.Rename);
      UO.Print("CalebConfig.DestroySpiderWeb: " + CalebConfig.DestroySpiderWeb);
      UO.Print("CalebConfig.HealMinDamagePerc: " + CalebConfig.HealMinDamagePerc);
      UO.Print("CalebConfig.HealType: " + CalebConfig.HealType);
      UO.Print("CalebConfig.Loot: " + CalebConfig.Loot);
      UO.Print("CalebConfig.TrainPoison: " + CalebConfig.TrainPoison);
      UO.Print("CalebConfig.HandleFrozen: " + CalebConfig.HandleFrozen);

      UO.Print("CalebConfig.HealType: " + (AutoHealType)Config.Profile.UserSettings.GetAttribute((int)AutoHealType.Automatic, "Value", "HealType"));

    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public void SwitchWarRunOff()
    {
      Game.PrintMessage("SwitchWarRunOff", Game.Val_LightGreen);
      Targeting.ResetTarget();

      World.Player.ChangeWarmode(WarmodeChange.Switch);
      Game.Wait(200);
      if (!World.Player.Warmode)
        World.Player.ChangeWarmode(WarmodeChange.Switch);

      //WarmodeChange to = World.Player.Warmode ? WarmodeChange.Peace : WarmodeChange.War;

      //World.Player.ChangeWarmode(to);
      //if (to == WarmodeChange.Peace)
      //{
      //  Game.Wait(250);
      //  World.Player.ChangeWarmode(WarmodeChange.War);
      //}

      RunScript(5);
    }

    //---------------------------------------------------------------------------------------------


    [Executable]
    public static void HideObjectTarget(string target)
    {
      World.Player.PrintMessage("[Vyber hide..]");


      TargetInfo t = Targeting.GetTarget(target);
      if (t.Success && t.Object.Serial.IsValid)
        UO.Hide(t.Object.Serial);
      else
        World.Player.PrintMessage("[Invalid hide..]");
    }

    //---------------------------------------------------------------------------------------------

    //private void BtnHide_MouseClick(object sender, MouseEventArgs e)
    //{
    //  lastUsedMethod = BtnHide_MouseClick;
    //  lastUsedObject = sender;
    //  lastUsedMouseEventArgs = e;

    //  World.Player.PrintMessage("[Vyber HIDE..]");
    //  new Thread(new ThreadStart(UO.Hide)).Start();

    //}

    //---------------------------------------------------------------------------------------------

    private static bool SemiHealOn = false;

    //---------------------------------------------------------------------------------------------

    [Executable]
    [BlockMultipleExecutions]
    public void SemiHealOnSwitch()
    {
      SemiHealOn = !SemiHealOn;
      string message = "OFF";
      ushort color = 0x0025;

      if (SemiHealOn)
      {
        message = "ON";
        color = 0x0040;
      }
      Game.PrintMessage("Heal " + message, color);
    }

    //---------------------------------------------------------------------------------------------

    public static bool IsHealOn
    {
      get
      {
        return CalebConfig.AutoHeal && (CalebConfig.HealType == AutoHealType.Automatic || CalebConfig.HealType == AutoHealType.SemiAutomatic && SemiHealOn);
      }
    }

    public static bool AliePrintPlace2More = false;
    [Executable]
    [BlockMultipleExecutions]
    public void SwitchAliePrintPlace2More()
    {
      AliePrintPlace2More = !AliePrintPlace2More;
      string message = "OFF";

      if (AliePrintPlace2More)
        message = "ON";

      Game.PrintMessage("Alert 3 policek " + message, Val_LightGreen);
    }

    //---------------------------------------------------------------------------------------------

    private static bool CheckTreasure = false;
    private static int CheckTreasureDistance = 20;
    //---------------------------------------------------------------------------------------------

    [Executable]
    [BlockMultipleExecutions]
    public void SwitchCheckTreasure()
    {
      SwitchCheckTreasure(CheckTreasureDistance);
    }

    [Executable]
    [BlockMultipleExecutions]
    public void SwitchCheckTreasure(int distanceCheck)
    {
      CheckTreasureDistance = distanceCheck;
      CheckTreasure = !CheckTreasure;
      string message = "OFF";

      if (CheckTreasure)
        message = "ON";

      Game.PrintMessage("CheckTreasure " + message, Val_LightGreen);
    }


    //---------------------------------------------------------------------------------------------

    [Executable("printtreasure")]
    public static void PrintKownObjects()
    {
      PrintKownObjects(false);
    }

    //---------------------------------------------------------------------------------------------

    [Executable("printtreasure")]
    public static void PrintKownObjects(bool printUknown)
    {
      PrintKownObjects(printUknown, Serial.Invalid);
    }

    //---------------------------------------------------------------------------------------------


    public static void PrintKownObjects(bool printUknown, Serial s)
    {
      int originalFindDistance = World.FindDistance;
      World.FindDistance = CheckTreasureDistance;

      List<object[]> uknow = new List<object[]>();
      List<object[]> know = new List<object[]>();
      List<UOItem> search = new List<UOItem>();
      if (new UOItem(s).Exist)
        search.Add(new UOItem(s));
      else
        search = World.Ground.ToList();

      foreach (UOItem item in search)
      {
          String printText = "Uknown";
          if (item.Graphic == 0x1FFF)
            printText = "Bedynka";
          else if (item.Graphic == 0x1BBF)
            printText = "Click";
          else if (item.Graphic == 0x1091)
            printText = "Switch";
          else if (item.Graphic == 0x1094)
            printText = "Lever";
        else if (item.Graphic == 0x0E40 || item.Graphic == 0x0E41)
          printText = "Truhla";
        else if (item.Graphic == 0x0FBD)
          printText = "Kniha";
        else if (item.Graphic == 0x0E24)//Ink
          printText = "Ink/Blood";


          //              //Serial: 0x4038D18D  Name: "Kniha Iskariotskych"  Position: 180.90.0  Flags: 0x0000  Color: 0x0B1D  Graphic: 0x0FBD  Amount: 1  Layer: None Container: 0x403854B5
        if (printText == "Uknown")
          uknow.Add(new object[] { item, printText });
        else
          know.Add(new object[] { item, printText });
      }

      if (printUknown)
      {
        foreach (object[] item in uknow)
          ((UOItem)item[0]).PrintMessage("[" + item[1] + "...]", Game.Val_GreenBlue);
          //UO.PrintObject(((UOItem)item[0]).Serial, Game.Val_GreenBlue, );
      }

      foreach (object[] item in know)
        ((UOItem)item[0]).PrintMessage("[" + item[1] + "...]", Game.Val_LightPurple);
      //UO.PrintObject(((UOItem)item[0]).Serial, Game.Val_LightPurple, "[" + item[1] + "...]");

      World.FindDistance = originalFindDistance;
    }

    //---------------------------------------------------------------------------------------------

    public static bool Banding = false;
    public static void CheckStopBanding()
    {
      if (Banding)
      {
        Game.PrintMessage("Banding force STOP...", Game.Val_GreenBlue);
        Game.CurrentGame.CurrentPlayer.SwitchWarmode();
        Banding = false;
      }
    }

    private IUOPosition lastPosition;
    public IUOPosition LastPosition
    {
      get
      {
        if (this.lastPosition == null)
          this.lastPosition = new UOPositionBase(World.Player.X, World.Player.Y, (ushort)World.Player.Z);
        return this.lastPosition;
      }

      set { this.lastPosition = value; }
    }

    public static bool PlayerMoving = false;
    
    public static void Secondary()
    {
      DateTime lastMoveTime = DateTime.MinValue;

      while (true)
      {
        Thread.Sleep(5);

        if (WalkHandling.DesiredPosition.X != World.Player.X || WalkHandling.DesiredPosition.Y != World.Player.Y)
        {
          lastMoveTime = DateTime.Now;
          PlayerMoving = true;
        }
        else if ((DateTime.Now - lastMoveTime).TotalMilliseconds > 125)
          PlayerMoving = false;
      }
    }

    //---------------------------------------------------------------------------------------------

    private static List<Serial> toDispatchItems;
    private static object SyncRootLoot;
    private  void World_ItemAdded(object sender, ObjectChangedEventArgs e)
    {

      lock (SyncRootLoot)
      {
        if (CalebConfig.Loot != LootType.None)
        {
          TryAddLootDispatchItem(e.Serial);
          TryAddLootDispatchItem(e.ItemSerial);
        }
        else
          TryClearAllLootDispatchItems();
      }
    }

    //---------------------------------------------------------------------------------------------

    private static void TryClearAllLootDispatchItems()
    {
      lock (SyncRootLoot)
      {
        toDispatchItems.Clear();
      }
    }

    //---------------------------------------------------------------------------------------------

    private static void TryAddAllGroundItemToDispatch(int maxDistance)
    {
      int origDistance = World.FindDistance;
      World.FindDistance = maxDistance;

      List<UOItem> allItems = World.Ground.Where(i => i.Distance <= maxDistance && i.Graphic == 0x0E76 && i.Color == 0x049A).ToList();
      int add = 0;

      foreach (UOItem item in allItems)
      {
        if (TryAddLootDispatchItem(item.Serial))
          add++;
      }
      World.FindDistance = origDistance;
      Game.PrintMessage(String.Format("Pridano Loot itemu {0} z {1}.", add, allItems.Count));
    }

    //---------------------------------------------------------------------------------------------

    private static bool TryAddLootDispatchItem(Serial s)
    {
      UOItem item = new UOItem(s);
      UOItem itemCont = new UOItem(item.Container);
      //"0x0E76" Color="0x049A"
      bool validItem = item.Graphic == 0x2006 || (Loot.IsLootItem(item) && itemCont.Graphic == 0x2006 || !itemCont.Exist && item.Graphic == 0x0E76 && item.Color == 0x049A);//ze zeme jen lootpytlik
      if (validItem)
      {
        lock (SyncRootLoot)
        {
          if (!toDispatchItems.Contains(s))
          {
            toDispatchItems.Add(s);
            return true;
          }
        }
      }
      return false;
    }

    //---------------------------------------------------------------------------------------------

    public static void Main()
    {
      Serial lastHealedSerial = Serial.Invalid;
      List<Serial> dennyMobs = new List<Serial>();
      List<Serial> skip = new List<Serial>();
      List<Serial> lootitems = new List<Serial>();
      List<string> bodies = new List<string>();
      Dictionary<string, LootCorpseInfo> bodieTry = new Dictionary<string, LootCorpseInfo>();
      DateTime? deystroyWebLastCantReach = null;
      Dictionary<Serial, StatusRequestInfo> alieStatusRequest = new Dictionary<Serial, StatusRequestInfo>();
      DateTime statusRequestLastTime = DateTime.Now;
      DateTime renameLastTime = DateTime.Now;
      List<Serial> webs = new List<Serial>();
      List<Serial> printedTreasure = new List<Serial>();
      Hashtable renameHandled = new Hashtable();
      Hashtable wsPrints = new Hashtable();
      DateTime lastCleanBandageCheck = DateTime.Now;
      Hashtable lootBodies = new Hashtable();
      //Hashtable tryBodies = new Hashtable();
      //List<Serial> tryBodiesList = new List<Serial>();
      Hashtable cutedBodies = new Hashtable();
      Hashtable doneBodies = new Hashtable();



      List<Serial> poisonTrainDoneList = new List<Serial>();

      int bandageSucces = 0;
      int bandageFailed = 0;
      List<HealInfo> bandageInfos = new List<HealInfo>();

      World.FindDistance = 5;
      while (true)
      {
        try
        {
          Thread.Sleep(5);

          bool handled = false;
          counter++;
          totalCounter += 5;

          if (World.Player.Hits <= 0 || Game.CurrentGame.Mode == GameMode.Working)
            continue;

          #region WorldSavePrint

          if (Game.CurrentGame.loginStartTime.HasValue && (DateTime.Now - Game.CurrentGame.loginStartTime.Value).TotalSeconds > 30)
          {
            DateTime? lastWsTime = Game.CurrentGame.LastWorldSaveTime;

            if (lastWsTime.HasValue)
            {
              DateTime now = DateTime.Now;
              WebWorldSaveInfo wsInfo = WebWorldSaveTime.GetInfo(lastWsTime.Value, 0);
              double nextWsMinutes = Math.Abs(wsInfo.NextTimeSpan.TotalMinutes);

              string sufixCode = "";
              bool alert = false;
              if (wsInfo.NextTime > now && nextWsMinutes <= 1)
              {
                alert = true;
                sufixCode = "1";
              }
              else if (wsInfo.NextTime > now && nextWsMinutes <= 3)
              {
                alert = true;
                sufixCode = "3";
              }
              else if (wsInfo.NextTime > now && nextWsMinutes <= 5)
                sufixCode = "5";
              else if (wsInfo.NextTime > now && nextWsMinutes <= 10)
                sufixCode = "10";
              else if (wsInfo.NextTime > now && nextWsMinutes <= 15)
                sufixCode = "15";
              else if (wsInfo.NextTime > now && nextWsMinutes <= 30)
                sufixCode = "30";
              else if (wsInfo.NextTime > now && nextWsMinutes <= 60)
                sufixCode = "60";

              string code = lastWsTime.Value.Ticks + "___" + sufixCode;
              if (!String.IsNullOrEmpty(sufixCode) && wsPrints[code] == null)
              {
                wsPrints[code] = DateTime.Now;
                Game.PrintMessage(String.Format("WS < {0}min: {1} ({2:HH:mm:ss}))", sufixCode, wsInfo.NextTimeStr, wsInfo.NextTime), Val_GreenBlue);
                if (alert)
                  World.Player.PrintMessage("WS < {0}min!", MessageType.Info, sufixCode);
              }
            }
          }
          #endregion

          #region moved

          IUOPosition current = new UOPositionBase(World.Player.X, World.Player.Y, (ushort)World.Player.Z);
          if (Robot.GetRelativeVectorLength(current, Game.CurrentGame.LastPosition) > 1.5)
          {
            Game.CurrentGame.LastPosition = new UOPositionBase(World.Player.X, World.Player.Y, (ushort)World.Player.Z);

            if (CheckTreasure)
            {
              int originalFindDistance = World.FindDistance;
              World.FindDistance = CheckTreasureDistance;

              for (int i = printedTreasure.Count - 1; i >= 0; i --)
              {
                UOItem item = new UOItem(printedTreasure[i]);
                if (!item.Exist || item.Distance > CheckTreasureDistance || item.Distance < 0)
                  printedTreasure.RemoveAt(i);

              }


//              Serial: 0x4010C388  Name: "metal chest"  Position: 1430.3872.18  Flags: 0x0020  Color: 0x0000  Graphic: 0x0E41  Amount: 0  Layer: None Container: 0x00000000

              //Serial: 0x4013F112  Position: 1424.3882.18  Flags: 0x0000  Color: 0x0000  Graphic: 0x0E40  Amount: 0  Layer: None Container: 0x00000000


              //foreach (Serial s in printedTreasure.Keys)
              //{
              //  if (!new UOItem(s).Exist)
              //    printedTreasure[s] = null;
              //}

              foreach (UOItem item in World.Ground)
              {
                if (!printedTreasure.Contains(item.Serial)  && (item.Graphic == 0x0E40 || item.Graphic == 0x0E41 || item.Graphic == 0x1FFF || item.Graphic == 0x1BBF || item.Graphic == 0x1091 || item.Graphic == 0x1094 || item.Graphic == 0x0FBD))//TODO pokaldy
                {
                  //PrintKownObjects(false, item.Serial);
                  String printText = "";
                  if (item.Graphic == 0x1FFF)
                    printText = "Bedynka";
                  else if (item.Graphic == 0x1BBF)
                    printText = "Click";
                  else if (item.Graphic == 0x1091)
                    printText = "Switch";
                  else if (item.Graphic == 0x1094)
                    printText = "Lever";
                  else if (item.Graphic == 0x0E40 || item.Graphic == 0x0E41)
                    printText = "Truhla";
                  else if (item.Graphic == 0x0E24)//Ink
                    printText = "Ink/Blood";
                  else if (item.Graphic == 0x0FBD)
                    printText = "Kniha";

                  item.PrintMessage("[" + printText + "...]", Game.Val_GreenBlue);
                  //                UO.PrintObject(item.Serial, Game.Val_GreenBlue, );
                  printedTreasure.Add(item.Serial);
                }
              }


              World.FindDistance = originalFindDistance;
            }

          }


          #endregion

          #region SpiderWeb

          if (CalebConfig.DestroySpiderWeb)
          {
            //23:56 System: You are caught in the web.
            int destoryed = 0;

            int origFindDistance = World.FindDistance;
            World.FindDistance = 2;

            foreach (UOItem item in World.Ground)
            {
              if (destoryed >= 1 || deystroyWebLastCantReach != null && (DateTime.Now - deystroyWebLastCantReach.Value).TotalMilliseconds < 2000)
                break;

              if (item.Distance <= 1)
              {
                if (
                  !webs.Contains(item.Serial) &&
                   (item.Graphic == (new Graphic(0x0EE1)) ||
                   item.Graphic == (new Graphic(0x0EE2)) ||
                   item.Graphic == (new Graphic(0x0EE3)) ||
                   item.Graphic == (new Graphic(0x0EE4)) ||
                   item.Graphic == (new Graphic(0x0EE5)) ||
                   item.Graphic == (new Graphic(0x0EE6))
                   ))
                {
                  webs.Add(item.Serial);
                  item.Use();
                  if (Journal.WaitForText(true, 250, "can't reach"))
                  {
                    handled = true;
                    Game.Wait(150);
                    deystroyWebLastCantReach = DateTime.Now;
                  }
                  else
                    deystroyWebLastCantReach = null;

                  handled = !item.Exist;
                  if (handled)
                    Game.PrintMessage("WebDestroy");
                  destoryed++;
                  //break;
                }
              }
            }
            World.FindDistance = origFindDistance;

          }
          #endregion

          #region rename

          if (CalebConfig.Rename == RenameType.MainMethod || CalebConfig.Rename == RenameType.Booth)
          {

          }

          #endregion
          //TODO predelat. Udelat ht na opnutatela, cheknuta tela, telacosemaji riznou, riznta tela. Zkusit vyresit jak poznat telo humana/hrace

          #region not casting 

          if (Magery.CastingSpellInfo == null || !Magery.CastingSpellInfo.IsRunning)
          {
            #region loot

            //bool lastIsTryBody = false;
            if (CalebConfig.Loot != LootType.None)
            {
              Serial s = Serial.Invalid;
              List<UOItem> ground = new List<UOItem>();

              lock (SyncRootLoot)
              {
                for (int i = toDispatchItems.Count - 1; i >= 0; i--)
                {
                  UOItem item = new UOItem(toDispatchItems[i]);
                  UOItem itemContainer = new UOItem(item.Container);

                  bool isCorpse = item.Graphic == 0x2006;
                  bool isInCorpse = itemContainer.Exist && itemContainer.Graphic == 0x2006;

                  if (doneBodies[item.GetUniqueKeyPostion()] != null)
                  {
                    toDispatchItems.RemoveAt(i);
                    continue;
                  }

                  //if (tryBodies[item.GetUniqueKeyPostion()] != null && (string)tryBodies[item.GetUniqueKeyPostion()] == World.Player.GetUniqueKeyPostion())
                  //  continue;

                  if (isCorpse)
                  {
                    if (isCorpse && item.Distance <= 5)
                      ground.Add(item);
                  }
                  else if (isInCorpse)
                  {
                    if (item.Distance <= 3)
                      ground.Add(item);
                  }
                  else
                  {
                    if (item.Distance <= 1)
                      ground.Add(item);
                  }

                  //if (item.Exist && (item.Graphic == 0x2006 && item.Distance <= 1 || item.Graphic != 0x2006))
                  //  ground.Add(item);
                }
              }

              ground = ground.OrderBy(i => i.Distance).ToList();

              bool remove = true;
              if (ground.Count > 0)
              {
                Journal.Clear();
                UOItem lootItem = ground[0];
                bool isCorpse = lootItem.Graphic == 0x2006;
                s = lootItem.Serial;

                if (isCorpse && !lootItem.Opened)
                {
                  doneBodies[lootItem.GetUniqueKeyPostion()] = lootItem.Serial;
                  //if (lastIsTryBody)
                  //  Game.Wait(750);

                  //if (tryBodiesList.Contains(lootItem.Serial))
                  //  lastIsTryBody = true;
                  //else
                  //  lastIsTryBody = false;

                  lootItem.Use();

                  if (Journal.WaitForText(true, 175, "You can't reach"))
                  {
                    //if (!tryBodiesList.Contains(lootItem.Serial))
                    //  tryBodiesList.Add(lootItem.Serial);
                    //tryBodies[lootItem.GetUniqueKeyPostion()] = World.Player.GetUniqueKeyPostion();
                    lootItem.PrintMessage("[ Can't reach ]", MessageType.Error);
                    //remove = false;
                  }
                  else
                  {
                    lootItem.PrintMessage("[ Loot Body ]", Game.Val_GreenBlue);
                  }


                  //Game.Wait(125);
                }

                if (!isCorpse && CalebConfig.Loot != LootType.OpenCorpse)
                {
                  List<Serial> bpkState = ItemHelper.ContainerState(World.Player.Backpack);

                  UOItem itemContainer = new UOItem(lootItem.Container);
                  bool isInCorpse = itemContainer.Exist && itemContainer.Graphic == 0x2006;
                  bool lootDone = false;
                  bool valid = false;
                  bool isLootBag = lootItem.Graphic == 0x0E76 && lootItem.Color == 0x049A;

                  if (isInCorpse)
                  {
                    bool hasLootBag = lootBodies.Contains(itemContainer.GetUniqueKeyPostion()) || itemContainer.Items.Count(si => si.Graphic == 0x0E76 && si.Color == 0x049A) > 0;

                    if (!hasLootBag)//Halucinace
                    {
                      foreach (UOItem bag in itemContainer.Items.Where(b => b.Graphic == 0x0E76))
                      {
                        string name = ItemHelper.GetItemName(bag);

                        if (name.ToLower().StartsWith("loot"))
                        {
                          if (bag.Serial == lootItem.Serial)
                            isLootBag = true;

                          hasLootBag = true;
                          break;
                        }
                      }
                    }
                    valid = hasLootBag;

                    if (valid)
                      lootBodies[itemContainer.GetUniqueKeyPostion()] = itemContainer.Serial;

                  }
                  else if (!itemContainer.Exist)
                  {
                    if (lootItem.Graphic == 0x0E76 && !isLootBag)
                    {
                      string name = ItemHelper.GetItemName(lootItem);
                      valid = name.ToLower().StartsWith("loot");
                    }
                    else
                      valid = true;//predpokladam ze je spravne nastaveno v definici lootitemu ;][
                  }

                  if (valid)
                  {
                    lootDone = lootItem.Move(lootItem.Amount, LootBag);
                    Game.Wait(150);

                    if (CalebConfig.Loot == LootType.QuickCut && isInCorpse && isLootBag && cutedBodies[itemContainer.GetUniqueKeyPostion()] == null)
                    {
                      DwarfKnife.Use();
                      UO.WaitTargetObject(itemContainer);
                      cutedBodies[itemContainer.GetUniqueKeyPostion()] = itemContainer.Serial;
                    }
                  }

                  if (lootDone)
                  {
                    List<Serial> bpkAfterLoot = ItemHelper.ContainerState(World.Player.Backpack);
                    List<Serial> diff = ItemHelper.ContainerStateDiff(bpkState, bpkAfterLoot);

                    foreach (Serial ds in diff)
                    {
                      if (!lootitems.Contains(ds))
                        lootitems.Add(ds);
                    }
                  }
                }
              }

              if (remove)
              {
                lock (SyncRootLoot)
                {
                  for (int i = toDispatchItems.Count - 1; i >= 0; i--)
                  {
                    if (toDispatchItems[i] == s)
                      toDispatchItems.RemoveAt(i);
                  }
                }
              }

              if (!CheckRunning() && LootBag.Serial != World.Player.Backpack.Serial && World.Player.Hits >= World.Player.MaxHits)
              {
                for (int i = lootitems.Count - 1; i >= 0; i--)
                {
                  UOItem item = new UOItem(lootitems[i]);

                  if (item.Exist && item.Container == World.Player.Backpack.Serial)
                  {
                    if (item.Move(65000, LootBag))
                      lootitems.RemoveAt(i);

                    Game.Wait();
                    break;
                  }
                  else
                    lootitems.RemoveAt(i);
                }
              }
            }

            #endregion

          }
          #endregion


          if (!CheckRunning())
          {
            #region Frozen

            if (CalebConfig.HandleFrozen)
            {
              if (Journal.Contains(true, "You are frozen and can not move"/*, "You can't reach that"*/, "You are caught in the web"))
              {
                UO.Cast(StandardSpell.MagicArrow, Aliases.Self);
                Game.Wait(1250);
                handled = true;
              }
            }

            #endregion

            #region healing

            if (CalebConfig.AutoHeal)
            {
              if ((DateTime.Now - lastCleanBandageCheck).TotalMilliseconds > 5000 )
              {
                lastCleanBandageCheck = DateTime.Now;

                if (!Healing.CleanBandage.Exist)
                {
                  UOItem waterBasin = World.Player.Backpack.AllItems.FindType(Healing.VAL_WashBasin.Graphic, Healing.VAL_WashBasin.Color);
                  if (waterBasin.Exist)
                  {
                    UOItem bloodyBandages = World.Player.FindType(Healing.VAL_BloodyBandage_1.Graphic, Healing.VAL_BloodyBandage_1.Color);
                    if (!bloodyBandages.Exist)
                      bloodyBandages = World.Player.FindType(Healing.VAL_BloodyBandage_2.Graphic, Healing.VAL_BloodyBandage_2.Color);

                    if (bloodyBandages.Exist && bloodyBandages.Amount > 10)
                    {
                      int moveAmount = -1;

                      if (Game.CurrentGame.CurrentPlayer.PlayerSubClass == PlayerSubClass.Medic)
                        moveAmount = bloodyBandages.Amount - 100;
                      else
                        moveAmount = bloodyBandages.Amount - 10;

                      if (moveAmount > 0 && bloodyBandages.Move((ushort)moveAmount, World.Player.Backpack, 10, 10))
                      {
                        Game.Wait(350);
                        UO.WaitTargetObject(waterBasin);
                        bloodyBandages.Use();
                        Game.Wait(350);
                        bloodyBandages.Move(bloodyBandages.Amount, bloodyBandages.Container);
                        Game.Wait(350);
                      }
                      Game.PrintMessage("Cistim bandy");

                      handled = true;
                    }
                  }
                }
              }

              if ((CalebConfig.HealType == AutoHealType.Automatic || (CalebConfig.HealType == AutoHealType.SemiAutomatic && SemiHealOn)))
              {
                bool ableCurePoions = SkillsHelper.GetSkillValue("Healing").RealValue >= 850 || SkillsHelper.GetSkillValue("Veterinary").RealValue >= 850;
                UOCharacter result = null;
                int minDmg = CalebConfig.HealMinDamagePerc;
                bool useMinDmg = minDmg > 0;


                TargetAliasResult hover = Targeting.ParseTargets("hover");
                if (hover.IsValid && hover.Serial.IsValid)
                {
                  UOCharacter hoverChar = new UOCharacter(hover.Serial);
                  if (!hoverChar.Dead && hoverChar.Hits > 0 && (hoverChar.Notoriety != Notoriety.Murderer && hoverChar.Notoriety != Notoriety.Enemy) && hoverChar.Exist && hoverChar.Distance <= 5 && (hoverChar.Hits < hoverChar.MaxHits || hoverChar.Poisoned && ableCurePoions))
                    result = hoverChar;
                }

                if (result == null)
                {
                  List<CharHealPriority> chhpList = Healing.GetCharHealPriorityList(5.5, true, CalebConfig.HealMoby ? Game.CurrentGame.HealAlies : null);
                  if (useMinDmg)
                    chhpList = chhpList.Where(c => c.Char.Serial == World.Player.Serial || c.Char.Poisoned && ableCurePoions || c.DamagePerc >= minDmg).ToList();

                  var sortedList = chhpList.Where(c => !skip.Contains(c.Char.Serial) && !dennyMobs.Contains(c.Char.Serial)).ToList();

                  if (sortedList.Count == 0)
                  {
                    skip.Clear();
                    sortedList = chhpList.Where(c => !skip.Contains(c.Char.Serial) && !dennyMobs.Contains(c.Char.Serial)).ToList();
                  }

                  if (sortedList.Count > 0)
                    result = sortedList[0].Char;
                }

                if (result != null && result.ExistCust())
                {
                  if (Healing.CleanBandage.Exist)
                  {
                    bool poisoned = result.Poisoned;
                    bool damaged = result.Hits < result.MaxHits;

                    if (damaged || (ableCurePoions && poisoned) /*!poisonTry.Contains(result.Serial))*/)
                    {
                      if (result.Serial == World.Player.Serial || !World.Player.Hidden || SkillsHelper.GetSkillValue("Healing").RealValue > 950)//Jen clerda muze s hidu
                      {
                        Game.atackCounterEnabled = false;
                        Game.Wait(150);
                        if (!CheckRunning())
                        {

                          if (Debug)
                            Game.PrintMessage("HEAL ... " + ScriptRunning);


                          Banding = true;
                          HealInfo hinfo = Healing.BandSafe(result.Serial);
                          lastHealedSerial = result.Serial;
                          handled = true;
                          Banding = false;

                          if (hinfo.Success)
                          {
                            bandageSucces++;
                            bandageInfos.Add(hinfo);

                            if (poisoned && ableCurePoions && result.Hits >= result.MaxHits)
                            {
                              Healing.BandSafe(result.Serial);
                            }
                          }
                          else if (hinfo.Failed)
                            bandageFailed++;

                          if (hinfo.TryHealSummon)
                            dennyMobs.Add(result.Serial);

                          if (hinfo.CantReach)
                            skip.Add(result.Serial);

                          if (hinfo.FullHealed)
                            result.RequestStatus(100);


                          int bandTotal = bandageFailed + bandageSucces;

                          if (bandTotal > 0 && bandTotal % 4 == 0)
                            Game.PrintMessage(String.Format("BInfo: {0:N1}% [{4}], Avg: {1:N1}, Min: {2}, Max:{3}", (bandTotal > 0 ? ((decimal)bandageSucces / (decimal)bandTotal) * 100 : 0m), (bandageInfos.Count > 0 ? (decimal)bandageInfos.Sum(fo => fo.Gain) / (decimal)bandageInfos.Count : 0m), bandageInfos.Count > 0 ? bandageInfos.Min(fo => fo.Gain) : 0, bandageInfos.Count > 0 ? bandageInfos.Max(fo => fo.Gain) : 0, bandTotal), Game.Val_LightGreen);
                        }
                      }
                    }
                  }
                  else
                  {
                    Game.Wait(1000);
                  }
                }
                else
                {
                  skip.Clear();
                  //SemiHealOn = false;
                }
              }
            }

            #endregion

            #region poison

            if (CalebConfig.TrainPoison)
            {
              List<UOCharacter> characters = new List<UOCharacter>();
              characters.AddRange(World.Characters.ToArray());
              UOItem trainKit = World.Player.Backpack.AllItems.FindType(0x1837, 0x0000);

              if (trainKit.Exist && !World.Player.Hidden)
              {
                foreach (UOCharacter ch in characters)
                {
                  if (!poisonTrainDoneList.Contains(ch.Serial) &&
                    (ch.Notoriety != Notoriety.Guild && (ch.Notoriety == Notoriety.Enemy || ch.Notoriety == Notoriety.Murderer || ch.Notoriety == Notoriety.Criminal || ch.Notoriety == Notoriety.Neutral))
                    && ch.Serial != World.Player.Serial
                    && ch.Distance <= 1
                    && !Game.IsMob(ch.Serial)
                    && !Game.CurrentGame.IsAlie(ch.Serial)
                    && !Game.CurrentGame.IsHealAlie(ch.Serial)
                    //&& ch.Serial != Game.CurrentGame.CurrentPlayer.Mount
                    && !ch.Renamable 
                    && !(ItemLibrary.IsMostCommonPlayerSummon(ch)))
                  {
                    UO.WaitTargetObject(ch.Serial);
                    trainKit.Use();
                    
                    JournalEventWaiter jew = new JournalEventWaiter(true, "Uspesne jsi otravil svuj cil", "Uspesne jsi otravila svuj cil", "Kdyz se snazis pracovat s jedem, nemel bys delat nic jineho", "Na tomhle nemuzes trenovat", "Z teto nestvury se nic noveho nenaucis", "Na cili jiz nekdo trenoval");//todo 

                    if (jew.Wait(1500))
                    {
                      if (Journal.Contains(true, "Uspesne jsi otravil svuj cil", "Uspesne jsi otravila svuj cil", "Na tomhle nemuzes trenovat", "Z teto nestvury se nic noveho nenaucis", "Na cili jiz nekdo trenoval"))
                      {
                        ch.PrintMessage("[Poisn Done..]", Val_LightPurple);
                        poisonTrainDoneList.Add(ch.Serial);
                      }
                      else if (Journal.Contains(true, "Kdyz se snazis pracovat"))
                      {
                        //Game.CurrentGame.CurrentPlayer.SwitchWarmode();
                        Game.Wait(500);//war mod radsi ne aby to nefizovalo
                      }
                    }

                    handled = true;
                    break;
                  }
                }
              }
            }

            #endregion

          }

          if (handled)
            Journal.Clear();

          if (counter >= 1000)
          {
            //UO.Print("AutoHeal: " + CalebConfig.AutoHeal + " Web: " + CalebConfig.DestroySpiderWeb + " Moby: " + CalebConfig.HealMoby);
            counter = 0;
          }
        }
        catch (Exception ex)
        {

          UO.Print("Chyba v Main: ulozeno v caldebug.txt");

          try
          {
            File.WriteAllText("caldebug.txt", ex.Message + Environment. NewLine + ex.StackTrace, Encoding.UTF8);
          }
          catch 
          {

          }

          Game.Wait(2000);
        }
      }
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void UseTypeCust(Graphic gra, UOColor c, string target)
    {
      UseTypeCust(gra, c, target, "[Target..]", null);
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void UseTypeCust(Graphic gra, UOColor c, string target, string targetText, string playerText)
    {
      UseTypeCust(gra, c, target, targetText, playerText, Val_LightGreen, Val_LightGreen);
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void UseTypeCust(Graphic gra, UOColor c, string target, string targetText, string playerText, int? duration)
    {
      UseTypeCust(gra, c, target, targetText, playerText, Val_LightGreen, Val_LightGreen, duration);
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void UseTypeCust(Graphic gra, UOColor c, string target, string targetText, string playerText, UOColor targetTextColor, UOColor playerTextColor)
    {
      UseTypeCust(gra, c, target, targetText, playerText, targetTextColor, playerTextColor, null);
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void UseTypeCust(Graphic gra, UOColor c, string target, string targetText, string playerText, UOColor targetTextColor, UOColor playerTextColor, int? duration)
    {
      Targeting.ResetTarget();

      if (!String.IsNullOrEmpty(playerText))
        World.Player.PrintMessage(playerText, playerTextColor);

      Serial s = Targeting.ParseTargets(target);
      StaticTarget st = null;
      if (!s.IsValid && target != "none")
      {
        st = UIManager.Target();//todo, co TILE? 
        if (st.Type == TargetType.Object)
          s = st.Serial;
      }

      UOObject o = new UOObject(s);

      if (s.IsValid && o.Exist)
      {
        if (!String.IsNullOrEmpty(targetText))
          o.PrintMessage(targetText, targetTextColor);
        UO.WaitTargetObject(o);
      }
      else if (st != null)
      {
        UO.WaitTargetTile(st.X, st.Y, st.Z, st.Graphic);
      }
      
      Game.RunScriptCheck(duration.GetValueOrDefault(2500));
      if (gra == 0x0000)
      {
        foreach (UOItem item in World.Player.Backpack.Items)
        {
          if (item.Color == c)
          {
            item.Use();
            break;
          }
        }

        foreach (UOItem item in World.Player.Backpack.AllItems)
        {
          if (item.Color == c)
          {
            item.Use();
            break;
          }
        }
      }
      else
        UO.UseType(gra, c);
      
      if (target == "none")
      {
        Game.Wait(100);
        if (UIManager.CurrentState != UIManager.State.Ready)
        {
          Targeting.CancelClientTarget();
          Targeting.ResetTarget();
        }
      }
    }



    //---------------------------------------------------------------------------------------------

    //private static object SyncRoot;

    //private static void EnsureSyncRoot()
    //{
    //  if (SyncRoot == null)
    //    SyncRoot = new object();
    //}

    //---------------------------------------------------------------------------------------------

    [Executable]
    [BlockMultipleExecutions]
    public static void LootSwitch()
    {
      CalebConfig.Loot++;
      if (!Enum.IsDefined(typeof(LootType), CalebConfig.Loot))
        CalebConfig.Loot = LootType.None;

      if (CalebConfig.Loot != LootType.None)
        TryAddAllGroundItemToDispatch(25);
      else
        TryClearAllLootDispatchItems();

      Game.PrintMessage("AutoLoot: " + CalebConfig.Loot);
    }

    //---------------------------------------------------------------------------------------------
    public static bool Debug;

    [Executable]
    [BlockMultipleExecutions]
    public static void DebugSwitch()
    {
      Debug = !Debug;

      Game.PrintMessage("Debug: " + Debug);
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    [BlockMultipleExecutions]
    public static void GameModeSwitch()
    {
      if (Game.CurrentGame.Mode == GameMode.Dungeon)
        Game.CurrentGame.Mode = GameMode.Working;
      else
        Game.CurrentGame.Mode = GameMode.Dungeon;

      Game.PrintMessage("GameMode: " + Game.CurrentGame.Mode + (Game.CurrentGame.Mode == GameMode.Working ? " [Automatika vypnuta]" : ""));
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    [BlockMultipleExecutions]
    public void TrainPoisonSwitch()
    {
      CalebConfig.TrainPoison = !CalebConfig.TrainPoison;
      string message = "OFF";
      ushort color = 0x0025;

      if (SemiHealOn)
      {
        message = "ON";
        color = 0x0040;
      }
      Game.PrintMessage("TrainPoison " + message, color);
    }

    //---------------------------------------------------------------------------------------------

    private static System.Timers.Timer _t;
    private static System.Timers.Timer t
    {
      get
      {
        if (_t == null)
        {
          _t = new System.Timers.Timer();
          _t.Elapsed += TimerElapsed;
          _t.Interval = 5;
          _t.Enabled = false;
        }
        return _t;
      }
    }

    //---------------------------------------------------------------------------------------------
   // private static int waitTragetTries = 0;
    public static void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
      if (Debug)
      {
       // Game.PrintMessage("TimerElapsed: " + e.SignalTime);
      }

      if (RunscriptCounter.Current == null || RunscriptCounter.Current.CurrentDuration <= 0)
      {
        StopScript();
      }
    }

    //---------------------------------------------------------------------------------------------

    private void UIManager_StateChanged(object sender, EventArgs e)
    {
      if (!CheckRunning() && UIManager.CurrentState !=  UIManager.State.Ready)
      {
        RunScriptCheck(750);
      }
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void StopScript()
    {
      ScriptRunning = false;
      lastRunscriptDuration = 5;
      t.Stop();

      RunscriptCounter.Stop();

      if (Debug)
        Game.PrintMessage("RunScript END - " + (DateTime.Now - runStartTime).TotalMilliseconds);
    }

    //---------------------------------------------------------------------------------------------

    private static bool runScriptOffSwitch = false;

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void RunScriptSwitch()
    {
      int duration = 60000;
      string message = "PAUSE";
      ushort color = 0x0025;

      if (runScriptOffSwitch)
      {
        duration = 5;
        message = "ON";
        color = 0x0040;
        runScriptOffSwitch = false;
      }
      Game.PrintMessage(message, color);

      RunScript(duration);
    }

    //---------------------------------------------------------------------------------------------

    public static object lockRunObj = new object();
    [Executable]
    public static void RunScriptCheck(int duration)
    {
      if (!CheckRunning() || RunscriptCounter.Current == null || RunscriptCounter.Current.CurrentDuration < duration)// || (currentRc!= null && currentRc.Duration < duration))
        RunScript(duration);
    }

    //---------------------------------------------------------------------------------------------
    private static DateTime  runStartTime;
    private static int lastRunscriptDuration = 5;
    [Executable]
    public static void RunScript(int duration)
    {
      runStartTime = DateTime.Now;
      RunscriptCounter.Stop();
      Thread.Sleep(10);

      lastRunscriptDuration = duration + (int)(Core.CurrentLatency * 1.5);

      runmt();
      if (lastRunscriptDuration > 5)
        runScriptOffSwitch = true;
      ScriptRunning = true;
      t.Stop();
      t.Interval = 5;


      if (RunscriptCounter.Current != null)
      {
        RunscriptCounter.Current.RunscriptRun -= Rc_RunscriptRun;
      }

      RunscriptCounter currentRc = new RunscriptCounter();
      currentRc.Duration = lastRunscriptDuration;
      currentRc.RunscriptRun += Rc_RunscriptRun;
      currentRc.Run();
      t.Start();

      if (Debug)
        Game.PrintMessage("RunScript [" + lastRunscriptDuration + "]");
    }

    private static void Rc_RunscriptRun(object sender, RunscriptCounterEventArgs e)
    {
      if (Debug)
      {
        if (e.Count % 500 == 0 || e.IsStoped)
        {

          //Game.PrintMessage("" + e.Duration + " / " + (e.CurrentTime - e.StartTime).TotalMilliseconds + " / " + e.IsStoped, Game.Val_SuperLightYellow);
        }
      }

      if (Game.CurrentGame.RunscriptRun != null)
        Game.CurrentGame.RunscriptRun(sender, e);
    }


    public  RunscriptCounterEventHandler RunscriptRun;

    //---------------------------------------------------------------------------------------------

    public static bool ScriptRunning = false;
    public static bool CheckRunning()
    {
      return ScriptRunning;
    }

    //---------------------------------------------------------------------------------------------

    //private DateTime? lastShamanMorfTime;
    public static JournalEntry LastEntry;
    public static List<JournalEntry> EntryHistory;
    public static DateTime? lastRefreshRequest= null;

    public static bool atackCounterEnabled = false;

    protected void Journal_EntryAdded(object sender, JournalEntryAddedEventArgs e)
    {
      if (EntryHistory == null)
        EntryHistory = new List<JournalEntry>();

      EntryHistory.Add(e.Entry);
      LastEntry = e.Entry;

      string textSafe = e.Entry.Text + String.Empty;

      if (textSafe.Contains("World save has been initiated."))
      {
        this.WorldSaves.Add(DateTime.Now);
        Game.PrintMessage(String.Format("World Save add: {0:HH:mm:ss}", DateTime.Now), Val_GreenBlue);
      }
      else if (textSafe.ToLower().Contains("resync") && (e.Entry.Text).ToLower().Contains("initiated"))
      {
        this.Resyncs.Add(DateTime.Now);
        Game.PrintMessage(String.Format("_Resync add: {0:HH:mm:ss}", DateTime.Now), Val_GreenBlue);
      }
      else if (textSafe.ToLower().EndsWith("restart"))
      {
        World.Player.PrintMessage("[Reastart...!]", Game.Val_Radiation);
      }
      else if (textSafe.ToLower().Contains("resync") && (e.Entry.Text).ToLower().Contains("initiated"))
      {
        this.Resyncs.Add(DateTime.Now);
        Game.PrintMessage(String.Format("_Resync add: {0:HH:mm:ss}", DateTime.Now), Val_GreenBlue);
      }
      else if (textSafe.Contains("You have hidden yourself well"))
      {
        Hiding.HideRunning = false;
      }
      else if (textSafe.Contains("You can't seem to hide here"))
      {
        Hiding.HideRunning = false;
      }
      else if (textSafe.StartsWith("Vylecil jsi halucinace hraci"))
      {
        string name = Regex.Match(textSafe, "Vylecil jsi halucinace hraci (?<name>.*)!").Groups["name"].Value;

        bool found = false;
        if (!String.IsNullOrEmpty(name))
        {
          foreach (UOCharacter ch in World.Characters)
          {
            if (ch.Serial != World.Player.Serial && !String.IsNullOrEmpty(ch.Name) && ch.Name.ToLower().Trim() == name.ToLower().Trim())
            {
              ch.PrintMessage("[Haluze OK]", Game.Val_GreenBlue);
              found = true;
              break;
            }
          }
        }

        if (!found)
          World.Player.PrintMessage("[Haluze OK]", Game.Val_GreenBlue);
      }
      else if (textSafe.StartsWith("rsh:"))
      {
        //lastRefreshRequest
        UOCharacter chr = new UOCharacter(e.Entry.Serial);
        if (chr.Exist && (!lastRefreshRequest.HasValue || (DateTime.Now - lastRefreshRequest.Value).TotalMilliseconds > 350))
        {
          lastRefreshRequest = DateTime.Now;
          new UOCharacter(e.Entry.Serial).RequestStatus(150);
        }
      }

      #region attack
      else if 
        (
          textSafe.ToLower().StartsWith("You Miss".ToLower()) ||
          textSafe.ToLower().StartsWith("You smash".ToLower()) ||
          textSafe.ToLower().StartsWith("You score".ToLower()) ||
          textSafe.ToLower().StartsWith("You knock".ToLower()) ||
          textSafe.ToLower().StartsWith("You land".ToLower()) ||
          textSafe.ToLower().StartsWith("You hit".ToLower()) 
        )
      {
        if (atackCounterEnabled)
        {
          if (Targeting.lastAttackTime.HasValue)
          {
            double diff = (DateTime.Now - Targeting.lastAttackTime.Value).TotalMilliseconds;
          //  Game.PrintMessage(String.Format("Attack time: {0:N0}", diff));
            Targeting.lastDynamicAttackDelay = ((int)(diff / 50) * 50);
          }

          Game.RunScript(Targeting.DynamicAttackDelay);
          Targeting.lastAttackTime = DateTime.Now;
        }
      }
      #endregion

      #region magery
      else if (textSafe.Contains("The spell is not in your spellbook"))
      {
        if (Magery.CastingSpellInfo != null)
          Magery.CastingSpellInfo.NotYourSpell = true;

        Magery.Casting = false;
      }
      else if (textSafe.Contains("You lack reagents for this spell"))
      {
        if (Magery.CastingSpellInfo != null)
          Magery.CastingSpellInfo.LackReagets = true;

        Magery.Casting = false;
      }
      else if (textSafe.Contains("You lack sufficient mana for this spell"))
      {
        if (Magery.CastingSpellInfo != null)
          Magery.CastingSpellInfo.LackMana = true;

        Magery.Casting = false;
      }
      else if (textSafe.Contains("You can't cast"))
      {
        if (Magery.CastingSpellInfo != null)
          Magery.CastingSpellInfo.CantCast = true;

        Magery.Casting = false;
      }
      else if (textSafe.Contains("You can't read that"))
      {
        if (Magery.CastingSpellInfo != null)
          Magery.CastingSpellInfo.CantRead = true;

        Magery.Casting = false;
      }
      else if (textSafe.Contains("The spell fizzles"))
      {
        if (Magery.CastingSpellInfo != null)
          Magery.CastingSpellInfo.Fizzles = true;

        if (Magery.CastingSpellInfo == null || (DateTime.Now - Magery.CastingSpellInfo.CastTime).TotalMilliseconds > 250)
          Magery.Casting = false;

        if (CalebConfig.CastMessageType != MessagePrintType.None)
        {
          if (Magery.CastingSpellInfo == null || (DateTime.Now - Magery.CastingSpellInfo.CastTime).TotalMilliseconds > 250)
            World.Player.PrintMessage("[Spell fizzles]", MessageType.Error);
        }
      }
      else if (textSafe.Contains("You can't see the target"))
      {
        if (Magery.CastingSpellInfo != null)
          Magery.CastingSpellInfo.CantSee = true;

        Magery.Casting = false;
      }
      else if (textSafe.Contains("Target is not in line of sight"))
      {
        if (Magery.CastingSpellInfo != null)
          Magery.CastingSpellInfo.NoInLineOfSight = true;

        Magery.Casting = false;
      }
      else if (textSafe.Contains("Jen se projdi po svych"))
      {
        if (Magery.CastingSpellInfo != null)
          Magery.CastingSpellInfo.CantCast = true;

        Magery.Casting = false;
      }
      else if (textSafe.Contains("Targeting Cancelled"))
      {
        if (Magery.CastingSpellInfo != null)
          Magery.CastingSpellInfo.TargetCancel = true;

        Magery.Casting = false;
      }
      //

      #endregion
      else if (textSafe.Contains("Pri praci s nadobou nemuzes delat neco jineho"))
      {
        //Targeting.ResetTarget();
      }
    }

    //---------------------------------------------------------------------------------------------

    [Command("pnwsi")]
    public void PrintNextWorldSaveInfo()
    {
      if (this.WorldSaves.Count > 0)
      {
        DateTime lastTime = this.LastWorldSaveTime.GetValueOrDefault();

        WebWorldSaveInfo info = WebWorldSaveTime.GetInfo(lastTime, 0);
        Game.PrintMessage(String.Format("WS za: {0} ({1:HH:mm})", info.NextTimeStr, info.NextTime), Val_GreenBlue);
      }
      else
        Game.PrintMessage("Neni info o WS", Val_GreenBlue);
    }

    //---------------------------------------------------------------------------------------------

    public bool WorldSave()
    {
      return this.WorldSave(DateTime.Now, 60);
    }

    //---------------------------------------------------------------------------------------------

    public bool WorldSave(DateTime date, int secondSpan)
    {
      if (this.WorldSaves.Count > 0)
      {
        TimeSpan ts = (TimeSpan)(date - this.WorldSaves[this.WorldSaves.Count - 1]);

        bool result = ts.TotalSeconds <= secondSpan;
        if (result)
          Game.PrintMessage("World Save pred: " + ts.TotalSeconds + "s", Val_GreenBlue);
        else
        {
          if (this.Resyncs.Count > 0)
          {
            ts = (TimeSpan)(date - this.Resyncs[this.Resyncs.Count - 1]);

            result = ts.TotalSeconds <= secondSpan;
            if (result)
              Game.PrintMessage("Resync pred: " + ts.TotalSeconds + "s", Val_GreenBlue);
          }
        }
        return result;
      }

      return false;
    }

    public GameMode Mode;
    //---------------------------------------------------------------------------------------------

    void Window_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
    {
      Cal.Engine.SaveDataBase();
    }


    //---------------------------------------------------------------------------------------------

    private static Game currentGame;
    public static Game CurrentGame
    {
      get { return currentGame; }
    }

    //---------------------------------------------------------------------------------------------

    private PlayerExtended currentPlayer;
    public PlayerExtended CurrentPlayer
    {
      get { return currentPlayer; }
    }

    //---------------------------------------------------------------------------------------------

    public static void Wait()
    {
      Wait(NormalWait);
    }

    //---------------------------------------------------------------------------------------------

    public static void Wait(int milliseconds)
    {
      Wait(milliseconds, false);
    }

    //---------------------------------------------------------------------------------------------

    public static void Wait(bool resetWSSychr)
    {
      Wait(NormalWait, false);
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void SwitchPauseScript()
    {
      PauseScript = !PauseScript;
      UO.Print("" + (PauseScript ? "Pause ON" : "Pause OFF"));
    }

    //---------------------------------------------------------------------------------------------

    private static bool CheckPause()
    {
      return PauseScript;
    }
    
    public static bool PauseScript = false;
    //---------------------------------------------------------------------------------------------

    public static void Wait(int milliseconds, bool resetWSSychr)
    {
      int wsSychr = 0;

      while (CheckPause())
      {
        UO.Wait(500);
      }


      if (Game.CurrentGame.Mode == GameMode.Working)
      {
        if (Game.CurrentGame.WorldSave())
        {
          wsSychr = 30000;
        }
      }

      //if (milliseconds < 500)
      //  milliseconds = 500;

      if (Core.CurrentLatency > 500 && Game.CurrentGame.Mode != GameMode.Working)
        UO.Wait(milliseconds + wsSychr);
      else
        UO.Wait(milliseconds + wsSychr + Core.CurrentLatency);
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void SendSystemText(string text)
    {
      string[] lines = text.Split('\n');
      for (int i = 0; i < lines.Length; i++)
      {
        string line = "";

        for (int x = 0; x < lines[i].Length; x++)
        {
          if (lines[i][x] >= ' ')
            line += lines[i][x];
        }


        if (text.Length > Core.MaxSpeechLenght)
          text = text.Remove(Core.MaxSpeechLenght);

        PacketWriter writer = new PacketWriter(0x9D);

        writer.WriteBlockSize();
        //writer.Write((byte)type);
        //writer.Write(color);
        //writer.Write((ushort)font);
        writer.WriteAsciiString("CSY", 4);
        writer.WriteUnicodeString(text);


        byte[] data = writer.GetBytes();

        Core.SendToServer(data, true);
      }
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void PrintSkillValue(string skillName)
    {
     SkillValue sk =  SkillsHelper.GetSkillValue(skillName);
      Game.PrintMessage(skillName + ": " + sk.RealValue + " / " + sk.Value + " / " + sk.MaxValue);

    }

    //---------------------------------------------------------------------------------------------

    [ClientMessageHandler(0xB1)]
    public CallbackResult OnGumpTarger(byte[] data, CallbackResult prevResult)
    {
      Game.RunScriptCheck(2750);

      return CallbackResult.Normal;
    }

    //---------------------------------------------------------------------------------------------
    //Serial: 0x401D0F6D  Position: 10.75.0  Flags: 0x0000  Color: 0x0B88  Graphic: 0x0EFA  Amount: 1  Layer: None Container: 0x4019287E GANGREL SPELL BOOL
    public static void OnUserAction(string commandName, StatusFormActionEventArgs e)
    {
      if (e.mobile.Serial == World.Player.Serial)
      {
        if (commandName == "A")
        {
          new Thread(new ThreadStart(StatusWrapper.ShowEnemyWrapper)).Start();
          new Thread(new ThreadStart(StatusWrapper.ShowMobWrapper)).Start();
          new Thread(new ThreadStart(StatusWrapper.ShowOtherWrapper)).Start();
          new Thread(new ThreadStart(StatusWrapper.ShowUtilityForm)).Start();



          //StatusWrapper.ShowMobWrapper();
          //StatusWrapper.ShowOtherWrapper();


        }
        else if (commandName == "B")
        {
         // World.Player.PrintMessage("Detecting Hidden", Game.Val_LightGreen);
         
          UO.UseSkill(StandardSkill.Hiding);
        }
        else if (commandName == "X")
        {
          Game.GameModeSwitch();
        }
        else if (commandName == "Y")
        {
          Game.SwitchPauseScript();
        }
      }
      else
      {

        if ((e.mobile.Notoriety == Notoriety.Murderer || e.mobile.Notoriety == Notoriety.Enemy) && !Game.CurrentGame.IsAlie(e.mobile.Serial))
        {

          if (commandName == "A")
          {
            Game.CurrentGame.CurrentPlayer.GetSkillInstance<Magery>().CastSpell(StandardSpell.MagicArrow, e.mobile, false, false);
          }
          else if (commandName == "B")//TODO vyresot mekal chytreji, treba pres spell book
          {
            StandardSpell? spell = null;

            if (Game.CurrentGame.CurrentPlayer.PlayerClass == PlayerClass.Mage || 
              Game.CurrentGame.CurrentPlayer.PlayerClass == PlayerClass.Necromancer ||
                Game.CurrentGame.CurrentPlayer.PlayerClass == PlayerClass.Baset ||
                  Game.CurrentGame.CurrentPlayer.PlayerClass == PlayerClass.Cleric ||
                  Game.CurrentGame.CurrentPlayer.PlayerClass == PlayerClass.Vampire ||
                  Game.CurrentGame.CurrentPlayer.PlayerClass == PlayerClass.Ranger && Game.CurrentGame.CurrentPlayer.PlayerSubClass == PlayerSubClass.Druid
              )
            {
              spell = StandardSpell.Paralyze;
            }

            if (spell != null)
            {
              Game.CurrentGame.CurrentPlayer.GetSkillInstance<Magery>().CastSpell(spell.Value, e.mobile, false, false);
            }
          }
          else if (commandName == "X")//TODO vyresot mekal chytreji, treba pres spell book
          {
            StandardSpell? spell = null;

            if (Game.CurrentGame.CurrentPlayer.PlayerClass == PlayerClass.Mage || 
              Game.CurrentGame.CurrentPlayer.PlayerClass == PlayerClass.Necromancer  ||
              Game.CurrentGame.CurrentPlayer.PlayerClass == PlayerClass.Baset ||
              Game.CurrentGame.CurrentPlayer.PlayerClass == PlayerClass.Cleric
              )
            {
              spell = StandardSpell.Dispel;
            }

            if (spell != null)
            {
              Game.CurrentGame.CurrentPlayer.GetSkillInstance<Magery>().CastSpell(spell.Value, e.mobile, false, false);
            }
          }
          else if (commandName == "Y")//TODO vyresot mekal chytreji, treba pres spell book
          {
            StandardSpell? spell = null;

            if (Game.CurrentGame.CurrentPlayer.PlayerClass == PlayerClass.Mage || Game.CurrentGame.CurrentPlayer.PlayerClass == PlayerClass.Necromancer)
            {
              spell = StandardSpell.Paralyze;
            }
            else if (Game.CurrentGame.CurrentPlayer.PlayerClass == PlayerClass.Baset)
            {
              spell = StandardSpell.BladeSpirit;
            }

            if (spell != null)
            {
              Game.CurrentGame.CurrentPlayer.GetSkillInstance<Magery>().CastSpell(spell.Value, e.mobile, true, false);
            }
          }
        }
        else
        {

          if (commandName == "A")
          {
            Game.CurrentGame.CurrentPlayer.GetSkillInstance<Magery>().CastSpell(StandardSpell.MagicArrow, e.mobile, false, false);
          }
          else if (commandName == "B")//TODO vyresot mekal chytreji, treba pres spell book
          {
            StandardSpell? spell = null;
            StandardSpell? altSpell = null;

            if (Game.CurrentGame.CurrentPlayer.PlayerClass == PlayerClass.Vampire)
            {
              altSpell = spell = StandardSpell.Agility;

            }
            else if (Game.CurrentGame.CurrentPlayer.PlayerClass == PlayerClass.Mage || Game.CurrentGame.CurrentPlayer.PlayerClass == PlayerClass.Necromancer)
            {
              altSpell = spell = StandardSpell.Dispel;
            }
            else if (Game.CurrentGame.CurrentPlayer.PlayerClass == PlayerClass.Cleric)
            {
              //
              if (Game.CurrentGame.CurrentPlayer.PlayerSubClass == PlayerSubClass.Medic)
              {
                altSpell = StandardSpell.Dispel;
                spell = StandardSpell.Bless;
              }
              else
              {
                altSpell = StandardSpell.Cunning;
                spell = StandardSpell.Strength;
              }
            }
            else if (Game.CurrentGame.CurrentPlayer.PlayerClass == PlayerClass.Paladin || Game.CurrentGame.CurrentPlayer.PlayerClass == PlayerClass.Baset)
            {
              if (Game.CurrentGame.CurrentPlayer.PlayerSubClass == PlayerSubClass.Teuton)
              {
                spell = StandardSpell.Bless;

                if (e.mobile.Model == 0x0004)
                  spell = StandardSpell.Agility;
              }
              else
                spell = StandardSpell.Strength;

              altSpell = StandardSpell.Cunning;
            }

            if (spell != null)
            {
              if (e.mobile.Model == 0x0004 || e.mobile.Model == 0x0027)//vampir garga / imp
                Game.CurrentGame.CurrentPlayer.GetSkillInstance<Magery>().CastSpell(altSpell.Value, e.mobile, false, false);
              else
                Game.CurrentGame.CurrentPlayer.GetSkillInstance<Magery>().CastSpell(spell.Value, e.mobile, false, false);
            }
          }
          else if (commandName == "X")
          {
            StandardSpell? spell = null;

            if (Game.CurrentGame.CurrentPlayer.PlayerClass == PlayerClass.Cleric || Game.CurrentGame.CurrentPlayer.PlayerClass == PlayerClass.Paladin || Game.CurrentGame.CurrentPlayer.PlayerClass == PlayerClass.Baset)
            {
              //
              if (Game.CurrentGame.CurrentPlayer.PlayerSubClass != PlayerSubClass.Medic)
                spell = StandardSpell.ReactiveArmor;
            }
            else if (Game.CurrentGame.CurrentPlayer.PlayerClass == PlayerClass.Mage || Game.CurrentGame.CurrentPlayer.PlayerClass == PlayerClass.Necromancer)
            {
              spell = StandardSpell.Reflection;
            }

            if (spell != null)
              Game.CurrentGame.CurrentPlayer.GetSkillInstance<Magery>().CastSpell(StandardSpell.ReactiveArmor, e.mobile, true, false);
            else
            {
              //Healing.BandSafe(e.mobile.Serial);
              Healing.CleanBandage.Use();
              UO.WaitTargetObject(e.mobile.Serial);
              e.mobile.PrintMessage("Zde BAND", 0x0053);

            }
          }
          else if (commandName == "Y")
          {
            Game.Wait(250);
            Game.RunScript(6000);
            e.mobile.RequestStatus(350);

            if (e.mobile.Hits <= 0)
              Game.CurrentGame.CurrentPlayer.GetSkillInstance<Magery>().CastSpell(StandardSpell.Ressurection, e.mobile, true, false);
          }
        }
      }
    }

    //---------------------------------------------------------------------------------------------

    public static int ClickWait { get { return 150; } }
    public static int SmallestWait { get { return 250; } }
    public static int SmallWait { get { return 400; } }
    public static int NormalWait { get { return 500; } }
    //public static int MediumWait { get { return 1000; } }
    //public static int MidWait { get { return 750; } }

    //---------------------------------------------------------------------------------------------

    public static List<T> MergeLists<T>(params List<T>[] lists) where T : UOObject
    {
      List<T> mergeList = new List<T>();
      Hashtable helper = new Hashtable();

      foreach (List<T> list in lists)
      {
        foreach (T item in list)
        {
          if (helper[item.Serial] == null)
          {
            mergeList.Add(item);
            helper[item.Serial] = item;
          }
        }
      }
      return mergeList;
    }

    //---------------------------------------------------------------------------------------------

    public static UOColor GetAlieColorByHits(Serial s)
    {
      UOColor result = new UOColor(0x023b);//43-47

      UOCharacter ch = new UOCharacter(s);
      if (s.IsValid && ch.Exist && ch.Hits > 0)
      {
        double h = ch.Hits;
        double mh = ch.MaxHits;
        double perc = h / mh;

        if (perc >= 0.8)
          result = new UOColor(0x003e);
        else if (perc >= 0.6)
          result = new UOColor(0x003f);
        else if (perc >= 0.4)
          result = new UOColor(0x0040);
        else if (perc >= 0.2)
          result = new UOColor(0x0041);
        else if (perc >= 0.1)
          result = new UOColor(0x0042);
      }
      return result;
    }

    //---------------------------------------------------------------------------------------------

    public static UOColor GetEnemyColorByHits(Serial s)
    {
      UOColor result = new UOColor(0x021d); //0x021d nearDead
                                            //25-29
      UOCharacter ch = new UOCharacter(s);
      if (s.IsValid && ch.Exist && ch.Hits > 0)
      {
        double h = ch.Hits;
        double mh = ch.MaxHits;
        double perc = h / mh;

        if (perc >= 0.8)
          result = new UOColor(0x0025);
        else if (perc >= 0.6)
          result = new UOColor(0x0026);
        else if (perc >= 0.4)
          result = new UOColor(0x0027);
        else if (perc >= 0.2)
          result = new UOColor(0x0028);
        else if (perc >= 0.1)
          result = new UOColor(0x0029);
      }

      return result;
    }

    //---------------------------------------------------------------------------------------------

    public static UOColor GetMessageTypeColor(MessageType type)
    {
      UOColor infoColor = Val_PureWhite; //0x0B1D;//0x0B31;// Game.Val_SuperLightYellow;
      UOColor warningColor = Val_PureOrange;//0x0B30;// 0x0B32;//0x0B91;//Game.Val_LightPurple;//  0x013a;//Game.Val_LightOrange;
      UOColor errorColor = Val_LightPurple;//0x0B8E;//0x0B34;//Game.Val_LightRed;


      //0x0B27
      UOColor color = infoColor;
      if (type == MessageType.Warning)
        color = warningColor;
      else if (type == MessageType.Error)
        color = errorColor;

      return color;
    }

    //---------------------------------------------------------------------------------------------

    public static void PrintMessage(string message)
    {
      PrintMessage(message, MessageType.Info);
    }

    //---------------------------------------------------------------------------------------------

    public static void PrintMessage(string message, MessageType type)
    {
      PrintMessage(message, type, new object[0]);
    }

    //---------------------------------------------------------------------------------------------

    public static void PrintMessage(string message, MessageType type, params object[] args)
    {
      PrintMessage(message, Game.GetMessageTypeColor(type), args);
    }

    //---------------------------------------------------------------------------------------------

    public static void PrintMessage(string message, UOColor color)
    {
      UO.Print(color, message);
    }

    //---------------------------------------------------------------------------------------------

    public static void PrintMessage(string message, UOColor color, params object[] args)
    {
      UO.Print(color, message, args);
    }


    //---------------------------------------------------------------------------------------------
    private UOPlayer player;
    private int lastHits;
    private int lastMana;
    private int lastStam;
    private int lastStr;
    private int lastInt;
    private int lastDex;

    private void Player_Changed(object sender, ObjectChangedEventArgs e)
    {
      if (player.Hits != lastHits || player.Mana != lastMana || player.Stamina != lastStam || player.Strenght != lastStr || player.Intelligence != lastInt || player.Strenght != lastDex)
      {
        if (player.Mana != lastMana)
        {
          int diff = player.Mana - lastMana;

          if (diff < 0)
          {
            if (Magery.Casting)
            {
              if (Game.Debug)
                Game.PrintMessage("Manadown");
              Journal.Clear();
              if (Magery.CastingSpellInfo != null && Journal.WaitForText(true, 100, "The spell fizzles"))
                Magery.CastingSpellInfo.Fizzles = true;

              Magery.Casting = false;
            }
          }
          else
          {
          }
        }

        lastHits = player.Hits;
        lastMana = player.Mana;
        lastStam = player.Stamina;

        lastStr = player.Strenght;
        lastInt = player.Intelligence;
        lastDex = player.Dexterity;
      }
    }

    //---------------------------------------------------------------------------------------------
  }

  public class DurationElapsedEventArgs : EventArgs
  {
    public int CurrentDuration = 0;
    public int DurationCounter = 0;
  }

  public enum GameMode
  {
    Dungeon = 1,
    Working = 2
  }

  public enum LootType
  {
    None = 0,
    Quick = 1,
    QuickCut = 2,
    OpenCorpse = 3
  }

  public enum RenameType
  {
    None = 0,
    OnAppeared = 1,
    MainMethod = 2,
    Booth = 4
  }

  public enum AutoHealType
  {
    None = 0,
    Automatic = 1,
    SemiAutomatic = 2
  }

  public enum MessagePrintType
  {
    None = 0,
    Default = 1,
    Console = 2
  }

  //---------------------------------------------------------------------------------------------

  public class LootCorpseInfo
  {
    public DateTime LastTry;
    public int Tries = 0;
    public IUOPosition PlayerPosition;
  }

  //---------------------------------------------------------------------------------------------

  public class StatusRequestInfo
  {
    public DateTime LastRequestTime;
    public UOCharacter Character;

  }

  //---------------------------------------------------------------------------------------------

  public class CastSpellInfo
  {
    public CastSpellInfo(StandardSpell spell) : this(spell, false)
    {
    }

    public CastSpellInfo(StandardSpell spell, bool scrool) : this(spell, scrool, false)
    {
    }

    public CastSpellInfo(StandardSpell spell, bool scrool, bool silence)
    {
      Spell = spell;
      CastTime = DateTime.Now;
      Circle = Magery.GetSpellCircle(Spell);
      Timeout = Magery.GetCircleRunscriptTime(Circle);
      IsScrool = scrool;
      Silence = silence;
    }

    public int Circle;
    public StandardSpell Spell;
    public DateTime CastTime;
    public int Timeout;
    public bool IsScrool = false;
    public bool Silence = false;

    public bool NotYourSpell = false;
    public bool LackReagets = false;
    public bool LackMana = false;
    public bool CantCast = false;
    public bool CantRead = false;
    public bool Fizzles = false;
    public bool CantSee = false;
    public bool NoInLineOfSight = false;
    public bool TargetCancel = false;

    public bool IsRunning
    {
      get { return !HasError && CastRunDuration <= Timeout; }
    }


    public double CastRunDuration
    {
      get { return (DateTime.Now - CastTime).TotalMilliseconds;  }
    }


    public bool HasError
    {
      get
      {
        return
          NotYourSpell ||
           LackReagets ||
            LackMana ||
             CantCast ||
              CantRead ||
               Fizzles ||
                CantSee ||
                 NoInLineOfSight ||
                 TargetCancel;

      }
    }

  }



  //---------------------------------------------------------------------------------------------




}
