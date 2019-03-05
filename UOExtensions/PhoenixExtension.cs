using System.Collections.Generic;
using System.Text;
using System.Linq;
using Phoenix;
using Phoenix.WorldData;
using Phoenix.Communication;
using Caleb.Library.CAL.Business;
using CalExtension;
using System;
using System.Threading;

namespace CalExtension.UOExtensions
{
  public enum MessageType
  {
    Info = 1,
    Error = 2,
    Warning = 4
  }

  public static class PhoenixExtension
  {
    //---------------------------------------------------------------------------------------------

    public static bool IsItemType(this UOItem obj, UOItemType type)
    {
      return type.Is(obj);
    }

    //---------------------------------------------------------------------------------------------

    public static bool IsValidCust(this Serial serial)
    {
      return serial.IsValid && (uint)serial > 0;
    }

    //---------------------------------------------------------------------------------------------

    public static bool ExistCust(this UOObject obj)
    {
      return obj.Exist && obj.Serial.IsValidCust();
    }

    //---------------------------------------------------------------------------------------------

    public static void PrintMessage(this UOObject obj, string message)
    {
      PrintMessage(obj, message, MessageType.Info);
    }

    //---------------------------------------------------------------------------------------------

    public static void PrintMessage(this UOObject obj, string message, MessageType type)
    {
      PrintMessage(obj, message, type, new object[0]);
    }

    //---------------------------------------------------------------------------------------------

    public static void PrintMessage(this UOObject obj, string message, MessageType type, params object[] args)
    {
      PrintMessage(obj, message, Game.GetMessageTypeColor(type), args);
    }

    //---------------------------------------------------------------------------------------------

    public static void PrintMessage(this UOObject obj, string message, UOColor color)
    {
      PrintMessage(obj, message, color, false);
    }

    //---------------------------------------------------------------------------------------------

    public static void PrintMessage(this UOObject obj, string message, UOColor color, bool supperssPrevMessages)
    {
      PrintMessage(obj, message, color, supperssPrevMessages, new object[0]);
    }

    //---------------------------------------------------------------------------------------------

    public static void PrintMessage(this UOObject obj, string message, UOColor color, params object[] args)
    {
      PrintMessage(obj, message, color, false, args);
    }

    //---------------------------------------------------------------------------------------------

    public static void PrintMessage(this UOObject obj, string message, UOColor color, bool supperssPrevMessages, params object[] args)
    {
      string test = message + String.Empty;
      bool needBranches = test.Contains("[");
      bool needSpaces = test.Contains("..") || test.Contains("..") || test.Contains("[ ");
      test = test.Replace("[", "").Replace("]", "").Replace("....", "").Replace("...", "").Replace("..", "").Trim(new char[] { ' ' });
      //.ToLower();
      //if (needBranches)
      //  test = test.ToLower();

      if (!String.IsNullOrEmpty(test) && needBranches)
        test = "[" + (needSpaces ? " " : "") + test + (needSpaces ? " " : "") + "]";

      if (obj.Serial.IsValidCust())
      {
        if (supperssPrevMessages)
        {
          UO.PrintObject(obj.Serial, color, "", args);//reset 1
          UO.PrintObject(obj.Serial, color, "", args);//reset 2
        }
        UO.PrintObject(obj.Serial, color, test, args);
      }
      else
        UO.Print(color, test, args);
    }

    //---------------------------------------------------------------------------------------------

    public static bool WaitForName(this UOObject obj)
    {
      int counter = 0;
      while (String.IsNullOrEmpty(obj.Name) && counter < 150)
      {
        Thread.Sleep(5);
        counter += 5;
      }

      return !String.IsNullOrEmpty(obj.Name);
    }

    //---------------------------------------------------------------------------------------------

    public static void PrintHitsMessage(this UOCharacter character, string message)
    {
      PrintHitsMessage(character, message, MessageType.Info);
    }

    //---------------------------------------------------------------------------------------------

    public static void PrintHitsMessage(this UOCharacter character, string message, params object[] args)
    {
      string test = message + String.Empty;
      bool needBranches = test.Contains("[");
      bool needSpaces = test.Contains("..") || test.Contains(".") || test.Contains("[ ");
      test = test.Replace("[", "").Replace("]", "").Replace("....", "").Replace("...", "").Replace("..", "").Replace(".", "").Trim(new char[] { ' ' });
      //.ToLower();
      //if (needBranches)
      //  test = test.ToLower();

      if (!String.IsNullOrEmpty(test) && needBranches)
        test = "[" + (needSpaces ? " " : "") + test + (needSpaces ? " " : "") + "]";

      UOColor color = Game.Val_GreenBlue;

      if (character.Notoriety == Notoriety.Murderer || character.Notoriety == Notoriety.Enemy)
        color = Game.GetEnemyColorByHits(character);
      else
        color = Game.GetAlieColorByHits(character);

      //character.Print("");//reset 1
      //character.Print("");//reset 2
      character.Print(color, test, args);
    }

    //---------------------------------------------------------------------------------------------

    public static UOItem FindColor(this UOPlayer player, UOColor color)
    {
      return FindColor(player, color, new Graphic[0]);
    }

    //---------------------------------------------------------------------------------------------

    public static UOItem FindColor(this UOPlayer player, UOColor color, params Graphic[] graphics)
    {
      UOItem searchItem = new UOItem(Serial.Invalid);

      List<UOItem> items = new List<UOItem>();
      items.AddRange(player.Backpack.AllItems.ToArray());
      
      foreach (UOItem item in World.Player.Layers)
        items.Add(item);

      foreach (UOItem item in items)
      {
        if (item.Color == color && (graphics.Length == 0 || Array.IndexOf(graphics, item.Graphic)>= 0))
        {
          searchItem = item;
          break;
        }

      }

      return searchItem;
    }

    //---------------------------------------------------------------------------------------------

    public static UOItem FindType(this UOPlayer player, Graphic graphic)
    {
      return FindType(player, graphic, UOColor.Invariant);
    }

    //---------------------------------------------------------------------------------------------

    public static UOItem FindType(this UOPlayer player, IUOItemType itemType)
    {
      return FindType(player, itemType.Graphic, itemType.Color);
    }

    //---------------------------------------------------------------------------------------------

    public static UOItem FindType(this ItemsCollection items, IUOItemType itemType)
    {
      if (!itemType.Color.IsConstant)
        return items.FindType(itemType.Graphic);
      else
        return items.FindType(itemType.Graphic, itemType.Color);
    }

    //---------------------------------------------------------------------------------------------
    public static UOItem FindType(this UOPlayer player, Graphic graphic, UOColor color)
    {
      return FindType(player, graphic, color, Serial.Invalid);
    }

    public static UOItem FindType(this UOPlayer player, Graphic graphic, UOColor color, Serial serial)
    {
      UOItem searchItem = new UOItem(Serial.Invalid);

      if (serial.IsValid)
      {
        List<UOItem> search = new List<UOItem>();
        search.AddRange(player.Backpack.AllItems.ToArray());
        search.AddRange(player.Layers.ToArray());

        foreach (UOItem itm in search)
        {
          if (itm.Serial.IsValid && itm.Exist && itm.Serial == serial)
          {
            searchItem = itm;
            break;
          }
        }
      }

      if (!searchItem.Serial.IsValidCust() || !searchItem.Exist)
      {
        searchItem = color.IsConstant ? player.Backpack.Items.FindType(graphic, color) : player.Backpack.Items.FindType(graphic);

        if (!searchItem.Serial.IsValidCust() || !searchItem.Exist)
        {
          searchItem = color.IsConstant ? player.Backpack.AllItems.FindType(graphic, color) : player.Backpack.AllItems.FindType(graphic);
          if (!searchItem.Serial.IsValidCust() || !searchItem.Exist)
          {
            searchItem = color.IsConstant ? player.Layers.FindType(graphic, color) : player.Layers.FindType(graphic);
          }
        }
      }
      return searchItem;
    }

    //---------------------------------------------------------------------------------------------

    public static UOItem CurrentWeapon(this UOPlayer player)
    {
      if (World.Player.Layers[Layer.RightHand].ExistCust())//jednorucka
        return World.Player.Layers[Layer.RightHand];
      else if (World.Player.Layers[Layer.LeftHand].ExistCust() && !World.Player.Layers[Layer.RightHand].ExistCust())//obourucka
        return World.Player.Layers[Layer.LeftHand];

      return new UOItem(Serial.Invalid);
    }

    //---------------------------------------------------------------------------------------------

    public static UOItem CurrentShield(this UOPlayer player)
    {
       if (World.Player.Layers[Layer.LeftHand].ExistCust() && ItemLibrary.Shields.GraphicArray.Where(i=> i ==  World.Player.Layers[Layer.LeftHand].Graphic).ToArray().Length > 0)//obourucka
        return World.Player.Layers[Layer.LeftHand];

      return new UOItem(Serial.Invalid);
    }

    //---------------------------------------------------------------------------------------------

    public static IUOPosition GetPosition(this UOObject obj)
    {
      return new UOPositionBase(obj.X, obj.Y, (ushort)obj.Z);
    }

    //---------------------------------------------------------------------------------------------

    public static double GetDistance(this UOObject obj)
    {
      return GetDistance(obj, World.Player);
    }

    //---------------------------------------------------------------------------------------------

    public static double GetDistance(this UOObject obj, UOObject distanceFrom)
    {
      return GetDistance(obj, distanceFrom.GetPosition());
    }

    //---------------------------------------------------------------------------------------------

    public static double GetDistance(this UOObject obj, IUOPosition positionFrom)
    {
      return Robot.GetRelativeVectorLength(positionFrom, obj.GetPosition());
    }

    //---------------------------------------------------------------------------------------------

    public static double GetRealDistance(this UOObject obj)
    {
      return GetRealDistance(obj, World.Player);
    }

    //---------------------------------------------------------------------------------------------

    public static double GetRealDistance(this UOObject obj, UOObject distanceFrom)
    {
      return GetRealDistance(obj, distanceFrom.GetPosition());
    }

    //---------------------------------------------------------------------------------------------

    public static double GetRealDistance(this UOObject obj, IUOPosition positionFrom)
    {
      return Robot.GetRealDistance(positionFrom, obj.GetPosition());
    }

    //---------------------------------------------------------------------------------------------

    public static string GetUniqueKey(this UOObject obj)
    {
      if (obj is UOCharacter)
        return obj.Serial + "|" + ((UOCharacter)obj).Model + "|" + obj.Color;
      else if (obj is UOItem)
        return obj.Serial + "|" + ((UOItem)obj).Graphic + "|" + obj.Color;
      else
        return obj.Serial + String.Empty;
    }

    //---------------------------------------------------------------------------------------------

    public static string GetUniqueKeyPostion(this UOObject obj)
    {
      if (obj is UOCharacter)
        return obj.Serial + "|" + ((UOCharacter)obj).Model + "|" + obj.Color + "|" + obj.X + "." + obj.Y;
      else if (obj is UOItem)
        return obj.Serial + "|" + ((UOItem)obj).Graphic + "|" + obj.Color + "|" + obj.X + "." + obj.Y;
      else
        return obj.Serial + String.Empty + "|" + obj.X + "." + obj.Y;
    }

    //---------------------------------------------------------------------------------------------

  }

}