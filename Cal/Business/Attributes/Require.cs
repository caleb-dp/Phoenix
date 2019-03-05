using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Caleb.Library.CAL.Business
{
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
  public class Require : System.Attribute
  {
  }
}