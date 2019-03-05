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
  public class PathNode
  {
    public ushort X;
    public ushort Y;
    public int G;
    public int H;
    public int F { get { return G + H; } }
    public bool? Walkable;
    public PathNode Parent;
    public bool DiagonalToParent = false;
    public bool Finish = false;

    public override string ToString()
    {
      return this.X + "," + this.Y;
    }
  }
}
