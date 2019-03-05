using Phoenix;
using Phoenix.Communication.Packets;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CalExtension.UOExtensions
{
  public class Utils
  {
    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void PrintCursor()
    {
      Game.PrintMessage("[" + System.Windows.Forms.Cursor.Position.X + ":" + System.Windows.Forms.Cursor.Position.Y + "]");
    }
    //---------------------------------------------------------------------------------------------

    public static int? ToNullInt(object value)
    {
      if (value == null) return null;
      try { return Int32.Parse(value.ToString()); }
      catch { return null; }
    }

    //---------------------------------------------------------------------------------------------

    public static ushort? ToNullUshort(object value)
    {
      if (value == null) return null;
      try { return ushort.Parse(value.ToString()); }
      catch { return null; }
    }

    //---------------------------------------------------------------------------------------------

    public static sbyte? ToNullSbyte(object value)
    {
      if (value == null) return null;
      try { return sbyte.Parse(value.ToString()); }
      catch { return null; }
    }

    //---------------------------------------------------------------------------------------------

    public static uint? ToNullUInt(object value)
    {
      if (value == null) return null;
      try { return uint.Parse(value.ToString()); }
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

    public static string FormatXml(XmlElement xmlEl)
    {
      System.IO.MemoryStream ms = new System.IO.MemoryStream();
      XmlTextWriter xmlw = new XmlTextWriter(ms, System.Text.Encoding.UTF8);
      xmlw.Formatting = Formatting.Indented;
      xmlEl.WriteTo(xmlw);
      xmlw.Flush();
      ms.Seek(0, System.IO.SeekOrigin.Begin);
      System.IO.StreamReader sr = new System.IO.StreamReader(ms, System.Text.Encoding.UTF8);
      string configData = sr.ReadToEnd();
      xmlw.Close();
      sr.Close();
      ms.Close();
      return configData;
    }

    //---------------------------------------------------------------------------------------------
  }
}