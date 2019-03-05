using System;
using System.Collections.Generic;
using System.Text;

namespace Caleb.Library
{
  public class CalStatusMessageCollection : List<CalStatusMessage>
  {
    public bool IsValid
    {
      get
      {
        foreach (CalStatusMessage msg in this)
          if (msg.Type == CalStatusMessageType.Error || msg.Type == CalStatusMessageType.Warning)
            return false;
        return true;
      }
    }

    public void Add(string description)
    {
      CalStatusMessage msg = new CalStatusMessage();
      msg.Type = CalStatusMessageType.Info;
      msg.Description = description;
      this.Add(msg);
      if (this.AddItem != null) this.AddItem(msg, new EventArgs());
    }

    public void Add(CalStatusMessageType type, string description)
    {
      CalStatusMessage msg = new CalStatusMessage();
      msg.Type = type;
      msg.Description = description;
      this.Add(msg);
      if (this.AddItem != null) this.AddItem(msg, new EventArgs());
    }

    public void Add(CalStatusMessageType type, string description, ushort color)
    {
      CalStatusMessage msg = new CalStatusMessage();
      msg.Type = type;
      msg.Description = description;
      msg.Color = color;
      this.Add(msg);
      if (this.AddItem != null) this.AddItem(msg, new EventArgs());
    }

    public void Add(string description, ushort color)
    {
      CalStatusMessage msg = new CalStatusMessage();
      msg.Description = description;
      msg.Color = color;
      this.Add(msg);
      if (this.AddItem != null) this.AddItem(msg, new EventArgs());
    }

    public event EventHandler AddItem;
  }
}