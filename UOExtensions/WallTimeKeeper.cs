using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;
using System.Threading;

namespace CalExtension.UOExtensions
{
  //---------------------------------------------------------------------------------------------

  [RuntimeObject]
  public class WallTimeKeeper
  {
    public static Graphic VAL_WoS = 0x0080;
    public static Graphic VAL_EF1 = 0x3956;
    public static Graphic VAL_EF2 = 0x3947;
    public static Graphic VAL_PF1 = 0x3979;

    //---------------------------------------------------------------------------------------------

    private static object SyncRoot;
    public static WallTimeKeeper Current;
    public WallTimeKeeper()
    {
      if (Current == null)
      {
        Current = this;
        SyncRoot = new object();
      }
    }

    //---------------------------------------------------------------------------------------------

    protected static void RemoveUnusedWalls()
    {
      for (int i = Walls.Count - 1; i >= 0; i--)
      {
        WallOfField wof = Walls[i];
        if (wof.ElapsedTime > 180)
        {
          if (Game.Debug)
            Game.PrintMessage("WallRemove " + wof.ID);

          Walls.RemoveAt(i);
          continue;
        }
      }
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void PrintAllWalls()
    {

        //RemoveUnusedWalls();
        for (int i = Walls.Count - 1; i >= 0; i--)
        {
          WallOfField wof = Walls[i];
          wof.PrintTime();
        }
    }

    //---------------------------------------------------------------------------------------------

    private static Dictionary<uint, List<string>> uniqueCounter;
    private static Dictionary<uint, List<string>> UniqueCounter
    {
      get { if (uniqueCounter == null) uniqueCounter = new Dictionary<uint, List<string>>(); return uniqueCounter; }
    }

    //---------------------------------------------------------------------------------------------

    private static List<WallOfField> walls;
    private static List<WallOfField> Walls
    {
      get
      {
        if (walls == null)
          walls = new List<WallOfField>();
        return walls;
      }

    }

    //---------------------------------------------------------------------------------------------

    public static int WallIDCounter = 0;
    public static bool TryFoundAddWall()
    {
      World.FindDistance = 20;
      List<UOItem> itemsAll = World.Ground.Where(i =>
      i.Graphic == VAL_WoS ||
      i.Graphic == VAL_EF1 ||
      i.Graphic == VAL_EF2 //||
      /*i.Graphic == VAL_PF1*/
      ).ToList();


      if (Game.Debug) 
        Game.PrintMessage("TryFoundAddWall " + itemsAll.Count) ;

      bool result = false;


      List<UOItem> items = new List<UOItem>();
      foreach (UOItem allItem in itemsAll)
      {
        string positionKey = allItem.X + "|" + allItem.Y;

        try
        {
          if (UniqueCounter.ContainsKey(allItem.Serial) && !UniqueCounter[allItem.Serial].Contains(positionKey))
          {
            UniqueCounter[allItem.Serial].Add(positionKey);

            if (Game.Debug)
              Game.PrintMessage("Duplicate ID " + allItem.Serial);
          }
          else if (! UniqueCounter.ContainsKey(allItem.Serial))
          {
            UniqueCounter.Add(allItem.Serial, new List<string>() { positionKey });
          }
        }
        catch (Exception ex)
        {
          if (Game.Debug)
            Game.PrintMessage(ex.Message);
        }

        bool isExisting = false;
      //  RemoveUnusedWalls();
        for (int i = Walls.Count - 1; i >= 0; i--)
        {
          WallOfField wof = Walls[i];
          for (int b = 0; b < wof.Bricks.Length; b++)//each (//uint s in wof.Bricks)
          {
            //UOItem brick = new UOItem(s);
            if (wof.BrickExist(b) && allItem.Serial == wof.Bricks[b])
            {
              isExisting = true;
              break;
            }
          }

          if (isExisting)
            break;
        }
        if (!isExisting)
          items.Add(allItem);
      }

      if (Game.Debug)
        Game.PrintMessage("TryFoundAddWall 1 " + items.Count);

      if (items.Count > 0)
      {
        List<ushort> xs = new List<ushort>();
        List<ushort> ys = new List<ushort>();

        foreach (UOItem itm in items)
        {
          if (!xs.Contains(itm.X))
            xs.Add(itm.X);

          if (!ys.Contains(itm.Y))
            ys.Add(itm.Y);

        }

        Dictionary<string, List<UOItem>> posibleWalls = new Dictionary<string, List<UOItem>>();

        foreach (ushort x in xs)
          posibleWalls.Add("X:" + x, items.Where(i => i.X == x /*&& i.Graphic == xItem.Graphic*/).OrderBy(i => i.Y).ToList());

        foreach (ushort y in ys)
          posibleWalls.Add("Y:" + y, items.Where(i => i.Y == y /*&& i.Graphic == yItem.Graphic*/).OrderBy(i => i.X).ToList());

        //List<UOItem> snItems = items.Where(i => i.X == items[0].X && i.Graphic == items[0].Graphic).OrderBy(i => i.Y).ToList();
        //List<UOItem> weItems = items.Where(i => i.Y == items[0].Y && i.Graphic == items[0].Graphic).OrderBy(i => i.X).ToList();

        if (Game.Debug)
          Game.PrintMessage("TryFoundAddWall 3 " + posibleWalls.Count);

        foreach (KeyValuePair<string, List<UOItem>> kvp in posibleWalls)
        {
          try
          {
            WallOfField wof = null;
            WallType wt = kvp.Key.StartsWith("X") ? WallType.SN : WallType.WE;

            if (Game.Debug)
              Game.PrintMessage("TryFoundAddWall 4 [" + kvp.Key + "] " + kvp.Value.Count);

            if (kvp.Value.Count > 2)
            {
              wof = new WallOfField(++WallIDCounter);
              wof.Type = wt;

              for (int i = 0; i < kvp.Value.Count; i++)
              {
                wof.Bricks[i] = kvp.Value[i];
                wof.BricksPositions[i] = kvp.Value[i].X + "." + kvp.Value[i].Y;
              }
            }

            if (wof != null)
            {
              Walls.Add(wof);
              wof.PrintInit();
            }
          }
          catch (Exception ex)
          {
            if (Game.Debug)
              Game.PrintMessage("Ex: " + ex);
          }
        }
      }

      return result;
    }

    //---------------------------------------------------------------------------------------------
  }

  public enum WallType
  {
    SN = 1,
    WE = 2
  }

  //---------------------------------------------------------------------------------------------
  
  public class WallOfField
  {
    //---------------------------------------------------------------------------------------------

    public WallOfField(int id)
    {
      ID = id;
      CreationTime = DateTime.Now;
      Bricks = new uint[7];
      BricksPositions = new string[7];
      t = new System.Timers.Timer();
      t.Elapsed += T_Elapsed;
      t.Interval = 3000;
      t.Start();

      for (int i = 0; i < Bricks.Length; i++)
      {
        Bricks[i] = Serial.Invalid;
        BricksPositions[i] = "-1.-1";

      }
    }

    private void T_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
      if (ElapsedTime > 45)// && ElapsedTime < 85)
        PrintTime();

      if (ElapsedTime > 95 || Invalidate)
        t.Stop();
    }

    //---------------------------------------------------------------------------------------------

    private System.Timers.Timer t;
    public int ID;
    public string ShortCut = "WoS"; 
    public WallType Type;
    public DateTime CreationTime;
    public uint[] Bricks;
    public string[] BricksPositions;
    public double ElapsedTime
    {
      get
      {
        return (DateTime.Now - CreationTime).TotalSeconds;
      }
    }

    //---------------------------------------------------------------------------------------------

    public double Distance
    {
      get
      {
        if (BricksItems.Count > 0)
          return BricksItems[0].GetDistance();

        return -1;
      }
    }

    //---------------------------------------------------------------------------------------------

    public bool Invalidate
    {
      get { return BricksItems.Count <= 5 || Distance < 0 || Distance > 18; }
    }

    //---------------------------------------------------------------------------------------------

    public List<UOItem> BricksItems
    {
      get
      {
        List<UOItem> list = new List<UOItem>();

        for (int i = 0; i < this.Bricks.Length; i++)
        {
          if (this.BrickExist(i))
            list.Add(new UOItem(this.Bricks[i]));
          else
            list.Add(new UOItem(Serial.Invalid));
        }

        return list;
      }
    }


    //---------------------------------------------------------------------------------------------

    public UOItem RightBrick
    {
      get
      {
        for (int i = 0; i < this.Bricks.Length;i++)
        {
          if (BrickExist(i))
            return new UOItem(this.Bricks[i]);
        }
        return new UOItem(Serial.Invalid);
      }
    }

    //---------------------------------------------------------------------------------------------

    public UOItem LeftBrick
    {
      get
      {
        for (int i = this.Bricks.Length - 1; i >=0; i--)
        {
          if (BrickExist(i))
            return new UOItem(this.Bricks[i]);
        }
        return new UOItem(Serial.Invalid);
      }
    }

    //---------------------------------------------------------------------------------------------

    public UOItem CenterBrick
    {
      get
      {
        for (int i = 3; i < this.Bricks.Length; i++)
        {
          int prevIndex = 3 - (i - 3);

          if (BrickExist(i))
            return new UOItem(this.Bricks[i]);
          else if (BrickExist(prevIndex))
            return new UOItem(this.Bricks[prevIndex]);
        }

        return new UOItem(Serial.Invalid);
      }
    }

    //---------------------------------------------------------------------------------------------

    public bool  BrickExist(int index)
    {
      UOItem currentBrick = new UOItem(this.Bricks[index]);
      string posStr = currentBrick.X + "." + currentBrick.Y;
      return currentBrick.ExistCust() && this.BricksPositions[index] == posStr;
    }

    //---------------------------------------------------------------------------------------------

    public bool Exist
    {
      get
      {
        for (int i = 0; i < Bricks.Length; i++)
        {
          if (BrickExist(i))
            return true;
        }
        return false;
      }
    }

    //---------------------------------------------------------------------------------------------

    public bool FullWall
    {
      get
      {
        for (int i = 0; i < Bricks.Length; i++)
        {
          UOItem currentBrick = new UOItem(this.Bricks[i]);
          string posStr = currentBrick.X + "." + currentBrick.Y;

          if (!currentBrick.ExistCust() || this.BricksPositions[i] != posStr)
          {
            return false;
          }
        }
        return true;
      }
    }

    //---------------------------------------------------------------------------------------------

    public bool IsMyBrick(Serial s)
    {
      UOItem compareBrick = new UOItem(s);
      string posCompare = compareBrick.X + "." + compareBrick.Y;
      for (int i = 0; i < Bricks.Length; i++)
      {
        if (this.Bricks[i] == s && this.BricksPositions[i] == posCompare)
          return true;
      }

      return false;
    }

    //---------------------------------------------------------------------------------------------

    public void PrintInit()
    {
      if (Game.Debug)
      {
        foreach (UOItem brick in BricksItems)
        {
          if (brick.ExistCust())
            brick.PrintMessage(".");
        }

        this.LeftBrick.PrintMessage("L");
        this.RightBrick.PrintMessage("R");
      }
      this.CenterBrick.PrintMessage(String.Empty + ID, MessageType.Info);
    }

    //---------------------------------------------------------------------------------------------

    public void PrintTime()
    {
      double seconds = ElapsedTime;

      UOColor color = Game.Val_PureWhite;

      if (seconds > 95)
        color = Game.Val_LightRed;
      else if (seconds > 75)
        color = Game.Val_LightYellow;
      else if (seconds > 50)
        color = Game.Val_PureOrange;

      if (this.Exist)
        this.CenterBrick.PrintMessage("{0:N0}" + (Game.Debug ? " [{1}]" : ""), color, true, seconds, ID);
    }

    //---------------------------------------------------------------------------------------------
  }
}


