using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Caleb.Library.CAL.Business;
using System.Data;
using System.Xml;
using System.Collections;

namespace Caleb.Library.CAL
{
  public abstract class CalBusiness : ICalBusiness, IEquatable<CalBusiness>//, IComparable<CalBusiness>
  {
    //---------------------------------------------------------------------------------------------

    public static int? ToID(object value)
    {
      return Utils.ToNullInt(value);
    }
    
    //---------------------------------------------------------------------------------------------

    protected int? id;
    public int? ID
    {
      get
      {
        return this.id;
      }
      set { this.id = value; }
    }

    //---------------------------------------------------------------------------------------------

    public CalBusiness()
    {
      this.OnInit();
    }

    //---------------------------------------------------------------------------------------------

    protected CalStatusMessageCollection statusMessages;
    public CalStatusMessageCollection StatusMessages
    {
      get
      {
        if (this.statusMessages == null) this.statusMessages = new CalStatusMessageCollection();
        return this.statusMessages;
      }
      set { this.statusMessages = value; }
    }

    //---------------------------------------------------------------------------------------------

    public bool IsValid
    {
      get { return this.StatusMessages.IsValid; }
    }

    //---------------------------------------------------------------------------------------------

    protected virtual void OnInit()
    {
    }

    //---------------------------------------------------------------------------------------------

    protected string dbPrimaryKey;
    public string DbPrimaryKey
    {
      get { return this.dbPrimaryKey; }
      set { this.dbPrimaryKey = value; }
    }

    //---------------------------------------------------------------------------------------------

    protected string dbTableName;
    public string DbTableName
    {
      get { return this.dbTableName; }
      set { this.dbTableName = value; }
    }

    //---------------------------------------------------------------------------------------------

    protected string dbViewNameForLoad;
    public string DbViewNameForLoad
    {
      get { return  String.IsNullOrEmpty(this.dbViewNameForLoad) ? this.dbTableName : this.dbViewNameForLoad; }
      set { this.dbViewNameForLoad = value; }
    }

    //---------------------------------------------------------------------------------------------

    //public virtual int CompareTo(CalBusiness other)
    //{
    //  return 0;
    //}

    //---------------------------------------------------------------------------------------------

    public virtual bool Equals(CalBusiness other)
    {
      if (this == other) return true;
      return false;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    //---------------------------------------------------------------------------------------------

    public override bool Equals(object obj)
    {
      return this.Equals((CalBusiness)obj);
    }

    //---------------------------------------------------------------------------------------------

    public Attribute GetAttribute(MemberInfo mInfo, Type attributeType)
    {
      Attribute[] attributes = Attribute.GetCustomAttributes(mInfo, true);
       foreach (Attribute attr in attributes)
       {
         if (attr.GetType() == attributeType) return attr;
       }
       return null;
    }

    //---------------------------------------------------------------------------------------------
    private static Hashtable propertyCache;

    private static PropertyInfo[] PropertyCache(Type type)
    {

      if (propertyCache == null)
      {
        propertyCache = new Hashtable();
      }

      if (propertyCache[type.AssemblyQualifiedName] as PropertyInfo[] == null)
      {
        propertyCache[type.AssemblyQualifiedName] = type.GetProperties();

      }
      return propertyCache[type.AssemblyQualifiedName] as PropertyInfo[];
    }

    //---------------------------------------------------------------------------------------------
    private static Hashtable parameterAttributeCache;

    private static ParameterAttribute ParameterAttributeCache(CalBusiness o, PropertyInfo pInfo)
    {
      if (parameterAttributeCache == null)
      {
        parameterAttributeCache = new Hashtable();
      }

      string key = o.GetType().AssemblyQualifiedName + "|" + pInfo.Name;

      if (parameterAttributeCache[key] as ParameterAttribute == null)
      {
        ParameterAttribute attrParam = (ParameterAttribute)o.GetAttribute(pInfo, typeof(ParameterAttribute));
        parameterAttributeCache[key] = attrParam == null ? new ParameterAttribute("__None") : attrParam;

      }
      return parameterAttributeCache[key] as ParameterAttribute;
    }


    //---------------------------------------------------------------------------------------------

    public void FillFromDataRow(DataRow row)
    {
      PropertyInfo[] props = PropertyCache(this.GetType());

      foreach (PropertyInfo pInfo in props)
      {
        ParameterAttribute attrParam = ParameterAttributeCache(this, pInfo);//(ParameterAttribute)this.GetAttribute(pInfo, typeof(ParameterAttribute));

        if (attrParam != null && attrParam.Name != "__None")
        {
          Usages defaultUsage = Usages.Add | Usages.Load | Usages.Save;

          UsageAttribute usage = (UsageAttribute)this.GetAttribute(pInfo, typeof(UsageAttribute));
          if (usage != null) defaultUsage = usage.Usage;

          if ((defaultUsage & Usages.Load) == Usages.Load)
          {
            object value =  (row.Table.Columns.Contains(attrParam.Name)) ? Utils.DBNullToNull(row[attrParam.Name]) : null;

            if (pInfo.PropertyType.GetInterface("Caleb.Library.CAL.ICalBusiness") != null)
            {
              ICalBusiness business = (ICalBusiness)pInfo.GetValue(this, null);
              business.ID = CalBusiness.ToID(value);
            }
            else
            {
              pInfo.SetValue(this, value, null);
            }
          }
        }
      }
    }

    //---------------------------------------------------------------------------------------------

    protected bool needLoad = true;

    //---------------------------------------------------------------------------------------------

    public void EnsureLoad()
    {
      if (this.needLoad) this.Load();
    }

    //---------------------------------------------------------------------------------------------

    public bool Load()
    {
      if (this.ID.HasValue)
      {
        DataTable dt = Cal.Engine.GetDataTable(this.DbViewNameForLoad);
        DataRow row = dt.Rows.Find(this.ID); //dt.Select(this.DbPrimaryKey + "=" + this.ID);
        if (row != null)
        {
          this.needLoad = false;
          this.FillFromDataRow(row);
          return true;
        }
      }
      return false;
    }

    //---------------------------------------------------------------------------------------------

    public bool LoadByCondition(string condition)
    {
      DataTable dt = Cal.Engine.GetDataTable(this.DbViewNameForLoad);
      if (dt == null || dt.Columns.Count == 0 || dt.Rows.Count == 0) return false;
      DataRow[] rows = dt.Select(condition);
      if (rows != null && rows.Length > 0)
      {
        this.needLoad = false;
        DataRow row = rows[0];
        this.ID = CalBusiness.ToID(row[this.DbPrimaryKey]);
        this.FillFromDataRow(row);
        return true;
      }
      return false;
    }

    //---------------------------------------------------------------------------------------------

    public bool Save()
    {
      if (this.ID.HasValue)
      {
        this.Validate();
        if (!this.IsValid) return false;
        DataTable dt = Cal.Engine.GetDataTable(this.DbViewNameForLoad);
        DataRow row = dt.Rows.Find(this.ID);
        if (row != null)
        {
          this.FillToDataRow(row);
          this.needLoad = true;
          return true;
        }
        this.StatusMessages.Clear();
      }
      else return this.Add();

      return false;
    }

    //---------------------------------------------------------------------------------------------

    public bool Remove()
    {
      if (this.ID.HasValue)
      {
        DataTable dt = Cal.Engine.GetDataTable(this.DbViewNameForLoad);
        DataRow row = dt.Rows.Find(this.ID);
        if (row != null)
        {
          dt.Rows.Remove(row);
          this.ID = null;

          return true;
        }
      }

      return false;
    }

    //---------------------------------------------------------------------------------------------

    public bool Add()
    {
      if (!this.ID.HasValue)
      {
        this.Validate();
        if (!this.IsValid) return false;
        DataTable dt = Cal.Engine.GetOtSetDataTable(this.DbViewNameForLoad);
        DataRow row = null;
        DataColumn primaryKey = null;
        if (!dt.Columns.Contains(this.DbPrimaryKey)) dt.Columns.Add(this.DbPrimaryKey, typeof(Int32));
        primaryKey = dt.Columns[this.DbPrimaryKey];
        primaryKey.AutoIncrement = true;
        primaryKey.AutoIncrementStep = 1;
        primaryKey.AutoIncrementSeed = 1;
        primaryKey.Unique = true;
        dt.PrimaryKey = new DataColumn[] { primaryKey };
        row = dt.NewRow();
        if ((row = FillToDataRow(row)) != null)
        {
          dt.Rows.Add(row);
          this.ID = CalBusiness.ToID(row[this.DbPrimaryKey]);
        }
        this.StatusMessages.Clear();
        return true;
      }
      return false;
    }

    //---------------------------------------------------------------------------------------------

    public DataRow FillToDataRow(DataRow row)
    {
      foreach (PropertyInfo pInfo in this.GetType().GetProperties())
      {
        ParameterAttribute attrParam = (ParameterAttribute)this.GetAttribute(pInfo, typeof(ParameterAttribute));
        if (attrParam != null)
        {
          Usages defaultUsage = Usages.Add | Usages.Save | Usages.Load;
          UsageAttribute usage = (UsageAttribute)this.GetAttribute(pInfo, typeof(UsageAttribute));
          if (usage != null) defaultUsage = usage.Usage;
          if ((defaultUsage & Usages.Add) == Usages.Add)
          {
            bool isBusiness = pInfo.PropertyType.GetInterface("Caleb.Library.CAL.ICalBusiness") != null;
            Type type = pInfo.PropertyType;
            bool isGenericNullable = pInfo.PropertyType.IsGenericType && pInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>);
            type = isGenericNullable ? Nullable.GetUnderlyingType(pInfo.PropertyType) : pInfo.PropertyType;
            if (isBusiness) type = typeof(Int32);
            if (!row.Table.Columns.Contains(attrParam.Name)) { row.Table.Columns.Add(attrParam.Name, type); }
            if (isBusiness)
            {
              ICalBusiness business = (ICalBusiness)pInfo.GetValue(this, null);
              row[attrParam.Name] = Utils.ToDBDataNullValue(business.ID);
            }
            else
            {
              row[attrParam.Name] = Utils.ToDBDataNullValue(pInfo.GetValue(this, null));
            }
          }
        }
      }
      return row;
    }

    //---------------------------------------------------------------------------------------------

    protected virtual void Validate()
    {
      this.ValidateUnique();
      this.ValidateRequire();
    }

    //---------------------------------------------------------------------------------------------

    protected virtual void ValidateUnique()
    {
      foreach (PropertyInfo pInfo in this.GetType().GetProperties())
      {
        ParameterAttribute attrParam = (ParameterAttribute)this.GetAttribute(pInfo, typeof(ParameterAttribute));
        if (attrParam != null)
        {
          Unique attr = (Unique)this.GetAttribute(pInfo, typeof(Unique));
          if (attr != null)
          {
            string strVal = "";
            if (pInfo.PropertyType.GetInterface("Caleb.Library.CAL.ICalBusiness") != null)
              strVal = ((ICalBusiness)pInfo.GetValue(this, null)).ID + "";
            else
              strVal = pInfo.GetValue(this, null) + "";

            if (!this.IsDistinctByParam(attrParam.Name, strVal))
              this.StatusMessages.Add(CalStatusMessageType.Error, this.GetType().Name + " - " + attrParam.Name + " musi byt unikatni!");
          }
        }
      }
    }

    //---------------------------------------------------------------------------------------------

    protected virtual void ValidateRequire()
    {
      foreach (PropertyInfo pInfo in this.GetType().GetProperties())
      {
        ParameterAttribute attrParam = (ParameterAttribute)this.GetAttribute(pInfo, typeof(ParameterAttribute));
        if (attrParam != null)
        {
          Require attr = (Require)this.GetAttribute(pInfo, typeof(Require));
          if (attr != null)
          {
            object value = null;
            if (pInfo.PropertyType.GetInterface("Caleb.Library.CAL.ICalBusiness") != null)
              value = ((ICalBusiness)pInfo.GetValue(this, null)).ID;
            else value = pInfo.GetValue(this, null);
            if (value == null)
              this.StatusMessages.Add(CalStatusMessageType.Error, this.GetType().Name + " - " + attrParam.Name + " je povinny!");
          }
        }
      }
    }


    //---------------------------------------------------------------------------------------------

    protected bool IsDistinctByParam(string param, string value)
    {
      if (String.IsNullOrEmpty(value)) return true;
      DataTable dt = Cal.Engine.GetOtSetDataTable(this.DbViewNameForLoad);
      if (dt.Columns.Count == 0) return true;
      string cond = "";
      if (this.ID.HasValue) cond = this.DbPrimaryKey + " <> " + this.ID + " AND ";
      cond += param + "='" + value + "'";
      return dt.Select(cond).Length == 0;
    }

    //---------------------------------------------------------------------------------------------

    public bool IsDistinctByCondition(string condition)
    {
      DataTable dt = Cal.Engine.GetOtSetDataTable(this.DbViewNameForLoad);
      if (dt.Columns.Count == 0) return true;
      if (this.ID.HasValue) condition += " AND " + this.DbPrimaryKey + " <> " + this.ID;
      return dt.Select(condition).Length == 0;
    }

    //---------------------------------------------------------------------------------------------
  }
}