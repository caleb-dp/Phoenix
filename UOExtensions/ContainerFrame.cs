using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;
using Caleb.Library;
using Caleb.Library.CAL.Business;
using CalExtension.Skills;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Collections;
using System.Threading;

namespace CalExtension.UOExtensions
{
  public class ContainerFrame
  {
    //---------------------------------------------------------------------------------------------

    public ContainerFrame(Serial container, int width, int height, int slotW, int slotH)
    {
      this.container = container;
      this.width = width;
      this.height = height;
      this.slotW = slotW;
      this.slotH = slotH;
     
      this.rows = height / slotH;
      this.columns = width / slotW;
      this.slots = new ContainerSlot[rows * columns];
      //this.slotsMatice = new ContainerSlot[]

      for (int i = 0; i < this.rows; i++)
      {
        for (int u = 0; u < this.columns; u++)
        {
          int x = (u * slotW) + 1;
          int y = (i * slotH) + 1;
          this.slots[i * this.rows + u] = new ContainerSlot(container, x, y, slotW, slotH, i, u);
        }
      }
    }

    private Serial container;
    private int width;
    private int height;
    private int slotW;
    private int slotH;
    private int rows;
    private int columns;
    private ContainerSlot[] slots;

    //---------------------------------------------------------------------------------------------

    public List<ContainerSlot> Slots
    {
      get
      {
        List<ContainerSlot> slots = new List<ContainerSlot>();
        slots.AddRange(this.slots);
        return slots;
      }
    }

    //---------------------------------------------------------------------------------------------

    public List<List<ContainerSlot>> Rows
    {
      get
      {
        List<List<ContainerSlot>> list = new List<List<ContainerSlot>>();
        for (int i = 0; i < this.rows; i++)
        {
          list.Add(new List<ContainerSlot>());
          for (int u = 0; u < this.columns; u++)
          {
            list[i].Add(this.slots[i * this.rows + u]);
          }

        }
        return list;
      }
    }

    //---------------------------------------------------------------------------------------------

    //public List<List<ContainerSlot>> Columns
    //{
    //  get
    //  {
    //    //List<List<ContainerSlot>> list = new List<List<ContainerSlot>>();
        
    //  }
    //}

    //---------------------------------------------------------------------------------------------

  }

  public class ContainerSlot
  {
    public ContainerSlot(Serial owner, int x, int y, int width, int height, int row, int column)
    {
      this.owner = owner;
      this.x = x;
      this.y = y;
      this.width = width;
      this.height = height;
      this.row = row;
      this.column = column;
    }

    //---------------------------------------------------------------------------------------------

    private Serial owner;
    private int x;
    private int y;
    private int width;
    private int height;
    private int row;
    private int column;

    public ushort X
    {
      get { return (ushort)this.x; }
    }

    public ushort Y
    {
      get { return (ushort)this.y;  }
    }


    //---------------------------------------------------------------------------------------------

    public List<UOItem> Items
    {
      get
      {
        List<UOItem> items = new List<UOItem>();
        UOItem cont = new UOItem(this.owner);
        foreach (UOItem item in cont.Items)
        {
          if (item.X >= this.x && item.X < this.x + this.width && item.Y >= this.y && item.Y < this.y + this.height)
            items.Add(item);
        }
        return items;
      }
    }

    //---------------------------------------------------------------------------------------------

    public bool Empty
    {
      get
      {
        return this.Items.Count == 0;
      }
    }

    //---------------------------------------------------------------------------------------------

    public void Push(UOItem item)
    {
      Push(0, item, ContainerSlotAlign.TopLeft);
    }

    //---------------------------------------------------------------------------------------------

    public void Push(ushort amount, UOItem item)
    {
      Push(amount, item, ContainerSlotAlign.TopLeft);
    }

    //---------------------------------------------------------------------------------------------

    public bool Push(ushort amount, UOItem item, ContainerSlotAlign align)
    {
      int x = this.x;
      int y = this.y;

      if (align == ContainerSlotAlign.TopRight)
        x = x + this.width - 1;
      else if (align == ContainerSlotAlign.BottomLeft)
        y = y + this.height - 1;
      else if (align == ContainerSlotAlign.BottomRight)
      {
        x = x + this.width - 1;
        y = y + this.height - 1;
      }
      else if (align == ContainerSlotAlign.Center)
      {
        x = x + (this.width / 2);
        y = y + (this.height / 2);
      }

      if (amount == 0)
        amount = item.Amount;

     return item.Move(amount, this.owner, (ushort)x, (ushort)y);
    }

    //---------------------------------------------------------------------------------------------



  }

  public enum ContainerSlotAlign
  {
    TopLeft = 0,
    TopRight = 1,
    BottomRight = 2,
    BottomLeft = 4,
    Center = 8
  }


}

