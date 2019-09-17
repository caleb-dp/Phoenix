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

namespace CalExtension.UI.Status
{
  public class UtilityForm : InGameWindow
  {
    public static UtilityForm Current;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:SuppliesForm"/> class.
    /// </summary>
    /// 


    public UtilityForm()
    {
      if (Current == null)
        Current = this;

      MouseEnter += StatusForm_MouseEnter;
      MouseLeave += StatusForm_MouseLeave;
      this.InitializeComponent();

      if (t == null)
      {
        t = new Thread(new ThreadStart(Main));
        t.Start();
      }

      if (tWatch == null)
      {
        tWatch = new Thread(new ThreadStart(MainWatch));
        tWatch.Start();

      }
    }

    //---------------------------------------------------------------------------------------------
    Thread tWatch;
    Thread t;
    private void Main()
    {
      while (true)
      {
        Thread.Sleep(500);
        this.RefreshMixState();
      }
    }

    private void MainWatch()
    {
      while (true)
      {
        Thread.Sleep(1000);
        this.RefresWsWatch();
      }
    }

    //---------------------------------------------------------------------------------------------

    bool ctrlHold = false;
    protected void RefreshMixState()
    {
      if (InvokeRequired)
      {
        BeginInvoke(new ThreadStart(RefreshMixState));
        return;
      }

      bool currCtrlHold = (ModifierKeys & Keys.Control) == Keys.Control;
      if (currCtrlHold != ctrlHold)
      {
        ctrlHold = currCtrlHold;

        if (ctrlHold)
        {
          this.btnMixurePotion.BackColor = Color.Firebrick;
          this.btnMixurePotion.Text = "MIX";
        }
        else
        {
          this.btnMixurePotion.BackColor = Color.DodgerBlue;
          this.btnMixurePotion.Text = "DRINK";
        }
        this.btnMixurePotion.Invalidate();
      }
    }


    //---------------------------------------------------------------------------------------------

    private Potion potion = Potion.Cure;
    public Potion Potion
    {
      get { return this.potion; }
      set
      {
        this.potion = value;
        Config.Profile.UserSettings.SetAttribute(this.potion.Name, "Value", "UtilityForm_Potion");
      }
    }

    //---------------------------------------------------------------------------------------------

    private PotionQuality potionQuality = PotionQuality.Lesser;
    public PotionQuality PotionQuality
    {
      get { return this.potionQuality; }
      set
      {
        this.potionQuality = value;
        Config.Profile.UserSettings.SetAttribute((int)this.potionQuality, "Value", "UtilityForm_PotionQuality");
      }
    }

    //---------------------------------------------------------------------------------------------

    protected override void OnMove(EventArgs e)
    {
      base.OnMove(e);
    }

    //---------------------------------------------------------------------------------------------

    protected Color? DefaultColor = Color.Black;
    public bool MouseHovering = false;

    void StatusForm_MouseEnter(object sender, EventArgs e)
    {
      MouseHovering = true;
      //BackColor = Color.FromArgb(64, 64, 64);
      //Color.GreenYellow;

      //this.Invalidate();
    }

    //---------------------------------------------------------------------------------------------

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);


      int defaultX = 0;
      int defaultY = 0;


      int x = Config.Profile.UserSettings.GetAttribute(defaultX, "Value", "UtilityForm_LocationX");
      int y = Config.Profile.UserSettings.GetAttribute(defaultY, "Value", "UtilityForm_LocationY");

      this.BackColor = DefaultColor.Value;
      this.Location = new Point(x, y);

      //this.chbxAuto.Checked = this.auto;
      int index = 0;
      for (int i = 0; i < PotionCollection.Potions.Count; i++)
      {
        if (PotionCollection.Potions[i].Name == this.potion.Name)
        {
          index = i;
          break;
        }
      }

      this.cbxPotions.SelectedIndex = index; 
      this.cbxPotionQualities.SelectedIndex = Array.IndexOf(Enum.GetValues(typeof(PotionQuality)), this.potionQuality);
      this.RefresWsWatch();
      this.RegisterCounters();
      this.Invalidate();
      initRun = false;
    }

    //---------------------------------------------------------------------------------------------

    void StatusForm_MouseLeave(object sender, EventArgs e)
    {
      if (this.DefaultColor.HasValue)
      {
        //BackColor = this.DefaultColor.Value;
        //this.Invalidate();
      }
      MouseHovering = false;

      Config.Profile.UserSettings.SetAttribute(this.Location.X, "Value", "UtilityForm_LocationX");
      Config.Profile.UserSettings.SetAttribute(this.Location.Y, "Value", "UtilityForm_LocationY");
    }

    //---------------------------------------------------------------------------------------------

    protected void RegisterCounters()
    {
      SupplyCounter lcCounter = new SupplyCounter(World.Player.Backpack, Potion.Cure.DefaultGraphic, Potion.Cure.Qualities[PotionQuality.Lesser].Color);
      lcCounter.AmountChanged += LcCounter_AmountChanged;
      lcCounter.Recalc();

      SupplyCounter gcCounter = new SupplyCounter(World.Player.Backpack, Potion.Cure.DefaultGraphic, Potion.Cure.Qualities[PotionQuality.Greater].Color);
      gcCounter.AmountChanged += GcCounter_AmountChanged;
      gcCounter.Recalc();

      SupplyCounter gsCounter = new SupplyCounter(World.Player.Backpack, Potion.Strength.DefaultGraphic, Potion.Strength.Qualities[PotionQuality.Greater].Color);
      gsCounter.AmountChanged += GsCounter_AmountChanged;
      gsCounter.Recalc();

      SupplyCounter trCounter = new SupplyCounter(World.Player.Backpack, Potion.Refresh.DefaultGraphic, Potion.Refresh.Qualities[PotionQuality.Total].Color);
      trCounter.AmountChanged += TrCounter_AmountChanged;
      trCounter.Recalc();

      SupplyCounter ghCounter = new SupplyCounter(World.Player.Backpack, Potion.Heal.DefaultGraphic, Potion.Heal.Qualities[PotionQuality.Greater].Color);
      ghCounter.AmountChanged += GhCounter_AmountChanged;
      ghCounter.Recalc();

      SupplyCounter mrCounter = new SupplyCounter(World.Player.Backpack, Potion.ManaRefresh.DefaultGraphic, Potion.ManaRefresh.Qualities[PotionQuality.None].Color);
      mrCounter.AmountChanged += MrCounter_AmountChanged;
      mrCounter.Recalc();


      SupplyCounter tmrCounter = new SupplyCounter(World.Player.Backpack, Potion.TotalManaRefresh.DefaultGraphic, Potion.TotalManaRefresh.Qualities[PotionQuality.None].Color);
      tmrCounter.AmountChanged += TmrCounter_AmountChanged;
      tmrCounter.Recalc();

      SupplyCounter invCounter = new SupplyCounter(World.Player.Backpack, Potion.Invisibility.DefaultGraphic, Potion.Invisibility.Qualities[PotionQuality.None].Color);
      invCounter.AmountChanged += InvCounter_AmountChanged;
      invCounter.Recalc();
    }

    private void InvCounter_AmountChanged(object sender, EventArgs e)
    {
      this.btnMixureINV.Text = "INV (" + ((SupplyCounter)sender).CurrentAmount + ")";
      this.btnMixureINV.Invalidate();
    }

    private void TmrCounter_AmountChanged(object sender, EventArgs e)
    {
      this.btnMixureTMR.Text = "TMR (" + ((SupplyCounter)sender).CurrentAmount + ")";
      this.btnMixureTMR.Invalidate();
    }

    private void MrCounter_AmountChanged(object sender, EventArgs e)
    {
      this.btnMixureMR.Text = "MR (" + ((SupplyCounter)sender).CurrentAmount + ")";
      this.btnMixureMR.Invalidate();
    }

    private void GhCounter_AmountChanged(object sender, EventArgs e)
    {
      this.btnMixureGH.Text = "GH (" + ((SupplyCounter)sender).CurrentAmount + ")";
      this.btnMixureGH.Invalidate();
    }

    private void TrCounter_AmountChanged(object sender, EventArgs e)
    {
      this.btnMixureTR.Text = "TR (" + ((SupplyCounter)sender).CurrentAmount + ")";
      this.btnMixureTR.Invalidate();
    }

    private void GsCounter_AmountChanged(object sender, EventArgs e)
    {
      this.btnMixureGS.Text = "GS (" + ((SupplyCounter)sender).CurrentAmount + ")";
      this.btnMixureGS.Invalidate();
    }

    private void GcCounter_AmountChanged(object sender, EventArgs e)
    {
      this.btnMixureGC.Text = "GC (" + ((SupplyCounter)sender).CurrentAmount + ")";
      this.btnMixureGC.Invalidate();
    }

    private void LcCounter_AmountChanged(object sender, EventArgs e)
    {
      this.btnMixureLC.Text = "LC (" + ((SupplyCounter)sender).CurrentAmount + ")";
      this.btnMixureLC.Invalidate();
    }

    //---------------------------------------------------------------------------------------------

    //protected void RefresCounters()
    //{
    //  //int newlcPotionCount = World.Player.Backpack.AllItems.Count(Potion.Cure.DefaultGraphic, Potion.Cure.Qualities[PotionQuality.Lesser].Color);
    //  //int newgcPotionCount = World.Player.Backpack.AllItems.Count(Potion.Cure.DefaultGraphic, Potion.Cure.Qualities[PotionQuality.Greater].Color);
    //  //int newgsPotionCount = World.Player.Backpack.AllItems.Count(Potion.Strength.DefaultGraphic, Potion.Strength.Qualities[PotionQuality.Greater].Color);
    //  //int newtrPotionCount = World.Player.Backpack.AllItems.Count(Potion.Refresh.DefaultGraphic, Potion.Refresh.Qualities[PotionQuality.Total].Color);
    //  //int newghPotionCount = World.Player.Backpack.AllItems.Count(Potion.Heal.DefaultGraphic, Potion.Heal.Qualities[PotionQuality.Greater].Color);
    //  //int newmrPotionCount = World.Player.Backpack.AllItems.Count(Potion.ManaRefresh.DefaultGraphic, Potion.ManaRefresh.Qualities[PotionQuality.None].Color);
    //  //int newtmrPotionCount = World.Player.Backpack.AllItems.Count(Potion.TotalManaRefresh.DefaultGraphic, Potion.TotalManaRefresh.Qualities[PotionQuality.None].Color);
    //  //int newinvPotionCount = World.Player.Backpack.AllItems.Count(Potion.Invisibility.DefaultGraphic, Potion.Invisibility.Qualities[PotionQuality.None].Color);

    //  //bool needRefresh =
    //  //  lcPotionCount != newlcPotionCount ||
    //  //  gcPotionCount != newgcPotionCount ||
    //  //  gsPotionCount != newgsPotionCount ||
    //  //  trPotionCount != newtrPotionCount ||
    //  //  ghPotionCount != newghPotionCount ||
    //  //  mrPotionCount != newmrPotionCount ||
    //  //  tmrPotionCount != newtmrPotionCount ||
    //  //  invPotionCount != newinvPotionCount;


    //  //if (needRefresh)
    //  //{
    //  //  if (lcPotionCount != newlcPotionCount)
    //  //  {
    //  //    this.btnMixureLC.Text = "LC (" + newlcPotionCount + ")";
    //  //    this.btnMixureLC.Invalidate();
    //  //  }

    //  //  if (gcPotionCount != newgcPotionCount)
    //  //  {
    //  //    this.btnMixureGC.Text = "GC (" + newgcPotionCount + ")";
    //  //    this.btnMixureGC.Invalidate();
    //  //  }

    //  //  if (gsPotionCount != newgsPotionCount)
    //  //  {
    //  //    this.btnMixureGS.Text = "GS (" + newgsPotionCount + ")";
    //  //    this.btnMixureGS.Invalidate();
    //  //  }

    //  //  if (trPotionCount != newtrPotionCount)
    //  //  {
    //  //    this.btnMixureTR.Text = "TR (" + newtrPotionCount + ")";
    //  //    this.btnMixureTR.Invalidate();
    //  //  }

    //  //  if (ghPotionCount != newghPotionCount)
    //  //  {
    //  //    this.btnMixureGH.Text = "GH (" + newghPotionCount + ")";
    //  //    this.btnMixureGH.Invalidate();
    //  //  }

    //  //  if (mrPotionCount != newmrPotionCount)
    //  //  {
    //  //    this.btnMixureMR.Text = "MR (" + newmrPotionCount + ")";
    //  //    this.btnMixureMR.Invalidate();
    //  //  }

    //  //  if (tmrPotionCount != newtmrPotionCount)
    //  //  {
    //  //    this.btnMixureTMR.Text = "TMR (" + newtmrPotionCount + ")";
    //  //    this.btnMixureTMR.Invalidate();
    //  //  }


    //  //  if (invPotionCount != newinvPotionCount)
    //  //  {
    //  //    this.btnMixureINV.Text = "INV (" + newinvPotionCount + ")";
    //  //    this.btnMixureINV.Invalidate();
    //  //  }

    //  //  this.Invalidate();
    //  //}

    //  //lcPotionCount = newlcPotionCount;
    //  //gcPotionCount = newgcPotionCount;
    //  //gsPotionCount = newgsPotionCount;
    //  //trPotionCount = newtrPotionCount;
    //  //ghPotionCount = newghPotionCount;
    //  //mrPotionCount = newmrPotionCount;
    //  //tmrPotionCount = newtmrPotionCount;
    //  //invPotionCount = newinvPotionCount;
    //}

    //---------------------------------------------------------------------------------------------

    protected void RefresWsWatch()
    {
      if (InvokeRequired)
      {
        this.BeginInvoke(new ThreadStart(RefresWsWatch));
        return;
      }
      DateTime? lastWsTime = Game.CurrentGame.LastWorldSaveTime;

      if (lastWsTime.HasValue)
      {
        DateTime now = DateTime.Now;
        WebWorldSaveInfo wsInfo = WebWorldSaveTime.GetInfo(lastWsTime.Value, 0);

        this.wsInfo.Text = String.Format("WS >> {0} ({1:HH:mm:ss})", wsInfo.NextTimeStr, wsInfo.NextTime);
        this.wsInfo.Invalidate();

        //double nextWsMinutes = Math.Abs(wsInfo.NextTimeSpan.TotalMinutes);
      }
    }

    //---------------------------------------------------------------------------------------------

    protected override void OnClosing(CancelEventArgs e)
    {


      if (this.t != null)
      {
        this.t.Abort();
        this.t = null;
      }

      if (this.tWatch != null)
      {
        this.tWatch.Abort();
        this.tWatch = null;
      }
      if (Current == this)
        Current = null;
      base.OnClosing(e);
    }

    //---------------------------------------------------------------------------------------------

    protected override void Dispose(bool disposing)
    {

      if (this.t != null)
      {
        this.t.Abort();
        this.t = null;
      }

      if (this.tWatch != null)
      {
        this.tWatch.Abort();
        this.tWatch = null;
      }
      if (Current == this)
        Current = null;

      base.Dispose(disposing);
    }

    //---------------------------------------------------------------------------------------------

    #region WinForms

    Label name;

    Button btnOpen;
    Button btnOpenSlotForm;

    ComboBox cbxPotions; 
    ComboBox cbxPotionQualities;
    Button btnMixurePotion;


    Button btnMixureLC;
    Button btnMixureGC;
    Button btnMixureGS;
    Button btnMixureTR;
    Button btnMixureGH;
    Button btnMixureMR;
    Button btnMixureTMR;
    Button btnMixureINV;

    Button btnStatRepair;
    Button btnSortBackpack;
    Button btnBracNeck;

    Button btnNbRuna;
    Button btnNbCech;
    Button btnPrintItems;

    Button btnSkillTrackAnimal;
    Button btnSkillTrackPk;
    Button btnSkillTrackDetect;
    Button btnSkillTrackForensic;
    Button btnSkillTrackItemId;
    Button btnInfo;

    Button btnL500;
    Button btnL2500;
    Button btnTAll;

    Button btnLatency;
    Button btnHide;
    Button btnResync;

    Button btnRunInvis1;
    Button btnVyhodKlamakNa;
    Button btnSnap;

    Button btnTLeft;
    Button btnForward;
    Button btnTRight;
    Button btnLeft;
    Button btnStop;
    Button btnRight;
    Button btnDock;
    Button btnBackward;
    Button btnLockShip;

    Button btnMoveRegyAll;
    Button btnNajdiaPresun;
    Button btnSortitemName;

    Button btnNajdiAVloz;
    Button btnNaplnSperky;
    Button btnNaplABoxy;

    Button btnMoveType;
    Button btnSortType;
    Button btnUnlock;

    Button btnBuy;
    Button btnSell;
    Button btnCorpseQuest;

    Button btnMRegy200;
    Button btnMRegy1000;
    Button btnNRegy50;

    Button btnTame;
    Button btnWrongKlic;
    Button btnBankBuy;

    Button btnOpenInventory;
    Button btnKokon;
    Button btnTreasureSearch;

    Label wsInfo;

    private int lcPotionCount = 0;
    private int gcPotionCount = 0;
    private int gsPotionCount = 0;
    private int trPotionCount = 0;
    private int ghPotionCount = 0;
    private int mrPotionCount = 0;
    private int tmrPotionCount = 0;
    private int invPotionCount = 0;


    //---------------------------------------------------------------------------------------------
    private bool initRun = true;
    private void InitializeComponent()
    {
      initRun = true;
      this.SuspendLayout();

      int alchemyValue = SkillsHelper.GetSkillValue("Alchemy").Value;

      Font font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      Font fontSmall = new System.Drawing.Font("Arial", 6.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

      int defaultPadding = 2;
      int maxX = 0;
      int maxY = 0;
      int currentLine = 0 + defaultPadding;
      int currentPosition = 0 + defaultPadding;

      int buttonWidth = 48;
      int buttonHeight = 20;
      int buttonMiddleWidth = 64;

      this.name = new Label();
      this.name.AutoSize = true;
      this.name.BackColor = System.Drawing.Color.Transparent;
      this.name.Enabled = false;
      this.name.Font = font;
      this.name.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.name.Name = "name";
      this.name.TabIndex = 0;
      this.name.Text = "Utility Form";
      this.Controls.Add(this.name);

      #region leftPanel

      currentLine = 24;

      this.cbxPotions = new ComboBox();
      this.cbxPotions.DisplayMember = "Name";
      this.cbxPotions.DataSource = PotionCollection.Potions;
      this.cbxPotions.Name = "cbxPotions";
      this.cbxPotions.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.cbxPotions.Size = new System.Drawing.Size(100, 10);
      this.cbxPotions.Enabled = true;
      this.Controls.Add(this.cbxPotions);

      Control prevCont = this.cbxPotions;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.cbxPotionQualities = new ComboBox();
      this.cbxPotionQualities.DataSource = Enum.GetValues(typeof(PotionQuality));//Enum.GetNames(typeof(StatusFormWrapperSortType));
      this.cbxPotionQualities.Name = "cbxPotionQualities";
      this.cbxPotionQualities.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.cbxPotionQualities.Size = new System.Drawing.Size(50, 10);
      this.cbxPotionQualities.Enabled = true;
      this.Controls.Add(this.cbxPotionQualities);

      prevCont = this.cbxPotionQualities;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnMixurePotion = new Button();
      this.btnMixurePotion.Name = "btnMixurePotion";
      this.btnMixurePotion.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnMixurePotion.Enabled = true;
      this.btnMixurePotion.Text = "DRINK";
      this.btnMixurePotion.Font = fontSmall;
      this.btnMixurePotion.Size = new Size(45, buttonHeight);
      // this.btnMixurePotion.AutoSize = true;
      this.btnMixurePotion.BackColor = Color.DodgerBlue;
      this.btnMixurePotion.ForeColor = Color.White;
      this.btnMixurePotion.Padding = new Padding(0);
      this.btnMixurePotion.TabStop = false;
      this.btnMixurePotion.FlatStyle = FlatStyle.Flat;
      this.btnMixurePotion.FlatAppearance.BorderSize = 0;

      this.btnMixurePotion.MouseClick += BtnMixurePotion_Click;
      this.Controls.Add(this.btnMixurePotion);

      prevCont = this.btnMixurePotion;

      #region QuickAlchemyButtons



      currentLine = prevCont.Location.Y + prevCont.Size.Height + defaultPadding;
      currentPosition = defaultPadding;

      this.btnMixureLC = new Button();
      this.btnMixureLC.Name = "btnMixureLC";
      this.btnMixureLC.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnMixureLC.Enabled = true;
      this.btnMixureLC.Text = "LC (" + lcPotionCount + ")";
      this.btnMixureLC.Font = fontSmall;
      this.btnMixureLC.Size = new Size(buttonWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnMixureLC.BackColor = Color.Orange;
      this.btnMixureLC.ForeColor = Color.White;
      this.btnMixureLC.Padding = new Padding(0);
      this.btnMixureLC.TabStop = false;
      this.btnMixureLC.FlatStyle = FlatStyle.Flat;
      this.btnMixureLC.FlatAppearance.BorderSize = 0;

      this.btnMixureLC.MouseClick += BtnMixureLC_Click;//.Click += BtnMixureLC_Click;
      this.Controls.Add(this.btnMixureLC);

      prevCont = this.btnMixureLC;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnMixureGC = new Button();
      this.btnMixureGC.Name = "btnMixureGC";
      this.btnMixureGC.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnMixureGC.Enabled = true;
      this.btnMixureGC.Text = "GC (" + gcPotionCount + ")";
      this.btnMixureGC.Font = fontSmall;
      this.btnMixureGC.Size = new Size(buttonWidth, buttonHeight);

      //this.btnMixureLC.AutoSize = true;
      this.btnMixureGC.BackColor = Color.OrangeRed;
      this.btnMixureGC.ForeColor = Color.White;
      this.btnMixureGC.Padding = new Padding(0);
      this.btnMixureGC.TabStop = false;
      this.btnMixureGC.FlatStyle = FlatStyle.Flat;
      this.btnMixureGC.FlatAppearance.BorderSize = 0;


      this.btnMixureGC.MouseClick += BtnMixureGC_Click;
      this.Controls.Add(this.btnMixureGC);

      prevCont = this.btnMixureGC;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnMixureGS = new Button();
      this.btnMixureGS.Name = "btnMixureGS";
      this.btnMixureGS.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnMixureGS.Enabled = true;
      this.btnMixureGS.Text = "GS (" + gsPotionCount + ")";
      this.btnMixureGS.Font = fontSmall;
      this.btnMixureGS.Size = new Size(buttonWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnMixureGS.BackColor = Color.GhostWhite;
      this.btnMixureGS.ForeColor = Color.Black;
      this.btnMixureGS.Padding = new Padding(0);
      this.btnMixureGS.TabStop = false;
      this.btnMixureGS.FlatStyle = FlatStyle.Flat;
      this.btnMixureGS.FlatAppearance.BorderSize = 0;

      this.btnMixureGS.MouseClick += BtnMixureGS_Click;
      this.Controls.Add(this.btnMixureGS);

      prevCont = this.btnMixureGS;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnMixureTR = new Button();
      this.btnMixureTR.Name = "btnMixureTR";
      this.btnMixureTR.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnMixureTR.Enabled = true;
      this.btnMixureTR.Text = "TR (" + trPotionCount + ")";
      this.btnMixureTR.Font = fontSmall;
      this.btnMixureTR.Size = new Size(buttonWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnMixureTR.BackColor = Color.Crimson;
      this.btnMixureTR.ForeColor = Color.White;
      this.btnMixureTR.Padding = new Padding(0);
      this.btnMixureTR.TabStop = false;
      this.btnMixureTR.FlatStyle = FlatStyle.Flat;
      this.btnMixureTR.FlatAppearance.BorderSize = 0;

      this.btnMixureTR.MouseClick += BtnMixureTR_Click;
      this.Controls.Add(this.btnMixureTR);

      prevCont = this.btnMixureTR;
      currentPosition = defaultPadding;
      currentLine = prevCont.Location.Y + prevCont.Size.Height + defaultPadding;

      this.btnMixureGH = new Button();
      this.btnMixureGH.Name = "btnMixureGH";
      this.btnMixureGH.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnMixureGH.Enabled = true;
      this.btnMixureGH.Text = "GH (" + ghPotionCount + ")";
      this.btnMixureGH.Font = fontSmall;
      this.btnMixureGH.Size = new Size(buttonWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnMixureGH.BackColor = Color.Gold;
      this.btnMixureGH.ForeColor = Color.Black;
      this.btnMixureGH.Padding = new Padding(0);
      this.btnMixureGH.TabStop = false;
      this.btnMixureGH.FlatStyle = FlatStyle.Flat;
      this.btnMixureGH.FlatAppearance.BorderSize = 0;

      this.btnMixureGH.MouseClick += BtnMixureGH_Click;
      this.Controls.Add(this.btnMixureGH);

      prevCont = this.btnMixureGH;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnMixureMR = new Button();
      this.btnMixureMR.Name = "btnMixureMR";
      this.btnMixureMR.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnMixureMR.Enabled = true;
      this.btnMixureMR.Text = "MR (" + mrPotionCount + ")";
      this.btnMixureMR.Font = fontSmall;
      this.btnMixureMR.Size = new Size(buttonWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnMixureMR.BackColor = Color.CornflowerBlue;
      this.btnMixureMR.ForeColor = Color.White;
      this.btnMixureMR.Padding = new Padding(0);
      this.btnMixureMR.TabStop = false;
      this.btnMixureMR.FlatStyle = FlatStyle.Flat;
      this.btnMixureMR.FlatAppearance.BorderSize = 0;

      this.btnMixureMR.MouseClick += BtnMixureMR_Click;
      this.Controls.Add(this.btnMixureMR);

      prevCont = this.btnMixureMR;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnMixureTMR = new Button();
      this.btnMixureTMR.Name = "btnMixureTMR";
      this.btnMixureTMR.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnMixureTMR.Enabled = true;
      this.btnMixureTMR.Text = "TMR (" + tmrPotionCount + ")";
      this.btnMixureTMR.Font = fontSmall;
      this.btnMixureTMR.Size = new Size(buttonWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnMixureTMR.BackColor = Color.Blue;
      this.btnMixureTMR.ForeColor = Color.White;
      this.btnMixureTMR.Padding = new Padding(0);
      this.btnMixureTMR.TabStop = false;
      this.btnMixureTMR.FlatStyle = FlatStyle.Flat;
      this.btnMixureTMR.FlatAppearance.BorderSize = 0;

      this.btnMixureTMR.MouseClick += BtnMixureTMR_Click;
      this.Controls.Add(this.btnMixureTMR);

      prevCont = this.btnMixureTMR;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;


      this.btnMixureINV = new Button();
      this.btnMixureINV.Name = "btnMixureINV";
      this.btnMixureINV.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnMixureINV.Enabled = true;
      this.btnMixureINV.Text = "INV (" + invPotionCount + ")";
      this.btnMixureINV.Font = fontSmall;
      this.btnMixureINV.Size = new Size(buttonWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnMixureINV.BackColor = Color.AliceBlue;
      this.btnMixureINV.ForeColor = Color.Black;
      this.btnMixureINV.Padding = new Padding(0);
      this.btnMixureINV.TabStop = false;
      this.btnMixureINV.FlatStyle = FlatStyle.Flat;
      this.btnMixureINV.FlatAppearance.BorderSize = 0;

      this.btnMixureINV.MouseClick += BtnMixureINV_Click;
      this.Controls.Add(this.btnMixureINV);

      prevCont = this.btnMixureINV;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      #endregion

      #region UtilButtons

      currentPosition = defaultPadding;
      currentLine = prevCont.Location.Y + prevCont.Size.Height + defaultPadding;

      this.btnStatRepair = new Button();
      this.btnStatRepair.Name = "btnStatRepair";
      this.btnStatRepair.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnStatRepair.Enabled = true;
      this.btnStatRepair.Text = "Staty";
      this.btnStatRepair.Font = fontSmall;
      this.btnStatRepair.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnStatRepair.BackColor = Color.LightSlateGray;
      this.btnStatRepair.ForeColor = Color.Black;
      this.btnStatRepair.Padding = new Padding(0);
      this.btnStatRepair.TabStop = false;
      this.btnStatRepair.FlatStyle = FlatStyle.Flat;
      this.btnStatRepair.FlatAppearance.BorderSize = 0;

      this.btnStatRepair.MouseClick += BtnStatRepair_Click;
      this.Controls.Add(this.btnStatRepair);

      prevCont = this.btnStatRepair;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnSortBackpack = new Button();
      this.btnSortBackpack.Name = "btnSortBackpack";
      this.btnSortBackpack.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnSortBackpack.Enabled = true;
      this.btnSortBackpack.Text = "Sortbackpack";
      this.btnSortBackpack.Font = fontSmall;
      this.btnSortBackpack.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnSortBackpack.BackColor = Color.LightSlateGray;
      this.btnSortBackpack.ForeColor = Color.Black;
      this.btnSortBackpack.Padding = new Padding(0);
      this.btnSortBackpack.TabStop = false;
      this.btnSortBackpack.FlatStyle = FlatStyle.Flat;
      this.btnSortBackpack.FlatAppearance.BorderSize = 0;

      this.btnSortBackpack.MouseClick += BtnSortBackpack_MouseClick; ;
      this.Controls.Add(this.btnSortBackpack);

      prevCont = this.btnSortBackpack;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnBracNeck = new Button();
      this.btnBracNeck.Name = "btnBracNeck";
      this.btnBracNeck.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnBracNeck.Enabled = true;
      this.btnBracNeck.Text = "Bracneck";
      this.btnBracNeck.Font = fontSmall;
      this.btnBracNeck.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnBracNeck.BackColor = Color.LightSlateGray;
      this.btnBracNeck.ForeColor = Color.Black;
      this.btnBracNeck.Padding = new Padding(0);
      this.btnBracNeck.TabStop = false;
      this.btnBracNeck.FlatStyle = FlatStyle.Flat;
      this.btnBracNeck.FlatAppearance.BorderSize = 0;

      this.btnBracNeck.MouseClick += BtnBracNeck_MouseClick;
      this.Controls.Add(this.btnBracNeck);

      prevCont = this.btnBracNeck;


      currentPosition = defaultPadding;
      currentLine = prevCont.Location.Y + prevCont.Size.Height + defaultPadding;

      this.btnNbRuna = new Button();
      this.btnNbRuna.Name = "btnNbRuna";
      this.btnNbRuna.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnNbRuna.Enabled = true;
      this.btnNbRuna.Text = "NB runa";
      this.btnNbRuna.Font = fontSmall;
      this.btnNbRuna.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnNbRuna.BackColor = Color.LightSlateGray;
      this.btnNbRuna.ForeColor = Color.Black;
      this.btnNbRuna.Padding = new Padding(0);
      this.btnNbRuna.TabStop = false;
      this.btnNbRuna.FlatStyle = FlatStyle.Flat;
      this.btnNbRuna.FlatAppearance.BorderSize = 0;

      this.btnNbRuna.MouseClick += BtnNbRuna_MouseClick; ; ;
      this.Controls.Add(this.btnNbRuna);

      prevCont = this.btnNbRuna;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnNbCech = new Button();
      this.btnNbCech.Name = "btnNbCech";
      this.btnNbCech.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnNbCech.Enabled = true;
      this.btnNbCech.Text = "Arrange";
      this.btnNbCech.Font = fontSmall;
      this.btnNbCech.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnNbCech.BackColor = Color.LightSlateGray;
      this.btnNbCech.ForeColor = Color.Black;
      this.btnNbCech.Padding = new Padding(0);
      this.btnNbCech.TabStop = false;
      this.btnNbCech.FlatStyle = FlatStyle.Flat;
      this.btnNbCech.FlatAppearance.BorderSize = 0;

      this.btnNbCech.MouseClick += BtnNbCech_MouseClick;
      this.Controls.Add(this.btnNbCech);

      prevCont = this.btnNbCech;

      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnPrintItems = new Button();
      this.btnPrintItems.Name = "btnPrintItems";
      this.btnPrintItems.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnPrintItems.Enabled = true;
      this.btnPrintItems.Text = "Print All";
      this.btnPrintItems.Font = fontSmall;
      this.btnPrintItems.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnPrintItems.BackColor = Color.LightSlateGray;
      this.btnPrintItems.ForeColor = Color.Black;
      this.btnPrintItems.Padding = new Padding(0);
      this.btnPrintItems.TabStop = false;
      this.btnPrintItems.FlatStyle = FlatStyle.Flat;
      this.btnPrintItems.FlatAppearance.BorderSize = 0;

      this.btnPrintItems.MouseClick += BtnPrintItems_MouseClick; ;
      this.Controls.Add(this.btnPrintItems);

      prevCont = this.btnPrintItems;


      currentPosition = defaultPadding;
      currentLine = prevCont.Location.Y + prevCont.Size.Height + defaultPadding;

      this.btnSkillTrackAnimal = new Button();
      this.btnSkillTrackAnimal.Name = "btnSkillTrackAnimal";
      this.btnSkillTrackAnimal.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnSkillTrackAnimal.Enabled = true;
      this.btnSkillTrackAnimal.Text = "Track Anml";
      this.btnSkillTrackAnimal.Font = fontSmall;
      this.btnSkillTrackAnimal.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnSkillTrackAnimal.BackColor = Color.LightSlateGray;
      this.btnSkillTrackAnimal.ForeColor = Color.Black;
      this.btnSkillTrackAnimal.Padding = new Padding(0);
      this.btnSkillTrackAnimal.TabStop = false;
      this.btnSkillTrackAnimal.FlatStyle = FlatStyle.Flat;
      this.btnSkillTrackAnimal.FlatAppearance.BorderSize = 0;

      this.btnSkillTrackAnimal.MouseClick += BtnSkillTrackAnimal_MouseClick; ;
      this.Controls.Add(this.btnSkillTrackAnimal);

      prevCont = this.btnSkillTrackAnimal;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnSkillTrackPk = new Button();
      this.btnSkillTrackPk.Name = "btnSkillTrackPk";
      this.btnSkillTrackPk.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnSkillTrackPk.Enabled = true;
      this.btnSkillTrackPk.Text = "Track PK";
      this.btnSkillTrackPk.Font = fontSmall;
      this.btnSkillTrackPk.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnSkillTrackPk.BackColor = Color.LightSlateGray;
      this.btnSkillTrackPk.ForeColor = Color.Black;
      this.btnSkillTrackPk.Padding = new Padding(0);
      this.btnSkillTrackPk.TabStop = false;
      this.btnSkillTrackPk.FlatStyle = FlatStyle.Flat;
      this.btnSkillTrackPk.FlatAppearance.BorderSize = 0;

      this.btnSkillTrackPk.MouseClick += BtnSkillTrackPk_MouseClick;
      this.Controls.Add(this.btnSkillTrackPk);

      prevCont = this.btnSkillTrackPk;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnSkillTrackDetect = new Button();
      this.btnSkillTrackDetect.Name = "btnSkillTrackDetect";
      this.btnSkillTrackDetect.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnSkillTrackDetect.Enabled = true;
      this.btnSkillTrackDetect.Text = "Detect";
      this.btnSkillTrackDetect.Font = fontSmall;
      this.btnSkillTrackDetect.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnSkillTrackDetect.BackColor = Color.LightSlateGray;
      this.btnSkillTrackDetect.ForeColor = Color.Black;
      this.btnSkillTrackDetect.Padding = new Padding(0);
      this.btnSkillTrackDetect.TabStop = false;
      this.btnSkillTrackDetect.FlatStyle = FlatStyle.Flat;
      this.btnSkillTrackDetect.FlatAppearance.BorderSize = 0;

      this.btnSkillTrackDetect.MouseClick += BtnSkillTrackDetect_MouseClick; ;
      this.Controls.Add(this.btnSkillTrackDetect);

      prevCont = this.btnSkillTrackDetect;

      currentPosition = defaultPadding;
      currentLine = prevCont.Location.Y + prevCont.Size.Height + defaultPadding;

      this.btnSkillTrackForensic = new Button();
      this.btnSkillTrackForensic.Name = "btnSkillTrackForensic";
      this.btnSkillTrackForensic.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnSkillTrackForensic.Enabled = true;
      this.btnSkillTrackForensic.Text = "Forens.";
      this.btnSkillTrackForensic.Font = fontSmall;
      this.btnSkillTrackForensic.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnSkillTrackForensic.BackColor = Color.LightSlateGray;
      this.btnSkillTrackForensic.ForeColor = Color.Black;
      this.btnSkillTrackForensic.Padding = new Padding(0);
      this.btnSkillTrackForensic.TabStop = false;
      this.btnSkillTrackForensic.FlatStyle = FlatStyle.Flat;
      this.btnSkillTrackForensic.FlatAppearance.BorderSize = 0;

      this.btnSkillTrackForensic.MouseClick += BtnSkillTrackForensic_MouseClick;
      this.Controls.Add(this.btnSkillTrackForensic);

      prevCont = this.btnSkillTrackForensic;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnSkillTrackItemId = new Button();
      this.btnSkillTrackItemId.Name = "btnSkillTrackItemId";
      this.btnSkillTrackItemId.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnSkillTrackItemId.Enabled = true;
      this.btnSkillTrackItemId.Text = "Item Id.";
      this.btnSkillTrackItemId.Font = fontSmall;
      this.btnSkillTrackItemId.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnSkillTrackItemId.BackColor = Color.LightSlateGray;
      this.btnSkillTrackItemId.ForeColor = Color.Black;
      this.btnSkillTrackItemId.Padding = new Padding(0);
      this.btnSkillTrackItemId.TabStop = false;
      this.btnSkillTrackItemId.FlatStyle = FlatStyle.Flat;
      this.btnSkillTrackItemId.FlatAppearance.BorderSize = 0;

      this.btnSkillTrackItemId.MouseClick += BtnSkillTrackItemId_MouseClick;
      this.Controls.Add(this.btnSkillTrackItemId);

      prevCont = this.btnSkillTrackItemId;

      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnInfo = new Button();
      this.btnInfo.Name = "btnInfo";
      this.btnInfo.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnInfo.Enabled = true;
      this.btnInfo.Text = "Info";
      this.btnInfo.Font = fontSmall;
      this.btnInfo.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnInfo.BackColor = Color.LightSlateGray;
      this.btnInfo.ForeColor = Color.Black;
      this.btnInfo.Padding = new Padding(0);
      this.btnInfo.TabStop = false;
      this.btnInfo.FlatStyle = FlatStyle.Flat;
      this.btnInfo.FlatAppearance.BorderSize = 0;

      this.btnInfo.MouseClick += BtnInfo_MouseClick;
      this.Controls.Add(this.btnInfo);

      prevCont = this.btnInfo;


      currentPosition = defaultPadding;
      currentLine = prevCont.Location.Y + prevCont.Size.Height + defaultPadding;

      this.btnL500= new Button();
      this.btnL500.Name = "btnL500";
      this.btnL500.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnL500.Enabled = true;
      this.btnL500.Text = "Lux 500";
      this.btnL500.Font = fontSmall;
      this.btnL500.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnL500.BackColor = Color.LightSlateGray;
      this.btnL500.ForeColor = Color.Black;
      this.btnL500.Padding = new Padding(0);
      this.btnL500.TabStop = false;
      this.btnL500.FlatStyle = FlatStyle.Flat;
      this.btnL500.FlatAppearance.BorderSize = 0;

      this.btnL500.MouseClick += BtnL500_MouseClick; ;
      this.Controls.Add(this.btnL500);

      prevCont = this.btnL500;

      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnL2500 = new Button();
      this.btnL2500.Name = "btnL2500";
      this.btnL2500.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnL2500.Enabled = true;
      this.btnL2500.Text = "Lux 2500";
      this.btnL2500.Font = fontSmall;
      this.btnL2500.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnL2500.BackColor = Color.LightSlateGray;
      this.btnL2500.ForeColor = Color.Black;
      this.btnL2500.Padding = new Padding(0);
      this.btnL2500.TabStop = false;
      this.btnL2500.FlatStyle = FlatStyle.Flat;
      this.btnL2500.FlatAppearance.BorderSize = 0;

      this.btnL2500.MouseClick += BtnL2500_MouseClick;
      this.Controls.Add(this.btnL2500);

      prevCont = this.btnL2500;

      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnTAll= new Button();
      this.btnTAll.Name = "btnTAll";
      this.btnTAll.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnTAll.Enabled = true;
      this.btnTAll.Text = "Term. All";
      this.btnTAll.Font = fontSmall;
      this.btnTAll.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnTAll.BackColor = Color.LightSlateGray;
      this.btnTAll.ForeColor = Color.Black;
      this.btnTAll.Padding = new Padding(0);
      this.btnTAll.TabStop = false;
      this.btnTAll.FlatStyle = FlatStyle.Flat;
      this.btnTAll.FlatAppearance.BorderSize = 0;

      this.btnTAll.MouseClick += BtnTAll_MouseClick;
      this.Controls.Add(this.btnTAll);

      prevCont = this.btnTAll;

      currentPosition = defaultPadding;
      currentLine = prevCont.Location.Y + prevCont.Size.Height + defaultPadding;

      this.btnLatency= new Button();
      this.btnLatency.Name = "btnLatency";
      this.btnLatency.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnLatency.Enabled = true;
      this.btnLatency.Text = "Latency";
      this.btnLatency.Font = fontSmall;
      this.btnLatency.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnLatency.BackColor = Color.LightSlateGray;
      this.btnLatency.ForeColor = Color.Black;
      this.btnLatency.Padding = new Padding(0);
      this.btnLatency.TabStop = false;
      this.btnLatency.FlatStyle = FlatStyle.Flat;
      this.btnLatency.FlatAppearance.BorderSize = 0;

      this.btnLatency.MouseClick += BtnLatency_MouseClick;
      this.Controls.Add(this.btnLatency);

      prevCont = this.btnLatency;

      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnHide = new Button();
      this.btnHide.Name = "btnHide";
      this.btnHide.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnHide.Enabled = true;
      this.btnHide.Text = "Hide";
      this.btnHide.Font = fontSmall;
      this.btnHide.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnHide.BackColor = Color.LightSlateGray;
      this.btnHide.ForeColor = Color.Black;
      this.btnHide.Padding = new Padding(0);
      this.btnHide.TabStop = false;
      this.btnHide.FlatStyle = FlatStyle.Flat;
      this.btnHide.FlatAppearance.BorderSize = 0;

      this.btnHide.MouseClick += BtnHide_MouseClick;
      this.Controls.Add(this.btnHide);

      prevCont = this.btnHide;

      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnResync = new Button();
      this.btnResync.Name = "btnHide";
      this.btnResync.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnResync.Enabled = true;
      this.btnResync.Text = "Resync";
      this.btnResync.Font = fontSmall;
      this.btnResync.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnResync.BackColor = Color.LightSlateGray;
      this.btnResync.ForeColor = Color.Black;
      this.btnResync.Padding = new Padding(0);
      this.btnResync.TabStop = false;
      this.btnResync.FlatStyle = FlatStyle.Flat;
      this.btnResync.FlatAppearance.BorderSize = 0;

      this.btnResync.MouseClick += BtnResync_MouseClick;
      this.Controls.Add(this.btnResync);

      prevCont = this.btnResync;

      currentPosition = defaultPadding;
      currentLine = prevCont.Location.Y + prevCont.Size.Height + defaultPadding;

      this.btnRunInvis1 = new Button();
      this.btnRunInvis1.Name = "btnRunInvis1";
      this.btnRunInvis1.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnRunInvis1.Enabled = true;
      this.btnRunInvis1.Text = "Runinvis 0";
      this.btnRunInvis1.Font = fontSmall;
      this.btnRunInvis1.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnRunInvis1.BackColor = Color.LightSlateGray;
      this.btnRunInvis1.ForeColor = Color.Black;
      this.btnRunInvis1.Padding = new Padding(0);
      this.btnRunInvis1.TabStop = false;
      this.btnRunInvis1.FlatStyle = FlatStyle.Flat;
      this.btnRunInvis1.FlatAppearance.BorderSize = 0;

      this.btnRunInvis1.MouseClick += BtnRunInvis1_MouseClick;
      this.Controls.Add(this.btnRunInvis1);

      prevCont = this.btnRunInvis1;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnVyhodKlamakNa = new Button();
      this.btnVyhodKlamakNa.Name = "btnVyhodKlamakNa";
      this.btnVyhodKlamakNa.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnVyhodKlamakNa.Enabled = true;
      this.btnVyhodKlamakNa.Text = "IfiInvi";
      this.btnVyhodKlamakNa.Font = fontSmall;
      this.btnVyhodKlamakNa.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnVyhodKlamakNa.BackColor = Color.LightSlateGray;
      this.btnVyhodKlamakNa.ForeColor = Color.Black;
      this.btnVyhodKlamakNa.Padding = new Padding(0);
      this.btnVyhodKlamakNa.TabStop = false;
      this.btnVyhodKlamakNa.FlatStyle = FlatStyle.Flat;
      this.btnVyhodKlamakNa.FlatAppearance.BorderSize = 0;

      this.btnVyhodKlamakNa.MouseClick += BtnVyhodKlamakNa_MouseClick;
      this.Controls.Add(this.btnVyhodKlamakNa);

      prevCont = this.btnVyhodKlamakNa;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnSnap = new Button();
      this.btnSnap.Name = "btnSnap";
      this.btnSnap.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnSnap.Enabled = true;
      this.btnSnap.Text = "Snap";
      this.btnSnap.Font = fontSmall;
      this.btnSnap.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnSnap.BackColor = Color.LightSlateGray;
      this.btnSnap.ForeColor = Color.Black;
      this.btnSnap.Padding = new Padding(0);
      this.btnSnap.TabStop = false;
      this.btnSnap.FlatStyle = FlatStyle.Flat;
      this.btnSnap.FlatAppearance.BorderSize = 0;

      this.btnSnap.MouseClick += BtnSnap_MouseClick; ;
      this.Controls.Add(this.btnSnap);

      prevCont = this.btnSnap;


      currentPosition = defaultPadding;
      currentLine = prevCont.Location.Y + prevCont.Size.Height + defaultPadding;


      this.wsInfo = new Label();
      this.wsInfo.Name = "wsInfo";
      this.wsInfo.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.wsInfo.Enabled = true;
      this.wsInfo.Text = "WS za ";
      this.wsInfo.Font = font;
      this.wsInfo.Size = new Size(200, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.wsInfo.BackColor = Color.Transparent;
      this.wsInfo.ForeColor = Color.GhostWhite;
      this.wsInfo.Padding = new Padding(0);
      this.wsInfo.TabStop = false;
      this.wsInfo.FlatStyle = FlatStyle.Flat;

      this.Controls.Add(this.wsInfo);

      prevCont = this.wsInfo;

      currentPosition = defaultPadding;
      currentLine = prevCont.Location.Y + prevCont.Size.Height + defaultPadding;





      #endregion

      #endregion


      foreach (Control c in this.Controls)
      {
        if (maxX < c.Location.X + c.Size.Width)
          maxX = c.Location.X + c.Size.Width;

        if (maxY < c.Location.Y + c.Size.Height)
          maxY = c.Location.Y + c.Size.Height;
      }

      baseWidth = maxX + defaultPadding;

      #region rightPanel




      currentLine = 24;
      currentPosition = baseWidth + defaultPadding;

      this.btnTLeft = new Button();
      this.btnTLeft.Name = "btnTLeft";
      this.btnTLeft.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnTLeft.Enabled = true;
      this.btnTLeft.Text = "↖";
      this.btnTLeft.Font = fontSmall;
      this.btnTLeft.Size = new Size(buttonMiddleWidth, buttonHeight);
      this.btnTLeft.BackColor = Color.LightSlateGray;
      this.btnTLeft.ForeColor = Color.Black;
      this.btnTLeft.Padding = new Padding(0);
      this.btnTLeft.TabStop = false;
      this.btnTLeft.FlatStyle = FlatStyle.Flat;
      this.btnTLeft.FlatAppearance.BorderSize = 0;

      this.btnTLeft.MouseClick += BtnTLeft_MouseClick;
      this.Controls.Add(this.btnTLeft);

      prevCont = this.btnTLeft;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnForward = new Button();
      this.btnForward.Name = "btnForward";
      this.btnForward.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnForward.Enabled = true;
      this.btnForward.Text = "↑";
      this.btnForward.Font = fontSmall;
      this.btnForward.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnForward.BackColor = Color.LightSlateGray;
      this.btnForward.ForeColor = Color.Black;
      this.btnForward.Padding = new Padding(0);
      this.btnForward.TabStop = false;
      this.btnForward.FlatStyle = FlatStyle.Flat;
      this.btnForward.FlatAppearance.BorderSize = 0;

      this.btnForward.MouseClick += BtnForward_MouseClick;
      this.Controls.Add(this.btnForward);

      prevCont = this.btnForward;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnTRight = new Button();
      this.btnTRight.Name = "btnTRight";
      this.btnTRight.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnTRight.Enabled = true;
      this.btnTRight.Text = "↗";
      this.btnTRight.Font = fontSmall;
      this.btnTRight.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnTRight.BackColor = Color.LightSlateGray;
      this.btnTRight.ForeColor = Color.Black;
      this.btnTRight.Padding = new Padding(0);
      this.btnTRight.TabStop = false;
      this.btnTRight.FlatStyle = FlatStyle.Flat;
      this.btnTRight.FlatAppearance.BorderSize = 0;

      this.btnTRight.MouseClick += BtnTRight_MouseClick;
      this.Controls.Add(this.btnTRight);

      prevCont = this.btnTRight;
       
      currentPosition = baseWidth + defaultPadding;
      currentLine = prevCont.Location.Y + prevCont.Size.Height + defaultPadding;

      this.btnLeft = new Button();
      this.btnLeft.Name = "btnLeft";
      this.btnLeft.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnLeft.Enabled = true;
      this.btnLeft.Text = "←";
      this.btnLeft.Font = fontSmall;
      this.btnLeft.Size = new Size(buttonMiddleWidth, buttonHeight);
      this.btnLeft.BackColor = Color.LightSlateGray;
      this.btnLeft.ForeColor = Color.Black;
      this.btnLeft.Padding = new Padding(0);
      this.btnLeft.TabStop = false;
      this.btnLeft.FlatStyle = FlatStyle.Flat;
      this.btnLeft.FlatAppearance.BorderSize = 0;

      this.btnLeft.MouseClick += BtnLeft_MouseClick;
      this.Controls.Add(this.btnLeft);

      prevCont = this.btnLeft;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnStop = new Button();
      this.btnStop.Name = "btnStop";
      this.btnStop.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnStop.Enabled = true;
      this.btnStop.Text = "Stop";
      this.btnStop.Font = fontSmall;
      this.btnStop.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnStop.BackColor = Color.LightSlateGray;
      this.btnStop.ForeColor = Color.Black;
      this.btnStop.Padding = new Padding(0);
      this.btnStop.TabStop = false;
      this.btnStop.FlatStyle = FlatStyle.Flat;
      this.btnStop.FlatAppearance.BorderSize = 0;

      this.btnStop.MouseClick += BtnStop_MouseClick;
      this.Controls.Add(this.btnStop);

      prevCont = this.btnStop;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnRight = new Button();
      this.btnRight.Name = "btnRight";
      this.btnRight.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnRight.Enabled = true;
      this.btnRight.Text = "→";
      this.btnRight.Font = fontSmall;
      this.btnRight.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnRight.BackColor = Color.LightSlateGray;
      this.btnRight.ForeColor = Color.Black;
      this.btnRight.Padding = new Padding(0);
      this.btnRight.TabStop = false;
      this.btnRight.FlatStyle = FlatStyle.Flat;
      this.btnRight.FlatAppearance.BorderSize = 0;

      this.btnRight.MouseClick += BtnRight_MouseClick; ;
      this.Controls.Add(this.btnRight);

      prevCont = this.btnRight;

      currentPosition = baseWidth + defaultPadding;
      currentLine = prevCont.Location.Y + prevCont.Size.Height + defaultPadding;

      this.btnDock = new Button();
      this.btnDock.Name = "btnDock";
      this.btnDock.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnDock.Enabled = true;
      this.btnDock.Text = "Dock";
      this.btnDock.Font = fontSmall;
      this.btnDock.Size = new Size(buttonMiddleWidth, buttonHeight);
      this.btnDock.BackColor = Color.LightSlateGray;
      this.btnDock.ForeColor = Color.Black;
      this.btnDock.Padding = new Padding(0);
      this.btnDock.TabStop = false;
      this.btnDock.FlatStyle = FlatStyle.Flat;
      this.btnDock.FlatAppearance.BorderSize = 0;

      this.btnDock.MouseClick += BtnDock_MouseClick;
      this.Controls.Add(this.btnDock);

      prevCont = this.btnDock;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnBackward = new Button();
      this.btnBackward.Name = "btnBackward";
      this.btnBackward.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnBackward.Enabled = true;
      this.btnBackward.Text = "↓";
      this.btnBackward.Font = fontSmall;
      this.btnBackward.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnBackward.BackColor = Color.LightSlateGray;
      this.btnBackward.ForeColor = Color.Black;
      this.btnBackward.Padding = new Padding(0);
      this.btnBackward.TabStop = false;
      this.btnBackward.FlatStyle = FlatStyle.Flat;
      this.btnBackward.FlatAppearance.BorderSize = 0;

      this.btnBackward.MouseClick += BtnBackward_MouseClick;
      this.Controls.Add(this.btnBackward);

      prevCont = this.btnBackward;

      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnLockShip = new Button();
      this.btnLockShip.Name = "btnLockShip";
      this.btnLockShip.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnLockShip.Enabled = true;
      this.btnLockShip.Text = "Lock";
      this.btnLockShip.Font = fontSmall;
      this.btnLockShip.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnLockShip.BackColor = Color.LightSlateGray;
      this.btnLockShip.ForeColor = Color.Black;
      this.btnLockShip.Padding = new Padding(0);
      this.btnLockShip.TabStop = false;
      this.btnLockShip.FlatStyle = FlatStyle.Flat;
      this.btnLockShip.FlatAppearance.BorderSize = 0;

      this.btnLockShip.MouseClick += BtnLockShip_MouseClick;
      this.Controls.Add(this.btnLockShip);

      prevCont = this.btnLockShip;

      currentPosition = baseWidth + defaultPadding;
      currentLine = prevCont.Location.Y + prevCont.Size.Height + defaultPadding;

      this.btnMoveRegyAll = new Button();
      this.btnMoveRegyAll.Name = "btnMoveRegyAll";
      this.btnMoveRegyAll.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnMoveRegyAll.Enabled = true;
      this.btnMoveRegyAll.Text = "Move Regy";
      this.btnMoveRegyAll.Font = fontSmall;
      this.btnMoveRegyAll.Size = new Size(buttonMiddleWidth, buttonHeight);
      this.btnMoveRegyAll.BackColor = Color.LightSlateGray;
      this.btnMoveRegyAll.ForeColor = Color.Black;
      this.btnMoveRegyAll.Padding = new Padding(0);
      this.btnMoveRegyAll.TabStop = false;
      this.btnMoveRegyAll.FlatStyle = FlatStyle.Flat;
      this.btnMoveRegyAll.FlatAppearance.BorderSize = 0;

      this.btnMoveRegyAll.MouseClick += BtnMoveRegyAll_MouseClick;
      this.Controls.Add(this.btnMoveRegyAll);

      prevCont = this.btnMoveRegyAll;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnNajdiaPresun = new Button();
      this.btnNajdiaPresun.Name = "btnNajdiaPresun";
      this.btnNajdiaPresun.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnNajdiaPresun.Enabled = true;
      this.btnNajdiaPresun.Text = "Naj a presun";
      this.btnNajdiaPresun.Font = fontSmall;
      this.btnNajdiaPresun.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnNajdiaPresun.BackColor = Color.LightSlateGray;
      this.btnNajdiaPresun.ForeColor = Color.Black;
      this.btnNajdiaPresun.Padding = new Padding(0);
      this.btnNajdiaPresun.TabStop = false;
      this.btnNajdiaPresun.FlatStyle = FlatStyle.Flat;
      this.btnNajdiaPresun.FlatAppearance.BorderSize = 0;

      this.btnNajdiaPresun.MouseClick += BtnNajdiaPresun_MouseClick;
      this.Controls.Add(this.btnNajdiaPresun);

      prevCont = this.btnNajdiaPresun;

      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnSortitemName = new Button();
      this.btnSortitemName.Name = "btnSortitemName";
      this.btnSortitemName.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnSortitemName.Enabled = true;
      this.btnSortitemName.Text = "Sort Name";
      this.btnSortitemName.Font = fontSmall;
      this.btnSortitemName.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnSortitemName.BackColor = Color.LightSlateGray;
      this.btnSortitemName.ForeColor = Color.Black;
      this.btnSortitemName.Padding = new Padding(0);
      this.btnSortitemName.TabStop = false;
      this.btnSortitemName.FlatStyle = FlatStyle.Flat;
      this.btnSortitemName.FlatAppearance.BorderSize = 0;

      this.btnSortitemName.MouseClick += BtnSortitemName_MouseClick;
      this.Controls.Add(this.btnSortitemName);

      prevCont = this.btnSortitemName;

      currentPosition = baseWidth + defaultPadding;
      currentLine = prevCont.Location.Y + prevCont.Size.Height + defaultPadding;


      this.btnNajdiAVloz = new Button();
      this.btnNajdiAVloz.Name = "btnNajdiAVloz";
      this.btnNajdiAVloz.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnNajdiAVloz.Enabled = true;
      this.btnNajdiAVloz.Text = "Najdi a vloz";
      this.btnNajdiAVloz.Font = fontSmall;
      this.btnNajdiAVloz.Size = new Size(buttonMiddleWidth, buttonHeight);
      this.btnNajdiAVloz.BackColor = Color.LightSlateGray;
      this.btnNajdiAVloz.ForeColor = Color.Black;
      this.btnNajdiAVloz.Padding = new Padding(0);
      this.btnNajdiAVloz.TabStop = false;
      this.btnNajdiAVloz.FlatStyle = FlatStyle.Flat;
      this.btnNajdiAVloz.FlatAppearance.BorderSize = 0;

      this.btnNajdiAVloz.MouseClick += BtnNajdiAVloz_MouseClick;
      this.Controls.Add(this.btnNajdiAVloz);

      prevCont = this.btnNajdiAVloz;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnNaplnSperky = new Button();
      this.btnNaplnSperky.Name = "btnNaplnSperky";
      this.btnNaplnSperky.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnNaplnSperky.Enabled = true;
      this.btnNaplnSperky.Text = "Nap sperky";
      this.btnNaplnSperky.Font = fontSmall;
      this.btnNaplnSperky.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnNaplnSperky.BackColor = Color.LightSlateGray;
      this.btnNaplnSperky.ForeColor = Color.Black;
      this.btnNaplnSperky.Padding = new Padding(0);
      this.btnNaplnSperky.TabStop = false;
      this.btnNaplnSperky.FlatStyle = FlatStyle.Flat;
      this.btnNaplnSperky.FlatAppearance.BorderSize = 0;

      this.btnNaplnSperky.MouseClick += BtnNaplnSperky_MouseClick;
      this.Controls.Add(this.btnNaplnSperky);

      prevCont = this.btnNaplnSperky;

      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnNaplABoxy = new Button();
      this.btnNaplABoxy.Name = "btnNaplABoxy";
      this.btnNaplABoxy.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnNaplABoxy.Enabled = true;
      this.btnNaplABoxy.Text = "Nap aboxy";
      this.btnNaplABoxy.Font = fontSmall;
      this.btnNaplABoxy.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnNaplABoxy.BackColor = Color.LightSlateGray;
      this.btnNaplABoxy.ForeColor = Color.Black;
      this.btnNaplABoxy.Padding = new Padding(0);
      this.btnNaplABoxy.TabStop = false;
      this.btnNaplABoxy.FlatStyle = FlatStyle.Flat;
      this.btnNaplABoxy.FlatAppearance.BorderSize = 0;

      this.btnNaplABoxy.MouseClick += BtnNaplABoxy_MouseClick; ;
      this.Controls.Add(this.btnNaplABoxy);

      prevCont = this.btnNaplABoxy;

      currentPosition = baseWidth + defaultPadding;
      currentLine = prevCont.Location.Y + prevCont.Size.Height + defaultPadding;


      this.btnMoveType = new Button();
      this.btnMoveType.Name = "btnMoveType";
      this.btnMoveType.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnMoveType.Enabled = true;
      this.btnMoveType.Text = "Move Typ";
      this.btnMoveType.Font = fontSmall;
      this.btnMoveType.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnMoveType.BackColor = Color.LightSlateGray;
      this.btnMoveType.ForeColor = Color.Black;
      this.btnMoveType.Padding = new Padding(0);
      this.btnMoveType.TabStop = false;
      this.btnMoveType.FlatStyle = FlatStyle.Flat;
      this.btnMoveType.FlatAppearance.BorderSize = 0;

      this.btnMoveType.MouseClick += BtnMoveType_MouseClick;
      this.Controls.Add(this.btnMoveType);

      prevCont = this.btnMoveType;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnSortType = new Button();
      this.btnSortType.Name = "btnSortType";
      this.btnSortType.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnSortType.Enabled = true;
      this.btnSortType.Text = "Sort Typ";
      this.btnSortType.Font = fontSmall;
      this.btnSortType.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnSortType.BackColor = Color.LightSlateGray;
      this.btnSortType.ForeColor = Color.Black;
      this.btnSortType.Padding = new Padding(0);
      this.btnSortType.TabStop = false;
      this.btnSortType.FlatStyle = FlatStyle.Flat;
      this.btnSortType.FlatAppearance.BorderSize = 0;

      this.btnSortType.MouseClick += BtnSortType_MouseClick; ;
      this.Controls.Add(this.btnSortType);

      prevCont = this.btnSortType;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnUnlock = new Button();
      this.btnUnlock.Name = "btnUnlock";
      this.btnUnlock.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnUnlock.Enabled = true;
      this.btnUnlock.Text = "Unlock";
      this.btnUnlock.Font = fontSmall;
      this.btnUnlock.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnUnlock.BackColor = Color.LightSlateGray;
      this.btnUnlock.ForeColor = Color.Black;
      this.btnUnlock.Padding = new Padding(0);
      this.btnUnlock.TabStop = false;
      this.btnUnlock.FlatStyle = FlatStyle.Flat;
      this.btnUnlock.FlatAppearance.BorderSize = 0;

      this.btnUnlock.MouseClick += BtnUnlock_MouseClick;
      this.Controls.Add(this.btnUnlock);

      prevCont = this.btnUnlock;

      currentPosition = baseWidth + defaultPadding;
      currentLine = prevCont.Location.Y + prevCont.Size.Height + defaultPadding;

      this.btnBuy = new Button();
      this.btnBuy.Name = "btnBuy";
      this.btnBuy.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnBuy.Enabled = true;
      this.btnBuy.Text = "Buy";
      this.btnBuy.Font = fontSmall;
      this.btnBuy.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnBuy.BackColor = Color.LightSlateGray;
      this.btnBuy.ForeColor = Color.Black;
      this.btnBuy.Padding = new Padding(0);
      this.btnBuy.TabStop = false;
      this.btnBuy.FlatStyle = FlatStyle.Flat;
      this.btnBuy.FlatAppearance.BorderSize = 0;

      this.btnBuy.MouseClick += BtnBuy_MouseClick;
      this.Controls.Add(this.btnBuy);

      prevCont = this.btnBuy;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnSell = new Button();
      this.btnSell.Name = "btnSortType";
      this.btnSell.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnSell.Enabled = true;
      this.btnSell.Text = "Sell";
      this.btnSell.Font = fontSmall;
      this.btnSell.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnSell.BackColor = Color.LightSlateGray;
      this.btnSell.ForeColor = Color.Black;
      this.btnSell.Padding = new Padding(0);
      this.btnSell.TabStop = false;
      this.btnSell.FlatStyle = FlatStyle.Flat;
      this.btnSell.FlatAppearance.BorderSize = 0;

      this.btnSell.MouseClick += BtnSell_MouseClick;
      this.Controls.Add(this.btnSell);

      prevCont = this.btnSell;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnCorpseQuest = new Button();
      this.btnCorpseQuest.Name = "btnCorpseQuest";
      this.btnCorpseQuest.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnCorpseQuest.Enabled = true;
      this.btnCorpseQuest.Text = "Pick<=1";
      this.btnCorpseQuest.Font = fontSmall;
      this.btnCorpseQuest.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnCorpseQuest.BackColor = Color.LightSlateGray;
      this.btnCorpseQuest.ForeColor = Color.Black;
      this.btnCorpseQuest.Padding = new Padding(0);
      this.btnCorpseQuest.TabStop = false;
      this.btnCorpseQuest.FlatStyle = FlatStyle.Flat;
      this.btnCorpseQuest.FlatAppearance.BorderSize = 0;

      this.btnCorpseQuest.MouseClick += BtnCorpseQuest_MouseClick;
      this.Controls.Add(this.btnCorpseQuest);

      prevCont = this.btnCorpseQuest;

      currentPosition = baseWidth + defaultPadding;
      currentLine = prevCont.Location.Y + prevCont.Size.Height + defaultPadding;

      this.btnMRegy200 = new Button();
      this.btnMRegy200.Name = "btnMRegy200";
      this.btnMRegy200.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnMRegy200.Enabled = true;
      this.btnMRegy200.Text = "Reg 200";
      this.btnMRegy200.Font = fontSmall;
      this.btnMRegy200.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnMRegy200.BackColor = Color.LightSlateGray;
      this.btnMRegy200.ForeColor = Color.Black;
      this.btnMRegy200.Padding = new Padding(0);
      this.btnMRegy200.TabStop = false;
      this.btnMRegy200.FlatStyle = FlatStyle.Flat;
      this.btnMRegy200.FlatAppearance.BorderSize = 0;

      this.btnMRegy200.MouseClick += BtnMRegy200_MouseClick;
      this.Controls.Add(this.btnMRegy200);

      prevCont = this.btnMRegy200;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnMRegy1000 = new Button();
      this.btnMRegy1000.Name = "btnMRegy1000";
      this.btnMRegy1000.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnMRegy1000.Enabled = true;
      this.btnMRegy1000.Text = "Reg 1000";
      this.btnMRegy1000.Font = fontSmall;
      this.btnMRegy1000.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnMRegy1000.BackColor = Color.LightSlateGray;
      this.btnMRegy1000.ForeColor = Color.Black;
      this.btnMRegy1000.Padding = new Padding(0);
      this.btnMRegy1000.TabStop = false;
      this.btnMRegy1000.FlatStyle = FlatStyle.Flat;
      this.btnMRegy1000.FlatAppearance.BorderSize = 0;

      this.btnMRegy1000.MouseClick += BtnMRegy1000_MouseClick;
      this.Controls.Add(this.btnMRegy1000);

      prevCont = this.btnMRegy1000;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnNRegy50 = new Button();
      this.btnNRegy50.Name = "btnNRegy50";
      this.btnNRegy50.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnNRegy50.Enabled = true;
      this.btnNRegy50.Text = "NReg 50";
      this.btnNRegy50.Font = fontSmall;
      this.btnNRegy50.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnNRegy50.BackColor = Color.LightSlateGray;
      this.btnNRegy50.ForeColor = Color.Black;
      this.btnNRegy50.Padding = new Padding(0);
      this.btnNRegy50.TabStop = false;
      this.btnNRegy50.FlatStyle = FlatStyle.Flat;
      this.btnNRegy50.FlatAppearance.BorderSize = 0;

      this.btnNRegy50.MouseClick += BtnNRegy50_MouseClick;
      this.Controls.Add(this.btnNRegy50);

      prevCont = this.btnNRegy50;

      currentPosition = baseWidth + defaultPadding;
      currentLine = prevCont.Location.Y + prevCont.Size.Height + defaultPadding;

      this.btnTame = new Button();
      this.btnTame.Name = "btnTame";
      this.btnTame.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnTame.Enabled = true;
      this.btnTame.Text = "Tamni";
      this.btnTame.Font = fontSmall;
      this.btnTame.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnTame.BackColor = Color.LightSlateGray;
      this.btnTame.ForeColor = Color.Black;
      this.btnTame.Padding = new Padding(0);
      this.btnTame.TabStop = false;
      this.btnTame.FlatStyle = FlatStyle.Flat;
      this.btnTame.FlatAppearance.BorderSize = 0;

      this.btnTame.MouseClick += BtnTame_MouseClick;
      this.Controls.Add(this.btnTame);

      prevCont = this.btnTame;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnWrongKlic = new Button();
      this.btnWrongKlic.Name = "btnWrongKlic";
      this.btnWrongKlic.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnWrongKlic.Enabled = true;
      this.btnWrongKlic.Text = "Wrong Key";
      this.btnWrongKlic.Font = fontSmall;
      this.btnWrongKlic.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnWrongKlic.BackColor = Color.LightSlateGray;
      this.btnWrongKlic.ForeColor = Color.Black;
      this.btnWrongKlic.Padding = new Padding(0);
      this.btnWrongKlic.TabStop = false;
      this.btnWrongKlic.FlatStyle = FlatStyle.Flat;
      this.btnWrongKlic.FlatAppearance.BorderSize = 0;

      this.btnWrongKlic.MouseClick += BtnWrongKlic_MouseClick;
      this.Controls.Add(this.btnWrongKlic);

      prevCont = this.btnWrongKlic;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnBankBuy = new Button();
      this.btnBankBuy.Name = "btnBankBuy";
      this.btnBankBuy.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnBankBuy.Enabled = true;
      this.btnBankBuy.Text = "Bank";
      this.btnBankBuy.Font = fontSmall;
      this.btnBankBuy.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnBankBuy.BackColor = Color.LightSlateGray;
      this.btnBankBuy.ForeColor = Color.Black;
      this.btnBankBuy.Padding = new Padding(0);
      this.btnBankBuy.TabStop = false;
      this.btnBankBuy.FlatStyle = FlatStyle.Flat;
      this.btnBankBuy.FlatAppearance.BorderSize = 0;

      this.btnBankBuy.MouseClick += BtnBankBuy_MouseClick;
      this.Controls.Add(this.btnBankBuy);

      prevCont = this.btnBankBuy;

      currentPosition = baseWidth + defaultPadding;
      currentLine = prevCont.Location.Y + prevCont.Size.Height + defaultPadding;

      this.btnOpenInventory = new Button();
      this.btnOpenInventory.Name = "btnOpenInventory";
      this.btnOpenInventory.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnOpenInventory.Enabled = true;
      this.btnOpenInventory.Text = "Open inv";
      this.btnOpenInventory.Font = fontSmall;
      this.btnOpenInventory.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnOpenInventory.BackColor = Color.LightSlateGray;
      this.btnOpenInventory.ForeColor = Color.Black;
      this.btnOpenInventory.Padding = new Padding(0);
      this.btnOpenInventory.TabStop = false;
      this.btnOpenInventory.FlatStyle = FlatStyle.Flat;
      this.btnOpenInventory.FlatAppearance.BorderSize = 0;

      this.btnOpenInventory.MouseClick += BtnOpenInventory_MouseClick;
      this.Controls.Add(this.btnOpenInventory);

      prevCont = this.btnOpenInventory;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnKokon = new Button();
      this.btnKokon.Name = "btnKokon";
      this.btnKokon.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnKokon.Enabled = true;
      this.btnKokon.Text = "3 pole";
      this.btnKokon.Font = fontSmall;
      this.btnKokon.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnKokon.BackColor = Color.LightSlateGray;
      this.btnKokon.ForeColor = Color.Black;
      this.btnKokon.Padding = new Padding(0);
      this.btnKokon.TabStop = false;
      this.btnKokon.FlatStyle = FlatStyle.Flat;
      this.btnKokon.FlatAppearance.BorderSize = 0;

      this.btnKokon.MouseClick += BtnKokon_MouseClick;
      this.Controls.Add(this.btnKokon);

      prevCont = this.btnKokon;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnTreasureSearch = new Button();
      this.btnTreasureSearch.Name = "btnTreasureSearch";
      this.btnTreasureSearch.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnTreasureSearch.Enabled = true;
      this.btnTreasureSearch.Text = "Poklady";
      this.btnTreasureSearch.Font = fontSmall;
      this.btnTreasureSearch.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnTreasureSearch.BackColor = Color.LightSlateGray;
      this.btnTreasureSearch.ForeColor = Color.Black;
      this.btnTreasureSearch.Padding = new Padding(0);
      this.btnTreasureSearch.TabStop = false;
      this.btnTreasureSearch.FlatStyle = FlatStyle.Flat;
      this.btnTreasureSearch.FlatAppearance.BorderSize = 0;

      this.btnTreasureSearch.MouseClick += BtnTreasureSearch_MouseClick; ;
      this.Controls.Add(this.btnTreasureSearch);

      prevCont = this.btnTreasureSearch;

      //currentPosition = baseWidth + defaultPadding;
      //currentLine = prevCont.Location.Y + prevCont.Size.Height + defaultPadding;

      #endregion

      this.btnOpenSlotForm = new Button();
      this.btnOpenSlotForm.Name = "btnOpenSlotForm";
      this.btnOpenSlotForm.Location = new System.Drawing.Point(baseWidth - defaultPadding - 50 - defaultPadding, defaultPadding);
      this.btnOpenSlotForm.Enabled = true;
      this.btnOpenSlotForm.Text = "SF";
      this.btnOpenSlotForm.Font = fontSmall;
      this.btnOpenSlotForm.Size = new Size(25, 20);
      // this.btnMixurePotion.AutoSize = true;
      this.btnOpenSlotForm.BackColor = Color.DodgerBlue;
      this.btnOpenSlotForm.ForeColor = Color.White;
      this.btnOpenSlotForm.Padding = new Padding(0);
      this.btnOpenSlotForm.TabStop = false;
      this.btnOpenSlotForm.FlatStyle = FlatStyle.Flat;
      this.btnOpenSlotForm.FlatAppearance.BorderSize = 0;

      this.btnOpenSlotForm.MouseClick += BtnOpenSlotForm_MouseClick;
      this.Controls.Add(this.btnOpenSlotForm);


      this.btnOpen = new Button();
      this.btnOpen.Name = "btnOpen";
      this.btnOpen.Location = new System.Drawing.Point(baseWidth - defaultPadding - 25, defaultPadding);
      this.btnOpen.Enabled = true;
      this.btnOpen.Text = ">";
      this.btnOpen.Font = fontSmall;
      this.btnOpen.Size = new Size(25, 20);
      // this.btnMixurePotion.AutoSize = true;
      this.btnOpen.BackColor = Color.DodgerBlue;
      this.btnOpen.ForeColor = Color.White;
      this.btnOpen.Padding = new Padding(0);
      this.btnOpen.TabStop = false;
      this.btnOpen.FlatStyle = FlatStyle.Flat;
      this.btnOpen.FlatAppearance.BorderSize = 0;

      this.btnOpen.MouseClick += BtnOpen_MouseClick; ;
      this.Controls.Add(this.btnOpen);

      this.Size = new Size(maxX + defaultPadding, maxY + defaultPadding);

      this.selectedPotion = this.potion = PotionCollection.Potions.GetItemByName(Config.Profile.UserSettings.GetAttribute(Potion.Cure.name, "Value", "UtilityForm_Potion"));
      this.selectedQuality = this.potionQuality = (PotionQuality)(Config.Profile.UserSettings.GetAttribute((int)PotionQuality.Lesser, "Value", "UtilityForm_PotionQuality"));

      this.cbxPotions.SelectedValueChanged += CbxPotions_SelectedValueChanged;
      this.cbxPotionQualities.SelectedValueChanged += CbxPotionQualities_SelectedValueChanged;

      this.DoubleBuffered = true;
      this.Name = "UtilityForm";
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    private void BtnOpenSlotForm_MouseClick(object sender, MouseEventArgs e)
    {
      new Thread(new ThreadStart(StatusWrapper.ShowSlotForm)).Start();
    }

    //  Serial: 0x40358901  Name: "Scimitar of Vanquishing"  Position: 134.131.0  Flags: 0x0000  Color: 0x044D  Graphic: 0x13B5  Amount: 1  Layer: RightHand Container: 0x00381EB5


    //---------------------------------------------------------------------------------------------

    private void BtnTreasureSearch_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnTreasureSearch_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["SwitchCheckTreasure"]);
    }


    //---------------------------------------------------------------------------------------------

    private void BtnOpen_MouseClick(object sender, MouseEventArgs e)
    {
      if (this.open)
      {
        this.open = false;
        this.btnOpen.Text = ">";
        this.Width = baseWidth;
      }
      else
      {
        this.open = true;
        this.btnOpen.Text = "<";
        this.Width = this.Width + baseWidth;
      }

      this.Invalidate();
      //throw new NotImplementedException();
    }

    private int baseWidth = 0;
    private bool open = false;

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void UseLastUtiltityButton()
    {
      if (Current.lastUsedMethod == null)
      {
        World.Player.PrintMessage("[Zvol button...]", MessageType.Error);
      }
      else
      {
        WindowManager.GetDefaultManager().BeginInvoke(Current.LastUsedMethodRun);
        return;
      }
    }



    public delegate void KlikMethod(object sender, MouseEventArgs e);
    private KlikMethod lastUsedMethod;
    private object _lastUsedObject;
    private object lastUsedObject
    {
      get { return _lastUsedObject; }
      set
      {
        _lastUsedObject = value;
        if (lastUsedObject as Button != null)
        {
          this.name.Text = "Utility Form (" + ((Button)lastUsedObject).Text + ")";
          this.name.Invalidate();
        }
      }
    }
    private static MouseEventArgs lastUsedMouseEventArgs;

    private void LastUsedMethodRun()
    {
      lastUsedMethod(lastUsedObject, lastUsedMouseEventArgs);
    }

    //---------------------------------------------------------------------------------------------

    private UOItem corpseQuestItem = new UOItem(Serial.Invalid);
    private List<string> usedCorpses = new List<string>();

    //---------------------------------------------------------------------------------------------

    private void BtnCorpseQuest_MouseClick(object sender, MouseEventArgs e)
    {


      lastUsedMethod = BtnCorpseQuest_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;


      Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["GrabGround"], 1);
      //    GrabGround
      //  new Thread(new ThreadStart(CorpseQuest)).Start();
    }

    //---------------------------------------------------------------------------------------------

    protected void CorpseQuest()
    {
      if (!corpseQuestItem.Exist || ctrlHold)
      {
        World.Player.PrintMessage("[Zvol zapisnik..]");
        corpseQuestItem = new TargetInfo().GetTarget().Item;
      }

      if (corpseQuestItem.Exist)
      {
        int orig = World.FindDistance;
        World.FindDistance = 4;

        UOItem corpse = new UOItem(Serial.Invalid);
        List<UOItem> corpses = World.Ground.Where(g => g.Graphic == 0x2006 && g.Distance <= 1 && !usedCorpses.Contains(g.Serial + "|" + g.X + "." + g.Y)).OrderBy(g => g.GetRealDistance()).ToList();
        if (corpses.Count > 0)
          corpse = corpses[0];

        if (!corpse.Exist)
        {
          World.Player.PrintMessage("[Vyber mrtvolku..]");
          corpse = new TargetInfo().GetTarget().Item;
        }

        if (corpse.Exist)
        {
          usedCorpses.Add(corpse.Serial + "|" + corpse.X + "." + corpse.Y);

          UO.WaitTargetObject(corpse);
          corpseQuestItem.Use();
        }
        else
        {
          World.Player.PrintMessage("[Zadna mrtvolka..]");
        }

        World.FindDistance = orig;
      }
      else
      {
        World.Player.PrintMessage("[Zvol zapisnik..]", MessageType.Error);
      }

    }


    //---------------------------------------------------------------------------------------------

    private void BtnKokon_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnKokon_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

     Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["SwitchAliePrintPlace2More"]);
             // Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["SipkaKokonNew"]);
    }


    //---------------------------------------------------------------------------------------------

    private void BtnOpenInventory_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnOpenInventory_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      World.Player.Backpack.Use();
    }

    //---------------------------------------------------------------------------------------------

    private void BtnBracNeck_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnBracNeck_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["EquipNBBracNeck"]);
    }

    //---------------------------------------------------------------------------------------------

    private void BtnWrongKlic_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnWrongKlic_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["WrongKlic"]);
    }

    //---------------------------------------------------------------------------------------------

    private void BtnTame_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnTame_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["TrainTamingManual2"]);
    }

    //---------------------------------------------------------------------------------------------

    private void BtnNRegy50_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnNRegy50_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["MoveNRegy50"]);
    }

    //---------------------------------------------------------------------------------------------

    private void BtnMRegy1000_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnMRegy1000_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["MoveMRegy1000"]);
    }

    //---------------------------------------------------------------------------------------------

    private void BtnMRegy200_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnMRegy200_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["MoveMRegy200"]);
    }

    //---------------------------------------------------------------------------------------------

    private void BtnSell_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnSell_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["SellManual"]);
    }

    //---------------------------------------------------------------------------------------------

    private void BtnBuy_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnBuy_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["BuyManual"]);
    }

    //---------------------------------------------------------------------------------------------

    private void BtnVyhodKlamakNa_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnVyhodKlamakNa_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      //VyhodKlamakNa
      Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["RunWaitForInvis"], 10000000);// 
      //  Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["VyhodKlamakNa"]);
    }

    //---------------------------------------------------------------------------------------------

    private void BtnRunInvis1_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnRunInvis1_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["RunInvis"]);
     // new Thread(new ThreadStart(Invis.RunInvis1)).Start();
    }

    //---------------------------------------------------------------------------------------------

    private void BtnUnlock_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnUnlock_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["Unlock"]);
    }

    //---------------------------------------------------------------------------------------------

    private void BtnNaplABoxy_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnNaplABoxy_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["naplnaboxy"]);
      //new Thread(new ThreadStart(ItemHelper.naplnaboxy)).Start();
    }

    //---------------------------------------------------------------------------------------------

    private void BtnNaplnSperky_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnNaplnSperky_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["naplnsperky"]);
      //new Thread(new ThreadStart(ItemHelper.naplnsperky)).Start();
    }

    //---------------------------------------------------------------------------------------------

    private void BtnNajdiAVloz_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnNajdiAVloz_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["najdiavloz"]);
      //new Thread(new ThreadStart(ItemHelper.najdiavloz)).Start();
    }


    //---------------------------------------------------------------------------------------------
    private void BtnSortitemName_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnSortitemName_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["SortitemName"]);
      //new Thread(new ThreadStart(ItemHelper.SortitemName)).Start();
    }

    //---------------------------------------------------------------------------------------------

    private void BtnNajdiaPresun_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnNajdiaPresun_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["najdiapresun"]);
     // new Thread(new ThreadStart(ItemHelper.najdiapresun)).Start();
    }

    //---------------------------------------------------------------------------------------------

    private void BtnMoveRegyAll_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnMoveRegyAll_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;


      Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["MoveRegyAll"]);
//      new Thread(new ThreadStart(ItemHelper.MoveRegyAll)).Start();
    }

    //---------------------------------------------------------------------------------------------

    private void BtnLockShip_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnLockShip_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      new Thread(new ThreadStart(Ship.LockShip)).Start();
    }

    //---------------------------------------------------------------------------------------------

    private void BtnBackward_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnBackward_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      UO.Say("Backward");
    }

    //---------------------------------------------------------------------------------------------

    private void BtnDock_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnDock_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["DockShip"]);
      //new Thread(new ThreadStart(Ship.Current.DockShip)).Start();
    }

    //---------------------------------------------------------------------------------------------

    private void BtnTRight_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnTRight_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      UO.Say("Turn Right");
    }

    //---------------------------------------------------------------------------------------------

    private void BtnStop_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnStop_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      UO.Say("Stop");
    }

    //---------------------------------------------------------------------------------------------

    private void BtnLeft_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnLeft_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      UO.Say("Left");
    }

    //---------------------------------------------------------------------------------------------

    private void BtnRight_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnRight_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      UO.Say("Right");
    }

    //---------------------------------------------------------------------------------------------

    private void BtnForward_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnForward_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      UO.Say("Forward");
    }

    //---------------------------------------------------------------------------------------------

    private void BtnTLeft_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnTLeft_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      UO.Say("Turn Left");
    }

    //---------------------------------------------------------------------------------------------

    private void BtnSnap_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnSnap_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      new Thread(new ThreadStart(new ScreenCapture.ScreenCapture().snap)).Start();
    }

    //---------------------------------------------------------------------------------------------

    private void BtnSortType_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnSortType_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;


      Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["SortitemType"]);
     // new Thread(new ThreadStart(ItemHelper.SortitemType)).Start();
    }

    //---------------------------------------------------------------------------------------------

    private void BtnMoveType_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnMoveType_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;


      Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["MoveitemType"]);
      //new Thread(new ThreadStart(ItemHelper.MoveitemType)).Start();
    }

    //---------------------------------------------------------------------------------------------
    //  private static Button lastUsedButton;
    private void BtnBankBuy_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnBankBuy_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      UO.Say("Muj Bank ovni ucet, prosim.");
    }

    //---------------------------------------------------------------------------------------------

    private void BtnInfo_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnInfo_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      World.Player.PrintMessage("[Vyber Info..]");
      new Thread(new ThreadStart(UO.Info)).Start();
    }

    //---------------------------------------------------------------------------------------------

    private void BtnPrintItems_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnPrintItems_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      Game.PrintKownObjects(true);

    }

    //---------------------------------------------------------------------------------------------

    private void BtnResync_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnResync_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      UO.Resync();
    }

    //---------------------------------------------------------------------------------------------

    private void BtnHide_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnHide_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      World.Player.PrintMessage("[Vyber HIDE..]");
      new Thread(new ThreadStart(UO.Hide)).Start();

    }

    //---------------------------------------------------------------------------------------------

    private void BtnLatency_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnLatency_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      if (!World.Player.Hidden)
        new Thread(new ThreadStart(UO.Latency)).Start();
      else
      {
        World.Player.PrintMessage("Ale ale, nejsi v Hidu?");
        World.Player.PrintMessage(String.Format("Latency {0} ms", Core.Latency));
      }
    }

    //---------------------------------------------------------------------------------------------

    private void BtnTAll_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnTAll_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      RuntimeCore.Executions.TerminateAll();
    }

    //---------------------------------------------------------------------------------------------

    private void BtnL2500_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnL2500_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      World.Player.PrintMessage("[Lux 2500..]");

      Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.CommandList["luxing"], 2500);
     // ItemHelper.luxing(2500);
    }

    //---------------------------------------------------------------------------------------------

    private void BtnL500_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnL500_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      World.Player.PrintMessage("[Lux 500..]");

      Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.CommandList["luxing"], 500);
    }

    //---------------------------------------------------------------------------------------------

    private void BtnSkillTrackItemId_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnSkillTrackItemId_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      UO.UseSkill(StandardSkill.ItemIdentification);
    }

    //---------------------------------------------------------------------------------------------

    private void BtnSkillTrackForensic_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnSkillTrackForensic_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      UO.UseSkill(StandardSkill.ForensicEvaluation);
    }

    //---------------------------------------------------------------------------------------------

    private void BtnSkillTrackDetect_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnSkillTrackDetect_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      UO.UseSkill(StandardSkill.DetectingHidden);
    }

    //---------------------------------------------------------------------------------------------

    private void BtnSkillTrackPk_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnSkillTrackPk_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      UO.WaitMenu("Tracking", "Players");
      UO.UseSkill(StandardSkill.Tracking);
    }

    //---------------------------------------------------------------------------------------------

    private void BtnSkillTrackAnimal_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnSkillTrackAnimal_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      UO.WaitMenu("Tracking", "Animals");
      UO.UseSkill(StandardSkill.Tracking);
    }

    //---------------------------------------------------------------------------------------------

    private void BtnNbCech_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnNbCech_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.CommandList["arrange"], 250);
      ////Todo
      //if (KlicekRakev.Klicek.Exist)
      //  KlicekRakev.Current.KlicekRakevUse("Chci domu");
      //else if (Kniha.CestovniKniha.Exist)
      //{
      //  Kniha.Current.CestovniKnihaUse(1);
      //  Game.Wait(500);
      //  Kniha.Current.CestovniKnihaUse(4);
      //}
      //else if (Kniha.TravelBook.Exist)
      //  Kniha.Current.TravelBookUse(3);
      //else if (Kniha.RuneBook.Exist)
      //  Kniha.Current.RuneBookUse(1);
      //else
      //  World.Player.PrintMessage("Neni cim!", MessageType.Error);
    }

    //---------------------------------------------------------------------------------------------

    private void BtnNbRuna_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnNbRuna_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.CommandList["nbruna"]);
    }

    //---------------------------------------------------------------------------------------------

    private void BtnSortBackpack_MouseClick(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnSortBackpack_MouseClick;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["SortBasicBackpack"]);
    }

    //---------------------------------------------------------------------------------------------

    private void BtnMixureLC_Click(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnMixureLC_Click;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      selectedPotion = Potion.Cure;
      selectedQuality = PotionQuality.Lesser;

      if ((ModifierKeys & Keys.Control) == Keys.Control)
        new Thread(new ThreadStart(MixureSelection)).Start();
      else
        new Thread(new ThreadStart(DrinkSelection)).Start();
    }

    //---------------------------------------------------------------------------------------------

    private void BtnMixureGC_Click(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnMixureGC_Click;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      selectedPotion = Potion.Cure;
      selectedQuality = PotionQuality.Greater;

      if ((ModifierKeys & Keys.Control) == Keys.Control)
        new Thread(new ThreadStart(MixureSelection)).Start();
      else
        new Thread(new ThreadStart(DrinkSelection)).Start();
    }

    //---------------------------------------------------------------------------------------------

    private void BtnMixureGS_Click(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnMixureGS_Click;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      selectedPotion = Potion.Strength;
      selectedQuality = PotionQuality.Greater;

      if ((ModifierKeys & Keys.Control) == Keys.Control)
        new Thread(new ThreadStart(MixureSelection)).Start();
      else
        new Thread(new ThreadStart(DrinkSelection)).Start();
    }

    //---------------------------------------------------------------------------------------------

    private void BtnMixureTR_Click(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnMixureTR_Click;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      selectedPotion = Potion.Refresh;
      selectedQuality = PotionQuality.Total;

      if ((ModifierKeys & Keys.Control) == Keys.Control)
        new Thread(new ThreadStart(MixureSelection)).Start();
      else
        new Thread(new ThreadStart(DrinkSelection)).Start();
    }

    //---------------------------------------------------------------------------------------------

    private void BtnMixureGH_Click(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnMixureGH_Click;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      selectedPotion = Potion.Heal;
      selectedQuality = PotionQuality.Greater;

      if ((ModifierKeys & Keys.Control) == Keys.Control)
        new Thread(new ThreadStart(MixureSelection)).Start();
      else
        new Thread(new ThreadStart(DrinkSelection)).Start();
    }

    //---------------------------------------------------------------------------------------------

    private void BtnMixureMR_Click(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnMixureMR_Click;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      selectedPotion = Potion.ManaRefresh;
      selectedQuality = PotionQuality.None;

      if ((ModifierKeys & Keys.Control) == Keys.Control)
        new Thread(new ThreadStart(MixureSelection)).Start();
      else
        new Thread(new ThreadStart(DrinkSelection)).Start();
    }

    //---------------------------------------------------------------------------------------------

    private void BtnMixureTMR_Click(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnMixureTMR_Click;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      selectedPotion = Potion.TotalManaRefresh;
      selectedQuality = PotionQuality.None;

      if ((ModifierKeys & Keys.Control) == Keys.Control)
        new Thread(new ThreadStart(MixureSelection)).Start();
      else
        new Thread(new ThreadStart(DrinkSelection)).Start();
    }

    //---------------------------------------------------------------------------------------------

    private void BtnMixureINV_Click(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnMixureINV_Click;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      selectedPotion = Potion.Invisibility;
      selectedQuality = PotionQuality.None;

      if (e.Button == MouseButtons.Left)
        new Thread(new ThreadStart(DrinkSelection)).Start();
      else
        new Thread(new ThreadStart(MixureSelection)).Start();
    }

    //---------------------------------------------------------------------------------------------

    private void BtnStatRepair_Click(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnStatRepair_Click;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["opravstaty"]);
      //new Thread(new ThreadStart(ItemHelper.opravstaty)).Start();
    }

    //---------------------------------------------------------------------------------------------

    private void CbxPotionQualities_SelectedValueChanged(object sender, EventArgs e)
    {

      if (!this.initRun)
      {
        this.PotionQuality = (PotionQuality)this.cbxPotionQualities.SelectedItem;
      }
    }

    //---------------------------------------------------------------------------------------------

    private void CbxPotions_SelectedValueChanged(object sender, EventArgs e)
    {
      if (!this.initRun)
      {
        this.Potion = (Potion)this.cbxPotions.SelectedItem;
      }
    }

    //---------------------------------------------------------------------------------------------

    private void BtnMixurePotion_Click(object sender, MouseEventArgs e)
    {
      lastUsedMethod = BtnMixurePotion_Click;
      lastUsedObject = sender;
      lastUsedMouseEventArgs = e;

      selectedPotion = (Potion)this.cbxPotions.SelectedItem;
      selectedQuality = (PotionQuality)this.cbxPotionQualities.SelectedItem;

      if (ctrlHold)
        MixureSelection();
      else
        DrinkSelection();


     // new Thread(new ThreadStart(MixureSelection)).Start();
      //Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["MixurePotion"], ((Potion)this.cbxPotions.SelectedItem).Name, ((PotionQuality)this.cbxPotionQualities.SelectedItem).ToString());
      //Game.CurrentGame.CurrentPlayer.GetSkillInstance<Alchemy>().MixurePotion((Potion)this.cbxPotions.SelectedItem, (PotionQuality)this.cbxPotionQualities.SelectedItem);
      //Alchemy.ExecMixurePotion("", "");
    }

    //---------------------------------------------------------------------------------------------
    private Potion selectedPotion;
    private PotionQuality selectedQuality;
    private void  MixureSelection()
    {


      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Alchemy>().MixurePotion(selectedPotion, selectedQuality);
    }

    private void DrinkSelection()
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Alchemy>().DrinkPotion(selectedPotion);
    }

    //---------------------------------------------------------------------------------------------

    #endregion

    //---------------------------------------------------------------------------------------------
  }
}
