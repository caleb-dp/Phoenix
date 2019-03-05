using System;
using System.Collections.Generic;
using System.Text;
using Caleb.Library.CAL;
using Caleb.Library.CAL.Business;
using Caleb.Library;
using Phoenix;

namespace Caleb.Library.CAL.Business
{
  public class UOItemTypeCategory : CalBusiness
  {
    protected string name = String.Empty;
    [Parameter("Name", System.Data.DbType.String)]
    [Require]
    [Unique]
    public string Name
    {
      get { this.EnsureLoad(); return this.name; }
      set { this.name = value; }
    }

    protected override void OnInit()
    {
      base.OnInit();

      this.DbPrimaryKey = "UOItemTypeCategoryID";
      this.DbTableName = "UO_UOItemTypeCategory";
    }
  }
}
