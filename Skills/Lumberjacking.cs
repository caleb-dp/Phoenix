//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Phoenix;
//using Phoenix.WorldData;
//using Phoenix.Communication;
//using Caleb.Library.CAL.Business;
//using CalExtension;
//using System.Data;
//using Caleb.Library;
//using CalExtension.UOExtensions;

//namespace DP_Scripts
//{
//  public class Lumberjacking
//  {
//    public static UOPosition MinocBank = new UOPosition(2494, 547, 0);
//    public static UOPosition BaracekGS = new UOPosition(1370, 2743, 0);
//    public static Graphic HatchetGraphic = 0x0F43;
//    public static Graphic LogGraphic = 0x1BDD;
//    public static List<Graphic> TreeGraphic { get { return new List<Graphic>() { 0x0CD0, 0x0CE0, 0x0CE6, 0x0CDD, 0x0C9E, 0x0CD3, 0x0CD8, 0x0CCD, 0x0CDA }; } }
//    //---------------------------------------------------------------------------------------------

//    public Lumberjacking()
//    {
//      this.dennyLogList = new List<string>();
//    }

//    protected UOPositionCollection positionHistory;
//    public UOPositionCollection PositionHistory
//    {
//      get
//      {
//        if (this.positionHistory == null)
//          this.positionHistory = new UOPositionCollection();
//        return this.positionHistory;
//      }
//      set { this.positionHistory = value; }
//    }

//    protected UOPositionCollection treePositions;
//    public UOPositionCollection TreePositions
//    {
//      get
//      {
//        if (this.treePositions == null)
//        {
//          this.treePositions = new UOPositionCollection();
//          //Minoc default na test
//          this.treePositions.Add(new UOPosition(2500, 567, 0));
//          this.treePositions.Add(new UOPosition(2504, 567, 0));
//          this.treePositions.Add(new UOPosition(2508, 567, 0));
//          this.treePositions.Add(new UOPosition(2511, 567, 0));
//          this.treePositions.Add(new UOPosition(2512, 567, 0));
//          this.treePositions.Add(new UOPosition(2508, 573, 0));
//          this.treePositions.Add(new UOPosition(2504, 573, 0));
//          this.treePositions.Add(new UOPosition(2496, 570, 0));
//          this.treePositions.Add(new UOPosition(2496, 573, 0));
//          this.treePositions.Add(new UOPosition(2500, 576, 0));
//          this.treePositions.Add(new UOPosition(2496, 579, 0));
//          this.treePositions.Add(new UOPosition(2500, 579, 0));
//          this.treePositions.Add(new UOPosition(2504, 579, 0));
//          this.treePositions.Add(new UOPosition(2508, 576, 0));
//          this.treePositions.Add(new UOPosition(2512, 576, 0));
//          this.treePositions.Add(new UOPosition(2508, 579, 0));
//          this.treePositions.Add(new UOPosition(2496, 582, 0));
//          this.treePositions.Add(new UOPosition(2496, 585, 0));
//          this.treePositions.Add(new UOPosition(2500, 588, 0));
//          this.treePositions.Add(new UOPosition(2504, 588, 0));
//          this.treePositions.Add(new UOPosition(2512, 582, 0));
//        }
//        return this.treePositions;
//      }
//      set { this.treePositions = value; }
//    }

//    protected UOPosition actualTreePosition;
//    public UOPosition ActualTreePosition
//    {
//      get { return this.actualTreePosition; }
//      set { this.actualTreePosition = value; }
//    }

//    protected List<StaticTarget> trees;
//    public List<StaticTarget> Trees
//    {
//      get
//      {
//        if (this.trees == null)
//        {
//          this.trees = new List<StaticTarget>();
//          //this.trees.Add(new StaticTarget(Serial.Invalid, 2500, 567, 0, 0x0CCE));
//          //this.trees.Add(new StaticTarget(Serial.Invalid, 2504, 567, 0, 0x0CDB));
//          //this.trees.Add(new StaticTarget(Serial.Invalid, 2508, 567, 0, 0x0CE4));
//          //this.trees.Add(new StaticTarget(Serial.Invalid, 2511, 567, 0, 0x0C9E));
//          //this.trees.Add(new StaticTarget(Serial.Invalid, 2512, 567, 0, 0x0CD9));
//          //this.trees.Add(new StaticTarget(Serial.Invalid, 2508, 573, 0, 0x0CD9));
//          //this.trees.Add(new StaticTarget(Serial.Invalid, 2504, 573, 0, 0x0CCE));
//          //this.trees.Add(new StaticTarget(Serial.Invalid, 2496, 570, 0, 0x0CDB));
//          //this.trees.Add(new StaticTarget(Serial.Invalid, 2496, 573, 0, 0x0CDA));
//          //this.trees.Add(new StaticTarget(Serial.Invalid, 2500, 576, 0, 0x0CE7));
//          //this.trees.Add(new StaticTarget(Serial.Invalid, 2496, 579, 0, 0x0CCE));
//          //this.trees.Add(new StaticTarget(Serial.Invalid, 2500, 579, 0, 0x0CE4));
//          //this.trees.Add(new StaticTarget(Serial.Invalid, 2504, 579, 0, 0x0CD1));
//          //this.trees.Add(new StaticTarget(Serial.Invalid, 2508, 576, 0, 0x0CD7));
//          //this.trees.Add(new StaticTarget(Serial.Invalid, 2512, 576, 0, 0x0CD7));
//          //this.trees.Add(new StaticTarget(Serial.Invalid, 2508, 579, 0, 0x0CCE));
//          //this.trees.Add(new StaticTarget(Serial.Invalid, 2496, 582, 0, 0x0CE4));
//          //this.trees.Add(new StaticTarget(Serial.Invalid, 2496, 585, 0, 0x0CE1));
//          //this.trees.Add(new StaticTarget(Serial.Invalid, 2500, 588, 0, 0x0CD9));
//          //this.trees.Add(new StaticTarget(Serial.Invalid, 2504, 588, 0, 0x0CDD));
//          //this.trees.Add(new StaticTarget(Serial.Invalid, 2512, 582, 0, 0x0CDB));

//          this.trees.Add(new StaticTarget(Serial.Invalid, 2501, 567, 0, 0x0CCE));
//          this.trees.Add(new StaticTarget(Serial.Invalid, 2500, 567, 0, 0x0CCE));
//          this.trees.Add(new StaticTarget(Serial.Invalid, 2504, 567, 0, 0x0CCE));
//          this.trees.Add(new StaticTarget(Serial.Invalid, 2508, 567, 0, 0x0CCE));
//          this.trees.Add(new StaticTarget(Serial.Invalid, 2511, 567, 0, 0x0CCE));
//          this.trees.Add(new StaticTarget(Serial.Invalid, 2512, 567, 0, 0x0CCE));
//          this.trees.Add(new StaticTarget(Serial.Invalid, 2508, 573, 0, 0x0CCE));
//          this.trees.Add(new StaticTarget(Serial.Invalid, 2504, 573, 0, 0x0CCE));
//          this.trees.Add(new StaticTarget(Serial.Invalid, 2496, 570, 0, 0x0CCE));
//          this.trees.Add(new StaticTarget(Serial.Invalid, 2496, 573, 0, 0x0CCE));
//          this.trees.Add(new StaticTarget(Serial.Invalid, 2500, 576, 0, 0x0CCE));
//          this.trees.Add(new StaticTarget(Serial.Invalid, 2496, 579, 0, 0x0CCE));
//          this.trees.Add(new StaticTarget(Serial.Invalid, 2500, 579, 0, 0x0CCE));
//          this.trees.Add(new StaticTarget(Serial.Invalid, 2504, 579, 0, 0x0CCE));
//          this.trees.Add(new StaticTarget(Serial.Invalid, 2508, 576, 0, 0x0CCE));
//          this.trees.Add(new StaticTarget(Serial.Invalid, 2512, 576, 0, 0x0CCE));
//          this.trees.Add(new StaticTarget(Serial.Invalid, 2508, 579, 0, 0x0CCE));
//          this.trees.Add(new StaticTarget(Serial.Invalid, 2496, 582, 0, 0x0CCE));
//          this.trees.Add(new StaticTarget(Serial.Invalid, 2496, 585, 0, 0x0CCE));
//          this.trees.Add(new StaticTarget(Serial.Invalid, 2500, 588, 0, 0x0CCE));
//          this.trees.Add(new StaticTarget(Serial.Invalid, 2504, 588, 0, 0x0CCE));
//          this.trees.Add(new StaticTarget(Serial.Invalid, 2512, 582, 0, 0x0CCE));

//          //this.trees.Add(new StaticTarget(Serial.Invalid, 2501, 567, 0, Graphic.Invariant));
//          //this.trees.Add(new StaticTarget(Serial.Invalid, 2500, 567, 0, Graphic.Invariant));
//          //this.trees.Add(new StaticTarget(Serial.Invalid, 2504, 567, 0, Graphic.Invariant));
//          //this.trees.Add(new StaticTarget(Serial.Invalid, 2508, 567, 0, Graphic.Invariant));
//          //this.trees.Add(new StaticTarget(Serial.Invalid, 2511, 567, 0, Graphic.Invariant));
//          //this.trees.Add(new StaticTarget(Serial.Invalid, 2512, 567, 0, Graphic.Invariant));
//          //this.trees.Add(new StaticTarget(Serial.Invalid, 2508, 573, 0, Graphic.Invariant));
//          //this.trees.Add(new StaticTarget(Serial.Invalid, 2504, 573, 0, Graphic.Invariant));
//          //this.trees.Add(new StaticTarget(Serial.Invalid, 2496, 570, 0, Graphic.Invariant));
//          //this.trees.Add(new StaticTarget(Serial.Invalid, 2496, 573, 0, Graphic.Invariant));
//          //this.trees.Add(new StaticTarget(Serial.Invalid, 2500, 576, 0, Graphic.Invariant));
//          //this.trees.Add(new StaticTarget(Serial.Invalid, 2496, 579, 0, Graphic.Invariant));
//          //this.trees.Add(new StaticTarget(Serial.Invalid, 2500, 579, 0, Graphic.Invariant));
//          //this.trees.Add(new StaticTarget(Serial.Invalid, 2504, 579, 0, Graphic.Invariant));
//          //this.trees.Add(new StaticTarget(Serial.Invalid, 2508, 576, 0, Graphic.Invariant));
//          //this.trees.Add(new StaticTarget(Serial.Invalid, 2512, 576, 0, Graphic.Invariant));
//          //this.trees.Add(new StaticTarget(Serial.Invalid, 2508, 579, 0, Graphic.Invariant));
//          //this.trees.Add(new StaticTarget(Serial.Invalid, 2496, 582, 0, Graphic.Invariant));
//          //this.trees.Add(new StaticTarget(Serial.Invalid, 2496, 585, 0, Graphic.Invariant));
//          //this.trees.Add(new StaticTarget(Serial.Invalid, 2500, 588, 0, Graphic.Invariant));
//          //this.trees.Add(new StaticTarget(Serial.Invalid, 2504, 588, 0, Graphic.Invariant));
//          //this.trees.Add(new StaticTarget(Serial.Invalid, 2512, 582, 0, Graphic.Invariant));

//        }
//        return this.trees;
//      }
//      set { this.trees = value; }
//    }

//    protected StaticTarget tree;
//    public StaticTarget Tree
//    {
//      get { return this.tree; }
//      set { this.tree = value; }
//    }

//    protected IUOPosition lastposition;
//    public IUOPosition LastPosition
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

//    protected int weightOffset = 50;
//    public int WeightOffset
//    {
//      get { return Math.Abs(this.weightOffset); }
//      set { this.weightOffset = value; }
//    }

//    public bool WeightLimitReached
//    {
//      get { return World.Player.Weight > (World.Player.MaxWeight - this.WeightOffset); }
//    }

//    //protected Robot robot;
//    //public Robot Robot
//    //{
//    //  get
//    //  {
//    //    if (this.robot == null)
//    //      this.robot = new Robot();
//    //    return this.robot;
//    //  }
//    //}

//    public UOItem Hatchet
//    {
//      get { return GetHatchet(); }
//    }

//    public static UOItem GetHatchet()
//    {
//      UOItem krumpac = new UOItem(Serial.Invalid);
//      if (!(krumpac = World.Player.Layers.FindType(HatchetGraphic)).Exist)
//      {
//        World.Player.Backpack.AllItems.FindType(HatchetGraphic);
//      }
//      if (!krumpac.Exist) UO.Print("Nemas sekyru!");
//      return krumpac;
//    }

//    public delegate bool LumberBagFull();

//    protected LumberBagFull lumberFullMethod;
//    protected LumberBagFull LumberFullMethod
//    {
//      get { return this.lumberFullMethod; }
//      set { this.lumberFullMethod = value; }
//    }

//    public bool SingleSek()
//    {
//      return this.SingleSek(this.Tree);
//    }

//    public ushort SearchSuqareSize = 600;

//    //---------------------------------------------------------------------------------------------

//    public bool SingleSek(StaticTarget tree)
//    {
//      if (!Hatchet.Exist || tree == null)
//        return false;

//      bool result = true;

//      if (Game.CurrentGame.WorldSave())
//      {
//        Game.Wait();
//        this.Hatchet.Move(1, World.Player.Backpack);
//        Game.Wait();
//        this.Hatchet.Click();
//        Game.Wait();
//      }

//      UO.DeleteJournal();

//      JournalEventWaiter jew = new JournalEventWaiter(true, "akce skoncila");
//      Hatchet.Use();
//      UO.WaitTargetTile(tree.X, tree.Y, tree.Z, tree.Graphic);
//      jew.Wait(6000 + LatencyMeasurement.CurrentLatency);

//      if (Journal.Contains(true, "There are no logs left here to chop", "You can't think of a way to use that item", "That's too far away to chop", "Try chopping a tree"))
//      {
//        result = false;
//      }


//      foreach (UOItem log in World.Ground)
//      {
//        UO.DeleteJournal();
//        if (log.Distance < 3 && log.Graphic == LogGraphic)
//        {
//          bool leave = false;
//          if (log.Color == 0x0000 && Array.IndexOf(dennyLogList.ToArray(), "log") > -1)
//            leave = true;
//          else if (log.Color == 0x06D3 && Array.IndexOf(dennyLogList.ToArray(), "fir") > -1)
//            leave = true;
//          else if (log.Color == 0x0979 && Array.IndexOf(dennyLogList.ToArray(), "plum") > -1)
//            leave = true;
//          else if (log.Color == 0x0972 && Array.IndexOf(dennyLogList.ToArray(), "lime") > -1)
//            leave = true;
//          else if (log.Color == 0x05A6 && Array.IndexOf(dennyLogList.ToArray(), "oak") > -1)
//            leave = true;
//          else if (log.Color == 0x0522 && Array.IndexOf(dennyLogList.ToArray(), "ebony") > -1)//TODO ebony
//            leave = true;

//          if (!leave)
//          {

//            this.allTimeStatistic[log.Color]++;
//            log.Move(10, World.Player.Backpack);

//            if (Journal.WaitForText(true, 350, "You put the logs at your feet. It is too heavy.."))
//            {
//              break;
//            }
//          }
//        }
//      }


//      return result;
//    }

//    //---------------------------------------------------------------------------------------------

//    public void RemoveTree()
//    {
//      StaticTarget tree = UIManager.Target();
//      UO.Print(tree.Graphic);

//      if (tree.Graphic != 0)
//      {
//        UOPosition pos = new UOPosition(tree.X, tree.Y, (ushort)tree.Z);
//        if (pos.LoadByPosition())
//        {
//          if (pos.Remove())
//            UO.Print("Strom odebran");
//          else
//            UO.Print("Strom se nepodarilo odebrat");
//        }
//        else
//          UO.Print("Strom neni v DB");
//      }
//    }

//    //---------------------------------------------------------------------------------------------

//    public void AddTree()
//    {
//      DataTable dt = Cal.Engine.GetOtSetDataTable(new UOPosition().DbViewNameForLoad);
     
//      if (dt.Rows.Count > 0)
//        Game.CurrentGame.Messages.Add("Tree count: " + dt.Select("IsTree=1").Length);

//      StaticTarget tree = UIManager.Target();
//      UO.Print(tree.Graphic);

//      if (tree.Graphic != 0)
//      {
//        UOPosition pos = new UOPosition(tree.X, tree.Y, (ushort)tree.Z);
//        pos.EnsureLoadByPosition();
//        pos.IsTree = true;
//        pos.TypeChecked = true;
//        pos.Stepable = false;
//        if (pos.Save())
//          Game.CurrentGame.Messages.Add("Tree add: " + pos.ToString());
//      }
//    }

//    //---------------------------------------------------------------------------------------------

//    public bool CheckIsTree(StaticTarget tree)
//    {
//      if (!Hatchet.Exist || tree == null)
//        return false;
//      JournalEventWaiter jew = new JournalEventWaiter(true, "...akce skoncila");
//      Hatchet.Use();
//      UO.WaitTargetTile(tree.X, tree.Y, tree.Z, tree.Graphic);
//      jew.Wait(8000 + Core.Latency);
//      if (Journal.Contains(true, "There are no logs left here to chop", "You hack at the tree for a while, but fail to produce any useable wood", "You put the logs in your pack"))
//      {
//        Journal.Clear();
//        return true;
//      }
//      return false;
//    }

//    //---------------------------------------------------------------------------------------------

//    protected MovementDirection currentLumberDirection = MovementDirection.DownRight;
//    protected int maxRowIndex = 10;
//    protected int currentRowIndex = 0;

//    //---------------------------------------------------------------------------------------------

//    protected void SwitchCurrentDirect()
//    {
//      if (currentLumberDirection == MovementDirection.DownRight)
//        currentLumberDirection = MovementDirection.UpLeft;
//      else
//        currentLumberDirection = MovementDirection.DownRight;
//    }

//    //---------------------------------------------------------------------------------------------

//    protected Dictionary<UOColor, int> allTimeStatistic;


//    //---------------------------------------------------------------------------------------------

//    protected int runsCount = 0;

//    public ushort? MaxE;
//    public ushort? MaxS;
//    public ushort? MaxW;
//    public ushort? MaxN;

//    //---------------------------------------------------------------------------------------------

//    private List<string> dennyLogList;

//    public UOPosition ContainerPosition;
//    public UOPosition StartPosition;
//    public Serial ContainerSerial;// securkaubaracku
//    public void UniversalLumberjackingRecursive()
//    {
//      runsCount++;
//      if (allTimeStatistic == null)
//      {
//        allTimeStatistic = new Dictionary<UOColor, int>();
//        allTimeStatistic.Add(0x0000, 0);//log
//        allTimeStatistic.Add(0x0979, 0);//Plum
//        allTimeStatistic.Add(0x06D3, 0);//Fir
//        allTimeStatistic.Add(0x0972, 0);//Lime
//        allTimeStatistic.Add(0x05A6, 0);//Oak
//        allTimeStatistic.Add(0x0522, 0);//Ebony
//      }

//      Game.CurrentGame.Mode = GameMode.Working;
//      int currentX = new Robot().ActualPosition.X.GetValueOrDefault();
//      int currentY = new Robot().ActualPosition.Y.GetValueOrDefault();
//      int minX = 0;
//      int minY = 0;
//      int maxX = 0;
//      int maxY = 0;

//      minX = (currentX - this.SearchSuqareSize < 0 ? 0 : currentX - this.SearchSuqareSize);
//      maxX = (currentX + this.SearchSuqareSize > 6000 ? 6000 : currentX + this.SearchSuqareSize);
//      minY = (currentY - this.SearchSuqareSize < 0 ? 0 : currentY - this.SearchSuqareSize);
//      maxY = (currentY + this.SearchSuqareSize > 6000 ? 6000 : currentY + this.SearchSuqareSize);

//      if (this.MaxE.HasValue)
//        maxX = Math.Min(maxX, MaxE.Value);
//      if (this.MaxS.HasValue)
//        maxY = Math.Min(maxY, MaxS.Value);
//      if (this.MaxW.HasValue)
//        minX = Math.Max(minX, MaxW.Value);
//      if (this.MaxN.HasValue)
//        minY = Math.Max(minY, MaxN.Value);

//      UOPositionCollection allTrees = new UOPositionCollection();
//      allTrees.Load(String.Format("IsTree=1 AND X>={0} AND X<={1} AND Y>={2} AND Y<={3}", minX, maxX, minY, maxY));
//      allTrees = allTrees.SortByOptimalTrack(new Robot().ActualPosition);

//      Game.CurrentGame.Messages.Add("Lumber start :" + allTrees.Count);
//      int counter = 0;

//      foreach (UOPosition position in allTrees)
//      {
//        Robot r = new Robot();
//        r.EnableLog = true;

//        if (UO.Dead)
//          return;

//        if (World.Player.Backpack.AllItems.FindType(LogGraphic, 0x0979).Amount >= 8
//                || World.Player.Backpack.AllItems.FindType(LogGraphic, 0x06D3).Amount >= 4
//                || World.Player.Backpack.AllItems.FindType(LogGraphic, 0x0972).Amount >= 3
//                || World.Player.Backpack.AllItems.FindType(LogGraphic, 0x05A6).Amount >= 2
//                || World.Player.Backpack.AllItems.FindType(LogGraphic, 0x0522).Amount >= 1
//          /*|| counter % (allTrees.Count / 2) == 0*/

//          )
//          BezVylozit();

//        counter++;
//        Game.CurrentGame.Messages.Add("Pozice:" + counter + " / " + allTrees.Count + " [" + position.ToString() + "]");


//        if (!this.PositionHistory.Contains(position))
//          this.PositionHistory.Add(position);
//        else
//          continue;

//        if (r.GoTo(position, 1, 100))
//        {
//          this.LastPosition = r.ActualPosition;

//          StaticTarget target = new StaticTarget(Serial.Invalid, position.X.GetValueOrDefault(), position.Y.GetValueOrDefault(), (sbyte)position.Z.GetValueOrDefault(), TreeGraphic[0]);

//          //Game.CurrentGame.CurrentPlayer.Messages.Add("Sek :" + position.ToString());

//          while (this.SingleSek(target))
//          {
//          }

//          if (dennyLogList.Count > 0)
//          {
//            foreach (UOItem item in Game.CurrentGame.CurrentPlayer.Player.Backpack.AllItems)
//            {
//              if (item.Graphic == LogGraphic)
//              {
//                bool drop = false;
//                if (item.Color == 0x0000 && Array.IndexOf(dennyLogList.ToArray(), "log") > -1)
//                  drop = true;
//                else if (item.Color == 0x06D3 && Array.IndexOf(dennyLogList.ToArray(), "fir") > -1)
//                  drop = true;
//                else if (item.Color == 0x0979 && Array.IndexOf(dennyLogList.ToArray(), "plum") > -1)
//                  drop = true;
//                else if (item.Color == 0x0972 && Array.IndexOf(dennyLogList.ToArray(), "lime") > -1)
//                  drop = true;
//                else if (item.Color == 0x05A6 && Array.IndexOf(dennyLogList.ToArray(), "oak") > -1)
//                  drop = true;
//                else if (item.Color == 0x0522 && Array.IndexOf(dennyLogList.ToArray(), "ebony") > -1)//TODO ebony
//                  drop = true;

//                if (drop)
//                {
//                  item.DropHere();
//                  Game.Wait(250);
//                }
//              }
//            }
//          }

//          if (this.WeightLimitReached || !Hatchet.Exist)
//          {
//            BezVylozit();
//          }
//        }
//        else
//        {
//          this.LastPosition = r.ActualPosition;
//          Game.CurrentGame.Messages.Add("Lumber nepodarilo se dojit na pozici:" + position.ToString());
//          continue;
//        }
//      }

//      Game.CurrentGame.Messages.Add(runsCount + ". kolo dokonceno. Zatim:");
//      this.PrintLogy();
//      this.PositionHistory.Clear();

//      if (allTrees.Count > 0)
//      {
//        this.BezVylozit();

//        if (new Robot().GoTo(this.StartPosition))
//        {
//          UniversalLumberjackingRecursive();
//        }
//      }
//    }

//    //---------------------------------------------------------------------------------------------

//    protected void PrintCurrentLogy()
//    {
//      Game.CurrentGame.Messages.Add(String.Format("Plum: {0}", World.Player.Backpack.AllItems.FindType(LogGraphic, 0x0979).Amount));
//      Game.CurrentGame.Messages.Add(String.Format("Fir: {0}", World.Player.Backpack.AllItems.FindType(LogGraphic, 0x06D3).Amount));
//      Game.CurrentGame.Messages.Add(String.Format("Lime: {0}", World.Player.Backpack.AllItems.FindType(LogGraphic, 0x0972).Amount));
//      Game.CurrentGame.Messages.Add(String.Format("Oak: {0}", World.Player.Backpack.AllItems.FindType(LogGraphic, 0x05A6).Amount));
//      Game.CurrentGame.Messages.Add(String.Format("Ebony: {0}", World.Player.Backpack.AllItems.FindType(LogGraphic, 0x0522).Amount));
//    }

//    //---------------------------------------------------------------------------------------------

//    protected void PrintLogy()
//    {
//      Game.CurrentGame.Messages.Add(String.Format("Plum: {0}", this.allTimeStatistic[0x0979]));
//      Game.CurrentGame.Messages.Add(String.Format("Fir: {0}", this.allTimeStatistic[0x06D3]));
//      Game.CurrentGame.Messages.Add(String.Format("Lime: {0}", this.allTimeStatistic[0x0972]));
//      Game.CurrentGame.Messages.Add(String.Format("Oak: {0}", this.allTimeStatistic[0x05A6]));
//      Game.CurrentGame.Messages.Add(String.Format("Ebony: {0}", this.allTimeStatistic[0x0522]));
//    }

//    //---------------------------------------------------------------------------------------------
//    public bool Vykladat = true;
//    protected void BezVylozit()
//    {
//      if (Vykladat)
//      {
//        Robot r = new Robot();
//        Game.CurrentGame.Messages.Add("Jdu se vylozit:");
//        this.PrintCurrentLogy();
//        r.GoTo(this.StartPosition); 

//        if (r.GoTo(ContainerPosition, 1))
//        {
//          UOItem container = new UOItem(ContainerSerial);//0x40000CB9);
//          if (container.Exist)
//          {
//            UOItem chest = new UOItem(container);
//            chest.Use();
//            Game.Wait();

//            if (!chest.Exist)
//            {
//              UO.Print("Neni bedna na odhazovani");
//            }
//          }


//          UOItem log = new UOItem(Serial.Invalid);
//          while ((log = UO.Backpack.Items.FindType(LogGraphic)).Exist)
//          {
//            log.Move(150, container);
//            Game.Wait();
//          }

//          Game.Wait();
//          bool opened = false;
//          while (World.Player.Backpack.AllItems.Count(HatchetGraphic) < 6)
//          {
//            if (!opened)
//            {
//              ItemHelper.OpenContainerRecursive(container);
//              Game.Wait();
//              opened = true;
//            }
//            if (container.AllItems.FindType(HatchetGraphic).Exist)
//            {
//              container.AllItems.FindType(HatchetGraphic).Move(1, World.Player.Backpack);
//              Game.Wait();
//            }
//            else
//              break;
//          }

//          UO.Wait(2500);
//        }

//        r.GoTo(this.StartPosition);
//      }

//    }

//    //---------------------------------------------------------------------------------------------

//    protected bool DoMinocBankyAZpet()//predelano na domecek u dolu
//    {
//      Robot r = new Robot();

//      if (r.GoTo(BaracekGS, 1))
//      {
//        UOItem container = new UOItem(0x40000CB9);
//        if (container.Exist)
//        {
//          UOItem chest = new UOItem(container);
//          chest.Use();
//          UO.Wait(500);

//          if (!chest.Exist)
//          {
//            UO.Print("Neni bedna na odhazovani");
//            return false;
//          }
//        }


//        UOItem log = new UOItem(Serial.Invalid);
//        while ((log = UO.Backpack.Items.FindType(LogGraphic)).Exist)
//        {
//          log.Move(150, container);
//          UO.Wait(500);
//        }
//        UO.Wait(2500);

//        if (r.GoTo(this.LastPosition))
//          return true;

//        return false;
//      }
//      return false;
//    }
//    //---------------------------------------------------------------------------------------------


//    [Executable]
//    [BlockMultipleExecutions]
//    public static void StartUniversalLumber(ushort containerX, ushort containerY, Serial containerSerial)
//    {
//      StartUniversalLumber(containerX, containerY, containerSerial, null);
//    }

//    //---------------------------------------------------------------------------------------------

//    [Executable]
//    [BlockMultipleExecutions]
//    public static void StartUniversalLumber(ushort containerX, ushort containerY, Serial containerSerial, string denyorenames)
//    {
//      StartUniversalLumber(containerX, containerY, containerSerial, denyorenames, 150);

//    }

//    //---------------------------------------------------------------------------------------------
//    [Executable]
//    [BlockMultipleExecutions]
//    public static void StartUniversalLumber(ushort containerX, ushort containerY, Serial containerSerial, string denyorenames, ushort squareSize)
//    {
//      StartUniversalLumber(containerX, containerY, containerSerial, denyorenames, 150, true, 0, 0, 0, 0);
//    }

//    //---------------------------------------------------------------------------------------------

//    [Executable]
//    [BlockMultipleExecutions]
//    public static void StartUniversalLumber(ushort containerX, ushort containerY, Serial containerSerial, string denyorenames, ushort squareSize, bool vykladat, ushort maxE, ushort maxS, ushort maxW, ushort maxN)
//    {
//      Lumberjacking lumber = new Lumberjacking();
//      lumber.ContainerPosition = new UOPosition(containerX, containerY, 0);
//      lumber.StartPosition = new UOPosition(World.Player.X, World.Player.Y, (ushort)World.Player.Z);//; new Robot().ActualPosition.Clone();
//      lumber.ContainerSerial = containerSerial;
//      lumber.SearchSuqareSize = squareSize;
//      lumber.Vykladat = vykladat;

//      if (maxE > 0)
//        lumber.MaxE = maxE;
//      if (maxS > 0)
//        lumber.MaxS = maxS;
//      if (maxW > 0)
//        lumber.MaxW = maxW;
//      if (maxN > 0)
//        lumber.MaxN = maxN;


//      if (!String.IsNullOrEmpty(denyorenames))
//      {
//        denyorenames = denyorenames.ToLower();
//        lumber.dennyLogList.AddRange(denyorenames.Split(','));
//      }

//      lumber.UniversalLumberjackingRecursive();


//    }

//    //---------------------------------------------------------------------------------------------

//    [Executable]
//    [BlockMultipleExecutions]
//    public static void AddTreePosition()
//    {
//      Lumberjacking lumber = new Lumberjacking();
//      lumber.AddTree();
//    }

//    //---------------------------------------------------------------------------------------------

//    [Executable]
//    [BlockMultipleExecutions]
//    public static void RemoveTreePosition()
//    {
//      Lumberjacking lumber = new Lumberjacking();
//      lumber.RemoveTree();
//    }
//  }
//}
