using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;

namespace Caleb.Library.CAL
{
  public interface ICalBusinessCollection<T> : IList<T> where T : ICalBusiness
  {
  }
}