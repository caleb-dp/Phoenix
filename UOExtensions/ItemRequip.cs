using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;
using Caleb.Library;
using Caleb.Library.CAL.Business;
using CalExtension.Skills;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Collections;

namespace CalExtension.UOExtensions
{
  public class ItemRequip
  {
    //---------------------------------------------------------------------------------------------
    [Executable]
    public static void RefullKlamakyClear(Serial aboxbedna, Serial targetbedna, params string[] options)
    {

      ItemHelper.naplnaboxy(World.Player.Backpack, aboxbedna);
      RefullKlamaky(aboxbedna, targetbedna, options);
    }

    [Executable]
    public static void RefullKlamakyClear(params string[] options)
    {
      Game.PrintMessage("Vyber bednu s Animal boxy:");
      UOItem targetCont = new UOItem(UIManager.TargetObject());

      Game.PrintMessage("Vyber pytel kam:");
      UOItem targetBag = new UOItem(UIManager.TargetObject());

      RefullKlamakyClear(targetCont, targetBag, options);
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void RefullKlamaky(params string[] options)
    {
      RefullKlamaky(Serial.Invalid, Serial.Invalid, options);
    }

    //---------------------------------------------------------------------------------------------


    public static void RefullKlamaky(Serial aboxbedna, Serial targetbedna, params string[] options)
    {
      List<ItemRequipInfo> infos = ParseInfo<ItemRequipInfo>(options);

      UOItem targetCont = new UOItem(aboxbedna);

      if (targetCont.Serial == Serial.Invalid)
      {
        Game.PrintMessage("Vyber bednu s Animal boxy:");
        targetCont = new UOItem(UIManager.TargetObject());
      }

      UOItem targetBag = new UOItem(targetbedna);

      if (targetBag.Serial == Serial.Invalid)
      {
        Game.PrintMessage("Vyber pytel kam:");
        targetBag = new UOItem(UIManager.TargetObject());
      }

      Game.PrintMessage("Nacitam Animal boxy ...");
      List<UOItem> items = ItemHelper.OpenContainerRecursive(targetCont);

      Dictionary<string, List<UOItem>> htBox = new Dictionary<string, List<UOItem>>();

      foreach (UOItem box in items)
      {
        if (box.Graphic == 0x09A8)//abox
        {
          if (String.IsNullOrEmpty(box.Name))
          {
            box.Click();
            Game.Wait(250);
          }

          string name = box.Name + String.Empty;
          name = NormalizeItemName(name).ToLower();//name.Replace("an ", "").Replace("a ", "").Trim()//.ToLower();

          if (!htBox.ContainsKey(name))
          {
            htBox.Add(name, new List<UOItem>());
          }

          htBox[name].Add(box);
        }
      }

      Game.PrintMessage("Nalezeno " + htBox.Keys.Count + " Animal boxu");
      List<UOItem> bagitems = new List<UOItem>();

      if (ItemHelper.IsInBackpack(targetBag))
        bagitems.AddRange(ItemHelper.OpenContainerRecursive(World.Player.Backpack));
      else
        bagitems.AddRange(ItemHelper.OpenContainerRecursive(targetBag));

      foreach (ItemRequipInfo info in infos)
      {
        foreach (string alter in info.NameAlternates)
        {
          string a = alter.Trim().ToLower();

          if (info.Items.Count >= info.Count)
            break;

          foreach (UOItem item in bagitems)
          {
            if (String.IsNullOrEmpty(item.Name))
            {
              item.Click();
              Game.Wait(Game.SmallestWait);
            }

            string name = item.Name + String.Empty;
            name = NormalizeItemName(name).ToLower();//name.Replace("an ", "").Replace("a ", "").Trim().ToLower();

            if (name == a && ItemLibrary.ShrinkKlamaci.Contains(item.Graphic) && !info.Items.Contains(item))
              info.Items.Add(item);
            else
            {
              UOItemType foundType = null;
              foreach (UOItemType shrinK in ItemLibrary.ShrinkKlamaci)
              {
                if (a == shrinK.Name.ToLower() && shrinK.Graphic == item.Graphic && shrinK.Color == item.Color)
                {
                  foundType = shrinK;
                  break;
                }
              }

              if (foundType != null && !info.Items.Contains(item))
                info.Items.Add(item);
            }

          }
        }

        if (info.Items.Count < info.Count)
        {
          foreach (string alter in info.NameAlternates)
          {
            string a = alter.Trim().ToLower();

            if (info.Items.Count >= info.Count)
              break;

            if (htBox.ContainsKey(a))
            {
              Dictionary<Serial, object[]> originalPositions = new Dictionary<Serial, object[]>();

              foreach (UOItem box in htBox[a])
              {
                if (!originalPositions.ContainsKey(box.Serial))
                  originalPositions.Add(box.Serial, new object[3]);

                originalPositions[box.Serial][0] = box.X;
                originalPositions[box.Serial][1] = box.Y;
                originalPositions[box.Serial][2] = box.Container;

                box.Move(1, World.Player.Backpack);
                Game.Wait();

                for (int i = info.Items.Count; i < info.Count; i++)
                {
                  Journal.Clear();

                  UO.WaitTargetSelf();
                  box.Use();

                  if (Journal.WaitForText(true, 500, "You put the", "Box je prazdny"))
                  {
                    if (Journal.Contains(true, "Box je prazdny"))
                      break;
                    else
                      Game.Wait();
                  }
                }

                foreach (UOItem item in World.Player.Backpack.Items)
                {
                  if (String.IsNullOrEmpty(item.Name))
                  {
                    item.Click();
                    Game.Wait(Game.SmallestWait);
                  }

                  string name = item.Name + String.Empty;
                  name = NormalizeItemName(name).ToLower();//name.Replace("an ", "").Replace("a ", "").Trim().ToLower();

                  if (name == a && ItemLibrary.ShrinkKlamaci.Contains(item.Graphic) && !info.Items.Contains(item))
                    info.Items.Add(item);
                }

                if (info.Items.Count >= info.Count)
                  break;
              }


              foreach (UOItem box in htBox[a])
              {
                if (originalPositions.ContainsKey(box.Serial))
                {
                  box.Move(1, (Serial)originalPositions[box.Serial][2], (ushort)originalPositions[box.Serial][0], (ushort)originalPositions[box.Serial][1]);
                  Game.Wait();
                }
              }


            }
          }
        }

        foreach (Serial s in info.Items)
        {
          UOItem item = new UOItem(s);
          if (item.Container != targetBag.Container || (info.X != 0xFFFF && info.Y != 0xFFFF && (info.X != item.X || info.Y != item.Y)))
          {
            item.Move(1, targetBag, (ushort)info.X, (ushort)info.Y);
            Game.Wait(350);
          }


        }

        Game.PrintMessage(info.Name + " - " + info.Items.Count);
      }

      Game.PrintMessage("Konec");
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void RefullSperkyClear(Serial zdrojBagl, Serial cilBagl, params string[] options)
    {

      ItemHelper.naplnsperky(World.Player.Backpack, zdrojBagl);
      RefullSperky(zdrojBagl, cilBagl, options);
    }

    [Executable]
    public static void RefullSperkyClear(params string[] options)
    {
      Game.PrintMessage("Vyber bednu se Sperkovnicemi:");
      UOItem targetCont = new UOItem(UIManager.TargetObject());

      Game.PrintMessage("Vyber pytel kam:");
      UOItem targetBag = new UOItem(UIManager.TargetObject());

      RefullSperkyClear(targetCont, targetBag, options);
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void RefullSperky(params string[] options)
    {
      RefullSperky(Serial.Invalid, Serial.Invalid, options);
    }

  //  string name = box.Name + String.Empty;


    //name = name.Replace("an ", "").Replace("a ", "").Trim().ToLower();

    //---------------------------------------------------------------------------------------------

    public static string NormalizeItemName(string name)
    {
      if (name.StartsWith("an "))
        name = name.Remove(0, 3);

      if (name.StartsWith("a "))
        name = name.Remove(0, 2);

      name = name.Trim();

      return name;
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void RefullSperky(Serial zdrojBagl, Serial cilBagl, params string[] options)
    {
      List<ItemRequipInfo> infos = ItemRequip.ParseInfo<ItemRequipInfo>(options);

      UOItem zdrojKont = new UOItem(zdrojBagl);

      if (zdrojKont.Serial == Serial.Invalid)
      {
        Game.PrintMessage("Vyber bednu se Sperkovnicemi:");
        zdrojKont = new UOItem(UIManager.TargetObject());
      }

      UOItem cilKont = new UOItem(cilBagl);

      if (cilKont.Serial == Serial.Invalid)
      {
        Game.PrintMessage("Vyber pytel kam:");
        cilKont = new UOItem(UIManager.TargetObject());
      }

      Game.PrintMessage("Nacitam Sperkovnice ...");
      List<UOItem> items = ItemHelper.OpenContainerRecursive(zdrojKont);

      Dictionary<string, List<UOItem>> htBox = new Dictionary<string, List<UOItem>>();

      foreach (UOItem box in items)
      {
        if (box.Graphic == 0x09A8 && (box.Color == 0x054E || box.Color == 0x049F || box.Color == 0x0796 || box.Color == 0x06FB))// Barva: aktivni 0x054E  , neaktivni 0x049F  
        {
          if (String.IsNullOrEmpty(box.Name))
          {
            box.Click();
            Game.Wait(Game.SmallestWait);
          }

          string name = box.Name + String.Empty;

          if (name.StartsWith("an "))
            name = name.Remove(0, 3);

          if (name.StartsWith("a "))
            name = name.Remove(0, 2);

          name = name.Trim().ToLower();
          //name = name.Replace("an ", "").Replace("a ", "").Trim().ToLower();

          string fixBugName = String.Empty;
          if (name == "Great Alabaster Necklace".ToLower())
            fixBugName = "Great Alabastr Necklace".ToLower();

          if (!htBox.ContainsKey(name))
          {
            htBox.Add(name, new List<UOItem>());
          }

          if (!String.IsNullOrEmpty(fixBugName) && !htBox.ContainsKey(fixBugName))
          {
            htBox.Add(fixBugName, new List<UOItem>());
          }

          if (htBox.ContainsKey(fixBugName))
            htBox[fixBugName].Add(box);

          htBox[name].Add(box);
        }
      }

      Game.PrintMessage("Nalezeno " + htBox.Keys.Count + " Sperkovnic");
      List<UOItem> bagitems = new List<UOItem>();

      if (ItemHelper.IsInBackpack(cilKont))
        bagitems.AddRange(ItemHelper.OpenContainerRecursive(World.Player.Backpack));
      else
        bagitems.AddRange(ItemHelper.OpenContainerRecursive(cilKont));

      foreach (ItemRequipInfo info in infos)
      {
        Game.PrintMessage(info.Name + " - Count: " + info.Count + " / Items.Count: " + info.Items.Count + " / Amount: " + info.Amount);
        
        if (info.Items.Count < info.Count)
        {
          foreach (string alter in info.NameAlternates)
          {
            string a = alter.Trim().ToLower();

            if (info.Items.Count >= info.Count)
              break;

            if (htBox.ContainsKey(a))
            {
              Dictionary<Serial, object[]> originalPositions = new Dictionary<Serial, object[]>();

              foreach (UOItem box in htBox[a])
              {
                if (!originalPositions.ContainsKey(box.Serial))
                  originalPositions.Add(box.Serial, new object[3]);

                originalPositions[box.Serial][0] = box.X;
                originalPositions[box.Serial][1] = box.Y;
                originalPositions[box.Serial][2] = box.Container;

                if (box.Container != World.Player.Backpack)
                {
                  box.Move(1, World.Player.Backpack);
                  Game.Wait();
                }

                if (box.Color == 0x049F || box.Color == 0x0796)
                {
                  UO.WaitTargetObject(box);
                  box.Use();
                  Game.Wait(Game.SmallestWait);
                }

                for (int i = info.Items.Count; i < info.Count; i++)
                {
                  Journal.Clear();

                  UO.WaitTargetSelf();
                  box.Use();

                  if (Journal.WaitForText(true, 500, "You put the", "Sperkovnice je prazdna"))
                  {
                    if (Journal.Contains(true, "Sperkovnice je prazdna"))
                      break;
                    else
                    {
                      //Ve sperkovnici nejsou zadna nabiti!
                      Game.Wait(Game.SmallestWait);

                      foreach (UOItem item in World.Player.Backpack.Items)
                      {
                        if (Jewelry.IsJewelry(item))
                        {
                          UOItemExtInfo extInfo = ItemHelper.GetItemExtInfo(item, a);

                          if (extInfo.Success && !info.Items.Contains(item))
                          {
                            int count = extInfo.Charges.GetValueOrDefault();
                            int amount = info.Amount - count;

                            for (int u = amount; u > 0; u -= 2)
                            {
                              UO.WaitTargetObject(item);
                              box.Use();
                              if (Journal.WaitForText(true, 500, "Ve sperkovnici nejsou zadna nabiti!"))
                              {
                                break;
                              }
                              else
                                Game.Wait(Game.SmallestWait);
                            }

                            UOItem itemAfter = new UOItem(item.Serial);
                            extInfo = ItemHelper.GetItemExtInfo(itemAfter, a);
                            Game.PrintMessage(extInfo.Name + " nabit na + " + (extInfo.Charges.GetValueOrDefault() - count));

                            info.Items.Add(itemAfter);
                          }
                        }
                      }

                      Game.Wait(Game.SmallestWait);
                    }
                  }
                }

                if (info.Items.Count >= info.Count)
                  break;
              }

              foreach (UOItem box in htBox[a])
              {
                if (originalPositions.ContainsKey(box.Serial))
                {
                  box.Move(1, (Serial)originalPositions[box.Serial][2], (ushort)originalPositions[box.Serial][0], (ushort)originalPositions[box.Serial][1]);
                  Game.Wait(Game.SmallestWait);
                }
              }
            }
          }
        }
        
        foreach (Serial s in info.Items)
        {
          UOItem item = new UOItem(s);
          if (item.Container != cilKont.Container || (info.X != 0xFFFF && info.Y != 0xFFFF && (info.X != item.X || info.Y != item.Y)))
          {
            item.Move(1, cilKont, (ushort)info.X, (ushort)info.Y);
            Game.Wait(350);
          }
        }
        Game.Wait(Game.SmallestWait);

        Game.PrintMessage(info.Name + " - " + info.Items.Count);
      }

      Jewelry.SetridSperky(cilKont);

      Game.PrintMessage("Konec");
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void RefullToulce()
    {
      RefullToulce(Serial.Invalid, 700);
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void RefullToulce(Serial zdrojBagl, int pocet)
    {

      UOItem zdrojKont = new UOItem(zdrojBagl);

      if (zdrojKont.Serial == Serial.Invalid)
      {
        Game.PrintMessage("Vyber bednu s zdrojem:");
        zdrojKont = new UOItem(UIManager.TargetObject());
      }

      Game.PrintMessage("Nacitam Itemy ...");
      List<UOItem> items = ItemHelper.OpenContainerRecursive(zdrojKont);

      UOItem boltq = World.Player.Backpack.Items.FindType(0x1EA0, 0x083A);
      UOItem arrowq = World.Player.Backpack.Items.FindType(0x1EA0, 0x0747);

      UOItem bolt = zdrojKont.AllItems.FindType(0x1BFB, 0x0000);
      UOItem arrow = zdrojKont.AllItems.FindType(0x0F3F, 0x0000);

      if (boltq.Exist)
      {
        UOItemExtInfo boltqInfo = ItemHelper.GetItemExtInfo(boltq);
        int tofill = pocet - boltqInfo.Charges.GetValueOrDefault();

        if (tofill > 0)
        {

          bolt.Move((ushort)tofill, World.Player.Backpack);
          Game.Wait();
          for (int i = World.Player.Backpack.Items.FindType(0x1BFB, 0x0000).Amount; i > 0; i = i - 100)
          {
            boltq.Use();
            Game.Wait(250);
          }
          //21:14 Star Lord: 100 sipu bylo vydano z magickeho toulce.
        }
      }


      if (arrowq.Exist && arrow.Exist)
      {
        UOItemExtInfo arrowqInfo = ItemHelper.GetItemExtInfo(arrowq);
        int tofill = pocet - arrowqInfo.Charges.GetValueOrDefault();

        if (tofill > 0)
        {

          arrow.Move((ushort)tofill, World.Player.Backpack);
          Game.Wait();
          for (int i = World.Player.Backpack.Items.FindType(0x0F3F, 0x0000).Amount; i > 0; i = i - 100)
          {
            arrowq.Use();
            Game.Wait(250);
          }
          //21:14 Star Lord: 100 sipu bylo vydano z magickeho toulce.
        }

      }
    }


    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void RefullCommon(params string[] options)
    {
      RefullCommon(Serial.Invalid, Serial.Invalid, options);

    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void RefullCommon(Serial zdrojBagl, Serial cilBagl, params string[] options)
    {
      List<ItemRequipInfo> infos = ParseInfo<ItemRequipInfo>(options);

      UOItem zdrojKont = new UOItem(zdrojBagl);

      if (zdrojKont.Serial == Serial.Invalid)
      {
        Game.PrintMessage("Vyber bednu s zdrojem:");
        zdrojKont = new UOItem(UIManager.TargetObject());
      }

      UOItem cilKont = new UOItem(cilBagl);

      if (cilKont.Serial == Serial.Invalid)
      {
        Game.PrintMessage("Vyber pytel kam:");
        cilKont = new UOItem(UIManager.TargetObject());
      }

      Game.PrintMessage("Nacitam Itemy ...");
      List<UOItem> items = ItemHelper.OpenContainerRecursive(zdrojKont);


      foreach (ItemRequipInfo info in infos)
      {


        if (info.Name.ToLower() == "magregy")
        {
          int x = info.X;
          int y = info.Y;

          foreach (Reagent r in ReagentCollection.MagReagents)
          {
            RefullCommon(zdrojKont.Serial, cilKont.Serial, String.Format("Name: {0}, Graphic: {1}, Color: {2}, Amount: {3}, X: {4}, Y: {5}", r.ShortName, r.Graphic, r.Color, info.Amount, x, y));
            x += 10;
          }
          //        else if (type == "magregy")
          //{
          //  if (ReagentCollection.MagReagents.Contains(item))
          //    toMove.Add(item);
          //}
          //else if (type == "necroregy")
          //{
          //  if (ReagentCollection.NecroReagents.Contains(item))
          //    toMove.Add(item);
          //}

          continue;
        }

        if (info.Graphic.IsInvariant)
        {
          Game.PrintMessage("Info " + info.Name + " Graphic.Invariant ! " + info.Graphic + " / " + Graphic.Invariant);
          continue;
        }

        UOItem itm = cilKont.Items.FindType(info.Graphic, info.Color);
        UOItem src = zdrojKont.AllItems.FindType(info.Graphic, info.Color);

        ushort moveAmount = 0;
        int moveDirection = 1;
        int itmAmount = cilKont.Items.Where(ia => (ia.Graphic == info.Graphic) && ia.Color == info.Color).Sum(ia => ia.Amount);
        int itmCount = cilKont.Items.Count(ia => (ia.Graphic == info.Graphic) && ia.Color == info.Color);
        string messageFormat = "Info {0} {1} {2}, Stav: {3}>{4}";
        string directionMessage = "Nezmeneno..";

        if (src.Exist)
        {
          if (itm.Exist)
          {
            int diffAmount = info.Amount - itmAmount;
            if (diffAmount > 0)
              moveAmount = (ushort)diffAmount;
            else
            {
              moveAmount = (ushort)Math.Abs(diffAmount);
              moveDirection = -1;
            }
          }
          else
            moveAmount = (ushort)info.Amount;
        }

        if (moveAmount > 0)
        {
          if (moveDirection > 0)
          {
            directionMessage = "Doplnuji..";
            //Game.PrintMessage("Info " + info.Name + " Doplnuji... " + moveAmount);


            //src.Move(moveAmount, cilKont);
            UOItem[] srcItems = zdrojKont.AllItems.Where(srcItem => (srcItem.Graphic == info.Graphic) && srcItem.Color == info.Color).ToArray();

            foreach (UOItem srcItm in srcItems)
            {
              if (moveAmount <= 0)
                break;

              int tmpAmout = moveAmount - srcItm.Amount;
              srcItm.Move(moveAmount, cilKont);

              if (tmpAmout > 0)
                moveAmount = (ushort)tmpAmout;
              else
                moveAmount = 0;

              Game.Wait(Game.SmallestWait);
            }
          }
          else if (moveDirection < 0)
          {
            directionMessage = "Vracim..";
            //Game.PrintMessage("Info " + info.Name + " Vracim... " + moveAmount);
            //itm.Move(moveAmount, zdrojKont);

            UOItem[] srcItems = cilKont.AllItems.Where(srcItem => (srcItem.Graphic == info.Graphic) && srcItem.Color == info.Color).ToArray();
            foreach (UOItem srcItm in srcItems)
            {
              if (moveAmount <= 0)
                break;

              int tmpAmout = moveAmount - srcItm.Amount;
              srcItm.Move(moveAmount, zdrojKont);

              if (tmpAmout > 0)
                moveAmount = (ushort)tmpAmout;
              else
                moveAmount = 0;

              Game.Wait(Game.SmallestWait);
            }
          }
          Game.Wait(Game.SmallestWait);
        }

        Game.PrintMessage(messageFormat, MessageType.Info, info.Name, directionMessage, moveAmount, itmAmount, cilKont.Items.FindType(info.Graphic, info.Color).Amount);

        info.Items.AddRange(cilKont.Items.Where(i => (i.Graphic == info.Graphic) && i.Color == info.Color).Select(i => i.Serial).ToArray());

        foreach (Serial s in info.Items)
        {
          UOItem item = new UOItem(s);

          if (item.Container != cilKont.Container || (info.X != 0xFFFF && info.Y != 0xFFFF && (info.X != item.X || info.Y != item.Y)))
          {
            item.Move((ushort)(item.Amount + 1), cilKont, (ushort)info.X, (ushort)info.Y);
            Game.Wait(350);
          }
        }

      }

      Game.PrintMessage("Konec");
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void RefullKade(params string[] options)
    {
      RefullKade(Serial.Invalid, Serial.Invalid, options);
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void RefullKade(Serial zdrojBagl, Serial cilBagl, params string[] options)
    {
      List<ItemRequipInfo> infos = ParseInfo<ItemRequipInfo>(options);
      UOItem zdrojKont = new UOItem(zdrojBagl);
      World.FindDistance = 20;
      if (zdrojKont.Serial == Serial.Invalid && zdrojKont.Serial > 0)
      {
        Game.PrintMessage("Vyber bednu s zdrojem (ESC = Ground):");
        zdrojKont = new UOItem(UIManager.TargetObject());
      }

      UOItem cilKont = new UOItem(cilBagl);

      if (cilKont.Serial == Serial.Invalid)
      {
        Game.PrintMessage("Vyber pytel kam:");
        cilKont = new UOItem(UIManager.TargetObject());
      }


      Game.PrintMessage("Nacitam Itemy ...");
      List<UOItem> items = new List<UOItem>();
      if (zdrojBagl == 0 || !zdrojBagl.IsValidCust() || !zdrojKont.ExistCust())// { World.FindDistance = 8; isGround = true;  }
         items.AddRange(World.Ground.ToArray().Where(a => a.Distance <= 5));
      else
        items = ItemHelper.OpenContainerRecursive(zdrojKont);

      Game.PrintMessage("items ..." + items.Count);

      List<UOItem> bagitems = new List<UOItem>();

      if (ItemHelper.IsInBackpack(cilKont))
        bagitems.AddRange(ItemHelper.OpenContainerRecursive(World.Player.Backpack));
      else
        bagitems.AddRange(ItemHelper.OpenContainerRecursive(cilKont));

      ushort startX = 100;
      ushort startY = 16;
      //ushort currentX = startX;
      //ushort currentY = startY;
      items = items.OrderBy(a => (Math.Abs(a.Z - World.Player.Z))).ThenBy(a => a.Distance).ToList();
      for (int i = 0; i < infos.Count; i++)
      {
        Game.PrintMessage("infos ..." + i);
        ItemRequipInfo info = infos[i];
        int minPerc = 80;
        if (info.Amount >= 200)
        {
          minPerc = 90;
        }

        Potion potion = PotionCollection.Potions.GetItemByName(info.Name);
        Graphic kadGra = potion.Qualities[info.Quality].KadGraphic;
        UOColor kadColor = potion.Qualities[info.Quality].KadColor;

        Game.PrintMessage("kad ..." + kadGra + " / " + kadColor);
        UOItem sourcekad = new UOItem(Serial.Invalid);

        foreach (UOItem item in bagitems)
        {
          if (item.Graphic == kadGra && item.Color == kadColor)
          {
            info.Items.Add(item);
          }
        }

        foreach (UOItem item in items)
        {
          if (item.Graphic == kadGra && item.Color == kadColor)
          {
            sourcekad = item;
            break;
          }
        }

        if (sourcekad.Exist)
        {
          Game.PrintMessage("info.Items ..." + info.Items.Count);
          foreach (Serial item in info.Items)
          {
            UOItem kad = new UOItem(item);
            if (String.IsNullOrEmpty(kad.Name))
            {
              kad.Click();
              Game.Wait(Game.SmallestWait);
            }
            
            Game.PrintMessage("Kad OK " + kad.Name);
            UOItemExtInfo extInfo = ItemHelper.GetItemExtInfo(kad, null);

            //UOItem from = new UOItem(Serial.Invalid);
            //UOItem to = new UOItem(Serial.Invalid);

            if (extInfo.Success)
            {
              if (extInfo.Charges < info.Amount )//TODO dodelat pokud je v kadi 90%-+ tak nedolevat? 
              {
                if (((extInfo.Charges.GetValueOrDefault() / (decimal)info.Amount) * 100) < minPerc)
                {
                  //Game.PrintMessage("V kadi je " + String.Format("{0:N1}", ((extInfo.Charges.GetValueOrDefault() / (decimal)info.Amount) * 100)) + "% / "  + minPerc + "% -  " + extInfo.Charges);
                  int toFill = info.Amount - extInfo.Charges.GetValueOrDefault();

                  Serial orgiCont = kad.Container;
                  if (!ItemHelper.IsInBackpack(kad))
                  {
                    kad.Move(1, World.Player.Backpack);
                    Game.Wait();
                  }

                  for (int u = toFill; u > 0; u -= 50)
                  {
                    UO.WaitTargetObject(kad);
                    sourcekad.Use();
                    Game.Wait(Game.SmallestWait);
                  }

                  if (new UOItem(kad.Serial).Container != orgiCont)
                  {
                    kad.Move(1, orgiCont);
                    Game.Wait(Game.SmallestWait);
                  }

                  Game.PrintMessage(info.Name + " doplneno " + toFill);
                }
                else
                  Game.PrintMessage("V kadi je " + ((extInfo.Charges.GetValueOrDefault() / (decimal)info.Amount) * 100) + "% / " + minPerc + "% - " + extInfo.Charges);
              }
              else if (extInfo.Charges > info.Amount + 50)//odlejvat ?? uvidime
              {
              }
            }
            else
              Game.PrintMessage("Kad extInfo FAIL " + kad.Name);
          }
        }
        else
        {
          Game.PrintMessage("Kad " + info.Name + " NENALEZENA.");
        }

        foreach(Serial s in info.Items)
        {
          UOItem item = new UOItem(s);

          ushort[] xy = FindEmptySlotCoordinates(cilKont, startX, startY, 165, 8, 16);
          //if (currentX > 165)
          //{
          //  currentX = startX;
          //  currentY += 16;
          //}

          if (item.X != xy[0] && item.Y != xy[1] || item.Container != cilKont.Serial)
          {
            item.Move(1, cilKont.Serial, xy[0], xy[1]);
            Game.Wait(Game.SmallWait);
          }
          //currentX += 8;
        }

      }

      Game.PrintMessage("Konec");
    }

    //---------------------------------------------------------------------------------------------

    public static ushort[] FindEmptySlotCoordinates(UOItem container, ushort startX, ushort startY, ushort xMax, ushort xShift, ushort yShift)
    {
      ushort[] xy = new ushort[] { startX, startY };
      ushort currentX = startX;
      ushort currentY = startY;

      while (currentY < (startY + (5 * yShift)))
      {
        if (container.Items.Where(i => i.X == currentX && i.Y == currentY).Count() == 0)
        {
          xy = new ushort[2];
          xy[0] = currentX;
          xy[1] = currentY;
          break;
        }
        else
        {
          currentX += 8;

          if (currentX > 165)
          {
            currentX = startX;
            currentY += 16;
          }
        }
      }

      return xy;
    }

    //---------------------------------------------------------------------------------------------

    public static List<T> ParseInfo<T>(string[] opts)
    {
      List<T> list = new List<T>();

      for(int i = 0; i < opts.Length; i++)
      {
        string opt = opts[i];

        string lastVal = String.Empty;

        try
        {
          T info = Activator.CreateInstance<T>();
          foreach (FieldInfo fInfo in info.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
          {
            Match match = Regex.Match(opt, fInfo.Name + ":[ ]?(?<value>[^}{,]+),?");
            if (match.Success)
            {
              string strValue = (match.Groups["value"].Value + String.Empty).Trim();
              object converValue = null;
              lastVal = strValue + " / " + fInfo.FieldType;

              if (fInfo.FieldType == typeof(System.UInt16))
                converValue = Convert.ToUInt16(strValue, 16);
              else if (fInfo.FieldType == typeof(Graphic))
                converValue = new Graphic(Convert.ToUInt16(strValue, 16));
              else if (fInfo.FieldType == typeof(UOColor))
                converValue = new UOColor(Convert.ToUInt16(strValue, 16));
              else if (fInfo.FieldType == typeof(Serial))
                converValue = new Serial(Convert.ToUInt32(strValue, 16));
              else if (fInfo.FieldType == typeof(PotionQuality))
                converValue = Enum.Parse(typeof(PotionQuality), strValue);
              else 
                converValue = Convert.ChangeType(strValue, fInfo.FieldType);

              fInfo.SetValue(info, converValue);

              //if (fInfo.Name == "Graphic")
              //  Game.PrintMessage("Graphic " + strValue + " / " + converValue + " / " + fInfo.GetValue(info));
            }
          }
          list.Add(info);
        }
        catch (Exception ex)
        {
          Game.PrintMessage("Nepodarilo se nacist parametr [" + i + "]!");
          Game.PrintMessage(ex.Message);
          Game.PrintMessage(lastVal);
        }
      }
      return list;
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void RefullLahve(params string[] options)
    {
      RefullLahve(Serial.Invalid, Serial.Invalid, options);
    }

    [Executable]
    public static void RefullLahve(Serial zdrojBagl, Serial cilBagl, params string[] options)
    {
      Game.PrintMessage("options.Length: " + options.Length);

      Dictionary<string, ReuqipItemInfo> requipInfo = new Dictionary<string, ReuqipItemInfo>();
      try
      {
        foreach (string opt in options)
        {
          //if (opt.StartsWith("Potion:"))
          //{
            ReuqipItemInfo optInfo = new ReuqipItemInfo();

            foreach (FieldInfo fInfo in optInfo.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
              Match match = Regex.Match(opt, fInfo.Name + ":[ ]?(?<value>[^}{,]+),?");
              if (match.Success)
              {
                string strValue = (match.Groups["value"].Value + String.Empty).Trim();
                object converValue = null;
                if (fInfo.FieldType == typeof(PotionQuality))
                {
                  converValue = Enum.Parse(typeof(PotionQuality), strValue);
                }
                else
                {
                  converValue = Convert.ChangeType(strValue, fInfo.FieldType);
                }
                fInfo.SetValue(optInfo, converValue);
              }
            }

            Potion potion = PotionCollection.Potions.GetItemByName(optInfo.Name);

            if (potion != null && !requipInfo.ContainsKey(optInfo.Name))
            {
              optInfo.OriginalOpt = opt;
              optInfo.Type = new UOItemType() { Graphic = potion.Qualities[optInfo.Quality].Graphic, Color = potion.Qualities[optInfo.Quality].Color };
              requipInfo.Add(optInfo.Name, optInfo);
            }
            else
            {
              Game.PrintMessage("Potion Failed OPT NAME: " + optInfo.Name + " " + (potion == null));
            }
          //}
          //else
          //  Game.PrintMessage("Not implemented OPT");
        }
      }
      catch (Exception ex)
      {
        Game.PrintMessage(ex.Message);
        return;
      }

      UOItem zdrojKont = new UOItem(zdrojBagl);

      if (zdrojKont.Serial == Serial.Invalid && zdrojBagl > 0)
      {
        Game.PrintMessage("Vyber bednu s zdrojem (ESC = Ground):");
        zdrojKont = new UOItem(UIManager.TargetObject());
      }

      bool ground = !zdrojKont.ExistCust();

      UOItem cilKont = new UOItem(cilBagl);

      if (cilKont.Serial == Serial.Invalid)
      {
        Game.PrintMessage("Vyber pytel kam:");
        cilKont = new UOItem(UIManager.TargetObject());
      }

      Game.PrintMessage("Nacitam Itemy ...");

      List<UOItem> bagitems = new List<UOItem>();

      bagitems.AddRange(ItemHelper.OpenContainerRecursive(cilKont));

      Game.Wait(Game.SmallestWait);

      foreach (UOItem item in bagitems)
      {
        foreach (KeyValuePair<string, ReuqipItemInfo> kvp in requipInfo)
        {
          ReuqipItemInfo info = kvp.Value;
          if (info.Type.Graphic == item.Graphic && item.Color == info.Type.Color)
          {
            info.Items.Add(item);
            info.Count++;
          }
        }
      }

      Game.Wait(Game.SmallestWait);


      Game.PrintMessage("requipInfo.Count: " + requipInfo.Count);

      ushort startX = 15;
      ushort startY = 15;

      ushort currentX = startX;
      ushort currentY = startY;

      int counter = 0;
      foreach (KeyValuePair<String, ReuqipItemInfo> kvp in requipInfo)
      {
        ReuqipItemInfo info = kvp.Value;

        if (counter == 0)
        {
          if (info.PositionX > 0)
            currentX = startX = info.PositionX;
          if (info.PositionY > 0)
            currentY = startY = info.PositionY;
        }

        Potion potion = PotionCollection.Potions.GetItemByName(info.Name);
        UOItem kad = new UOItem(Serial.Invalid);
        if (ground)
        {
          kad = World.Ground.FindType(potion.Qualities[info.Quality].KadGraphic, potion.Qualities[info.Quality].KadColor);
        }
        else
        {
          kad = zdrojKont.Items.FindType(potion.Qualities[info.Quality].KadGraphic, potion.Qualities[info.Quality].KadColor);
          if (kad.ExistCust())
          {
            kad.Move(1, World.Player.Backpack);
            Game.Wait(Game.SmallestWait);
          }
        }

        Game.PrintMessage(info.Name + ": " + kad.Exist + " / " + kad.Distance + " / " + info.Count + " / " + info.MaxItem);

        if (kad.ExistCust() && (!ground || kad.Distance <= 6))
        {
          while (info.Count < info.MaxItem)
          {
            UOItem empty = World.Player.Backpack.AllItems.FindType(Potion.Empty); 

            if (!empty.Exist)
            {
              if (!empty.Exist)
                empty = World.Ground.FindType(Potion.Empty);

              if (!empty.Exist)
              {
                if (zdrojBagl.IsValid)
                {
                  Game.PrintMessage("Nacitam Itemy ...");
                  ItemsCollection items = new ItemsCollection(zdrojKont, true);// ItemHelper.OpenContainerRecursive(zdrojKont);

                  empty = items.FindType(Potion.Empty);

                  if (empty.Exist)
                  {
                    empty.Move(2, World.Player.Backpack);
                    Game.Wait(Game.SmallestWait);
                    empty = World.Player.Backpack.AllItems.FindType(Potion.Empty);
                  }
                }
              }

            }

            if (empty.Exist)
            {
              kad.Use();
              UO.WaitTargetObject(empty);// nebo jednoduse targettype ..?
              Game.Wait(Game.SmallestWait);
            }

            info.Count++;
          }
          Game.Wait(Game.SmallestWait);
          if (!ground)
          {
            kad.Move(1, zdrojKont.Serial);
            Game.Wait(Game.SmallestWait);
          }

          bagitems.Clear();
          bagitems.AddRange(ItemHelper.OpenContainerRecursive(World.Player.Backpack));

          foreach (UOItem item in bagitems)
          {
            info = kvp.Value;
            if (info.Type.Graphic == item.Graphic && item.Color == info.Type.Color && !info.Items.Contains(item.Serial))
            {
              info.Items.Add(item);
            }
          }
        }

        counter++;
        Game.Wait(Game.SmallestWait);

        for (int i = 0; i < kvp.Value.Items.Count; i++)
        {
          UOItem item = new UOItem(kvp.Value.Items[i]);

          if (currentX > 95)
          {
            currentX = startX;
            currentY += 8;
          }

          if (item.X != currentX && item.Y != currentY || item.Container !=cilKont.Serial)
          {
            item.Move(1, cilKont.Serial, currentX, currentY);
            Game.Wait(Game.SmallWait);
          }
          currentX += 4;
        }
      }

      Game.PrintMessage("Konec.");
    }

    //---------------------------------------------------------------------------------------------
  }

  public class ItemRequipInfo
  {
    public string Type = String.Empty;
    public string Name =  String.Empty;
    public Graphic Graphic = Graphic.Invariant;
    public UOColor Color = UOColor.Invariant;
    public PotionQuality Quality = PotionQuality.None;
    public int Count = 1;
    public int Amount = 1;
    public int X = 0xFFFF;
    public int Y = 0xFFFF;
    public List<Serial> Items = new List<Serial>();
    public string[] NameAlternates
    {
      get
      {
        return this.Name.Trim().Split(new char[] { '|' });
      }
    }
  }

  //---------------------------------------------------------------------------------------------

  public class ReuqipItemInfo
  {
    public string OriginalOpt;
    public UOItemType Type;
    public string Name;
    public PotionQuality Quality = PotionQuality.None;
    public int Count = 0;
    public ushort PositionX = 0;
    public ushort PositionY = 0;
    public int MaxItem = 10;
    public List<Serial> Items = new List<Serial>();
  }
}

