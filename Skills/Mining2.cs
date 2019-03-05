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
using System.Collections;

namespace CalExtension.Skills
{
  public class Mining2 : Skill
  {
    //public List<Graphic> OreGraphic = new List<Graphic>() { 0x19B7, 0x19BA, 0x19B8, 0x19B9 }

    //---------------------------------------------------------------------------------------------

    public enum OreGraphic
    {
      One = 0x19B7,
      Two = 0x19BA,
      Tree = 0x19B8,
      Many = 0x19B9
    }

    //---------------------------------------------------------------------------------------------

    public enum IngotGraphic
    {
      type1 = 0x1BEF,//0x1BEF  
      type2 = 0x1BE3,
      type3 = 0x1BF5,
      type4 = 0x1BE9
    }

    //---------------------------------------------------------------------------------------------

    public enum JewelGraphic
    {
      A = 0x0F13,
      B = 0x0F15,
      C = 0x0F0F,
      D = 0x0F10,
      E = 0x0F26,
      F = 0x0F16
    }

    //---------------------------------------------------------------------------------------------

    public enum OreColor
    {
      Iron = 0x0000,
      Copper = 0x0289,
      Bronze = 0x01BF,
      Silver = 0x0482,
      Shadow = 0x0322,
      Rose = 0x0665,
      Golden = 0x0160,
      Verite = 0x01CB,
      Valorite = 0x0253,
      BloodRock = 0x04C2,
      BlackRock = 0x0455,
      Mytheril = 0x052D,
      StarSapphire = 0x0006,
      Emerald = 0x0041,
      Citrine = 0x002C,
      Amethyst = 0x0015,
      Ruby = 0x0027,
      Diamond = 0x03E9
    }

    //---------------------------------------------------------------------------------------------

    public enum IgnotColor//TODO zatim je spravne jen bronze
    {
      Iron = 0x0000,
      Copper = 0x0289,
      Bronze = 0x06D6,
      Silver = 0x0482,
      Shadow = 0x0322,
      Rose = 0x0665,
      Golden = 0x0160,
      Verite = 0x01CB,
      Valorite = 0x0253,
      BloodRock = 0x04C2,
      BlackRock = 0x0455,
      Mytheril = 0x052D,
      StarSapphire = 0x0006,
      Emerald = 0x0041,
      Citrine = 0x002C,
      Amethyst = 0x0015,
      Ruby = 0x0027,
      Diamond = 0x03E9
    }

    //---------------------------------------------------------------------------------------------
    public static List<string> AllOreNames { get { return new List<string>() { "Iron", "Copper", "Bronze", "Silver", "Shadow", "Rose", "Golden", "Verite", "Valorite", "BloodRock", "BlackRock", "Mytheril", "StarSapphire", "Emerald", "Citrine", "Amethyst", "Ruby", "Diamond" }; } }
    public static Graphic ForgeGraphic = 0x0FB1;
    public static Graphic KrumpacGraphic = 0x0E85;

    //---------------------------------------------------------------------------------------------

    public Serial Container;
    public Serial InnerContainer;
    public Layer ContainerLayer = Layer.None;
    //public IUOPosition ContainerPosition;
    public IUOPosition StartPosition;
    public IUOPosition LastPosition;
    //public IUOPosition ForgePosition;
    public List<IUOPosition> RessPositionPath;
    //public IUOPosition EntryPosition;
    public int MaxN = 0;
    public int MaxE = 0;
    public int MaxS = 0;
    public int MaxW = 0;
    public string DenyOresString;
    public bool UseHiding = false;
    public int WeightOffset = 70;
    public int KumpLoad = 6;
    public bool WeightLimitReached { get { return World.Player.Weight > (World.Player.MaxWeight - this.WeightOffset); } }
    public bool WeightLimitReachedSmelted { get { return World.Player.Weight > (World.Player.MaxWeight - 150); } }
    public List<IUOPosition> BodyNespojitosti;
    public int indexBodyNespojitosti = -1;
    public int MaxForensicSkill = 0;
    public bool EnableVisitorInfo = true;
    public VisitorInfoList Visitors;
    public int ContainerDistance = 0;
    public string MoveKind = "EW";
    public bool Loop = true;
    public string PathToMine;
    public bool ForceVykladat = true;

    public string ContainerPath;
    public string StartPath;
    public string ForgePath;
    public string ForgePath2;
    public bool FullMine = true;

    //--------------------------------------------------------------------------------------------- 
    [Executable]
    public static void StartMiningSimple(string direction)
    {
      Mining2 mining = new Mining2();
      mining.TakeOre = true;

      while (!UO.Dead)
      {
        KopInfo lastKop = mining.MineTile();

        if (lastKop.Mined || lastKop.WrongTile || lastKop.Deny)
        {
          if (direction == "SN")
          {
            mining.MoveSN();
          }
          else
            mining.MoveEW();
        }
      }
    }

    [Executable("StartMining2")]
    public static void StartMining(Serial container, Serial innerContainer, string entryPosition, string startPosition, string containerPosition, string forgePosition, string ressPosition, string bodyNespojitosti, string denyOres, int maxForesnicSkill, int kumpLoad, bool useHiding, bool enableVisitor)
    {
      StartMining(container, innerContainer, entryPosition, startPosition, containerPosition, forgePosition, ressPosition, bodyNespojitosti, denyOres, maxForesnicSkill, kumpLoad, useHiding, enableVisitor, null);
    }

    //--------------------------------------------------------------------------------------------- 

    [Executable("StartMining2")]
    public static void StartMining(Serial container, Serial innerContainer, string entryPosition, string startPosition, string containerPosition, string forgePosition, string ressPosition, string bodyNespojitosti, string denyOres, int maxForesnicSkill, int kumpLoad, bool useHiding, bool enableVisitor, string moveKind)
    {
      Mining2 mining = new Mining2();
      mining.Container = container;
      mining.InnerContainer = innerContainer;
      //mining.ContainerPosition = UOPositionBase.Parse(containerPosition);
      mining.ContainerPath = containerPosition;
      mining.ForgePath = forgePosition;
      

     // mining.ForgePosition = UOPositionBase.Parse(forgePosition);
      mining.RessPositionPath = UOPositionBase.ParseList(ressPosition);
      //mining.EntryPosition = UOPositionBase.Parse(entryPosition);
      mining.PathToMine = entryPosition;


      mining.StartPosition = String.IsNullOrEmpty(startPosition) ? new UOPositionBase(World.Player.X, World.Player.Y, (ushort)World.Player.Z) : UOPositionBase.Parse(startPosition);
      mining.BodyNespojitosti = new List<IUOPosition>() { mining.StartPosition };
      mining.BodyNespojitosti.AddRange(UOPositionBase.ParseList(bodyNespojitosti));
      mining.MaxForensicSkill = maxForesnicSkill;
      mining.KumpLoad = kumpLoad;
      mining.UseHiding = useHiding;
      mining.EnableVisitorInfo = enableVisitor;
      mining.MoveKind = moveKind;

      foreach (string s in denyOres.Split(new char[] { ',' }))
        mining.DenyOres.Add(s);

      UO.Print(0x0035, "StartMining s nasledujicimi parametry:");
      UO.Print(0x0035, "Container:" + mining.Container);
      UO.Print(0x0035, "InnerContainer:" + mining.InnerContainer);
      UO.Print(0x0035, "ContainerPath:" + mining.ContainerPath);
      UO.Print(0x0035, "ForgePath:" + mining.ForgePath);
      UO.Print(0x0035, "RessPosition:" + UOPositionBase.ListToString(mining.RessPositionPath));
      UO.Print(0x0035, "PathToMine:" + mining.PathToMine);
      UO.Print(0x0035, "StartPosition:" + mining.StartPosition);
      UO.Print(0x0035, "BodyNespojitosti:" + UOPositionBase.ListToString(mining.BodyNespojitosti));
      UO.Print(0x0035, "MaxForensicSkill:" + mining.MaxForensicSkill);
      UO.Print(0x0035, "KumpLoad:" + mining.KumpLoad);
      UO.Print(0x0035, "UseHiding:" + mining.UseHiding);
      UO.Print(0x0035, "EnableVisitorInfo:" + mining.EnableVisitorInfo);
      UO.Print(0x0191,"DenyOres:" + String.Join(",", mining.DenyOres.ToArray()));

      mining.Mine();
    }

    //--------------------------------------------------------------------------------------------- 

    [Executable]
    public static void StartMiningMultiple(params string[] mineDefinition)
    {
      while (!UO.Dead)
      {
        Game.PrintMessage("mineDefinition: " + mineDefinition.Length);

        foreach (string def in mineDefinition)
        {
          Mining2 mining = new Mining2();
          mining.FillFromDefinition(def);
          mining.Loop = false;

          UO.Print(0x0035, "StartMining s nasledujicimi parametry:");
          UO.Print(0x0035, "Container:" + mining.Container);
          UO.Print(0x0035, "InnerContainer:" + mining.InnerContainer);
          UO.Print(0x0035, "ContainerPath:" + mining.ContainerPath);
          UO.Print(0x0035, "ForgePosition:" + mining.ForgePath);
          UO.Print(0x0035, "ForgePath:" + UOPositionBase.ListToString(mining.RessPositionPath));
          UO.Print(0x0035, "PathToMine:" + mining.PathToMine);
          UO.Print(0x0035, "StartPosition:" + mining.StartPosition);
          UO.Print(0x0035, "BodyNespojitosti:" + UOPositionBase.ListToString(mining.BodyNespojitosti));
          UO.Print(0x0035, "MaxForensicSkill:" + mining.MaxForensicSkill);
          UO.Print(0x0035, "KumpLoad:" + mining.KumpLoad);
          UO.Print(0x0035, "UseHiding:" + mining.UseHiding);
          UO.Print(0x0035, "EnableVisitorInfo:" + mining.EnableVisitorInfo);
          UO.Print(0x0191, "DenyOres:" + String.Join(",", mining.DenyOres.ToArray()));


          mining.Mine();


        }
      }
    }
    //--------------------------------------------------------------------------------------------- 

    private void FillFromDefinition(string definition)
    {
      string[] split = definition.Split(new char[] { ' ', ';' }, StringSplitOptions.None);

      this.PathToMine = split[0];

      this.Container = Serial.Parse(split[1]);
      if (!String.IsNullOrEmpty(split[2]))
        this.InnerContainer = Serial.Parse(split[2]);
      //split[3];//zrusit
      this.StartPosition = String.IsNullOrEmpty(split[4]) ? new UOPositionBase(World.Player.X, World.Player.Y, (ushort)World.Player.Z) : UOPositionBase.Parse(split[4]);
      this.ContainerPath = split[5];//TODO udelat jako path
      this.ForgePath = split[6];//udelat jako path
      this.RessPositionPath = UOPositionBase.ParseList(split[7]);
      this.BodyNespojitosti = new List<IUOPosition>() { this.StartPosition };

      this.BodyNespojitosti.AddRange(UOPositionBase.ParseList(split[8]));

      foreach (string s in (split[9] + String.Empty).Split(new char[] { ',' }))
        this.DenyOres.Add(s);

      this.MaxForensicSkill = Int32.Parse(split[10]);
      this.KumpLoad = Int32.Parse(split[11]);
      this.UseHiding = bool.Parse(split[12]);
      this.EnableVisitorInfo = bool.Parse(split[13]);
      this.MoveKind = split[14];

      if (split.Length > 15)
        this.ContainerLayer = (Layer)Enum.Parse(typeof(Layer), split[15]);

      if (split.Length > 16)
        this.ForceVykladat = bool.Parse(split[16]);

      if (split.Length > 17 )
        this.ForgePath2 = split[17];

      if (split.Length > 18)
        this.FullMine = bool.Parse(split[18]);
    }

    //--------------------------------------------------------------------------------------------- 

    public Mining2()
    {
      Game.CurrentGame.Mode = GameMode.Working;
      World.CharacterAppeared += World_CharacterAppeared;
      this.Visitors = new VisitorInfoList();
    }

    ~Mining2()
    {
      World.CharacterAppeared -= World_CharacterAppeared;
    }

    //---------------------------------------------------------------------------------------------

    private void World_CharacterAppeared(object sender, CharacterAppearedEventArgs e)
    {
      if (this.EnableVisitorInfo)
      {
        VisitorInfo info = this.Visitors[e.Serial];
        if (String.IsNullOrEmpty(info.Name))
        {
          info.Character.Click();
          Game.Wait(250);
          info.Name = info.Character.Name;
        }

        if (info.LastVisit == null ||( DateTime.Now  - info.LastVisit.VisitTime).TotalMinutes > 1)
          info.Visits.Add(new VisitInfo() { VisitTime = DateTime.Now });
      }
    }

    //---------------------------------------------------------------------------------------------

    protected List<string> denyOres;
    public List<string> DenyOres
    {
      get
      {
        if (this.denyOres == null)
        {
          this.denyOres = new List<string>();
        }
        return this.denyOres;
      }
      set { this.denyOres = value; }
    }

    //---------------------------------------------------------------------------------------------

    protected Robot robot;
    public Robot Robot
    {
      get
      {
        if (this.robot == null)
        {
          this.robot = new Robot();
          this.robot.UseCachedPathList = true;
          //this.robot.EnableLog = true;
        }
        return this.robot;
      }
    }

    //---------------------------------------------------------------------------------------------

    public static UOItem Krumpac
    {
      get
      {
        UOItem krumpac = new UOItem(Serial.Invalid);
        if (!(krumpac = World.Player.Layers.FindType(KrumpacGraphic)).Exist)
        {
          World.Player.Backpack.AllItems.FindType(KrumpacGraphic);
        }
        if (!krumpac.Exist) UO.Print("Nemas krupac!");
        return krumpac;
      }
    }

    //---------------------------------------------------------------------------------------------

    public static bool MamSkladacku
    {
      get
      {
        return Krumpac.Exist && Krumpac.Color == 0x0481;
      }
    }

    //---------------------------------------------------------------------------------------------

    protected void EnsureHiding()
    {
      if (UseHiding)
       // && !World.Player.Hidden)
      {
        while (!World.Player.Hidden)
        {
          UO.DeleteJournal();

          UO.UseSkill(StandardSkill.Hiding);
          Journal.WaitForText(true, 4000, "You have hidden yourself", "seem to hide here");
        }
        Game.Wait(250);
      }
    }

    //---------------------------------------------------------------------------------------------

    protected bool MimoZonu()
    {
      if (this.MaxN != 0 && this.MaxN >= World.Player.Y)
        return true;
      if (this.MaxE != 0 && this.MaxE <= World.Player.X)
        return true;
      if (this.MaxS != 0 && this.MaxS <= World.Player.Y)
        return true;
      if (this.MaxW != 0 && this.MaxW >= World.Player.X)
        return true;

      return false;
    }

    //---------------------------------------------------------------------------------------------

    protected bool BezVylozit()
    {
      if (!String.IsNullOrEmpty(this.ContainerPath))
      {
        UO.Print("BezVylozit");

        if (PlayerExtended.GoByPath(this.ContainerPath))//this.Robot.GoTo(this.ContainerPosition, this.ContainerDistance))
        {
          Serial container;

          if (this.ContainerLayer == Layer.Bank)
          {
            UO.Say("Bank");
            Game.Wait();
            container = World.Player.Layers[Layer.Bank];
          }
          else
            container = this.Container;

          UOItem mainCont = new UOItem(container);
          ItemHelper.EnsureContainer(mainCont);
          Game.Wait();
          UO.Backpack.Use();
          Game.Wait();

          Serial dropContainer = container;
          if (this.InnerContainer != null && new UOItem(this.InnerContainer).Exist)
            dropContainer = this.InnerContainer;

          UO.Print(dropContainer + " / ");

          UOItem ingot = new UOItem(Serial.Invalid);
          //List<UOItem> backpackItems = new List<UOItem>();
          //backpackItems.AddRange(World.Player.Backpack.Items.ToArray());

          //foreach (UOItem bItem in backpackItems)
          //{
          //  if (IsIngot(bItem))

          //  {
          //    bItem.Move(100, dropContainer);
          //    Game.Wait();
          //  }

          //}
          int sychr = 0;
          while (sychr < 12 && dropContainer.IsValid && ((ingot = FindIngot(UO.Backpack.Items)).Exist || (ingot = FindDarhokam(UO.Backpack.Items)).Exist))
          {
            sychr++;
            ingot.Move(100, dropContainer);
            Game.Wait();
          }

          Game.Wait();

          bool opened = false;
          sychr = 0;
          if (!MamSkladacku)
          {
            while (World.Player.Backpack.AllItems.Count(KrumpacGraphic) <= this.KumpLoad && sychr < this.KumpLoad * 2)
            {
              if (!opened)
              {
                ItemHelper.OpenContainerRecursive(container);
                Game.Wait();
                opened = true;
              }
              if (mainCont.AllItems.FindType(KrumpacGraphic).Exist)
              {
                sychr++;
                mainCont.AllItems.FindType(KrumpacGraphic).Move(1, World.Player.Backpack);
                Game.Wait();
              }
              else
                break;
            }
          }

          return PlayerExtended.GoByPath(this.ContainerPath, true, false) && this.BezLast();
        }
        else
          UO.Print("BezVylozit - !GoTo ContainerPosition");
      }
      else
        UO.Print("BezVylozit - ContainerPosition == null");

      return false;
    }

    //---------------------------------------------------------------------------------------------

    protected List<Serial> lackOres = new List<Serial>();
    protected bool BezSmeltit()
    {
      if (!String.IsNullOrEmpty(this.ForgePath))
      {
        UO.Print("BezSmeltit");

        string path = this.ForgePath;
        IUOPosition c = new UOPositionBase(World.Player.X, World.Player.Y, 0);

        if (!String.IsNullOrEmpty(this.ForgePath2))
        {
          List<IUOPosition> l1 = UOPositionBase.ParseList(this.ForgePath);
          List<IUOPosition> l2 = UOPositionBase.ParseList(this.ForgePath2);
          IUOPosition p1 = l1[l1.Count - 1];
          IUOPosition p2 = l2[l2.Count - 1];


          double d1 = Robot.GetRelativeVectorLength(c, p1);
          double d2 = Robot.GetRelativeVectorLength(c, p2);

          if (d2 < d1)
          {
            path = ForgePath2;
          }
        }

        List<IUOPosition> pl = UOPositionBase.ParseList(path);

        if (pl.Count > 0)
        {
          IUOPosition pl1 = pl[0];
          IUOPosition pl2 = pl[pl.Count - 1];

          double pld1 = Robot.GetRelativeVectorLength(c, pl1);
          double pld2 = Robot.GetRelativeVectorLength(c, pl2);

          //if (pld2 < pld1)
          //  path = pl2.X + "." + pl2.Y;

        }

        if (PlayerExtended.GoByPath(path))//this.Robot.GoTo(this.ForgePosition, 1))
        {
          this.EnsureHiding();
          UOItem findForge = World.Ground.FindType(ForgeGraphic);

          if (findForge.Exist)
          {
            while
            (
              UO.Backpack.Items.FindType((int)OreGraphic.One).Exist || UO.Backpack.Items.FindType((int)OreGraphic.Two).Exist ||
              UO.Backpack.Items.FindType((int)OreGraphic.Tree).Exist || UO.Backpack.Items.FindType((int)OreGraphic.Many).Exist
            )
            {

              UOItem ore = null;

              foreach (UOItem item in UO.Backpack.Items)
              {
                if (!lackOres.Contains(item.Serial) && (item.Graphic == (int)OreGraphic.One || item.Graphic == (int)OreGraphic.Two || item.Graphic == (int)OreGraphic.Tree || item.Graphic == (int)OreGraphic.Many))
                {
                  ore = item;
                  break;

                }
              }

              if (ore != null)
              {
                JournalEventWaiter jew = new JournalEventWaiter(true, "You lack the skill to smelt this ore");
                findForge.Use();
                UO.WaitTargetObject(ore.Serial);
                if (jew.Wait(500 + Core.Latency))
                {
                  lackOres.Add(ore.Serial);
                  ore.DropHere();
                }

                Game.Wait(500);
              }
              else
              {
                break;
              }

    
            }

            return pl.Count > 1 ? PlayerExtended.GoByPath(path, true, false) : true;
          }
          else
            UO.Print("BezSmeltit - !Forge");


        }
        else
          UO.Print("BezSmeltit - !GoTo ForgePosition");
      }
      else
        UO.Print("BezSmeltit - ContainerPosition == null");

      return false;
    }

    //---------------------------------------------------------------------------------------------

    //---------------------------------------------------------------------------------------------

    protected bool BezSeResnout()
    {
      bool ressSuccess = false;
      if (this.RessPositionPath != null)
      {
        UO.Print("BezSeResnout");
        int gotoDistance = this.RessPositionPath.Count == 1 ? 1 : 0;

        if (this.Robot.GoTo(this.RessPositionPath, gotoDistance))
        {
          UOCharacter resser = null;

          foreach (UOCharacter ch in World.Characters)
          {
            if (ch.Notoriety == Notoriety.Innocent || ch.Notoriety == Notoriety.Invulnerable || ch.Notoriety == Notoriety.Neutral || ch.Distance < 10)
            {
              resser = ch;
              break;
            }
          }

          if (World.Ground.FindType(0x1E5D).Exist)////0x1E5D ress kriz
          {
            UOItem ressKriz = World.Ground.FindType(0x1E5D);
            if (ressKriz > 3)
            {
              this.Robot.GoTo(ressKriz.X, ressKriz.Y, 1, 100);
            }

            ressKriz.Click();
            Game.Wait();
            World.Player.ChangeWarmode(WarmodeChange.Switch);
            Game.Wait();
            World.Player.ChangeWarmode(WarmodeChange.War);
            Game.Wait();
            ressKriz.Use();
            Game.Wait();
            World.Player.RequestStatus(1000);

            ressSuccess = !UO.Dead;
          }
          else if (resser != null && resser.Exist)
          {
            if (this.Robot.GoTo(resser.X, resser.Y, 2, 50))
            {
              resser.Click();
              Game.Wait();
              World.Player.ChangeWarmode(WarmodeChange.Switch);
              Game.Wait();
              World.Player.ChangeWarmode(WarmodeChange.War);
              resser.Tell("ress");
              Game.Wait();
              World.Player.RequestStatus(1000);

              ressSuccess = !UO.Dead;
            }
            else
              UO.Print("BezSeResnout - !Goto resser");
          }
          else
            UO.Print("BezSeResnout - !resser");
        }
        else
          UO.Print("BezSeResnout - !GoTo ForgePosition");

        if (ressSuccess && this.RessPositionPath.Count > 1)
        {
          List<IUOPosition> reverse = new List<IUOPosition>();
          reverse.AddRange(this.RessPositionPath.ToArray());
          reverse.Reverse();

          this.Robot.GoTo(reverse, gotoDistance);
        }
      }
      else
      {
        UO.Print("BezSeResnout - RessPosition == null");
        Game.Wait(1500);
      }

      return ressSuccess;
    }

    //---------------------------------------------------------------------------------------------

    protected UOItem MyDeadBody()
    {
      foreach (UOItem item in World.Ground)
      {
        if (item.Distance < 3 && item.Graphic == 0x2006)
        {
          if (String.IsNullOrEmpty(item.Name))
          {
            item.Click();
            Game.Wait();
          }

          if (String.IsNullOrEmpty(World.Player.Name))
          {
            World.Player.Click();
            Game.Wait();
          }

          if ((String.Empty + item.Name).Contains("Body of " + World.Player.Name))
          {
            return item;
          }
        }
      }
      return null;
    }

    //---------------------------------------------------------------------------------------------

    protected void VylotSiTelo()
    {
      UOItem myDeadBody = this.MyDeadBody();
      if (myDeadBody != null && myDeadBody.Exist)
      {
        if (!myDeadBody.Opened)
        {
          myDeadBody.Use();
          Game.Wait();
        }

        foreach (UOItem lootItem in myDeadBody.AllItems)
        {
          lootItem.Move(10000, World.Player.Backpack);
          Game.Wait();
        }
      }

      foreach (UOItem lootItem in World.Ground)
      {
        if (lootItem.Distance < 4 && lootItem.Graphic == KrumpacGraphic)
        {
          lootItem.Move(10000, World.Player.Backpack);
          Game.Wait();
        }
      }
    }

    //---------------------------------------------------------------------------------------------


    //"↑" - MovementDirection.Up
   
    //"↗" - MovementDirection.UpRight
    //"→" - MovementDirection.Right
    //"↘" - MovementDirection.DownRight

    //"↖" - MovementDirection.UpLeft)
    //"←" - MovementDirection.Left)
    //"↙" - MovementDirection.DownLeft)

    //"↓" -  MovementDirection.Down


    protected bool MoveEW()
    {
      if (!this.Robot.Move(MovementDirection.UpLeft))
      {
        if (this.Robot.Move(MovementDirection.DownLeft))
        {
          int steps = 0;
          while (this.Robot.Move(MovementDirection.DownRight))
          {
            TakeAllOre();
            steps++;
            if (steps > 1)
            {
              steps = 0;
            }
          }
          TakeAllOre();
          return true;
        }
        else
        {
          bool findPath = false;
          while (this.Robot.Move(MovementDirection.DownRight))
          {
            TakeAllOre();
            if (this.Robot.Move(MovementDirection.DownLeft))
            {
              findPath = true;
              break;
            }
          }
          if (findPath)
          {
            int steps = 0;
            while (this.Robot.Move(MovementDirection.DownRight))
            {
              TakeAllOre();
              steps++;
              if (steps > 1)
              {
                steps = 0;
              }
            }
            TakeAllOre();
            return true;
          }
          else
          {
            return false;
          }
        }
      }
      else
      {
        TakeAllOre();
        return true;
      }
    }

    //---------------------------------------------------------------------------------------------

    private bool TakeOre = false;
    protected void TakeAllOre()
    {
      if (TakeOre)
      {
        int originalFindDistance = World.FindDistance;
        World.FindDistance = 4;

        List<UOItem> ores = World.Ground.Where(i => IsAllowOre(i) && i.Distance > 0 && i.Distance <= 4).ToList();
        foreach (UOItem ore in ores)
        {
          ore.Move(50, World.Player.X, World.Player.Y, World.Player.Z);
          Game.Wait(150);
        }

        World.FindDistance = originalFindDistance;
      }
    }

    //---------------------------------------------------------------------------------------------


    protected bool MoveSN()
    {
      if (!this.Robot.Move(MovementDirection.UpRight))
      {
        if (this.Robot.Move(MovementDirection.UpLeft))
        {
          int steps = 0;
          while (this.Robot.Move(MovementDirection.DownLeft))
          {
            TakeAllOre();
            steps++;
            if (steps > 1)
            {
              steps = 0;
            }
          }
          TakeAllOre();
          return true;
        }
        else
        {
          bool findPath = false;
          while (this.Robot.Move(MovementDirection.DownLeft))
          {
            TakeAllOre();
            if (this.Robot.Move(MovementDirection.UpLeft))
            {
              findPath = true;
              break;
            }
          }
          if (findPath)
          {
            int steps = 0;
            while (this.Robot.Move(MovementDirection.DownLeft))
            {
              TakeAllOre();
              steps++;
              if (steps > 1)
              {
                steps = 0;
              }
            }
            TakeAllOre();
            return true;
          }
          else
          {
            return false;
          }
        }
      }
      else
      {
        TakeAllOre();
        return true;
      }
    }

    //---------------------------------------------------------------------------------------------

    public bool GoToNext()
    {
      if (this.BodyNespojitosti != null && this.BodyNespojitosti.Count > 0)
      {
        this.indexBodyNespojitosti++;

        if (this.indexBodyNespojitosti > this.BodyNespojitosti.Count - 1)
        {
          this.indexBodyNespojitosti = 0;
          if (!this.Loop)
          {
            UO.Print("!this.Loop - End");
            return false;
          }
        }

        UO.Print("GoToNext - GoTo - " + this.indexBodyNespojitosti + " / " + this.BodyNespojitosti[this.indexBodyNespojitosti].ToString());
        return this.Robot.GoTo(this.BodyNespojitosti[this.indexBodyNespojitosti]);
      }
      else
        UO.Print("GoToNext - BodyNespojitosti == null || == 0");

      return false;
    }

    //---------------------------------------------------------------------------------------------

    protected KopInfo MineTile()
    {
      KopInfo info = new KopInfo();
      Journal.Clear();

      if (Game.CurrentGame.WorldSave())
      {
        Game.Wait(30000);
        Krumpac.Move(1, World.Player.Backpack);
        Game.Wait(500);
        Krumpac.Click();
      }

      SkillValue miningSkill = SkillsHelper.GetSkillValue("Mining");

      List<string> waitTexts = new List<string>();
      if (MamSkladacku)
      {
        waitTexts.Add("There is no");
        waitTexts.Add("Try mining in rock");
        waitTexts.Add("You put");
        waitTexts.Add("You loosen some");
      }
      else
        waitTexts.Add("...akce skoncila");

      JournalEventWaiter jew = new JournalEventWaiter(true, waitTexts.ToArray());

      Krumpac.Use();
      UO.WaitTargetTileRel(0, 0, 0, 0);
      jew.Wait(7500 + Core.Latency);

      info.Mined = Journal.Contains(true, "There is no");
      info.WrongTile = Journal.Contains(true, "Try mining in rock");
      info.Deny = JournalContainsDenyOre();

      if (MamSkladacku)
      {
        Game.Wait(400);
      }

      return info;
    }

    //---------------------------------------------------------------------------------------------

    public bool BezLast()
    {
      if (this.LastPosition != null)
      {
        UO.Print("BezLast");


        if (this.Robot.GoTo(this.LastPosition))
          return true;
        else
          UO.Print("BezLast - !GoTo LastPosition");
      }
      else
        UO.Print("BezLast - LastPosition == null");

      return false;
    }

    //---------------------------------------------------------------------------------------------

    public void Mine()
    {
      World.Player.RequestStatus(1000);
      World.Player.Click();
      Game.Wait();
      bool doMine = true;
      if (!String.IsNullOrEmpty(this.PathToMine))
      {
        doMine = PlayerExtended.GoByPath(this.PathToMine);
      }

      if (doMine)
        doMine = this.GoToNext();
      bool move = false;
      int iteration = 0;
      KopInfo lastKop = new KopInfo();

      while (doMine)
      {
        UO.Print("doMine" + iteration);
        iteration++;

        UOItem myDeadBody = this.MyDeadBody();
        bool dead = UO.Dead;
        bool returnToLast = false;
        bool goToNext = false;

        bool trainForensic = this.MaxForensicSkill > 0 && !dead && myDeadBody != null && myDeadBody.Exist;
        this.LastPosition = new UOPositionBase(World.Player.X, World.Player.Y, 0);

        UO.DeleteJournal();

        if (Game.CurrentGame.WorldSave())
        {
          Game.Wait(30000);
          if (!dead)
          {
            Krumpac.Move(1, World.Player.Backpack);
            Game.Wait(500);
            Krumpac.Click();
          }
          Game.Wait();

          if (World.Player.Backpack.AllItems.FindType(0x0FF0).Exist)
          {
            World.Player.Backpack.AllItems.FindType(0x0FF0).Move(1, World.Player.Backpack);//Sychr test na zasek
            Game.Wait();
          }
          if (myDeadBody != null && myDeadBody.Exist)
          {
            myDeadBody.Click();
          }
        }

        if (this.EnableVisitorInfo)
        {
          VisitorInfo visitor;
          if (this.NavstevnikHandle(out visitor).Result == ActionResult.Continue)
            continue;

          if (visitor != null && UO.Dead && visitor.LastVisit != null)
            visitor.LastVisit.ZabilMe = true;
        }

        if (!trainForensic) this.EnsureHiding();
        if (World.Player.Warmode)
          World.Player.ChangeWarmode(WarmodeChange.Peace);

        if (dead)
        {
          this.BezSeResnout();
          this.BezLast();
          this.VylotSiTelo();
          continue;
        }

        if (this.WeightLimitReached || (this.ForceVykladat && lastKop.Mined && FindOre(World.Player.Backpack.AllItems, (int)OreColor.Mytheril).Exist || FindOre(World.Player.Backpack.AllItems, (int)OreColor.BlackRock).Exist))
        {
          this.BezSmeltit();
          if (this.WeightLimitReachedSmelted || (this.ForceVykladat && lastKop.Mined && (FindIngot(World.Player.Backpack.AllItems, (int)IgnotColor.Mytheril).Exist || FindIngot(World.Player.Backpack.AllItems, (int)IgnotColor.BlackRock).Exist)))
            this.BezVylozit();
          returnToLast = true;
        }

        if (!dead && !Krumpac.Exist)
        {
          this.BezVylozit();
          returnToLast = true;
        }

        if (this.MimoZonu())
        {
          goToNext = true;
          UO.Print("MimoZonu - END");
        }

        if (returnToLast)
          this.Robot.GoTo(this.LastPosition);

        this.VylotSiTelo();
        this.DropDenyOre();

        if (trainForensic && SkillsHelper.GetSkillValue("ForensicEvaluation").Value < this.MaxForensicSkill)
        {
          Game.Wait(150);
          UO.Print("Train - ForensicEvaluation");
          UO.WaitTargetObject(myDeadBody);
          UO.UseSkill(StandardSkill.ForensicEvaluation);
          Journal.WaitForText(true, 2500, "this is body of", "You can tell nothing about the corpse.");// Game.Wait(1500);
          Game.Wait(150);
          continue;
        }

        if (move)
        {
          bool isMoved = false;
          if (this.MoveKind == "SN")
            isMoved = this.MoveSN();
          else
            isMoved = this.MoveEW();

          if (!isMoved)
          {
            goToNext = true;
            UO.Print("!isMoved - END");
          }
        }
        
        if (goToNext && !this.GoToNext())
        {
          if (this.BezSmeltit())
          {
            //this.BezVylozit();
          }
          break;
        }

        SkillValue miningSkill = SkillsHelper.GetSkillValue("Mining");

        lastKop = this.MineTile();
        move = lastKop.Mined || (lastKop.Deny && miningSkill.Value >= 1000 && !this.FullMine);

        if (lastKop.WrongTile && !this.GoToNext())
        {
          if (this.BezSmeltit())
          {
            //this.BezVylozit();
          }
          break;
        }
      }
    }

    //---------------------------------------------------------------------------------------------

    public ActionInfo NavstevnikHandle(out VisitorInfo visitor)
    {
      ActionInfo result = new ActionInfo();
      
      visitor = this.Visitors.LastVisitor;
      if (visitor != null && visitor.Character.Exist && visitor.Character.Distance < 10 && !UO.Dead)
      {
        VisitInfo visit = visitor.LastVisit;
        if (visit != null)
        {
          if (!visit.Snaped)
          {
            visitor.Character.Click();
            Game.Wait(250);
            ScreenCapture.ScreenCapture capture = new ScreenCapture.ScreenCapture();
            capture.snap((visitor.Name + String.Empty).Replace(" ", "_") + visit.VisitTime.ToString("yyyyMMdd_HHmmssfff"));
            visit.Snaped = true;
          }

          if ((visitor.Character.Model == 0x0190 || visitor.Character.Model == 0x0191) && (visitor.Character.Notoriety == Notoriety.Murderer || visitor.Character.Notoriety == Notoriety.Criminal || visitor.Character.Notoriety == Notoriety.Enemy))//Toto male/ female 
                                                                                                                                                                                                                                                     //hlasky dle deni doby atd.
          {
            if (!World.Player.Hidden)
            {
              if (visitor.KillCount > 0)
              {
                if (visit.Pruchod == 0)
                {
                  UO.Say("Jsi zpět krutý vrahu!");
                  Game.Wait(3500);
                  UO.Say("Přišel jsi opět zabít svou nebohou oběť, budiž... ");
                  Game.Wait(500);
                }
                else if (visit.Pruchod == 1)
                {
                  UO.Say("Můj nebohý úděl je již po " + (visitor.KillCount + 1) + " skonat tvou rukou..");
                  Game.Wait(6000);
                  UO.Say("Nechť jsou k tobě bozi miloství ... za zlo které pácháš ...");
                }
              }
              else if (visitor.Visits.Count > 1)
              {
                if (visit.Pruchod == 0)
                {
                  UO.Say("Zdravím tě můj největší ze zbojníků!!!");
                  Game.Wait(3500);
                  UO.Say("Jsem potěšen, že tvá velikost mě poctila svou návševou!!!");
                  Game.Wait(500);
                }
                else if (visit.Pruchod == 1)
                {
                  UO.Say("Všem jsem již vylíčil tvojí hrůznost a velikost v boji!!!");
                  Game.Wait(6000);
                  UO.Say("Pouze ta slova, je naplňují hrůzou");
                  Game.Wait(500);
                }
                else if (visit.Pruchod == 2)
                {
                  UO.Say("Věčná sláva tobě můj pane " + visitor.Name + "!!");
                  //TODO
                }
              }
              else
              {
                if (visit.Pruchod == 0)
                {
                  UO.Say("Aaaááááááá loupežník!!!");
                  Game.Wait(3500);
                  UO.Say("Vzdávam se *odhazuje krumpáč*");
                  Game.Wait(500);
                  //Krumpac.DropHere();

                }
                else if (visit.Pruchod == 1)
                {
                  UO.Say("Prosím o slitovaní óóó všemocný nejrůznější vladce zdejších lesů..");
                  Game.Wait(6000);
                  UO.Say("Ušetři můj bídný nicotný život... můsím celý den stravit v dole... ");
                  Game.Wait(6000);
                  UO.Say("abych užvil/a svou rodinu...");
                }
                else if (visit.Pruchod == 2)
                {
                  UO.Say("Pokud mě ušetříš, budu šiřit slova o tvé hrůze a nepřemožitelnosti...");
                  Game.Wait(6000);
                  UO.Say("Óóó nejhruznější z nejhruznějších !!!");
                  Game.Wait(2000);
                }
                else if (visit.Pruchod == 3)
                {
                  UO.Say("Věčná sláva tobě největší " + visitor.Name + "!!");
                  //TODO
                }
              }

              visit.Pruchod++;
            }
            else
            {
              Game.Wait();
              UO.Print("Cekam v HIDU");
            }
            Game.Wait(3500);
            result.Result = ActionResult.Continue;
          }
         
        }
      }

      return result;
    }

    //---------------------------------------------------------------------------------------------

    protected void DropDenyOre()
    {
      foreach (UOItem item in World.Player.Backpack.Items)
      {
        if (IsOre(item) && !IsAllowOre(item))
        {
          item.DropHere(item.Amount);
          Game.Wait(200);
        }
      }
      //foreach (object gra in Enum.GetValues(typeof(OreGraphic)))
      //{
      //  foreach (string name in Enum.GetNames(typeof(OreColor)))
      //  {
      //    bool found = false;
      //    foreach (string deny in this.DenyOres)
      //    {
      //      if (name.ToLower() == deny.ToLower())
      //      {
      //        found = true;
      //        break;
      //      }
      //    }

      //    if (found)
      //    {
      //      string correctedName = name.Substring(0, 1) + name.Substring(1, name.Length - 1);
      //      int oreColor = (int)Enum.Parse(typeof(OreColor), correctedName, true);

      //      if (UO.Backpack.Items.FindType((int)gra, oreColor).Exist)
      //      {
      //        Game.PrintMessage("Deny found: " + correctedName);

              
      //        UO.Backpack.Items.FindType((int)gra, oreColor).DropHere(10000);
      //        Game.Wait(200);
      //      }
      //    }
      //  }
      //}
    }

    //---------------------------------------------------------------------------------------------

    public static bool IsOre(UOItem item)
    {
      List<int> gra = new List<int>();
      foreach (object v in Enum.GetValues(typeof(OreGraphic)))
        gra.Add((int)v);

      if (gra.Count(g => g == item.Graphic) > 0)
        return true;

      return false;
    }

    //---------------------------------------------------------------------------------------------

    protected bool IsAllowOre(UOItem item)
    {
      List<int> col = new List<int>();

      foreach (object v in Enum.GetValues(typeof(OreColor)))
      {
        string name = Enum.GetName(typeof(OreColor), v);
        if (this.DenyOres.Where(d=>d.ToLower() == name.ToLower()).Count() == 0)
          col.Add((int)v);
      }

      if (IsOre(item) && col.Count(c => c == item.Color) > 0)
        return true;

      return false;
    }

    //---------------------------------------------------------------------------------------------

    protected bool JournalContainsDenyOre()
    {
      if (this.DenyOres.Count == 0) return false;
      else
      {
        foreach (string deny in this.DenyOres)
        {
          if (Journal.Contains(true, "You put the " + deny.ToLower() + " ores in your pack"))
            return true;
        }
        return false;
      }
    }

    //---------------------------------------------------------------------------------------------

    public static UOItem FindDarhokam(ItemsCollection items)
    {
      foreach (UOItem item in items)
      {
        if (IsDrahokam(item))
          return item;
      }
      return new UOItem(Serial.Invalid);
    }

    //---------------------------------------------------------------------------------------------

    public static bool IsDrahokam(UOItem ingot)
    {
      foreach (JewelGraphic gra in Enum.GetValues(typeof(JewelGraphic)))
      {
        if (ingot.Graphic == (int)gra)
        {
          return true;
        }
      }
      return false;
    }

    //---------------------------------------------------------------------------------------------

    public static UOItem FindIngot(ItemsCollection items)
    {
      foreach (UOItem item in items)
      {
        if (IsIngot(item))
          return item;
      }
      return new UOItem(Serial.Invalid);
    }

    //---------------------------------------------------------------------------------------------

    public static UOItem FindIngot(ItemsCollection items, UOColor color)
    {
      foreach (UOItem item in items)
      {
        if (IsIngot(item, color))
          return item;
      }
      return new UOItem(Serial.Invalid);
    }

    //---------------------------------------------------------------------------------------------

    public static bool IsIngot(UOItem ingot)
    {
      foreach (IngotGraphic ingotGraphic in Enum.GetValues(typeof(IngotGraphic)))
      {
        if (ingot.Graphic == (int)ingotGraphic)
        {
          return true;
        }
      }
      return false;
    }

    //---------------------------------------------------------------------------------------------

    public static bool IsIngot(UOItem ingot, UOColor color)
    {
      foreach (IngotGraphic ingotGraphic in Enum.GetValues(typeof(IngotGraphic)))
      {
        if (ingot.Graphic == (int)ingotGraphic && ingot.Color == color)
        {
          return true;
        }
      }
      return false;
    }

    //---------------------------------------------------------------------------------------------

    public static UOItem FindOre(ItemsCollection items, UOColor color)
    {
      foreach (UOItem item in items)
      {
        if (IsOre(item, color))
          return item;
      }
      return new UOItem(Serial.Invalid);
    }

    //---------------------------------------------------------------------------------------------

    public static bool IsOre(UOItem ore, UOColor color)
    {
      foreach (OreGraphic graphic in Enum.GetValues(typeof(OreGraphic)))
      {
        if (ore.Graphic == (int)graphic && ore.Color == color)
        {
          return true;
        }
      }
      return false;
    }

    //---------------------------------------------------------------------------------------------

    #region exec


    #endregion
  }

  public class KopInfo
  {
    public bool Mined = false;
    public bool Deny = false;
    public bool WrongTile = false;
  }

  public class VisitorInfoList
  {
    private List<VisitorInfo> list;
    private Hashtable ht;

    public VisitorInfoList()
    {
      list = new List<VisitorInfo>();
      ht = new Hashtable();
    }

    public VisitorInfo this[Serial serial]
    {
      get
      {
        VisitorInfo info = this.ht[serial] as VisitorInfo;

        if (info == null)
        {
          info = new VisitorInfo();
          info.Serial = serial;
          this.ht[serial] = info;
          this.list.Add(info);
        }
        return info;
      }
    }

    public VisitorInfo LastVisitor
    {
      get
      {
        if (this.list.Count > 0)
        {
          foreach (VisitorInfo visitor in this.list)
          {
            if (visitor.Character.Exist && (visitor.Character.Notoriety == Notoriety.Enemy || visitor.Character.Notoriety == Notoriety.Murderer || visitor.Character.Notoriety == Notoriety.Criminal))
              return visitor;
          }

          return this.list[this.list.Count - 1];
        }
        return null;
      }
    }
  }

  public class ActionInfo
  {
    public ActionResult Result = ActionResult.Void;
  }

  public enum ActionResult
  {
    Void = 1,
    True = 2,
    False = 3,
    Continue = 4
  }

  public class VisitorInfo
  {
    public string Name;
    public Serial Serial = Serial.Invalid;
    public List<VisitInfo> Visits;

    public VisitInfo LastVisit
    {
      get
      {
        if (Visits.Count > 0)
          return Visits[Visits.Count - 1];

        return null;
      }
    }

    public UOCharacter Character
    {
      get
      {
        return new UOCharacter(this.Serial);
      }
    }

    public VisitorInfo()
    {
      Visits = new List<VisitInfo>();
    }

    public int KillCount
    {
      get
      {
        int counter = 0;
        foreach (VisitInfo visit in this.Visits)
        {
          if (visit.ZabilMe)
            counter++;
        }
        return counter;
      }
    }
  
  }

  public class VisitInfo
  {
    public DateTime VisitTime;
    public bool ZabilMe = false;
    public bool Snaped = false;
    public int Pruchod = 0;
  }

}


//--------------------------------------------------------------------------------------------- 

//Dul 1
//                  Kontejner  Pylik      Vstup       Start       Kontejner   Vyhen       Ress        Nespojitost Vyhaz  Forensic Krumpy Hiding Navstevnici 
//exec StartMining2 0x40000CB9 0x402110BB "1367.2731" "1388.2704" "1369.2743" "1379.2705" "1376.2981" "1373.2712" "" 700 6 false false

//--------------------------------------------------------------------------------------------- 


//Dul 2 - na N
//                  Kontejner  Pylik      Vstup     Start     Kontejner Vyhen     Ress       Nespojitost Vyhaz  Forensic Krumpy Hiding Navstevnici
//exec StartMining2 0x402CB29D 0x4007B7EC "2472.67" "2484.42" "2475.54|2475.63|2472.67|2469.91" "2475.54|2460.51" "2627.165" "2471.58|2462.48" "" 700 6 false false


//Dul 2 - na W
//                  Kontejner  Pylik      Vstup     Start     Kontejner Vyhen     Ress       Nespojitost Vyhaz  Forensic Krumpy Hiding Navstevnici
//exec StartMining2 0x402CB29D 0x4007B7EC "2441.94" "2439.97" "2427.93|2444.94|2469.91" "2427.92" "2627.165" "2429.100" "" 700 6 false false "SN"

//Homare
//c22|333.3368|378.3315|394.3293|425.3274|480.3271|486.3280|501.3279|502.3269

// ,exec startminingmultiple 
//,exec startminingmultiple "c22|333.3368|378.3315|394.3293|425.3274|480.3271|486.3280|501.3279|502.3269;0x402CB29D;;502.3272;525.3245;502.3269|501.3279|486.3280|480.3271|425.3274|394.3293|378.3315|333.3368|332.3419;522.3262;328.3417;554.3266|555.3275|553.3284;Iron;700;6;false;false;EW;Bank;false;548.3275;false"  "c5|2471.68|2472.63|2472.62;0x402CB29D;0x4007B7EC;2472.67;2484.42;2475.54|2475.63|2472.67|2469.91;2475.54|2460.51;2627.165;2471.58|2462.48;Iron;700;6;false;false;EW;None;true;;false" "c5|2441.94;0x402CB29D;0x4007B7EC;2441.94;2439.97;2427.93|2444.94|2469.91;2427.92;2627.165;2429.100;Iron;700;6;false;false;SN;None;true;;false"  

//,exec startminingmultiple "c5|2471.68|2472.63|2472.62;0x402CB29D;0x4007B7EC;2472.67;2484.42;2475.54|2475.63|2472.67|2469.91;2475.54|2460.51;2627.165;2471.58|2462.48;Iron;700;6;false;false;EW;None;true;;false" "c5|2441.94;0x402CB29D;0x4007B7EC;2441.94;2439.97;2427.93|2444.94|2469.91;2427.92;2627.165;2429.100;Iron;700;6;false;false;SN;None;true;;false"  

//,exec startminingmultiple "c5|2471.68|2472.63|2472.62;0x402CB29D;0x4007B7EC;2472.67;2484.42;2475.54|2475.63|2472.67|2469.91;2475.54|2460.51;2627.165;2471.58|2462.48;Iron;700;6;false;false;EW;None;true;;false" "c5|2441.94;0x402CB29D;0x4007B7EC;2441.94;2439.97;2427.93|2444.94|2469.91;2427.92;2627.165;2429.100;Iron;700;6;false;false;SN;None;true;;false"  



//"c5|2471.68|2472.63|2472.62;0x402CB29D;0x4007B7EC;2472.67;2484.42;2475.54|2475.63|2472.67|2469.91;2475.54|2460.51;2627.165;2471.58|2462.48;Iron;700;6;false;false;EW;None;true;;false" 
//"c5|2441.94;0x402CB29D;0x4007B7EC;2441.94;2439.97;2427.93|2444.94|2469.91;2427.92;2627.165;2429.100;;700;6;false;false;SN" 
//"c22|333.3368|378.3315|394.3293|425.3274|480.3271|486.3280|501.3279|502.3269;0x402CB29D;;502.3272;525.3245;502.3269|501.3279|486.3280|480.3271|425.3274|394.3293|378.3315|333.3368|332.3419;522.3262;328.3417;554.3266|555.3275|553.3284;Iron,Copper,Bronze,Silver,Shadow,Rose,Golden,Verite,Valorite;700;6;false;false;EW;Bank;false;548.3275"
//"c22|333.3368|378.3315|394.3293|425.3274|480.3271|486.3280|501.3279|502.3269;0x402CB29D;;502.3272;525.3245;502.3269|501.3279|486.3280|480.3271|425.3274|394.3293|378.3315|333.3368|332.3419;522.3262;328.3417;554.3266|555.3275|553.3284;Iron,Copper;700;6;false;false;EW;Bank;false;548.3275;false"

//"c22|333.3368|378.3315|394.3293|425.3274|480.3271|486.3280|501.3279|502.3269;0x402CB29D;;502.3272;525.3245;502.3269|501.3279|486.3280|480.3271|425.3274|394.3293|378.3315|333.3368|332.3419;522.3262;328.3417;553.3255|555.3263|555.3264|555.3265.20;Iron,Copper,Bronze,Silver,Shadow,Rose,Golden,Verite,Valorite;700;6;false;false;EW;Bank;false;548.3275"

//--------------------------------------------------------------------------------------------- 

//  ,exec startminingmultiple "c22|333.3368|378.3315|394.3293|425.3274|480.3271|486.3280|501.3279|502.3269;0x402CB29D;;502.3272;525.3245;502.3269|501.3279|486.3280|480.3271|425.3274|394.3293|378.3315|333.3368|332.3419;522.3262;328.3417;553.3255|555.3263|555.3264|555.3265.20;Iron,Copper;700;6;false;false;EW;Bank;false;548.3275"