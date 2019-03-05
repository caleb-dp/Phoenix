using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace CalExtension.UI.Status
{
  public class HealthBar : Control
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:HealthBar"/> class.
    /// </summary>
    public HealthBar()
    {
      Hits = 75;
      MaxHits = 100;

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
    private int hits;
    private int maxhits;
    private bool poison;

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

    public int Hits
    {
      get { return hits; }
      set
      {
        if (value != hits)
        {
          hits = value;
          Invalidate();
        }
      }
    }

    public int MaxHits
    {
      get { return maxhits; }
      set
      {
        if (value != maxhits)
        {
          maxhits = value;
          Invalidate();
        }
      }
    }

    public bool Poison
    {
      get { return poison; }
      set
      {
        if (value != poison)
        {
          poison = value;
          Invalidate();
        }
      }
    }

    private Color missincolor = Color.FromArgb(0x730818 | (0xFF << 24));
    private Color hitcolor = Color.FloralWhite; //Color.FromArgb(0x10215A | (0xFF << 24));
    private Color poisoncolor = Color.FromArgb(0x529C18 | (0xFF << 24));

    protected override void OnPaint(PaintEventArgs e)
    {
      using (var bh = new SolidBrush(poison ? poisoncolor : hitcolor))
      using (var bm = new SolidBrush(missincolor))
      {
        if (Hits > 0 && MaxHits > 0)
        {
          int percent = Math.Min(100 * Hits / MaxHits, 100);

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
