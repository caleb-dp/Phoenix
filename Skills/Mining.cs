//using System;
//using System.Linq;
//using System.Collections.Generic;
//using System.Text;
//using Phoenix.WorldData;
//using Phoenix.Runtime;
//using Phoenix;
//using Caleb.Library;
//using System.Threading;
//using CalExtension.Skills;
//using CalExtension.UOExtensions;
//using Caleb.Library.CAL.Business;

//namespace CalExtension.Skills
//{
//  public class Mining : Skill
//  {
//    //---------------------------------------------------------------------------------------------

//    public enum OreGraphic
//    {
//      One = 0x19B7,
//      Two = 0x19BA,
//      Tree = 0x19B8,
//      Many = 0x19B9
//    }

//    //---------------------------------------------------------------------------------------------

//    public enum OreColor
//    {
//      Iron = 0x0000,
//      Copper = 0x0289,
//      Bronze = 0x01BF,
//      Silver = 0x0482,
//      Shadow = 0x0322,
//      Rose = 0x0665,
//      Golden = 0x0160,
//      Verite = 0x01CB,
//      Valorite = 0x0253,
//      BloodRock = 0x04C2,
//      BlackRock = 0x0455,
//      Mytheril = 0x052D,
//      StarSapphire = 0x0006,
//      Emerald = 0x0041,
//      Citrine = 0x002C,
//      Amethyst = 0x0015,
//      Ruby = 0x0027,
//      Diamond = 0x03E9
//    }

//    //---------------------------------------------------------------------------------------------

//    public enum IgnotColor//TODO zatim je spravne jen bronze
//    {
//      Iron = 0x0000,
//      Copper = 0x0289,
//      Bronze = 0x06D6,
//      Silver = 0x0482,
//      Shadow = 0x0322,
//      Rose = 0x0665,
//      Golden = 0x0160,
//      Verite = 0x01CB,
//      Valorite = 0x0253,
//      BloodRock = 0x04C2,
//      BlackRock = 0x0455,
//      Mytheril = 0x052D,
//      StarSapphire = 0x0006,
//      Emerald = 0x0041,
//      Citrine = 0x002C,
//      Amethyst = 0x0015,
//      Ruby = 0x0027,
//      Diamond = 0x03E9
//    }
//    //    Serial: 0x40162333  Name: "gold ingot"  Position: 117.74.0  Flags: 0x0000  Color: 0x0000  Graphic: 0x1BE9  Amount: 63  Layer: None  Container: 0x40339E2D

//    //Serial: 0x401619B4  Position: 110.111.0  Flags: 0x0000  Color: 0x0000  Graphic: 0x1BF5  Amount: 3  Layer: None  Container: 0x40339E2D

//    //---------------------------------------------------------------------------------------------

//    public enum IngotGraphic
//    {
//      type1 = 0x1BEF,//0x1BEF  
//      type2 = 0x1BE3,
//      type3 = 0x1BF5,
//      type4 = 0x1BE9
//    }

//    //    public static Graphic IngotGraphic = 0x1BEF;
//    public static Graphic ForgeGraphic = 0x0FB1;
//    public static Graphic KrumpacGraphic = 0x0E85;

//    //---------------------------------------------------------------------------------------------

//    public static List<string> AllOreNames { get { return new List<string>() { "Iron", "Copper", "Bronze", "Silver", "Shadow", "Rose", "Golden", "Verite", "Valorite", "BloodRock", "BlackRock", "Mytheril", "StarSapphire", "Emerald", "Citrine", "Amethyst", "Ruby", "Diamond" }; } }

//    //---------------------------------------------------------------------------------------------

//    protected UOPositionCollection positionHistory;
//    protected UOPositionCollection PositionHistory
//    {
//      get
//      {
//        if (this.positionHistory == null)
//          this.positionHistory = new UOPositionCollection();
//        return this.positionHistory;
//      }
//    }

//    //---------------------------------------------------------------------------------------------
//    protected UOPosition forgePosition;
//    protected UOPosition ForgePosition
//    {
//      get { return this.forgePosition; }
//      set { this.forgePosition = value; }
//    }

//    protected UOPosition startPosition;
//    protected UOPosition StartPosition
//    {
//      get { return this.startPosition; }
//      set { this.startPosition = value; }
//    }

//    protected UOPosition containerPosition;
//    protected UOPosition ContainerPosition
//    {
//      get { return this.containerPosition; }
//      set { this.containerPosition = value; }
//    }

//    protected UOPosition entrancePosition;
//    protected UOPosition EntrancePosition
//    {
//      get { return this.entrancePosition; }
//      set { this.entrancePosition = value; }
//    }

//    public string FullContainerType = "Chest";//Chest, Bank

//    protected UOPosition lastposition;
//    public UOPosition LastPosition
//    {
//      get { return this.lastposition; }
//      set { this.lastposition = value; }
//    }

//    protected string trackName = "";
//    public string TrackName
//    {
//      get { return this.trackName; }
//      set { this.trackName = value; }
//    }

//    protected Graphic bankContainerType;
//    public Graphic BankContainerType
//    {
//      get { return this.bankContainerType; }
//      set { this.bankContainerType = value; }
//    }

//    protected UOItem underFeatContainer;
//    public UOItem UnderFeatContainer
//    {
//      get { return this.underFeatContainer; }
//      set { this.underFeatContainer = value; }
//    }

//    protected List<string> denyOres;
//    public List<string> DenyOres
//    {
//      get
//      {
//        if (this.denyOres == null)
//        {
//          this.denyOres = new List<string>();
//        }
//        return this.denyOres;
//      }
//      set { this.denyOres = value; }
//    }

//    protected List<Graphic> denyOreGraphic;
//    public List<Graphic> DenyOreGraphic
//    {
//      get
//      {
//        if (this.denyOreGraphic == null)
//        {
//          this.denyOreGraphic = new List<Graphic>();
//        }
//        return this.denyOreGraphic;
//      }
//      set { this.denyOreGraphic = value; }
//    }

//    protected ushort ignotAmount = 100;
//    public ushort IgnotAmount
//    {
//      get { return this.ignotAmount; }
//      set { this.ignotAmount = value; }
//    }


//    protected int weightOffset = 70;
//    public int WeightOffset
//    {
//      get { return Math.Abs(this.weightOffset); }
//      set { this.weightOffset = value; }
//    }

//    public bool WeightLimitReached
//    {
//      get { return World.Player.Weight > (World.Player.MaxWeight - this.WeightOffset); }
//    }

//    internal int? weightLimitReachedSmeltedValue = null;
//    public bool WeightLimitReachedSmelted
//    {
//      get
//      {
//        return World.Player.Weight > (World.Player.MaxWeight - 150/*(weightLimitReachedSmeltedValue.GetValueOrDefault(-150))*/);
//      }
//    }

//    protected bool smelt = true;
//    public bool Smelt
//    {
//      get { return this.smelt; }
//      set { this.smelt = value; }
//    }

//    protected Robot robot;
//    public Robot Robot
//    {
//      get
//      {
//        //return Game.CurrentGame.CurrentPlayer.Robot;
//        if (this.robot == null)
//        {
//          this.robot = new Robot();
//          this.robot.EnableLog = true;
//        }
//        return this.robot;
//      }
//    }

//    public UOItem Krumpac
//    {
//      get { return GetKrumpac(); }
//    }

//    protected UOItem forge;
//    public UOItem Forge
//    {
//      get
//      {
//        UOItem findForge = World.Ground.FindType(ForgeGraphic);
//        return findForge;
//      }
//    }

//    protected MiningBagFull mimingFullMethod;
//    protected MiningBagFull MimingFullMethod
//    {
//      get { return this.mimingFullMethod; }
//      set { this.mimingFullMethod = value; }
//    }

//    public static UOItem GetKrumpac()
//    {
//      UOItem krumpac = new UOItem(Serial.Invalid);
//      if (!(krumpac = World.Player.Layers.FindType(KrumpacGraphic)).Exist)
//      {
//        World.Player.Backpack.AllItems.FindType(KrumpacGraphic);
//      }
//      if (!krumpac.Exist) UO.Print("Nemas krupac!");
//      return krumpac;
//    }

//    public bool UseHiding = false;

//    //---------------------------------------------------------------------------------------------

//    protected bool GoToContainer()
//    {
//      if (this.MimingFullMethod != null)
//      {
//        if (!this.MimingFullMethod())
//        {
//          UO.Print("!MimingFullMethod.");
//          return false;
//        }
//      }
//      else if (this.ContainerPosition != null)
//      {
//        UO.Print("GOTO Container: " + this.ContainerPosition);
//        if (this.Robot.GoTo(this.ContainerPosition, 1))
//        {
//          if (UseHiding && !World.Player.Hidden)
//            UO.UseSkill(StandardSkill.Hiding);

//          Serial container = Serial.Invalid;

//          UO.Wait(2500);

//          if (this.FullContainerType.ToLower() == "bank")
//          {
//            UO.Say("Bank");
//            UO.Wait(500);
//            container = World.Player.Layers[Layer.Bank];

//            if (!this.BankContainerType.IsInvariant)
//            {
//              container = World.Player.Layers[Layer.Bank].Items.FindType(this.BankContainerType);
//              if (container.IsValid)
//              {
//                UOItem chest = new UOItem(container);
//                chest.Use();
//                UO.Wait(500);
//              }
//              else
//                container = World.Player.Layers[Layer.Bank];
//            }
//          }
//          else
//          {
//            container = World.Ground.FindType(this.BankContainerType);
//            if (container.IsValid)
//            {
//              UOItem chest = new UOItem(container);
//              chest.Use();
//              UO.Wait(500);

//              if (!chest.Exist)
//              {
//                UO.Print("Neni bedna na odhazovani");
//                return false;
//              }
//            }
//          }

//          UOItem ingot = new UOItem(Serial.Invalid);
//          while (container.IsValid && (ingot = FindIngot(UO.Backpack.Items)).Exist)
//          {
//            ingot.Move(150, container);
//            UO.Wait(500);
//          }
//          UO.Wait(2500);

//          ingot = new UOItem(Serial.Invalid);
//          if ((ingot = FindIngot(World.Ground)).Exist && ingot.Distance < 2)
//          {
//            UO.Print("Dropnul sem ingoty");
//            return false;
//          }

//          UO.Wait(Robot.MoveWait);

//          UO.Print("GOTO LastPosition: " + this.LastPosition);
//          if (!this.Robot.GoTo(this.LastPosition))
//          {
//            UO.Print("!this.Robot.GoTo(this.LastPosition) ");
//            return false;
//          }

//          if (UseHiding && !World.Player.Hidden)
//            UO.UseSkill(StandardSkill.Hiding);

//          UO.Wait(Robot.MoveWait);
//        }
//        else
//          return false;
//      }
//      else
//      {
//        UO.Print("Dosazen limi vahy.");
//        return false;
//      }
//      return true;
//    }

//    //---------------------------------------------------------------------------------------------

//    public void MiningRecursive()
//    {
//      Game.CurrentGame.Mode = GameMode.Working;
//      // WatchDogHelper.Watch();
//      this.PositionHistory.Add(new UOPosition(World.Player.X, World.Player.Y, 0));

//      if (UO.Dead)
//        return;

//      if (UseHiding && !World.Player.Hidden)
//        UO.UseSkill(StandardSkill.Hiding);

//      while (SingleKop())
//      {
//        if (World.Player.Y >= this.EntrancePosition.Y.GetValueOrDefault(10000))
//        {
//          Game.CurrentGame.Messages.Add("Jsem u vychodu jdu na start:" + this.StartPosition);

//          if (!this.Robot.GoTo(this.StartPosition, 0, 100))
//          {
//            Game.CurrentGame.Messages.Add("Nepodarilo se vratit na start");
//            return;
//          }
//          else
//          {
//            Game.CurrentGame.Messages.Add("Jedu dokola");
//            Game.Wait();
//          }
//        }

//        UO.Print("{0}/{1}", World.Player.Weight, World.Player.MaxWeight);
//        this.LastPosition = new UOPosition(World.Player.X, World.Player.Y, 0);

//        if (this.WeightLimitReached)
//        {
//          UO.Print("Jdeme smeltit!");
//          this.DropDenyOre();

//          UOItem ore = new UOItem(Serial.Invalid);
//          if (forgePosition != null)
//          {
//            UO.Print("Forge na pozici :" + forgePosition);
//            this.Robot.GoTo(forgePosition, 1);
//            if (UseHiding && !World.Player.Hidden)
//              UO.UseSkill(StandardSkill.Hiding);

//            if (this.Forge.Exist && this.Forge.Distance < 2)
//            {
//              this.ForgeAllOre();
//            }
//            else
//            {
//              UO.Print("Nejsem u forge!");
//              return;
//            }

//            if (this.WeightLimitReachedSmelted)
//            {
//              if (!this.GoToContainer())
//              {
//                UO.Print("!this.GoToContainer()");
//                return;


//              }

//            }
//            else
//            {
//              this.Robot.GoTo(this.LastPosition);
//              if (UseHiding && !World.Player.Hidden)
//                UO.UseSkill(StandardSkill.Hiding);
//            }
//          }
//          else
//          {
//            UO.Print("Neni forge Dropuju");
//            this.DropAllOre();
//          }
//        }
//      }

//      if (Game.CurrentGame.WorldSave())
//      {
//        Game.Wait(30000);
//        this.Krumpac.Click();
//      }

//      if (!Krumpac.Exist)
//        return;

//      if (!this.Robot.Move(MovementDirection.UpLeft))
//      {
//        if (this.Robot.Move(MovementDirection.DownLeft))
//        {
//          this.MoveIgnotUnderFeat();
//          int steps = 0;
//          while (this.Robot.Move(MovementDirection.DownRight))
//          {
//            steps++;
//            if (steps > 1)
//            {
//              MoveIgnotUnderFeat();
//              steps = 0;
//            }
//          }
//          this.MiningRecursive();
//        }
//        else
//        {
//          bool findPath = false;
//          while (this.Robot.Move(MovementDirection.DownRight))
//          {
//            this.MoveIgnotUnderFeat();
//            if (this.Robot.Move(MovementDirection.DownLeft))
//            {
//              findPath = true;
//              break;
//            }
//          }
//          if (findPath)
//          {
//            int steps = 0;
//            while (this.Robot.Move(MovementDirection.DownRight))
//            {
//              steps++;
//              if (steps > 1)
//              {
//                this.MoveIgnotUnderFeat();
//                steps = 0;
//              }
//            }
//            this.MiningRecursive();
//          }
//          else
//          {
//            //UOPosition nextTry = null;
//            //Game.CurrentGame.Messages.Add("Next Try");

//            //UO.Print("Try nextTry! " + this.PositionHistory.Count);
//            //foreach (UOPosition possible in this.Robot.SquarePositions)
//            //{
//            //  if (this.PositionHistory.GetItemByPosition(possible) == null && this.Robot.ActualPosition.X != possible.X && this.Robot.ActualPosition.Y != possible.Y)
//            //  {
//            //    double distance = this.Robot.ActualPosition.GetRelativeVectorLength(possible);
//            //    if (nextTry == null || this.Robot.ActualPosition.GetRelativeVectorLength(nextTry) > distance)
//            //      nextTry = possible;
//            //  }
//            //}

//            //if (nextTry != null)
//            //{
//            //  this.PositionHistory.Add(nextTry);
//            //  UO.Print("nextTry! " + nextTry);
//            //  if (!this.Robot.GoTo(nextTry))
//            //  {
//            //    UO.Print("Nelze najit dalsu cestu nextTry! ");
//            //    return;
//            //  }
//            //  else
//            //    this.MiningRecursive();
//            //}
//            //else
//            //{
//            //  UO.Print("Nelze najit dalsu cestu!");
//            //  return;
//            //}
//          }
//        }
//      }
//      else
//      {
//        this.MoveIgnotUnderFeat();
//        this.MiningRecursive();
//      }

//      return;
//    }

//    //---------------------------------------------------------------------------------------------

//    public void SetForgePosition()
//    {
//      UOItem targetItem = new UOItem(UIManager.TargetObject());

//      if (targetItem.Graphic != 0x0FB1)
//      {
//        UO.Print("Toto neni forge!");
//      }
//      else
//      {
//        UO.Print("Toto je forge na pozici {0},{1}.", targetItem.X, targetItem.Y);
//        this.forgePosition = new UOPosition(targetItem.X, targetItem.Y, 0);
//      }
//    }

//    //---------------------------------------------------------------------------------------------

//    public void MoveIgnotUnderFeat()
//    {
//      if (this.UnderFeatContainer != null && this.UnderFeatContainer.Exist)
//      {
//        UOItem ingot = new UOItem(Serial.Invalid);

//        List<UOItem> ignots = new List<UOItem>();
//        foreach (UOItem item in World.Player.Backpack.Items)
//        {
//          if (IsIngot(item))
//            ignots.Add(item);
//        }

//        foreach (UOItem item in ignots)
//        {
//          if (item.Amount > 25)
//          {
//            item.Move(150, this.UnderFeatContainer);
//            UO.Wait(300);
//          }
//        }

//        UO.Wait(300);
//        this.UnderFeatContainer.Move(1, World.Player.X, World.Player.Y, World.Player.Z);
//      }
//      else
//      {

//        foreach (UOItem item in World.Ground)
//        {
//          if (IsIngot(item) && item.Distance > 0)
//          {
//            item.Move(this.IgnotAmount, World.Player.X, World.Player.Y, World.Player.Z);
//            UO.Wait(250 + Core.Latency);
//          }
//        }
//      }
//    }

//    //---------------------------------------------------------------------------------------------

//    public void DropAllOre()
//    {
//      UOItem ore = new UOItem(Serial.Invalid);
//      while
//      (
//        (ore = UO.Backpack.AllItems.FindType((int)OreGraphic.One)).Exist || (ore = UO.Backpack.AllItems.FindType((int)OreGraphic.Two)).Exist ||
//        (ore = UO.Backpack.AllItems.FindType((int)OreGraphic.Tree)).Exist || (ore = UO.Backpack.AllItems.FindType((int)OreGraphic.Many)).Exist
//      )
//      {
//        UO.Wait(500);
//        ore.DropHere();
//      }
//    }

//    //---------------------------------------------------------------------------------------------

//    public void ForgeAllOre()
//    {
//      if (!Forge.Exist)
//      {
//        UO.Print("ForgeAllOre - Neni forge!");
//      }
//      UOItem ore = new UOItem(Serial.Invalid);
//      while
//      (
//        (ore = UO.Backpack.AllItems.FindType((int)OreGraphic.One)).Exist || (ore = UO.Backpack.AllItems.FindType((int)OreGraphic.Two)).Exist ||
//        (ore = UO.Backpack.AllItems.FindType((int)OreGraphic.Tree)).Exist || (ore = UO.Backpack.AllItems.FindType((int)OreGraphic.Many)).Exist
//      )
//      {
//        JournalEventWaiter jew = new JournalEventWaiter(true, "You lack the skill to smelt this ore");
//        this.Forge.Use();
//        UO.WaitTargetObject(ore.Serial);
//        if (jew.Wait(500 + Core.Latency))
//          ore.DropHere();
//      }
//    }

//    //---------------------------------------------------------------------------------------------

//    public bool SingleKop()
//    {
//      if (!Krumpac.Exist)
//        return false;

//      if (Game.CurrentGame.WorldSave())
//      {
//        Game.Wait(30000);
//        this.Krumpac.Click();
//      }

//      JournalEventWaiter jew = new JournalEventWaiter(true, "...akce skoncila");
//      Krumpac.Use();
//      UO.WaitTargetTileRel(0, 0, 0, 0);
//      jew.Wait(5000 + Core.Latency);

//      SkillValue miningSkill = SkillsHelper.GetSkillValue("Mining");

//      if (Journal.Contains(true, "There is no") || Journal.Contains(true, "Try mining in rock") || (miningSkill.Value == 1000 && JournalContainsDenyOre()))
//      {
//        if (JournalContainsDenyOre())
//        {
//          this.DropDenyOre();
//        }

//        Journal.Clear();
//        return false;
//      }
//      else
//      {
//        if (JournalContainsDenyOre())
//        {
//          this.DropDenyOre();
//        }
//      }
//      Journal.Clear();
//      // || JournalContainsDenyOre())

//      return true;
//    }

//    //---------------------------------------------------------------------------------------------

//    protected void DropDenyOre()
//    {
//      foreach (object gra in Enum.GetValues(typeof(OreGraphic)))
//      {
//        foreach (string name in Enum.GetNames(typeof(OreColor)))
//        {
//          bool found = false;
//          foreach (string deny in this.DenyOres)
//          {
//            if (name.ToLower() == deny.ToLower())
//            {
//              found = true;
//              break;
//            }
//          }

//          if (found)
//          {
//            string correctedName = name.Substring(0, 1) + name.Substring(1, name.Length - 1);
//            int oreColor = (int)Enum.Parse(typeof(OreColor), correctedName, true);

//            if (UO.Backpack.Items.FindType((int)gra, oreColor).Exist)
//            {
//              Game.CurrentGame.Messages.Add("Deny found: " + correctedName);

//              UO.Backpack.Items.FindType((int)gra, oreColor).DropHere(10000);
//              Game.Wait(200);
//            }
//          }
//        }
//      }
//    }

//    //---------------------------------------------------------------------------------------------

//    protected bool JournalContainsDenyOre()
//    {
//      if (this.DenyOres.Count == 0) return false;
//      else
//      {
//        foreach (string deny in this.DenyOres)
//        {
//          if (Journal.Contains(true, "You put the " + deny.ToLower() + " ores in your pack"))
//            return true;
//        }
//        return false;
//      }
//    }

//    //---------------------------------------------------------------------------------------------

//    public static string GetNameFromOreList(string name)
//    {
//      foreach (string oreName in AllOreNames)
//      {
//        if (oreName.ToLower() == name.ToLower())
//          return oreName;
//      }
//      return null;
//    }

//    //---------------------------------------------------------------------------------------------

//    public delegate bool MiningBagFull();

//    //---------------------------------------------------------------------------------------------

//    //Serial: 0x40000CB9  Position: 1370.2743.0  Flags: 0x0000  Color: 0x084D  Graphic: 0x0E41  Amount: 0  Layer: None  Container: 0x00000000//Scurka u domecku
//    //exec gotostatic 1370 2743

//    protected bool DoDomeckuAZpet()
//    {
//      Robot r = new Robot();

//      if (r.GoTo(1370, 2743, 1, 100))//this.Robot.GoByTrack(track))
//      {
//        UO.Wait(2500);
//        UOItem guildChest = new UOItem(0x40000CB9);
//        guildChest.Use();
//        UO.Wait(500);

//        UOItem ingot = new UOItem(Serial.Invalid);
//        while ((ingot = FindIngot(UO.Backpack.Items)).Exist)
//        {
//          ingot.Move(150, guildChest);
//          UO.Wait(500);
//        }
//        UO.Wait(2500);


//        ingot = new UOItem(Serial.Invalid);
//        if ((ingot = FindIngot(World.Ground)).Exist && ingot.Distance < 2)
//        {
//          UO.Print("Dropnul sem ingoty");
//          return false;
//        }

//        //this.Robot.GoByTrack(track, -1);
//        UO.Wait(Robot.MoveWait);
//        r.GoTo(this.LastPosition);
//        UO.Wait(Robot.MoveWait);
//        return true;
//      }
//      return false;
//    }

//    //---------------------------------------------------------------------------------------------

//    //protected bool DoBankyAZpet()
//    //{
//    //  Track dobanky = null;
//    //  if (!String.IsNullOrEmpty(this.TrackName) && (dobanky = Robot.Readedtracks[this.TrackName]) != null)
//    //  {
//    //    if (this.Robot.GoByTrack(dobanky))
//    //    {
//    //      UO.Wait(2500);
//    //      UO.Say("Bank");
//    //      UO.Wait(2500);

//    //      UOItem bankContainer = World.Player.Layers[Layer.Bank];
//    //      if (this.BankContainerType != null && World.Player.Layers[Layer.Bank].AllItems.FindType(this.BankContainerType).Exist)
//    //        bankContainer = World.Player.Layers[Layer.Bank].AllItems.FindType(this.BankContainerType);
//    //      else if (World.Player.Layers[Layer.Bank].AllItems.FindType(0x0E76).Exist)
//    //        bankContainer = World.Player.Layers[Layer.Bank].AllItems.FindType(0x0E76);

//    //      UOItem ingot = new UOItem(Serial.Invalid);
//    //      while ((ingot = FindIngot(UO.Backpack.Items)).Exist)
//    //      {
//    //        ingot.Move(150, bankContainer);
//    //        UO.Wait(500);
//    //      }
//    //      UO.Wait(2500);


//    //      ingot = new UOItem(Serial.Invalid);
//    //      if ((ingot = FindIngot(World.Ground)).Exist && ingot.Distance < 2)
//    //      {
//    //        UO.Print("Dropnul sem ingoty");
//    //        return false;
//    //      }

//    //      this.Robot.GoByTrack(dobanky, -1);
//    //      UO.Wait(Robot.MoveWait);
//    //      this.Robot.GoTo(this.LastPosition);
//    //      UO.Wait(Robot.MoveWait);
//    //      return true;
//    //    }
//    //    return false;
//    //  }
//    //  return false;
//    //}

//    //---------------------------------------------------------------------------------------------

//    protected bool KVykupuAZpet()
//    {
//      UOPosition vykupVDolu = new UOPosition(4050, 442, 3);
//      UO.Wait(Robot.MoveWait);

//      if (new Robot().GoTo(vykupVDolu, 1))
//      {
//        int sychr = 0;

//        while (sychr < 10 && Mining.FindIngot(World.Player.Backpack.AllItems).Exist && Mining.FindIngot(World.Player.Backpack.AllItems).Amount > 100)
//        {
//          UOObject obj = new UOObject(0x4022DBB4);
//          Game.Wait();
//        }

//        UO.Wait(Robot.MoveWait);
//        new Robot().GoTo(this.LastPosition);
//        UO.Wait(Robot.MoveWait);
//      }

//      return false;
//    }

//    [Executable]
//    public static void StartMiningXorine(string fullContainerType, bool hide, ushort startX, ushort startY, ushort forgeX, ushort forgetY, ushort containerX, ushort containerY, ushort entranceX, ushort entranceY, ushort barnkcontainertype, params string[] oreKinds)
//    {
//      Mining mining = new Mining();
//      //mining.BankContainerType = 0x0E7E;
//      mining.FullContainerType = fullContainerType;
//      mining.StartPosition = new UOPosition((startX == 0 ? World.Player.X : startX), (startY == 0 ? World.Player.Y : startY), 0);
//      mining.ForgePosition = new UOPosition(forgeX, forgetY, 0);
//      mining.ContainerPosition = new UOPosition(containerX, containerY, 0);
//      mining.EntrancePosition = new UOPosition(entranceX, entranceY, 0);
//      mining.UseHiding = hide;
//      mining.BankContainerType = new Graphic(barnkcontainertype);
//      mining.MimingFullMethod = new MiningBagFull(mining.GoToContainerPresKop);

//      UO.Print("StartMining s nasledujicimi parametry:");
//      UO.Print("FullContainerType:" + mining.FullContainerType);
//      UO.Print("StartPosition:" + mining.StartPosition);
//      UO.Print("ForgePosition:" + mining.ForgePosition);
//      UO.Print("ContainerPosition:" + mining.ContainerPosition);
//      UO.Print("EntrancePosition:" + mining.EntrancePosition);
//      UO.Print("UseHiding:" + mining.UseHiding);
//      UO.Print("BankContainerType:" + mining.BankContainerType);

//      if (oreKinds.Length > 0)
//      {
//        foreach (string ore in AllOreNames)
//        {
//          bool found = false;
//          foreach (string allowedOre in oreKinds)
//          {
//            if (allowedOre.ToLower() == ore.ToLower())
//            {
//              found = true;
//              break;
//            }
//          }

//          if (!found)
//          {
//            Game.CurrentGame.Messages.Add("Deny : " + ore);
//            mining.DenyOres.Add(ore);
//          }
//        }
//      }

//      Game.CurrentGame.Messages.Add("StartPosition: " + mining.StartPosition);
//      if (new Robot().GoTo(mining.StartPosition, 0))
//      {
//        mining.MiningRecursive();
//      }
//      Game.CurrentGame.Messages.Add("Kopani dodkonceno.");
//    }

//    protected bool GoToContainerPresKop()
//    {
//      Robot r = new Robot();

//      if (this.ContainerPosition != null)
//      {
//        UOPosition current = new UOPosition(r.ActualPosition.X.Value, r.ActualPosition.Y.Value, 0);

//        Phoenix.Runtime.RuntimeCore.Executions.Execute(RuntimeCore.ExecutableList["RuneBookUse"], 1);

//        while (current.X == r.ActualPosition.X && current.Y == r.ActualPosition.Y)
//        {
//          UO.Print("Cekam na kop.. nehybat");
//          UO.Wait(1000);
//        }

//        if (Game.CurrentGame.WorldSave())
//        {
//          UO.Print("WS opakovani kopu za 45s");
//          Game.Wait(15000);
//          Phoenix.Runtime.RuntimeCore.Executions.Execute(RuntimeCore.ExecutableList["RuneBookUse"], 1);

//          while (current.X == r.ActualPosition.X && current.Y == r.ActualPosition.Y)
//          {
//            UO.Print("Cekam na kop.. nehybat");
//            UO.Wait(1000);
//          }
//        }

//        UO.Print("GOTO Container: " + this.ContainerPosition);
//        if (r.GoTo(this.ContainerPosition, 1))
//        {
//          Serial container = Serial.Invalid;

//          UO.Wait(2500);

//          if (this.FullContainerType.ToLower() == "bank")
//          {
//            UO.Say("Bank");
//            UO.Wait(500);
//            container = World.Player.Layers[Layer.Bank];

//            if (!this.BankContainerType.IsInvariant)
//            {
//              container = World.Player.Layers[Layer.Bank].Items.FindType(this.BankContainerType);
//              if (container.IsValid)
//              {
//                UOItem chest = new UOItem(container);
//                chest.Use();
//                UO.Wait(500);
//              }
//              else
//                container = World.Player.Layers[Layer.Bank];
//            }
//          }
//          else
//          {
//            container = World.Ground.FindType(this.BankContainerType);
//            if (container.IsValid)
//            {
//              UOItem chest = new UOItem(container);
//              chest.Use();
//              UO.Wait(500);

//              if (!chest.Exist)
//              {
//                UO.Print("Neni bedna na odhazovani");
//                return false;
//              }
//            }
//          }

//          UOItem ingot = new UOItem(Serial.Invalid);
//          while (container.IsValid && (ingot = FindIngot(UO.Backpack.Items)).Exist)
//          {
//            ingot.Move(150, container);
//            UO.Wait(500);
//          }
//          UO.Wait(2500);

//          ingot = new UOItem(Serial.Invalid);
//          if ((ingot = FindIngot(World.Ground)).Exist && ingot.Distance < 2)
//          {
//            UO.Print("Dropnul sem ingoty");
//            return false;
//          }

//          UO.Wait(Robot.MoveWait);

//          current = new UOPosition(r.ActualPosition.X.Value, r.ActualPosition.Y.Value, 0);

//          //
//          //Sem dat script na kop zpet do dolu
//          new TravelBook().TravelBookUse(4);

//          //Sem dat script na kop zpet do dolu 
//          Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["TravelBookUse"], 1);
//          UO.Wait(1000);
//          Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["TravelBookUse"], 5);
//          while (current.X == r.ActualPosition.X && current.Y == r.ActualPosition.Y)
//          {
//            UO.Wait(1000);
//          }

//          if (Game.CurrentGame.WorldSave())
//          {
//            UO.Print("WS opakovani kopu za 45s");
//            Game.Wait(15000);

//            Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["TravelBookUse"], 1);
//            UO.Wait(1000);
//            Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["TravelBookUse"], 5);
//            while (current.X == r.ActualPosition.X && current.Y == r.ActualPosition.Y)
//            {
//              UO.Wait(1000);
//            }

//          }


//          while (current.X == r.ActualPosition.X && current.Y == r.ActualPosition.Y)
//          {
//            UO.Print("Cekam na kop.. nehybat");
//            UO.Wait(1000);
//          }

//          UO.Wait(500);

//          UO.Print("GOTO LastPosition: " + this.LastPosition);
//          if (!r.GoTo(this.LastPosition))
//            return false;
//          UO.Wait(Robot.MoveWait);
//        }
//        else
//          return false;
//      }
//      else
//      {
//        UO.Print("Dosazen limi vahy.");
//        return false;
//      }
//      return true;
//    }

//    //---------------------------------------------------------------------------------------------

//    public static UOItem FindIngot(ItemsCollection items)
//    {
//      foreach (UOItem item in items)
//      {
//        if (IsIngot(item))
//          return item;
//      }
//      return new UOItem(Serial.Invalid);
//    }

//    //---------------------------------------------------------------------------------------------

//    public static UOItem FindIngot(ItemsCollection items, UOColor color)
//    {
//      foreach (UOItem item in items)
//      {
//        if (IsIngot(item, color))
//          return item;
//      }
//      return new UOItem(Serial.Invalid);
//    }

//    //---------------------------------------------------------------------------------------------

//    public static bool IsIngot(UOItem ingot)
//    {
//      foreach (IngotGraphic ingotGraphic in Enum.GetValues(typeof(IngotGraphic)))
//      {
//        if (ingot.Graphic == (int)ingotGraphic)
//        {
//          return true;
//        }
//      }
//      return false;
//    }

//    //---------------------------------------------------------------------------------------------

//    public static bool IsIngot(UOItem ingot, UOColor color)
//    {
//      foreach (IngotGraphic ingotGraphic in Enum.GetValues(typeof(IngotGraphic)))
//      {
//        if (ingot.Graphic == (int)ingotGraphic && ingot.Color == color)
//        {
//          return true;
//        }
//      }
//      return false;
//    }

//    //---------------------------------------------------------------------------------------------

//    [Executable]
//    public static void StartMining(string fullContainerType, bool hide, ushort startX, ushort startY, ushort forgeX, ushort forgetY, ushort containerX, ushort containerY, ushort entranceX, ushort entranceY, params string[] oreKinds)
//    {
//      StartMining(fullContainerType, hide, startX, startY, forgeX, forgetY, containerX, containerY, entranceX, entranceY, Graphic.Invariant, oreKinds);
//    }

//    //---------------------------------------------------------------------------------------------

//    [Executable]
//    public static void StartMining(string fullContainerType, bool hide, ushort startX, ushort startY, ushort forgeX, ushort forgetY, ushort containerX, ushort containerY, ushort entranceX, ushort entranceY, ushort barnkcontainertype, params string[] oreKinds)
//    {
//      Mining mining = new Mining();
//      //mining.BankContainerType = 0x0E7E;
//      mining.FullContainerType = fullContainerType;
//      mining.StartPosition = new UOPosition((startX == 0 ? World.Player.X : startX), (startY == 0 ? World.Player.Y : startY), 0);
//      mining.ForgePosition = new UOPosition(forgeX, forgetY, 0);
//      mining.ContainerPosition = new UOPosition(containerX, containerY, 0);
//      mining.EntrancePosition = new UOPosition(entranceX, entranceY, 0);
//      mining.UseHiding = hide;
//      mining.BankContainerType = new Graphic(barnkcontainertype);

//      UO.Print("StartMining s nasledujicimi parametry:");
//      UO.Print("FullContainerType:" + mining.FullContainerType);
//      UO.Print("StartPosition:" + mining.StartPosition);
//      UO.Print("ForgePosition:" + mining.ForgePosition);
//      UO.Print("ContainerPosition:" + mining.ContainerPosition);
//      UO.Print("EntrancePosition:" + mining.EntrancePosition);
//      UO.Print("UseHiding:" + mining.UseHiding);
//      UO.Print("BankContainerType:" + mining.BankContainerType);

//      if (oreKinds.Length > 0)
//      {
//        foreach (string ore in AllOreNames)
//        {
//          bool found = false;
//          foreach (string allowedOre in oreKinds)
//          {
//            if (allowedOre.ToLower() == ore.ToLower())
//            {
//              found = true;
//              break;
//            }
//          }

//          if (!found)
//          {
//            Game.CurrentGame.Messages.Add("Deny : " + ore);
//            mining.DenyOres.Add(ore);
//          }
//        }
//      }

//      Game.CurrentGame.Messages.Add("StartPosition: " + mining.StartPosition);
//      if (new Robot().GoTo(mining.StartPosition, 0))
//      {
//        mining.MiningRecursive();
//      }
//      Game.CurrentGame.Messages.Add("Kopani dodkonceno.");
//    }

//    //---------------------------------------------------------------------------------------------

//    [Executable]
//    public static void StartMiningUnderFeat(string fullContainerType, bool hide, ushort startX, ushort startY, ushort forgeX, ushort forgetY, ushort containerX, ushort containerY, ushort entranceX, ushort entranceY, ushort barnkcontainertype, params string[] oreKinds)
//    {
//      Mining mining = new Mining();
//      //mining.BankContainerType = 0x0E7E;
//      mining.FullContainerType = fullContainerType;
//      mining.StartPosition = new UOPosition((startX == 0 ? World.Player.X : startX), (startY == 0 ? World.Player.Y : startY), 0);//new Robot().ActualPosition.Clone();
//      mining.ForgePosition = new UOPosition(forgeX, forgetY, 0);
//      mining.ContainerPosition = new UOPosition(containerX, containerY, 0);
//      mining.EntrancePosition = new UOPosition(entranceX, entranceY, 0);
//      mining.UseHiding = hide;
//      mining.BankContainerType = new Graphic(barnkcontainertype);

//      mining.UnderFeatContainer = new UOItem(UIManager.TargetObject());

//      UO.Print("StartMining s nasledujicimi parametry:");
//      UO.Print("FullContainerType:" + mining.FullContainerType);
//      UO.Print("StartPosition:" + mining.StartPosition);
//      UO.Print("ForgePosition:" + mining.ForgePosition);
//      UO.Print("ContainerPosition:" + mining.ContainerPosition);
//      UO.Print("EntrancePosition:" + mining.EntrancePosition);
//      UO.Print("UseHiding:" + mining.UseHiding);
//      UO.Print("BankContainerType:" + mining.BankContainerType);
//      UO.Print("UnderFeatContainer:" + mining.UnderFeatContainer.Serial);

//      if (oreKinds.Length > 0)
//      {
//        foreach (string ore in AllOreNames)
//        {
//          bool found = false;
//          foreach (string allowedOre in oreKinds)
//          {
//            if (allowedOre.ToLower() == ore.ToLower())
//            {
//              found = true;
//              break;
//            }
//          }

//          if (!found)
//          {
//            Game.CurrentGame.Messages.Add("Deny : " + ore);
//            mining.DenyOres.Add(ore);
//          }
//        }
//      }

//      Game.CurrentGame.Messages.Add("StartPosition: " + mining.StartPosition);
//      if (new Robot().GoTo(mining.StartPosition, 0))
//      {
//        mining.MiningRecursive();
//      }
//      Game.CurrentGame.Messages.Add("Kopani dodkonceno.");
//    }


//    //---------------------------------------------------------------------------------------------

//    [Executable]
//    public static void StartMiningDomecek(/*string trackName, */params string[] oreKinds)
//    {
//      //Nastaveni tezby
//      Mining mining = new Mining();
//      mining.BankContainerType = 0x0E7E;//Tym kontaineru v bance do ktereho se budou ukladat ingoty. Pokud neni definovan hodi se to do banky.
//      mining.MimingFullMethod = new MiningBagFull(mining.DoDomeckuAZpet); //metoda ketra se provede pri plnem batohu ingotu

//      if (oreKinds != null && oreKinds.Length > 0)
//      {
//        foreach (string ore in AllOreNames)
//        {
//          bool found = false;
//          foreach (string allowedOre in oreKinds)
//          {
//            if (allowedOre.ToLower() == ore.ToLower())
//            {
//              found = true;
//              break;
//            }
//          }

//          if (!found)
//          {
//            Game.CurrentGame.Messages.Add("Deny : " + ore);
//            mining.DenyOres.Add(ore);
//          }
//          //if (Array.IndexOf(oreKinds, ore.ToLower()) < 0)
//          //{
//          //  Game.CurrentGame.Messages.Add("Deny found: " + correctedName);
//          //  mining.DenyOres.Add(ore);
//          //}
//        }
//      }

//      UO.Print("10s do zacatku miningu, vyberte forge pro smelovani a jdete na startovaci pozici.");
//      mining.SetForgePosition();
//      UO.Wait(10000);
//      mining.MiningRecursive();

//      UO.Print("Kopani dodkonceno.");
//    }

//    //---------------------------------------------------------------------------------------------

//    #region exec



//    #endregion
//  }
//}
