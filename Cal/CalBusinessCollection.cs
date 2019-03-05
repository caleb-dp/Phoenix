using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Caleb.Library.CAL
{
  public abstract class CalBusinessCollection<T> : List<T> , ICalBusinessCollection<T> where T : ICalBusiness 
  {
    public CalBusinessCollection()
    {
      this.OnInit();
    }

    protected virtual void OnInit()
    {
    }

    protected T instanceOfItemType;
    public T InstanceOfItemType
    {
      get
      {
        if (this.instanceOfItemType == null)
        {
          this.instanceOfItemType = this.CreateInstanceOfItemType();
        }
        return this.instanceOfItemType;
      }
    }

    public T CreateInstanceOfItemType()
    {
      if (this.itemType != null)
        return (T)Activator.CreateInstance(this.itemType);
      else
        return (T)Activator.CreateInstance<T>();

    }

    protected System.Type itemType;
    public System.Type ItemType
    {
      get { return this.itemType; }
      set { this.itemType = value; }
    }

    protected CalBusiness owner;
    public CalBusiness Owner
    {
      get { return this.owner; }
      set { this.owner = value; }
    }

    public CalBusinessCollection<T> Load()
    {
      return this.Load("");
    }

    public CalBusinessCollection<T> Load(string where)
    {
      DataTable dt = Cal.Engine.DataBase.Tables[this.InstanceOfItemType.DbViewNameForLoad];
      if (dt != null)
      {
        if (String.IsNullOrEmpty(where)) return this.Load(dt.Rows);
        else return this.Load(dt.Select(where));
      }
      return this;
    }

    public CalBusinessCollection<T> Load(DataRowCollection rows)
    {

      DataRow[] drows = new DataRow[rows.Count];
      for (int i = 0; i < rows.Count; i++)
      {
        drows[i] = rows[i];
      }
      return this.Load(drows);
    }

    public CalBusinessCollection<T> Load(DataRow[] rows)
    {
      if (rows == null) return null;

      foreach (DataRow row in rows)
      {
        T business = this.CreateInstanceOfItemType();
        business.ID = (int?)row[business.DbPrimaryKey];
        if (business.ID.HasValue)
          business.FillFromDataRow(row);

        this.Add(business);
      }
      return this;
    }

    public new virtual void Add(T item)
    {
      base.Add(item);
    }

    public virtual bool AddAndSave(T business)
    {
      bool success = business.Save();
      this.Add(business);
      return success;
    }
  }
}

