using Phoenix;
using Phoenix.Communication.Packets;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CalExtension.UOExtensions
{
  [RuntimeObject]
  public class IngameConsoleOverride
  {
    //---------------------------------------------------------------------------------------------

    public static event EventHandler AsciiSpeechBeforeOverride;

    //---------------------------------------------------------------------------------------------

    [ServerMessageHandler(0x1C)]
    public CallbackResult OnAsciiSpeech(byte[] data, CallbackResult prevState)
    {
      AsciiSpeech packet = new AsciiSpeech(data);

      if (AsciiSpeechBeforeOverride != null)
        AsciiSpeechBeforeOverride(packet, new EventArgs());

      ConsoleOverrideItem item = GetOverrideItem(packet);

      if (item != null)
      {
        item.Print();
        return item.CallbackResult;
      }

      return CallbackResult.Normal;
    }

    //---------------------------------------------------------------------------------------------

    protected ConsoleOverrideItem GetOverrideItem(AsciiSpeech packet)
    {
      XmlDocument doc = CalebConfig.GlobalDocument;

      if (doc != null)
      {
        XmlElement items = doc.DocumentElement.SelectSingleNode("IngameConsoleOverride/Items") as XmlElement;
        if (items != null && items != null)
        {
          foreach (XmlNode node in items.ChildNodes)
          {
            XmlElement item = node as XmlElement;
            if (item == null)
              continue;

            ConsoleOverrideItem oItem = new ConsoleOverrideItem();

            string attrValue = item.GetAttribute("CompareType");
            if (!String.IsNullOrEmpty(attrValue))
              oItem.CompareType = attrValue;

            attrValue = item.GetAttribute("PrintType");
            if (!String.IsNullOrEmpty(attrValue))
              oItem.PrintType = attrValue;

            attrValue = item.GetAttribute("CallbackResult");
            if (!String.IsNullOrEmpty(attrValue))
            {
              try { oItem.CallbackResult = (CallbackResult)Enum.Parse(typeof(CallbackResult), attrValue); }
              catch { }
            }

            attrValue = item.GetAttribute("PrintColor");
            if (!String.IsNullOrEmpty(attrValue))
            {
              if (attrValue.StartsWith("Color_"))
              {
                oItem.PrintColor = CalebConfig.GetUOColorOrDefault(attrValue, oItem.PrintColor);
              }
              else
              {

                try { oItem.PrintColor = UOColor.Parse(attrValue); }
                catch { }
              }
            }

            attrValue = item.GetAttribute("IgnoreCase");
            if (!String.IsNullOrEmpty(attrValue))
            {
              try { oItem.IgnoreCase = Boolean.Parse(attrValue); }
              catch { }
            }

            attrValue = item.GetAttribute("Active");
            if (!String.IsNullOrEmpty(attrValue))
            {
              try { oItem.Active = Boolean.Parse(attrValue); }
              catch { }
            }

            oItem.Text = item.GetAttribute("Text");
            oItem.AlterText = item.GetAttribute("AlterText");

            string itemText = oItem.Text + String.Empty;
            string compareText = packet.Text + String.Empty;

            if (oItem.IgnoreCase)
            {
              itemText = itemText.ToLower();
              compareText = compareText.ToLower();
            }

            bool success = false;
            if (oItem.CompareType == "Equal")
              success = itemText == compareText;
            else if (oItem.CompareType == "Contains")
              success = compareText.Contains(itemText);
            else if (oItem.CompareType == "StartsWith")
              success = compareText.StartsWith(itemText);
            else if (oItem.CompareType == "EndsWith")
              success = compareText.EndsWith(itemText);

            if (success && oItem.Active)
              return oItem;

          }
        }
        else if (Game.Debug)
          Notepad.WriteLine("GetOverrideItem !/IngameConsoleOverride/Items");

      }


      return null;
    }


    //---------------------------------------------------------------------------------------------

  }

  public class ConsoleOverrideItem
  {
    public CallbackResult CallbackResult = CallbackResult.Normal;
    public string Text = String.Empty;
    public string AlterText = String.Empty;
    public string CompareType = "Equal";
    public bool IgnoreCase = false;
    public bool Active = true;
    public string PrintType = "Console";
    public UOColor PrintColor = Game.Val_PureWhite;

    public void Print()
    {
      if (!String.IsNullOrEmpty(AlterText))
      {
        if (PrintType == "Player")
        {
          World.Player.PrintMessage(AlterText, PrintColor);
        }
        else if (PrintType == "Console")
        {
          Game.PrintMessage(AlterText, PrintColor);
        }

      }

    }
  }


}