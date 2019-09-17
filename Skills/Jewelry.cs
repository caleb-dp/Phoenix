using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;
using Caleb.Library;
using System.Threading;
using CalExtension.Skills;
using CalExtension.UOExtensions;
using Caleb.Library.CAL.Business;

namespace CalExtension.Skills
{
  public class Jewelry : Skill
  {
    public static UOItemType RR = new UOItemType() { Graphic = 0x108A, Color = 0x0496 };
    public static UOItemType GRR = new UOItemType() { Graphic = 0x108A, Color = 0x0B21 };
    public static UOItemType GRR2 = new UOItemType() { Graphic = 0x108A, Color = 0x0B98 };
    public static UOItemType GGR = new UOItemType() { Graphic = 0x108A, Color = 0x0000 };
    public static UOItemType HoDF = new UOItemType() { Graphic = 0x136C, Color = 0x0B89 };
    //0x1086 Bracelet
    //0x136C Heart of dark fortress
    //0x1088 Necklace
    public static bool IsJewelry(UOItem item)
    {
      return item.Graphic == 0x108A || item.Graphic == 0x1086 || item.Graphic == 0x1087 || item.Graphic == 0x1088 || item.Graphic == 0x136C;
    }

    //---------------------------------------------------------------------------------------------

    [Command]
    public static void SetridSperky()
    {
      SetridSperky(World.Player.Backpack);
    }

    [Command]
    public static void SetridSperky(UOItem container)
    {
      SetridSperky(container, 20, 45);
    }

    [Command]
    public static void SetridSperky(UOItem container, ushort x, ushort y)//TODO ostani
    {
      Game.PrintMessage("SetridSperky - start");
      foreach (UOItem item in container.Items)
      {
        if (
            item.Graphic == RR.Graphic && item.Color == RR.Color ||
            item.Graphic == GRR.Graphic && item.Color == GRR.Color ||
            item.Graphic == GRR2.Graphic && item.Color == GRR2.Color ||
            item.Graphic == GGR.Graphic && item.Color == GGR.Color ||
            item.Graphic == 0x1088 || item.Graphic == 0x1086 || item.Graphic == 0x0FC7 //medailon golden
          )
        {
          item.Move(1, World.Player.Backpack, x, y);
          Game.Wait();
          x += 10;
        }
      }

      Game.PrintMessage("SetridSperky - konec");
    }

    //---------------------------------------------------------------------------------------------
    private List<Serial> usedGreatGoldRing = new List<Serial>();
    public bool UseGreatGoldRing()
    {
      return UseJewlery("GGR", 0x108A, 0x0000, Layer.Neck, ref usedGreatGoldRing).Success;
    }

    //---------------------------------------------------------------------------------------------
    //Serial: 0x40093F46  Name: "Heart of Dark Forest (0 charg"  Position: 119.144.0  Flags: 0x0020  Color: 0x0B89  Graphic: 0x136C  Amount: 1  Layer: None  Container: 0x4038402B
    UseJewleryInfo lastRRInfo;

    public bool UseReflexRing()
    {
      List<string> types = new List<string>();

      if (World.Player.Backpack.AllItems.FindType(GRR.Graphic, GRR.Color).Exist)
        types.Add("GRR");
      if (World.Player.Backpack.AllItems.FindType(GRR2.Graphic, GRR2.Color).Exist)
        types.Add("GRR2");
      if (World.Player.Backpack.AllItems.FindType(RR.Graphic, RR.Color).Exist)
        types.Add("RR");
      if (World.Player.Backpack.AllItems.FindType(HoDF.Graphic, HoDF.Color).Exist)
        types.Add("HoDF");


      string useType = "GRR";
      Graphic g = GRR.Graphic;
      UOColor c = GRR.Color;

      int index = 0;
      for (int i = 0; i < types.Count; i++)
      {
        if (lastRRInfo != null && lastRRInfo.Title == types[i] && i < types.Count - 1)
        {
          index = i + 1;
          break;
        }
      }

      if (types.Count > index)
        useType = types[index];

      if (useType == "GRR")
        c = GRR.Color;
      else if (useType == "GRR2")
        c = GRR2.Color;
      else if (useType == "HoDF")
      {
        g = HoDF.Graphic;
        c = HoDF.Color;
      }
      else
        c = RR.Color;

      lastRRInfo = UseJewlery(useType, g, c, "Ring");

      return lastRRInfo.Success;
    }

    //---------------------------------------------------------------------------------------------

    private List<Serial> usedDiamondBracelet = new List<Serial>();
    public bool UseDiamondBracelet()
    {
      return UseJewlery("DB", 0x1086, 0x0000, Layer.Neck, ref usedDiamondBracelet).Success;
    }

    //---------------------------------------------------------------------------------------------

    private List<Serial> lastUsedNeklak = new List<Serial>();
    public bool UseTitanNeklak()
    {
      return UseJewlery("TN", 0x1088, 0x0485, Layer.Neck, ref lastUsedNeklak).Success;
    }

    //---------------------------------------------------------------------------------------------

    private Dictionary<string, List<Serial>> universalUsedList;
    private Dictionary<string, List<Serial>> UniversalUsedList
    {
      get
      {
        if (this.universalUsedList == null)
          this.universalUsedList = new Dictionary<string, List<Serial>>();
        return this.universalUsedList;
      }
    }

    //---------------------------------------------------------------------------------------------

    private Dictionary<string, List<Serial>> universalOutOfCharges;
    private Dictionary<string, List<Serial>> UniversalOutOfCharges
    {
      get
      {
        if (this.universalOutOfCharges == null)
          this.universalOutOfCharges = new Dictionary<string, List<Serial>>();
        return this.universalOutOfCharges;
      }
    }

    //---------------------------------------------------------------------------------------------

    public UseJewleryInfo UseJewlery(string title, Graphic graphic, UOColor color, string layerStr)
    {
      Layer layer = (Layer)Enum.Parse(typeof(Layer), layerStr);
      List<Serial> usedList = new List<Serial>();
      string key = graphic.ToString() + "|" + color.ToString();

      if (UniversalUsedList.ContainsKey(key))
        usedList = UniversalUsedList[key];
      else
        UniversalUsedList.Add(key, usedList);


      return UseJewlery(title, graphic, color, layer, ref usedList);
    }

    //---------------------------------------------------------------------------------------------


    public UseJewleryInfo UseJewlery(string title, Graphic graphic, UOColor color, Layer l, ref List<Serial> usedList)
    {
      List<UOItem> jewelries = new List<UOItem>();
      List<Serial> outOfCharges = new List<Serial>();

      UOItem jewelry = null;
      UseJewleryInfo result = new UseJewleryInfo();
      string key = graphic.ToString() + "|" + color.ToString();

      if (UniversalOutOfCharges.ContainsKey(key))
        outOfCharges = UniversalOutOfCharges[key];
      else
        UniversalOutOfCharges.Add(key, outOfCharges);

      foreach (UOItem item in World.Player.Backpack.AllItems)
      {
        if (item.Graphic == graphic && item.Color == color && !outOfCharges.Contains(item.Serial))
          jewelries.Add(item);
      }

      Game.PrintMessage(title + ": " + jewelries.Count + " Used: " + usedList.Count + " Out:" + outOfCharges.Count);
      foreach (UOItem item in jewelries)
      {
        if (!usedList.Contains(item.Serial))
        {
          jewelry = item;
          break;
        }
      }

      if (jewelry == null)
      {
        usedList.Clear();
        Game.PrintMessage(title + " reset");
        if (jewelries.Count > 0)
        {
          jewelry = jewelries[0];
        }
      }

      if (jewelry != null)
      {
        usedList.Add(jewelry.Serial);
        result = UseRemoveJewlery(jewelry, l);
        result.LastInRow = jewelries.Count == usedList.Count;
        result.Title = title;

        if (result.OutOfCharges)
          outOfCharges.Add(jewelry.Serial);

        result.Depleted = jewelries.Count == outOfCharges.Count;
      }

      if (result.Success)
        World.Player.PrintMessage(String.Format("{0} [{1}/{2}] OK", title, usedList.Count, jewelries.Count));
      else
        World.Player.PrintMessage(String.Format("{0} [{1}/{2}] Fail", title, usedList.Count, jewelries.Count), (ushort)Game.Val_LightGreen);

      return result;
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static UseJewleryInfo UseRemoveJewlery(Graphic graphic, UOColor color, Layer l)//, string layer)
    {
      return UseRemoveJewlery(World.Player.Backpack.AllItems.FindType(graphic, color), l);
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static UseJewleryInfo UseRemoveJewlery(UOItem item, Layer l)
    {
      UseJewleryInfo result = new UseJewleryInfo();
      UOItem equipedItem = World.Player.Layers[l];
      UOItem jewlery = item;
      Journal.Clear();

      if (jewlery.Exist)
      {
        ushort origX = jewlery.X;
        ushort origY = jewlery.Y;

        //jewlery.Move(1, World.Player.Backpack);
        //Game.Wait(250);
        jewlery.Use();
        if (!Journal.WaitForText(true, 250, "The item should be equipped to use", "It too soon to use it again", "You have to wait", "You must recharge it"))
          result.Success = true;
        else
        {
          if (Journal.Contains(true, "You must recharge it"))
          {
            result.OutOfCharges = true;
            Game.PrintMessage("OutOfCharges");
          }
        }


        jewlery.Move(1, World.Player.Backpack, origX, origY);
        //Game.Wait(250);
      }
      else
      {
        Game.PrintMessage("Nemas sperk!");
      }

      if (l != Layer.None && equipedItem.Exist && equipedItem.Container == World.Player.Backpack)
        equipedItem.Use();

      return result;
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void NahodStatyPresItemy()
    {
      if (!Game.CurrentGame.CurrentPlayer.GetSkillInstance<Jewelry>().UseDiamondBracelet())//UseRemoveJewlery(0x1088, 0x0485, Layer.Neck))
      {
        if (!Game.CurrentGame.CurrentPlayer.GetSkillInstance<Jewelry>().UseTitanNeklak())
        {
          UOItem alt = World.Player.Backpack.AllItems.FindType(0x0EB2, 0x0000);
          if (alt.Exist)
          {
            Targeting.ResetTarget();
            UO.WaitTargetSelf();
            alt.Use();

            World.Player.PrintMessage("Harp of Bless!");
          }
          else
          {
            //alt = World.Player.Backpack.AllItems.FindType(0x09CD, 0x0850);//zelena ryba
            //if (alt.Exist)
            //{
            //  alt.Use();


            //}
            World.Player.PrintMessage("Nemas sperky/nastroj");
          }
        }
        else
          World.Player.PrintMessage("Titan Neklak!");
      }
      else World.Player.PrintMessage("Diamond Bracelet!");
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void NahodSiluPresItemy()
    {
      if (!Game.CurrentGame.CurrentPlayer.GetSkillInstance<Jewelry>().UseTitanNeklak())//UseRemoveJewlery(0x1088, 0x0485, Layer.Neck))
      {
        UOItem alt = World.Player.Backpack.AllItems.FindType(0x0EB2, 0x0000);
        if (alt.Exist)
        {
          Targeting.ResetTarget();
          UO.WaitTargetSelf();
          alt.Use();

          World.Player.PrintMessage("Harp of Bless!");
        }
        else
        {
          //alt = World.Player.Backpack.AllItems.FindType(0x09CD, 0x0850);//zelena ryba
          //if (alt.Exist)
          //{
          //  alt.Use();

          //  World.Player.PrintMessage("Zelena ryba!");
          //}

          World.Player.PrintMessage("Nemas sperky/nastroj");
        }
      }
      else World.Player.PrintMessage("Titan Neklak!");

    }


    //---------------------------------------------------------------------------------------------

    public void RemoveRings()
    {
      World.Player.Layers[Layer.Ring].Move(0, World.Player.Backpack);
    }

    //---------------------------------------------------------------------------------------------

    public void RemoveNecklace()
    {
      UOItem neck = World.Player.Layers[Layer.Neck];

      Graphic[] neclases = new Graphic[] { 0x1085, 0x1088, 0x1089, 0x1F05, 0x1F08, 0x1F0A };
      if (Array.IndexOf<Graphic>(neclases, neck.Graphic) >= 0)
      {
        neck.Move(0, World.Player.Backpack);
      }
    }

    //---------------------------------------------------------------------------------------------

    public void RemoveBracelet()
    {
      World.Player.Layers[Layer.Bracelet].Move(0, World.Player.Backpack);
    }

    //---------------------------------------------------------------------------------------------

    public void RemoveEarrings()
    {
      World.Player.Layers[Layer.Earrings].Move(0, World.Player.Backpack);
    }

    //---------------------------------------------------------------------------------------------

    #region exec

    [Executable]
    [BlockMultipleExecutions]
    public static void ReflexRing()
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Jewelry>().UseReflexRing();
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void TitanNeklak()
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Jewelry>().UseTitanNeklak();
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void DiamondBracelet()
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Jewelry>().UseDiamondBracelet();
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void GreatGoldRing()
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Jewelry>().UseGreatGoldRing();
    }


    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void UniversalJewlery(string title, Graphic graphic, UOColor color, string layerStr)
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Jewelry>().UseJewlery(title, graphic, color, layerStr);
    }

    #endregion
  }

  public class UseJewleryInfo
  {
    public bool Success = false;
    public bool LastInRow = false;
    public string Title = String.Empty;
    public bool OutOfCharges = false;
    public bool Depleted = false;
  }

}
