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
  public interface IUOPosition 
  {
    //---------------------------------------------------------------------------------------------

   // IUOPosition CloneUntyped();

    //---------------------------------------------------------------------------------------------

    ushort? X { get; set; }
    ushort? Y { get; set; }
    ushort? Z { get; set; }
    double RealDistance(IUOPosition positionFrom);
    ushort Distance(IUOPosition positionFrom);
    double RealDistance();
    ushort Distance();
    string CommandName { get; set; }
    // bool? Stepable { get; set; }
    // bool IsSepable { get; }
    //bool TypeChecked { get; set; }
    //bool IsTree { get; set; }
    //ushort? GraphicNum { get; set; }
    //string TrackName { get; set; }
    //int TrackPosition { get; set; }
  }
}
