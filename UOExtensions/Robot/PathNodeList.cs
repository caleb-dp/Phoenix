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
  public class PathNodeList : List<PathNode>, IDisposable
  {
    //---------------------------------------------------------------------------------------------

    private Hashtable ht;
    public PathNodeList()
    {
      this.ht = new Hashtable();
    }

    //---------------------------------------------------------------------------------------------

    public new void Add(PathNode item)
    {
      this.ht[item.X + "," + item.Y] = item;
      base.Add(item);
    }

    //---------------------------------------------------------------------------------------------

    public new void RemoveAt(int index)
    {
      this.ht.Remove(this[index].X + "," + this[index].Y);
      base.RemoveAt(index);
    }

    //---------------------------------------------------------------------------------------------

    public new bool Remove(PathNode item)
    {
      this.ht.Remove(item.X + "," + item.Y);
      return base.Remove(item);
    }

    //---------------------------------------------------------------------------------------------

    public bool ContainsNode(PathNode other)
    {
      return FindNode(other.X, other.Y) != null;
    }

    //---------------------------------------------------------------------------------------------

    public PathNode FindNode(ushort x, ushort y)
    {
      return this.ht[x + "," + y] as PathNode;
    }

    //---------------------------------------------------------------------------------------------

    public void MergePossible(PathNodeList possibleList)
    {
      int count = this.Count;
      for (int i = count - 1; i >= 0; i--)
      {
        PathNode findNode = possibleList.FindNode(this[i].X, this[i].Y);
        if (findNode == null || findNode.Walkable.HasValue && !findNode.Walkable.Value)
          this.RemoveAt(i);
      }
    }

    //---------------------------------------------------------------------------------------------

    public void MergeIdent(PathNodeList possibleList)
    {
      int count = this.Count;
      for (int i = count - 1; i >= 0; i--)
      {
        PathNode findNode = possibleList.FindNode(this[i].X, this[i].Y);
        if (findNode != null)
          this.RemoveAt(i);
      }
    }

    //---------------------------------------------------------------------------------------------

    public void NormalizeReferences(PathNodeList originalList)
    {
      for (int i = 0; i < this.Count; i++)
      {
        PathNode originalFound = originalList.FindNode(this[i].X, this[i].Y);
        if (originalFound != null)
          this[i] = originalFound;
      }
    }

    //---------------------------------------------------------------------------------------------

    public void RemoveDiagonalCorners(PathNode parent, PathBuilder builder)
    {
      int count = this.Count;
      for (int i = count - 1; i >= 0; i--)
      {
        PathNode current = this[i];
        if (current.DiagonalToParent)
        {
          PathNodeList currentAdje = builder.FindAdjacentNodes(current);
          currentAdje.MergePossible(this);
          if (currentAdje.Count == 1 || currentAdje.Count == 0)
          {
            this.RemoveAt(i);
          }
        }
      }
    }

    //---------------------------------------------------------------------------------------------

    public PathNodeList FromPositionCollection(List<UOPositionBase> positions)
    {
      PathNodeList list = new PathNodeList();

      foreach (UOPositionBase position in positions)
      {
        PathNode node = new PathNode();
        node.X = position.X.GetValueOrDefault();
        node.Y = position.Y.GetValueOrDefault();
        node.Walkable = position.IsSepable;
        if (!list.ContainsNode(node))
          list.Add(node);
      }
      return list;
    }

    //---------------------------------------------------------------------------------------------

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    //---------------------------------------------------------------------------------------------

    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.ht != null) this.ht = null;
      }
    }

    //---------------------------------------------------------------------------------------------
  }
}
