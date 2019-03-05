using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using Phoenix;

namespace Caleb.Library
{
  public class Cal
  {
    protected DataSet dataBase;
    public DataSet DataBase
    {
      get 
      {
        if (dataBase == null)
        {
          dataBase = new DataSet("UODataBase");
        }
        return dataBase;
      }
      set { dataBase = value; }
    }

    public DataTable GetDataTable(string tableName)
    {
      return Cal.Engine.DataBase.Tables[tableName];
    }

    public DataTable GetOtSetDataTable(string tableName)
    {
      DataTable dt = null;
      if ((dt = this.GetDataTable(tableName)) == null)
      {
        dt = new DataTable(tableName);
        Cal.Engine.DataBase.Tables.Add(dt);
      }
      return dt;
    }

    public void SaveDataBase()
    {
      try
      {
        if (!Directory.Exists(Cal.DataBasePath))
          Directory.CreateDirectory(Cal.DataBasePath);

        //using (MemoryStream ms = new MemoryStream())
        //{
        //  this.DataBase.WriteXml(ms, XmlWriteMode.WriteSchema);
        //}

        this.DataBase.WriteXml(this.DataBaseFullPath, XmlWriteMode.WriteSchema);
      }
      catch
      { }
    }

    public string DataBaseFullPath = Path.Combine(DataBasePath, DataBaseName);

    public static string DataBasePath = Core.Directory;
    public static string DataBaseName = "UODataBase.xml";
    public static string DataBaseBakName = "UODataBase.xml.backup";    

    public static void InitEngine()
    {
      Cal engine = new Cal();
      string bakPath = Path.Combine(DataBasePath, DataBaseBakName);

      if (File.Exists(engine.DataBaseFullPath))
      {
        try
        {
          engine.DataBase.ReadXml(engine.DataBaseFullPath);

          try
          {
            File.Copy(engine.DataBaseFullPath, bakPath, true);
          }
          catch {  }
        }
        catch 
        {

          if (File.Exists(bakPath))
          {
            try
            {
              engine.DataBase.ReadXml(bakPath);
            }
            catch { }
          }
        }
      }

      Engine = engine;
    }

    public static Cal Engine = null;
  }
}