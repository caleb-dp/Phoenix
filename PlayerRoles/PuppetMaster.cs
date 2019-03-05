using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;
using Caleb.Library;
using System.Threading;
using CalExtension.Skills;
using CalExtension.UOExtensions;
using CalExtension.UI.Status;
using Caleb.Library.CAL.Business;

namespace CalExtension.PlayerRoles
{
  public class PuppetMaster : PlayerRole
  {
    public object SyncRoot;

    public PuppetMaster()
    {
      this.OnInit();
    }

    //---------------------------------------------------------------------------------------------

    private MobCollection Mobs;
    private Mob currentMob;
    protected string selectedCommand = PuppetMaster.Commands[0];
    protected string selectedStrategy = PuppetMaster.Strategies[0];

    //---------------------------------------------------------------------------------------------

    protected string SelectedCommandSet
    {
      set
      {
        this.selectedCommand = value;
        Game.CurrentGame.Messages.Add("PuppetMaster Command: " + value);
      }
    }

    //---------------------------------------------------------------------------------------------

    protected string SelectedStrategySet
    {
      set
      {
        this.selectedStrategy = value;
        Game.CurrentGame.Messages.Add("PuppetMaster Strategy: " + value);
      }
    }

    //---------------------------------------------------------------------------------------------

    public void TryAddMob(object sender, CharacterAppearedEventArgs e)
    {
      //Pro frantu na zachovani laststatusu tam kde byl
      Serial laststatus = Aliases.GetObject("laststatus");

      string name = String.Empty;
      this.CheckRemove();
      
      UOCharacter mobChar = new UOCharacter(e.Serial);
//      UO.Print("{0}, {1}, {2}", mobChar.Model, mobChar.Color, mobChar.Description);

      //if (true)//ItemLibrary.PlayerSummons.Contains(mobChar.Model, mobChar.Color) || ItemLibrary.PlayerKlamaci.Contains(mobChar.Model, mobChar.Color))//Dodelat podporu i pro animal boxy
      if (mobChar.Model != 0x0190  )//clovek preskakujeme
      {
        if (mobChar.MaxHits < 0)
          mobChar.RequestStatus(5000);

        if (mobChar.MaxHits > -1)
        {
         
          if (!PuppetMaster.DenyModels.Contains(mobChar.Model) && mobChar.Renamable)
          {
            if (!String.IsNullOrEmpty(name = FirstPosibleName()))
            {
              lock (this.SyncRoot)
              {
                Mob mob = null;
                bool addBar = false;

                if (this.Mobs[mobChar.Serial.ToString()] != null)
                {
                  mob = this.Mobs[mobChar.Serial.ToString()];

                  if (String.IsNullOrEmpty(mob.MobChar.Name))
                  {
                    mob.MobChar.Click();
                    Game.Wait();
                  }

                  if (String.IsNullOrEmpty(mob.MobChar.Name) && mob.MobChar.Name != mob.GivenName && mob.MobChar.Exist)
                  {
                    mob.GivenName = name;
                    mob.MobChar.Rename(name);
                    Game.CurrentGame.Messages.Add("ENSUREMOB: " + mob.GivenName);
                    addBar = true;
                  }
                }
                else
                {
                  Game.Wait();
                  mob = new Mob(mobChar, name);
                  Mobs.Add(mob);
                  Game.CurrentGame.Messages.Add("ADDMOB: " + mob.GivenName);
                  addBar = true;
                }

                if (addBar)
                  new StatusBar().Show(mob.MobChar.Serial);

              }
            }
            else
              Game.CurrentGame.Messages.Add(CalStatusMessageType.Error, "Dosly jmena pro Mobiky!");
          }
        }
        //else
        //  Game.CurrentGame.Messages.Add(CalStatusMessageType.Error, "Vyprsel timeout na zalozku MOB: " + e.Serial);
      }

      if (laststatus != Serial.Invalid)
        Aliases.SetObject("laststatus", laststatus);

      //World.CharacterAppeared += TryAddMob;
    }

    //---------------------------------------------------------------------------------------------

    public void TryAddMobManual()
    {
      UOCharacter mob = new UOCharacter(UIManager.TargetObject());
      if (mob.Exist)
      {
        this.TryAddMob(this, new CharacterAppearedEventArgs(mob.Serial));
      }
    }

    //---------------------------------------------------------------------------------------------

    public void CheckRemove()
    {
      lock (this.SyncRoot)
      {
        MobCollection resortedMobs = new MobCollection();
        foreach (Mob mob in this.Mobs)
        {
          if (mob.Exists)
            resortedMobs.Add(mob);
        }
        this.Mobs = resortedMobs;
        if (this.Mobs.Count == 0) currentMob = null;
      }
    }

    //---------------------------------------------------------------------------------------------

    protected void EnsureMobs()
    {
      foreach (UOCharacter character in World.Characters)
        TryAddMob(this, new CharacterAppearedEventArgs(character.Serial));
    }

    //---------------------------------------------------------------------------------------------

    public string FirstPosibleName()
    {
      foreach (string name in PossibleNames)
        if (!this.Mobs.Contains(name))
          return name;
      return "";
    }

    //---------------------------------------------------------------------------------------------

    protected bool SayCommand(string command)
    {
      if (this.Mobs.Count == 0)
        this.EnsureMobs();
      
      this.CheckRemove();

      if (currentMob == null)
        this.SelectNextMob();

      if (this.Mobs.Count > 0 && currentMob != null)
      {
        string finalCommand = command;

        if (String.IsNullOrEmpty(command))
          finalCommand = currentMob.GivenName + " " + selectedCommand;
        else
        {
          if (command.Trim().ToLower().StartsWith("all"))
            finalCommand = command;
          else
            finalCommand = currentMob.GivenName + " " + command;
        }

        Game.CurrentGame.CurrentPlayer.Say(finalCommand, 0x015d);
        this.SelectNextMob();

        return true;
      }
      else
      {
        Game.CurrentGame.Messages.Add("Vsichni mobici jsou pryc. " + currentMob);
        return false;
      }
    }

    //---------------------------------------------------------------------------------------------

    protected void SelectNextMob()
    {
      if (this.Mobs.Count > 0)
      {
        int index = Utils.GetSwitchIndex((currentMob != null ? this.Mobs.IndexOf(currentMob) : 0), 1, this.Mobs.Count);
        currentMob = this.Mobs[index];
      }
    }

    //---------------------------------------------------------------------------------------------

    public void MoveCommandNext(int direction)
    {
      int index = Utils.GetSwitchIndex(Commands.IndexOf(this.selectedCommand), direction, Commands.Count);
      this.SelectedCommandSet = Commands[index];
    }

    //---------------------------------------------------------------------------------------------

    public void MoveStrategyNext(int direction)
    {
      int index = Utils.GetSwitchIndex(Strategies.IndexOf(this.selectedStrategy), direction, Strategies.Count);
      this.SelectedStrategySet = Strategies[index];
    }


    //---------------------------------------------------------------------------------------------

    public void MobAutoControl()
    {
      if (this.Mobs.Count == 0)
        this.EnsureMobs();

      Serial target = Serial.Invalid;

      if (this.selectedStrategy == "KillNearest")
      {
        target = Characters.GetNearestEnemy().Serial;
        Game.CurrentGame.CurrentPlayer.Messages.Add("KillNearest");
      }
      else if (this.selectedStrategy == "KillWeakest")
      {
        target = Characters.GetWeakestEnemy().Serial;
        Game.CurrentGame.CurrentPlayer.Messages.Add("KillWeakest");
      }
      else if (this.selectedStrategy == "KillLast")
      {
        target = Aliases.LastTarget;
        Game.CurrentGame.CurrentPlayer.Messages.Add("KillLast");
      }
      else if (this.selectedStrategy == "KillAttack")
      {
        
        if (!Aliases.LastAttack.IsValid || !new UOCharacter(Aliases.LastAttack).Exist)
          new Targeting().Find();

        target = Aliases.LastAttack;
        Game.CurrentGame.CurrentPlayer.Messages.Add("KillAttack");
      }
      else if (this.selectedStrategy == "KillSelect")
      {
        target = UIManager.TargetObject();
        Game.CurrentGame.CurrentPlayer.Messages.Add("KillSelect");
      }
      else if (this.selectedStrategy == "KillStatusOrAttack")
      {
        target = Aliases.GetObject("LastStatusManual");

        if (!target.IsValid || !new UOCharacter(target).Exist)
        { 
          if (!Aliases.LastAttack.IsValid || !new UOCharacter(Aliases.LastAttack).Exist)
            new Targeting().Find();

          target = Aliases.LastAttack;
        }
        Game.CurrentGame.CurrentPlayer.Messages.Add("KillStatusOrAttack");
      }

      UOCharacter enemy = new UOCharacter(target);

      if (!enemy.RequestStatus(5000))
      {
        Game.CurrentGame.Messages.Add("Vyprsel Timeout na zalozku enemyho");
        return;
      }

      Game.CurrentGame.Messages.Add(String.Format("TARGET TO KILL: {0}, {1}/{2}", enemy.Name, enemy.Hits, enemy.MaxHits));
      Targeting.SelectNPCInClient(enemy.Serial);

      int counter = 0;
      int wait = 385;
      int incr = 75;

      while (enemy.Exist && enemy.Hits > 0)
      {
        if (counter > 0)
          UO.Wait(wait);

        if (wait >= 1000)
        {
          wait = 763;
          //wait = 425;
          //incr = 75;
        }

        if (!Game.CheckRunning())
        {
          if (!this.SayCommand("Kill"))
            return;

          UO.WaitTargetObject(enemy.Serial);
        }
        else
          wait = 550;

        if (Journal.Contains(true, "Speech spam detected"))
        {
          Journal.Clear();
          Game.Wait(2500);
        }

        if (incr < 150)
          incr = incr + 25;

        if (wait != 763)
          wait = wait + incr + Core.Latency;
        counter++;
      }

      Game.Wait(250);
      new Targeting().Find();
    }

    //---------------------------------------------------------------------------------------------

    //public void VyhodKlamak()
    //{
    //  UOItem itemKlamak = new UOItem(Serial.Invalid);
    //  foreach (UOItemType klamak in ItemLibrary.ShrinkKlamaci)
    //  {
    //    itemKlamak = UO.Backpack.AllItems.FindType(klamak.Graphic, klamak.Color);
    //    if (itemKlamak.Exist)
    //      break;
    //    else
    //      itemKlamak = UO.Backpack.AllItems.FindType(klamak.Graphic);

    //    if (itemKlamak.Exist)
    //      break;
    //  }

    //  if (itemKlamak.Serial.IsValid)
    //  {
    //    World.CharacterAppeared += Game.CurrentGame.CurrentPlayer.GetPlayerRoleInstance<MobMaster>().TryAddMob;

    //    Game.CurrentGame.CurrentPlayer.SwitchWarmode();
    //    itemKlamak.Use();
    //    Game.CurrentGame.CurrentPlayer.Messages.Add("Vyhazuji kalamaka");

    //  }
    //  else
    //  {
    //    Game.CurrentGame.CurrentPlayer.Messages.Add("Neni klamak");
    //  }
    //}

    //---------------------------------------------------------------------------------------------

    public void ShrinkniKlamaky()
    {
      Game.CurrentGame.Messages.Add("ShrinkniKlamaky ...");
      UOItem shrinkKad = World.Player.Backpack.AllItems.FindType(0x1843, 0x0724);

      if (shrinkKad.Exist)
      {
        foreach (UOCharacter klamak in World.Characters)
        {
          if (klamak.Serial == World.Player.Serial)
            continue;

          if (klamak.Distance <= 3)
          {
            if (String.IsNullOrEmpty(klamak.Name))
            {
              klamak.Click();
              Game.Wait(50);
            }

            if (klamak.Renamable)
            {
//              Game.CurrentGame.Messages.Add("Shrink 1: " + klamak.Name);
              UO.WaitTargetObject(klamak);
              shrinkKad.Use();
              Game.Wait(300);
            }
            else
            {
//              Game.CurrentGame.Messages.Add("Neni klamak: " + klamak.Name);
              if (klamak.RequestStatus(500))
              {
                if (klamak.Renamable)
                {
 //                 Game.CurrentGame.Messages.Add("Shrink 2: " + klamak.Name);
                  UO.WaitTargetObject(klamak);
                  shrinkKad.Use();
                  Game.Wait(300);

                }
                else
                {
//                  Game.CurrentGame.Messages.Add("Stale neni klamak: " + klamak.Name);
                }

              }
            }
          }
        }

        Game.Wait(100);

        foreach (UOItem item in World.Ground)
        {

          if (item.Distance < 5)
          {
            bool isKlamak = false;
            foreach (Graphic g in ItemLibrary.ShrinkKlamaciArray)
            {
              if (item.Graphic == g)
                isKlamak = true;
            }

            if (isKlamak)
            {
              item.Click();
              Game.Wait(50);
            }

            //            Game.CurrentGame.Messages.Add("Item: " + item.Name + " Distance: " + item.Distance + " IsKlamak: " + (Array.IndexOf(ItemLibrary.ShrinkKlamaci.GraphicArray, item.Graphic) > -1 || Array.IndexOf(Taming.ShrinkKlamaci, item.Graphic) > -1) + " isKlamak: " + isKlamak);

            if (Array.IndexOf(ItemLibrary.ShrinkKlamaci.GraphicArray, item.Graphic) > -1 || Array.IndexOf(ItemLibrary.ShrinkKlamaciArray, item.Graphic) > -1 || isKlamak)
            {
              item.Move(1, (ushort)(World.Player.X + 1), (ushort)(World.Player.Y + 1), item.Z);
              Game.Wait(250);
              item.Move(1, World.Player.Backpack, 30, 30);
              Game.Wait(250);
            }
          }
          Game.Wait(50);
        }
      }
      else
        Game.CurrentGame.Messages.Add("Neni shrink kad!");
    }

    //---------------------------------------------------------------------------------------------

    public void HealujKlamaky(int tries)
    {
      Game.CurrentGame.Messages.Add("HealujKlamaky ...");
      if (tries == 0)
        tries = 10000;

      foreach (UOCharacter klamak in World.Characters)
      {
        if (klamak.Distance <= 3 && klamak.Renamable)
        {
          klamak.Print(klamak.Hits);

          for (int i = 0; i < tries; i ++ )
          {
            if (Game.CurrentGame.CurrentPlayer.GetSkillInstance<Healing>().BandageCharacter(klamak))
              break;
          }
        }
      }
    }

    //---------------------------------------------------------------------------------------------

    protected void OnInit()
    {
      this.SyncRoot = new object();
      this.Mobs = new MobCollection();
    }

    //---------------------------------------------------------------------------------------------

    public static List<Graphic> DenyModels { get { return new List<Graphic> { 0x00DC/*LAMA*/ }; } }

    //---------------------------------------------------------------------------------------------

    public static List<string> PossibleNames 
    {
      get 
      { 
        return new List<string> { "Vrahoun", "MrFiluta", "Psirena", "VelkyUchyl", "Demon", "Andel", "Magor", "Caleb", "Stranger", "Gumak", "Death", "Nigth", "Titanus" }; //TODO z konfigu
      } 
    }

    //---------------------------------------------------------------------------------------------

    public static List<string> Commands
    {
      get { return new List<string>() { "Go", "Stay", "Come", "Kill", }; }
    }

    //---------------------------------------------------------------------------------------------

    public static List<string> Strategies
    {
      get { return new List<string>() { "KillStatusOrAttack", "KillAttack", "KillNearest", "KillSelect", "KillWeakest", "KillLast", "KillAll" }; }
    }

    //---------------------------------------------------------------------------------------------

    #region exec

    [Executable("MoveCommandNext")]
    public static void ExecMoveCommandNext(int direction)
    {
      Game.CurrentGame.CurrentPlayer.GetPlayerRoleInstance<PuppetMaster>().MoveCommandNext(direction);
    }

    //---------------------------------------------------------------------------------------------

    [Executable("MoveStrategyNext")]
    public static void ExecMoveStrategyNext(int direction)
    {
      Game.CurrentGame.CurrentPlayer.GetPlayerRoleInstance<PuppetMaster>().MoveStrategyNext(direction);
    }

    //---------------------------------------------------------------------------------------------

    [Executable("SayCommand")]
    public static void ExecSayCommand(string command)
    {
      Game.CurrentGame.CurrentPlayer.GetPlayerRoleInstance<PuppetMaster>().SayCommand(command);
    }

    //---------------------------------------------------------------------------------------------

    [Executable("SayCommand")]
    public static void ExecSayCommand()
    {
      Game.CurrentGame.CurrentPlayer.GetPlayerRoleInstance<PuppetMaster>().SayCommand(null);
    }

    //---------------------------------------------------------------------------------------------

    [Executable("MobAutoControl")]
    [BlockMultipleExecutions]
    public static void ExecMobAutoControl()
    {
      Game.CurrentGame.CurrentPlayer.GetPlayerRoleInstance<PuppetMaster>().MobAutoControl();
    }

    //---------------------------------------------------------------------------------------------

    //[Executable("VyhodKlamak")]
    //[BlockMultipleExecutions]
    //public static void ExecVyhodKlamak()
    //{
    //  Game.CurrentGame.CurrentPlayer.GetPlayerRoleInstance<PuppetMaster>().VyhodKlamak();
    //}

    //---------------------------------------------------------------------------------------------

    [Executable("ShrinkniKlamaky")]
    [BlockMultipleExecutions]
    public static void ExecShrinkniKlamaky()
    {
      Game.CurrentGame.CurrentPlayer.GetPlayerRoleInstance<PuppetMaster>().ShrinkniKlamaky();
    }

    //---------------------------------------------------------------------------------------------

    [Executable("HealujKlamaky")]
    [BlockMultipleExecutions]
    public static void ExecHealujKlamaky(int tries)
    {
      Game.CurrentGame.CurrentPlayer.GetPlayerRoleInstance<PuppetMaster>().HealujKlamaky(tries);
    }

    //---------------------------------------------------------------------------------------------

    [Executable("TryAddMobManual")]
    [BlockMultipleExecutions]
    public static void ExecTryAddMobManual()
    {
      Game.CurrentGame.CurrentPlayer.GetPlayerRoleInstance<PuppetMaster>().TryAddMobManual();
    }

    //---------------------------------------------------------------------------------------------


    #endregion

    //---------------------------------------------------------------------------------------------
  }
}
