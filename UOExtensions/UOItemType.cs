using System;
using System.Collections.Generic;
using System.Text;
using Caleb.Library.CAL;
using Caleb.Library.CAL.Business;
using Caleb.Library;
using Phoenix;
using Phoenix.WorldData;
using CalExtension;

namespace Caleb.Library.CAL.Business
{
  public enum Precision
  {
    GraphicOnly = 1,
    GraphicColor = 2,
    Both = 4
  }

  public enum Search
  {
    Backpack = 1,
    Ground = 2,
    Both = 4
  }

  public class UOItemType : IUOItemType
  {
    //---------------------------------------------------------------------------------------------

    protected string name = String.Empty;
    public string Name
    {
      get { return this.name; }
      set { this.name = value; }
    }

    //---------------------------------------------------------------------------------------------

    protected List<string> names = new List<string>();
    public List<string> Names
    {
      get { return this.names; }
      set { this.names = value; }
    }

    //---------------------------------------------------------------------------------------------

    protected string shortName = String.Empty;
    public string ShortName
    {
      get { return this.shortName; }
      set { this.shortName = value; }
    }

    //---------------------------------------------------------------------------------------------

    protected ushort? graphicNum;
    public ushort? GraphicNum
    {
      get { return this.graphicNum; }
      set { this.graphicNum = value; }
    }

    //---------------------------------------------------------------------------------------------

    protected ushort? colorNum;
    public ushort? ColorNum
    {
      get { return this.colorNum; }
      set { this.colorNum = value; }
    }

    //---------------------------------------------------------------------------------------------

    protected bool twoHanded = false;
    public bool TwoHanded
    {
      get { return this.twoHanded; }
      set { this.twoHanded = value; }
    }

    //---------------------------------------------------------------------------------------------

    protected bool equip = false;
    public bool Equip
    {
      get { return this.equip; }
      set { this.equip = value; }
    }

    //---------------------------------------------------------------------------------------------

    protected Graphic graphic;
    public Graphic Graphic
    {
      get 
      { 
        if (this.graphic == null) 
          this.graphic = new Graphic(this.GraphicNum.Value); 
        return this.graphic;  
      }
      set { this.graphic = value; this.graphicNum = this.graphic; } 
    }

    //---------------------------------------------------------------------------------------------

    protected UOColor color;
    public UOColor Color
    {
      get
      {
        if (this.color == null)
          this.color = new UOColor(this.ColorNum.Value);
        return this.color;
      }
      set { this.color = value; this.colorNum = this.color; }
    }

    //---------------------------------------------------------------------------------------------

    public bool MatchSearchParams(params string[] args)
    {
      return false;
    }

    //---------------------------------------------------------------------------------------------

    public bool EqualUOItem(UOItem item)
    {
      return this.Graphic == item.Graphic && this.Color == item.Color;
    }

    //---------------------------------------------------------------------------------------------

    public bool UseType()
    {
      UOItem item = FindItem(Precision.Both, Search.Both);
      if (item != null)
      {
        item.Use();
        return true;
      }
      else
      {
        Journal.Clear();
        UO.UseType(this.Graphic, this.Color);
        return !Journal.WaitForText(true, 150, "Type not found");
      }
    }

    //---------------------------------------------------------------------------------------------

    public bool Use()
    {
      return this.Use(Precision.Both, Search.Both);
    }

    //---------------------------------------------------------------------------------------------

    public bool UseAsGCB()
    {
      return this.Use(Precision.GraphicColor, Search.Backpack);
    }

    //---------------------------------------------------------------------------------------------

    public bool Use(Precision prec, Search sea)
    {
      UOItem item = World.Player.Backpack.AllItems.FindType(this.Graphic, this.Color);
      if (item.Exist)
      {
        item.Use();
        return true;
      }

      return false;
    }

    //---------------------------------------------------------------------------------------------

    public UOItem FindItem()
    {
      return this.FindItem(Precision.GraphicColor, Search.Backpack);
    }

    //---------------------------------------------------------------------------------------------

    public UOItem FindItem(Precision prec, Search sea)
    {
     // Game.PrintMessage("FindItem " + this.Graphic);
      UOItem item = null;
      if (sea == Search.Backpack || sea == Search.Both)
      {
        item = prec == Precision.GraphicColor ? World.Player.Backpack.AllItems.FindType(this.Graphic, this.Color) : World.Player.Backpack.AllItems.FindType(this.Graphic);
        if (item == null || !item.Exist)
          item = prec == Precision.GraphicColor ? World.Player.Layers.FindType(this.Graphic, this.Color) : World.Player.Layers.FindType(this.Graphic);

        if ((item == null || !item.Exist) && World.Player.Layers[Layer.LeftHand].Graphic == this.Graphic)
          item = World.Player.Layers[Layer.LeftHand];

        if ((item == null || !item.Exist) && World.Player.Layers[Layer.RightHand].Graphic == this.Graphic)
          item = World.Player.Layers[Layer.RightHand];

        //Game.PrintMessage("LeftHand " + World.Player.Layers[Layer.LeftHand].Graphic);
        //Game.PrintMessage("RightHand " + World.Player.Layers[Layer.RightHand].Graphic);

        //foreach (UOItem li in World.Player.Layers)
        //  Game.PrintMessage("li " + li.Graphic);
      }

      if ((item == null || !item.Exist) && (sea == Search.Ground || sea == Search.Both))
        item = prec == Precision.GraphicColor ? World.Ground.FindType(this.Graphic, this.Color) : World.Player.Backpack.AllItems.FindType(this.Graphic);

      return item;
    }

    //---------------------------------------------------------------------------------------------

    public bool Move(ushort amount, ushort x, ushort y, sbyte z, Serial serial)
    {
      return this.Move(Precision.Both, Search.Both, amount, x, y, z, serial);
    }

    //---------------------------------------------------------------------------------------------

    public bool MoveBackpack(ushort amount, ushort x, ushort y, sbyte z, Serial serial)
    {
      return this.Move(Precision.Both, Search.Backpack, amount, x, y, z, serial);
    }

    //---------------------------------------------------------------------------------------------

    public bool Move(Precision prec, Search sea, ushort amount, ushort x, ushort y, sbyte z, Serial serial)
    {
      UOItem item = FindItem(prec, sea);
      if (item != null)
      {
        if (serial.IsValid)
          return item.Move(amount > 0 ? amount : item.Amount, serial, x, y);
        else
          return item.Move(amount > 0 ? amount : item.Amount, x, y, z);
        //else
        //  item.Move(amount > 0 ? amount : item.Amount, item.Container, x, y);
      }

      return false;
    }

    //---------------------------------------------------------------------------------------------

    public bool Is(UOItem item)
    {
      return this.Graphic == item.Graphic && this.Color == item.Color;
    }
  }
}
