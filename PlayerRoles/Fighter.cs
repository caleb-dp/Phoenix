using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;
using Caleb.Library;
using System.Threading;
using CalExtension.Skills;
using CalExtension.UOExtensions;
using CalExtension.UI.Status;
using Caleb.Library.CAL.Business;
using System.Collections;
using CalExtension.Abilities;

namespace CalExtension.PlayerRoles
{
  public class Fighter : PlayerRole
  {
    public object SyncRoot;
    private static Fighter current;
    public static Fighter Current
    {
      get
      {
        if (current == null)
          current = Game.CurrentGame.CurrentPlayer.GetPlayerRoleInstance<Fighter>();
        return current;
      }
    }

    public Fighter()
    {
      this.OnInit();

    }

    protected void OnInit()
    {
      Slots = new Dictionary<string, Serial>();
      SyncRoot = new object();
    }

    //---------------------------------------------------------------------------------------------

    protected Dictionary<string, Serial> Slots;

    //---------------------------------------------------------------------------------------------

    protected void SetSlotItem(string key, Serial s)
    {
      lock (SyncRoot)
      {
        if (this.Slots.ContainsKey(key))
          this.Slots[key] = s;
        else
          this.Slots.Add(key, s);
      }
    }

    //---------------------------------------------------------------------------------------------

    public UOItem GetSlotItem(string key)
    {
      UOItem item = new UOItem(Serial.Invalid);
      lock (SyncRoot)
      {
        if (Slots.ContainsKey(key))
          item = new UOItem(Slots[key]);
      }
      return item;
    }

    //---------------------------------------------------------------------------------------------

    //exec usetypecust 0x13B5  0x044D  "none" "" "[Scim...]" 

    //exec usetypecust 0x0000  0x08A1  "none" "" "[Verite...]" 0x0000 0x08A1

    [Executable]
    public static void EquipSlotItem(string key)
    {
      EquipSlotItem(key, 0x0000);
    }

    [Executable]
    public static void EquipSlotItem(string key, Graphic g)
    {
      EquipSlotItem(key, g, 0x0000);
    }

    [Executable]
    public static void EquipSlotItem(string key, Graphic g, UOColor c)
    {
      EquipSlotItem(key, g, c, true);
    }

    [Executable]
    public static void EquipSlotItem(string key, Graphic g, UOColor c, bool ensureShield)
    {
      UOItem slotItem = Current.GetSlotItem(key);
      //Game.PrintMessage(String.Format("EquipSlotItem: {0}, {1}, {2}, {3}", key, g, c, ensureShield));

      if (!slotItem.Exist)
      {
        if (g == 0x0000 && c > 0)
        {
          foreach (UOItem item in World.Player.Backpack.Items)
          {
            if (item.Color == c)
            {
              slotItem = item;
              break;
            }
          }

          foreach (UOItem item in World.Player.Backpack.AllItems)
          {
            if (item.Color == c)
            {
              slotItem = item;
              break;
            }
          }

         
        }
        else if (g > 0)
        {
          foreach (UOItem item in World.Player.Backpack.Items)
          {
            if (item.Graphic == g && item.Color == c)
            {
              slotItem = item;
              break;
            }
          }

          foreach (UOItem item in World.Player.Backpack.AllItems)
          {
            if (item.Graphic == g && item.Color == c)
            {
              slotItem = item;
              break;
            }
          }

          slotItem = World.Player.FindType(g, c);

          if (!slotItem.Exist && c == 0x0000)
          {
            foreach (UOItem item in World.Player.Backpack.Items)
            {
              if (item.Graphic == g)
              {
                slotItem = item;
                break;
              }
            }

            foreach (UOItem item in World.Player.Backpack.AllItems)
            {
              if (item.Graphic == g)
              {
                slotItem = item;
                break;
              }
            }
          }
        }
      }

      if (!slotItem.Exist)
      {
        if (World.Player.Layers[Layer.RightHand].Exist && !Current.GetSlotItem(key).Exist)
        {
          World.Player.PrintMessage("[ RHand = " + key +" ]");
          slotItem = World.Player.Layers[Layer.RightHand];
        }
        else
        {
          World.Player.PrintMessage("[ Vyber " + key + " ]");
          slotItem = new UOItem(UIManager.TargetObject());
        }
      }
      else
      {
        if (CheckCurrentWeapon(true))
        {
          if (!(slotItem.Layer == Layer.LeftHand || slotItem.Layer == Layer.RightHand))
          {
            Current.SetSlotItem("__LastWeapon", slotItem.Serial);
            slotItem.Use();
            Game.Wait(100);
            if (UIManager.CurrentState != UIManager.State.Ready)
              Targeting.ResetTarget();

            World.Player.PrintMessage("[ " + key +" ]");
          }

          if (ensureShield && !World.Player.Layers[Layer.LeftHand].Exist)
          {
            UOItem shield = Current.GetSlotItem("__LastShield");
            if (!shield.Exist)
            {
              Current.SwitchShield();
            }
            else
              shield.Use();
          }
        }
      }

      Current.SetSlotItem(key, slotItem.Serial);
    }

    //---------------------------------------------------------------------------------------------

    public UOItemTypeCollection AllWeapons
    {
      get
      {
        UOItemTypeCollection items = new UOItemTypeCollection();
        items.AddRange(ItemLibrary.WeaponsFenc);
        items.AddRange(ItemLibrary.WeaponsSword);
        items.AddRange(ItemLibrary.WeaponsMace);
        items.AddRange(ItemLibrary.WeaponsArch);
        return items;
      }
    }

    //---------------------------------------------------------------------------------------------

    protected UOItemTypeCollection GetWeaponTypes()
    {
      UOItemTypeCollection list = new UOItemTypeCollection();
      list.AddRange(ItemLibrary.WeaponsFenc);
      list.AddRange(ItemLibrary.WeaponsSword);
      list.AddRange(ItemLibrary.WeaponsMace);
      list.AddRange(ItemLibrary.WeaponsArch);
      return list;
    }

    //Serial: 0x4035D7D5  Name: "Chachumbabu's warrior's brace"  Position: 79.137.0  Flags: 0x0000  Color: 0x0480  Graphic: 0x1086  Amount: 1  Layer: None  Container: 0x40322584

     // VYRESIT Naramek + schozeni ?

    //---------------------------------------------------------------------------------------------

    public List<UOItem> GetCurrentWeapons()//params string[] filter)
    {
      List<UOItem> items = new List<UOItem>();
      List<UOItem> searchItems = new List<UOItem>();
      UOItemTypeCollection toEquipColl = this.GetWeaponTypes();

      searchItems.Add(World.Player.Layers[Layer.LeftHand]);
      searchItems.Add(World.Player.Layers[Layer.RightHand]);

      foreach (UOItem item in World.Player.Backpack.Items)
      {
        if (item != null && item.Exist && item.Graphic != null)
          searchItems.Add(item);
      }

      foreach (UOItem item in World.Ground)
      {
        if (item != null && item.Exist && item.Graphic != null && item.Distance < 3)
          searchItems.Add(item);
      }

      foreach (UOItem item in searchItems)
      {
        if (item.IsItemType(ItemLibrary.DeathsServantBardiche))//bad idea :]
          continue;


        if (item != null && item.Exist && item.Graphic != null && toEquipColl.FindItem(item.Graphic) != null)
          items.Add(item);
      }

      items.Sort(delegate (UOItem x, UOItem y) {
        if (x.Serial < y.Serial) return -1;
        else if (x.Serial == y.Serial) return 0;
        else return 1;
      });

      return items;
    }

    //---------------------------------------------------------------------------------------------

    public List<UOItem> GetCurrentShields()
    {
      List<UOItem> items = new List<UOItem>();
      List<UOItem> searchItems = new List<UOItem>();
      UOItemTypeCollection toEquipColl = ItemLibrary.Shields;

      searchItems.Add(World.Player.Layers[Layer.LeftHand]);

      foreach (UOItem item in World.Player.Backpack.Items)
      {
        if (item != null && item.Exist && item.Graphic != null)
          searchItems.Add(item);
      }

      foreach (UOItem item in World.Ground)
      {
        if (item != null && item.Exist && item.Graphic != null && item.Distance < 3)
          searchItems.Add(item);
      }

      foreach (UOItem item in searchItems)
      {
        if (item.IsItemType(ItemLibrary.GoldenscaleShield))//supina :]
          continue;

        if (item != null && item.Exist && item.Graphic != null && toEquipColl.FindItem(item.Graphic) != null)
          items.Add(item);
      }

      items.Sort(delegate (UOItem x, UOItem y) {
        if (x.Serial < y.Serial) return -1;
        else if (x.Serial == y.Serial) return 0;
        else return 1;
      });

      return items;
    }

    //  Serial: 0x401B7C69  Name: "Inscribed Staff of Frenzy"  Position: 0.0.0  Flags: 0x0000  Color: 0x0B16  Graphic: 0x26C9  Amount: 0  Layer: LeftHand Container: 0x00002965

    //---------------------------------------------------------------------------------------------

    public static bool CheckCurrentWeapon(bool oneHand)
    {
      if (World.Player.CurrentWeapon().Exist)
      {
        if (oneHand && World.Player.CurrentWeapon().IsItemType(ItemLibrary.MytherilScimitar))
        {
          World.Player.PrintMessage("[MythScim!]", MessageType.Error);
          return false;
        }
        else if (World.Player.CurrentWeapon().IsItemType(ItemLibrary.OakwoodQuarterStaff))
        {
          World.Player.PrintMessage("[QuarterStaff!]", MessageType.Error);
          return false;
        }
      }

      return true;
    }


    //---------------------------------------------------------------------------------------------
    protected UOItem currentWeapon;
    public void SwitchWeapon(params string[] filter)
    {
      if (!CheckCurrentWeapon(true))
        return;

      List<UOItem> list = GetCurrentWeapons();

      if (currentWeapon != null && currentWeapon.Serial != World.Player.Layers[Layer.LeftHand].Serial && currentWeapon.Serial != World.Player.Layers[Layer.RightHand].Serial)
        currentWeapon = null;

      UOItem origWeapon = currentWeapon;

      if (currentWeapon == null)
      {
        if (list.Count > 0)
          currentWeapon = list[0];
      }
      else
      {
        int index = 0;

        for (int i = 0; i < list.Count; i++)
        {
          if (list[i].Serial == currentWeapon.Serial)
          {
            index = i + 1;
            break;
          }
        }

        if (index >= list.Count)
          index = 0;

        currentWeapon = list[index];
      }

      if (currentWeapon != null)
      {
        if (origWeapon == null || origWeapon.Serial != currentWeapon.Serial)
        {
          SetSlotItem("__LastWeapon", currentWeapon.Serial);
          currentWeapon.Use();
          Game.Wait(250);

          if (!World.Player.Layers[Layer.LeftHand].Exist)
          {
            UOItem shield = Current.GetSlotItem("__LastShield");
            if (!shield.Exist)
            {
              Current.SwitchShield();
            }
            else
              shield.Use();
          }
        }
      }
    }

    //---------------------------------------------------------------------------------------------
    protected UOItem currentShield = null;
    public void SwitchShield()
    {
      if (!CheckCurrentWeapon(false))
        return;

      List<UOItem> list = GetCurrentShields();

      if (currentShield != null && currentShield.Serial != World.Player.Layers[Layer.LeftHand].Serial && currentShield.Serial != World.Player.Layers[Layer.RightHand].Serial)
        currentShield = null;

      UOItem origShield = currentShield;

      if (currentShield == null)
      {
        if (list.Count > 0)
          currentShield = list[0];
      }
      else
      {
        int index = 0;

        for (int i = 0; i < list.Count; i++)
        {
          if (list[i].Serial == currentShield.Serial)
          {
            index = i + 1;
            break;
          }
        }

        if (index >= list.Count)
          index = 0;

        currentShield = list[index];
      }

      if (currentShield != null)
      {
        if (origShield == null || origShield.Serial != currentShield.Serial)
        {
          SetSlotItem("__LastShield", currentShield.Serial);
          currentShield.Use();
          Game.Wait();
        }
      }
    }

    //---------------------------------------------------------------------------------------------

    #region exec

    [Executable("SwitchShield")]
    public static void ExecSwitchShield()
    {
      Game.CurrentGame.CurrentPlayer.GetPlayerRoleInstance<Fighter>().SwitchShield();
    }

    [Executable("SwitchWeapon")]
    public static void ExecSwitchWeapon()
    {
      Game.CurrentGame.CurrentPlayer.GetPlayerRoleInstance<Fighter>().SwitchWeapon();
    }

    [Executable("SwitchWeapon")]
    public static void ExecSwitchWeapon(params string[] args)
    {
      Game.CurrentGame.CurrentPlayer.GetPlayerRoleInstance<Fighter>().SwitchWeapon(args);
    }


    //---------------------------------------------------------------------------------------------

    [Executable]
    [BlockMultipleExecutions]
    public static void EquipArmor(params string[] filter)
    {
      List<UOItem> searchItems = new List<UOItem>();
      List<Graphic> toEquip = new List<Graphic>();

      foreach (UOItem item in World.Player.Backpack.AllItems)
      {
        if (item != null && item.Exist && item.Graphic != null)
          searchItems.Add(item);
      }

      foreach (UOItem item in World.Ground)
      {
        if (item != null && item.Exist && item.Graphic != null && item.Distance < 3)
          searchItems.Add(item);
      }

      bool useFilter = filter.Length > 0;

      if (!useFilter || Array.IndexOf(filter, "ringmail") > -1)
        toEquip.AddRange(ItemLibrary.RingmailArmor.GraphicArray);
      if (!useFilter || Array.IndexOf(filter, "platemail") > -1)
        toEquip.AddRange(ItemLibrary.PlatemailArmor.GraphicArray);
      if (!useFilter || Array.IndexOf(filter, "chainmail") > -1)
        toEquip.AddRange(ItemLibrary.ChainmailArmor.GraphicArray);
      if (!useFilter || Array.IndexOf(filter, "bone") > -1)
        toEquip.AddRange(ItemLibrary.BoneArmor.GraphicArray);
      if (!useFilter || Array.IndexOf(filter, "leather") > -1)
        toEquip.AddRange(ItemLibrary.LeatherArmor.GraphicArray);
      if (!useFilter || Array.IndexOf(filter, "studded") > -1)
        toEquip.AddRange(ItemLibrary.StuddedArmor.GraphicArray);
      if (!useFilter || Array.IndexOf(filter, "robe") > -1)
        toEquip.AddRange(ItemLibrary.Robes.GraphicArray);
      if (!useFilter || Array.IndexOf(filter, "clothes") > -1)
        toEquip.AddRange(ItemLibrary.Clothes.GraphicArray);
      if (!useFilter || Array.IndexOf(filter, "shield") > -1)
        toEquip.AddRange(ItemLibrary.Shields.GraphicArray);
      if (!useFilter || Array.IndexOf(filter, "hats") > -1)
        toEquip.AddRange(ItemLibrary.Hats.GraphicArray);

      int counter = 0;
      foreach (UOItem item in searchItems)
      {
        if (item != null && item.Exist && item.Graphic != null)
        {
          if (Array.IndexOf(toEquip.ToArray(), item.Graphic) > -1)
          {
            item.Use();
            Game.Wait();
            counter++;
          }
        }
      }

      World.Player.PrintMessage("[Armor nahozen " + counter + "]");
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    [BlockMultipleExecutions]
    public static void EquipWeapon(params string[] filter)
    {
      List<UOItem> searchItems = new List<UOItem>();
      UOItemTypeCollection toEquipColl = new UOItemTypeCollection();

      foreach (UOItem item in World.Player.Backpack.AllItems)
      {
        if (item != null && item.Exist && item.Graphic != null)
          searchItems.Add(item);
      }

      foreach (UOItem item in World.Ground)
      {
        if (item != null && item.Exist && item.Graphic != null && item.Distance < 3)
          searchItems.Add(item);
      }

      bool useFilter = filter.Length > 0;

      if (!useFilter || Array.IndexOf(filter, "weaponfenc") > -1)
        toEquipColl.AddRange(ItemLibrary.WeaponsFenc);

      if (!useFilter || Array.IndexOf(filter, "weaponsword") > -1)
        toEquipColl.AddRange(ItemLibrary.WeaponsSword);

      if (!useFilter || Array.IndexOf(filter, "weaponmace") > -1)
        toEquipColl.AddRange(ItemLibrary.WeaponsMace);

      UOItem leftHand = World.Player.Layers[Layer.LeftHand];
      UOItem rightHand = World.Player.Layers[Layer.RightHand];

      int counter = 0;
      if (!rightHand.Serial.IsValid || !rightHand.Exist)
      {
        foreach (UOItem item in searchItems)
        {
          if (item != null && item.Exist && item.Graphic != null)
          {
            if (toEquipColl.FindItem(item.Graphic) != null)//Array.IndexOf(toEquipColl.GraphicArray, item.Graphic) > -1)
            {
              item.Use();
              Game.Wait();
              counter++;
              break;
            }
          }
        }
      }
      else
        World.Player.PrintMessage("* Uz mas zbran *");

      if (counter > 0)
        World.Player.PrintMessage("* Zbran nahozena *");
      else
        World.Player.PrintMessage("* Neni zbran *");

      
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    [BlockMultipleExecutions]
    public static void UnEquipArmor()
    {
      List<UOItem> equip = new List<UOItem>();
      equip.Add(World.Player.Layers[Layer.Arms]);
      equip.Add(World.Player.Layers[Layer.Cloak]);
      equip.Add(World.Player.Layers[Layer.Gloves]);
      equip.Add(World.Player.Layers[Layer.Hat]);
      equip.Add(World.Player.Layers[Layer.InnerLegs]);
      equip.Add(World.Player.Layers[Layer.InnerTorso]);
      equip.Add(World.Player.Layers[Layer.MiddleTorso]);
      equip.Add(World.Player.Layers[Layer.Neck]);
      equip.Add(World.Player.Layers[Layer.Arms]);
      equip.Add(World.Player.Layers[Layer.OuterLegs]);
      equip.Add(World.Player.Layers[Layer.OuterTorso]);
      equip.Add(World.Player.Layers[Layer.Pants]);
      //equip.Add(World.Player.Layers[Layer.LeftHand]);
      equip.Add(World.Player.Layers[Layer.Shirt]);
      equip.Add(World.Player.Layers[Layer.Shoes]);
      equip.Add(World.Player.Layers[Layer.Waist]);

      UOItem containerTo = null;//World.Player.Backpack.Items.FindType(0x0E75); kvuly NB vecem radsi do batuzku
      if (containerTo == null || !containerTo.Exist)
        containerTo = World.Player.Backpack;

      if (!containerTo.Opened)
        ItemHelper.EnsureContainer(containerTo);

      int counter = 0;
      foreach (UOItem item in equip)
      {
        if (item.IsItemType(ItemLibrary.KnightCloakGold) || item.IsItemType(ItemLibrary.KnightCloakBlack))
          continue;

        if (item != null && item.Exist && item.Graphic != null)
        {
          item.Move(1, containerTo.Serial, 143, 123);
          counter++;
        }
      }

      World.Player.PrintMessage("* Armor schozen " + counter + " *");
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    [BlockMultipleExecutions]
    public static void UnEquipShield()
    {
      List<UOItem> equip = new List<UOItem>();
      equip.Add(World.Player.Layers[Layer.LeftHand]);

      UOItem containerTo = null;//World.Player.Backpack.Items.FindType(0x0E75); kvuly NB vecem radsi do batuzku
      if (containerTo == null || !containerTo.Exist)
        containerTo = World.Player.Backpack;

      if (!containerTo.Opened)
        ItemHelper.EnsureContainer(containerTo);

      int counter = 0;
      foreach (UOItem item in equip)
      {
        if (item != null && item.Exist && item.Graphic != null)
        {
          if (Array.IndexOf(ItemLibrary.Shields.GraphicArray, item.Graphic) > -1)
          {
            item.Move(1, containerTo.Serial, 143, 123);
            counter++;
          }
        }
      }

      if (counter > 0)
        World.Player.PrintMessage("* Stit schozen *");
      else
        World.Player.PrintMessage("* Nemas Stit *");
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    [BlockMultipleExecutions]
    public static void UnEquipWeapon()
    {
      List<UOItem> equip = new List<UOItem>();
      equip.Add(World.Player.Layers[Layer.RightHand]);
      equip.Add(World.Player.Layers[Layer.LeftHand]);

      UOItem containerTo = null;//World.Player.Backpack.Items.FindType(0x0E75); kvuly NB vecem radsi do batuzku
      if (containerTo == null || !containerTo.Exist)
        containerTo = World.Player.Backpack;

      int counter = 0;
      foreach (UOItem item in equip)
      {
        if (item != null && item.Exist && item.Graphic != null)
        {
          if (Array.IndexOf(ItemLibrary.Shields.GraphicArray, item.Graphic) < 0)
          {
            UOItem finalTo = containerTo;
            item.Move(1, containerTo.Serial, 143, 123);
            counter++;
          }
        }
      }

      if (counter > 0)
        World.Player.PrintMessage("* Zbran schozena *");
      else
        World.Player.PrintMessage("* Nemas zbran *");
    }
    // pozice v backpack 143.123.0  
    // type batuzku backpack 0x0000  Graphic: 0x0E75 

    //---------------------------------------------------------------------------------------------

    [Executable]
    [BlockMultipleExecutions]
    public static void EquipAll(params string[] filter)
    {
      EquipArmor(filter);
      EquipWeapon(filter);
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    [BlockMultipleExecutions]
    public static void UnEquipAll()
    {
      UnEquipArmor();
      UnEquipShield();
      UnEquipWeapon();
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    [BlockMultipleExecutions]
    public static void UnEquipShieldAndWeapon()
    {
      UnEquipShield();
      UnEquipWeapon();
    }


    #endregion

    //---------------------------------------------------------------------------------------------
  }
}
