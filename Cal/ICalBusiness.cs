using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Caleb.Library.CAL
{
  public interface ICalBusiness 
  {
    int? ID { get; set; }
    string DbPrimaryKey { get; set; }
    string DbTableName { get; set; }
    string DbViewNameForLoad { get; set; }
    bool Load();
    bool Save();
    bool Add();
    DataRow FillToDataRow(DataRow row);
    void FillFromDataRow(DataRow row);
  }
}