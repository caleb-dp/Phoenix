using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;
using System.Data;
using System.Collections;
using System.Windows.Forms;
using CalExtension.UOExtensions;
namespace CalExtension
{
  public class UOPositionBase : IUOPosition 
  {
    //---------------------------------------------------------------------------------------------

    public UOPositionBase()
    {
    }

    //---------------------------------------------------------------------------------------------

    public override bool Equals(object obj)
    {
      if (obj is IUOPosition)
      {
        return ((IUOPosition)obj).X == this.X && ((IUOPosition)obj).Y == this.Y;
      }
      return false;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    //---------------------------------------------------------------------------------------------

    public UOPositionBase(ushort x, ushort y, ushort z)
    {
      this.x = x;
      this.y = y;
      this.z = z;
    }

    //---------------------------------------------------------------------------------------------

    public IUOPosition CloneUntyped()
    {
      return (IUOPosition)this.Clone();
    }


    public UOPositionBase Clone()
    {
      return new UOPositionBase(this.x.GetValueOrDefault(), this.y.GetValueOrDefault(), this.z.GetValueOrDefault());
    }

    //---------------------------------------------------------------------------------------------

    protected ushort? x;
    public ushort? X
    {
      get { return this.x; }
      set { this.x = value; }
    }

    //---------------------------------------------------------------------------------------------

    protected ushort? y;
    public ushort? Y
    {
      get { return this.y; }
      set { this.y = value; }
    }

    //---------------------------------------------------------------------------------------------

    protected ushort? z = 0;
    public ushort? Z
    {
      get { return this.z; }
      set { this.z = value; }
    }

    //---------------------------------------------------------------------------------------------

    protected bool? stepable;
    public bool? Stepable
    {
      get { return this.stepable; }
      set { this.stepable = value; }
    }

    //---------------------------------------------------------------------------------------------

    public bool IsSepable
    {
      get { return this.Stepable.GetValueOrDefault(true) && !this.IsTree; }
    }

    //---------------------------------------------------------------------------------------------

    protected bool typeChecked = false;
    public bool TypeChecked
    {
      get { return this.typeChecked; }
      set { this.typeChecked = value; }
    }

    //---------------------------------------------------------------------------------------------

    protected bool isTree = false;
    public bool IsTree
    {
      get { return this.isTree; }
      set { this.isTree = value; }
    }

    //---------------------------------------------------------------------------------------------

    protected ushort? graphicNum = Graphic.Invariant;
    public ushort? GraphicNum
    {
      get { return this.graphicNum; }
      set { this.graphicNum = value; }
    }

    //---------------------------------------------------------------------------------------------

    protected string trackName = String.Empty;
    public string TrackName
    {
      get { return this.trackName; }
      set { this.trackName = value; }
    }

    //---------------------------------------------------------------------------------------------

    protected int trackPosition = 0;
    public int TrackPosition
    {
      get {  return this.trackPosition; }
      set { this.trackPosition = value; }
    }

    //---------------------------------------------------------------------------------------------

    protected string commandName = String.Empty;
    public string CommandName
    {
      get { return this.commandName; }
      set { this.commandName = value; }
    }


    //---------------------------------------------------------------------------------------------

    public override string ToString()
    {
      return this.X + "," + this.Y + "," + this.Z; //base.ToString();
    }

    //---------------------------------------------------------------------------------------------

    public List<IUOPosition> Adjacents
    {
      get
      {
        List< IUOPosition> col = new List<IUOPosition>();
        IUOPosition current = this;

        col.Add(new UOPositionBase((ushort)(current.X.Value - 1), (ushort)(current.Y.Value - 1), current.Z.GetValueOrDefault()));
        col.Add(new UOPositionBase((ushort)(current.X.Value - 1), (ushort)(current.Y.Value), current.Z.GetValueOrDefault()));
        col.Add(new UOPositionBase((ushort)(current.X.Value - 1), (ushort)(current.Y.Value + 1), current.Z.GetValueOrDefault()));
        col.Add(new UOPositionBase((ushort)(current.X.Value), (ushort)(current.Y.Value + 1), current.Z.GetValueOrDefault()));
        col.Add(new UOPositionBase((ushort)(current.X.Value + 1), (ushort)(current.Y.Value + 1), current.Z.GetValueOrDefault()));
        col.Add(new UOPositionBase((ushort)(current.X.Value + 1), (ushort)(current.Y.Value), current.Z.GetValueOrDefault()));
        col.Add(new UOPositionBase((ushort)(current.X.Value + 1), (ushort)(current.Y.Value - 1), current.Z.GetValueOrDefault()));
        col.Add(new UOPositionBase((ushort)(current.X.Value), (ushort)(current.Y.Value - 1), current.Z.GetValueOrDefault()));

        return col;
      }
    }
    //---------------------------------------------------------------------------------------------

    public static string ListToString(List<IUOPosition> list)
    {
      string[] arr = new string[list.Count];
      for (int i = 0; i < list.Count;i++)
        arr[i] = list[i].ToString();

      return String.Join("|", arr);
    }


    //---------------------------------------------------------------------------------------------

    public static List<IUOPosition> ParseList(string str)
    {
      List<IUOPosition> list = new List<IUOPosition>();
      string[] split = str.Split(new char[] { '|' });

      foreach (string strPos in split)
      {
        UOPositionBase pos = Parse(strPos);
        if (pos != null)
          list.Add(pos);
      }

      return list;
    }

    //---------------------------------------------------------------------------------------------

    

    public static UOPositionBase Parse(string str)
    {
      string[] split = str.Split(new char[] { '.' });

      ushort x;
      ushort y;
      ushort z;

      UOPositionBase position = new UOPositionBase();

      if (split.Length > 3)
        position.CommandName = split[3];

      if (split.Length > 2 && ushort.TryParse(split[2], out z))
        position.Z = z;
      else
        position.Z = 0;

      if (split.Length > 1 && ushort.TryParse(split[1], out y))
        position.Y = y;
      if (split.Length > 0 && ushort.TryParse(split[0], out x))
        position.X = x;

      if (position.X.HasValue && position.Y.HasValue)
        return position;

      return null;
    }

    //---------------------------------------------------------------------------------------------

    public static IUOPosition PlayerPosition
    {
      get
      {
        return CharacterPosition(World.Player);
      }
    }

    //---------------------------------------------------------------------------------------------

    public static IUOPosition CharacterPosition(UOCharacter ch)
    {
        return new UOPositionBase(ch.X, ch.Y, (ushort)ch.Z);
    }



    //---------------------------------------------------------------------------------------------

    public static implicit operator UOPositionBase(WorldLocation loc)
    {
      return new UOPositionBase(loc.X, loc.Y, (ushort)loc.Z);
    }

    //---------------------------------------------------------------------------------------------

    public static implicit operator WorldLocation(UOPositionBase pos)
    {
      return new WorldLocation(pos.X.GetValueOrDefault(), pos.Y.GetValueOrDefault(), (sbyte)pos.Z.GetValueOrDefault());
    }

    //---------------------------------------------------------------------------------------------

    public double RealDistance()
    {
      return RealDistance(World.Player.GetPosition());
    }

    //---------------------------------------------------------------------------------------------

    public ushort Distance()
    {
      return Distance(World.Player.GetPosition());
    }

    //---------------------------------------------------------------------------------------------

    public double RealDistance(IUOPosition positionFrom)
    {
      return Robot.GetRelativeVectorLength(positionFrom, this);
    }

    //---------------------------------------------------------------------------------------------

    public ushort Distance(IUOPosition positionFrom)
    {
      int distance = Math.Max(Math.Abs(positionFrom.X.GetValueOrDefault() - this.X.GetValueOrDefault()), Math.Abs(positionFrom.Y.GetValueOrDefault() - this.Y.GetValueOrDefault()));
      if (distance < 0)
        distance = 0;
      return (ushort)distance;
    }
  }
}
