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
using CalExtension.UOExtensions;

namespace CalExtension.UI.Status
{
  class StatusMultipleForm : InGameWindow
  {
    private Label name;


    /// <summary>
    /// Initializes a new instance of the <see cref="T:StatusForm"/> class.
    /// </summary>
    public StatusMultipleForm()
    {
      InitializeComponent();
    }

    protected override bool Targettable
    {
      get { return true; }
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
    }

    protected override void OnShown(EventArgs e)
    {
      base.OnShown(e);

      World.CharacterAppeared += World_CharacterAppeared;
    }

    private void World_CharacterAppeared(object sender, CharacterAppearedEventArgs e)
    {
      UOCharacter ch = new UOCharacter(e.Serial);
      if (String.IsNullOrEmpty(ch.Name))
      {
        ch.RequestStatus(250);
      }

      StatusMultipleItem itm = new StatusMultipleItem(e.Serial);
      this.Controls.Add(itm);
      //this.name.Text +=  ch.Name + Environment.NewLine;

      this.Invalidate();
    }

    protected override void OnClosed(EventArgs e)
    {
      base.OnClosed(e);
    }

    #region WinForms

    private void InitializeComponent()
    {


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

      this.BackColor = System.Drawing.Color.DarkCyan;

      this.Controls.Add(this.name);

      this.DoubleBuffered = true;
      this.Name = "StatusMultipleForm";
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    #endregion
  }
}
