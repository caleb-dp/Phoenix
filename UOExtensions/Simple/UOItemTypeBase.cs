using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;
using Caleb.Library;
using Caleb.Library.CAL.Business;
using System.Data;
using Caleb.Library.CAL;
using System.Collections;
using System.Windows.Forms;

namespace CalExtension
{
  public class UOItemTypeBase : IUOItemType
  {
    public UOItemTypeBase(Graphic graphic, UOColor color)
    {
      this.Graphic = graphic;
      this.Color = color;
    }

    public Graphic Graphic { get; set; }
    public UOColor Color { get; set; }
    public string Name { get; set; }

    public static bool ListContains(UOItem item, List<IUOItemType> items)
    {
      return ListContains(item.Graphic, item.Color, items);
    }

    public static bool ListContains(Graphic graphic, List<IUOItemType> items)
    {
      foreach (IUOItemType itemType in items)
      {
        if (graphic == itemType.Graphic)
          return true;
      }
      return false;
    }

    public static bool ListContains(Graphic graphic, UOColor color, List<IUOItemType> items)
    {
      foreach (IUOItemType itemType in items)
      {
        if (graphic == itemType.Graphic && color == itemType.Color)
          return true;
      }
      return false;
    }



  }


  public class UOItemTypeBaseCollection : List<UOItemTypeBase>
  {
    public UOItemTypeBaseCollection()
    {
    }

    public UOItemTypeBaseCollection(Graphic graphic) : this(graphic, 0xFFFF)
    {
    }

    public UOItemTypeBaseCollection(Graphic graphic, UOColor color) : this(new UOItemTypeBase(graphic, color))
    {
    }

    public UOItemTypeBaseCollection(params UOItemTypeBase[] items)
    {
      foreach (UOItemTypeBase item in items)
      {
        this.Add(item);
      }
    }

    public List<Graphic> GraphicList
    {
      get
      {
        List<Graphic> list = new List<Graphic>();

        foreach (UOItemTypeBase item in this)
        {
          list.Add(item.Graphic);
        }

        return list;
      }
    }

  }
}

