using System;
using System.Collections.Generic;
using System.Text;

namespace Caleb.Library
{
  public class Utils
  {
    //---------------------------------------------------------------------------------------------

    public static object ToDBDataNullValue(object value)
    {
      if (value == null) return DBNull.Value;
      return value;
    }

    //---------------------------------------------------------------------------------------------

    public static object DBNullToNull(object value)
    {
      if (value is DBNull) return null;
      return value;
    }

    //---------------------------------------------------------------------------------------------

    public static int? ToNullInt(object value)
    {
      if (value == null) return null;
      try { return Int32.Parse(value.ToString()); }
      catch { return null; }
    }

    //---------------------------------------------------------------------------------------------

    public static bool? ToNullBool(object value)
    {
      try { return bool.Parse(value.ToString()); }
      catch { return null; }
    }

    //---------------------------------------------------------------------------------------------

    public static string JoinDelimitedString(string text, string joinString, string delimiter)
    {
      if (String.IsNullOrEmpty(text)) 
      {
        if (String.IsNullOrEmpty(joinString)) return String.Empty;
        else return joinString;
      }
      else return text + delimiter + joinString;
    }

    //---------------------------------------------------------------------------------------------

    public static int GetSwitchIndex(int selected, int direction, int maxCount)
    {
      int index = selected + direction;
      if (index > maxCount - 1) index = 0;
      else if (index < 0) index = maxCount - 1;
      return index;
    }

    //---------------------------------------------------------------------------------------------
  }
}