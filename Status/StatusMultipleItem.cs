using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Phoenix;
using Phoenix.WorldData;
using System.Threading;
using MulLib;

namespace CalExtension.UI.Status
{
  public class StatusMultipleItem : Control
  {
    private UOColor[] notorietyColors = new UOColor[] { Env.DefaultConsoleColor, 0x0063, 0x0044, 0x03E9, 0x03E5, 0x0030, 0x0026, 0x0481 };
    private Label name;
    private Label hits;
    private HealthBar healthBar;
    protected UOCharacter mobile;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:HealthBar"/> class.
    /// </summary>
    public StatusMultipleItem(Serial id)
    {
      Enabled = false;
      this.mobile = new UOCharacter(id);
      mobile.Changed += new ObjectChangedEventHandler(mobile_Changed);

      InitializeComponent();
    }


    public void OnClose(EventArgs e)
    {
      mobile.Changed -= mobile_Changed;
    }

    void mobile_Changed(object sender, ObjectChangedEventArgs e)
    {
      UpdateStats();
    }



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

      if (mobile.MaxHits > -1)
      {
        hits.Text = String.Format("{0}/{1}", mobile.Hits, mobile.MaxHits);
        healthBar.Hits = mobile.Hits;
        healthBar.MaxHits = mobile.MaxHits;
        healthBar.Poison = mobile.Poisoned;
      }
      Notoriety n = mobile.Notoriety;
      HueEntry notoh = DataFiles.Hues.Get(notorietyColors[(int)n]);
      ushort noto = notoh.Colors[12];

      this.BackColor = Color.FromArgb(UOColorConverter.ToArgb(noto) | (0xFF << 24));
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
    }

    protected override void OnKeyPress(KeyPressEventArgs e)
    {
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
    }

    #region WinForms

    private void InitializeComponent()
    {
      this.name = new System.Windows.Forms.Label();
      this.hits = new System.Windows.Forms.Label();
      this.healthBar = new CalExtension.UI.Status.HealthBar();


      this.SuspendLayout();

      this.name = new System.Windows.Forms.Label();
      this.name.AutoSize = true;
      this.name.BackColor = System.Drawing.Color.Transparent;
      this.name.Enabled = false;
      this.name.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

      this.name.Location = new System.Drawing.Point(3, 3);
      this.name.Name = "name";
      this.name.Size = new System.Drawing.Size(100, 14);
      this.name.TabIndex = 0;
      this.name.Text = "label1";


      this.hits.AutoSize = true;
      this.hits.BackColor = System.Drawing.Color.Transparent;
      this.hits.Enabled = false;
      this.hits.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));

      this.hits.ForeColor = System.Drawing.Color.Silver;
      this.hits.Location = new System.Drawing.Point(3, 18);
      this.hits.Name = "hits";
      this.hits.Size = new System.Drawing.Size(50, 10);
      this.hits.TabIndex = 1;
      this.hits.Text = "0000/0000";

      this.healthBar.Enabled = false;
      this.healthBar.Hits = 0;
      this.healthBar.Location = new System.Drawing.Point(3, 12);
      this.healthBar.MaxHits = 0;
      this.healthBar.Name = "healthBar";
      this.healthBar.Size = new System.Drawing.Size(100, 5);
      this.healthBar.TabIndex = 2;
      this.healthBar.Text = "healthBar1";
      this.healthBar.Unknown = false;

      this.Size = new Size(110, 25);

      this.BackColor = System.Drawing.Color.Black;

      this.Controls.Add(this.hits);
      this.Controls.Add(this.name);
      this.Controls.Add(this.healthBar);

      this.DoubleBuffered = true;
      this.Name = "StatusMultipleForm";
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    #endregion

  }
}
