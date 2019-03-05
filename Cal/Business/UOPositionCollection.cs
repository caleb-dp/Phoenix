using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Caleb.Library.CAL;
using Caleb.Library.CAL.Business;
using System.Collections;
using CalExtension;

namespace Caleb.Library.CAL.Business
{
  public class UOPositionCollection : CalBusinessCollection<UOPosition>
  {
    private Hashtable fastCache;
    public int containsCalls = 0;
    public int callsTime = 0;

    protected override void OnInit()
    {
      base.OnInit();
      this.itemType = typeof(UOPosition);
      this.fastCache = new Hashtable();
    }

    public UOPosition GetItemByPosition(UOPosition other)
    {
      return this.GetItemByPosition(other.X, other.Y);
    }

    public override void Add(UOPosition item)
    {
      this.fastCache[item.X + "," + item.Y] = item;
      base.Add(item);
    }

    public UOPosition GetItemByPosition(ushort? x, ushort? y)
    {
      containsCalls++;
      return this.fastCache[x + "," + y] as UOPosition;
      //if (this.fastCache == null)
      //{
      //  this.fastCache = new Hashtable();
      //  foreach (UOPosition position in this)
      //  {
      //    fastCache[position.X + "," + position.Y] = position;
      //  }
      //}

      //DateTime start = DateTime.Now;
      //UOPosition result =  this.fastCache[x + "," + y] as UOPosition;

      //callsTime += (start - DateTime.Now).Milliseconds;
      //return result;
      //foreach (UOPosition position in this)
      //{
      //  if (position.X == x && position.Y == y)
      //    return position;
      //}
      
     // return null;
    }

    public void AddUniqueRange(UOPositionCollection other)
    {
      foreach (UOPosition otherItem in other)
      {
        if (this.GetItemByPosition(otherItem) == null)
          this.Add(otherItem);
      }
    }

    public UOPositionCollection SortByDistanceToPosition(IUOPosition positionFrom)
    {
      UOPositionCollection sortedCol = new UOPositionCollection();
      sortedCol.AddRange(this.ToArray());

      sortedCol.Sort(delegate(UOPosition a, UOPosition b) 
      {
        double distanceA = Robot.GetRelativeVectorLength(positionFrom,a);
        double distanceB = Robot.GetRelativeVectorLength(positionFrom,b);


        return distanceA.CompareTo(distanceB); 
      });

      return sortedCol;
    }

    public UOPositionCollection SortByOptimalTrack(IUOPosition positionFrom)
    {
      IUOPosition last = positionFrom;
      UOPositionCollection sortedCol = new UOPositionCollection();
      
      for (int i = 0; i < this.Count; i++)
      {
        UOPositionCollection search = this.SortByDistanceToPosition(last);

        if (search.Count > 0)
        {
          foreach (UOPosition pos in search)
          {
            if (!sortedCol.Contains(pos))
            {
              sortedCol.Add(pos);
              last = pos;
              break;
            }
          }

        }
      }
      return sortedCol;
    }
  }
}
