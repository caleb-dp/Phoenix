//using System;
//using System.Collections.Generic;
//using System.Text;
//using Caleb.Library.CAL;
//using Caleb.Library.CAL.Business;
//using Caleb.Library;
//using Phoenix;
//using Phoenix.WorldData;
//using CalExtension;

//namespace Caleb.Library.CAL.Business
//{
//  public enum Precision
//  {
//    GraphicOnly = 1,
//    GraphicColor = 2,
//    Both = 4
//  }

//  public enum Search
//  {
//    Backpack = 1,
//    Ground = 2,
//    Both = 4
//  }

//  public class UOItemType : CalBusiness
//  {
//    //---------------------------------------------------------------------------------------------

//    protected string name = String.Empty;
//    [Parameter("Name", System.Data.DbType.String)]
//    public string Name
//    {
//      get { this.EnsureLoad(); return this.name; }
//      set { this.name = value; }
//    }

//    //---------------------------------------------------------------------------------------------

//    protected string shortName = String.Empty;
//    [Parameter("ShortName", System.Data.DbType.String)]
//    public string ShortName
//    {
//      get { this.EnsureLoad(); return this.shortName; }
//      set { this.shortName = value; }
//    }

//    //---------------------------------------------------------------------------------------------

//    protected UOItemTypeCategory category;
//    [Parameter("CategoryID", System.Data.DbType.Int32)]
//    public UOItemTypeCategory Category
//    {
//      get
//      {
//        this.EnsureLoad();
//        if (this.category == null) this.category = new UOItemTypeCategory();
//        return this.category;
//      }
//      set { this.category = value; }
//    }

//    //---------------------------------------------------------------------------------------------

//    protected ushort? graphicNum;
//    [Parameter("GraphicNum", System.Data.DbType.UInt16)]
//    [Require] 
//    public ushort? GraphicNum
//    {
//      get { this.EnsureLoad(); return this.graphicNum; }
//      set { this.graphicNum = value; }
//    }

//    //---------------------------------------------------------------------------------------------

//    protected ushort? colorNum;
//    [Parameter("ColorNum", System.Data.DbType.UInt16)]
//    [Require]
//    public ushort? ColorNum
//    {
//      get { this.EnsureLoad(); return this.colorNum; }
//      set { this.colorNum = value; }
//    }

//    //---------------------------------------------------------------------------------------------

//    protected bool twoHanded = false;
//    [Parameter("TwoHanded", System.Data.DbType.Boolean)]
//    public bool TwoHanded
//    {
//      get { this.EnsureLoad(); return this.twoHanded; }
//      set { this.twoHanded = value; }
//    }

//    //---------------------------------------------------------------------------------------------

//    protected bool equip = false;
//    [Parameter("Equip", System.Data.DbType.Boolean)]
//    public bool Equip
//    {
//      get { this.EnsureLoad(); return this.equip; }
//      set { this.equip = value; }
//    }

//    //---------------------------------------------------------------------------------------------

//    protected Graphic graphic;
//    public Graphic Graphic
//    {
//      get 
//      { 
//        if (this.graphic == null) 
//          this.graphic = new Graphic(this.GraphicNum.Value); 
//        return this.graphic;  
//      }
//      set { this.graphic = value; this.graphicNum = this.graphic; } 
//    }

//    //---------------------------------------------------------------------------------------------

//    protected UOColor color;
//    public UOColor Color
//    {
//      get
//      {
//        if (this.color == null)
//          this.color = new UOColor(this.ColorNum.Value);
//        return this.color;
//      }
//      set { this.color = value; this.colorNum = this.color; }
//    }

//    //---------------------------------------------------------------------------------------------

//    public bool LoadUnique()
//    {
//      if (this.GraphicNum.HasValue && this.ColorNum.HasValue)
//        return this.LoadByCondition("ColorNum=" + this.ColorNum + " AND GraphicNum=" + this.GraphicNum);
//      return false;
//    }

//    //---------------------------------------------------------------------------------------------

//    public override bool Equals(CalBusiness other)
//    {
//      UOItemType itemType = (UOItemType)other;
//      if (itemType.Graphic == this.Graphic && itemType.Color == this.Color) return true;
//      return false;
//    }

//    //---------------------------------------------------------------------------------------------

//    protected override void Validate()
//    {
//      base.Validate();
//      if (this.GraphicNum.HasValue && this.ColorNum.HasValue && !this.IsDistinctByCondition("ColorNum=" + this.ColorNum + " AND GraphicNum=" +  this.GraphicNum))
//        this.StatusMessages.Add(CalStatusMessageType.Error,this.GetType().Name + " jiz v databazi existuje");
//    }

//    //---------------------------------------------------------------------------------------------

//    public bool MatchSearchParams(params string[] args)
//    {
//      return false;
//    }

//    //---------------------------------------------------------------------------------------------

//    public bool EqualUOItem(UOItem item)
//    {
//      return this.Graphic == item.Graphic && this.Color == item.Color;
//    }

//    //---------------------------------------------------------------------------------------------

//    protected override void OnInit()
//    {
//      base.OnInit();

//      this.DbPrimaryKey = "UOItemTypeID";
//      this.DbTableName = "UO_UOItemType";
//    }

//    //---------------------------------------------------------------------------------------------

//    public bool Use()
//    {
//      return this.Use(Precision.Both, Search.Both);
//    }

//    //---------------------------------------------------------------------------------------------

//    public bool UseAsGCB()
//    {
//      return this.Use(Precision.GraphicColor, Search.Backpack);
//    }

//    //---------------------------------------------------------------------------------------------

//    public bool Use(Precision prec, Search sea)
//    {
//      UOItem item = FindItem(prec, sea);
//      if (item != null)
//      {
//        item.Use();
//        return true;
//      }

//      return false;
//    }

//    //---------------------------------------------------------------------------------------------

//    public UOItem FindItem()
//    {
//      return this.FindItem(Precision.GraphicColor, Search.Backpack);
//    }

//    //---------------------------------------------------------------------------------------------

//    public UOItem FindItem(Precision prec, Search sea)
//    {
//     // Game.CurrentGame.Messages.Add("FindItem " + this.Graphic);
//      UOItem item = null;
//      if (sea == Search.Backpack || sea == Search.Both)
//      {
//        item = prec == Precision.GraphicColor ? World.Player.Backpack.AllItems.FindType(this.Graphic, this.Color) : World.Player.Backpack.AllItems.FindType(this.Graphic);
//        if (item == null || !item.Exist)
//          item = prec == Precision.GraphicColor ? World.Player.Layers.FindType(this.Graphic, this.Color) : World.Player.Layers.FindType(this.Graphic);

//        if ((item == null || !item.Exist) && World.Player.Layers[Layer.LeftHand].Graphic == this.Graphic)
//          item = World.Player.Layers[Layer.LeftHand];

//        if ((item == null || !item.Exist) && World.Player.Layers[Layer.RightHand].Graphic == this.Graphic)
//          item = World.Player.Layers[Layer.RightHand];

//        //Game.CurrentGame.Messages.Add("LeftHand " + World.Player.Layers[Layer.LeftHand].Graphic);
//        //Game.CurrentGame.Messages.Add("RightHand " + World.Player.Layers[Layer.RightHand].Graphic);

//        //foreach (UOItem li in World.Player.Layers)
//        //  Game.CurrentGame.Messages.Add("li " + li.Graphic);
//      }

//      if ((item == null || !item.Exist) && (sea == Search.Ground || sea == Search.Both))
//        item = prec == Precision.GraphicColor ? World.Ground.FindType(this.Graphic, this.Color) : World.Player.Backpack.AllItems.FindType(this.Graphic);

//      return item;
//    }

//    //---------------------------------------------------------------------------------------------

//    public bool Move(ushort amount, ushort x, ushort y, sbyte z, Serial serial)
//    {
//      return this.Move(Precision.Both, Search.Both, amount, x, y, z, serial);
//    }
//    //---------------------------------------------------------------------------------------------

//    public bool Move(Precision prec, Search sea, ushort amount, ushort x, ushort y, sbyte z, Serial serial)
//    {
//      UOItem item = FindItem(prec, sea);
//      if (item != null)
//      {
//        if (serial.IsValid)
//          item.Move(amount > 0 ? amount : item.Amount, serial, x, y);
//        else 
//          item.Move(amount > 0 ? amount : item.Amount, x, y, z);
//        //else
//        //  item.Move(amount > 0 ? amount : item.Amount, item.Container, x, y);

//        return true;
//      }

//      return false;
//    }
//  }
//}
