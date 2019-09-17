using Caleb.Library;
using CalExtension.PlayerRoles;
using CalExtension.Skills;
using CalExtension.UI.Status;
using CalExtension.UOExtensions;
using Phoenix;
using Phoenix.Runtime;
using Phoenix.WorldData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace CalExtension
{
  public class PlayerExtended
  {
    public PlayerExtended()
    {
      this.OnInit();
    }
    //---------------------------------------------------------------------------------------------

    private void OnInit()
    {
    }



    //---------------------------------------------------------------------------------------------

    

    //---------------------------------------------------------------------------------------------

    public UOPlayer Player
    {
      get { return (UOPlayer)(World.Player ?? new UOCharacter(Serial.Invalid)); }
    }

    //---------------------------------------------------------------------------------------------

    private Robot robot;
    public Robot Robot
    {
      get
      {
        if (this.robot == null)
          this.robot = new Robot();
        return this.robot;
      }
    }

    //---------------------------------------------------------------------------------------------

    private Hashtable skills;

    public T GetSkillInstance<T>() where T : Skill
    {
      Hashtable ht = new Hashtable();

      if (this.skills == null)
        this.skills = Hashtable.Synchronized(ht);


      T skill = this.skills[typeof(T) + ""] as T;

      if (skill == null)
      {
        skill = Activator.CreateInstance(typeof(T)) as T;
        this.skills[typeof(T) + ""] = skill;
      }

      return skill;
    }

    //---------------------------------------------------------------------------------------------

    private Hashtable playerRoles;

    public T GetPlayerRoleInstance<T>() where T : PlayerRole
    {
      Hashtable ht = new Hashtable();

      if (this.playerRoles == null)
        this.playerRoles = Hashtable.Synchronized(ht);


      T skill = this.playerRoles[typeof(T) + ""] as T;

      if (skill == null)
      {
        skill = Activator.CreateInstance(typeof(T)) as T;
        this.playerRoles[typeof(T) + ""] = skill;
      }

      return skill;
    }

    //---------------------------------------------------------------------------------------------

    public void SwitchWarmode()
    {
      bool origStaus = World.Player.Warmode;
      World.Player.ChangeWarmode(WarmodeChange.Switch);
      Game.Wait(25);
      World.Player.ChangeWarmode(origStaus ? WarmodeChange.War : WarmodeChange.Peace);
      Game.Wait(25);
    }

    //---------------------------------------------------------------------------------------------

    public void EnsureWarMode()
    {
      if (!World.Player.Warmode && !Game.CheckRunning())
        World.Player.ChangeWarmode(WarmodeChange.War);
    }
    


    //---------------------------------------------------------------------------------------------

    public void Say(string text, ushort color)
    {
      UO.Say(color, text);  
    }

    //---------------------------------------------------------------------------------------------

    private PlayerClass? playerClass;
    public PlayerClass PlayerClass
    {
      get
      {
        if (this.playerClass == null)
        {
          this.playerClass = GetCurrentPlayerClass();
          Game.PrintMessage("Class: " + this.playerClass);
        }
        return this.playerClass.Value;
      }
    }

    //---------------------------------------------------------------------------------------------

    private PlayerSubClass? playerSubClass;
    public PlayerSubClass PlayerSubClass
    {
      get
      {
        if (this.playerSubClass == null)
        {
          this.playerSubClass = GetCurrentPlayerSubClass();
          Game.PrintMessage("SubClass: " + this.playerSubClass);
        }
        return this.playerSubClass.Value;
      }
    }

    //---------------------------------------------------------------------------------------------

    public static PlayerClass GetCurrentPlayerClass()
    {
      //TODO detekce podle spellbooku, zatim jedina moznost
      if (!World.Player.Backpack.Opened)
      {
        World.Player.Backpack.Use();
        Game.Wait(300);
      }

      UOItem book = World.Player.Backpack.AllItems.FindType(0x0EFA);//SpellBook
      PlayerClass spec = PlayerClass.Invalid;

      if (book.Color == 0x0021)
        spec = PlayerClass.Mage;
      else if (book.Color == 0x0455 || book.Color == 0x0413)
        spec = PlayerClass.Necromancer;
      else if (book.Color == 0x0026 || book.Color == 0x0B88 || book.Color == 0x0B89 || book.Color == 0x0B85)//??, gangrel, brujah, tremere
        spec = PlayerClass.Vampire;
      else if (book.Color == 0x0037) //Warrior
        spec = PlayerClass.Warrior;
      else if (book.Color == 0x0835 || book.Color == 0x0B2F || book.Color == 0x07D0) //obyc, monk, medik
        spec = PlayerClass.Cleric;
      else if (book.Color == 0x0482)
        spec = PlayerClass.Baset;
      else if (book.Color == 0x00BD)//0x016F  
        spec = PlayerClass.Paladin;
      else if (book.Color == 0x016F)//0x016F  Golemaster
        spec = PlayerClass.Craftsman;
      else if (book.Color == 0x0B78 || book.Color == 0x0856)//0x0B78  Druid, ostro
        spec = PlayerClass.Ranger;

      
      //  

      //      UO.Print(CalStatusMessage.Val_InfoColor, "Player Specialization: " + spec + " - " + book.Color);

      return spec;
    }

    //---------------------------------------------------------------------------------------------

    public static PlayerSubClass GetCurrentPlayerSubClass()
    {
      //TODO detekce podle spellbooku, zatim jedina moznost
      if (!World.Player.Backpack.Opened)
      {
        World.Player.Backpack.Use();
        Game.Wait(300);
      }

      UOItem book = World.Player.Backpack.AllItems.FindType(0x0EFA);//SpellBook
      PlayerSubClass spec = PlayerSubClass.None;

      if (book.Color == 0x0021)
        spec = PlayerSubClass.None;//mage
      else if (book.Color == 0x0455 || book.Color == 0x0413)
        spec = PlayerSubClass.None;//nekro
      else if (book.Color == 0x0B88 || book.Color == 0x0B89 || book.Color == 0x0B85)//??, gangrel, brujah, tremere
        spec = PlayerSubClass.Gangrel;
      else if (book.Color == 0x0B89)
        spec = PlayerSubClass.Brujah;
      else if (book.Color == 0x0B85)
        spec = PlayerSubClass.Tremere;
      else if (book.Color == 0x0B2F)
        spec = PlayerSubClass.Monk;
      else if (book.Color == 0x07D0)
        spec = PlayerSubClass.Medic;
      else if (book.Color == 0x0493)
        spec = PlayerSubClass.Bishop;
      else if (book.Color == 0x00BD)
        spec = PlayerSubClass.Teuton;

     //else if (book.Color == 0x016F)//0x016F  
      //  spec = PlayerSubClass.Craftsman;
      else if (book.Color == 0x0B78)//0x0B78  Druid, ostro
        spec = PlayerSubClass.Druid;
      else if (book.Color == 0x0856)//0x0B78  Druid, ostro
        spec = PlayerSubClass.Sharpshooter;
      //  

      return spec;
    }


    //---------------------------------------------------------------------------------------------

    private string lastNotDummyConfigName;

    //---------------------------------------------------------------------------------------------

    [Executable]
    [BlockMultipleExecutions]
    public static void SwitchToDummyProfile(string dummyProfile)
    {
      PlayerExtended pe = Game.CurrentGame.CurrentPlayer;
      bool result = false;
      string exMesage = String.Empty;

      if (String.IsNullOrEmpty(pe.lastNotDummyConfigName) || Config.Profile.ProfileName != dummyProfile)
      {
        pe.lastNotDummyConfigName = Config.Profile.ProfileName;

        try
        {
          Config.Profile.ChangeProfile(dummyProfile);
          result = true;
        }
        catch (Exception ex)
        {
          exMesage = ex.Message;
          Config.Profile.ChangeProfile(pe.lastNotDummyConfigName); 
        }

        if (result)
          Game.PrintMessage("Dummy profile nahran");
      }
      else if (!String.IsNullOrEmpty(pe.lastNotDummyConfigName))
      {
        try
        {
          Config.Profile.ChangeProfile(pe.lastNotDummyConfigName);
          result = true;
        }
        catch (Exception ex)
        {
          exMesage = ex.Message;
          Config.Profile.ChangeProfile("Default");
        }

        if (result)
          Game.PrintMessage("Predchozi profile nahran");
      }

      if (!result)
      {
        Game.PrintMessage("SwitchToDummyProfile Chyba: " + exMesage, MessageType.Warning);
      }
      
      //Config.Profile.ChangeProfile(profile);
    }

    //---------------------------------------------------------------------------------------------

    [Executable("PlayerGoByPath")]
    public static bool GoByPath(string path)
    {
      return GoByPath(path, false, false);
    }
    //---------------------------------------------------------------------------------------------

    [Executable("PlayerGoByPath")]
    public static bool GoByPath(string path, bool reverse, bool noKop)
    {
      string[] options = path.Split('|');
      int button = -1;
      string bookType = "r";

      int index = 0;

      if (!noKop)
      {
        if (!String.IsNullOrEmpty(options[0]) && Regex.IsMatch(options[0], "(?<booktype>[a-z])(?<button>\\d{1,2})"))
        {
          Match m = Regex.Match(options[0], "(?<booktype>[a-z])(?<button>\\d{1,2})");
          bookType = m.Groups["booktype"].Value.ToLower();
          button = Int32.Parse(m.Groups["button"].Value);
          index = 1;
        }

        if (button > -1)
        {
          string book = "RuneBookUse";
          if (bookType == "t")
          {
            Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["TravelBookUse"], 1);
            UO.Wait(1000);
            book = "TravelBookUse";
          }
          else if (bookType == "c")
          {
            Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["CestovniKnihaUse"], 1);
            UO.Wait(1000);
            book = "CestovniKnihaUse";
          }



          bool teleported = false;
          while (!teleported)
          {
            UO.DeleteJournal();

            Phoenix.Runtime.RuntimeCore.Executions.Execute(RuntimeCore.ExecutableList[book], button);
            Game.Wait(500);
            if (!World.Player.Hidden)
              UO.UseSkill("Hiding");

            UO.Print("Cekam na kop.. nehybat");

            if (Journal.WaitForText(true, 2000, "Nesmis vykonavat zadnou akci"))
            {
              Game.CurrentGame.CurrentPlayer.SwitchWarmode();
              Game.Wait(1000);
            }
            else if (Journal.WaitForText(true, 120000, "You have been teleported"))
              teleported = true;

            if (Game.CurrentGame.WorldSave())
            {
              UO.Print("WS opakovani kopu za 45s");
              Game.Wait(45000);
              if (bookType == "t")
              {
                Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["TravelBookUse"], 1);
                UO.Wait(1000);
              }
              Game.Wait(500);
            }
          }
        }
      }

      List<string> pathList = new List<string>();
      for (int i = index; i < options.Length; i++)
        pathList.Add(options[i]);

      if (reverse)
        pathList.Reverse();

      foreach (string opt in pathList)
      {
        if (UO.Dead)
          return false;

        string[] parm = opt.Split('.');

        UOPositionBase pos = new UOPositionBase(ushort.Parse(parm[0]), ushort.Parse(parm[1]), (ushort)0);

        int distance = parm.Length > 2 ? UOExtensions.Utils.ToNullInt(parm[2]).GetValueOrDefault(1) : 1;
        int gotries = parm.Length > 3 ? UOExtensions.Utils.ToNullInt(parm[3]).GetValueOrDefault(1000) : 1000;

        Robot r = new Robot();
        r.UseTryGoOnly = true;
        r.UseMinWait = true;
        r.UseRun = true;
        r.SearchSuqareSize = 450;

        if (r.GoTo(pos, distance, gotries))
        {
          if (parm[parm.Length - 1].ToLower() == "opendoor")
          {
            ItemHelper.OpenDoorAll();
            Game.Wait();
          }
          else if (parm[parm.Length - 1].ToLower() == "useopendoor")
          {
            ItemHelper.OpenDoor();
            Game.Wait();
          }
        }
        else if (gotries < 5)
          return false;

      }
      return true;
    }

    //---------------------------------------------------------------------------------------------

    #region exec

    [Executable]
    [BlockMultipleExecutions]
    public static void OpenInventory()
    {
      World.Player.Backpack.Use();
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    [BlockMultipleExecutions]
    public static void OpenPaperdoll()
    {
      World.Player.Use();
    }

    #endregion

    //---------------------------------------------------------------------------------------------
  }

  //---------------------------------------------------------------------------------------------

  public enum PlayerClass: byte
  {
    Invalid = 0,
    Warrior = 1,
    Mage = 2,
    Necromancer = 3,
    Vampire = 4,
    Ranger = 5,
    Craftsman = 6,
    Paladin = 7,
    Cleric = 8,
    Baset = 9,
    Krysa = 10,
    Vlkodlak = 11
  }

  //---------------------------------------------------------------------------------------------

  public enum PlayerSubClass
  {
    None = 0,
    Medic = 1,
    Bishop = 2,
    Monk = 3,
    GolemMaster = 4,
    MagicMiner = 5,
    Iskarot = 6,
    Teuton = 7,
    MagicPaladin = 8,
    Druid = 8,
    Sharpshooter = 9,
    Shaman = 10,
    Brujah = 11,
    Gangrel = 12,
    Tremere = 13,
    Destroyer = 11,
    Guardian = 12,
    WitchHuner = 13

  }


  //---------------------------------------------------------------------------------------------
}
