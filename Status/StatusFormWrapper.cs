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

namespace CalExtension.UI.Status
{
  public enum StatusFormWrapperType
  {
    None = 0,
    Enemy = 1,
    Mob = 2,
    Other = 4,
    Free = 8
  }

  public enum StatusFormWrapperSortType
  {
    Distance = 1,
    Hits = 2,
    MaxHits = 4,
    Name = 8, 
    Time = 16,
    Model = 32,
    Damage = 64
  }

  public enum StatusFormWrapperSortDirection
  {
    Asc = 1,
    Desc = 2
  }


  public class StatusFormWrapper : InGameWindow
  {
    private bool auto = false;
    public bool Auto
    {
      get { return this.auto;  }
      set
      {
        this.auto = value;
        Config.Profile.UserSettings.SetAttribute(this.auto, "Value", "StatusFormWrapper_" + this.WrapperType + "_Auto");
      }
    }

    private int maxBars = 12;
    public int MaxBars
    {
      get { return this.maxBars; }
      set
      {
        this.maxBars = value;
        Config.Profile.UserSettings.SetAttribute(this.maxBars, "Value", "StatusFormWrapper_" + this.WrapperType + "_MaxBars");
      }
    }

    private StatusFormWrapperSortType sortType = StatusFormWrapperSortType.Time;
    public StatusFormWrapperSortType SortType
    {
      get { return this.sortType; }
      set
      {
        this.sortType = value;
        Config.Profile.UserSettings.SetAttribute((int)this.sortType, "Value", "StatusFormWrapper_" + this.WrapperType + "_SortType");
      }
    }

    private StatusFormWrapperSortDirection sortDir = StatusFormWrapperSortDirection.Asc;
    public StatusFormWrapperSortDirection SortDir
    {
      get { return this.sortDir; }
      set
      {
        this.sortDir = value;
        Config.Profile.UserSettings.SetAttribute((int)this.sortDir, "Value", "StatusFormWrapper_" + this.WrapperType + "_SortDir");
      }
    }


    protected List<Serial> registeredSerials;
    public StatusFormWrapperType WrapperType = StatusFormWrapperType.Enemy;
    /// <summary>
    /// Initializes a new instance of the <see cref="T:SuppliesForm"/> class.
    /// </summary>
    public StatusFormWrapper(StatusFormWrapperType wrapperType)
    {
      MouseEnter += StatusForm_MouseEnter;
      MouseLeave += StatusForm_MouseLeave;

      this.WrapperType = wrapperType;
      this.registeredSerials = new List<Serial>();
      if (this.WrapperType == StatusFormWrapperType.Enemy)
      {
        this.auto = true;
        this.BackColor = Color.Firebrick;
      }
      else if (this.WrapperType == StatusFormWrapperType.Mob)
        this.BackColor = Color.Gold;
      else if (this.WrapperType == StatusFormWrapperType.Free)
        this.BackColor = Color.Goldenrod;

      else
        this.BackColor = Color.RoyalBlue;

      this.DefaultColor = this.BackColor;

      this.InitializeComponent();

      if (t == null)
      {
        t = new Thread(new ThreadStart(Main));
        t.Start();
      }

      StatusBar.OnStatusShow += StatusBar_OnStatusShow;
      //World.Player.Changed += Player_Changed;
      World.CharacterAppeared += World_CharacterAppeared;
    }

    //---------------------------------------------------------------------------------------------

    Thread t;
    private void Main()
    {
      while (true)
      {
        Thread.Sleep(50);
        RefreshForms();
      }
    }

    //---------------------------------------------------------------------------------------------

    private void GetAllCharacters()
    {
      List<UOCharacter> all = World.Characters.ToList();

      //lock (this.SyncRoot)
      //{
        foreach (UOCharacter ch in all)
        {
          if (!this.toDispatchSerials.Contains(ch.Serial))
            toDispatchSerials.Add(ch.Serial);
        }
      //}

    }

    //---------------------------------------------------------------------------------------------

    private void StatusBar_OnStatusShow(object sender, CharacterAppearedEventArgs e)
    {
      //lock (this.SyncRoot)
      //{
        if (!this.toDispatchSerials.Contains(e.Serial))
          toDispatchSerials.Add(e.Serial);
 //     }
      //RefreshForms();
    }

    //---------------------------------------------------------------------------------------------

    private void World_CharacterAppeared(object sender, CharacterAppearedEventArgs e)
    {
      if (this.Auto)
      {
        //lock(this.SyncRoot)
        //{
          if (!this.toDispatchSerials.Contains(e.Serial))
            toDispatchSerials.Add(e.Serial);
      //  }

      }
    }


    //---------------------------------------------------------------------------------------------

  //  private object SyncRoot = new object();
    private List<Serial> toDispatchSerials = new List<Serial>();

    //---------------------------------------------------------------------------------------------

    private bool refreshing = false;
    private static bool errorOccured = false;
    protected void RefreshForms()
    {
      if (refreshing)
        return;

      if (InvokeRequired)
      {
        BeginInvoke(new ThreadStart(RefreshForms));
        return;
      }

      refreshing = true;
      try
      {
        List<Serial> refresSerials = new List<Serial>();
        List<Serial> currentChars = new List<Serial>();
        WindowManager defaultManager = WindowManager.GetDefaultManager();

        //lock (this.SyncRoot)
        //{
          for (int i = this.toDispatchSerials.Count - 1; i >= 0; i--)
          {
            Serial s = this.toDispatchSerials[i];
            if (StatusWrapper.GetWrapperType(s) == this.WrapperType)
              currentChars.Add(s);


            UOCharacter ch = new UOCharacter(s);

            if (!ch.Exist || ch.Distance > 30)
              this.toDispatchSerials.RemoveAt(i);
          }
      //  }

        List<StatusForm> forms = WindowManager.GetDefaultManager().OwnedWindows.OfType<StatusForm>().Where(f => this.WrapperType == StatusWrapper.GetWrapperType(f.MobileId) && !currentChars.Contains(f.MobileId)).ToList();
        foreach (StatusForm sf in forms)
        {
          if (sf.Manual)
            currentChars.Insert(0, sf.MobileId);
          else
            currentChars.Add(sf.MobileId);
        }

        currentChars = SortChars(currentChars);

        List<Serial> dummy = new List<Serial>();
        dummy.AddRange(currentChars.ToArray());

        List<Serial> filteredSorted = new List<Serial>();
        filteredSorted.AddRange(currentChars.ToArray());
        filteredSorted = filteredSorted.Where(s => WindowManager.GetDefaultManager().OwnedWindows.OfType<StatusForm>().Where(f => f.MobileId == s && (f.Keep || f.MouseHovering || f.Manual)).ToArray().Length > 0).ToList();

        Dictionary<Serial, int> order = new Dictionary<Serial, int>();

        for (int i = filteredSorted.Count - 1; i >= 0; i--)
        {
          Serial s = filteredSorted[i];
          int priority = 0;

          StatusForm sf = WindowManager.GetDefaultManager().OwnedWindows.OfType<StatusForm>().Where(f => f.MobileId == s).FirstOrDefault();
          if (sf != null)
          {
            if (sf.MouseHovering)
              priority += 10000;

            if (sf.Keep)
              priority += 1000;

            if (sf.Manual)
              priority += 100;

            if (!order.ContainsKey(s))
              order.Add(s, priority);
          }
          else
            filteredSorted.RemoveAt(i);
        }

        filteredSorted = filteredSorted.OrderByDescending(s => order[s]).ToList();

        currentChars = new List<Serial>();
        currentChars.AddRange(filteredSorted.ToArray());

        foreach (Serial s in dummy)
        {
          if (!currentChars.Contains(s))
            currentChars.Add(s);
        }

        int addCounter = 0;
        for (int i = 0; i < currentChars.Count && addCounter < this.MaxBars /*Math.Min(currentChars.Count, this.MaxBars)*/; i++)
        {
          UOCharacter ch = new UOCharacter(currentChars[i]);
          StatusForm sf = WindowManager.GetDefaultManager().OwnedWindows.OfType<StatusForm>().Where(f => f.MobileId == ch.Serial).FirstOrDefault();

          if (sf != null || this.Auto)
          {
            bool add = (sf == null || !sf.DisableWrapper) && ch.Distance < 25 && ch.Exist || sf != null && (sf.Keep || sf.IsNew || sf.MouseHovering) && !sf.DisableWrapper;

            if (add)
            {
              addCounter++;
              refresSerials.Add(ch.Serial);

              if (sf == null)
              {
                defaultManager.CreateWindow(delegate ()
                {
                  var f = new StatusForm(ch.Serial);
                  f.Transparency = true;
                  return f;
                });
              }
            }
            else if (sf != null && !sf.DisableWrapper && !sf.Keep && !sf.MouseHovering)
            {
              if (defaultManager.InvokeRequired)
                defaultManager.BeginInvoke(sf.Close);
              else
                sf.Close();
            }
          }
        }

        if (this.Auto)//??
        {
          List<StatusForm> delete = WindowManager.GetDefaultManager().OwnedWindows.OfType<StatusForm>().Where(sf => sf.MobileId != World.Player.Serial && StatusWrapper.GetWrapperType(sf.MobileId) == this.WrapperType && !refresSerials.Contains(sf.MobileId) && !sf.DisableWrapper && !sf.Keep && !sf.MouseHovering).ToList();
          foreach (StatusForm sf in delete)
          {
            if (defaultManager.InvokeRequired)
              defaultManager.BeginInvoke(sf.Close);
            else
              sf.Close();
          }
        }

        int y = this.Location.Y + this.Height;
        int x = this.Location.X;

        Dictionary<int, Serial> slots = new Dictionary<int, Serial>();
        List<int> slotPointer = new List<int>();

        for (int i = 0; i < this.MaxBars; i++)
        {
          int slotY = y + (i * StatusForm.DefaultHeight);
          slotPointer.Add(slotY);
          slots.Add(slotY, (Serial)Serial.Invalid);
        }

        StatusForm disabledSlot = WindowManager.GetDefaultManager().OwnedWindows.OfType<StatusForm>().Where(f => refresSerials.Contains(f.MobileId) && f.MouseHovering).FirstOrDefault();

        if (disabledSlot != null && slots.ContainsKey(disabledSlot.Location.Y))
          slots[disabledSlot.Location.Y] = disabledSlot.MobileId;

       // refresSerials = SortChars(refresSerials);

        for (int i = 0; i < refresSerials.Count; i++)
        {
          Serial s = refresSerials[i];
          StatusForm sf = WindowManager.GetDefaultManager().OwnedWindows.OfType<StatusForm>().Where(f => f.MobileId == s).FirstOrDefault();

          if (sf != null)
          {
            for (int u = 0; u < slotPointer.Count; u++)
            {
              int slotY = slotPointer[u];
              Serial slotId = slots[slotY];

              if (!slotId.IsValid && slots.Values.Count(sv => sv == sf.MobileId) == 0 || slotId == sf.MobileId)
              {
                if (sf.Location.X != x || sf.Location.Y != slotY)
                {
                  sf.Location = new Point(x, slotPointer[u]);
                  sf.Invalidate();
                }

                slots[slotY] = sf.MobileId;
                break;
              }
            }
          }
        }

        this.registeredSerials = refresSerials;
      }
      catch (Exception e)
      {
        Game.PrintMessage("Chyba v zalozkach", MessageType.Error);

        if (!errorOccured)
        {
          Notepad.WriteLine("Chyba v zalozkach");
          Notepad.WriteLine("" + e.Message);
          Notepad.WriteLine();
          Notepad.WriteLine("" + e.StackTrace);
          Notepad.WriteLine("Inner: " + (e.InnerException != null ? e.InnerException.Message : ""));
          Notepad.WriteLine();
          Notepad.WriteLine((e.InnerException != null ? e.InnerException.StackTrace : ""));
          errorOccured = true;
        }
      }
      finally
      {
        refreshing = false;
      }
    }

    //---------------------------------------------------------------------------------------------

    protected List<Serial> SortChars(List<Serial> serials)
    {
      if (this.SortDir == StatusFormWrapperSortDirection.Asc)
      {
        if (this.SortType == StatusFormWrapperSortType.Time)
          return serials.OrderBy(s => WindowManager.GetDefaultManager().OwnedWindows.OfType<StatusForm>().Where(f => f.MobileId == s).FirstOrDefault() == null ? DateTime.Now : WindowManager.GetDefaultManager().OwnedWindows.OfType<StatusForm>().Where(f => f.MobileId == s).FirstOrDefault().InitTime).ToList();
        else if (this.SortType == StatusFormWrapperSortType.Hits)
          return serials.OrderBy(s => new UOCharacter(s).Hits).ToList();
        else if (this.SortType == StatusFormWrapperSortType.MaxHits)
          return serials.OrderBy(s => new UOCharacter(s).MaxHits).ToList();
        else if (this.SortType == StatusFormWrapperSortType.Name)
          return serials.OrderBy(s => new UOCharacter(s).Name).ToList();
        else if (this.SortType == StatusFormWrapperSortType.Model)
          return serials.OrderBy(s => new UOCharacter(s).Model + String.Empty).ToList();
        else if (this.SortType == StatusFormWrapperSortType.Damage)
          return serials.OrderBy(s => ((decimal)(new UOCharacter(s).MaxHits - new UOCharacter(s).Hits) / (decimal)new UOCharacter(s).MaxHits) * 100.0m).ToList();
        else 
          return serials.OrderBy(s => new UOCharacter(s).GetDistance()).ToList();
      }
      else
      {
        if (this.SortType == StatusFormWrapperSortType.Time)
          return serials.OrderByDescending(s => WindowManager.GetDefaultManager().OwnedWindows.OfType<StatusForm>().Where(f => f.MobileId == s).FirstOrDefault() == null ? DateTime.Now : WindowManager.GetDefaultManager().OwnedWindows.OfType<StatusForm>().Where(f => f.MobileId == s).FirstOrDefault().InitTime).ToList();
        else if (this.SortType == StatusFormWrapperSortType.Hits)
          return serials.OrderByDescending(s => new UOCharacter(s).Hits).ToList();
        else if (this.SortType == StatusFormWrapperSortType.MaxHits)
          return serials.OrderByDescending(s => new UOCharacter(s).MaxHits).ToList();
        else if (this.SortType == StatusFormWrapperSortType.Name)
          return serials.OrderByDescending(s => new UOCharacter(s).Name).ToList();
        else if (this.SortType == StatusFormWrapperSortType.Model)
          return serials.OrderByDescending(s => new UOCharacter(s).Model + String.Empty).ToList();
        else if (this.SortType == StatusFormWrapperSortType.Damage)
          return serials.OrderByDescending(s => ((decimal)(new UOCharacter(s).MaxHits - new UOCharacter(s).Hits) / (decimal)new UOCharacter(s).MaxHits) * 100.0m).ToList();
        else
          return serials.OrderByDescending(s => new UOCharacter(s).GetDistance()).ToList();
      }
    }

    //---------------------------------------------------------------------------------------------

    protected override void OnMove(EventArgs e)
    {
      base.OnMove(e);
      RefreshForms();
    }

    //---------------------------------------------------------------------------------------------

    protected Color? DefaultColor = null;
    public bool MouseHovering = false;

    void StatusForm_MouseEnter(object sender, EventArgs e)
    {
      MouseHovering = true;
      BackColor = Color.GreenYellow;

      this.Invalidate();
    }

    //---------------------------------------------------------------------------------------------

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      this.ClientSize = new Size(170, 24);

      this.Size = this.ClientSize;
      this.Width = 170;
      this.Height = 24;

      int maxY = Screen.PrimaryScreen.Bounds.Height;
      int maxX = Screen.PrimaryScreen.Bounds.Width - 170;

      int defaultX = 0;
      int defaultY = 0;

      if (this.WrapperType == StatusFormWrapperType.Enemy) { }
      else if (this.WrapperType == StatusFormWrapperType.Other)
        defaultX = maxX;//this.Location = new Point(maxX, 0);
      else if (this.WrapperType == StatusFormWrapperType.Mob)
      {
        defaultX = maxX;
        defaultY = maxY / 2;
      }

      int x = Config.Profile.UserSettings.GetAttribute(defaultX, "Value", "StatusFormWrapper_" + this.WrapperType + "_LocationX");
      int y = Config.Profile.UserSettings.GetAttribute(defaultY, "Value", "StatusFormWrapper_" + this.WrapperType + "_LocationY");

      this.Location = new Point(x, y);

      this.chbxAuto.Checked = this.auto;

      this.cbxMaxBars.SelectedIndex = this.posibleMaxs.ToList().IndexOf(this.maxBars);
      this.cbxSortDirection.SelectedIndex = Array.IndexOf(Enum.GetValues(typeof(StatusFormWrapperSortDirection)), this.sortDir);
      this.cbxSortType.SelectedIndex = Array.IndexOf(Enum.GetValues(typeof(StatusFormWrapperSortType)), this.sortType);

      initRun = false;
      this.Invalidate();
    }

    //---------------------------------------------------------------------------------------------

    void StatusForm_MouseLeave(object sender, EventArgs e)
    {
      if (this.DefaultColor.HasValue)
      {
        BackColor = this.DefaultColor.Value;
        this.Invalidate();
      }
      MouseHovering = false;

      Config.Profile.UserSettings.SetAttribute(this.Location.X, "Value", "StatusFormWrapper_" + this.WrapperType + "_LocationX");
      Config.Profile.UserSettings.SetAttribute(this.Location.Y, "Value", "StatusFormWrapper_" + this.WrapperType + "_LocationY");
    }

    //---------------------------------------------------------------------------------------------

    protected override void OnClosing(CancelEventArgs e)
    {
      //World.CharacterAppeared -= World_CharacterAppeared;
      //World.Player.Changed -= Player_Changed;
      StatusBar.OnStatusShow -= StatusBar_OnStatusShow;
      if (this.t != null)
      {
        this.t.Abort();
        this.t = null;
      }
      base.OnClosing(e);
    }

    //---------------------------------------------------------------------------------------------

    protected override void Dispose(bool disposing)
    {
      //World.CharacterAppeared -= World_CharacterAppeared;
      //World.Player.Changed -= Player_Changed;
      StatusBar.OnStatusShow -= StatusBar_OnStatusShow;
      if (this.t != null)
      {
        this.t.Abort();
        this.t = null;
      }

      base.Dispose(disposing);
    }

    //---------------------------------------------------------------------------------------------

    #region WinForms

    CheckBox chbxAuto;
    Label name;
    // TextBox tbxMaxBars;
    ComboBox cbxMaxBars = new ComboBox();
    ComboBox cbxSortType = new ComboBox();
    ComboBox cbxSortDirection = new ComboBox();

    //---------------------------------------------------------------------------------------------
    private bool initRun = false;
    private int[]  posibleMaxs = new int[] { 12, 6, 18, 24 };
    private void InitializeComponent()
    {
      initRun = true;
      this.SuspendLayout();

      Font font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.name = new Label();
      this.name.AutoSize = true;
      this.name.BackColor = System.Drawing.Color.Transparent;
      this.name.Enabled = false;
      this.name.Font = font;
      this.name.Location = new System.Drawing.Point(2, 2);
      this.name.Name = "name";
      this.name.TabIndex = 0;
      this.name.Text = "" + this.WrapperType.ToString()[0].ToString();
      this.Controls.Add(this.name);

      this.cbxSortType = new ComboBox();
      this.cbxSortType.DataSource = Enum.GetValues(typeof(StatusFormWrapperSortType));//Enum.GetNames(typeof(StatusFormWrapperSortType));
      this.cbxSortType.Name = "cbxSortType";
      this.cbxSortType.Location = new System.Drawing.Point(30, 2);
      this.cbxSortType.Size = new System.Drawing.Size(50, 10);
      this.cbxSortType.Enabled = true;
      this.Controls.Add(this.cbxSortType);


      this.cbxSortDirection = new ComboBox();
      this.cbxSortDirection.DataSource = Enum.GetValues(typeof(StatusFormWrapperSortDirection));//Enum.GetNames(typeof(StatusFormWrapperSortDirection));
      this.cbxSortDirection.Name = "cbxSortDirection";
      this.cbxSortDirection.Location = new System.Drawing.Point(82, 2);
      this.cbxSortDirection.Size = new System.Drawing.Size(36, 10);
      this.cbxSortDirection.Enabled = true;
      this.Controls.Add(this.cbxSortDirection);


      this.cbxMaxBars = new ComboBox();
      this.cbxMaxBars.DataSource = posibleMaxs;
      this.cbxMaxBars.Name = "cbxMaxBars";
      this.cbxMaxBars.Location = new System.Drawing.Point(118, 2);
      this.cbxMaxBars.Size = new System.Drawing.Size(36, 10);
      this.cbxMaxBars.Enabled = true;
      this.Controls.Add(this.cbxMaxBars);



      this.chbxAuto = new CheckBox();
      this.chbxAuto.Name = "chbxAuto";
      this.chbxAuto.Location = new System.Drawing.Point(156, 4);
      this.chbxAuto.Size = new System.Drawing.Size(12, 12);
      this.chbxAuto.TabIndex = 1;
      this.chbxAuto.BackColor = Color.Transparent;
      this.Controls.Add(this.chbxAuto);


      this.ClientSize = new Size(190, 20);


      this.maxBars = Config.Profile.UserSettings.GetAttribute(12, "Value", "StatusFormWrapper_" + this.WrapperType + "_MaxBars");
      this.auto = Config.Profile.UserSettings.GetAttribute(this.WrapperType == StatusFormWrapperType.Enemy ? true : false, "Value", "StatusFormWrapper_" + this.WrapperType + "_Auto");
      this.sortType = (StatusFormWrapperSortType)Config.Profile.UserSettings.GetAttribute((int)StatusFormWrapperSortType.Distance, "Value", "StatusFormWrapper_" + this.WrapperType + "_SortType");
      this.sortDir = (StatusFormWrapperSortDirection)Config.Profile.UserSettings.GetAttribute((int)StatusFormWrapperSortDirection.Asc, "Value", "StatusFormWrapper_" + this.WrapperType + "_SortDir");

      this.cbxSortType.SelectedValueChanged += CbxSortType_SelectedValueChanged;
      this.cbxSortDirection.SelectedValueChanged += CbxSortDirection_SelectedValueChanged;
      this.cbxMaxBars.SelectedValueChanged += CbxMaxBars_SelectedValueChanged; 
      this.chbxAuto.CheckedChanged += ChbxAuto_CheckedChanged;

      this.DoubleBuffered = true;
      this.Name = "StatusFormWrapper" + this.WrapperType;
      this.ResumeLayout(false);
      this.PerformLayout();
      //this.Invalidate();
      //this.Refresh();

    }

    //---------------------------------------------------------------------------------------------

    private void CbxMaxBars_SelectedValueChanged(object sender, EventArgs e)
    {
      if (!initRun)
      {
        int max = 12;
        if (!Int32.TryParse(this.cbxMaxBars.SelectedValue + String.Empty, out max))
          max = 12;
        this.MaxBars = max;
      }
    }

    //---------------------------------------------------------------------------------------------

    private void CbxSortType_SelectedValueChanged(object sender, EventArgs e)
    {
      if (!initRun)
      {
        this.SortType = (StatusFormWrapperSortType)this.cbxSortType.SelectedItem;//(StatusFormWrapperSortType)Enum.Parse(typeof(StatusFormWrapperSortType), this.cbxSortType.SelectedValue + String.Empty);
      }
    }

    //---------------------------------------------------------------------------------------------

    private void CbxSortDirection_SelectedValueChanged(object sender, EventArgs e)
    {
      if (!this.initRun)
      {
        this.SortDir = (StatusFormWrapperSortDirection)this.cbxSortDirection.SelectedItem;//(StatusFormWrapperSortDirection)Enum.Parse(typeof(StatusFormWrapperSortDirection), this.cbxSortDirection.SelectedValue + String.Empty);
      }
    }

    //---------------------------------------------------------------------------------------------

    private void ChbxAuto_CheckedChanged(object sender, EventArgs e)
    {
      this.Auto = this.chbxAuto.Checked;
      if (this.Auto)
      {
        this.GetAllCharacters();
        this.RefreshForms();
      }
    }

    //---------------------------------------------------------------------------------------------

    #endregion

    //---------------------------------------------------------------------------------------------
  }
}
