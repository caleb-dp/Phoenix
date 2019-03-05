using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace CalExtension.UI.Status
{
  public class StaminaBar : Control
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:HealthBar"/> class.
    /// </summary>
    public StaminaBar()
    {
      Stam = 75;
      MaxStam = 100;

      Enabled = false;
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

    private bool unknown;
    private int stam;
    private int maxStam;

    public bool Unknown
    {
      get { return unknown; }
      set
      {
        if (value != unknown)
        {
          unknown = value;
          Invalidate();
        }
      }
    }

    public int Stam
    {
      get { return stam; }
      set
      {
        if (value != stam)
        {
          stam = value;
          Invalidate();
        }
      }
    }

    public int MaxStam
    {
      get { return maxStam; }
      set
      {
        if (value != maxStam)
        {
          maxStam = value;
          Invalidate();
        }
      }
    }

    private Color missincolor = Color.FromArgb(0x730818 | (0xFF << 24));
    private Color manacolor = Color.FloralWhite;//Color.FromArgb(0x10215A | (0xFF << 24));

    protected override void OnPaint(PaintEventArgs e)
    {
      using (var bh = new SolidBrush(manacolor))
      using (var bm = new SolidBrush(missincolor))
      {
        if (Stam > 0 && MaxStam > 0)
        {
          int percent = Math.Min(100 * Stam / MaxStam, 100);

          Rectangle hits = new Rectangle(0, 0, percent * Width / 100, Height);
          e.Graphics.FillRectangle(bh, hits);

          Rectangle missing = new Rectangle(percent * Width / 100, 0, 0, Height);
          missing.Width = Width - missing.X;
          e.Graphics.FillRectangle(bm, missing);
        }
        else {
          Rectangle missing = new Rectangle(0, 0, Width, Height);
          e.Graphics.FillRectangle(bm, missing);
        }
      }

      if (Unknown)
      {
        using (var b = new SolidBrush(Color.FromArgb(150, 128, 128, 128)))
        {
          e.Graphics.FillRectangle(b, new Rectangle(0, 0, Width, Height));
        }
      }

      using (var p = new Pen(Color.FromArgb(255, Color.FromArgb(0x7B6B29)), 1))
      {
        e.Graphics.DrawRectangle(p, 0, 0, Width - 1, Height - 1);
      }
    }
  }
}
