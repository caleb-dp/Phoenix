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
using CalExtension.UI.Status;
using CalExtension.PlayerRoles;

namespace CalExtension.UOExtensions
{
  public class Targeting : PlayerRole
  {
    //---------------------------------------------------------------------------------------------

    public void Find3()
    {
      SelectNextTargetEnemy(800);
    }

    //---------------------------------------------------------------------------------------------

    public UOCharacter SelectNextTargetEnemy(int resetTimer)
    {
      return SelectNextTarget(resetTimer, new Notoriety[] { Notoriety.Enemy, Notoriety.Murderer, Notoriety.Neutral, Notoriety.Guild });
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public UOCharacter SelectNextTarget(int resetTimer, string notorities)
    {
      List<Notoriety> notos = new List<Notoriety>();
      foreach (string s in notorities.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
      {
        try
        {
          Notoriety res = (Notoriety)Enum.Parse(typeof(Notoriety), s);
          notos.Add(res);
        }
        catch  
        {

        }
      }

      return SelectNextTarget(resetTimer, notos.ToArray());
    }

    //---------------------------------------------------------------------------------------------

    public UOCharacter SelectNextTarget(int resetTimer, Notoriety[] notos)
    {
      UOCharacter ch = SearchNextTarget(World.Player.GetPosition(), notos, resetTimer, true);
      Aliases.LastAttack = ch.Serial;
      Aliases.SetObject("LastAttackMobs", ch.Serial);

      if (ch.ExistCust())
        SelectNPCInClient(ch.Serial);

      return ch;
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public UOCharacter SearchNearestEnemy(IUOPosition postion)
    {
      return SearchNextTarget(postion, new Notoriety[] { Notoriety.Enemy, Notoriety.Murderer, Notoriety.Neutral, Notoriety.Guild }, -1, false);
    }

    //---------------------------------------------------------------------------------------------

    private DateTime lastSearch = DateTime.MinValue;
    List<uint> searchDoneList = new List<uint>();

    public UOCharacter SearchNextTarget(IUOPosition center, Notoriety[] notorities, int resetTimer, bool useDoneList)
    {
      UOCharacter result = new UOCharacter(Serial.Invalid);
      bool isReset = false;

      if (resetTimer > 0 && (DateTime.Now - lastSearch).TotalMilliseconds >= resetTimer)
      {
        if (useDoneList)
          searchDoneList = new List<uint>();
        isReset = true;
      }

      lastSearch = DateTime.Now;

      List<UOCharacter> list = new List<UOCharacter>();

      list.AddRange(World.Characters.Where(i =>
   i.GetDistance() <= 25 &&
   i.Serial != World.Player.Serial &&
   (i.Notoriety == Notoriety.Murderer || i.Notoriety == Notoriety.Enemy) &&
   notorities.Where(n => n == i.Notoriety).Count() > 0 &&
   !Game.CurrentGame.IsAlie(i.Serial) &&
   !Characters.IsSummon(i)).OrderBy(i => i.GetDistance(center)).ThenBy(i => i.Hits).ToList());

      list.AddRange(World.Characters.Where(i =>
   i.GetDistance() <= 25 &&
   i.Serial != World.Player.Serial &&
   i.Notoriety == Notoriety.Neutral &&
   notorities.Where(n => n == i.Notoriety).Count() > 0 &&
   !Game.CurrentGame.IsAlie(i.Serial) &&
   !Characters.IsSummon(i)).OrderBy(i => i.GetDistance(center)).ThenBy(i => i.Hits).ToList());

      list.AddRange(World.Characters.Where(i =>
   i.GetDistance() <= 25 &&
   i.Serial != World.Player.Serial &&
   (i.Notoriety == Notoriety.Guild || i.Notoriety == Notoriety.Innocent) &&
   notorities.Where(n => n == i.Notoriety).Count() > 0 &&
   !Game.CurrentGame.IsAlie(i.Serial) &&
   !Characters.IsSummon(i)).OrderBy(i => i.GetDistance(center)).ThenBy(i => i.Hits).ToList());


      if (useDoneList && list.Where(i => !searchDoneList.Contains(i.Serial)).ToList().Count > 0)
      {
        result = list.Where(i => !searchDoneList.Contains(i.Serial)).ToList()[0];
      }
      else
      {
        if (useDoneList)
          searchDoneList = new List<uint>();

        isReset = true;

        if (list.Count > 0)
          result = list[0];
      }

      if (useDoneList && result.ExistCust())
        searchDoneList.Add(result.Serial);

      if (list.Count > 1)
      {
        if (Game.Debug)
        {
          for (int i = 1; i < Math.Min(3, list.Count); i++)
          {
            list[i].PrintHitsMessage("[" + i + "]");
          }
        }
      }

      if (isReset && Game.Debug)
        Game.PrintMessage("TargetNext - Reset");

      return result;
    }

    //---------------------------------------------------------------------------------------------

    public static int? lastDynamicAttackDelay = null;
    public static DateTime? lastAttackTime;
    public static int DynamicAttackDelay
    {
      get
      {
        int minDelay = 1500;
        int maxDelay = 9000;

        int result = (DateTime.Now - lastAttackTime.GetValueOrDefault(DateTime.MinValue)).TotalSeconds > 9 ? maxDelay : lastDynamicAttackDelay.GetValueOrDefault(maxDelay);
        if (result < minDelay)
          result = minDelay;
        else if (result > maxDelay)
          result = maxDelay;

        int incr = CalebConfig.AttackDelay;

        if (result  >= maxDelay)
          incr = 0;

        return result + incr;
      }
    }

    //---------------------------------------------------------------------------------------------

    int counter = 0;
    Serial lastEnemy = Serial.Invalid;
    public void AttackSelectTarget(params string[] targetAlternates)
    {
      Game.RunScriptCheck(Targeting.DynamicAttackDelay);
      UOCharacter enemy = new UOCharacter(Serial.Invalid);


      if (targetAlternates.Length == 0)
        enemy = new UOCharacter(Aliases.LastAttack);
      else
      {
        enemy = new UOCharacter(Targeting.ParseTargets(targetAlternates));
      }

      if (enemy.Serial == Serial.Invalid || !enemy.Exist)
      {
        new Targeting().Find3();
        Game.Wait(250);
        enemy = new UOCharacter(Aliases.LastAttack);
      }

      if (enemy.Exist)
      {
        if (!lastEnemy.IsValid || lastEnemy != enemy.Serial)
          counter = 0;

        if (!World.Player.Warmode)
        {
          UO.Warmode(true);
          Game.Wait(250);
        }
        UO.Attack(enemy.Serial);


        if (counter % 4 == 0)
        {
          Game.PrintMessage(String.Format("[{0}/{1}]", enemy.Hits, enemy.MaxHits), Game.GetEnemyColorByHits(enemy.Serial));

          // enemy.PrintMessage(String.Format("[{0}/{1}]", enemy.Hits, enemy.MaxHits), Game.GetEnemyColorByHits(enemy.Serial));
        }

        counter++;

      }
    }

    //---------------------------------------------------------------------------------------------

    //private static Serial lastSelectedNPC = Serial.Invalid;
    //private static DateTime lastSelectedNPCTime = DateTime.Now;

    //public static bool IsLastSelectedNPC(Serial s)
    //{
    //  if (lastSelectedNPC.IsValidCust() && s == lastSelectedNPC && (DateTime.Now - lastSelectedNPCTime).TotalMilliseconds < 50)
    //    return true;

    //  return false;
    //}
  
    public static void SelectNPCInClient(Serial serial)
    {
      //lastSelectedNPC = serial;
      //lastSelectedNPCTime = DateTime.Now;

      // Oznac npc v klientovy
      byte[] data = new byte[5];
      data[0] = 0xAA;
      ByteConverter.BigEndian.ToBytes((uint)serial, data, 1);
      Core.SendToClient(data, false);
    }

    //---------------------------------------------------------------------------------------------

    public static TargetAliasResult ParseTargets(params string[] targets)
    {
      List<string> finalTargets = new List<string>();

      if (targets != null)
      {
        foreach (string target in targets)
        {
          if (!String.IsNullOrEmpty(target))
            finalTargets.AddRange(target.Split(new char[] { ',', '|', ';' }, StringSplitOptions.RemoveEmptyEntries));
        }

        foreach (string target in finalTargets)
        {
          TargetAliasResult serial = Targeting.ParseTarget(target);
          UOObject o = new UOObject(serial);

          if (serial.IsStatic || (serial.IsValid && o.Exist && o.Distance < 28))
          {
            System.Diagnostics.Debug.WriteLine("Target: " + target + " - success [" + serial + ", " + o.Distance + ", " + o.Name + "]", "Caleb");
            return serial;
          }
          else
            System.Diagnostics.Debug.WriteLine("Target: " + target + " - fail [" + serial + ", " + o.Distance + ", " + o.Name + "]", "Caleb");
        }
      }
      return Serial.Invalid;
    }

    //---------------------------------------------------------------------------------------------

    private static TargetAliasResult ParseTarget(string targetName)
    {
      return ParseTarget(targetName, Serial.Invalid);
    }

    //---------------------------------------------------------------------------------------------
    
    private static TargetAliasResult ParseTarget(string targetName, Serial defaultValue)
    {
      if (targetName != null)
      {
        Serial target = Serial.Invalid;

        if (targetName.ToLower() == "self")
          return Aliases.Self;
        if (targetName.ToLower() == "lasttarget")
          return Aliases.LastTarget;
        if (targetName.ToLower() == "lastattack")
          return Aliases.LastAttack;
        if (targetName.ToLower() == "lastobject")
          return Aliases.LastObject;
        if (targetName.ToLower() == "laststatusmanual")
          return  Game.CurrentGame.IsAlie(target) ? (Serial)Serial.Invalid : Aliases.GetObject("LastStatusManual");
        if (targetName.ToLower() == "laststatus")
          return Aliases.GetObject("LastStatusManual");
        if (targetName.ToLower() == "hover")
          return Game.CurrentGame.CurrentHoverStatus;
        if (targetName.ToLower() == "selfmoving")
        {
          if (Game.PlayerMoving)
            return Aliases.Self;
          else
            return defaultValue;
        }
        if (targetName.ToLower() == "onestepwest")
        {
          TargetAliasResult r = new TargetAliasResult() { X = (ushort)(World.Player.X - 1), Y = World.Player.Y, Z = World.Player.Z };
          return r;
        }
        if (targetName.ToLower() == "onestepeast")
        {
          TargetAliasResult r = new TargetAliasResult() { X = (ushort)(World.Player.X + 1), Y = World.Player.Y, Z = World.Player.Z };
          return r;
        }

        if (targetName.StartsWith("tilerel"))
        {
          string relCoord = targetName.Replace("tilerel", "");
          string[] coords = relCoord.Split(new char[] { '.' });

          ushort x = World.Player.X;
          ushort y = World.Player.Y;
          sbyte z = World.Player.Z;


          if (coords.Length > 0)
            x = (ushort)(x + Utils.ToNullInt(coords[0]).GetValueOrDefault(x));
          if (coords.Length > 1)
            y = (ushort)(y + Utils.ToNullInt(coords[1]).GetValueOrDefault(y));
          if (coords.Length > 2)
            z = (sbyte)(z + Utils.ToNullInt(coords[2]).GetValueOrDefault(z));

          TargetAliasResult r = new TargetAliasResult() { X = x, Y = y, Z = z };
          return r;
        }


        UOCharacter t = new UOCharacter(target);

        if (target.IsValid && t.Exist && t.Distance < 35)
        {
          if (targetName.ToLower() == "hovera")
          {
            if (Game.CurrentGame.IsAlie(target))
              return Game.CurrentGame.CurrentHoverStatus;
          }

          if (targetName.ToLower() == "hovere")
          {
            if (t.Notoriety == Notoriety.Enemy || t.Notoriety == Notoriety.Murderer || t.Notoriety == Notoriety.Criminal)
              return Game.CurrentGame.CurrentHoverStatus;
          }
        }

        if (targetName.ToLower() == "nearestcorpse")
        {
          UOItem[] arr = World.Ground.Where(i => i.Graphic == 0x2006 && i.Distance <= 2).OrderBy(i => i.Distance).ToArray();
          if (arr.Length > 0)
            return arr[0].Serial;
        }

        if (targetName.ToLower() == "nearestenemy")
        {
          UOCharacter[] arr = World.Characters.Where(i => i.Notoriety == Notoriety.Enemy || i.Notoriety == Notoriety.Murderer || i.Serial == Aliases.LastAttack).OrderBy(i => i.Distance).ToArray();

          if (arr.Length > 0)
            return arr[0].Serial;
        }

        if (targetName.ToLower() == "onestepenemy")
        {
          UOCharacter[] arr = World.Characters.Where(i=> i.Serial != World.Player.Serial && !Game.CurrentGame.IsAlie(i.Serial) && !Game.CurrentGame.IsHealAlie(i.Serial) && !Game.IsMob(i.Serial) && !i.Renamable && i.Distance <= 1.5).OrderBy(i => i.Distance).ToArray();

          if (arr.Length > 0)
            return arr[0].Serial;
        }


        if (targetName.ToLower().StartsWith("enemydmax"))
        {
          string max = targetName.ToLower().Replace("enemydmax", "");
          int maxD = Utils.ToNullInt(max).GetValueOrDefault(2);

          UOCharacter[] arr = World.Characters.Where(i => i.Serial != World.Player.Serial && !Game.CurrentGame.IsAlie(i.Serial) && !Game.CurrentGame.IsHealAlie(i.Serial) && !Game.IsMob(i.Serial) && !i.Renamable && i.Distance <= maxD).OrderBy(i => i.Distance).ToArray();

          if (arr.Length > 0)
            return arr[0].Serial;
        }

        if (targetName.ToLower() == "nearestghost")
        {
          UOCharacter[] arr = Healing.GetGhosts().OrderBy(i => i.Distance).ToArray();
          if (arr.Length > 0)
            return arr[0].Serial;
        }

        if (targetName.ToLower() == "onestepghost")
        {
          UOCharacter[] arr = Healing.GetGhosts().Where(i=>i.Distance <= 1).OrderBy(i => i.Distance).ToArray();
          if (arr.Length > 0)
            return arr[0].Serial;
        }

        if (targetName.ToLower().StartsWith("ghostdmax"))
        {
          string max = targetName.ToLower().Replace("ghostdmax", "");
          int maxD = Utils.ToNullInt(max).GetValueOrDefault(3);

          UOCharacter[] arr = Healing.GetGhosts().Where(i => i.Distance <= maxD).OrderBy(i => i.Distance).ToArray();

          if (arr.Length > 0)
            return arr[0].Serial;
        }

        if (targetName.ToLower() == "nexthealalie")
        {
          string max = targetName.ToLower().Replace("nexthealalie", "");
          int maxD = Utils.ToNullInt(max + String.Empty).GetValueOrDefault(25);

          List<CharHealPriority> chhpList = Healing.GetCharHealPriorityList(maxD, false, Game.MergeLists<UOCharacter>(Game.CurrentGame.Alies, Game.CurrentGame.HealAlies));
          var sortedList = chhpList.Where(c => !alieHealSkip.Contains(c.Char.Serial)).ToList();

          if (sortedList.Count == 0)
          {
            alieHealSkip.Clear();
            sortedList = chhpList;
          }

          if (sortedList.Count > 0)
          {
            alieHealSkip.Add(sortedList[0].Char.Serial);
            return sortedList[0].Char.Serial;
          }
        }

        target = Aliases.GetObject(targetName);
        if (target.IsValid && new UOCharacter(target).ExistCust())
          return target;
      }

      return defaultValue;
    }
    private static List<Serial> alieHealSkip = new List<Serial>();

    //---------------------------------------------------------------------------------------------

    public static void ResetTarget()
    {
      //if (UIManager.CurrentState == UIManager.State.ServerTarget)
      //{
      //  UO.Press(System.Windows.Forms.Keys.Escape);
      //  Game.Wait(50);
      //}

      UIManager.Reset();
    }

    //---------------------------------------------------------------------------------------------

    #region exec



    //---------------------------------------------------------------------------------------------

    [Command]
    public static void AllEnemyNames()
    {
      foreach (UOCharacter mob in Characters.GetEnemiesByDistance(40, null))//World.Characters)
      {
        mob.Click();
        Game.Wait(50);
      }
    }


    //---------------------------------------------------------------------------------------------

    [Executable("AttackSelectTarget")]
    public static void ExecAttackSelectTarget(params string[] exceptFilters)
    {
      new Targeting().AttackSelectTarget(exceptFilters);
    }

    //---------------------------------------------------------------------------------------------

    [Command("caltargetnext")]
    public static void ExecFind()
    {
      Game.CurrentGame.CurrentPlayer.GetPlayerRoleInstance<Targeting>().Find3();
    }

    //---------------------------------------------------------------------------------------------

    [Command("caltargetnext2")]
    public static void ExecFind2()
    {
      Game.CurrentGame.CurrentPlayer.GetPlayerRoleInstance<Targeting>().Find3();
    }

    //---------------------------------------------------------------------------------------------

    [Command("caltargetnext3")]
    public static void ExecFind3()
    {
      Game.CurrentGame.CurrentPlayer.GetPlayerRoleInstance<Targeting>().Find3();
    }

    //---------------------------------------------------------------------------------------------

    [Command("caltargetnext3")]
    public static void ExecFind3(int resetTimer, string notos)
    {
      Game.CurrentGame.CurrentPlayer.GetPlayerRoleInstance<Targeting>().SelectNextTarget(resetTimer, notos);
    }

    //---------------------------------------------------------------------------------------------

    #endregion


    //---------------------------------------------------------------------------------------------

    [Executable]
    public static TargetInfo GetTarget(string targetExpression)
    {
      TargetInfo info = new TargetInfo(targetExpression).GetTarget();

      if (Game.Debug)
      {
        PrintInfo(info);
      }

      return info;
    }

    //---------------------------------------------------------------------------------------------
    [Executable]
    public static void PrintInfo()
    {
      Targeting.ResetTarget();
      PrintInfo(new TargetInfo(null).GetTarget());
    }

    public static void PrintInfo(TargetInfo info)
    {
      //Notepad.WriteLine(format, target.X, target.Y, target.Z, target.Graphic, name);
      //Notepad.WriteLine();
      Notepad.WriteLine("TargetInfo.Success: " + info.Success);
      Notepad.WriteLine("TargetInfo.TargetType: " + info.TargetType);
      Notepad.WriteLine("TargetInfo.X: " + info.StaticTarget.X);
      Notepad.WriteLine("TargetInfo.Y: " + info.StaticTarget.Y);
      Notepad.WriteLine("TargetInfo.Z: " + info.StaticTarget.Z);
      Notepad.WriteLine("TargetInfo.Position: " + info.Position);
      Notepad.WriteLine("TargetInfo.Distance: " + info.Object.Distance);
      Notepad.WriteLine("TargetInfo.Position.Distance: " + info.Position.Distance());
      Notepad.WriteLine("TargetInfo.Position.RealDistance: " + String.Format("{0:n2}", info.Position.RealDistance()));
      Notepad.WriteLine("TargetInfo.Graphic: " + info.StaticTarget.Graphic);
      Notepad.WriteLine("TargetInfo.Serial: " + info.StaticTarget.Serial);
      Notepad.WriteLine("TargetInfo.Serial.IsValidCust: " + ((Serial)info.StaticTarget.Serial).IsValidCust());
      Notepad.WriteLine();
    }


    //---------------------------------------------------------------------------------------------
  }

  //---------------------------------------------------------------------------------------------

  public class TargetInfo
  {
    public TargetInfo()
    {

    }

    public TargetInfo(string target)
    {
      targetExpression = target + String.Empty;
    }

    private string targetExpression = String.Empty;
    private Serial serial = Serial.Invalid;
    private ushort x = 0;
    private ushort y = 0;
    private sbyte z = 0;
    private Graphic graphic = 0;
    public bool Success = false;
    TargetType targetType;

    public UOCharacter Character { get { return (UOCharacter)this; } }
    public UOItem Item { get { return (UOItem)this; } }
    public UOObject Object { get { return (UOObject)this; } }
    public TargetType TargetType { get { return this.targetType; } }
    public StaticTarget StaticTarget { get { return (StaticTarget)this; } }
    public IUOPosition Position {  get { return new UOPositionBase(x,y,(ushort)z); } }

    private void Reset()
    {
      this.x = 0;
      this.y = 0;
      this.z = 0;
      this.graphic = 0;
      this.Success = false;
    }

    public static implicit operator Serial(TargetInfo info)
    {
      return info.serial;
    }

    public static implicit operator TargetInfo(uint serial)
    {
      TargetInfo info = new TargetInfo();
      return info.GetTarget(serial);
    }

    public static implicit operator TargetInfo(Serial serial)
    {
      TargetInfo info = new TargetInfo();
      return info.GetTarget(serial);
    }

    public static implicit operator TargetInfo(UOCharacter ch)
    {
      TargetInfo info = new TargetInfo();
      return info.GetTarget(ch.Serial);
    }

    public static implicit operator TargetInfo(UOItem item)
    {
      TargetInfo info = new TargetInfo();
      return info.GetTarget(item.Serial);
    }

    public static implicit operator TargetInfo(UOObject o)
    {
      TargetInfo info = new TargetInfo();
      return info.GetTarget(o.Serial);
    }

    public static implicit operator uint(TargetInfo info)
    {
      return info.serial;
    }

    public static implicit operator StaticTarget(TargetInfo info)
    {
      return new StaticTarget(info.serial, info.x, info.y, info.z, info.graphic);
    }

    public static implicit operator UOObject(TargetInfo info)
    {
      return new UOObject(info.serial);
    }

    public static implicit operator UOCharacter(TargetInfo info)
    {
      return new UOCharacter(info.serial);
    }

    public static implicit operator UOItem(TargetInfo info)
    {
      return new UOItem(info.serial);
    }

    private TargetInfo GetTarget(Serial s)
    {
      this.serial = s;

      if (this.Character.Exist)
      {
        this.Success = true;
        this.x = this.Character.X;
        this.y = this.Character.Y;
        this.z = this.Character.Z;
        this.graphic = this.Character.Model;
        this.targetType = TargetType.Object;
      }
      else if (this.Item.Exist)
      {
        this.Success = true;
        this.x = this.Item.X;
        this.y = this.Item.Y;
        this.z = this.Item.Z;
        this.graphic = this.Item.Graphic;
        this.targetType = TargetType.Object;
      }

      return this;
    }

    public TargetInfo GetTarget()
    {
      Targeting.ResetTarget();
      this.Reset();

      if (this.targetExpression.ToLower() != "none")
      {
        if (!String.IsNullOrEmpty(this.targetExpression))
        {
          GetTarget(Targeting.ParseTargets(this.targetExpression));
        }
        else
        {
          GetTarget(this.serial);
        }

        if (!this.serial.IsValidCust() || !((UOObject)this).Exist)
        {
          StaticTarget t = UIManager.Target();
          if (((Serial)t.Serial).IsValidCust() || t.X > 0 && t.X < 65535 || t.Y > 0 && t.Y < 65535)
          {
            this.Success = true;
            this.x = t.X;
            this.y = t.Y;
            this.z = t.Z;
            this.serial = t.Serial;
            this.graphic = t.Graphic;
            this.targetType = t.Type;
          }
        }
      }
      return this;
    }

    public IRequestResult WaitTarget()
    {
      if (Success)
      {
        if (Object.Exist)
          return UO.WaitTargetObject(Object);
        else
          return UO.WaitTargetTile(StaticTarget.X, StaticTarget.Y, StaticTarget.Z, StaticTarget.Graphic);
      }

      return UIManager.FailedResult; 

    }
    // public 
  }

  //---------------------------------------------------------------------------------------------

  public class TargetAliasResult
  {
    public Serial Serial = Serial.Invalid;
    public ushort? X;
    public ushort? Y;
    public sbyte? Z;

    public bool IsValid
    {
      get
      {
        return Serial.IsValid;
      }
    }

    public bool IsStatic
    {
      get
      {
        return X.HasValue && Y.HasValue && X.Value >= 0 && Y.Value >= 0;
      }
    }

    public void WaitTarget()
    {
      if (this.IsValid)
        UO.WaitTargetObject(this.Serial);
      else if (this.IsStatic)
        UO.WaitTargetTile(this.X.Value, this.Y.Value, this.Z.GetValueOrDefault(0), Graphic.Invariant); 

    }

    public static implicit operator Serial(TargetAliasResult tar)
    {
      return tar.Serial;
    }

    public static implicit operator TargetAliasResult(Serial s)
    {
      return new TargetAliasResult() { Serial = s };
    }

    public static implicit operator StaticTarget(TargetAliasResult tar)
    {
      UOObject o = new UOObject(tar.Serial);
      UOItem i = new UOItem(tar.Serial);
      UOCharacter ch = new UOCharacter(tar.Serial);

      return new StaticTarget(tar.Serial, tar.X.GetValueOrDefault(o.X), tar.Y.GetValueOrDefault(o.Y), tar.Z.GetValueOrDefault(o.Z), i.Exist ? i.Graphic : ch.Model);
    }

    public static implicit operator TargetAliasResult(StaticTarget st)
    {
      return new TargetAliasResult() { Serial = st.Serial, X = st.X, Y = st.Y, Z = st.Z };
    }

    public static implicit operator uint(TargetAliasResult tar)
    {
      return tar.Serial;
    }

    public static implicit operator TargetAliasResult(uint s)
    {
      return new TargetAliasResult() { Serial = s };
    }


    public static implicit operator TargetAliasResult(UOCharacter ch)
    {
      return new TargetAliasResult() { Serial = ch.Serial };
    }

    public static implicit operator TargetAliasResult(TargetInfo t)
    {
      return new TargetAliasResult() { Serial = t};
    }
  }


  //---------------------------------------------------------------------------------------------

  [RuntimeObject]
  public class ManualLastTarget
  {
    //---------------------------------------------------------------------------------------------

    public static bool EnableShowStatusBar = true;

    [Executable]
    [BlockMultipleExecutions]
    public static void SwitchShowStatusBar()
    {
      ManualLastTarget.EnableShowStatusBar = !ManualLastTarget.EnableShowStatusBar;
      string message = "OFF";

      if (EnableShowStatusBar)
        message = "ON";

      Game.PrintMessage("EnableShowStatusBar " + message, Game.Val_LightGreen);
    }

    [ClientMessageHandler(0x05)]
    public CallbackResult OnAttack(byte[] data, CallbackResult prevState)
    {
      Game.RunScriptCheck(Targeting.DynamicAttackDelay);

      return CallbackResult.Normal;
    }


    [ClientMessageHandler(0x34)]
    public CallbackResult OnOpenStatus(byte[] data, CallbackResult prevState)
    {
      if (prevState == CallbackResult.Normal)
      {
        if (data[5] == 4)
        {
          Serial s = ByteConverter.BigEndian.ToUInt32(data, 6);

          //if (!Targeting.IsLastSelectedNPC(s))
          //{
            UOCharacter ch = new UOCharacter(s);
            if (/*ch.Notoriety == Notoriety.Guild || ch.Notoriety == Notoriety.Innocent || */Game.CurrentGame.IsAlie(s))
            {
              Aliases.SetObject("laststatus", s);
              Aliases.SetObject("LastAlieManual", s);
              Aliases.SetObject("LastStatusManual", s);
              Aliases.SetObject("LastStatusManualMobs", s);
            }

            else
            {
              Aliases.SetObject("laststatus", s);
              Aliases.SetObject("LastStatusManual", s);
              Aliases.SetObject("LastStatusManualMobs", s);
            }

            if ((Game.IsMob(s) && Game.IsMobActive(s)) || ch.Renamable)
            {
              Aliases.SetObject("SelectedMob", s);
            }

          if (Game.IsPossibleMob(ch) || ch.Renamable)
            Rename.RenameCharacter(s);


          //     if (EnableShowStatusBar && !Rename.RenameCharacter(s))
          if (ch.Serial == World.Player.Serial || EnableShowStatusBar)
            new StatusBar().Show(s);

          }
          return CallbackResult.Normal;
      //  }
      }
      return CallbackResult.Normal;
    }
  }
}
