//using System;
//using System.Collections.Generic;
//using System.Text;
//using Caleb.Library.CAL;
//using Caleb.Library.CAL.Business;
//using Phoenix;

//namespace Caleb.Library.CAL.Business
//{
//  public class UOItemTypeCollection : CalBusinessCollection<UOItemType>
//  {
//    //---------------------------------------------------------------------------------------------

//    protected override void OnInit()
//    {
//      base.OnInit();
//      this.itemType = typeof(UOItemType);
//    }

//    //---------------------------------------------------------------------------------------------

//    public UOItemType this[string name]
//    {
//      get
//      {
//        if (name != null)
//        {
//          foreach (UOItemType item in this)
//          {
//            if (item.Name.ToLower() == name.ToLower())
//              return item;
//          }
//        }
//        return null;
//      }
//    }

//    //---------------------------------------------------------------------------------------------

//    public bool Contains(Graphic graphic, UOColor color)
//    {
//      foreach (UOItemType item in this)
//      {
//        if (item.Graphic == graphic && item.Color == color)
//          return true;
//      }
//      return false;
//    }

//    //---------------------------------------------------------------------------------------------

//    public UOItemType FindItem(Graphic graphic, params string[] args)
//    {
//      UOItemTypeCollection items = FindItems(graphic, args);
//      if (items.Count > 0)
//        return items[0];

//      return null;
//    }

//    //---------------------------------------------------------------------------------------------

//    public UOItemTypeCollection FindItems(Graphic graphic, params string[] args)
//    {
//      UOItemTypeCollection items = new UOItemTypeCollection();
//      foreach (UOItemType item in this)
//      {
//        if (item.Graphic == graphic && (args.Length == 0 || item.MatchSearchParams(args)))
//          items.Add(item);
//      }

//      return items;
//    }

//    //---------------------------------------------------------------------------------------------

//    public Graphic[] GraphicArray
//    {
//      get
//      {
//        Graphic[] arr = new Graphic[this.Count];

//        for (int i = 0; i < this.Count; i++)
//        {
//          arr[i] = this[i].Graphic;
//        }

//        return arr;
//      }
//    }

//    //---------------------------------------------------------------------------------------------

//    public UOColor[] ColorArray
//    {
//      get
//      {
//        UOColor[] arr = new UOColor[this.Count];

//        for (int i = 0; i < this.Count; i++)
//        {
//          arr[i] = this[i].Color;
//        }

//        return arr;
//      }
//    }
//  }
//}
