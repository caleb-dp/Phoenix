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
  public class PathBuilder : IDisposable
  {
    //---------------------------------------------------------------------------------------------

    public PathNodeList PossibleNodes;
    private PathNodeList OpenNodes;
    private PathNodeList CloseNodes;

    public PathNodeList ComputedPathNodes
    {
      get
      {
        PathNodeList list = new PathNodeList();

        if (this.CloseNodes != null)
        {
          PathNode finish = null;
          foreach (PathNode closed in CloseNodes)
          {
            if (closed.Finish)
            {
              finish = closed;
              break;
            }
          }

          if (finish != null)
          {
            PathNode current = finish;
            while (current.Parent != null)
            {
              list.Add(current);
              current = current.Parent;
            }
          }
        }

        return list;
      }
    }

    //---------------------------------------------------------------------------------------------

    public PathBuilder()
    {
      PossibleNodes = new PathNodeList();
    }


    public PathBuilder(PathNodeList possibleNodes)
    {
      PossibleNodes = possibleNodes;
    }

    //---------------------------------------------------------------------------------------------

    public PathNodeList FindAdjacentNodes(PathNode node)
    {
      PathNodeList list = new PathNodeList();
      PathNode adjNode = new PathNode() { X = (ushort)(node.X), Y = (ushort)(node.Y + 1) };//0
      list.Add(adjNode);
      adjNode = new PathNode() { X = (ushort)(node.X + 1), Y = (ushort)(node.Y + 1), DiagonalToParent = true };//45
      list.Add(adjNode);
      adjNode = new PathNode() { X = (ushort)(node.X + 1), Y = (ushort)(node.Y) };//90
      list.Add(adjNode);
      adjNode = new PathNode() { X = (ushort)(node.X + 1), Y = (ushort)(node.Y - 1), DiagonalToParent = true };//135
      list.Add(adjNode);
      adjNode = new PathNode() { X = (ushort)(node.X), Y = (ushort)(node.Y - 1) };//180
      list.Add(adjNode);
      adjNode = new PathNode() { X = (ushort)(node.X - 1), Y = (ushort)(node.Y - 1), DiagonalToParent = true };//225
      list.Add(adjNode);
      adjNode = new PathNode() { X = (ushort)(node.X - 1), Y = (ushort)(node.Y) };//270
      list.Add(adjNode);
      adjNode = new PathNode() { X = (ushort)(node.X - 1), Y = (ushort)(node.Y + 1), DiagonalToParent = true };//315
      list.Add(adjNode);
      return list;
    }

    //---------------------------------------------------------------------------------------------

    private int ComputeH(PathNode node, PathNode toNode)
    {
      return (Math.Abs((node.X - toNode.X)) + Math.Abs((node.Y- toNode.Y))) * 10;
    }

    //---------------------------------------------------------------------------------------------

    private int ComputeG(PathNode parent, PathNode child)
    {
      return parent.G + (child.DiagonalToParent ? 14 : 10);//(Math.Abs((node.X - toNode.X)) + Math.Abs((node.Y - toNode.Y))) * 10;
    }

    //---------------------------------------------------------------------------------------------

    public int Searchs = 0;
    //---------------------------------------------------------------------------------------------
    public bool ComputePath(PathNode start, PathNode end)
    {
      return ComputePath(start, end, 0);
    }

    public bool ComputePath(PathNode start, PathNode end, int fromDistance)
    {
      OpenNodes = new PathNodeList();
      CloseNodes = new PathNodeList();
      PathNode current = start;
      start.G = 0;
      start.H = ComputeH(start, end);

      if (!OpenNodes.ContainsNode(start))
        OpenNodes.Add(start);

      bool finish = false;
      bool result = false;

      if (!this.PossibleNodes.ContainsNode(start))
        this.PossibleNodes.Add(start);

      if (!this.PossibleNodes.ContainsNode(end))
        this.PossibleNodes.Add(end);

      while (!finish && Searchs < 10000)
      {
        Searchs++;
        OpenNodes.Remove(current);
        if (!CloseNodes.ContainsNode(current))
          CloseNodes.Add(current);

        PathNodeList searchNodes = FindAdjacentNodes(current);

        foreach (PathNode node in searchNodes)
        {
          if (!this.PossibleNodes.ContainsNode(node))
            this.PossibleNodes.Add(node);
        }

        searchNodes.MergePossible(this.PossibleNodes);
        searchNodes.RemoveDiagonalCorners(current, this);

        if (searchNodes.Count > 0)
          searchNodes.MergeIdent(CloseNodes);
        else
        {
          PathNodeList adjNodes = FindAdjacentNodes(current);
          foreach (PathNode adj in adjNodes)
          {
            PathNode sNode = this.PossibleNodes.FindNode(adj.X, adj.Y);
            if (sNode == null)
            {
              sNode = adj;
              this.PossibleNodes.Add(sNode);
            }
            sNode.Walkable = null;

            searchNodes.Add(sNode);
          }
          searchNodes.RemoveDiagonalCorners(current, this);
        }

        searchNodes.NormalizeReferences(OpenNodes);

        PathNode bestNode = null;
        foreach (PathNode searchNode in searchNodes)
        {
          if (!OpenNodes.ContainsNode(searchNode))
          {
            searchNode.Parent = current;
            searchNode.H = ComputeH(searchNode, end);
            searchNode.G = ComputeG(searchNode.Parent, searchNode);
            OpenNodes.Add(searchNode);
          }
          else if (searchNode.G > (current.G + (searchNode.DiagonalToParent ? 14 : 10)))
          {
            searchNode.Parent = current;
            searchNode.H = ComputeH(searchNode, end);
            searchNode.G = ComputeG(searchNode.Parent, searchNode);
          }
        }

        foreach (PathNode openNode in OpenNodes)
        {
          if (bestNode == null || (openNode.F < bestNode.F))
          {
            bestNode = openNode;
          }
        }

        if (bestNode == null)
        {
          finish = true;
        }
        else if (bestNode.H == (fromDistance * 10))
        {
          OpenNodes.Remove(bestNode);
          if (!CloseNodes.ContainsNode(bestNode))
            CloseNodes.Add(bestNode);

          bestNode.Finish = true;
          result = true;
          finish = true;
        }

        current = bestNode;
      }
      return result;
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
        if (this.OpenNodes != null)
        {
          this.OpenNodes.Dispose();
          this.OpenNodes = null;
        }

        if (this.CloseNodes != null)
        {
          this.CloseNodes.Dispose();
          this.CloseNodes = null;
        }
      }
    }

    //---------------------------------------------------------------------------------------------
  }
}
