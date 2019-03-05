using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Phoenix;
using Phoenix.WorldData;
using System.ComponentModel;
using System.Threading;
using CalExtension.UOExtensions;
using CalExtension.Skills;
using Phoenix.Runtime;
using CalExtension.PlayerRoles;

namespace CalExtension.UI.Status
{
  public class SlotForm : InGameWindow
  {
    public static SlotForm Current;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:SuppliesForm"/> class.
    /// </summary>
    /// 


    //---------------------------------------------------------------------------------------------

    protected static Dictionary<string, SlotItem> Slots;
    protected static List<string> SlotKeyList;
    protected static object SyncRoot;

    protected static SlotItem GetSlotItem(string key)
    {
      lock (SyncRoot)
      {
        if (Slots.ContainsKey(key))
          return Slots[key];

        return null;
      }
    }

    protected static void SetRemoveSlotItem(string key, SlotItem item)
    {
      Graphic g = 0;
      UOColor c = 0;
      Serial s = 0;
      lock (SyncRoot)
      {
        if (item == null)
        {
          if (Slots.ContainsKey(key))
          {
            SlotKeyList.Remove(key);
            Slots.Remove(key);
          }

          Config.Profile.UserSettings.SetAttribute(g.ToString(), "Graphic", "SlotForm_Slots_" + key);
          Config.Profile.UserSettings.SetAttribute(c.ToString(), "UOColor", "SlotForm_Slots_" + key);
          Config.Profile.UserSettings.SetAttribute(s.ToString(), "Serial", "SlotForm_Slots_" + key);
        }
        else
        {
          if (Slots.ContainsKey(key))
            Slots[key] = item;
          else
          {
            SlotKeyList.Add(key);
            Slots.Add(key, item);
          }
        }
      }
    }

    //---------------------------------------------------------------------------------------------

    protected static bool IsSelected(SlotItem slotItem)
    {
      if (slotItem.SlotType == "Weapon")
      {
        UOItem item = new UOItem(slotItem.Serial);

        UOItem leftHand = World.Player.Layers[Layer.LeftHand];
        UOItem rightHand = World.Player.Layers[Layer.RightHand];

        if (item.Exist && slotItem.Serial.IsValid && slotItem.Serial > 0 && (leftHand.Exist && leftHand.Serial == item.Serial || rightHand.Exist && rightHand.Serial == item.Serial))
          return true;

        if (slotItem.Graphic > 0 && slotItem.Graphic == leftHand.Graphic && slotItem.Color == leftHand.Color)
          return true;

        if (slotItem.Graphic > 0 && slotItem.Graphic == rightHand.Graphic && slotItem.Color == rightHand.Color)
          return true;
      }

      return false;
    }

    //---------------------------------------------------------------------------------------------

    protected static void EnsureUnique(SlotItem slot)
    {
      lock (SyncRoot)
      {
        foreach (string key in SlotKeyList)
        {
          if (key == slot.Key)
            continue;

          SlotItem sl = Slots[key];
          if (sl.Serial == slot.Serial || sl.Graphic == slot.Graphic && sl.Color == slot.Color)
          {
            sl.Control.DataIndex = 0;
            sl.Control.HueIndex = 0;

            sl.Color = 0;
            sl.Graphic = 0;
            sl.Serial = 0;

            sl.Control.Invalidate();
          }
        }
      }
    }

    //---------------------------------------------------------------------------------------------

    protected static bool IsSlotItem(Serial s, out SlotItem slotItem)
    {
      return IsSlotItem(new UOItem(s), out slotItem);
    }
    protected static bool IsSlotItem(UOItem item, out SlotItem slotItem)
    {
      lock (SyncRoot)
      {
        slotItem = null;
        foreach (string key in Slots.Keys)
        {
          SlotItem sl = Slots[key];
          if (item.Serial.IsValid && item.Serial == sl.Serial)
          {
            slotItem = sl;
            return true;
          }
          if (item.Graphic > 0 && item.Graphic == sl.Graphic && item.Color == sl.Color)
          {
            slotItem = sl;
            return true;
          }
        }
        return false;
      }
    }

    //---------------------------------------------------------------------------------------------
    protected static void ClearSelection(string slotType)
    {
      lock (SyncRoot)
      {
        foreach (string key in Slots.Keys)
        {
          SlotItem sl = Slots[key];
          if (sl.SlotType == slotType)
            sl.Control.Selected = false;
        }
      }
    }

    //---------------------------------------------------------------------------------------------

    protected static void EnsureCounter(ObjectChangedEventArgs e, string slotType)
    {
      lock (SyncRoot)
      {
        foreach (string key in Slots.Keys)
        {
          SlotItem sl = Slots[key];
          if (sl.SlotType != slotType)
            continue;

          UOItem itm = new UOItem(e.ItemSerial);

          if (!sl.IsEmpty && (e.Type == ObjectChangeType.SubItemRemoved || sl.Graphic == itm.Graphic && sl.Color == itm.Color))
          {
            sl.Control.Counter = World.Player.Backpack.AllItems.Count(sl.Graphic, sl.Color);
          }

        }
      }
    }

    //---------------------------------------------------------------------------------------------

    //protected static void EnsureExists()
    //{
    //  lock (SyncRoot)
    //  {
    //    foreach (string key in Slots.Keys)
    //    {
    //      SlotItem sl = Slots[key];
    //      sl.Control.Grayscale = !sl.IsEmpty && !sl.Exist;
    //    }
    //  }
    //}

    protected static void EnsureSelection(string slotType)
    {
      lock (SyncRoot)
      {
        Dictionary<string, bool> selectetBreak = new Dictionary<string, bool>();

        foreach (string key in Slots.Keys)
        {
          SlotItem sl = Slots[key];
          if (sl.SlotType != slotType)
            continue;

          if (!selectetBreak.ContainsKey(sl.SlotType))
            selectetBreak.Add(sl.SlotType, false);


          sl.Control.Selected = IsSelected(sl) && !selectetBreak[sl.SlotType];

          if (sl.Control.Selected)
            selectetBreak[sl.SlotType] = true;

          //sl.Control.Grayscale = !sl.IsEmpty && !sl.Exist;
        }
      }
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void SwitchSlotWeapon()
    {
      if (Current == null)
      {
        Fighter.ExecSwitchWeapon();
        return;
      }

      if (Current.InvokeRequired)
      {
        Current.BeginInvoke(new ThreadStart(SwitchSlotWeapon));
        return;
      }

      lock (SyncRoot)
      {
        int selIndex = 0;
        List<int> possible = new List<int>();
        for (int i = 0; i < SlotKeyList.Count; i++)
        {
          SlotItem sl = Slots[SlotKeyList[i]];
          if (sl.SlotType != "Weapon")
            continue;

          if (sl.Control.Selected)
          {
            selIndex = i;
            sl.Control.Selected = false;
          }
        
          if (!sl.IsEmpty && sl.Selectable && sl.Exist)
            possible.Add(i);
        }

        if (possible.Count > 0)
        {
          for (int i = 0; i < possible.Count; i++)
          {
            int curr = possible[i];
            if (curr > selIndex)
            {
              EquipSlotWeapon(Slots[SlotKeyList[curr]]);
              return;
            }
          }
          for (int i = 0; i < possible.Count; i++)
          {
            int curr = possible[i];
            if (curr <= selIndex)
            {
              if (curr == selIndex)
                Slots[SlotKeyList[curr]].Control.Selected = true;

              EquipSlotWeapon(Slots[SlotKeyList[curr]]);
              return;
            }
          }

        }
      
      }
    }


    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void UseSlotKlamak()
    {
      if (Current == null)
      {
        MobMaster.ExecVyhodKlamak();
        return;
      }

      if (Current.InvokeRequired)
      {
        Current.BeginInvoke(new ThreadStart(UseSlotKlamak));
        return;
      }

      SlotItem slot = null;
      lock (SyncRoot)
      {
        for (int i = 0; i < SlotKeyList.Count; i++)
        {
          SlotItem sl = Slots[SlotKeyList[i]];
          if (sl.SlotType != "Klamak")
            continue;

          if (sl.Control.Selected)
          {
            slot = sl;
            break;
          }
        }

        if (slot == null || !slot.Exist)
        {
          for (int i = 0; i < SlotKeyList.Count; i++)
          {
            SlotItem sl = Slots[SlotKeyList[i]];
            if (sl.SlotType != "Klamak")
              continue;

            if (!sl.IsEmpty && sl.Exist)
            {
              slot = sl;
              break;
            }
          }
        }
      }

      if (slot != null && slot.Exist)
      {
        Game.RunScriptCheck(650);
        World.Player.PrintMessage("[ " + slot.ItemName + " ]");
        Game.CurrentGame.CurrentPlayer.SwitchWarmode();
        slot.Item.Use();
        //itemKlamak.Use();
      }
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void SwitchSlotKlamak()
    {
      if (Current == null)
      {
        MobMaster.ExecMoveKlamakNext(1);
        return;
      }

      if (Current.InvokeRequired)
      {
        Current.BeginInvoke(new ThreadStart(SwitchSlotKlamak));
        return;
      }

      lock (SyncRoot)
      {
        int selIndex = 0;
        List<int> possible = new List<int>();
        for (int i = 0; i < SlotKeyList.Count; i++)
        {
          SlotItem sl = Slots[SlotKeyList[i]];
          if (sl.SlotType != "Klamak")
            continue;

          if (sl.Control.Selected)
          {
            selIndex = i;
            sl.Control.Selected = false;
          }

          if (!sl.IsEmpty && sl.Selectable && sl.Exist)
            possible.Add(i);
        }

        if (possible.Count > 0)
        {
          for (int i = 0; i < possible.Count; i++)
          {
            int curr = possible[i];
            if (curr > selIndex)
            {
              SlotItem s = Slots[SlotKeyList[curr]];
              s.Control.Selected = true;

              if (!String.IsNullOrEmpty(s.ItemName))
                World.Player.PrintMessage("[ " + s.ItemName + "]");

              //EquipSlotWeapon(Slots[SlotKeyList[curr]]);
              return;
            }
          }
          for (int i = 0; i < possible.Count; i++)
          {
            int curr = possible[i];
            if (curr <= selIndex)
            {
              SlotItem s = Slots[SlotKeyList[curr]];
              s.Control.Selected = true;

              if (!String.IsNullOrEmpty(s.ItemName))
                World.Player.PrintMessage("[ " + s.ItemName + "]");
              //EquipSlotWeapon(Slots[SlotKeyList[curr]]);
              return;
            }
          }
        }
      }
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void EquipSlotWeapon(string key, string printAlias)
    {
      if (Current == null)
      {
        Graphic g = Graphic.Parse(Config.Profile.UserSettings.GetAttribute("0", "Graphic", "SlotForm_Slots_" + key));
        UOColor c = UOColor.Parse(Config.Profile.UserSettings.GetAttribute("0", "UOColor", "SlotForm_Slots_" + key));
        Serial s = Serial.Parse(Config.Profile.UserSettings.GetAttribute("0", "Serial", "SlotForm_Slots_" + key));

        if (g > 0 || s.IsValid)
        {
          Fighter.EquipSlotItem(String.IsNullOrEmpty(printAlias) ? key : printAlias, g, c, true);
        }

        return;
      }

      SlotItem sl = GetSlotItem(key);
      if (sl != null && sl.SlotType == "Weapon")
        EquipSlotWeapon(sl, printAlias);
    }

    //---------------------------------------------------------------------------------------------

    public static void EquipSlotWeapon(SlotItem sl)
    {
      EquipSlotWeapon(sl, null);
    }

    //---------------------------------------------------------------------------------------------

    public static void EquipSlotWeapon(SlotItem sl, string printAlias)
    {
      if (sl.IsEmpty)
        return;

      UOItem slotItem = new UOItem(sl.Serial);
      if (!slotItem.Exist || slotItem.Distance > 3)
        slotItem = World.Player.FindType(sl.Graphic, sl.Color);

      if (Fighter.CheckCurrentWeapon(true))
      {
        if (!(slotItem.Layer == Layer.LeftHand || slotItem.Layer == Layer.RightHand))
        {

          slotItem.Use();
          Game.Wait(100);
          if (UIManager.CurrentState != UIManager.State.Ready)
            Targeting.ResetTarget();

          if (!String.IsNullOrEmpty(printAlias))
            World.Player.PrintMessage("[ " + printAlias + " ]");
        }

        if (!World.Player.Layers[Layer.LeftHand].Exist)
        {
          UOItem shield = Fighter.Current.GetSlotItem("__LastShield");
          if (!shield.Exist)
          {
            Fighter.Current.SwitchShield();
          }
          else
            shield.Use();
        }
      }
    }
    //---------------------------------------------------------------------------------------------

    private int slotsCount = 1;
    public SlotForm() : this(7)
    {

    }

    public SlotForm(int slotsCount)
    {
      if (Current == null)
        Current = this;

      if (Slots == null)
      {
        SlotKeyList = new List<string>();
        Slots = new Dictionary<string, SlotItem>();
      }

      if (SyncRoot == null)
        SyncRoot = new object();

      MouseLeave += StatusForm_MouseLeave;
      this.slotsCount = slotsCount;
      this.InitializeComponent();
      World.Player.Backpack.Changed += Backpack_Changed;
      World.Player.Changed += Player_Changed;
      CalebConfig.LootChanged += CalebConfig_LootChanged;
    }


    protected bool lastWarMode = World.Player.Warmode;
    //---------------------------------------------------------------------------------------------

    private void Player_Changed(object sender, ObjectChangedEventArgs e)
    {
      SlotItem sl = null;
      bool isSlotItem = IsSlotItem(e.ItemSerial, out sl);
      if (isSlotItem)
      {
        EnsureSelection("Weapon");
      }

      if (lastWarMode != World.Player.Warmode)
      {
        UpdateWarButtonByWarMode();
      }
      lastWarMode = World.Player.Warmode;
    }

    //---------------------------------------------------------------------------------------------

    private void Backpack_Changed(object sender, ObjectChangedEventArgs e)
    {
      SlotItem sl = null;
      bool isSlotItem = IsSlotItem(e.ItemSerial, out sl);
      if (isSlotItem)
      {
        EnsureSelection("Weapon");
      }
    }

    //---------------------------------------------------------------------------------------------

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      int defaultX = 0;
      int defaultY = 0;
      this.BackColor = Color.Black;
      int x = Config.Profile.UserSettings.GetAttribute(defaultX, "Value", "SlotForm_LocationX");
      int y = Config.Profile.UserSettings.GetAttribute(defaultY, "Value", "SlotForm_LocationY");
      this.Location = new Point(x, y);
      this.Invalidate();
     // initRun = false;
    }

    //---------------------------------------------------------------------------------------------

    protected Color? DefaultColor = Color.Black;
    public bool MouseHovering = false;

    void StatusForm_MouseEnter(object sender, EventArgs e)
    {
      MouseHovering = true;
    }

    //---------------------------------------------------------------------------------------------

    void StatusForm_MouseLeave(object sender, EventArgs e)
    {
      MouseHovering = false;

      Config.Profile.UserSettings.SetAttribute(this.Location.X, "Value", "SlotForm_LocationX");
      Config.Profile.UserSettings.SetAttribute(this.Location.Y, "Value", "SlotForm_LocationY");
    }

    //---------------------------------------------------------------------------------------------

    protected override void OnClosing(CancelEventArgs e)
    {
      World.Player.Backpack.Changed -= Backpack_Changed;
      World.Player.Changed -= Player_Changed;
      foreach (SlotItem sl in Slots.Values)
      {
        sl.DisposeCounter();
      }
      Current = null;
      base.OnClosing(e);

    }

    //---------------------------------------------------------------------------------------------

    protected override void Dispose(bool disposing)
    {
      World.Player.Backpack.Changed -= Backpack_Changed;
      World.Player.Changed -= Player_Changed;

      
      foreach (SlotItem sl in Slots.Values)
      {
        sl.DisposeCounter();
      }
      Current = null;
      base.Dispose(disposing);

    }

    //---------------------------------------------------------------------------------------------

    //private Phoenix.Gui.Controls.ArtImageControlExt slot1;

    Label name;
    Button warMode;
    Button btnLootType;

    //---------------------------------------------------------------------------------------------
    //  private bool initRun = true;
    private void InitializeComponent()
    {
      //initRun = true;
      //slots = new List<Phoenix.Gui.Controls.ArtImageControlExt>();
      Slots = new Dictionary<string, SlotItem>();
      this.SuspendLayout();

      Font font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      Font fontSmall = new System.Drawing.Font("Arial", 6.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

      int defaultPadding = 2;
      int maxX = 0;
      int maxY = 0;
      int currentLine = 0 + defaultPadding;
      int currentPosition = 0 + defaultPadding;

      int buttonWidth = 32;
      int buttonHeight = 32;

      this.name = new Label();
      this.name.AutoSize = true;
      this.name.BackColor = System.Drawing.Color.Transparent;
      this.name.Enabled = false;
      this.name.Font = font;
      this.name.Location = new System.Drawing.Point(2, 2);
      this.name.Name = "name";
      this.name.TabIndex = 0;
      this.name.Text = "Slots (hold CTRL to add, Alt to skip, RClick to remove, LClick to use)";
      this.Controls.Add(this.name);

      Control prevCont = this.name;

      currentPosition = defaultPadding;
      currentLine = prevCont.Location.Y + prevCont.Size.Height + defaultPadding;

      int slotIndex = -1;
      for (int i = 0; i < slotsCount; i++)
      {
        slotIndex++;
        Phoenix.Gui.Controls.ArtImageControlExt slot1 = new Phoenix.Gui.Controls.ArtImageControlExt();
        slot1.ArtData = DataFiles.Art.Items;
        slot1.Hues = DataFiles.Hues;
        slot1.Enabled = true;
        slot1.Location = new Point(currentPosition, currentLine);
        slot1.ImageAlignment = Phoenix.Gui.Controls.ImageAlignment.TopLeft;
        slot1.Stocked = true;
        
        //slot1.Selected = true;
        slot1.Size = new Size(buttonWidth, buttonHeight);
        slot1.Name = "Slot_" + slotIndex;
        slot1.MouseClick += Slot_MouseClick;
        
        Graphic g = Graphic.Parse(Config.Profile.UserSettings.GetAttribute("0", "Graphic", "SlotForm_Slots_" + slot1.Name));
        UOColor c = UOColor.Parse(Config.Profile.UserSettings.GetAttribute("0", "UOColor", "SlotForm_Slots_" + slot1.Name));
        Serial s = Serial.Parse(Config.Profile.UserSettings.GetAttribute("0", "Serial", "SlotForm_Slots_" + slot1.Name));
        bool selectable = Config.Profile.UserSettings.GetAttribute(true, "Selectable", "SlotForm_Slots_" + slot1.Name);
        string itemName = Config.Profile.UserSettings.GetAttribute("", "ItemName", "SlotForm_Slots_" + slot1.Name);

        SlotItem sl = new SlotItem() { Key = slot1.Name, Control = slot1, Graphic = g, Color = c, Serial = s, Selectable = selectable, SlotType = "Weapon", ItemName = itemName };
        SetRemoveSlotItem(slot1.Name, sl);

        slot1.Selected = IsSelected(sl);
        slot1.Mark1 = !selectable;

        if (!sl.IsEmpty)
        {
          sl.RegisterCounter();
        }

        if (c > 0 && c.IsConstant)
        {
          slot1.UseHue = true;
        }

        slot1.HueIndex = c;
        slot1.DataIndex = g;

        Controls.Add(slot1);

        prevCont = slot1;
        currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;
      }

      currentPosition = currentPosition + 10;

      for (int i = 0; i < 4; i++)
      {
        slotIndex++;
        Phoenix.Gui.Controls.ArtImageControlExt slot1 = new Phoenix.Gui.Controls.ArtImageControlExt();
        slot1.ArtData = DataFiles.Art.Items;
        slot1.Hues = DataFiles.Hues;
        slot1.Enabled = true;
        slot1.Location = new Point(currentPosition, currentLine);
        slot1.ImageAlignment = Phoenix.Gui.Controls.ImageAlignment.TopLeft;
        slot1.Stocked = true;

        //slot1.Selected = true;
        slot1.Size = new Size(buttonWidth, buttonHeight);
        slot1.Name = "Slot_" + slotIndex;
        slot1.MouseClick += Slot_MouseClick;

        Graphic g = Graphic.Parse(Config.Profile.UserSettings.GetAttribute("0", "Graphic", "SlotForm_Slots_" + slot1.Name));
        UOColor c = UOColor.Parse(Config.Profile.UserSettings.GetAttribute("0", "UOColor", "SlotForm_Slots_" + slot1.Name));
        Serial s = Serial.Parse(Config.Profile.UserSettings.GetAttribute("0", "Serial", "SlotForm_Slots_" + slot1.Name));
        bool selectable = Config.Profile.UserSettings.GetAttribute(true, "Selectable", "SlotForm_Slots_" + slot1.Name);
        string itemName = Config.Profile.UserSettings.GetAttribute("", "ItemName", "SlotForm_Slots_" + slot1.Name);

        SlotItem sl = new SlotItem() { Key = slot1.Name, Control = slot1, Graphic = g, Color = c, Serial = s, Selectable = selectable, SlotType = "Klamak", ItemName = itemName };
        SetRemoveSlotItem(slot1.Name, sl);

        slot1.Selected = IsSelected(sl);
        slot1.Mark1 = !selectable;
        sl.PrintCounter = true;

        if (!sl.IsEmpty)
        {
          sl.RegisterCounter();
        }
        else
        {

        }

        if (c > 0 && c.IsConstant)
        {
          slot1.UseHue = true;
        }

        slot1.HueIndex = c;
        slot1.DataIndex = g;

        Controls.Add(slot1);

        prevCont = slot1;
        currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;
      }

      currentPosition = currentPosition + 10;
      currentLine = defaultPadding;
      int potionBarStart = currentPosition;

      int potionSlotW = 16;
      int potionSlotH = 24;

      for (int i = 0; i < 4; i++)
      {
        slotIndex++;
        Phoenix.Gui.Controls.ArtImageControlExt slot1 = new Phoenix.Gui.Controls.ArtImageControlExt();
        slot1.ArtData = DataFiles.Art.Items;
        slot1.Hues = DataFiles.Hues;
        slot1.Enabled = true;
        slot1.Location = new Point(currentPosition, currentLine);
        slot1.ImageAlignment = Phoenix.Gui.Controls.ImageAlignment.TopLeft;
        slot1.Stocked = true;
        slot1.CounterTextFontSize = 6.0F;

        //slot1.Selected = true;
        slot1.Size = new Size(potionSlotW, potionSlotH);
        slot1.Name = "Slot_" + slotIndex;
        slot1.MouseClick += Slot_MouseClick;

        Graphic g = Graphic.Parse(Config.Profile.UserSettings.GetAttribute("0", "Graphic", "SlotForm_Slots_" + slot1.Name));
        UOColor c = UOColor.Parse(Config.Profile.UserSettings.GetAttribute("0", "UOColor", "SlotForm_Slots_" + slot1.Name));
        Serial s = Serial.Parse(Config.Profile.UserSettings.GetAttribute("0", "Serial", "SlotForm_Slots_" + slot1.Name));
        bool selectable = Config.Profile.UserSettings.GetAttribute(true, "Selectable", "SlotForm_Slots_" + slot1.Name);
        string itemName = Config.Profile.UserSettings.GetAttribute("", "ItemName", "SlotForm_Slots_" + slot1.Name);

        SlotItem sl = new SlotItem() { Key = slot1.Name, Control = slot1, Graphic = g, Color = c, Serial = s, Selectable = selectable, SlotType = "Potion", ItemName = itemName };
        SetRemoveSlotItem(slot1.Name, sl);

        slot1.Selected = IsSelected(sl);
        slot1.Mark1 = !selectable;
        sl.PrintCounter = true;

        if (!sl.IsEmpty)
        {
          sl.RegisterCounter();
        }

        if (c > 0 && c.IsConstant)
        {
          slot1.UseHue = true;
        }

        slot1.HueIndex = c;
        slot1.DataIndex = g;

        Controls.Add(slot1);

        prevCont = slot1;
        currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;
      }

      currentPosition = potionBarStart;
      currentLine = currentLine + potionSlotH + defaultPadding;

      for (int i = 0; i < 4; i++)
      {
        slotIndex++;
        Phoenix.Gui.Controls.ArtImageControlExt slot1 = new Phoenix.Gui.Controls.ArtImageControlExt();
        slot1.ArtData = DataFiles.Art.Items;
        slot1.Hues = DataFiles.Hues;
        slot1.Enabled = true;
        slot1.Location = new Point(currentPosition, currentLine);
        slot1.ImageAlignment = Phoenix.Gui.Controls.ImageAlignment.TopLeft;
        slot1.Stocked = true;
        slot1.CounterTextFontSize = 6.0F;

        //slot1.Selected = true;
        slot1.Size = new Size(potionSlotW, potionSlotH);
        slot1.Name = "Slot_" + slotIndex;
        slot1.MouseClick += Slot_MouseClick;

        Graphic g = Graphic.Parse(Config.Profile.UserSettings.GetAttribute("0", "Graphic", "SlotForm_Slots_" + slot1.Name));
        UOColor c = UOColor.Parse(Config.Profile.UserSettings.GetAttribute("0", "UOColor", "SlotForm_Slots_" + slot1.Name));
        Serial s = Serial.Parse(Config.Profile.UserSettings.GetAttribute("0", "Serial", "SlotForm_Slots_" + slot1.Name));
        bool selectable = Config.Profile.UserSettings.GetAttribute(true, "Selectable", "SlotForm_Slots_" + slot1.Name);
        string itemName = Config.Profile.UserSettings.GetAttribute("", "ItemName", "SlotForm_Slots_" + slot1.Name);

        SlotItem sl = new SlotItem() { Key = slot1.Name, Control = slot1, Graphic = g, Color = c, Serial = s, Selectable = selectable, SlotType = "Potion", ItemName = itemName };

        SetRemoveSlotItem(slot1.Name, sl);

        slot1.Selected = IsSelected(sl);
        slot1.Mark1 = !selectable;
        sl.PrintCounter = true;
        if (!sl.IsEmpty)
        {
          sl.RegisterCounter();
        }

        if (c > 0 && c.IsConstant)
        {
          slot1.UseHue = true;
        }

        slot1.HueIndex = c;
        slot1.DataIndex = g;

        Controls.Add(slot1);

        prevCont = slot1;
        currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;
      }

      currentLine = currentLine - (potionSlotH + defaultPadding);

      this.warMode = new Button();
      this.warMode.Name = "warMode";
      this.warMode.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.warMode.Enabled = true;

      this.warMode.Font = fontSmall;
      this.warMode.Size = new Size(40, potionSlotH);
      //this.btnMixureLC.AutoSize = true;

      this.warMode.ForeColor = Color.White;
      this.warMode.Padding = new Padding(0);
      this.warMode.TabStop = false;
      this.warMode.FlatStyle = FlatStyle.Flat;
      this.warMode.FlatAppearance.BorderSize = 0;
      this.warMode.MouseClick += WarMode_MouseClick;
      UpdateWarButtonByWarMode();
      this.Controls.Add(warMode);

      prevCont = warMode;

      currentPosition = prevCont.Location.X;
      currentLine = prevCont.Location.Y + potionSlotH + defaultPadding;

      this.btnLootType = new Button();
      this.btnLootType.Name = "btnLootType";
      this.btnLootType.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnLootType.Enabled = true;

      this.btnLootType.Font = fontSmall;
      this.btnLootType.Size = new Size(40, potionSlotH);
      //this.btnMixureLC.AutoSize = true;

      this.btnLootType.ForeColor = Color.White;
      this.btnLootType.Padding = new Padding(0);
      this.btnLootType.TabStop = false;
      this.btnLootType.FlatStyle = FlatStyle.Flat;
      this.btnLootType.FlatAppearance.BorderSize = 0;
      this.btnLootType.MouseClick += BtnLootType_MouseClick;
      UpdateLootTypeButton();
      this.Controls.Add(btnLootType);

      prevCont = btnLootType;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      foreach (Control c in this.Controls)
      {
        if (maxX < c.Location.X + c.Size.Width)
          maxX = c.Location.X + c.Size.Width;

        if (maxY < c.Location.Y + c.Size.Height)
          maxY = c.Location.Y + c.Size.Height;
      }

      this.Size = new Size(maxX + defaultPadding, maxY + defaultPadding);

      this.DoubleBuffered = true;
      this.Name = "SlotForm";
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    //---------------------------------------------------------------------------------------------

    private void BtnLootType_MouseClick(object sender, MouseEventArgs e)
    {
      Game.LootSwitch();
    }

    //---------------------------------------------------------------------------------------------

    private void WarMode_MouseClick(object sender, MouseEventArgs e)
    {
      World.Player.ChangeWarmode(WarmodeChange.Switch);
    }

    //---------------------------------------------------------------------------------------------

    private void CalebConfig_LootChanged(object sender, EventArgs e)
    {
      UpdateLootTypeButton();
    }


    //---------------------------------------------------------------------------------------------

    protected void UpdateWarButtonByWarMode()
    {
      if (this.InvokeRequired)
      {
        this.BeginInvoke(new ThreadStart(UpdateWarButtonByWarMode));
        return;
      }

      this.warMode.Text = World.Player.Warmode ? "War" : "Peace";
      this.warMode.BackColor = World.Player.Warmode ? Color.Crimson : Color.DodgerBlue;
      this.warMode.Invalidate();
    }

    //---------------------------------------------------------------------------------------------

    protected void UpdateLootTypeButton()
    {
      if (this.InvokeRequired)
      {
        this.BeginInvoke(new ThreadStart(UpdateLootTypeButton));
        return;
      }

      string text = "Off";
      if (CalebConfig.Loot == LootType.Quick)
        text = "Quick";
      else if (CalebConfig.Loot == LootType.QuickCut)
        text = "Cut";
      else if (CalebConfig.Loot == LootType.OpenCorpse)
        text = "Open";

      this.btnLootType.Text = text;
      this.btnLootType.BackColor = CalebConfig.Loot == LootType.None ? Color.SandyBrown : Color.DeepSkyBlue; // World.Player.Warmode ? Color.Crimson : Color.DodgerBlue;
      this.btnLootType.Invalidate();
    }

    //---------------------------------------------------------------------------------------------

    private void Slot_MouseClick(object sender, MouseEventArgs e)
    {

      Phoenix.Gui.Controls.ArtImageControlExt image = (Phoenix.Gui.Controls.ArtImageControlExt)sender;
      SlotItem slot = GetSlotItem(image.Name);
      if (slot == null)
        return;

      if (e.Button == MouseButtons.Right)
      {
        slot.Color = 0;
        slot.Graphic = 0;
        slot.Serial = 0;
        slot.ItemName = "";
        slot.Selectable = true;
        slot.DisposeCounter();


        image.Selected = false;
        image.Mark1 = false;
        image.Grayscale = false;
        image.DataIndex = 0;
        image.HueIndex = 0;
      }
      else if (e.Button == MouseButtons.Left)
      {
        bool currCtrlHold = (ModifierKeys & Keys.Control) == Keys.Control;
        bool currAltHold = (ModifierKeys & Keys.Alt) == Keys.Alt;

        if (currCtrlHold && !currAltHold)
        {
          image.Selection = true;
          UOItem item = new UOItem(UIManager.TargetObject());
          image.Selection = false;


          if (item.Exist)
          {
            slot.Graphic = item.Graphic;
            slot.Color = item.Color;
            slot.Serial = item.Serial;
            slot.Selectable = true;

            if (String.IsNullOrEmpty(item.Name))
            {
              item.Click();
              Game.Wait(100);
            }
            slot.ItemName = item.Name;

            image.Mark1 = !slot.Selectable;
            image.DataIndex = item.Graphic;

            if (item.Color > 0 && item.Color.IsConstant)
            {
              image.UseHue = true;
              image.HueIndex = item.Color;
            }
            else
            {
              image.UseHue = false;
            }

            
            if (slot.SlotType == "Klamak" || slot.SlotType == "Potion")
              slot.PrintCounter = true;

            slot.RegisterCounter();

            EnsureUnique(slot);
            EnsureSelection("Weapon");
            //EnsureExists();
          }
          else
          {
            //smazat? radsi ne 
          }
        }
        else if (currAltHold && !currCtrlHold)
        {
          if (slot.SlotType == "Potion")
          {

          }
          else
          {
            slot.Selectable = !slot.Selectable;
            image.Mark1 = !slot.Selectable;
          }
        }
        else
        {
          if (slot.SlotType == "Potion")
          {
            if (!slot.IsEmpty)
            {
              Potion p = PotionCollection.GetPotionBy(slot.Graphic, slot.Color);
              if (p != null)
              {
                Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["DrinkPotion"], p.Name);
              }
            }
          }
          else if (slot.Exist)
          {
            if (slot.SlotType == "Weapon")
              Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.CommandList["useobject"], slot.Item.Serial);
            else if (slot.SlotType == "Klamak")
            {
              //ClearSelection("Klamak");
              //image.Selected = true;

              Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["VyhodKlamakNa"], slot.Item.Serial);
            }
          }

        }
      }

      image.Invalidate();
    }


    //  Serial: 0x40358901  Name: "Scimitar of Vanquishing"  Position: 134.131.0  Flags: 0x0000  Color: 0x044D  Graphic: 0x13B5  Amount: 1  Layer: RightHand Container: 0x00381EB5

    //---------------------------------------------------------------------------------------------
  }

  public class SlotItem
  {

    public string Key = "";
    public string SlotType = "Weapon";
    public bool PrintCounter = false;

    public bool IsEmpty
    {
      get
      {
        return Serial == 0 && Graphic == 0;
      }
    }

    public bool Exist
    {
      get
      {
        return !IsEmpty && Counter != null && Counter.CurrentAmount > 0; //Item.Exist;
      }
    }

    public UOItem Item
    {
      get
      {
        UOItem item = new UOItem(Serial.Invalid);
        if (!item.Exist && !Graphic.IsInvariant && Graphic > 0)
          item = World.Player.FindType(Graphic, Color, Serial);

        return item;
      }
    }


    private Graphic graphic = 0;
    public Graphic Graphic
    {
      get { return graphic; }
      set
      {
        graphic = value;
        Config.Profile.UserSettings.SetAttribute(value.ToString(), "Graphic", "SlotForm_Slots_" + Key);
      }
    }
    private UOColor color = 0;
    public UOColor Color
    {
      get { return color; }
      set
      {
        color = value;
        Config.Profile.UserSettings.SetAttribute(value.ToString(), "UOColor", "SlotForm_Slots_" + Key);
      } 
    }

    private Serial serial = Serial.Invalid;
    public Serial Serial
    {
      get { return serial; }
      set
      {
        serial = value;
        Config.Profile.UserSettings.SetAttribute(value.ToString(), "Serial", "SlotForm_Slots_" + Key);
      }
    }

    private bool selectable = true;
    public bool Selectable
    {
      get { return selectable; }
      set
      {
        selectable = value;
        Config.Profile.UserSettings.SetAttribute(value, "Selectable", "SlotForm_Slots_" + Key);
      }
    }

    private string itemName = String.Empty;
    public string ItemName
    {
      get { return itemName; }
      set
      {
        itemName = value;
        Config.Profile.UserSettings.SetAttribute(value, "ItemName", "SlotForm_Slots_" + Key);
      }
    }

    private void counter_AmountChanged(object sender, EventArgs e)
    {
      if (Control.InvokeRequired)
      {
        Control.BeginInvoke(new EventHandler(counter_AmountChanged), sender, e);
        return;
      }

      if (PrintCounter)
        Control.Counter = Counter.CurrentAmount;

      Control.Grayscale = !this.IsEmpty && Counter.CurrentAmount <= 0;
    }

    public void RegisterCounter()
    {
      DisposeCounter();
      RegisterCounter(false);
    }
    public void RegisterCounter(bool track)
    {
      this.Counter = new SupplyCounter(World.Player, Graphic, Color) { Track = track };
      this.Counter.AmountChanged += counter_AmountChanged;
      this.Counter.Recalc();
    }

    public void DisposeCounter()
    {
      PrintCounter = false;
      if (Counter != null)
      {
        this.Control.Counter = null;
        this.Counter.AmountChanged -= counter_AmountChanged;
        this.Counter.Dispose();
        this.Counter = null;
      }
    }
    public Phoenix.Gui.Controls.ArtImageControlExt Control;
    public SupplyCounter Counter;
  }

}


      
         