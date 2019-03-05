using System;
using System.Collections.Generic;
using System.Text;

namespace Caleb.Library
{
  public class CalStatusMessage
  {
    public static ushort Val_InfoColor { get { return 0x0162; } }
    public static ushort Val_WarningColor { get { return 0x02b; } }
    public static ushort Val_ErrorColor { get { return 0x026; } }
    public static ushort Val_InvalidColor { get { return 0x03e5; } }

    protected string description;
    public string Description
    {
      get { return this.description; }
      set { this.description = value; }
    }

    protected CalStatusMessageType type = CalStatusMessageType.Info;
    public CalStatusMessageType Type
    {
      get { return this.type; }
      set { this.type = value; }
    }

    protected ushort? color;
    public ushort? Color
    {
      get 
      {
        if (this.color.HasValue)
          return this.color;
        else if (this.Type == CalStatusMessageType.Info)
          return CalStatusMessage.Val_InfoColor;
        else if (this.Type == CalStatusMessageType.Warning)
          return CalStatusMessage.Val_WarningColor;
        else if (this.Type == CalStatusMessageType.Error)
          return CalStatusMessage.Val_ErrorColor;
        else
          return CalStatusMessage.Val_InvalidColor;

      }
      set { this.color = value; }
    }
  }

  public enum CalStatusMessageType
  {
    Info = 1,
    Error = 2,
    Warning = 4
  }
}