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
using System.Threading;

namespace CalExtension.UOExtensions
{
  public class ItemHelper
  {
    //---------------------------------------------------------------------------------------------

    public UOItem LootBag
    {
      get
      {
        if (World.Player.Backpack.Items.FindType(0x0E76, 0x005C).Exist)
          return World.Player.Backpack.Items.FindType(0x0E76, 0x005C);

        return World.Player.Backpack;
      }
    }
    public UOItem DwarfKnife
    {
      get
      {
        if (World.Player.Backpack.Items.FindType(0x10E4).Exist)
          return World.Player.Backpack.Items.FindType(0x10E4);

        return null;
      }
    }

    //---------------------------------------------------------------------------------------------

    private static UOItem lastBracelet = new UOItem(Serial.Invalid);
    private static UOItem lastRing = new UOItem(Serial.Invalid);
    private static UOItem lastEarrings = new UOItem(Serial.Invalid);
    private static UOItem lastNeck = new UOItem(Serial.Invalid);
    private static UOItem lastLeftHand = new UOItem(Serial.Invalid);
    private static UOItem lastRightHand = new UOItem(Serial.Invalid);
    private static UOItem lastOuterTorso = new UOItem(Serial.Invalid);
    private static UOItem lastOuterLegs = new UOItem(Serial.Invalid);
    private static UOItem lastHat = new UOItem(Serial.Invalid);
    private static UOItem lastGloves = new UOItem(Serial.Invalid);
    private static UOItem lastArms = new UOItem(Serial.Invalid);
    private static UOItem lastInnerLegs = new UOItem(Serial.Invalid);
    private static UOItem lastMiddleTorso = new UOItem(Serial.Invalid);

    private static UOItem lastShoes = new UOItem(Serial.Invalid);
    private static UOItem lastInnerTorso = new UOItem(Serial.Invalid);
    private static UOItem lastCloak = new UOItem(Serial.Invalid);
    private static UOItem lastShirt = new UOItem(Serial.Invalid);
    private static UOItem lastPants = new UOItem(Serial.Invalid);
    private static UOItem lastWaist = new UOItem(Serial.Invalid);

    //---------------------------------------------------------------------------------------------

    public static List<Graphic> CloseDoors { get { return new List<Graphic> { 0x06AD, 0x06A5, 0x0677, 0x0675, 0x067D, 0x06A5, 0x083B, 0x0839, 0x0695, 0x086E, 0x06A7 }; } }
    public static List<Graphic> OpenDoors { get { return new List<Graphic> { 0x06AE, 0x06A6, 0x0678, 0x0676, 0x067E, 0x06A6, 0x083C, 0x083A, 0x0696, 0x086F, 0x06A8 }; } }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void PrintMyName()
    {
      World.Player.Click();
      Game.Wait();
      UO.Print(World.Player.Name);
    }


    //---------------------------------------------------------------------------------------------

    public static string GetItemName(UOItem item)
    {
      EnsureItem(item);
      return item.Name + String.Empty;
    }

    //---------------------------------------------------------------------------------------------

    public static void EnsureItem(UOItem item)
    {
      if (String.IsNullOrEmpty(item.Name))
        item.Click();

      Game.Wait(Game.ClickWait);
    }

    //---------------------------------------------------------------------------------------------

    public static void EnsureContainer(UOItem container)
    {
      if (IsContainer(container))
      {
        EnsureItem(container);

        if (!container.Opened)
        {
          container.Use();
          Game.Wait(Game.SmallestWait);
        }
      }
    }

    //---------------------------------------------------------------------------------------------

    public static bool IsInBackpack(UOItem item)
    {
      int sychr = 0;
      UOItem current = item;
      bool result = false;
      while (sychr < 6 && !result && current.Serial.IsValid)
      {
        result = current.Container == World.Player.Serial;
        current = new UOItem(current.Container);
        sychr++;
      }

      return result;

    }

    //---------------------------------------------------------------------------------------------

    public static bool IsContainer(UOItem item)
    {
      if (item.Graphic == 0x09B0 && item.Color == 0x0493)//KPZ
        return false;
      //0x0E75  
      return item.Serial == World.Player.Backpack.Serial || 
        item.Graphic == 0x0E40 || 
        item.Graphic == 0x0E41 || 
        item.Graphic == 0x09B0 || 
        item.Graphic == 0x0E76 || 
        item.Graphic == 0x0E75 || 
        item.Graphic == 0x0E77 || 
        item.Graphic == 0x0E78 || 
        item.Graphic == 0x0E79 || 
        item.Graphic == 0x0E7D;
    }

    //---------------------------------------------------------------------------------------------

    public static List<UOItem> OpenContainerRecursive(Serial container)
    {
      
      UOItem cont = new UOItem(container);
      EnsureContainer(cont);
      List<UOItem> list = new List<UOItem>();

      list.AddRange(cont.Items.ToArray());
      foreach (UOItem item in cont.Items)
      {
        if (IsContainer(item))
        {
          list.AddRange(OpenContainerRecursive(item.Serial));
        }
      }

      return list;
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    [BlockMultipleExecutions]
    public void FillLavaBombKad()
    {
      Potion potion = Potion.LavaBomb;
      Alchemy alch = Game.CurrentGame.CurrentPlayer.GetSkillInstance<Alchemy>();
      UOItem kad = alch.GetKadAny(potion);

      while (kad.Exist && World.Player.Backpack.AllItems.FindType(potion.DefaultGraphic, potion.DefaultColor).Exist)
      {
        UO.WaitTargetType(potion.DefaultGraphic, potion.DefaultColor);
        kad.Use();
        Game.Wait();
      }
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    [BlockMultipleExecutions]
    public void ThrowLavaBomb()
    {
      Game.RunScript(1000);
      Potion potion = Potion.LavaBomb;
      UOItem potionItem = null;
      string text = "";

      foreach (UOItem item in UO.Backpack.AllItems)
      {
        if (item.Graphic == potion.DefaultGraphic && item.Color == potion.DefaultColor && item.Flags == 0x0000)
        {
          potionItem = item;



          break;
        }
      }

      if (potionItem == null || !potionItem.Exist)
      {
        Alchemy alch = Game.CurrentGame.CurrentPlayer.GetSkillInstance<Alchemy>();
        potionItem = alch.GetPotionFromKad(potion);
        potionItem.Move(1, World.Player.Backpack);
        Game.Wait(250);
      }

      if (potionItem != null && potionItem.Exist)
      {
        World.Player.PrintMessage("Hazu lavabombu" + text);
        StaticTarget st = UIManager.Target();
        potionItem.Move(1, st.X, st.Y, st.Z);
      }
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    [BlockMultipleExecutions]
    public void UsumonLavaBomb()
    {
      UsumonLavaBomb(null);
    }

    [Executable]
    [BlockMultipleExecutions]
    public void UsumonLavaBomb(string targets)
    {
      Game.RunScript(2500);
      Alchemy alch = Game.CurrentGame.CurrentPlayer.GetSkillInstance<Alchemy>();
      Potion potion = Potion.LavaBomb;
      UOItem potionItem = World.Player.Backpack.AllItems.FindType(potion.DefaultGraphic, potion.DefaultColor);

      if (potionItem.Exist)
      {
        potionItem.DropHere();
        Game.Wait(200);
        potionItem.Move(1, (ushort)(potionItem.X + 1), (ushort)(potionItem.Y + 1), potionItem.Z);
        Game.Wait(200);
        potionItem.Move(1, World.Player.Backpack, 30, 30);
        Game.Wait(200);
      }
      else if (World.Player.Backpack.AllItems.FindType(Potion.KadGraphic, potion.TopKadColor).Exist)
      {
        potionItem = alch.GetPotionFromKad(potion);
      }

      if (potionItem.Exist)
      {
        World.Player.PrintMessage("[Unsumon lava..]");

        Serial target = Targeting.ParseTargets(targets);
        if (target.IsValid && new UOObject(target).Exist)
          UO.WaitTargetObject(target);

        potionItem.Use();
      }
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    [BlockMultipleExecutions]
    public static void SortBasicBackpack()
    {
      if (ItemLibrary.Mortar.MoveBackpack(0, 10, 30, 0, World.Player.Backpack))
        Game.Wait();
      if (ItemLibrary.RuneBook.MoveBackpack(0, 10, 45, 0, World.Player.Backpack))
        Game.Wait();
      if (ItemLibrary.TravelBook.MoveBackpack(0, 10, 60, 0, World.Player.Backpack))
        Game.Wait();
      if (ItemLibrary.SpellBook.MoveBackpack(0, 10, 75, 0, World.Player.Backpack))
        Game.Wait();
      if (ItemLibrary.KeyRing.MoveBackpack(0, 10, 90, 0, World.Player.Backpack))
        Game.Wait();
      if (ItemLibrary.DwarfKnife.MoveBackpack(0, 10, 105, 0, World.Player.Backpack))
        Game.Wait();
      if (ItemLibrary.PoisonKit.MoveBackpack(0, 10, 120, 0, World.Player.Backpack))
        Game.Wait();
      if (ItemLibrary.Voditko.Move(0, 10, 140, 0, World.Player.Backpack))
        Game.Wait();

      if (ItemLibrary.CleanBandages.Move(0, 10, 155, 0, World.Player.Backpack))
        Game.Wait();

      if (ItemLibrary.MagickyBrk.Move(0, 10, 170, 0, World.Player.Backpack))
        Game.Wait();

      if (ItemLibrary.DuchStepi.Move(0, 150, 30, 0, World.Player.Backpack))
        Game.Wait();

      if (ItemLibrary.DuchPralesa.Move(0, 170, 30, 0, World.Player.Backpack))
        Game.Wait();


      if (ItemLibrary.StoneVampKrystal.Move(Precision.GraphicColor, Search.Backpack, 0, 120, 10, 0, World.Player.Backpack))
        Game.Wait();

      if (ItemLibrary.VampKrystal.Move(Precision.GraphicColor, Search.Backpack, 0, 135, 10, 0, World.Player.Backpack))
        Game.Wait();

      if (ItemLibrary.StoneKad.Move(Precision.GraphicColor, Search.Backpack, 0, 150, 10, 0, World.Player.Backpack))
        Game.Wait();

      if (ItemLibrary.GreeziArtefakt.Move(Precision.GraphicColor, Search.Backpack, 0, 170, 10, 0, World.Player.Backpack))
        Game.Wait();



      if (ItemLibrary.CestovniKniha.Move(0, 180, 30, 0, World.Player.Backpack))
        Game.Wait();

      if (ItemLibrary.BoltQuiver.Move(Precision.GraphicColor, Search.Backpack, 0, 180, 45, 0, World.Player.Backpack))
        Game.Wait();

      if (ItemLibrary.BoltQuiver.Move(Precision.GraphicColor, Search.Backpack, 0, 185, 45, 0, World.Player.Backpack))
        Game.Wait();

      if (ItemLibrary.Scissors.MoveBackpack(0, 180, 60, 0, World.Player.Backpack))
        Game.Wait();

      if (ItemLibrary.LockNaramek.Move(Precision.GraphicColor, Search.Backpack, 0, 180, 75, 0, World.Player.Backpack))
        Game.Wait();

      if (ItemLibrary.KlanKniha.MoveBackpack(0, 180, 90, 0, World.Player.Backpack))
        Game.Wait();

      if (ItemLibrary.KlanKad.MoveBackpack(0, 180, 105, 0, World.Player.Backpack))
        Game.Wait();

      if (ItemLibrary.Backpack.MoveBackpack(0, 180, 120, 0, World.Player.Backpack))
        Game.Wait();

      if (ItemLibrary.BeltPouch.MoveBackpack(0, 180, 130, 0, World.Player.Backpack))
        Game.Wait();

      if (ItemLibrary.NbRuna.Move(0, 180, 140, 0, World.Player.Backpack))
        Game.Wait();

      if (ItemLibrary.DungeonScrool.Move(0, 180, 155, 0, World.Player.Backpack))
        Game.Wait();

      if (ItemLibrary.EmptyBottle.Move(0, 180, 170, 0, World.Player.Backpack))
        Game.Wait();


      ushort x = 15;
      
      foreach (UOItem item in World.Player.Backpack.Items)
      {
        if (ReagentCollection.MagReagents.Contains(item))
        {
          if (item.Move(item.Amount, item.Container, x, 180))
          {
            x += 10;

          }
          Game.Wait(250);
        }

      }

      Jewelry.SetridSperky();
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void MoveitemType()
    {
      MoveitemType(100000);
    }
    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void MoveitemType(int quantity)
    {
      MoveitemType(quantity, false, 1);
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void MoveitemType(int quantity, bool typeOnly, int amount)
    {
      Game.PrintMessage("Vyber typ >");
      UOItem type = new UOItem(UIManager.TargetObject());
      UOItem containerFrom = new UOItem(type.Container);
      Game.PrintMessage("Bagl do >");
      UOItem containerTo = new UOItem(UIManager.TargetObject());

      UOColor typeColor = type.Color;
      if (typeOnly)
        typeColor = UOColor.Invariant;

      if (amount == 0)
      {
        List<UOItem> toMove = new List<UOItem>();
        foreach (UOItem search in containerFrom.Items)
        {
          if (search.Distance < 5 && search.Graphic == type.Graphic && search.Color == typeColor)
          {
            toMove.Add(search);
          }
        }
        Game.PrintMessage("ToMove:" + toMove.Count);
        foreach (UOItem item in toMove)
        {
          item.Move(amount == 0 ? item.Amount : (ushort)amount, containerTo);
          Game.Wait();
        }
      }
      else
      {
        while (containerFrom.Items.FindType(type.Graphic, typeColor).Exist)
        {
          UOItem item = null;
          foreach (UOItem search in containerFrom.Items)
          {
            if (search.Distance < 5 && search.Graphic == type.Graphic && search.Color == typeColor)
            {
              item = search;
              break;
            }
          }

          if (item == null)
          {
            Game.PrintMessage("item == null");
            break;
          }

          item.Move(amount == 0 ? item.Amount : (ushort)amount, containerTo);
          Game.Wait();
        }
      }

      Game.PrintMessage("Konec prenosu.");
    }



    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void opravstaty()
    {
      DateTime start = DateTime.Now;

      lastBracelet = World.Player.Layers[Layer.Bracelet];
      lastRing = World.Player.Layers[Layer.Ring];
      lastEarrings= World.Player.Layers[Layer.Earrings];
      lastNeck = World.Player.Layers[Layer.Neck];
      lastLeftHand = World.Player.Layers[Layer.LeftHand];
      lastRightHand = World.Player.Layers[Layer.RightHand];

      lastOuterTorso = World.Player.Layers[Layer.OuterTorso];
      lastOuterLegs = World.Player.Layers[Layer.OuterLegs];

      lastHat = World.Player.Layers[Layer.Hat];
      lastGloves = World.Player.Layers[Layer.Gloves];
      lastArms = World.Player.Layers[Layer.Arms];

      lastInnerLegs = World.Player.Layers[Layer.InnerLegs];
      lastMiddleTorso = World.Player.Layers[Layer.MiddleTorso];

      lastShoes = World.Player.Layers[Layer.Shoes];
      lastInnerTorso = World.Player.Layers[Layer.InnerTorso];

      lastCloak = World.Player.Layers[Layer.Cloak];

      lastShirt = World.Player.Layers[Layer.Shirt];
      lastPants = World.Player.Layers[Layer.Pants];

      lastWaist = World.Player.Layers[Layer.Waist];

      World.Player.ChangeWarmode(WarmodeChange.Switch);
      Game.Wait(250);

      //Serial: 0x400C234F  Name: "Caleb III.'s Travel Book"  Position: 10.60.0  Flags: 0x0000  Color: 0x0482  Graphic: 0x0FEF  Amount: 1  Layer: None Container: 0x40222A86
      //Serial: 0x401E078C  Name: "Cestovni kniha"  Position: 180.30.0  Flags: 0x0000  Color: 0x0000  Graphic: 0x22C5  Amount: 1  Layer: None Container: 0x40222A86

       if (World.Player.Backpack.Items.FindType(0x22C5).Exist)
       {
         
      UO.WaitMenu("Po pouziti vam budou opraveny staty na spravne hodnoty. Pozor, pri oprave vam spadne vase brneni do batuzku!", "Ano, oprav");
      Kniha.Current.CestovniKnihaUse(2);
      //UO.Exec("CestovniKnihaUse", 2);
      //new Kniha().CestovniKnihaUse(2);

      }
      else if (World.Player.Backpack.Items.FindType(0x0FEF, 0x0482).Exist)
      {
        UO.WaitMenu("Po pouziti vam budou opraveny staty na spravne hodnoty. Pozor, pri oprave vam spadne vase brneni do batuzku!", "Ano, oprav");
        Kniha.Current.TravelBookUse(2);
      }

      JournalEventWaiter jew = new JournalEventWaiter(true, "You put");
      if (jew.Wait(1500))
      {
        if (lastBracelet.Exist && lastBracelet.Container == World.Player.Backpack.Serial)
        {
          lastBracelet.Use();
          Game.Wait();
        }

        if (lastRing.Exist && lastRing.Container == World.Player.Backpack.Serial)
        {
          lastRing.Use();
          Game.Wait();
        }

        if (lastEarrings.Exist && lastEarrings.Container == World.Player.Backpack.Serial)
        {
          lastEarrings.Use();
          Game.Wait();
        }


        if (lastNeck.Exist && lastNeck.Container == World.Player.Backpack.Serial)
        {
          lastNeck.Use();
          Game.Wait();
        }

        if (lastLeftHand.Exist && lastLeftHand.Container == World.Player.Backpack.Serial)
        {
          lastLeftHand.Use();
          Game.Wait();
        }

        if (lastRightHand.Exist && lastRightHand.Container == World.Player.Backpack.Serial)
        {
          lastRightHand.Use();
          Game.Wait();
        }

        if (lastOuterTorso.Exist && lastOuterTorso.Container == World.Player.Backpack.Serial)
        {
          lastOuterTorso.Use();
          Game.Wait();
        }

        if (lastOuterLegs.Exist && lastOuterLegs.Container == World.Player.Backpack.Serial)
        {
          lastOuterLegs.Use();
          Game.Wait();
        }

        if (lastHat.Exist && lastHat.Container == World.Player.Backpack.Serial)
        {
          lastHat.Use();
          Game.Wait();
        }

        if (lastGloves.Exist && lastGloves.Container == World.Player.Backpack.Serial)
        {
          lastGloves.Use();
          Game.Wait();
        }

        if (lastArms.Exist && lastArms.Container == World.Player.Backpack.Serial)
        {
          lastArms.Use();
          Game.Wait();
        }

        if (lastInnerLegs.Exist && lastInnerLegs.Container == World.Player.Backpack.Serial)
        {
          lastInnerLegs.Use();
          Game.Wait();
        }

        if (lastMiddleTorso.Exist && lastMiddleTorso.Container == World.Player.Backpack.Serial)
        {
          lastMiddleTorso.Use();
          Game.Wait();
        }

        if (lastShoes.Exist && lastShoes.Container == World.Player.Backpack.Serial)
        {
          lastShoes.Use();
          Game.Wait();
        }

        if (lastInnerTorso.Exist && lastInnerTorso.Container == World.Player.Backpack.Serial)
        {
          lastInnerTorso.Use();
          Game.Wait();
        }

        if (lastCloak.Exist && lastCloak.Container == World.Player.Backpack.Serial)
        {
          lastCloak.Use();
          Game.Wait();
        }

        if (lastShirt.Exist && lastShirt.Container == World.Player.Backpack.Serial)
        {
          lastShirt.Use();
          Game.Wait();
        }

        if (lastPants.Exist && lastPants.Container == World.Player.Backpack.Serial)
        {
          lastPants.Use();
          Game.Wait();
        }


        if (lastWaist.Exist && lastWaist.Container == World.Player.Backpack.Serial)
        {
          lastWaist.Use();
          Game.Wait();
        }
      }

      Game.PrintMessage("Oprava dokoncena za: " + (DateTime.Now - start).TotalSeconds.ToString("N1") + "s");
    }

    //---------------------------------------------------------------------------------------------

    [Command]
    public static void luxing(int wait)
    {
      Game.PrintMessage("Odkud >");
      UOItem containerFrom = new UOItem(UIManager.TargetObject());

      Game.PrintMessage("Kam (ESC = backpack) >");
      UOItem containerTo = new UOItem(UIManager.TargetObject());
      if (!containerTo.ExistCust())
        containerTo = World.Player.Backpack;

      containerFrom.Use();
      Game.Wait();


      List<UOItem> items = new List<UOItem>();
      items.AddRange(containerFrom.Items.ToArray());



      Game.PrintMessage("Pocet: " + items.Count + " ETA: " + String.Format("{0:N2}", ((decimal)items.Count * (decimal)wait) / 1000.0m));

      int couter = 0;
      foreach (UOItem item in items)
      {
        couter++;
        if (couter % 4 == 0)
          Game.PrintMessage("ETA: " + String.Format("{0:N2}", ((decimal)(items.Count - couter ) * (decimal)wait) / 1000.0m));

        item.Move(65000, containerTo);
        Game.Wait(wait);
      }
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void MoveitemAll(int amount)
    {
      Game.PrintMessage("Vyber typ >");
      UOItem containerFrom = new UOItem(UIManager.TargetObject());
      Game.PrintMessage("Bagl do >");
      UOItem containerTo = new UOItem(UIManager.TargetObject());


      while (containerFrom.Items.CountItems() > 0)
      {
        UOItem item = containerFrom.Items.FirstOrDefault();
        if (item == null)
          break;
        item.Move((ushort)amount, containerTo);
        Game.Wait();
      }

      Game.PrintMessage("Konec prenosu.");
    }


    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void SortitemType()
    {
      Game.PrintMessage("Vyber typ >");
      UOItem type = new UOItem(UIManager.TargetObject());
      UOItem containerFrom = new UOItem(type.Container);
      Game.PrintMessage("Item kam >");
      UOItem itemTo = new UOItem(UIManager.TargetObject());
      UOItem containerTo = new UOItem(itemTo.Container);
      if (!containerTo.Exist || !containerTo.Serial.IsValid)
        containerTo = containerFrom;

      ushort x, y;
      x = itemTo.X;
      y = itemTo.Y;
      List<UOItem> sortItems = new List<UOItem>();

      foreach (UOItem item in containerFrom.Items)
      {
        if (item.Graphic == type.Graphic && item.Color == type.Color)
        {
          sortItems.Add(item);
        }
      }

      foreach (UOItem sortItem in sortItems)
      {
        sortItem.Move(1, containerTo, x, y);
        Game.Wait(250);
      }

      Game.PrintMessage("Konec prenosu.");
    }

    [Executable]
    public static void SortitemType(ushort x, ushort y)
    {
      Game.PrintMessage("Vyber typ >");
      UOItem type = new UOItem(UIManager.TargetObject());
      UOItem containerFrom = new UOItem(type.Container);
      Game.PrintMessage("Bagl do >");
      UOItem containerTo = new UOItem(UIManager.TargetObject());
      if (!containerTo.Exist || !containerTo.Serial.IsValid)
        containerTo = containerFrom;

      List<UOItem> sortItems = new List<UOItem>();

      foreach (UOItem item in containerFrom.Items)
      {
        if (item.Graphic == type.Graphic && item.Color == type.Color)
        {
          sortItems.Add(item);
        }
      }

      foreach (UOItem sortItem in sortItems)
      {
        sortItem.Move(1, containerTo, x, y);
        Game.Wait(250);
      }

      Game.PrintMessage("Konec prenosu.");
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void SortitemName()
    {
      Game.PrintMessage("Vyber typ >");
      UOItem type = new UOItem(UIManager.TargetObject());
      UOItem containerFrom = new UOItem(type.Container);
      Game.PrintMessage("Item kam >");
      UOItem itemTo = new UOItem(UIManager.TargetObject());
      UOItem containerTo = new UOItem(itemTo.Container);
      if (!containerTo.Exist || !containerTo.Serial.IsValid)
        containerTo = containerFrom;

      ushort x, y;
      x = itemTo.X;
      y = itemTo.Y;
      List<UOItem> sortItems = new List<UOItem>();

      string name = String.Empty;
      if (String.IsNullOrEmpty(type.Name))
      {
        type.Click();
        Game.Wait();
       
      }
      name = type.Name;

      foreach (UOItem item in containerFrom.Items)
      {
        if (String.IsNullOrEmpty(item.Name))
        {
          item.Click();
          Game.Wait(200);
        }

        if (!String.IsNullOrEmpty(item.Name) && item.Name == name)
        {
          sortItems.Add(item);
        }
      }

      foreach (UOItem sortItem in sortItems)
      {
        sortItem.Move(1, containerTo, x, y);
        Game.Wait(250);
      }

      Game.PrintMessage("Konec prenosu.");
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void PrintDirection()
    {
      UOCharacter a = new UOCharacter(UIManager.TargetObject());

      UOPositionBase p1 = new UOPositionBase(World.Player.X, World.Player.Y, 0);
      UOPositionBase p2 = new UOPositionBase(a.X, a.Y, 0);



      Game.PrintMessage("Direct:" + a.Direction + " Angle: " + Robot.GetAngle(p2, p1) + " LookingDirectlyAt: " + Robot.LookingDirectlyAt(p2, p1, a.Direction) + " A: " + Robot.GetRelativeVectorLength(p2, p1));
    }


    [Executable]
    public static void MoveRegyAll()
    {
      MoveItem(65000.ToString(), "regy", false);
    }

    [Executable]
    public static void MoveMRegy200()
    {
      MoveItem(200.ToString(), "magregy", false);
    }

    [Executable]
    public static void MoveMRegy1000()
    {
      MoveItem(1000.ToString(), "magregy", false);
    }

    [Executable]
    public static void MoveNRegy50()
    {
      MoveItem(50.ToString(), "necroregy", false);
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void MoveItem(string amount, string type)
    {
      MoveItem(amount, type, false);
    }

    [Executable]
    public static void MoveItem(string amount, string type, bool recursiveFrom)
    {
      MoveItem(amount, type, recursiveFrom, null, null);
    }

    public static void MoveItem(string amount, string type, bool recursiveFrom, UOItem containerFrom, UOItem containerTo)
    {
      Game.PrintMessage("Bagl z >");
      if (containerFrom == null)
        containerFrom = new UOItem(UIManager.TargetObject());
      Game.PrintMessage("Bagl do >");
      if (containerTo == null)
        containerTo = new UOItem(UIManager.TargetObject());

      EnsureContainer(containerFrom);
      ushort a = 0;
      a = ushort.Parse(amount);

      List<UOItem> toMove = new List<UOItem>();

      List<UOItem> items = new List<UOItem>();
      items.AddRange(containerFrom.Items.ToArray());

      if (recursiveFrom)
      {
        items.AddRange(ItemHelper.OpenContainerRecursive(containerFrom.Serial));
      }


      foreach (UOItem item in items)
      {
        if (type == "all")
        {
          toMove.Add(item);
        }
        else if (type == "regy")
        {
          if (ReagentCollection.Reagents.Contains(item))
            toMove.Add(item);
        }
        else if (type == "magregy")
        {
          if (ReagentCollection.MagReagents.Contains(item))
            toMove.Add(item);
        }
        else if (type == "necroregy")
        {
          if (ReagentCollection.NecroReagents.Contains(item))
            toMove.Add(item);
        }
        if (type == "noncontainer")
        {
          if (!(item.Graphic == 0x0E76 || item.Graphic == 0x0E75 || item.Graphic == 0x0E77 || item.Graphic == 0x0E78 || item.Graphic == 0x0E79 || item.Graphic == 0x0E7D))
            toMove.Add(item);
        }
      }

      Dictionary<string, int> amountCounter = new Dictionary<string, int>();

      foreach (UOItem item in toMove)
      {
        string key = item.Graphic + "|" + item.Color;

        if (!amountCounter.ContainsKey(key))
          amountCounter.Add(key, 0);

        if (amountCounter[key] >= a)
          continue;

        item.Move(a, containerTo.Serial);
        Game.Wait(250 + Core.Latency);

        amountCounter[key] += item.Amount;
      }

      //if (recursiveFrom)
      //{
      //  foreach (UOItem item in containerFrom.Items)
      //  {
      //    if (item.Graphic == 0x0E76 || item.Graphic == 0x0E75 || item.Graphic == 0x0E77 || item.Graphic == 0x0E78 || item.Graphic == 0x0E79 || item.Graphic == 0x0E7D)
      //    {
      //      MoveItem(amount, type, true, item, containerTo);
      //    }
      //  }
      //}

      Game.PrintMessage("Konec prenosu.");
    }

    //---------------------------------------------------------------------------------------------
    public static Graphic BodyGraphic { get { return new Graphic(0x2006); } }



    //---------------------------------------------------------------------------------------------

    [Executable]
    public void PrintPlayerDistance()
    {
      UO.Print("PlayerDistance:" + World.Player.Distance + " - " + World.Player.Description);
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void OpenDoorAll()
    {
      //World.Player.PrintMessage("Oteviram dvere");
      //UO.OpenDoor(); nefunguje to uplne optimalne

      List<UOItem> openDoors = new List<UOItem>();
      List<UOItem> closeDoors = new List<UOItem>();

      foreach (UOItem item in World.Ground)
      {
        if (item.Distance <= 3)
        {
          if (OpenDoors.IndexOf(item.Graphic) > -1)
          {
            if (item.Z == World.Player.Z)
              openDoors.Add(item);
          }
          else if (CloseDoors.IndexOf(item.Graphic) > -1)
          {
            if (item.Z == World.Player.Z)
              closeDoors.Add(item);;
          }
        }
      }

      if (openDoors.Count > 0)
      {
        foreach (UOItem door in openDoors)
        {
          door.Use();
          World.Player.PrintMessage("Zaviram dvere");
          Game.Wait();
        }
      }
      else if (closeDoors.Count > 0)
      {
        foreach (UOItem door in closeDoors)
        {
          door.Use();
          World.Player.PrintMessage("Oteviram dvere");
          Game.Wait();
        }
      }
      else
      {
        World.Player.PrintMessage("Zadny dvere tu nejsou");
        UO.OpenDoor();

      }
    }

    //Serial: 0x4030513B  Name: "Spolek Secure Rune"  Position: 3351.350.10  Flags: 0x0000  Color: 0x0481  Graphic: 0x0E62  Amount: 0  Layer: None Container: 0x00000000

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void GuildRune()
    {
      List<UOItem> know = World.Ground.Where(i => i.Graphic == 0x0E62 && i.Distance < 3).OrderBy(i => i.Distance).ToList();
      if (know.Count > 0)
      {
        if (know[0].Color == 0x0481)
          World.Player.Print("[GRuna OFF..]");
        else
          World.Player.Print("[GRuna ON..]");

        know[0].Use();
      }
      else
        World.Player.Print("[Neni tu runa..]");
    }

    //---------------------------------------------------------------------------------------------
    //Serial: 0x401FF020  Name: "Gate"  Position: 3176.21.46  Flags: 0x0000  Color: 0x0000  Graphic: 0x373A  Amount: 0  Layer: None  Container: 0x00000000

    [Executable]
    public static void OpenDoor()
    {
   //   StandardSkill.
      List<UOItem> know = World.Ground.Where(i => OpenDoors.IndexOf(i.Graphic) > -1 || CloseDoors.IndexOf(i.Graphic) > -1 ||
      i.Graphic == 0x1BBF ||
      i.Graphic == 0x1090 ||
      i.Graphic == 0x1091 ||
      i.Graphic == 0x1092 ||
      i.Graphic == 0x1093 ||
      i.Graphic == 0x1094 ||
      i.Graphic == 0x1095 ||
      i.Graphic == 0x373A //Gata
      //|| (i.Graphic == 0x0E62 && i.Distance == 0)//Guild runa
      ).Where(i=> i.Distance < 5).OrderBy(i => Math.Abs(i.Z - World.Player.Z)).OrderBy(i => i.GetDistance() ).ToList();


      List<UOItem> unknown = World.Ground.Where(i => i.Distance <= 2).OrderBy(i => Math.Abs(i.Z - World.Player.Z)).OrderBy(i => i.GetDistance()).ToList();

      UOItem item = new UOItem(Serial.Invalid);

      if (know.Count > 0 && (know[0].Distance <= 1 || unknown.Count == 0 || unknown[0].Distance >= know[0].Distance))
        item = know[0];
      else if (unknown.Count > 0)
        item = unknown[0];

      if (item.Exist)
      {
        item.Use();
        item.PrintMessage("[Use...]");
      }
      else
      {
        World.Player.PrintMessage("[UO.OpenDoor...]");
        UO.OpenDoor();
      }
    }

    //---------------------------------------------------------------------------------------------
    //0x09A8  - animal box
    [Executable]
    public static void ZviratkaDoBoxiku()
    {
      Game.PrintMessage("Vyber kontejner s Animal boxy >");
      UOItem containerFrom = new UOItem(UIManager.TargetObject());
      Game.PrintMessage("Vyber kontejner se zviratky (esc = backpack) >");
      UOItem containerTo = new UOItem(UIManager.TargetObject());

      if (containerTo == Serial.Invalid)
        containerTo = World.Player.Backpack;

      EnsureContainer(containerTo);
      Game.Wait(500);
      EnsureContainer(containerFrom);

      Dictionary<Serial, UOItemType> mapping = new Dictionary<Serial, UOItemType>();
      //Pouzij na shrinkle zvire nebo na sebe!

      foreach (UOItem item in containerFrom.AllItems)
      {
        if (item.Graphic == 0x09A8 && !mapping.ContainsKey(item.Serial))
        {
          if (String.IsNullOrEmpty(item.Name))
            ItemHelper.EnsureItem(item);

          UO.Print("Hledam: " + item.Name);
          //else
          //  Game.Wait(250);

          UOItem animalItem = new UOItem(Serial.Invalid);
          string lowerName = item.Name.ToLower();

          foreach (UOItem subItem in containerTo.Items)
          {

            if (String.IsNullOrEmpty(subItem.Name))
              ItemHelper.EnsureItem(subItem);
            //else
            //  Game.Wait(250);

            if (subItem != null && subItem.Name.ToLower().EndsWith(lowerName))
            {
              animalItem = subItem;
              break;
            }
          }

          if (animalItem.Serial != Serial.Invalid)
          {
            mapping.Add(item.Serial, new UOItemType() { Graphic = animalItem.Graphic, Color = animalItem.Color });
            Game.PrintMessage("Pridan animal box: " + item.Name + ", pocet zviratek: " + containerTo.Items.Count(animalItem.Graphic, animalItem.Color));

            int sychr = 0;
            int count = containerTo.Items.Count(animalItem.Graphic, animalItem.Color);

            for (int i = 0; i < count; i++)
            {
              sychr++;

              if (item.Exist)
              {
                containerTo.Items.FindType(mapping[item.Serial].Graphic, mapping[item.Serial].Color).WaitTarget();
                UO.UseObject(item.Serial);
                Game.Wait(500);
              }
              else
                break;

              if (UO.InJournal("Pouzij na shrinkle zvire nebo na sebe!"))
              {
                Journal.Clear();
                break;
              }

              if (sychr > 1000)
              {
                break;
              }
            }
          }
        }
      }
    }

    [Executable]
    public static void StandAloneZviratkaDoBoxiku()
    {
      UO.Print("Vyber kontejner s Animal boxy >");
      UOItem containerFrom = new UOItem(UIManager.TargetObject());
      UO.Print("Vyber kontejner se zviratky (esc = backpack) >");
      UOItem containerTo = new UOItem(UIManager.TargetObject());

      if (containerTo == Serial.Invalid)
        containerTo = World.Player.Backpack;

      EnsureContainer(containerTo);
      UO.Wait(500);
      EnsureContainer(containerFrom);

      Dictionary<Serial, object[]> mapping = new Dictionary<Serial, object[]>();
      //Pouzij na shrinkle zvire nebo na sebe!

      foreach (UOItem item in containerFrom.AllItems)
      {
        if (item.Graphic == 0x09A8 && !mapping.ContainsKey(item.Serial))
        {
          if (String.IsNullOrEmpty(item.Name))
            EnsureItem(item);
          //else
          //  Game.Wait(250);

          UO.Print("Hledam: " + item.Name);

          UOItem animalItem = new UOItem(Serial.Invalid);
          string lowerName = item.Name.ToLower();

          foreach (UOItem subItem in containerTo.Items)
          {
            if (String.IsNullOrEmpty(subItem.Name))
              EnsureItem(subItem);
            //else
            //  Game.Wait(250);

            if (subItem.Name.ToLower().EndsWith(lowerName))
            {
              animalItem = subItem;
              break;
            }
          }

          if (animalItem.Serial != Serial.Invalid)
          {
            mapping.Add(item.Serial, new object[] { animalItem.Graphic, animalItem.Color });
            UO.Print("Pridan animal box: " + item.Name + ", pocet zviratek: " + containerTo.Items.Count(animalItem.Graphic, animalItem.Color));

            int sychr = 0;
            int count = containerTo.Items.Count((Graphic)mapping[item.Serial][0], (UOColor)mapping[item.Serial][1]);

            for (int i = 0; i < count; i++)
            {
              sychr++;

              if (item.Exist)
              {
                containerTo.Items.FindType((Graphic)mapping[item.Serial][0], (UOColor)mapping[item.Serial][1]).WaitTarget();
                UO.UseObject(item.Serial);
                UO.Wait(500);
              }
              else
                break;


              if (UO.InJournal("Pouzij na shrinkle zvire nebo na sebe!"))
              {
                Journal.Clear();
                break;
              }

              if (sychr > 1000)
              {
                break;
              }
            }
          }
        }
      }
    }
    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void SortBackpack1()
    {
      List<UOItem> items = new List<UOItem>();

      foreach (UOItem item in World.Player.Backpack.Items)
      {
        items.Add(item);
      }

      foreach (UOItem item in items)
      {
        if (item.Graphic == 0x01843 && item.Color == 0x08A7)//GH kad
          item.Move(1, World.Player.Backpack, 45, 65);
        else if (item.Graphic == 0x01843 && item.Color == 0x0481)//GS kad
          item.Move(1, World.Player.Backpack, 50, 65);
        else if (item.Graphic == 0x01843 && item.Color == 0x014D)//TR kad
          item.Move(1, World.Player.Backpack, 55, 65);
        else if (item.Graphic == 0x0F06 && item.Color == 0x0003)//TMR kad
          item.Move(1, World.Player.Backpack, 65, 65);
        else if (item.Graphic == 0x01843 && item.Color == 0x0000)//NS potion
          item.Move(1, World.Player.Backpack, 75, 65);
        else if (item.Graphic == 0x0F09 && item.Color == 0x0000)//GS potion
          item.Move(1, World.Player.Backpack, 85, 65);
        else if (item.Graphic == 0x0F0B && item.Color == 0x0000)//TR potion
          item.Move(1, World.Player.Backpack, 95, 65);
        else if (item.Graphic == 0x0F09 && item.Color == 0x0003)//TMR potion
          item.Move(1, World.Player.Backpack, 105, 65);
        else if (item.Graphic == 0x0F0C && item.Color == 0x0000)//GH potion
          item.Move(1, World.Player.Backpack, 115, 65);
        else if (item.Graphic == 0x0F07 && item.Color == 0x0000)//GC potion
          item.Move(1, World.Player.Backpack, 125, 65);
        else if (item.Graphic == 0x0EFA)//Spellbook
          item.Move(1, World.Player.Backpack, 140, 65);
        else if (item.Graphic == 0x0FF0 && item.Color == 0x08A5)//Rune book
          item.Move(1, World.Player.Backpack, 140, 80);
        else if (item.Graphic == 0x0FEF && item.Color == 0x0482)//Travel book
          item.Move(1, World.Player.Backpack, 140, 90);
        else if (item.Graphic == 0x0E76 && item.Color == 0x0000)//Bag s regy
          item.Move(1, World.Player.Backpack, 140, 115);
        else if (item.Graphic == 0x176B && item.Color == 0x0000)//Key ring
          item.Move(1, World.Player.Backpack, 140, 135);
        else if (item.Graphic == 0x0E20 && item.Color == 0x0000)//Blood bandy
          item.Move(10000, World.Player.Backpack, 45, 135);
        else if (item.Graphic == 0x0E21 && item.Color == 0x0000)//Bandy
          item.Move(10000, World.Player.Backpack, 65, 135);
        else if (item.Graphic == 0x0F0E && item.Color == 0x0000)//Prazdne lahve
          item.Move(10000, World.Player.Backpack, 45, 100);

        Game.Wait();
      }
    }

    //---------------------------------------------------------------------------------------------
    [Executable]
    public static void MoveIgnotUnderFeat(ushort amount)
    {
      World.Player.PrintMessage("MoveIgnotUnderFeat start");

      int count = 0;
      foreach (UOItem item in World.Ground)
      {
        if (Mining2.IsIngot(item) && item.Distance > 0)
        {
          item.Move(amount, World.Player.X, World.Player.Y, World.Player.Z);
          Game.Wait();
          count += amount;

          Game.PrintMessage("Total: " + count);
        }
      }

      World.Player.PrintMessage("MoveIgnotUnderFeat end");
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void MoveitemTypeUnderFeat(ushort amount)
    {
      MoveitemTypeUnderFeat(amount, 0);
    }

    [Executable]
    public static void MoveitemTypeUnderFeat(ushort amount, int totalItems)
    {
      Game.PrintMessage("Vyber typ >");
      UOItem type = new UOItem(UIManager.TargetObject());
      UOItem containerFrom = new UOItem(type.Container);

      Graphic typeG = type.Graphic;
      UOColor typeC = type.Color;

      int sychr = 0;
      int count = 0;

      while (containerFrom.Items.FindType(typeG, typeC).Exist)
      {
        sychr++;

        foreach (UOItem itemi in containerFrom.Items)
        {
          if (itemi.Graphic == typeG && itemi.Color == typeC && itemi.Distance > 1)
          {
            UOItem item = itemi;
            item.Move(amount, World.Player.X, World.Player.Y, World.Player.Z);
            Game.Wait();
            count += amount;

            if (totalItems > 0 && count >= totalItems)
              break;

            Game.PrintMessage("Total: " + count);
          }
        }
      }

      Game.PrintMessage("Konec prenosu.");
    }


    [Executable]
    public static void MoveitemTypeToBank(ushort amount)
    {
      MoveitemTypeToBank(amount, 0);
    }

    [Executable]
    public static void MoveitemTypeToBank(ushort amount, int totalItems)
    {
      UO.Print("Vyber typ >");
      UOItem type = new UOItem(UIManager.TargetObject());
      UOItem containerFrom = new UOItem(type.Container);

      UO.Print("Bagl do > (esc=bank)");
      UOItem containerTo = new UOItem(UIManager.TargetObject());
      if (!containerTo.Exist)
        containerTo = World.Player.Layers[Layer.Bank];

      Graphic typeG = type.Graphic;
      UOColor typeC = type.Color;

      int sychr = 0;
      int count = 0;

      while (containerFrom.Items.FindType(typeG, typeC).Exist)
      {
        sychr++;
        UOItem item = containerFrom.Items.FindType(typeG, typeC);
        item.Move(amount, containerTo);
        UO.Wait(500);
        count += amount;

        if (totalItems > 0 && count >= totalItems)
          break;

        UO.Print("Total: " + count);
      }

      UO.Print("Konec prenosu.");
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void MoveitemAllType(ushort amount)
    {
      Game.PrintMessage("Vyber typ >");
      UOItem type = new UOItem(UIManager.TargetObject());
      UOItem containerFrom = new UOItem(type.Container);
      Game.PrintMessage("Bagl do >");
      UOItem containerTo = new UOItem(UIManager.TargetObject());

      int sychr = 0;

      while (containerFrom.Items.FindType(type.Graphic, type.Color).Exist)
      {
        sychr++;
        UOItem item = containerFrom.Items.FindType(type.Graphic, type.Color);
        item.Move(amount, containerTo);
        Game.Wait();
      }

      Game.PrintMessage("Konec prenosu.");
    }

    //---------------------------------------------------------------------------------------------
    [Executable]
    public static void MoveIgnotToBank(ushort amount)
    {
      World.Player.PrintMessage("MoveIgnotToBank start");

      int count = 0;

      while (World.Ground.FindType(Blacksmithy.IronIngot.Graphic).Exist && World.Ground.FindType(Blacksmithy.IronIngot.Graphic).Distance == 0)
      {
        UOItem moveItem = World.Ground.FindType(Blacksmithy.IronIngot.Graphic);
        moveItem.Move(amount, World.Player.Layers[Layer.Bank]);
        Game.Wait();
        count += amount;
        Game.PrintMessage("Total: " + count);
      }

      World.Player.PrintMessage("MoveIgnotToBank end");
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void TrideniKockoKlamaku()
    {
      UO.Print("Zdrojovy kontejner >");
      UOItem containerFrom = new UOItem(UIManager.TargetObject());
      UO.Print("Cilovy kontejner (esc = Zdrojovy kontejner) >");
      UOItem containerTo = new UOItem(UIManager.TargetObject());

      if (containerTo == Serial.Invalid)
        containerTo = containerFrom;

      UOItemType cougar = new UOItemType() { Graphic = 0x2119, Color = 0x0000 };
      UOItemType panther = new UOItemType() { Graphic = 0x2119, Color = 0x0901 };
      UOItemType leopard = new UOItemType() { Graphic = 0x2119, Color = 0x0972 };
      UOItemType cat1 = new UOItemType() { Graphic = 0x211B, Color = 0x08FF };
      UOItemType cat2 = new UOItemType() { Graphic = 0x211B, Color = 0x0909 };

      EnsureContainer(containerTo);
      UO.Wait(500);
      EnsureContainer(containerFrom);

      List<UOItem> searchItems = new List<UOItem>();
      searchItems.AddRange(containerFrom.Items);

      UO.Print("Tridim cicmundy ;]");
      foreach (UOItem item in searchItems)
      {
        //UO.Print(((UOItemType)item).ToString() + ", " + ((UOItemType)item == leopard));
        if (cougar.EqualUOItem(item))
        {
          item.Move(1, containerTo, 20, 35);
          Game.Wait();
        }
        else if (panther.EqualUOItem(item))
        {
          item.Move(1, containerTo, 40, 35);
          Game.Wait();
        }
        else if (leopard.EqualUOItem(item))
        {
          item.Move(1, containerTo, 60, 35);
          Game.Wait();
        }
        else if (cat1.EqualUOItem(item))
        {
          item.Move(1, containerTo, 80, 35);
          Game.Wait();
        }
        else if (cat2.EqualUOItem(item))
        {
          item.Move(1, containerTo, 100, 35);
          Game.Wait();
        }

      }
      UO.Print("Tridim cicmundy ;] dokonceno");
    }

    //---------------------------------------------------------------------------------------------

    public class UOItemComparer : IEqualityComparer<UOItem>
    {

      public bool Equals(UOItem x, UOItem y)
      {
        if (x.Graphic == y.Graphic && x.Color == y.Color)
          return true;

        return false;
      }

      public int GetHashCode(UOItem obj)
      {
        return obj.GetHashCode();
      }
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void SortCont(ushort startX, ushort startY)
    {
      Game.PrintMessage("Vyber typ >");
      UOItem type = new UOItem(UIManager.TargetObject());
      Game.PrintMessage("Bagl do >");
      UOItem containerTo = new UOItem(UIManager.TargetObject());

      List<UOItem> toSort = new List<UOItem>();
      List<UOItem> all = new List<UOItem>();
      all.AddRange(new UOItem(type.Container).AllItems);

      foreach (UOItem item in all)
      {
        if (item.Graphic == type.Graphic && item.Color == type.Color)
        {
          toSort.Add(item);
        }
      }

      for (int i = 0; i < toSort.Count; i++)
      {
        UOItem item = toSort[i];

        ushort x = (ushort)((startX) + ((i + 1) * 3));

        item.Move(1, containerTo, x, startY);
        Game.Wait();
      }

      Game.PrintMessage("Konec");
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    [Command("sb")]
    public void Requip(params string[] options)
    {
      List<RequipItem> requipItems = new List<RequipItem>();
      foreach (string str in options)
      {
        RequipItem r = new RequipItem();
        if (r.Parse(str))
          requipItems.Add(r);
      }


      foreach (RequipItem requipItem in requipItems)
      {
        if (requipItem.HandleType == "GET")
        {


        }
      }
    }

    private class RequipItem
    {
      public string HandleType = "GET"; //REFILL ??
      public string ItemType1;
      public string ItemType2;

      public ushort? PositionX;
      public ushort? PositionY;


      public int? MaxItem = 10;
      public int Count = 0;
      public int Amount = 1;

      public UOItemType Type;
      public List<Serial> Items = new List<Serial>();

      public bool Parse(string str)
      {
        return true;
      }


    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void SortArmor()
    {
      Game.PrintMessage("Bagl zdroj >");
      UOItem containerFrom = new UOItem(UIManager.TargetObject());
      Game.PrintMessage("Bagl pytle >");
      UOItem containerTo = new UOItem(UIManager.TargetObject());
      Game.PrintMessage("Kam pytle >");
      UOItem bagleTo = new UOItem(UIManager.TargetObject());

      EnsureContainer(containerFrom);
      EnsureContainer(containerTo);
      EnsureContainer(bagleTo);

      ItemHelper.OpenContainerRecursive(containerFrom);

      List<UOItem> items = new List<UOItem>();
      items.AddRange(containerFrom.AllItems.ToArray());

      Dictionary<string, Serial> typeCont = new Dictionary<string, Serial>();

      foreach (UOItem item in items)
      {
        if (String.IsNullOrEmpty(item.Name))
        {
          item.Click();
          Game.Wait();
        }

        string name = (item.Name + string.Empty).Trim().Replace(Environment.NewLine, " ").Replace("\\r\\n", " ");
        Match m = Regex.Match(name, "of (?<typename>[A-Za-z]*)$");

        if (m.Success)
        {
          string typename = m.Groups["typename"].Value.Trim();
          if (typename.Length < 5)
          {
            UO.Print("invalid typename " + typename);
            continue;
          }

          typename = typename.Substring(0, 4);

          if (!typeCont.ContainsKey(typename))
          {
            typeCont.Add(typename, Serial.Invalid);

            foreach (UOItem e in bagleTo.Items)
            {
              if (e.Graphic == 0x0E76 && e.Items.CountItems() > 0)
              {
                UOItem et = e.Items.FirstOrDefault();
                if (String.IsNullOrEmpty(et.Name))
                {
                  et.Click();
                  Game.Wait();
                }

                if (Regex.IsMatch(et.Name, "of " + typename + "$", RegexOptions.IgnoreCase))
                {
                  typeCont[typename] = et.Serial;
                }

                break;
              }
            }


            if (typeCont[typename] == Serial.Invalid)
            {
              foreach (UOItem p in containerTo.Items)
              {
                if (p.Graphic == 0x0E76 && p.Items.CountItems() == 0)
                {
                  typeCont[typename] = p.Serial;
                  p.Move(1, bagleTo, (ushort)(typeCont.Count * 15), 35);
                  Game.Wait();
                  break;
                }
              }
            }

            UO.Print("typename: '" + typename + "' / " + typeCont[typename]);
            //0x0E76  
          }

          Serial contTo = typeCont[typename];
          if (contTo == Serial.Invalid)
          {
            UO.Print("" + typename + " neni cilovy pytlik.");
            continue;
          }

          item.Move(1, contTo);
          Game.Wait();

        }
      }
    }

    //---------------------------------------------------------------------------------------------

    [Command]
    public static void presunklamaky()
    {
      Game.PrintMessage("zdroj >");
      UOItem z = new UOItem(UIManager.TargetObject());
      Game.PrintMessage("cil >");
      UOItem c = new UOItem(UIManager.TargetObject());

      if (!z.Opened)
      {
        z.Use();
        Game.Wait();
      }

      foreach (UOItem item in z.Items)
      {
        foreach (Graphic g in ItemLibrary.ShrinkKlamaciArray)
        {
          if (item.Graphic == g)
          {
            item.Move(1, c);
            Game.Wait();
          }
        }
      }
    }

    //---------------------------------------------------------------------------------------------

    [Command]
    public static void seradpytle()
    {
      Game.PrintMessage("Bedna >");
      UOItem c = new UOItem(UIManager.TargetObject());
      seradpytle(c);
    }

    //---------------------------------------------------------------------------------------------

    public static void seradpytle(UOItem c)
    {
      if (!c.Opened)
      {
        c.Use();
        Game.Wait();
      }
      List<UOItem> items = new List<UOItem>();

      foreach (UOItem item in c.Items)
      {
        if (item.Graphic == 0x0e76)
        {
          items.Add(item);

          if (!item.Opened)
          {
            item.Use();
            Game.Wait();
          }
        }
      }

      var sorted = items.OrderBy(i => i.AllItems.Count()).Reverse().ToArray();

      ushort x = 5;
      ushort y = 40;

      for (int i = 0; i < sorted.Length; i++)
      {
        if (x > 150)
        {
          x = 5;
          y += 11;
        }
        UOItem item = sorted[i];
        item.Move(1, c, x, y);
        Game.Wait();

        x += 11;
      }
    }

    //---------------------------------------------------------------------------------------------


    [Executable]
    public static void rozlustMapy()
    {

      Game.PrintMessage("Vyber zdroj");
      UOItem sourceCont = new UOItem(UIManager.TargetObject());
      Game.PrintMessage("Vyber Cil Lvl 1");
      UOItem targetCont1 = new UOItem(UIManager.TargetObject());
      Game.PrintMessage("Vyber Cil Lvl 2");
      UOItem targetCont2 = new UOItem(UIManager.TargetObject());
      Game.PrintMessage("Vyber Cil Lvl 3");
      UOItem targetCont3 = new UOItem(UIManager.TargetObject());
      Game.PrintMessage("Vyber Cil Lvl 4");
      UOItem targetCont4 = new UOItem(UIManager.TargetObject());
      Game.PrintMessage("Vyber Cil Lvl 5");
      UOItem targetCont5 = new UOItem(UIManager.TargetObject());

      EnsureContainer(sourceCont);

      List<Serial> doneList = new List<Serial>();

      foreach (UOItem map in sourceCont.Items.Where(i => i.Graphic == 0x14eb))
      {
        //Treasure Map
        Journal.Clear();
        EnsureItem(map);

        if (map.Container == World.Player.Backpack.Serial || map.Move(1, World.Player.Backpack))
        {
          Game.Wait(50);
          map.Use();
          Game.Wait(100);
          //if (Journal.WaitForText(true, 250, "You put the Treasure Map Level"))
          //{

          //}
          //else
          //{
          //  //TODO
          //}

          foreach (UOItem newMap in World.Player.Backpack.Items.Where(i => i.Graphic == 0x14eb && !doneList.Contains(i.Serial)))
          {
            EnsureItem(newMap);
            string name = newMap.Name + String.Empty;

            if (name.EndsWith("1"))
              newMap.Move(1, targetCont1);
            else if (name.EndsWith("2"))
              newMap.Move(1, targetCont2);
            else if (name.EndsWith("3"))
              newMap.Move(1, targetCont3);
            else if (name.EndsWith("4"))
              newMap.Move(1, targetCont4);
            else if (name.EndsWith("5"))
              newMap.Move(1, targetCont5);
            else 
              newMap.Move(1, sourceCont);

            doneList.Add(newMap.Serial);
            Game.Wait(100);
          }
        }
      }
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void mapydlelevelu()
    {
      UOItem containerPackage = World.Player.Backpack.Items.FindType(0x0E3F);
      if (!containerPackage.Exist)
      {
        Game.PrintMessage("Nemas u sebe container package!");
        return;
      }


      if (World.Player.Backpack.Items.Count(0x0E76) > 0)
      {
        Game.PrintMessage("Nesmis mit u sebe zadny pylik!");
        return;
      }

      Game.PrintMessage("Vyber zdroj");
      UOItem sourceCont = new UOItem(UIManager.TargetObject());
      Game.PrintMessage("Vyber Cil");
      UOItem targetCont = new UOItem(UIManager.TargetObject());

      Dictionary<string, List<UOItem>> contTypeDict = new Dictionary<string, List<UOItem>>();

      sourceCont.Use();
      Game.Wait();
      targetCont.Use();
      Game.Wait();

      Game.PrintMessage("Prochazim cil: " + targetCont.Items.CountItems());

      foreach (UOItem bag in targetCont.Items)
      {
        if (bag.Graphic == 0x0E76)
        {
          bag.Use();
          Game.Wait();

          if (bag.Items.CountItems() == 0)
          {
            bag.Move(1, sourceCont);
            Game.Wait();
          }
          else
          {
            foreach (UOItem i in bag.Items)
            {
              if (String.IsNullOrEmpty(i.Name))
              {
                i.Click();
                Game.Wait(200);
              }

              if (!String.IsNullOrEmpty(i.Name))
              {
                if (!contTypeDict.ContainsKey(i.Name))
                  contTypeDict.Add(i.Name, new List<UOItem>());

                contTypeDict[i.Name].Add(bag);
              }
            }
          }
        }
      }

      Game.PrintMessage("Jdu presouvat: " + targetCont.Items.CountItems());

      bool newbagsok = true;

      foreach (UOItem bag in sourceCont.Items)
      {
        if (bag.Graphic == 0x0E76)
        {
          //if (!bag.Opened)
          //{
          bag.Use();
          Game.Wait();
          //}

          while (bag.Items.CountItems() > 0)
          {
            foreach (UOItem item in bag.Items)
            {
              UOItem typeBag = new UOItem(Serial.Invalid);
              List<UOItem> typeBags = new List<UOItem>();

              if (String.IsNullOrEmpty(item.Name))
              {
                item.Click();
                Game.Wait(200);
              }

              string key = item.Name;

              if (contTypeDict.ContainsKey(key))
                typeBags = contTypeDict[key];
              else
              {
                UO.WaitTargetSelf();
                containerPackage.Use();
                Game.Wait();

                UOItem newBag = World.Player.Backpack.Items.FindType(0x0E76);
                if (newBag.Exist)
                {
                  if (contTypeDict.ContainsKey(key))
                  {
                    contTypeDict[key].Add(newBag);
                  }
                  else
                  {
                    contTypeDict.Add(key, new List<UOItem>());
                    contTypeDict[key].Add(newBag);
                  }

                  typeBags = contTypeDict[key];

                  newBag.Move(1, targetCont);
                  Game.Wait(1000);

                  Game.PrintMessage("Novy bag: " + key);
                }
                else
                {
                  if (newbagsok)
                  {
                    Game.PrintMessage("Nejsou nove bagy");
                    Game.Wait();
                  }
                  newbagsok = false;
                }
              }

              foreach (UOItem tb in typeBags)
              {
                if (!tb.Opened)
                {
                  tb.Use();
                  Game.Wait();
                }

                if (tb.Items.CountItems() < 255)
                {
                  typeBag = tb;
                  break;
                }

              }

              if (!typeBag.Exist)
              {
                UO.WaitTargetSelf();
                containerPackage.Use();
                Game.Wait();

                UOItem newBag = World.Player.Backpack.Items.FindType(0x0E76);
                if (newBag.Exist)
                {
                  if (contTypeDict.ContainsKey(key))
                  {
                    contTypeDict[key].Add(newBag);
                  }
                  else
                  {
                    contTypeDict.Add(key, new List<UOItem>());
                    contTypeDict[key].Add(newBag);
                  }
                  typeBag = newBag;

                  newBag.Move(1, targetCont);
                  Game.Wait(1000);

                  Game.PrintMessage("Novy bag: " + key);
                }
                else
                {
                  if (newbagsok)
                  {
                    Game.PrintMessage("Nejsou nove bagy");
                    Game.Wait();
                  }
                  newbagsok = false;
                }
              }

              if (!typeBag.Exist)
              {
                Game.PrintMessage("Neni bag: " + key);
                Game.Wait();
              }
              else
              {
                item.Move(1, typeBag);
                Game.Wait(555);
              }


            }
          }

        }
      }

      Game.Wait();

      Game.PrintMessage("Jdu setridit: " + targetCont.Items.CountItems());

      seradpytle(targetCont);

      Game.PrintMessage("Konec");
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void klamakydletypu()
    {
      klamakydletypu(false);
    }

    [Executable]
    public static void klamakydletypu(bool ibarvy)
    {
      UOItem containerPackage = World.Player.Backpack.Items.FindType(0x0E3F);
      if (!containerPackage.Exist)
      {
        Game.PrintMessage("Nemas u sebe container package!");
        return;
      }


      if (World.Player.Backpack.Items.Count(0x0E76) > 0)
      {
        Game.PrintMessage("Nesmis mit u sebe zadny pylik!");
        return;
      }

      Game.PrintMessage("Vyber zdroj");
      UOItem sourceCont = new UOItem(UIManager.TargetObject());
      Game.PrintMessage("Vyber Cil");
      UOItem targetCont = new UOItem(UIManager.TargetObject());

      Dictionary<string, List<UOItem>> contTypeDict = new Dictionary<string, List<UOItem>>();

      sourceCont.Use();
      Game.Wait();
      targetCont.Use();
      Game.Wait();

      Game.PrintMessage("Prochazim cil: " + targetCont.Items.CountItems());

      foreach (UOItem bag in targetCont.Items)
      {
        if (bag.Graphic == 0x0E76)
        {
          bag.Use();
          Game.Wait();

          if (bag.Items.CountItems() == 0)
          {
            bag.Move(1, sourceCont);
            Game.Wait();
          }
          else
          {
            UOItem first = bag.Items.First<UOItem>();
            string key = first.Graphic.ToString() + (ibarvy ? "|" + first.Color : "");

            if (!contTypeDict.ContainsKey(key))
              contTypeDict.Add(key, new List<UOItem>());

            contTypeDict[key].Add(bag);
          }
        }
      }

      Game.PrintMessage("Jdu presouvat: " + targetCont.Items.CountItems());

      bool newbagsok = true;

      foreach (UOItem bag in sourceCont.Items)
      {
        if (bag.Graphic == 0x0E76)
        {
          //if (!bag.Opened)
          //{
            bag.Use();
            Game.Wait();
          //}

          while (bag.Items.CountItems() > 0)
          {
            foreach (UOItem item in bag.Items)
            {
              UOItem typeBag = new UOItem(Serial.Invalid);
              List<UOItem> typeBags = new List<UOItem>();
              string key = item.Graphic.ToString() + (ibarvy ? "|" + item.Color : ""); ;


              if (contTypeDict.ContainsKey(key))
                typeBags = contTypeDict[key];
              else
              {
                UO.WaitTargetSelf();
                containerPackage.Use();
                Game.Wait();

                UOItem newBag = World.Player.Backpack.Items.FindType(0x0E76);
                if (newBag.Exist)
                {
                  if (contTypeDict.ContainsKey(key))
                  {
                    contTypeDict[key].Add(newBag);
                  }
                  else
                  {
                    contTypeDict.Add(key, new List<UOItem>());
                    contTypeDict[key].Add(newBag);
                  }

                  typeBags = contTypeDict[key];

                  newBag.Move(1, targetCont);
                  Game.Wait(1000);

                  Game.PrintMessage("Novy bag: " + key);
                }
                else
                {
                  if (newbagsok)
                  {
                    Game.PrintMessage("Nejsou nove bagy");
                    Game.Wait();
                  }
                  newbagsok = false;
                }
              }

              foreach (UOItem tb in typeBags)
              {
                if (!tb.Opened)
                {
                  tb.Use();
                  Game.Wait();
                }

                if (tb.Items.CountItems() < 255)
                {
                  typeBag = tb;
                  break;
                }

              }

              if (!typeBag.Exist)
              {
                UO.WaitTargetSelf();
                containerPackage.Use();
                Game.Wait();

                UOItem newBag = World.Player.Backpack.Items.FindType(0x0E76);
                if (newBag.Exist)
                {
                  if (contTypeDict.ContainsKey(key))
                  {
                    contTypeDict[key].Add(newBag);
                  }
                  else
                  {
                    contTypeDict.Add(key, new List<UOItem>());
                    contTypeDict[key].Add(newBag);
                  }
                  typeBag = newBag;

                  newBag.Move(1, targetCont);
                  Game.Wait(1000);

                  Game.PrintMessage("Novy bag: " + key);
                }
                else
                {
                  if (newbagsok)
                  {
                    Game.PrintMessage("Nejsou nove bagy");
                    Game.Wait();
                  }
                  newbagsok = false;
                }
              }

              if (!typeBag.Exist)
              {
                Game.PrintMessage("Neni bag: " + key);
                Game.Wait();
              }
              else
              {
                item.Move(1, typeBag);
                Game.Wait(555);
              }


            }
          }

        }
      }

      Game.Wait();

      Game.PrintMessage("Jdu setridit: " + targetCont.Items.CountItems());

      seradpytle(targetCont);

      Game.PrintMessage("Konec");
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void najdiapresun()
    {
      najdiapresun(true, false);
    }

    [Executable]
    public static void najdiapresun(bool podlebarvy, bool podlenazvu)
    {
      Game.PrintMessage("Vyber zdrojovou Bednu:");
      UOItem sourceCont = new UOItem(UIManager.TargetObject());

      Game.PrintMessage("Vyber cilovou Bednu:");
      UOItem targetCont = new UOItem(UIManager.TargetObject());


      Serial origCont = targetCont.Container;
      ushort origX = targetCont.X;
      ushort origY = targetCont.Y;

      Game.PrintMessage("Vyber item (typ):");
      UOItem type = new UOItem(UIManager.TargetObject());

      string typeNme = type.Name;
      if (String.IsNullOrEmpty(typeNme))
      {
        type.Click();
        Game.Wait();
        typeNme = type.Name;
      }

      Game.PrintMessage("Oteviram");
      List<UOItem> items = ItemHelper.OpenContainerRecursive(sourceCont);

      int count = 0;
      int amount = 0;
      int invalidName = 0;
      List<UOItem> itemsFound = new List<UOItem>();

      Game.PrintMessage("Prochazim " + items.Count);



      foreach (UOItem item in items)
      {
        if (ItemHelper.IsContainer(item))
        {
          EnsureContainer(item);


          if (item.Items.ToArray().Length > 0)
            continue;
        }

        //        Game.PrintMessage("Prochazim " + ItemHelper.IsContainer(item)  + " / " + item.Items.ToArray() + " / " + item.Items.CountItems());

        if (item.Graphic == type.Graphic && (!podlebarvy || item.Color == type.Color))
        {
          if (podlenazvu)
          {
            if (String.IsNullOrEmpty(item.Name))
            {
              item.Click();
              Game.Wait(250);
            }

            if (item.Name != typeNme)
            {
              invalidName++;
              continue;
            }
          }

          itemsFound.Add(item);
          count++;
          amount += item.Amount;
        }
      }
      Game.PrintMessage("Nalezeno: " + count + " / " + amount);

      Game.PrintMessage("Presouvam");
      foreach (UOItem item in itemsFound)
      {




        item.Move(60000, targetCont);
        Game.Wait();
      }


      Game.PrintMessage("Presun konec - " + invalidName);
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void naplnaboxyzbacpacku(Serial aboxbedna)
    {


      naplnaboxy(World.Player.Serial, aboxbedna);
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void naplnaboxy()
    {
      naplnaboxy(Serial.Invalid, Serial.Invalid);
    }
    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void naplnaboxy(Serial klamakbedna, Serial aboxbedna)
    {
      UOItem sourceCont = new UOItem(klamakbedna);

      if (sourceCont.Serial == Serial.Invalid)
      {
        Game.PrintMessage("Vyber bednu s klamaky:");
        sourceCont = new UOItem(UIManager.TargetObject());
      }

      UOItem targetCont = new UOItem(aboxbedna);

      if (targetCont.Serial == Serial.Invalid)
      {
        Game.PrintMessage("Vyber bednu s Animal boxy:");
        targetCont = new UOItem(UIManager.TargetObject());
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
          name = name.Replace("an ", "").Replace("a ", "").Trim().ToLower();

          if (!htBox.ContainsKey(name))
          {
            htBox.Add(name, new List<UOItem>());
          }

          htBox[name].Add(box);
        }
      }

      Game.PrintMessage("Nalezeno " + htBox.Keys.Count + " Animal boxu");

      items = ItemHelper.OpenContainerRecursive(sourceCont);

      Dictionary<string, List<UOItem>> dictAnimals = new Dictionary<string, List<UOItem>>();

      Game.PrintMessage("Nacitam Zvirata ...");
      foreach (UOItem animal in items)
      {
        if (String.IsNullOrEmpty(animal.Name))
        {
          animal.Click();
          Game.Wait(250);
        }

        string name = animal.Name + String.Empty;
        name = name.Replace("an ", "").Replace("a ", "").Trim().ToLower();


        if (htBox.ContainsKey(name))
        {
          if (!dictAnimals.ContainsKey(name))
          {
            dictAnimals.Add(name, new List<UOItem>());
          }

          dictAnimals[name].Add(animal);
        }
        else
        {
          UOItemType foundType = null;
          foreach (UOItemType shrinK in ItemLibrary.ShrinkKlamaci)
          {
            if (htBox.ContainsKey(shrinK.Name.ToLower()) && shrinK.Graphic == animal.Graphic && shrinK.Color == animal.Color)
            {
              foundType = shrinK;
              break;
            }
          }

          if (foundType != null)
          {
            if (!dictAnimals.ContainsKey(foundType.Name.ToLower()))
              dictAnimals.Add(foundType.Name.ToLower(), new List<UOItem>());
            dictAnimals[foundType.Name.ToLower()].Add(animal);
          }
        }
      }
      Game.PrintMessage("Nacteno: ");

      foreach (string key in dictAnimals.Keys)
      {
        Game.PrintMessage(key + ": " + dictAnimals[key].Count);
        Game.Wait();
      }

      Game.PrintMessage("Presouvam: ");

      foreach (string key in dictAnimals.Keys)
      {
        Game.PrintMessage(key + ": " + dictAnimals[key].Count);

        Dictionary<Serial, object[]> originalPositions = new Dictionary<Serial, object[]>();


        foreach (UOItem box in htBox[key])
        {
          if (!originalPositions.ContainsKey(box.Serial))
          {
            originalPositions.Add(box.Serial, new object[3]);
          }

          originalPositions[box.Serial][0] = box.X;
          originalPositions[box.Serial][1] = box.Y;
          originalPositions[box.Serial][2] = box.Container;

          box.Move(1, World.Player.Backpack);
          Game.Wait();
        }

        List<UOItem> animals = dictAnimals[key];
        int counter = animals.Count;

        foreach (UOItem animal in animals)
        {
          
          if (animal.Move(1, World.Player.Backpack))
          {
            Game.Wait();

            foreach (UOItem box in htBox[key])
            {
              Journal.Clear();
              UO.WaitTargetObject(animal);
              box.Use();

              if (Journal.WaitForText(true, 500, "Toto zviratko sem nepatri"))
              {
                Game.Wait(350);
                continue;
              }
              else
              {
                break;
              }
            }

            Game.PrintMessage("" + counter);
            counter--;
          }
          else
            Game.Wait();
        }

        foreach (UOItem box in htBox[key])
        {
          box.Move(1, (Serial)originalPositions[box.Serial][2], (ushort)originalPositions[box.Serial][0], (ushort)originalPositions[box.Serial][1]);
          Game.Wait();
        }

      }

      Game.PrintMessage("Konec");
    }

    //---------------------------------------------------------------------------------------------
    [Executable]
    public static void naplnsperky()
    {
      naplnsperky(Serial.Invalid, Serial.Invalid);
    }

    [Executable]
    public static void naplnsperky(Serial zdrojbedna, Serial sperkovnicebedna)
    {
      UOItem sourceCont = new UOItem(zdrojbedna);

      if (sourceCont.Serial == Serial.Invalid)
      {
        Game.PrintMessage("Vyber bednu se sperky:");
        sourceCont = new UOItem(UIManager.TargetObject());
      }

      UOItem targetCont = new UOItem(sperkovnicebedna);

      if (targetCont.Serial == Serial.Invalid)
      {
        Game.PrintMessage("Vyber bednu s Sperkovnicemi:");
        targetCont = new UOItem(UIManager.TargetObject());
      }

      Game.PrintMessage("Nacitam Sperkovnice ...");
      List<UOItem> items = ItemHelper.OpenContainerRecursive(targetCont);

      Dictionary<string, List<UOItem>> htBox = new Dictionary<string, List<UOItem>>();

      foreach (UOItem box in items)
      {
        if (box.Graphic == 0x09A8 && (box.Color == 0x054E || box.Color == 0x049F || box.Color == 0x0796 || box.Color == 0x06FB))// Barva: aktivni 0x054E  , neaktivni 0x049F  Nove: neaktivni 0x0796 , aktivni 0x06FB  , empty: 0x061B  
        {
          if (String.IsNullOrEmpty(box.Name))
          {
            box.Click();
            Game.Wait(250);
          }

          string name = box.Name + String.Empty;
          name = name.Replace("an ", "").Replace("a ", "").Trim().ToLower();

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

      items = ItemHelper.OpenContainerRecursive(sourceCont);

      Dictionary<string, List<UOItem>> dictAnimals = new Dictionary<string, List<UOItem>>();

      Game.PrintMessage("Nacitam sperky ...");
      foreach (UOItem item in items)
      {
        if (Jewelry.IsJewelry(item))
        {
          if (String.IsNullOrEmpty(item.Name))
          {
            item.Click();
            Game.Wait(250);
          }

          string name = item.Name + String.Empty;
          name = name.Replace("an ", "").Replace("a ", "").Trim().ToLower();

          foreach (string key in htBox.Keys)
          {
            if (Regex.IsMatch(name, "^" + key))
            {
              if (!dictAnimals.ContainsKey(key))
                dictAnimals.Add(key, new List<UOItem>());

              dictAnimals[key].Add(item);

              break;
            }
          }
        }
      }

      Game.PrintMessage("Nacteno: ");

      foreach (string key in dictAnimals.Keys)
      {
        Game.PrintMessage(key + ": " + dictAnimals[key].Count);
        Game.Wait();
      }

      Game.PrintMessage("Presouvam: ");

      foreach (string key in dictAnimals.Keys)
      {
        Game.PrintMessage(key + ": " + dictAnimals[key].Count);

        Dictionary<Serial, object[]> originalPositions = new Dictionary<Serial, object[]>();


        foreach (UOItem box in htBox[key])
        {
          if (!originalPositions.ContainsKey(box.Serial))
          {
            originalPositions.Add(box.Serial, new object[3]);
          }

          originalPositions[box.Serial][0] = box.X;
          originalPositions[box.Serial][1] = box.Y;
          originalPositions[box.Serial][2] = box.Container;

          box.Move(1, World.Player.Backpack);
          Game.Wait();
        }

        List<UOItem> animals = dictAnimals[key];
        int counter = animals.Count;

        foreach (UOItem animal in animals)
        {
          bool moveOK = true;
          if (animal.Container != World.Player.Backpack.Serial)
          {
            moveOK = animal.Move(1, World.Player.Backpack);
            Game.Wait();
          }

          if (moveOK)
          {
            foreach (UOItem box in htBox[key])
            {
              Journal.Clear();

              if (box.Color == 0x054E || box.Color == 0x06FB)
              {
                UO.WaitTargetObject(box);
                box.Use();
                Game.Wait();
              }

              UO.WaitTargetObject(animal);
              box.Use();
              //Sperkovnice je plna!

              if (Journal.WaitForText(true, 500, "Tohle se k ostatnim vecem ve sperkovnici nehodi", "Sperkovnice je plna!"))
              {
                Game.Wait(350);
                continue;
              }
              else
              {
                break;
              }
            }

            Game.PrintMessage("" + counter);
            counter--;
          }
          else
            Game.Wait();
        }

        foreach (UOItem box in htBox[key])
        {
          box.Move(1, (Serial)originalPositions[box.Serial][2], (ushort)originalPositions[box.Serial][0], (ushort)originalPositions[box.Serial][1]);
          Game.Wait();
        }

      }

      Game.PrintMessage("Konec");
    }
    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void najdiavloz()
    {
      najdiavloz(true, false);
    }

    [Executable]
    public static void najdiavloz(bool podlebarvy, bool podlenazvu)
    {
      Game.PrintMessage("Vyber zdrojovou Bednu:");
      UOItem sourceCont = new UOItem(UIManager.TargetObject());

      Game.PrintMessage("Vyber cilovy Box:");
      UOItem targetCont = new UOItem(UIManager.TargetObject());

      Game.PrintMessage("Vyber item (typ):");
      UOItem type = new UOItem(UIManager.TargetObject());

      string typeNme = type.Name;
      if (String.IsNullOrEmpty(typeNme))
      {
        type.Click();
        Game.Wait();
        typeNme = type.Name;
      }

      Serial origCont = targetCont.Container;
      ushort origX = targetCont.X;
      ushort origY = targetCont.Y;

      if (targetCont.Container != World.Player.Backpack)
      {
        targetCont.Move(1, World.Player.Backpack);
        Game.Wait();
      }

      Game.PrintMessage("Oteviram");
      List<UOItem> items = ItemHelper.OpenContainerRecursive(sourceCont);

      int count = 0;
      int amount = 0;
      int invalidName = 0;
      List<UOItem> itemsFound = new List<UOItem>();

      Game.PrintMessage("Prochazim " + items.Count);



      foreach (UOItem item in items)
      {
        if (item.Graphic == type.Graphic && (!podlebarvy || item.Color == type.Color))
        {
          if (podlenazvu)
          {
            if (String.IsNullOrEmpty(item.Name))
            {
              item.Click();
              Game.Wait(250);
            }

            if (item.Name != typeNme)
            {
              invalidName++;
              continue;
            }
          }


          itemsFound.Add(item);
          count++;
          amount += item.Amount;
        }
      }
      Game.PrintMessage("Nalezeno: " + count + " / " + amount + " [" + invalidName + "]");

      Game.PrintMessage("Presouvam");
      foreach (UOItem item in itemsFound)
      {
        if (item.Container != World.Player.Backpack)
        {
          item.Move(60000, World.Player.Backpack);
          Game.Wait();
        }

        UO.WaitTargetObject(item);
        targetCont.Use();
        Game.Wait();
      }


      targetCont.Move(1, origCont, origX, origY);
      Game.Wait();

      Game.PrintMessage("Presun konec");
    }

    //---------------------------------------------------------------------------------------------\

    public static T EnsureObjectName<T>(T obj) where T : UOObject
    {
      if (String.IsNullOrEmpty(obj.Name))
      {
        obj.Click();
        Game.Wait(125);
      }
      return obj;
    }



    //---------------------------------------------------------------------------------------------\

    public static UOItemExtInfo GetItemExtInfo(UOItem item)
    {
      return GetItemExtInfo(item, null);
    }

    public static UOItemExtInfo GetItemExtInfo(UOItem item, string name)
    {
      string namePatern = String.IsNullOrEmpty(name) ? "[^)(]*" : name ;

      item.Click();
      item.WaitForName();
      Game.Wait(100);

      if (Game.Debug)
      {
        Game.PrintMessage("GetItemExtInfo: " + Game.LastEntry.Text);
      }

      string realName = Game.LastEntry.Text; 
      if (!realName.ToLower().StartsWith(item.Name.ToLower()))
      {
        for (int i = Game.EntryHistory.Count -1; i >= Game.EntryHistory.Count - 5 && i >= 0; i--)
        {
          if (Game.EntryHistory[i].Text.ToLower().StartsWith(item.Name.ToLower()))
          {
            realName = Game.EntryHistory[i].Text;
            break;
          }
        }
      }

      UOItemExtInfo info = new UOItemExtInfo();
      info.Item = item;

      string trim = realName + String.Empty;
      trim = trim.Replace("an ", "").Replace("a ", "").Trim();

      if (Game.Debug)
        Game.PrintMessage("GetItemExtInfo trim: " + trim + " / " + item.Name);

      if (!String.IsNullOrEmpty(trim))
      {
        //  Match m = Regex.Match(trim, "^(?<name>" + namePatern + ")[ ]?[(]?(?<charges>(?<chargescount>[0-9]*)[ ]?(?<chargesname>[A-Za-z]*))[)]?$", RegexOptions.IgnoreCase);
        string rgxPattern = "^(?<name>" + namePatern + ")[ ]?[(]?(?<charges>(?<chargescount>[0-9]*)[ ]?(?<chargesname>[A-Za-z]*))[)]?[ ]?(?<chargesname2>[A-Za-z]*)?.*$";
        Match m = Regex.Match(trim, rgxPattern, RegexOptions.IgnoreCase);
        if (m.Success)
        {
          info.Success = m.Success;
          info.Charges = Utils.ToNullInt(m.Groups["chargescount"]);
          info.ChargesName = m.Groups["chargesname"].Success && !String.IsNullOrEmpty(m.Groups["chargesname"].Value) ? m.Groups["chargesname"].Value : (m.Groups["chargesname2"].Success ? m.Groups["chargesname2"].Value : "");
          info.Name = trim;
        }
        else if (Game.Debug)
          Game.PrintMessage("GetItemExtInfo !Success: " + trim + " / " + rgxPattern);
      }

      return info;
    }
    //Serial: 0x40069137  Name: "Arrow quiver (1400) sipu"  Position: 135.78.0  Flags: 0x0000  Color: 0x0747  Graphic: 0x1EA0  Amount: 1  Layer: None  Container: 0x4029398B
    //    08:37 You see: Nadoba s Shrink(3764 lektvaru)
    //08:37 You see: 486 empty bottles
    //08:37 You see: Apperentice's poisoning kit (300 nabiti)
    //08:37 You see: wooden box crafted by Ungo(19 items)
    //08:37 You see: wooden box crafted by Ungo(36 items)
    //08:37 You see: a wooden box(20 items)
    //08:37 You see: a backpack(96 items)
    //08:37 You see: a Drum of Protection(5 charges)
    //08:37 You see: Great Diamant Bracelet
    //08:38 You see: Great Diamant Bracelet
    //08:38 You see: Reflex Ring
    //08:38 You see: a Lute of Reactive Armor(5 charges)
    //08:38 You see: a Lute of Reactive Armor(4 charges)


    //---------------------------------------------------------------------------------------------

    public static List<Serial> ContainerState(UOItem container)
    {
      List<Serial> list = new List<Serial>();

      foreach (UOItem item in container.Items)
        list.Add(item.Serial);

      return list;
    }

    public static List<Serial> ContainerStateDiff(List<Serial> a, List<Serial> b)
    {
      return b.Where(o => !a.Contains(o)).ToList();
    }

    //---------------------------------------------------------------------------------------------
    [Executable]
    public static void RecycleWeapon(params string[] qualities)
    {
      if (qualities.Length == 0)
      {
        Game.PrintMessage("Musite zadat aspon jednu kvalitu, Ruin,Might,Force,Power,Vanquishing");
        return;
      }

      UOItem recyc = World.Ground.FindType(0x0E32, 0x0B82);
      if (!recyc.Exist || recyc.Distance > 2)
      {
        Game.PrintMessage("Musis byt blizko recyklatoru");
        return;
      }

      Game.PrintMessage("Vyber zdrojovou kontainer:");
      UOItem sourceCont = new UOItem(UIManager.TargetObject());
      Game.Wait();

      OpenContainerRecursive(sourceCont);
      Game.Wait();

      List<UOItem> toRecycle = new List<UOItem>();
      foreach (UOItem item in sourceCont.AllItems)
      {
        UOItemExtInfo info = GetItemExtInfo(item, null);

        string name = (info.Name + String.Empty).ToLower();
        bool found = false;
        foreach (string q in qualities)
        {
          if (name.Contains(q.ToLower()))
          {
            found = true;
            //Game.PrintMessage("Nalezeno: " + name);
            toRecycle.Add(item);
          }
        }

        if (found)
          continue;

      }

      Game.PrintMessage("Nalezeno: " + toRecycle.Count);

      foreach (UOItem item in toRecycle)
      {
        if (item.Container != World.Player.Backpack)
        {
          item.Move(1, World.Player.Backpack);
          Game.Wait();
        }


        UO.WaitTargetObject(item);
        recyc.Use();
        Game.Wait();
      }
    }

    //---------------------------------------------------------------------------------------------

//19:32 You see: a Leather Tunic of Invulnerability
//19:32 You see: wooden box crafted by Pedisequus(23 items)
//19:32 You see: a +16 Bardiche of Vanquishing of

    //  ,exec SortLootTo "Vanquishing" "Invulnerability" "Paladin" "Cleric" "Warrior" "Grandmaster" "Ranger" "Arch" "Mytheril" "Wyrm" "Perfect" "Phoenix" "Power" "+9" "+25" "Blood" "Master"

    [Executable]
    public static void SortLootTo()
    {
      SortLootTo(null);
    }

    [Executable]
    public static void SortLootTo(params string[] rareItems)
    {
      Game.PrintMessage("Vyberte container pro Regy:");
      ExtContainer reagentsCont = new ExtContainer(Targeting.GetTarget(null));

      Game.PrintMessage("Vyberte container pro Zbrane:");
      ExtContainer weaponCont = new ExtContainer(Targeting.GetTarget(null));

      Game.PrintMessage("Vyberte container pro Zbroje:");
      ExtContainer armorCont = new ExtContainer(Targeting.GetTarget(null));

      Game.PrintMessage("Vyberte container pro Coby:");
      ExtContainer cobCont = new ExtContainer(Targeting.GetTarget(null));

      Game.PrintMessage("Vyberte container pro Stone:");
      ExtContainer stoneCont = new ExtContainer(Targeting.GetTarget(null));

      Game.PrintMessage("Vyberte container pro Knizky:");
      ExtContainer bookCont = new ExtContainer(Targeting.GetTarget(null));

      Game.PrintMessage("Vyberte container pro Mystiky:");
      ExtContainer mysticalCont = new ExtContainer(Targeting.GetTarget(null));

      Game.PrintMessage("Vyberte container pro Stity:");
      ExtContainer shieldCont = new ExtContainer(Targeting.GetTarget(null));

      Game.PrintMessage("Vyberte container pro Elixiry:");
      ExtContainer elixiryCont = new ExtContainer(Targeting.GetTarget(null));

      Game.PrintMessage("Vyberte container pro Vzacne veci:");
      ExtContainer rareCont = new ExtContainer(Targeting.GetTarget(null));

      Game.PrintMessage("Vyberte container pro Ostatni:");
      ExtContainer otherCont = new ExtContainer(Targeting.GetTarget(null));

      Dictionary<Serial, ExtContainer> stackContainers = new Dictionary<Serial, ExtContainer>();

      Game.PrintMessage("Vyberte container se Stack containery:");
      UOItem stackMain = Targeting.GetTarget(null);

      Game.PrintMessage("Vyberte container s Aboxy:");
      ExtContainer aboxCont = new ExtContainer(Targeting.GetTarget(null));

      Game.PrintMessage("Vyberte container se Sperkovnicemi:");
      ExtContainer sperkyCont = new ExtContainer(Targeting.GetTarget(null));


      Game.PrintMessage("Vyberte zdrojovy kontainer:");
      Serial zdrojCont = Targeting.GetTarget(null);
      List<UOItem> items = OpenContainerRecursive(zdrojCont);

      if (stackMain.Exist)
      {
        EnsureContainer(stackMain);

        foreach (UOItem cont in stackMain.Items)
        {
          if (IsStackContainer(cont) && !stackContainers.ContainsKey(cont))
          {
            stackContainers.Add(cont, new ExtContainer(cont));
            stackContainers[cont].Container.Click();
            Game.Wait(150);
          }
        }
      }

      Game.PrintMessage("Celkem Itemu: " + items.Count);
      Game.PrintMessage("Celkem Stack Contu: " + stackContainers.Count);

      naplnaboxy(zdrojCont, aboxCont.Container);
      naplnsperky(zdrojCont, sperkyCont.Container);

      foreach (UOItem item in items)
      {
        UOItemExtInfo extInfo = ItemHelper.GetItemExtInfo(item);

        if (IsContainer(item) || item.Items.Count() > 0)
          continue;

        bool found = false;

        foreach (string rarename in rareItems)
        {
          if ((extInfo.Name + String.Empty).ToLower().Contains(rarename.ToLower()))
          {
            rareCont.PutItem(item);
            found = true;
            break;
          }
        }

        foreach (Serial s in stackContainers.Keys)
        {
          if (IsStackContainerItem(stackContainers[s].Container, item))
          {
            stackContainers[s].PutItem(item);
            found = true;
            break;
          }
        }

        if (found)
          continue;
        
        if (ReagentCollection.Reagents.Contains(item))
          reagentsCont.PutItem(item);
        else if (item.Graphic == 0x1420 && item.Color == 0x0152)
          cobCont.PutItem(item);
        else if (item.Graphic == 0x0FCC)
          stoneCont.PutItem(item);
        else if (item.Graphic == 0x1848)
          elixiryCont.PutItem(item);
        else if (item.Graphic == 0x0FF4)//knizky
          bookCont.PutItem(item);
        else if (
          ItemLibrary.WeaponsArch.Contains(item.Graphic) ||
          ItemLibrary.WeaponsFenc.Contains(item.Graphic) ||
          ItemLibrary.WeaponsMace.Contains(item.Graphic) ||
          ItemLibrary.WeaponsSword.Contains(item.Graphic)
          )
          weaponCont.PutItem(item);
        else if (
          ItemLibrary.ChainmailArmor.Contains(item.Graphic) ||
          ItemLibrary.RingmailArmor.Contains(item.Graphic) ||
          ItemLibrary.LeatherArmor.Contains(item.Graphic) ||
          ItemLibrary.PlatemailArmor.Contains(item.Graphic) ||
          ItemLibrary.BoneArmor.Contains(item.Graphic) ||
          ItemLibrary.StuddedArmor.Contains(item.Graphic) ||
          ItemLibrary.Robes.Contains(item.Graphic)
          )
          armorCont.PutItem(item);
        else if (ItemLibrary.MysticComponents.Contains(item.Graphic, item.Color))
          mysticalCont.PutItem(item);
        else if (ItemLibrary.Shields.Contains(item.Graphic))
          shieldCont.PutItem(item);
        else 
          otherCont.PutItem(item);
      }

      foreach (Serial s in stackContainers.Keys)
      {
        stackContainers[s].EnsureOriginalPosition();

        if (stackContainers[s].PutItemCount > 0)
          Game.PrintMessage(stackContainers[s].Container.Name + ": " + stackContainers[s].PutItemCount);
      }

      if (reagentsCont.PutItemCount > 0)
        Game.PrintMessage("Regy: " + reagentsCont.PutItemCount);
      if (armorCont.PutItemCount > 0)
        Game.PrintMessage("Armor: " + armorCont.PutItemCount);
      if (weaponCont.PutItemCount > 0)
        Game.PrintMessage("Weapon: " + weaponCont.PutItemCount);
      if (shieldCont.PutItemCount > 0)
        Game.PrintMessage("Stity: " + shieldCont.PutItemCount);
      if (elixiryCont.PutItemCount > 0)
        Game.PrintMessage("Elixiry: " + elixiryCont.PutItemCount);
      if (cobCont.PutItemCount > 0)
        Game.PrintMessage("Cob: " + cobCont.PutItemCount);
      if (stoneCont.PutItemCount > 0)
        Game.PrintMessage("Stone: " + stoneCont.PutItemCount);
      if (mysticalCont.PutItemCount > 0)
        Game.PrintMessage("Mystic: " + mysticalCont.PutItemCount);
      if (bookCont.PutItemCount > 0)
        Game.PrintMessage("Knizky: " + bookCont.PutItemCount);
      if (rareCont.PutItemCount > 0)
        Game.PrintMessage("Rare: " + rareCont.PutItemCount);
      if (otherCont.PutItemCount > 0)
        Game.PrintMessage("Other: " + otherCont.PutItemCount);
    }


    //---------------------------------------------------------------------------------------------

    public static bool IsStackContainer(UOItem item)
    {
      return
        item.Graphic == 0x2834 && item.Color == 0x0498 ||//soul 
        item.Graphic == 0x2834 && item.Color == 0x0B16 ||//soul ench
        item.Graphic == 0x09B0 && item.Color == 0x0B7B ||//fairy dust
        item.Graphic == 0x09A8 && item.Color == 0x049D;//spirity box
    }

    //---------------------------------------------------------------------------------------------

    public static bool IsStackContainerItem(UOItem cont, UOItem item)
    {
      if (cont.Graphic == 0x2834 && cont.Color == 0x0498)
        return item.Graphic == 0x0FC4 && item.Color == 0x0498;//soul
      else if (cont.Graphic == 0x2834 && cont.Color == 0x0B16)
        return item.Graphic == 0x0FC4 && item.Color == 0x0B16;//soul ench
      else if (cont.Graphic == 0x09B0 && cont.Color == 0x0B7B)
        return item.Graphic == 0x103D && item.Color == 0x0B52;//fairy
      else if (cont.Graphic == 0x09A8 && cont.Color == 0x049D)
        return item.Graphic == 0x0E26 && item.Color == 0x049D;//spirit

      return false;

    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void SortItemByType(int w, int h, int slowW, int slotH)
    {
      Game.PrintMessage("Bagl z >");

      UOItem containerFrom = new UOItem(UIManager.TargetObject());
      Game.PrintMessage("Bagl do >");

      ContainerFrame frameTo = new ContainerFrame(UIManager.TargetObject(), w, h, slowW, slowW);
      List<ContainerSlot> slots = new List<ContainerSlot>();

      Game.PrintMessage("frameTo Rows: " + frameTo.Rows.Count);

      foreach (List<ContainerSlot> row in frameTo.Rows)
      {
        foreach (ContainerSlot slot in row)
        {
          if (slot.Empty)
            slots.Add(slot);
        }
      }

      Game.PrintMessage("frameTo slots: " + slots.Count);
      var sortedItems = containerFrom.Items.OrderBy(i => i.Graphic + "|" + i.Color).ToList();
      Game.PrintMessage("sortedItems : " + sortedItems.Count);

      for (int i = 0; i < Math.Min(sortedItems.Count, slots.Count); i++)
      {
        Game.PrintMessage(slots[i].X + " / " + slots[i].Y);
        slots[i].Push(sortedItems[i]);
        Game.Wait();
      }
      Game.PrintMessage("Konec");

    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void EquipNBBracNeck()
    {
      UOItem item = World.Player.FindType(0x1086, 0x0480);//war / pal brac

      if (!item.Exist)
        item = World.Player.FindType(0x1088, 0x0B9D);//shaman neck

      if (item.Exist)
      {
        if (item.Layer == Layer.None)
        {
          World.Player.PrintMessage("[Nasazuji..]");
          item.Use();
        }
        else
          World.Player.PrintMessage("[Jiz nasazen..]");
      }
      else
        World.Player.PrintMessage("[Neni bracneck..]", MessageType.Warning);
    }


    //Serial: 0x400E3039  Name: "Caleb's warrior's bracelet of"  Position: 142.111.0  Flags: 0x0000  Color: 0x0480  Graphic: 0x1086  Amount: 1  Layer: None  Container: 0x403174BC
    //Serial: 0x4004EA34  Name: "Nahrdelnik premeny"  Position: 84.81.0  Flags: 0x0000  Color: 0x0B9D  Graphic: 0x1088  Amount: 1  Layer: None Container: 0x40128393

    //---------------------------------------------------------------------------------------------

  }
  //  Serial: 0x402F2C62  Name: "Spirit of the Rain"  Position: 34.67.0  Flags: 0x0000  Color: 0x049D  Graphic: 0x0E26  Amount: 1  Layer: None Container: 0x4031522D

  //Serial: 0x40213E78  Name: "Fairy Dust"  Position: 130.102.0  Flags: 0x0020  Color: 0x0B52  Graphic: 0x103D  Amount: 1  Layer: None Container: 0x40384AF7

  //Serial: 0x401FFCB7  Position: 94.122.0  Flags: 0x0020  Color: 0x0B16  Graphic: 0x0FC4  Amount: 1  Layer: None Container: 0x40384AF7//soul ench 

  //Serial: 0x4038A746  Position: 99.88.0  Flags: 0x0020  Color: 0x0498  Graphic: 0x0FC4  Amount: 1  Layer: None Container: 0x40384AF7//soul


  //---------------------------------------------------------------------------------------------

  

  //---------------------------------------------------------------------------------------------




  public class UOItemExtInfo
  {
    public bool Success = false;
    public UOItem Item = new UOItem(Serial.Invalid);
    public int? Charges = null;
    public string ChargesName = String.Empty;
    public string Name = String.Empty;
  }

  public class ExtContainer
  {
    public ExtContainer(Serial serial)
    {
      Container = new UOItem(serial);
      ParentContainer = this.Container.Container;
      OriginalX = this.Container.X;
      OriginalY = this.Container.Y;
    }

    public UOItem Container;
    public Serial ParentContainer;
    public ushort OriginalX;
    public ushort OriginalY;
    public int PutItemCount = 0;

    public bool IsStackContiner
    {
      get
      {
        return ItemHelper.IsStackContainer(this.Container);
      }
    }


    public void EnsureForPut()
    {
      if (this.Container.ExistCust() && this.IsStackContiner && this.Container.Container != World.Player.Backpack)
      {
        this.Container.Move(1, World.Player.Backpack);
        Game.Wait(400);
      }
    }

    public void EnsureOriginalPosition()
    {
      if (this.ParentContainer.IsValidCust() &&  this.Container.Container != this.ParentContainer)
      {
        this.Container.Move(1, this.ParentContainer, this.OriginalX, this.OriginalY);
        Game.Wait(400);
      }
    }

    public bool PutItem(UOItem item)
    {
      bool success = false;
      this.EnsureForPut();
      if (this.IsStackContiner)
      {
        if (item.Container != World.Player.Backpack)
        {
          item.Move(item.Amount, World.Player.Backpack);
          Game.Wait(400);
        }

        UO.WaitTargetObject(item);
        this.Container.Use();
        Game.Wait(400);

        success = true;
      }
      else
      {
        success = item.Move(item.Amount, this.Container);
        Game.Wait(400);
      }

      if (success)
        PutItemCount += item.Amount;

      return success;
    }

    public void GetItem()
    {
      this.EnsureForPut();
      if (this.IsStackContiner)
      {
        UO.WaitTargetSelf();
        this.Container.Use();
        Game.Wait(400);
      }
    }
  }

  /*
  m = Regex.Match(name, "^(?<name>" + a + ")[ ]?[(]?(?<charges>(?<chargescount>[0-9]*)[ A-Za-z]*)[)]?$", RegexOptions.IgnoreCase);
                            //^(?<name>[^)(]*)[ ]?[(]?(?<charges>(?<chargescount>[0-9]*)[ A-Za-z]*)[)]?$*/

  //^(?<name>[^)(]*)[ ]?[(]?(?<charges>(?<chargescount>[0-9]*)[ ]?(?<chargesname>[A-Za-z]*))[)]?$
}




