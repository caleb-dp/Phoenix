using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;
using Caleb.Library;
using Caleb.Library.CAL.Business;
using System.Data;
using Caleb.Library.CAL;
using System.Collections;
using System.Windows.Forms;

namespace CalExtension
{
  public class ResultInfo 
  {
    public bool Success { get; set; }
    public bool PrintMessages { get; set; }
    public UOItem Item { get; set; }
    public object ReturnValue { get; set; }
    private List<StatusMessage> statusMessages;
    public List<StatusMessage> StatusMessages
    {
      get
      {
        if (statusMessages == null)
          statusMessages = new List<StatusMessage>();
        return statusMessages;
      }
      set { statusMessages = value; }
    }

    public void AddMessage(string description, UOExtensions.MessageType messageType)
    {
      this.StatusMessages.Add(new StatusMessage() { Description = description, MessageType = messageType });
    }
  }

  public class StatusMessage
  {
    public string Description { get; set; }
    private CalExtension.UOExtensions.MessageType messageType = UOExtensions.MessageType.Info;
    public CalExtension.UOExtensions.MessageType MessageType
    {
      get { return messageType; }
      set { messageType = value; }
    }
  }
}

