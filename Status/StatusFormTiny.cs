using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phoenix;
using System.Windows.Forms;
using Phoenix.WorldData;
using System.Drawing;
using Phoenix.Gui.Controls;
using MulLib;
using Phoenix.Communication.Packets;
using Phoenix.Communication;
using System.Threading;
using System.Runtime.InteropServices;
using System.Reflection;
using CalExtension.Skills;

namespace CalExtension.UI.Status
{
  class StatusFormTiny : InGameWindow
  {
    private UOColor[] notorietyColors = new UOColor[] { Env.DefaultConsoleColor, 0x0063, 0x0044, 0x03E9, 0x03E5, 0x0030, 0x0026, 0x0481 };

    private Label name;
    private Label hits;

    private UOCharacter mobile;
    private HealthBar healthBar;

    private WorldLocation lastloc;
    private bool lastWar;
    private DateTime lastRequest;
    private bool statusNeeded = true;
    public static object SyncRoot = new object();
    public bool Transparency { get; set; }
    public StatusType StatusType { get; set; }

    //---------------------------------------------------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="T:StatusForm"/> class.
    /// </summary>
    public StatusFormTiny(Serial id)
    {
      initTime = DateTime.Now;
      mobile = new UOCharacter(id);
      InitializeComponent();

      MouseEnter += new EventHandler(StatusForm_MouseEnter);
      MouseLeave += new EventHandler(StatusForm_MouseLeave);
      MouseDoubleClick += new MouseEventHandler(StatusForm_MouseDoubleClick);
      MouseClick += StatusForm_MouseClick;
      Target += new EventHandler(StatusForm_Target);

      if (id == World.Player.Serial)
        this.StatusType = StatusType.Player;
      else if (mobile.Renamable)
        this.StatusType = StatusType.Mob;
      else if (mobile.Notoriety == Notoriety.Guild || mobile.Notoriety == Notoriety.Innocent || mobile.Notoriety == Notoriety.Neutral)
        this.StatusType = StatusType.Friend;
      else
        this.StatusType = StatusType.Enemy;

      lastMaxHits = mobile.MaxHits;
    }

    //---------------------------------------------------------------------------------------------

    void StatusForm_MouseClick(object sender, MouseEventArgs e)
    {
      if (e.Button == System.Windows.Forms.MouseButtons.Right)
      {
      }
    }

    //---------------------------------------------------------------------------------------------


    protected override void OnMove(EventArgs e)
    {
   //   this.moved = true;
      base.OnMove(e);
    }

    //---------------------------------------------------------------------------------------------

    protected override bool Targettable
    {
      get { return true; }
    }

    //---------------------------------------------------------------------------------------------
    
    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      mobile.Changed += new ObjectChangedEventHandler(mobile_Changed);
      World.Player.Changed += new ObjectChangedEventHandler(Player_Changed);
      World.CharacterAppeared += new CharacterAppearedEventHandler(World_CharacterAppeared);

      if (Transparency)
      {
        Opacity = 0.8;
      }

      UpdateStats();
    }

    //---------------------------------------------------------------------------------------------

    protected override void Dispose(bool disposing)
    {
      mobile.Changed -= new ObjectChangedEventHandler(mobile_Changed);
      World.Player.Changed -= new ObjectChangedEventHandler(Player_Changed);
      World.CharacterAppeared -= new CharacterAppearedEventHandler(World_CharacterAppeared);

      base.Dispose(disposing);
    }

    //---------------------------------------------------------------------------------------------

    public Serial MobileId
    {
      get { return mobile.Serial; }
    }

    //---------------------------------------------------------------------------------------------

    void World_CharacterAppeared(object sender, CharacterAppearedEventArgs e)
    {
      if (e.Serial == mobile.Serial)
      {
        UpdateStats();
      }
    }

    //---------------------------------------------------------------------------------------------

    void Player_Changed(object sender, ObjectChangedEventArgs e)
    {
      if (WalkHandling.DesiredPosition.X != lastloc.X && WalkHandling.DesiredPosition.Y != lastloc.Y)
      {
        UpdateStats();
      }
      if (World.Player.Warmode != lastWar)
      {
        lastWar = World.Player.Warmode;
        UpdateCursor();
      }
    }

    //---------------------------------------------------------------------------------------------

    protected override void OnShown(EventArgs e)
    {
      base.OnShown(e);
      UpdateStats();
    }

    //---------------------------------------------------------------------------------------------

    protected override void OnClosed(EventArgs e)
    {
      mobile.Changed -= mobile_Changed;
      base.OnClosed(e);
    }

    //---------------------------------------------------------------------------------------------

    void mobile_Changed(object sender, ObjectChangedEventArgs e)
    {
      UpdateStats();
    }

    //---------------------------------------------------------------------------------------------
    
    private DateTime initTime;
    private short lastMaxHits;

    //---------------------------------------------------------------------------------------------

    bool statDown = false;
    private void UpdateStats()
    {

      if (InvokeRequired)
      {
        BeginInvoke(new ThreadStart(UpdateStats));
        return;
      }

      if (mobile.Name != null)
      {
        name.Text = mobile.Name;
      }

      //UO.Print("P: " + String.Format("{0}/{1}", mobile.Hits, mobile.MaxHits));

      if (mobile.MaxHits > -1)
      {
        hits.Text = String.Format("{0}/{1}", mobile.Hits, mobile.MaxHits);
        healthBar.Hits = mobile.Hits;
        healthBar.MaxHits = mobile.MaxHits;
        healthBar.Poison = mobile.Poisoned;
        healthBar.Unknown = false;
        statusNeeded = false;


        if (this.StatusType == StatusType.Player)
        {
        }
        else
        {
          lastMaxHits = mobile.MaxHits;
        }
      }
      else {
        healthBar.Unknown = true;
        if (mobile.Serial == World.Player.Serial)
        {
        }
        else
        {
          if ((DateTime.Now - initTime).TotalSeconds > 20 && mobile.Hits <= 1)
          {
            WindowManager.GetDefaultManager().BeginInvoke(Close);
            return;
          }
        }
      }

      Notoriety n = mobile.Notoriety;
      if (mobile.Distance > 18)
      {
        n = Notoriety.Unknown;
        healthBar.Unknown = true;
        if (mobile.Serial == World.Player.Serial)
        {
        }
      }

      HueEntry notoh = DataFiles.Hues.Get(notorietyColors[(int)n]);
      ushort noto = notoh.Colors[12];

      if (statDown)
      {
        noto = DataFiles.Hues.Get(0x01c0).Colors[12];
      }
      else
      {
        if (this.StatusType == StatusType.Mob)
          noto = DataFiles.Hues.Get(0x0035).Colors[12];

        if (this.StatusType == StatusType.Player && Game.IsHealOn)
        {
          noto = DataFiles.Hues.Get(0x0035).Colors[12];

          if (CalebConfig.HealMoby && Game.CurrentGame.HasAliveAlie)
            noto = DataFiles.Hues.Get(0x000d).Colors[12];
        }
      }

      //1c0

      BackColor = Color.FromArgb(UOColorConverter.ToArgb(noto) | (0xFF << 24));

      // Do we need status?
      if (healthBar.Unknown)
      {
        statusNeeded = true;
      }

      // Request if mob is visible
      if (statusNeeded && mobile.Distance < 19 && DateTime.Now - lastRequest < TimeSpan.FromSeconds(10))
      {
        statusNeeded = false;
        lastRequest = DateTime.Now;
        mobile.RequestStatus();
      }
    }

    //---------------------------------------------------------------------------------------------


    #region System

    void StatusForm_MouseEnter(object sender, EventArgs e)
    {
      if (Transparency)
      {
        // Opacity = 1.0;
      }
    }

    void StatusForm_MouseLeave(object sender, EventArgs e)
    {
      if (Transparency)
      {
        Opacity = 0.8;
      }
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
      base.OnPaintBackground(e);
      e.Graphics.DrawRectangle(Pens.Black, new Rectangle(0, 0, Width - 1, Height - 1));
    }

    void StatusForm_Target(object sender, EventArgs e)
    {
      // Simulate click
      byte[] data = PacketBuilder.Target(0, UIManager.ClientTargetId, 0, mobile.Serial, mobile.X, mobile.Y, mobile.Z, 0);
      Core.SendToServer(data, true);

      Client.Window.PostMessage(WM_KEYDOWN, 27, 0x00010001);
      UpdateCursor();

      // Select in client
      // MarkAsAttacked();
    }

    private void MarkAsAttacked()
    {
      byte[] data = new byte[5];
      data[0] = 0xAA;
      ByteConverter.BigEndian.ToBytes((uint)mobile.Serial, data, 1);

      Core.SendToClient(data, false);
    }

    void StatusForm_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      if (mobile.Notoriety == Notoriety.Enemy || mobile.Notoriety == Notoriety.Murderer)
      {
        World.Player.ChangeWarmode(WarmodeChange.War);
        UO.Attack(mobile.Serial);
      }
      else
      {
        if (World.Player.Warmode)
          UO.Attack(mobile.Serial);
        else
          mobile.Use();
      }
    }

    #endregion

    #region WinForms

    private void InitializeComponent()
    {

      this.name = new System.Windows.Forms.Label();
      this.hits = new System.Windows.Forms.Label();
      this.healthBar = new CalExtension.UI.Status.HealthBar();

      this.SuspendLayout();

      this.name.AutoSize = true;
      this.name.BackColor = System.Drawing.Color.Transparent;
      this.name.Enabled = false;
      this.name.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.name.ForeColor = System.Drawing.Color.Silver;
      this.name.Location = new System.Drawing.Point(1, 2);
      this.name.Name = "name";
      this.name.Size = new System.Drawing.Size(80, 20);
      this.name.TabIndex = 0;
      this.name.Text = "label1";
      

      // 
      // hits
      // 
      this.hits.AutoSize = true;
      this.hits.BackColor = System.Drawing.Color.Transparent;
      this.hits.Enabled = false;
      this.hits.Font = new System.Drawing.Font("Arial", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
      this.hits.ForeColor = System.Drawing.Color.Silver;
      this.hits.Location = new System.Drawing.Point(90, 2);
      this.hits.Name = "hits";
      this.hits.Size = new System.Drawing.Size(50, 12);
      this.hits.TabIndex = 1;
      this.hits.Text = "000/000";
      // 
      // healthBar
      // 
      this.healthBar.Enabled = false;
      this.healthBar.Hits = 0;
      this.healthBar.Location = new System.Drawing.Point(80, 15);
      this.healthBar.MaxHits = 0;
      this.healthBar.Name = "healthBar";
      this.healthBar.Size = new System.Drawing.Size(58, 7);
      this.healthBar.TabIndex = 2;
      this.healthBar.Text = "healthBar1";
      this.healthBar.Unknown = false;


      // 
      // StatusForm
      // 

      this.ClientSize = new System.Drawing.Size(140, 10);

      this.BackColor = System.Drawing.Color.Black;

      this.Controls.Add(this.hits);
      this.Controls.Add(this.name);
      this.Controls.Add(this.healthBar);

      this.DoubleBuffered = true;
      this.Name = "StatusFormTiny";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion
  }
}
