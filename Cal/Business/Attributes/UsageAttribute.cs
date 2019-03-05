using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Caleb.Library.CAL.Business
{
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
  public class UsageAttribute : System.Attribute
  {
    protected Usages usage = Usages.Load | Usages.Add | Usages.Save;
    public Usages Usage
    {
      get { return this.usage; }
      set { this.usage = value; }
    }
  }

  [FlagsAttribute]
  public enum Usages
  {
    None = 0,
    Load = 1,
    Add = 2,
    Save = 4
  }
}