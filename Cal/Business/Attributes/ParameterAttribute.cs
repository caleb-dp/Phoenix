using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Caleb.Library.CAL.Business
{
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
  public class ParameterAttribute : System.Attribute
  {
    public ParameterAttribute(string name)
    {
      this.name = name;
    }

    //---------------------------------------------------------------------------------------------

    public ParameterAttribute(string name, DbType dbType)
    {
      this.name = name;
      this.dbType = dbType;
    }

    //---------------------------------------------------------------------------------------------

    protected DbType dbType;
    public DbType DbType
    {
      get { return this.dbType; }
    }

    //---------------------------------------------------------------------------------------------
    protected string name;
    public string Name
    {
      get { return this.name; }
    }

    //---------------------------------------------------------------------------------------------
    protected int size;
    public int Size
    {
      get { return this.size; }
      set { this.size = value; }
    }
  }
}