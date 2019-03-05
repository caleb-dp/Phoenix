using System;
using System.Collections.Generic;
using System.Text;
using Caleb.Library.CAL;
using Caleb.Library.CAL.Business;

namespace Caleb.Library.CAL.Business
{
  public class UOItemTypeCollectionCategory : CalBusinessCollection<UOItemTypeCategory>
  {
    protected override void OnInit()
    {
      base.OnInit();
      this.itemType = typeof(UOItemTypeCategory);
    }
  }
}
