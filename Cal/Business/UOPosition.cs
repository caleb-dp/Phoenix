using System;
using System.Collections.Generic;
using System.Text;
using Caleb.Library.CAL;
using Caleb.Library.CAL.Business;
using Caleb.Library;
using Phoenix;
using Phoenix.WorldData;
using System.Text.RegularExpressions;
using CalExtension;
using CalExtension.UOExtensions;

namespace Caleb.Library.CAL.Business
{
  public class UOPosition : CalBusiness, IUOPosition
  {
    //---------------------------------------------------------------------------------------------

    public UOPosition()
    {
      this.OnInit();
    }

    //---------------------------------------------------------------------------------------------

    public override bool Equals(object obj)
    {
      if (obj is UOPosition)
      {
        return ((UOPosition)obj).X == this.X && ((UOPosition)obj).Y == this.Y;
      }
      return false;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    //---------------------------------------------------------------------------------------------

    public UOPosition(ushort x, ushort y, ushort z)
    {
      this.x = x;
      this.y = y;
      this.z = z;
      this.OnInit();
    }
    //protected string name = String.Empty;
    //[Parameter("Name", System.Data.DbType.String)]
    //[Require]
    //public string Name
    //{
    //  get { this.EnsureLoad(); return this.name; }
    //  set { this.name = value; }
    //}

    //---------------------------------------------------------------------------------------------

    public IUOPosition CloneUntyped()
    {
      return (IUOPosition)this.Clone();
    }


    public UOPosition Clone()
    {
      return new UOPosition(this.x.GetValueOrDefault(), this.y.GetValueOrDefault(), this.z.GetValueOrDefault());
    }

    //---------------------------------------------------------------------------------------------

    protected ushort? x;
    [Parameter("X", System.Data.DbType.Int16)]
    [Require]
    public ushort? X
    {
      get { this.EnsureLoad(); return this.x; }
      set { this.x = value; }
    }

    //---------------------------------------------------------------------------------------------

    protected ushort? y;
    [Parameter("Y", System.Data.DbType.Int16)]
    [Require]
    public ushort? Y
    {
      get { this.EnsureLoad(); return this.y; }
      set { this.y = value; }
    }

    //---------------------------------------------------------------------------------------------

    protected ushort? z = 0;
    [Parameter("Z", System.Data.DbType.Int16)]
    [Require]
    public ushort? Z
    {
      get { this.EnsureLoad(); return this.z; }
      set { this.z = value; }
    }

    //---------------------------------------------------------------------------------------------

    protected bool? stepable;
    [Parameter("Stepable", System.Data.DbType.Boolean)]
    public bool? Stepable
    {
      get { this.EnsureLoad(); return this.stepable; }
      set { this.stepable = value; }
    }

    //---------------------------------------------------------------------------------------------

    public bool IsSepable
    {
      get { return this.Stepable.GetValueOrDefault(true) && !this.IsTree; }
    }

    //---------------------------------------------------------------------------------------------

    protected bool typeChecked = false;
    [Parameter("TypeChecked", System.Data.DbType.Boolean)]
    public bool TypeChecked
    {
      get { this.EnsureLoad(); return this.typeChecked; }
      set { this.typeChecked = value; }
    }

    //---------------------------------------------------------------------------------------------

    protected bool isTree = false;
    [Parameter("IsTree", System.Data.DbType.Boolean)]
    public bool IsTree
    {
      get { this.EnsureLoad(); return this.isTree; }
      set { this.isTree = value; }
    }

    //---------------------------------------------------------------------------------------------

    protected ushort? graphicNum = Graphic.Invariant;
    [Parameter("GraphicNum", System.Data.DbType.UInt16)]
    [Require]
    public ushort? GraphicNum
    {
      get { this.EnsureLoad(); return this.graphicNum; }
      set { this.graphicNum = value; }
    }

    //---------------------------------------------------------------------------------------------

    protected string trackName = String.Empty;
    [Parameter("TrackName", System.Data.DbType.String)]
    public string TrackName
    {
      get { this.EnsureLoad(); return this.trackName; }
      set { this.trackName = value; }
    }

    //---------------------------------------------------------------------------------------------

    protected int trackPosition = 0;
    [Parameter("TrackPosition", System.Data.DbType.String)]
    public int TrackPosition
    {
      get { this.EnsureLoad(); return this.trackPosition; }
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

    public void EnsureLoadByPosition()
    {
      if (this.needLoad)
      {
        this.LoadByCondition("X=" + this.X.GetValueOrDefault() + " AND Y=" + this.Y.GetValueOrDefault());
        // this.Load();
      }
    }

    //---------------------------------------------------------------------------------------------

    public bool LoadByPosition()
    {
      return this.LoadByCondition("X=" + this.X.GetValueOrDefault() + " AND Y=" + this.Y.GetValueOrDefault());
    }

    //---------------------------------------------------------------------------------------------

    public override bool Equals(CalBusiness other)
    {
      UOPosition pos = (UOPosition)other;
      if (pos != null && pos.X == this.X && this.Y == pos.Y) return true;
      return false;
    }


    //---------------------------------------------------------------------------------------------

    protected override void Validate()
    {
      base.Validate();
      if (this.X.HasValue && this.Y.HasValue && !this.IsDistinctByCondition("X=" + this.X + " AND Y=" + this.Y))
        this.StatusMessages.Add(CalStatusMessageType.Error, "Pozice jiz existuje v databazi!");
    }


    //---------------------------------------------------------------------------------------------

    protected override void OnInit()
    {
      base.OnInit();

      this.DbPrimaryKey = "UOPositionID";
      this.DbTableName = "UO_UOPosition";
    }

    //---------------------------------------------------------------------------------------------

    public override string ToString()
    {
      return this.X + "," + this.Y + "," + this.Z; //base.ToString();
    }

    //---------------------------------------------------------------------------------------------

    public UOPositionCollection Adjacents
    {
      get
      {
        UOPositionCollection col = new UOPositionCollection();
        UOPosition current = this;

        col.Add(new UOPosition((ushort)(current.X.Value - 1), (ushort)(current.Y.Value - 1), current.Z.GetValueOrDefault()));
        col.Add(new UOPosition((ushort)(current.X.Value - 1), (ushort)(current.Y.Value), current.Z.GetValueOrDefault()));
        col.Add(new UOPosition((ushort)(current.X.Value - 1), (ushort)(current.Y.Value + 1), current.Z.GetValueOrDefault()));
        col.Add(new UOPosition((ushort)(current.X.Value), (ushort)(current.Y.Value + 1), current.Z.GetValueOrDefault()));
        col.Add(new UOPosition((ushort)(current.X.Value + 1), (ushort)(current.Y.Value + 1), current.Z.GetValueOrDefault()));
        col.Add(new UOPosition((ushort)(current.X.Value + 1), (ushort)(current.Y.Value), current.Z.GetValueOrDefault()));
        col.Add(new UOPosition((ushort)(current.X.Value + 1), (ushort)(current.Y.Value - 1), current.Z.GetValueOrDefault()));
        col.Add(new UOPosition((ushort)(current.X.Value), (ushort)(current.Y.Value - 1), current.Z.GetValueOrDefault()));

        return col;
      }
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

    //---------------------------------------------------------------------------------------------
  }
}
