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
using System.Linq;
using System.Collections;

namespace CalExtension.PlayerRoles
{
  public class MobMaster : PlayerRole
  {
    public object SyncRoot;

    //---------------------------------------------------------------------------------------------
    public MobMaster()
    {
      this.OnInit();
      if (current == null)
      {
        World.ItemAdded += World_ItemAdded;
        World.ItemUpdated += World_ItemUpdated;
      }
    }

    //---------------------------------------------------------------------------------------------

    protected void OnInit()
    {
      this.SyncRoot = new object();
    }

    //---------------------------------------------------------------------------------------------

    public bool debug { get { return Game.Debug; } }
    public void TryAddMob(object sender, CharacterAppearedEventArgs e)
    {
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public void MobCome()
    {
      UO.Say(Game.Val_GreenBlue, "all come");
      ResetMobQueue();
    }

    //---------------------------------------------------------------------------------------------
    

    [Executable]
    public void MobComeInActive()
    {
      MobKillInfo current = GetCurrentInActive();

      if (current != null && current.Mob.ExistCust())
        UO.Say(Game.Val_GreenBlue, current.Mob.Name + " come");
      else
        MobCome();
    }

    //---------------------------------------------------------------------------------------------
    Serial lastInactiveMob = Serial.Invalid;
    protected MobKillInfo GetCurrentInActive()
    {
      List<MobKillInfo> mobQueue = this.MobQueue;
      UOCharacter lastCh = new UOCharacter(Serial.Invalid);

      MobKillInfo current = null;
      MobKillInfo last = null;

      int startIndex = 0;

      for (int i = 0; i < mobQueue.Count; i++)
      {
        MobKillInfo k = mobQueue[i];
        if (lastInactiveMob.IsValid && lastInactiveMob == k.Mob.Serial)
        {
          if (i < mobQueue.Count - 1)
          {
            startIndex = i + 1;
            lastInactiveMob = Serial.Invalid;
          }
          else
            last = new MobKillInfo(new UOCharacter(lastInactiveMob), ResetTimerDefault);
        }

        if (!k.Enemy.ExistCust())
        {
          current = mobQueue[i];
          break;
        }
      }

      if (current == null)
      {
        for (int i = startIndex; i < mobQueue.Count; i++)
        {
          MobKillInfo k = mobQueue[i];
          if (!k.Enemy.ExistCust())
          {
            current = k;
            break;
          }
        }
      }

      if (current == null && last != null && last.NeedCommand())
        current = last;

      if (current != null)
        lastInactiveMob = current.Mob.Serial;

      return current;
    }



    //---------------------------------------------------------------------------------------------

    [Executable]
    public void MobStop()
    {
      UO.Say(Game.Val_GreenBlue, "all stop");

      ResetMobQueue();
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public void MobStopInActive()
    {
      MobKillInfo current = GetCurrentInActive();

      if (current != null && current.Mob.ExistCust())
        UO.Say(Game.Val_GreenBlue, current.Mob.Name + " stop");
      else
        MobStop();
    }

    //---------------------------------------------------------------------------------------------

    protected void ResetMobQueue()
    {
      Aliases.SetObject("LastAttackMobs", Serial.Invalid);
      Aliases.SetObject("LastStatusManualMobs", Serial.Invalid);

      if (Aliases.GetObject("LastStatusManual") == World.Player.Serial)
        Aliases.SetObject("LastStatusManual", Serial.Invalid);


      Aliases.SetObject("SelectedMob", Serial.Invalid);

      lastMob = Serial.Invalid;
      lastInactiveMob = Serial.Invalid;

      _mobQueue = null;
      //foreach (MobKillInfo k in MobQueue)
      //{
      //  k.CommandCount = 0;
      //  k.Enemy = new UOCharacter(Serial.Invalid);
      //}
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public void MobGo()
    {
      Serial mob = Aliases.GetObject("SelectedMob");
      UOCharacter ch = new UOCharacter(mob);
      string name = null;

      TargetInfo t = Targeting.GetTarget(""); 
      if (ch.Renamable && mob.IsValid && ch.Exist && ch.Distance < 18 && !String.IsNullOrEmpty(ch.Name))
      {
        ch.PrintMessage("[go...]");
        name = ch.Name;

        List<MobKillInfo> mobQueue = this.MobQueue;
        foreach   (MobKillInfo k in mobQueue)
        {
          if (k.Mob.Serial == ch.Serial)
          {
            k.CommandCount = 0;
            k.Enemy = new UOCharacter(Serial.Invalid);
          }
        }
      }

      string command = "all go";
      if (!String.IsNullOrEmpty(name))
        command = name + " go";

      if (t != null && t.Success)
      {
        UO.WaitTargetTile(t.StaticTarget.X, t.StaticTarget.Y, t.StaticTarget.Z, t.StaticTarget.Graphic);
        if (t.Character != null && t.Character.Exist && Game.IsMob(t.Character.Serial))
        {
          Aliases.SetObject("SelectedMob", t.Character.Serial);
        }
        UO.Say(Game.Val_GreenBlue, command);
      }



    }

    //---------------------------------------------------------------------------------------------

    Serial lastMob = Serial.Invalid;
    private List<MobKillInfo> _mobQueue;
    protected List<MobKillInfo> MobQueue
    {
      get
      {
        if (_mobQueue == null)
          _mobQueue = new List<MobKillInfo>();

        List<UOCharacter> mobs = new List<UOCharacter>();
        mobs.AddRange(Game.CurrentGame.Mobs.ToArray());

        for (int i = _mobQueue.Count - 1; i >= 0; i--)
        {
          MobKillInfo k = _mobQueue[i];
          if (!Game.IsMob(k.Mob.Serial))
          {
            _mobQueue[i].EnsureUnregisterEvent();
            _mobQueue.RemoveAt(i);
          }
        }

        foreach (UOCharacter ch in mobs)
        {
          bool found = false;
          foreach (MobKillInfo k in _mobQueue)
          {
            if (k.Mob.Serial == ch.Serial)
            {
              found = true;
              break;
            }
          }

          if (!found)
          {
            _mobQueue.Insert(0, new MobKillInfo(ch, ResetTimerDefault));
          }
        }


        return  _mobQueue;
      }
      set { _mobQueue = value; }
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public void MobKill()
    {
      MobKill(ResetTimerDefault);
    }
    protected static int ResetTimerDefault = 15000;
    [Executable]
    public void MobKill(int resetTimer)
    {
      ResetTimerDefault = resetTimer;
      Game.CurrentGame.CurrentPlayer.EnsureWarMode();

      List<MobKillInfo> mobQueue = this.MobQueue;

      MobKillInfo current = null;
      MobKillInfo last = null;

      int startIndex = 0;

      for (int i = 0; i < mobQueue.Count; i++)
      {
        MobKillInfo k = mobQueue[i];
        if (lastMob.IsValid && lastMob == k.Mob.Serial)
        {
          //Game.PrintMessage("startIndex " + startIndex + " / " + Game.RenameSufix(k.Mob.Name), Game.Val_GreenBlue);
          if (i < mobQueue.Count - 1)
          {
            startIndex = i + 1;
            lastMob = Serial.Invalid;
          }
          else
            last = k;//new MobKillInfo(new UOCharacter(lastMob), resetTimer);
        }

        if (k.CommandCount == 0 && k.Mob.Serial != k.GetEnemyOrDefault().Serial)
        {
          //Game.PrintMessage("New FOUND " + k.Mob.Name + " / " + i, Game.Val_GreenBlue);
          current = mobQueue[i];
          break;
        }
      }

      if (current == null)
      {
        for (int i = startIndex; i < mobQueue.Count; i++)
        {
          MobKillInfo k = mobQueue[i];
          if (k.NeedCommand())
          {
            //Game.PrintMessage("New OLD " + k.Mob.Name + " / " + i, Game.Val_GreenBlue);
            current = k;
            break;
          }
        }
      }

      if (current == null && last != null && last.NeedCommand())
        current = last;

      //Game.PrintMessage("Mobs [" + mobQueue.Count + "]", Game.Val_GreenBlue);

      string command = null;
      UOColor sayColor = Game.Val_LightGreen;

      UOCharacter finalEnemy = new UOCharacter(Serial.Invalid);

      if (current != null)
      {



        command = current.MobName + " kill";
        current.CommandCount++;
        current.LastCommandTime = DateTime.Now;
        sayColor = Game.GetAlieColorByHits(current.Mob.Serial);
        current.Mob.PrintMessage(Rename.RenameSufix(current.Mob.Name) + (!String.IsNullOrEmpty(current.CommandReasonCode) && Game.Debug ? " [" + current.CommandReasonCode + "]" : ""), sayColor);//zatim pro debug
        
        if (!current.Enemy.ExistCust())
          current.Enemy = current.GetEnemy();

        finalEnemy = current.Enemy;

        lastMob = current.Mob.Serial;
      }
      else if (mobQueue.Count == 0)
      {
        command = "all kill";
      }

      if (!String.IsNullOrEmpty(command) && finalEnemy.ExistCust() && finalEnemy.Distance < 22)
      {
        Targeting.ResetTarget();
        Game.RunScriptCheck(250);
        Game.Wait(50);
        UO.WaitTargetObject(finalEnemy);

        UO.Say(sayColor, command);
      }

      if (finalEnemy.ExistCust())
      {
        if (finalEnemy.Hits < 0)
          finalEnemy.RequestStatus(150);



        finalEnemy.PrintMessage(/*String.Format(">>{0}/{1}<<", finalEnemy.Hits, finalEnemy.MaxHits)*/"*", Game.GetEnemyColorByHits(finalEnemy.Serial));
        new StatusBar().Show(finalEnemy.Serial);
      }
    }

    //---------------------------------------------------------------------------------------------

    public class MobKillInfo
    {

      //---------------------------------------------------------------------------------------------

      ~MobKillInfo()
      {
        EnsureUnregisterEvent();
      }

      //---------------------------------------------------------------------------------------------

      public MobKillInfo(UOCharacter mob, int resetTimer)
      {
        this.Mob = mob;
        this.resetTimer = resetTimer;
        this.Enemy = new UOCharacter(Serial.Invalid);
        this.Mob.Changed += Mob_Changed;
      }

      //---------------------------------------------------------------------------------------------

      private void Mob_Changed(object sender, ObjectChangedEventArgs e)
      {
        if (Game.Debug)
          Game.PrintMessage("Mob_Changed: " + Rename.RenameSufix(this.Mob.Name) + (String.Format("{0:n1}", (DateTime.Now - LastChangedTime).TotalMilliseconds)));

        LastChangedTime = DateTime.Now;
      }

      //---------------------------------------------------------------------------------------------

      public void EnsureUnregisterEvent()
      {
        this.Mob.Changed -= Mob_Changed;
      }

      //---------------------------------------------------------------------------------------------

      public int CommandCount = 0;
      public UOCharacter Mob;
      public UOCharacter Enemy;
      private int resetTimer = int.MaxValue;
      public DateTime LastCommandTime = DateTime.MinValue;
      public DateTime LastChangedTime = DateTime.MinValue;
      public bool ZeroPosition = false;
      public bool ToFar = false;
      public bool NotLookingAt = false;
      public bool Dragon = false;
      public bool TimerReset = false;
      public bool ChangeReset = false;
      public string CommandReasonCode = String.Empty;
      public string MobName
      {
        get
        {
          string name = String.Empty;

          if (this.Mob != null)
          {
            name = this.Mob.Name;
            if (Rename.IsRenamedByValidName(name))
              return name;

            MobRenameInfo renameInfo = Rename.GetRenameInfo(this.Mob.Serial);
            if (renameInfo && Rename.IsRenamedByValidName(renameInfo.NewName))
            {
              if (Game.Debug)
                Game.PrintMessage("MobName1: " + this.Mob.Serial + " / " + name + " / " + renameInfo.NewName);
              //?? uvidime zda se jeste nekdy objevi
            }
            else
            {
              if (Game.Debug)
                Game.PrintMessage("MobName2: " + this.Mob.Serial + " / " + name);
            }
          }

          return name;
        }
      }
    

      public bool IsAliveReach
      {
        get { return Mob.Exist && Mob.Distance < 30; }
      }

      private bool IsDragon(UOCharacter m)
      {
        return m.Model == 0x003C || m.Model == 0x000C;//0x003CDrake//0x000CDragon
      }

      public UOCharacter GetEnemyOrDefault()
      {
        return this.Enemy.ExistCust() ? this.Enemy : GetEnemy();
      }

      //---------------------------------------------------------------------------------------------

      public UOCharacter GetEnemy()
      {
        Serial target = Aliases.GetObject("LastStatusManual");//Aliases.GetObject("LastStatusManualMobs");
        UOCharacter enemy = new UOCharacter(target);
        if (!enemy.ExistCust())
          enemy = new UOCharacter(Aliases.LastAttack);//new UOCharacter(Aliases.GetObject("LastAttackMobs"));


        if (!enemy.ExistCust())
          enemy = new Targeting().SearchNearestEnemy(this.Mob.GetPosition());
        else
          enemy = new UOCharacter(target);

        if (enemy.ExistCust() && String.IsNullOrEmpty(enemy.Name) && !enemy.RequestStatus(200))
          Game.PrintMessage("Vyprsel Timeout na zalozku enemyho");

        return enemy;
      }

      //---------------------------------------------------------------------------------------------

      public bool NeedCommand()
      {

        UOPositionBase ep = new UOPositionBase(GetEnemyOrDefault().X, GetEnemyOrDefault().Y, 0);
        UOPositionBase mp = new UOPositionBase(Mob.X, Mob.Y, 0);


        this.ZeroPosition = Robot.GetRelativeVectorLength(mp, ep) == 0;
        this.ToFar = Robot.GetRelativeVectorLength(mp, ep) > 1.5;
        this.NotLookingAt = !Robot.LookingDirectlyAt(mp, ep, Mob.Direction);
        this.Dragon = IsDragon(this.Mob);
        this.TimerReset = (DateTime.Now - LastCommandTime).TotalMilliseconds > resetTimer;
        this.ChangeReset = (DateTime.Now - LastChangedTime).TotalMilliseconds > 8500;

        bool result = this.IsAliveReach && GetEnemyOrDefault().Serial != this.Mob.Serial &&  
          (
          this.ZeroPosition ||
          this.ToFar ||
          this.NotLookingAt ||
          this.Dragon ||
          this.TimerReset ||
          this.ChangeReset
          );

        if (result)
        {
          if (this.ZeroPosition)
          {
            if (Game.Debug)
              Game.PrintMessage(Rename.RenameSufix(Mob.Name) + " - Pozice 0 [" + this.CommandCount + "]", Game.Val_GreenBlue);

            this.CommandReasonCode = "Z";
          }
          else if (this.NotLookingAt)
          {
            if (Game.Debug)
              Game.PrintMessage(Rename.RenameSufix(Mob.Name) + " - Nekouka [" + this.CommandCount + "]", Game.Val_GreenBlue);
            this.CommandReasonCode = "N";
          }
          else if (this.ToFar)
          {
            if (Game.Debug)
              Game.PrintMessage(Rename.RenameSufix(Mob.Name) + " - Je Daleko [" + this.CommandCount + "]", Game.Val_GreenBlue);
            this.CommandReasonCode = "F";
          }
          else if (this.Dragon)
          {
            if (Game.Debug)
              Game.PrintMessage(Rename.RenameSufix(Mob.Name) + " - Drak [" + this.CommandCount + "]", Game.Val_GreenBlue);
            this.CommandReasonCode = "D";
          }
          else if (this.TimerReset)
          {
            if (Game.Debug)
              Game.PrintMessage(Rename.RenameSufix(Mob.Name) + " - Casovac [" + this.CommandCount + "] " + (String.Format("{0:n1}", (DateTime.Now - LastCommandTime).TotalMilliseconds)), Game.Val_GreenBlue);
            this.CommandReasonCode = "T";
          }
          else if (this.ChangeReset)
          {
            if (Game.Debug)
              Game.PrintMessage(Rename.RenameSufix(Mob.Name) + " - Change [" + this.CommandCount + "] " + (String.Format("{0:n1}", (DateTime.Now - LastChangedTime).TotalMilliseconds)), Game.Val_GreenBlue);
            this.CommandReasonCode = "C";
          }

        }

        return result;
      }
    }

    //---------------------------------------------------------------------------------------------

    protected UOItemTypeCollection CurrentKlamaks
    {
      get
      {
        UOItemTypeCollection items = new UOItemTypeCollection();

        foreach (UOItem item in World.Player.Backpack.AllItems)
        {
          if (Array.IndexOf(ItemLibrary.ShrinkMountTypes.GraphicArray, item.Graphic) > -1 || 
            Array.IndexOf(ItemLibrary.ShrinkPackTypes.GraphicArray, item.Graphic) > -1 ||
              item.Graphic == 0x2119 && (item.Color == 0x0BB5 || item.Color == 0x0530 ||  item.Color == 0x0B45)   //Color: 0x0B45  Graphic: 0x2119)//baset
            )
            continue;//Mounti / Packy

          if (!items.Contains(item.Graphic, item.Color) && Array.IndexOf(ItemLibrary.ShrinkKlamaciArray, item.Graphic) > -1)
          {
            UOItemType itemType = new UOItemType();
            itemType.Graphic = item.Graphic;
            itemType.Color = item.Color;
            itemType.Name = item.Name;

            if (String.IsNullOrEmpty(itemType.Name))
            {
              if (ItemLibrary.ShrinkKlamaci.FindItem(item.Graphic, item.Color) != null)
              {
                itemType.Name = ItemLibrary.ShrinkKlamaci.FindItem(item.Graphic, item.Color).Name;
              }
              else
              {
                item.Click();
                UO.Wait(Core.Latency + 100);
                itemType.Name = item.Name;
              }
            }

            items.Add(itemType);
          }
        }
        return items;
      }
    }

    //---------------------------------------------------------------------------------------------

    protected UOItemTypeCollection possibleKlamaks;
    protected UOItemTypeCollection PossibleKlamaks
    {
      get
      {
        lock (SyncRoot)
        {
          // UOItemTypeCollection possibleKlamaks = null;
          if (possibleKlamaks == null || this.CurrentKlamaks.Count != possibleKlamaks.Count)
          {
            possibleKlamaks = new UOItemTypeCollection();
            possibleKlamaks.AddRange(CurrentKlamaks);
          }
          return possibleKlamaks;
        }
      }
    }

    //---------------------------------------------------------------------------------------------

    protected UOItemType selectedKlamak;
    protected UOItemType SelectedKlamakSet
    {
      set
      {
        this.selectedKlamak = value;

        int count =  World.Player.Backpack.AllItems.Count(value.Graphic, value.Color);

        World.Player.PrintMessage(value.Name + " [" + count + "]", Game.Val_GreenBlue);
      }
    }

    //---------------------------------------------------------------------------------------------

    public void MoveKlamakNext(int direction)
    {
      if (PossibleKlamaks.Count > 0)
      {
        if (selectedKlamak == null)
          this.SelectedKlamakSet = PossibleKlamaks[0];
        else
        {
          int index = 0;

          for (int i = index; i < PossibleKlamaks.Count; i++)
          {
            if (PossibleKlamaks[i].Graphic == this.selectedKlamak.Graphic && PossibleKlamaks[i].Color == selectedKlamak.Color)
            {
              index = i + 1;
              break;
            }
          }

          if (index > PossibleKlamaks.Count - 1)
            index = 0;

          this.SelectedKlamakSet = PossibleKlamaks[index];
        }


      }
      else
        World.Player.PrintMessage("Nejsou klamaci");

    }

    //---------------------------------------------------------------------------------------------

    //Bizonek(23.07.2018 23:46):
    //Serial: 0x4027C552  Name: "Magni's special abilities"  Position: 30.3.0  Flags: 0x0000  Color: 0x0530  Graphic: 0x2119  Amount: 1  Layer: None Container: 0x4037F05A

    //Bizonek(23.07.2018 23:46):
    //Serial: 0x4031939E  Name: "Claws of Destruction (Magni)"  Position: 73.3.0  Flags: 0x0000  Color: 0x0BB5  Graphic: 0x2119  Amount: 1  Layer: None Container: 0x4037F05A
    //Color: 0x0B45  Graphic: 0x2119

    public UOItem NajdiKlamak()
    {
      UOItem itemKlamak = new UOItem(Serial.Invalid);
      if (selectedKlamak != null && UO.Backpack.AllItems.FindType(selectedKlamak.Graphic, selectedKlamak.Color).Exist)
      {
        itemKlamak = UO.Backpack.AllItems.FindType(selectedKlamak.Graphic, selectedKlamak.Color);
      }
      else if (selectedKlamak != null && UO.Backpack.AllItems.FindType(selectedKlamak.Graphic).Exist)
      {
        itemKlamak = UO.Backpack.AllItems.FindType(selectedKlamak.Graphic);
      }
      else
      {
        foreach (Graphic klamak in ItemLibrary.ShrinkKlamaciArray)
        {
          if (Array.IndexOf(ItemLibrary.ShrinkMountTypes.GraphicArray, klamak) > -1 || 
            Array.IndexOf(ItemLibrary.ShrinkPackTypes.GraphicArray, klamak) > -1
            

            )
            continue;//Mounti / Packy

          itemKlamak = UO.Backpack.AllItems.FindType(klamak);
          if (itemKlamak.Exist && itemKlamak.Graphic != 0x2119 && (itemKlamak.Color != 0x0BB5 || itemKlamak.Color != 0x0530 || itemKlamak.Color != 0x0B45))
            break;
        }
      }

      return itemKlamak;
    }

    //---------------------------------------------------------------------------------------------

    public void VyhodKlamak()
    {
      UOItem itemKlamak = NajdiKlamak();
      if (!itemKlamak.Exist || !itemKlamak.Serial.IsValid)
      {
        ItemHelper.OpenContainerRecursive(World.Player.Backpack);
        itemKlamak = NajdiKlamak();
      }

      if (itemKlamak.Exist && itemKlamak.Serial.IsValid)
      {
        Game.RunScriptCheck(650);
        World.Player.PrintMessage("Vyhazuji kalamaka");
        Game.CurrentGame.CurrentPlayer.SwitchWarmode();
        itemKlamak.Use();
      }
      else
      {
        World.Player.PrintMessage("Neni klamak");
      }
    }

    //---------------------------------------------------------------------------------------------

    public void VyhodKlamakNa()
    {
      VyhodKlamakNa(Serial.Invalid);
    }

    public void VyhodKlamakNa(Serial s)
    {
      UOItem itemKlamak = new UOItem(s);
      if (!itemKlamak.Exist)
        itemKlamak = NajdiKlamak();

      if (!itemKlamak.Exist || !itemKlamak.Serial.IsValid)
      {
        ItemHelper.OpenContainerRecursive(World.Player.Backpack);
        itemKlamak = NajdiKlamak();
      }

      if (itemKlamak.Exist && itemKlamak.Serial.IsValid)
      {
        Game.RunScriptCheck(1500);
        World.Player.PrintMessage("Kam klamaka?");
        StaticTarget st = UIManager.Target();
        if (st.X > 0)
        {
          Game.RunScriptCheck(1500);
          itemKlamak.Move(1, st.X, st.Y, st.Z);
          Game.Wait(400);
          Game.CurrentGame.CurrentPlayer.SwitchWarmode();
          itemKlamak.Use();
        }
      }
      else
      {
        World.Player.PrintMessage("Neni klamak");
      }
    }

    //---------------------------------------------------------------------------------------------
    private List<string> deny = new List<string>();
    public void ShrinkniKlamaky(string target)
    {
      UOItem shrinkKad = World.Player.Backpack.AllItems.FindType(0x1843, 0x0724);

      if (shrinkKad.Exist)
      {
        UOCharacter t = new UOCharacter(Serial.Invalid);

        if (!String.IsNullOrEmpty(target))
          t = new UOCharacter(Targeting.ParseTargets(target));

        if (!t.ExistCust())
        {
          List<UOCharacter> characters = new List<UOCharacter>();
          characters.AddRange(World.Characters.ToArray());
          var arr = characters.Where(
            ch => ch.Distance <= 2 &&
            ch.Serial != World.Player.Serial &&
            !deny.Contains(ch.GetUniqueKey()) &&
            Game.IsMob(ch.Serial)
            ).OrderBy(i => i.Hits).ToArray();


          Game.PrintMessage("ShrinkniKlamaky [" + arr.Count() + "]");

          foreach (UOCharacter ch in arr)
          {
            string name = ch.Name;
            if (!Game.IsMob(ch))
            {

              if (String.IsNullOrEmpty(name))
              {
                ch.Click();
                Game.Wait(50 + Core.CurrentLatency);
                name = ch.Name;
              }
            }

            t = ch;

            break;
          }
        }

        if (t.ExistCust())
        {
          Journal.Clear();

          bool success = false;

          if (Game.IsMob(t))
          {
            UO.WaitTargetObject(t);
            shrinkKad.Use();

            success = !Journal.WaitForText(true, 150, "Ale co to delas?");
          }
          else
          {
            if (t.RequestStatus(250 + Core.CurrentLatency))
            {
              if (Game.IsMob(t))
              {
                UO.WaitTargetObject(t);
                shrinkKad.Use();

                success = !Journal.WaitForText(true, 150, "Ale co to delas?");
              }
            }
          }

          if (!success)
            deny.Add(t.GetUniqueKey());
          else
          {
            if (!t.Exist)
            {
              List<StatusForm> delete = WindowManager.GetDefaultManager().OwnedWindows.OfType<StatusForm>().Where(sf => sf.MobileId == t.Serial).ToList();
              foreach (StatusForm sf in delete)
              {
                if (WindowManager.GetDefaultManager().InvokeRequired)
                  WindowManager.GetDefaultManager().BeginInvoke(sf.Close);
                else
                  sf.Close();
              }
            }
          }
          Game.Wait(150);
        }

        List<UOItem> ground = new List<UOItem>();
        ground.AddRange(World.Ground.ToArray());

        foreach (UOItem item in ground)
        {
          if (item.Distance < 4)
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

            if (Array.IndexOf(ItemLibrary.ShrinkKlamaci.GraphicArray, item.Graphic) > -1 || Array.IndexOf(ItemLibrary.ShrinkKlamaciArray, item.Graphic) > -1 || isKlamak)
            {
              item.Move(1, (ushort)(World.Player.X + 1), (ushort)(World.Player.Y + 1), item.Z);
              Game.Wait(250);
              item.Move(1, World.Player.Backpack, 30, 30);
            }
          }
        }
      }
      else
        World.Player.PrintMessage("[Neni shrink kad!]", MessageType.Warning);
    }

    //---------------------------------------------------------------------------------------------

    #region exec

    //---------------------------------------------------------------------------------------------

    [Executable("VyhodKlamak")]
    [BlockMultipleExecutions]
    public static void ExecVyhodKlamak()
    {
      Game.CurrentGame.CurrentPlayer.GetPlayerRoleInstance<MobMaster>().VyhodKlamak();
    }

    //---------------------------------------------------------------------------------------------

    [Executable("VyhodKlamakNa")]
    [BlockMultipleExecutions]
    public static void ExecVyhodKlamakNa()
    {
      Game.CurrentGame.CurrentPlayer.GetPlayerRoleInstance<MobMaster>().VyhodKlamakNa();
    }

    //---------------------------------------------------------------------------------------------

    [Executable("VyhodKlamakNa")]
    [BlockMultipleExecutions]
    public static void ExecVyhodKlamakNa(Serial s)
    {
      Game.CurrentGame.CurrentPlayer.GetPlayerRoleInstance<MobMaster>().VyhodKlamakNa(s);
    }

    //---------------------------------------------------------------------------------------------

    [Executable("MoveKlamakNext")]
    public static void ExecMoveKlamakNext(int direction)
    {
      Game.CurrentGame.CurrentPlayer.GetPlayerRoleInstance<MobMaster>().MoveKlamakNext(direction);
    }

    //---------------------------------------------------------------------------------------------

    [Executable("EnsureAllMobs")]
    [BlockMultipleExecutions]
    public static void EnsureAllMobs()
    {
      MobMaster mobMaster = Game.CurrentGame.CurrentPlayer.GetPlayerRoleInstance<MobMaster>();

      foreach (UOCharacter character in World.Characters)
      {
        if (character.Distance < 20)
        {
          mobMaster.TryAddMob(mobMaster, new CharacterAppearedEventArgs(character.Serial));
        }
      }
    }

    //---------------------------------------------------------------------------------------------

    [Executable("ShrinkniKlamaky")]
    [BlockMultipleExecutions]
    public static void ExecShrinkniKlamaky()
    {
      Game.CurrentGame.CurrentPlayer.GetPlayerRoleInstance<MobMaster>().ShrinkniKlamaky("hover");
    }

    //---------------------------------------------------------------------------------------------

    [Executable("ShrinkniKlamaky")]
    [BlockMultipleExecutions]
    public static void ExecShrinkniKlamaky(string target)
    {
      Game.CurrentGame.CurrentPlayer.GetPlayerRoleInstance<MobMaster>().ShrinkniKlamaky(target);
    }

    #endregion

    //---------------------------------------------------------------------------------------------

    #region GolemMater

    public static Graphic VAL_GolemGraphicShrink { get { return new Graphic(0x2610); } }
    public static Graphic VAL_GolemGraphic { get { return new Graphic(0x0053); } }

    //---------------------------------------------------------------------------------------------

    protected UOItemTypeCollection possbileGolems;
    protected UOItemTypeCollection PossibleGolems
    {
      get
      {
        lock (SyncRoot)
        {
          if (this.possbileGolems == null)
          {
            this.possbileGolems = new UOItemTypeCollection();
            foreach (UOItem item in World.Player.Backpack.AllItems)
            {
              if (item.Graphic == VAL_GolemGraphicShrink && item.Color != 0x0000 && Array.IndexOf(this.possbileGolems.ColorArray, item.Color) < 0)
              {
                UOItemType itemType = new UOItemType();
                itemType.Graphic = item.Graphic;
                itemType.Color = item.Color;
                if (String.IsNullOrEmpty(item.Name))
                {
                  item.Click();
                  Game.Wait();
                }
                itemType.Name = item.Name;

                this.possbileGolems.Add(itemType);
              }
            }
          }
          return this.possbileGolems;
        }
      }
    }

    //---------------------------------------------------------------------------------------------

    protected UOItemType selectedGolem;
    protected UOItemType SelectedGolemSet
    {
      set
      {
        this.selectedGolem = value;
        World.Player.PrintMessage(value.Name);
      }
    }

    //---------------------------------------------------------------------------------------------

    public void SummonSelectedGolem()
    {
      SummonSelectedGolem(null);
    }

    //---------------------------------------------------------------------------------------------

    public void SummonSelectedGolem(string target)
    {
      if (CalExtension.Abilities.GolemMaster.AdaHammer.Exist)
      {
        if (this.selectedGolem != null && World.Player.Backpack.AllItems.FindType(this.selectedGolem.Graphic, this.selectedGolem.Color).Exist)
        {
          UOItem shrinkGolem = World.Player.Backpack.AllItems.FindType(this.selectedGolem.Graphic, this.selectedGolem.Color);

          TargetInfo tInfo = Targeting.GetTarget(target);

          if (tInfo.Success)
          {
            World.CharacterAppeared += World_CharacterAppeared;
            shrinkGolem.Move(1, tInfo.StaticTarget.X, tInfo.StaticTarget.Y, tInfo.StaticTarget.Z);
            Game.Wait();
            UO.WaitTargetObject(shrinkGolem);
            CalExtension.Abilities.GolemMaster.AdaHammer.Use();
          }
        }
        else
        {
          Game.PrintMessage("Vybrany golem uz neni!");
        }
      }
      else
        World.Player.PrintMessage("[Neni adahammer..]");
    }

    //---------------------------------------------------------------------------------------------

    protected void World_CharacterAppeared(object sender, CharacterAppearedEventArgs e)
    {
      World.CharacterAppeared -= World_CharacterAppeared;
      UOCharacter appeared = new UOCharacter(e.Serial);
      if (appeared.Model == 0x0053 && !appeared.Renamable)
      {
        if (CalExtension.Abilities.GolemMaster.AdaHammer.Exist)
        {
          Targeting.ResetTarget();
          UO.WaitTargetObject(appeared);
          CalExtension.Abilities.GolemMaster.AdaHammer.Use();

          if (!Game.IsMob(appeared) && !Rename.IsMobRenamed(appeared))
          {
            appeared.Click();
            Game.Wait(200);

            if (appeared.RequestStatus(200))
            {
              Rename.RenameCharacter(appeared);
            }
          }
        }
      }
    }

    //---------------------------------------------------------------------------------------------

    private static MobMaster current;
    public static MobMaster Current
    {
      get
      {
        if (current == null)
          current = Game.CurrentGame.CurrentPlayer.GetPlayerRoleInstance<MobMaster>();

        return current;
      }
    }

    //---------------------------------------------------------------------------------------------

    protected void World_ItemUpdated(object sender, ObjectChangedEventArgs e)
    {
      lock (SyncRoot)
      {
        this.possbileGolems = null;
      }
    }

    //---------------------------------------------------------------------------------------------

    protected void World_ItemAdded(object sender, ObjectChangedEventArgs e)
    {
      lock (SyncRoot)
      {
        this.possbileGolems = null;
      }
    }

    //---------------------------------------------------------------------------------------------

    public void MoveGolemNext(int direction)
    {
      int selectedIndex = this.selectedGolem == null ? 0 : Array.IndexOf(PossibleGolems.ColorArray, this.selectedGolem.Color);

      int index = UOExtensions.Utils.GetSwitchIndex(selectedIndex, direction, PossibleGolems.Count);
      if (PossibleGolems.Count - 1 >= index)
        this.SelectedGolemSet = PossibleGolems[index];
      else
        World.Player.PrintMessage("[Nemas formy...]", MessageType.Warning);
    }

    //---------------------------------------------------------------------------------------------

    [Executable("MoveGolemNext")]
    public static void ExecMoveGolemNext(int direction)
    {
      Game.CurrentGame.CurrentPlayer.GetPlayerRoleInstance<MobMaster>().MoveGolemNext(direction);
    }

    //---------------------------------------------------------------------------------------------

    [Executable("SummonSelectedGolem")]
    public static void ExecSummonSelectedGolem()
    {
      Game.CurrentGame.CurrentPlayer.GetPlayerRoleInstance<MobMaster>().SummonSelectedGolem();
    }

    //---------------------------------------------------------------------------------------------

    [Executable("SummonSelectedGolem")]
    public static void ExecSummonSelectedGolem(string target)
    {
      Game.CurrentGame.CurrentPlayer.GetPlayerRoleInstance<MobMaster>().SummonSelectedGolem(target);
    }

    //---------------------------------------------------------------------------------------------

    #endregion

  }
}
