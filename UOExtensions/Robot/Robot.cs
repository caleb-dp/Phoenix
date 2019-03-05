using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;
//using Caleb.Library;
//using Caleb.Library.CAL.Business;
using System.Data;
//using Caleb.Library.CAL;
using System.Collections;
using System.Windows.Forms;

namespace CalExtension
{
  //---------------------------------------------------------------------------------------------

  public enum MovementDirection
  {
    None = 9, Up = 7/*7*/, UpRight = 0/*0*/, Right = 1/*1*/, DownRight = 2/*2*/, Down = 3/*3*/, DownLeft = 4/*4*/, Left = 5/*5*/, UpLeft = 6/*6*/
  }

  //---------------------------------------------------------------------------------------------

  public class Robot : IDisposable //: Caleb.Library.CAL.CalBusiness 
  {
    public static int MoveWait { get { return 250 + (Core.CurrentLatency * 2); } }
    public static int ManipulationWait { get { return 500 + (Core.CurrentLatency * 2); } }
    public int SmallMoveWait { get { return UseMinWait ? Core.CurrentLatency : (100 + (Core.CurrentLatency * 2)); } }


    public object SyncRoot;
    public ushort SearchSuqareSize = 600;
    public bool UseTryGoOnly = true;
    public bool UseMinWait = true;
    public bool UseRun = false;
    public bool EnableLog = false;
    public bool UseCachedPathList = false;
    public Type PositionType = typeof(UOPositionBase);

    //---------------------------------------------------------------------------------------------

    public Robot()
    {
      this.SyncRoot = new object();
      this.OnInit();
    }

    //---------------------------------------------------------------------------------------------

    public void Dispose()
    {
    }

    //---------------------------------------------------------------------------------------------

    public IUOPosition CreatePositionInstance(ushort x, ushort y, ushort z)
    {
      IUOPosition instance = Activator.CreateInstance(this.PositionType) as IUOPosition;
      instance.X = x;
      instance.Y = y;
      instance.Z = z;

      return instance;
    }

    //---------------------------------------------------------------------------------------------

    public void OnInit()
    {
      this.lastPosition = this.CreatePositionInstance(World.Player.X, World.Player.Y, 0);
    }
    //---------------------------------------------------------------------------------------------

    public bool PositionChanged
    {
      get
      {
        //if (WalkHandling.DesiredPosition.X != lastloc.X && WalkHandling.DesiredPosition.Y != lastloc.Y)
        //{
        //  UpdateStats();
        //}

        //    return (WalkHandling.DesiredPosition.X != this.LatsPosition.X && WalkHandling.DesiredPosition.Y != this.LatsPosition.Y);TODO prozkoumat

        return !this.ActualPosition.Equals(this.LatsPosition);
      }
    }

    //---------------------------------------------------------------------------------------------

    protected IUOPosition lastPosition;
    public IUOPosition LatsPosition
    {
      get
      {
        if (this.lastPosition == null)
          this.lastPosition = ActualPosition;
        return this.lastPosition;
      }
      set
      {
        this.lastPosition = value;
        PathNode findNode = this.PossibleNodes.FindNode((ushort)this.lastPosition.X, (ushort)this.lastPosition.Y);
        if (findNode != null)
          findNode.Walkable = true;
      }
    }

    //---------------------------------------------------------------------------------------------

    public IUOPosition ActualPosition
    {
      get
      {
        PathNode findNode = this.PossibleNodes.FindNode(World.Player.X, World.Player.Y);
        if (findNode != null)
          findNode.Walkable = true;

        return this.CreatePositionInstance(World.Player.X, World.Player.Y, 0);
      }
    }

    //---------------------------------------------------------------------------------------------

    //public UOPositionCollection GetAcutal1StepCircle()
    //{
    //  UOPositionCollection col = new UOPositionCollection();
    //  UOPosition current = new UOPosition(this.ActualPosition.X.Value, this.ActualPosition.Y.Value, this.ActualPosition.Z.GetValueOrDefault());

    //  col.Add(new UOPosition((ushort)(current.X.Value - 1), (ushort)(current.Y.Value - 1), current.Z.GetValueOrDefault()));
    //  col.Add(new UOPosition((ushort)(current.X.Value - 1), (ushort)(current.Y.Value), current.Z.GetValueOrDefault()));
    //  col.Add(new UOPosition((ushort)(current.X.Value - 1), (ushort)(current.Y.Value + 1), current.Z.GetValueOrDefault()));
    //  col.Add(new UOPosition((ushort)(current.X.Value), (ushort)(current.Y.Value + 1), current.Z.GetValueOrDefault()));
    //  col.Add(new UOPosition((ushort)(current.X.Value + 1), (ushort)(current.Y.Value + 1), current.Z.GetValueOrDefault()));
    //  col.Add(new UOPosition((ushort)(current.X.Value + 1), (ushort)(current.Y.Value), current.Z.GetValueOrDefault()));
    //  col.Add(new UOPosition((ushort)(current.X.Value + 1), (ushort)(current.Y.Value - 1), current.Z.GetValueOrDefault()));
    //  col.Add(new UOPosition((ushort)(current.X.Value), (ushort)(current.Y.Value - 1), current.Z.GetValueOrDefault()));

    //  foreach (UOPosition pos in col)
    //  {
    //    pos.EnsureLoadByPosition();
    //  }

    //  return col;
    //}

    //---------------------------------------------------------------------------------------------

    protected IUOPosition targetPosition;
    public IUOPosition TargetPosition
    {
      get { return this.targetPosition; }
      set { this.targetPosition = value; }
    }

    //---------------------------------------------------------------------------------------------

    protected int tries = 0;
    protected int Tries
    {
      get { return this.tries; }
      set { this.tries = value; }
    }



    //---------------------------------------------------------------------------------------------

    //protected UOPositionCollection allPositions;
    //public UOPositionCollection AllPositions
    //{
    //  get
    //  {
    //    if (allPositions == null)
    //    {
    //      allPositions = new UOPositionCollection();
    //      allPositions.Load();
    //    }
    //    return allPositions;//(UOPositionCollection)new UOPositionCollection().Load();
    //  }
    //}

    //---------------------------------------------------------------------------------------------

    //protected UOPositionCollection squarePositions;
    //public UOPositionCollection SquarePositions
    //{
    //  get
    //  {
    //    if (this.squarePositions == null)
    //    {
    //      int currentX = this.ActualPosition.X.GetValueOrDefault();
    //      int currentY = this.ActualPosition.Y.GetValueOrDefault();
    //      int minX = 0;
    //      int minY = 0;
    //      int maxX = 0;
    //      int maxY = 0;

    //      minX = (currentX - this.SearchSuqareSize < 0 ? 0 : currentX - this.SearchSuqareSize);
    //      maxX = (currentX + this.SearchSuqareSize > 6000 ? 6000 : currentX + this.SearchSuqareSize);
    //      minY = (currentY - this.SearchSuqareSize < 0 ? 0 : currentY - this.SearchSuqareSize);
    //      maxY = (currentY + this.SearchSuqareSize > 6000 ? 6000 : currentY + this.SearchSuqareSize);

    //      this.squarePositions = new UOPositionCollection();
    //      this.squarePositions.Load(String.Format("X>={0} AND X<={1} AND Y>={2} AND Y<={3}", minX, maxX, minY, maxY));
    //    }
    //    return this.squarePositions;
    //  }
    //}

    //---------------------------------------------------------------------------------------------

    public bool Move(MovementDirection direction)
    {
      this.LatsPosition = this.CreatePositionInstance(World.Player.X, World.Player.Y, 0);

      if (World.Player.Direction != (byte)direction)
      {
        this.Press(GetDirectionKey(direction));
        //UO.Wait(75 + Core.Latency);
      }

      this.Press(GetDirectionKey(direction));
      if (!World.Player.Layers[Layer.Mount].Exist)
        UO.Wait(100 + Core.CurrentLatency);

      if (!this.PositionChanged)
      {
        this.Press(GetDirectionKey(direction));
        if (!World.Player.Layers[Layer.Mount].Exist)
          UO.Wait(100 + Core.CurrentLatency);
      }
      
      return this.PositionChanged;
    }

    //---------------------------------------------------------------------------------------------

    public void Press(Keys key)
    {
      if ((key & Keys.Modifiers) != Keys.None)
        throw new ScriptErrorException("Modifiers are not supported.");
      if (key == System.Windows.Forms.Keys.None)
        return;

      Client.PostMessage(Client.WM_KEYDOWN, (int)key, 0);
      Client.PostMessage(Client.WM_KEYUP, (int)key, 3);

      UO.Wait(100);
    }

    //---------------------------------------------------------------------------------------------

    public void MoveToEnd(MovementDirection direction)
    {
      while (this.Move(direction)) ;
    }

    //---------------------------------------------------------------------------------------------

    public bool GoTo(IUOPosition position)
    {
      return this.GoTo(position, 0);
    }

    //---------------------------------------------------------------------------------------------

    public bool GoTo(List<IUOPosition> path)
    {
      return GoTo(path, 0);
    }

    //---------------------------------------------------------------------------------------------

    public bool GoTo(List<IUOPosition> path, int fromDistance)
    {
      bool success = false;
      foreach (IUOPosition position in path)
        success = this.GoTo(position, fromDistance);
      return success;
    }

    //---------------------------------------------------------------------------------------------

    public bool GoTo(IUOPosition position, int fromDistance)
    {
      return this.GoTo((ushort)position.X, (ushort)position.Y, fromDistance, 500);
    }

    //---------------------------------------------------------------------------------------------

    public bool GoTo(IUOPosition position, int fromDistance, int tries)
    {
      return this.GoTo((ushort)position.X, (ushort)position.Y, fromDistance, tries);
    }

    //---------------------------------------------------------------------------------------------

    public bool GoTo(ushort x, ushort y)
    {
      return this.GoTo(x, y, 0, 500);
    }

    //---------------------------------------------------------------------------------------------

    public bool GoTo(ushort x, ushort y, int fromDistance, int tries)
    {
      return TryGoTo(x, y, fromDistance, tries);
    }

    //---------------------------------------------------------------------------------------------

    private PathNodeList possibleNodes;
    protected PathNodeList PossibleNodes
    {
      get
      {
        if (this.possibleNodes == null)
          this.possibleNodes = new PathNodeList();
        return this.possibleNodes;
      }
    }

    //---------------------------------------------------------------------------------------------

    public bool TryGoTo(ushort x, ushort y, int fromDistance, int tries)
    {
      return this.TryGoTo(x, y, fromDistance, tries, null);
    }

    //---------------------------------------------------------------------------------------------

    public event GotoEventHandler BeforeMove;
    public event GotoEventHandler AfterMoveSuccess;


    public event EventHandler GoToSuccess;

    private void OnGoToSuccess(object sender, EventArgs e)
    {
      if (this.GoToSuccess != null)
        this.GoToSuccess(this, e);
    }

    //---------------------------------------------------------------------------------------------

    private bool TryGoTo(ushort x, ushort y, int fromDistance, int tries, PathNodeList localList)
    {
      IUOPosition destination = this.CreatePositionInstance(x, y, 0);
      if (localList == null)
        localList = new PathNodeList();
      if (EnableLog)
        Game.PrintMessage("TryGoTo : " + x + ", " + y + " - " + tries);

      if (this.ActualPosition.Equals(destination) || Robot.GetRelativeVectorLength(this.ActualPosition, destination) <= fromDistance)
      {
        if (EnableLog)
          Game.PrintMessage("TryGoTo OK - 1: " + x + ", " + y);

        return true;
      }

      PathNode start = new PathNode() { X = World.Player.X, Y = World.Player.Y };
      PathNode end = new PathNode() { X = x, Y = y };


      if (tries > 0)
      {
        using (PathBuilder builder = new PathBuilder(UseCachedPathList ? this.PossibleNodes : localList))
        {
          if (builder.ComputePath(start, end, fromDistance))
          {
            PathNodeList computedPath = new PathNodeList();
            computedPath.AddRange(builder.ComputedPathNodes);
            computedPath.Reverse();

            if (EnableLog)
              Game.PrintMessage("TryGoTo Found: " + x + ", " + y + " / " + builder.Searchs);

            int step = 0;
            foreach (PathNode node in computedPath)
            {
              step++;
              IUOPosition pos = this.CreatePositionInstance(node.X, node.Y, x);
              GotoStepArgs args = new GotoStepArgs(pos, tries, fromDistance);
              if (this.BeforeMove != null)
                this.BeforeMove(this, args);

              bool moveFail = false;

              if (args.Abort)
              {
                Game.PrintMessage("TryGoTo Abort " + destination);
                return false;
              }

              if (args.IvalidDestination)
              {
                if (EnableLog)
                  Game.PrintMessage("Path IvalidDestination: " + this.ActualPosition.ToString() + " to " + node.ToString());
                moveFail = true;
              }
              else
              {
                if (!this.Move(GetMovementDirection(this.GetAngle(pos))))
                {
                  if (!this.Move(GetMovementDirection(this.GetAngle(pos))))
                    moveFail = true;
                }
              }

              if (moveFail)
              {
                if (EnableLog)
                  Game.PrintMessage("PathFail: " + this.ActualPosition.ToString() + " to " + node.ToString());

                PathNode findNode = builder.PossibleNodes.FindNode(node.X, node.Y);
                if (findNode != null && !findNode.Walkable.HasValue)
                  findNode.Walkable = false;

                return TryGoTo(x, y, fromDistance, --tries, localList);
              }

              if (this.AfterMoveSuccess != null)
                this.AfterMoveSuccess(this, new GotoStepArgs(pos, tries, fromDistance));

              if (args.Abort)
              {
                return false;
              }
            }

            //            09:44 Phoenix: TryGoTo: 1380, 2706 - 459
            //09:44 Phoenix: Compute Path FAIL: 1380,2707 to 1380,2706
            //09:44 Phoenix: TryGoTo END: 1380, 2706

            if (this.ActualPosition.Equals(destination) || Robot.GetRelativeVectorLength(this.ActualPosition, destination) <= fromDistance)
            {
              if (EnableLog)
                Game.PrintMessage("TryGoTo OK - 2: " + x + ", " + y);

              this.OnGoToSuccess(this, EventArgs.Empty);
              return true;
            }
            else
            {
              return TryGoTo(x, y, fromDistance, --tries, localList);
            }
          }
          else
          {
            if (EnableLog)
              Game.PrintMessage("Compute Path FAIL - Restart: " + start.ToString() + " to " + end.ToString());

            return TryGoTo(x, y, fromDistance, --tries, null);
          }
        }
      }


      if (EnableLog)
        Game.PrintMessage("TryGoTo END: " + x + ", " + y);

      return false;
    }

    //---------------------------------------------------------------------------------------------

    public bool GoToSimple(IUOPosition position)
    {
      return this.GoToSimple(position, 0);
    }

    //---------------------------------------------------------------------------------------------

    public bool GoToSimple(IUOPosition position, int fromDistance)
    {
      return this.GoToSimple((ushort)position.X, (ushort)position.Y, fromDistance);
    }

    //---------------------------------------------------------------------------------------------

    public bool GoToSimple(ushort x, ushort y, int fromDistance)
    {
      bool result = true;
      IUOPosition destination = this.CreatePositionInstance(x, y, 0);

      while (!this.ActualPosition.Equals(destination) && Robot.GetRelativeVectorLength(this.ActualPosition, destination) > fromDistance)
      {
        result = this.Move(GetMovementDirection(this.GetAngle(destination)));
        if (!result)
          break;
      }
      return result;
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    [BlockMultipleExecutions]
    public static void GoToStatic(ushort x, ushort y)
    {
      Robot robot = new Robot();
      robot.UseTryGoOnly = true;
      robot.GoTo(x, y);
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    [BlockMultipleExecutions]
    public static void GoToStatic(ushort x, ushort y, int fromDistance, int tries)
    {
      Robot robot = new Robot();
      robot.UseTryGoOnly = true;
      robot.EnableLog = true;
      robot.GoTo(x, y, fromDistance, tries);
    }

    //---------------------------------------------------------------------------------------------

    //       1371   2712.0  Flags: 0x0000  Color: 0x038A  Model: 0x0190  Renamable: False  Notoriety: Neutral  HP: -1/-1

    //Tile X=1371 Y=2713 Z=0 Graphic=0x053E Name=cave floor
    //Tile X=1372 Y=2713 Z=0 Graphic=0x053F Name=cave floor
    //Tile X=1372 Y=2712 Z=0 Graphic=0x053E Name=cave floor
    //Tile X=1372 Y=2711 Z=0 Graphic=0x053E Name=cave floor
    //Tile X=1371 Y=2711 Z=0 Graphic=0x053D Name=cave floor
    //Tile X=1370 Y=2711 Z=0 Graphic=0x053D Name=cave floor
    //Tile X=1370 Y=2712 Z=0 Graphic=0x053F Name=cave floor
    //Tile X=1370 Y=2713 Z=0 Graphic=0x053F Name=cave floor
    /* 
    * x0,y+1,a0
    * x+1,y+1,a45
    * x+1,y0,a90
    * x+1,y-1,a135
    * x0,y-1,a180
    * x-1,y-1,a225
    * x-1,y0,a270
    * x-1,y+1,a315
     */

    //---------------------------------------------------------------------------------------------

    public static double GetNextCoordinates(double angle, IUOPosition fromPosition, out IUOPosition nextPosition)
    {
      ushort nextX;
      ushort nextY;

      double nextAngle = GetNextCoordinates(angle, fromPosition.X.Value, fromPosition.Y.Value, out nextX, out nextY);

      if (nextAngle > -1)
        nextPosition = new UOPositionBase(nextX, nextY, 0);
      else
        nextPosition = null;

      return nextAngle;
    }

    //---------------------------------------------------------------------------------------------


    public static double GetNextCoordinates(double angle, ushort x, ushort y, out ushort resultX, out ushort resultY)
    {
      resultX = x;
      resultY = y;

      if (angle >= 22.5 && angle < 67.5)
      {
        resultX++; resultY++;
        return 45;
      }
      else if (angle >= 67.5 && angle < 112.5)
      {
        resultX++;
        return 90;
      }
      else if (angle >= 112.5 && angle < 157.5)
      {
        resultX++; resultY--;
        return 135;
      }
      else if (angle >= 157.5 && angle < 202.5)
      {
        resultY--;
        return 180;
      }
      else if (angle >= 202.5 && angle < 247.5)
      {
        resultX--; resultY--;
        return 225;
      }
      else if (angle >= 247.5 && angle < 292.5)
      {
        resultX--;
        return 270;
      }
      else if (angle >= 292.5 && angle < 337.5)
      {
        resultX--; resultY++;
        return 315;
      }
      else if (angle >= 337.5 && angle <= 360 || angle >= 0 && angle < 22.5)
      {
        resultY++;
        return 0;
      }
      else
      {
        return -1;
      }
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void PrintAngle(ushort x, ushort y)
    {
      IUOPosition pos = new UOPositionBase(x, y, 0);
      Game.PrintMessage("Angle To destination:" + new Robot().GetAngle(pos));
    }

    //---------------------------------------------------------------------------------------------

    public double GetAngle(IUOPosition destination)
    {
      return GetAngle(this.ActualPosition, destination);
    }

    //---------------------------------------------------------------------------------------------

    public static double GetAngle(IUOPosition start, IUOPosition destination)
    {
      int destX;
      int destY;
      Robot.GetRelativeCoordinates(start, destination, out destX, out destY);
      return Robot.GetVectorAngle(destX, destY);
    }

    //---------------------------------------------------------------------------------------------

    public static double GetRelativeAngle(double angle)
    {
      if (angle >= 22.5 && angle < 67.5)
        return angle - 22.5;
      else if (angle >= 67.5 && angle < 112.5)
        return angle - 67.5;
      else if (angle >= 112.5 && angle < 157.5)
        return angle - 112.5;
      else if (angle >= 157.5 && angle < 202.5)
        return angle - 157.5;
      else if (angle >= 202.5 && angle < 247.5)
        return angle - 202.5;
      else if (angle >= 247.5 && angle < 292.5)
        return angle - 247.5;
      else if (angle >= 292.5 && angle < 337.5)
        return angle - 292.5;
      else if (angle >= 337.5)
        return angle - 337.5;
      else
        return (angle + 22.5);
    }

    //---------------------------------------------------------------------------------------------

    public static double GetDirectionIndex(double relativeAngle)
    {
      return relativeAngle / 22.5;
    }

    //---------------------------------------------------------------------------------------------

    public static MovementDirection GetMovementDirection(double angle)
    {
      if ((angle >= 0 && angle < 22.5) || angle >= 337.5)
        return MovementDirection.DownLeft;
      else if (angle >= 22.5 && angle < 67.5)
        return MovementDirection.Down;
      else if (angle >= 67.5 && angle < 112.5)
        return MovementDirection.DownRight;
      else if (angle >= 112.5 && angle < 157.5)
        return MovementDirection.Right;
      else if (angle >= 157.5 && angle < 202.5)
        return MovementDirection.UpRight;
      else if (angle >= 202.5 && angle < 247.5)
        return MovementDirection.Up;
      else if (angle >= 247.5 && angle < 292.5)
        return MovementDirection.UpLeft;
      else if (angle >= 292.5 && angle < 337.5)
        return MovementDirection.Left;

      return MovementDirection.None;
    }

    //---------------------------------------------------------------------------------------------

    public static double GetNextAngle(double angle)
    {
      double result = 0;
      if (angle > 360) angle = angle - 360;
      double angleAngle = GetAngleDirection(angle);
      if (angleAngle >= 0) result = angle + 45;
      else result = angle - 45;
      if (result > 360) result = result - 360;
      else if (result < 0)
        result = 360 + result;
      return result;
    }

    //---------------------------------------------------------------------------------------------

    public static double GetAngleDirection(double angle)
    {
      if ((angle >= 0 && angle < 22.5) || angle >= 337.5)
      {
        if (angle > 337.5) return angle - 360;
        else if (angle >= 0 && angle < 22.5) return angle;
        else return 0;
      }
      else if (angle >= 22.5 && angle < 67.5) { return angle - 45; }
      else if (angle >= 67.5 && angle < 112.5) { return angle - 90; }
      else if (angle >= 112.5 && angle < 157.5) { return angle - 135; }
      else if (angle >= 157.5 && angle < 202.5) { return angle - 180; }
      else if (angle >= 202.5 && angle < 247.5) { return angle - 225; }
      else if (angle >= 247.5 && angle < 292.5) { return angle - 270; }
      else if (angle >= 292.5 && angle < 337.5) { return angle - 315; }

      return 0;
    }

    //---------------------------------------------------------------------------------------------

    public static System.Windows.Forms.Keys GetDirectionKey(MovementDirection direction)
    {
      switch (direction)
      {
        case MovementDirection.Up:
          return System.Windows.Forms.Keys.Up;
        case MovementDirection.UpRight:
          return System.Windows.Forms.Keys.PageUp;
        case MovementDirection.Right:
          return System.Windows.Forms.Keys.Right;
        case MovementDirection.DownRight:
          return System.Windows.Forms.Keys.PageDown;
        case MovementDirection.Down:
          return System.Windows.Forms.Keys.Down;
        case MovementDirection.DownLeft:
          return System.Windows.Forms.Keys.End;
        case MovementDirection.Left:
          return System.Windows.Forms.Keys.Left;
        case MovementDirection.UpLeft:
          return System.Windows.Forms.Keys.Home;
        default:
          return System.Windows.Forms.Keys.None;
      }
    }

    //---------------------------------------------------------------------------------------------

    public static string GetDirectionKeyStr(MovementDirection direction)
    {
      switch (direction)
      {
        case MovementDirection.Up:
          return "{UP}";
        case MovementDirection.UpRight:
          return "{PGUP}";
        case MovementDirection.Right:
          return "{RIGHT}";
        case MovementDirection.DownRight:
          return "{PGDN}";
        case MovementDirection.Down:
          return "{DOWN}";
        case MovementDirection.DownLeft:
          return "{END}";
        case MovementDirection.Left:
          return "{LEFT}";
        case MovementDirection.UpLeft:
          return "{HOME}";
        default:
          return "";
      }
    }


    //---------------------------------------------------------------------------------------------

    public static double GetVectorLength(int x, int y)
    {
      double vLength = Math.Pow((Math.Pow(x, 2) + Math.Pow(y, 2)), 0.5);
      return vLength;
    }

    //---------------------------------------------------------------------------------------------

    public static double GetVectorAngle(int x, int y)
    {
      double vectorLength = GetVectorLength(x, y);
      if ((x * y) > 0)
      {
        double angleSinus = Math.Abs(x) / vectorLength;
        double relativeAngle = RadiansToDegree(Math.Asin(angleSinus));
        if (x > 0 && y > 0) return relativeAngle;
        else return relativeAngle + 180;
      }
      else if ((x * y) < 0)
      {
        double angleSinus = Math.Abs(y) / vectorLength;
        double relativeAngle = RadiansToDegree(Math.Asin(angleSinus));
        if (x < 0) return relativeAngle + 270;
        else return relativeAngle + 90;
      }
      else
      {
        if (x == 0 && y > 0) return 0;
        else if (x == 0 && y < 0) return 180;
        else if (y == 0 && x > 0) return 90;
        else if (y == 0 && x < 0) return 270;
      }
      return -1;
    }

    //---------------------------------------------------------------------------------------------

    public static double RadiansToDegree(double value)
    {
      return (value * 180) / Math.PI;
    }

    //---------------------------------------------------------------------------------------------

    public static void GetRelativeCoordinates(IUOPosition start, IUOPosition position, out int x, out int y)
    {
      GetRelativeCoordinates(start.X.Value, start.Y.Value, position.X.Value, position.Y.Value, out x, out y);
    }

    //---------------------------------------------------------------------------------------------

    public static void GetRelativeCoordinates(ushort startX, ushort startY, ushort originalX, ushort originalY, out int x, out int y)
    {
      x = (originalX - startX);
      y = (originalY - startY);
    }

    //---------------------------------------------------------------------------------------------

    public static double GetRelativeVectorLength(IUOPosition start, IUOPosition position)
    {
      int x;
      int y;
      GetRelativeCoordinates(start, position, out x, out y);
      //UO.Print("{0}, {1}, {2}, {3}, {4}, {5}", x, y, position.X, position.Y, this.X, this.Y);
      return Robot.GetVectorLength(x, y);
    }

    //---------------------------------------------------------------------------------------------

    public static double GetRealDistance(IUOPosition start, IUOPosition position)
    {
      return Math.Max(Math.Abs(start.X.GetValueOrDefault() - position.X.GetValueOrDefault()), Math.Abs(start.Y.GetValueOrDefault() - position.Y.GetValueOrDefault()));
    }

    //---------------------------------------------------------------------------------------------

    public static bool LookingDirectlyAt(IUOPosition a, IUOPosition b, byte direction)
    {
      double angle = GetAngle(a, b);
      Dictionary<byte, double> transform = new Dictionary<byte, double>();
      transform.Add(0, 180);
      transform.Add(1, 135);
      transform.Add(2, 90);
      transform.Add(3, 45);
      transform.Add(4, 0);
      transform.Add(5, 315);
      transform.Add(6, 270);
      transform.Add(7, 225);


      return transform[direction] == Math.Round(angle, 10);
    }


  }

  public delegate void GotoEventHandler(Robot o, GotoStepArgs e);

  public class GotoStepArgs : EventArgs
  {
    public int Tries;
    public int FromDistance;
    public bool IvalidDestination = false;
    public bool Abort = false;
    public IUOPosition Destination;
    public GotoStepArgs(IUOPosition destination, int tries, int fromDistance)
    {
      this.Destination = destination;
      this.Tries = tries;
      this.FromDistance = fromDistance;
    }
  }

  public class GotoInfo
  {
    
  }


}

//Pokud UOCharacter.Direction = 0-7 a GetAngler = 0-315 tak ano jinak ne
//0;180
//1;135
//2;90
//3;45
//4;0
//5;315
//6;270
//7;225
