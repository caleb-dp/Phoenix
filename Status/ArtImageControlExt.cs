using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MulLib;
using CalExtension;
using System.IO;

namespace Phoenix.Gui.Controls
{
  [ToolboxBitmap(typeof(PictureBox))]
  public sealed class ArtImageControlExt : Control
  {
    private Bitmap bitmap;
    private Hues hues;
    private IArtData artData;
    private int dataIndex;
    private int hueIndex;
    private bool useHue;
    private bool selected;
    private bool selection;
    private bool mark1;
    private bool grayscale;
    private bool stocked;
    private ImageAlignment imageAlignment;
    private int? counter;

    public ArtImageControlExt()
    {
      SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
      SetStyle(ControlStyles.UserPaint, true);

      hueIndex = 24;
      imageAlignment = ImageAlignment.Center;
    }

    [Browsable(false)]
    [DefaultValue("")]
    public IArtData ArtData
    {
      get { return artData; }
      set
      {
        if (value != artData)
        {
          artData = value;
          RedrawBitmap();
        }
      }
    }

    [Browsable(false)]
    [DefaultValue("")]
    public Hues Hues
    {
      get { return hues; }
      set
      {
        if (value != hues)
        {
          hues = value;
          RedrawBitmap();
        }
      }
    }

    [Category("Appearance")]
    [DefaultValue(0)]
    public int DataIndex
    {
      get { return dataIndex; }
      set
      {
        if (value != dataIndex)
        {
          dataIndex = value;
          RedrawBitmap();
        }
      }
    }

    [Category("Appearance")]
    [DefaultValue(24)]
    public int HueIndex
    {
      get { return hueIndex; }
      set
      {
        if (value != hueIndex)
        {
          hueIndex = value;
          RedrawBitmap();
        }
      }
    }

    [Category("Appearance")]
    [DefaultValue(false)]
    public bool UseHue
    {
      get { return useHue; }
      set
      {
        if (value != useHue)
        {
          useHue = value;
          RedrawBitmap();
        }
      }
    }

    [Category("Appearance")]
    [DefaultValue(false)]
    public bool Selected
    {
      get { return selected; }
      set
      {
        if (value != selected)
        {
          selected = value;
          RedrawBitmap();
        }
      }
    }

    [Category("Appearance")]
    [DefaultValue(false)]
    public bool Selection
    {
      get { return selection; }
      set
      {
        if (value != selection)
        {
          selection = value;
          RedrawBitmap();
        }
      }
    }

    [Category("Appearance")]
    [DefaultValue(false)]
    public bool Mark1
    {
      get { return mark1; }
      set
      {
        if (value != mark1)
        {
          mark1 = value;
          RedrawBitmap();
        }
      }
    }

    [Category("Appearance")]
    [DefaultValue(false)]
    public bool Grayscale
    {
      get { return grayscale; }
      set
      {
        if (value != grayscale)
        {
          grayscale = value;
          RedrawBitmap();
        }
      }
    }

    [Category("Appearance")]
    [DefaultValue(ImageAlignment.Center)]
    public ImageAlignment ImageAlignment
    {
      get { return imageAlignment; }
      set
      {
        if (value != imageAlignment)
        {
          imageAlignment = value;
          Invalidate();
        }
      }
    }

    [Category("Appearance")]
    [DefaultValue(false)]
    public bool Stocked
    {
      get { return stocked; }
      set
      {
        if (value != stocked)
        {
          stocked = value;
          RedrawBitmap();
        }
      }
    }

    [Category("Appearance")]
    [DefaultValue(false)]
    public int? Counter
    {
      get { return counter; }
      set
      {
        if (value != counter)
        {
          counter = value;
          RedrawBitmap();
        }
      }
    }

    public float CounterTextFontSize = 6.0F;
    public Brush CounterBrush = Brushes.GreenYellow;

    public void RedrawBitmap()
    {
      bitmap = null;

      if (artData != null)
      {
        this.BackColor = Color.Black;
        Bitmap art = artData[dataIndex];

        if (useHue && hues != null)
        {
          try
          {
            HueEntry entry = hues[hueIndex];
            if (entry != null)
            {
              Dyes.RecolorFull(entry, art);
            }
          }
          catch { }
        }

        if (!stocked)
          bitmap = art;
        else
        {
          bitmap = new Bitmap(art.Width + 5, art.Height + 5, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

          int minX = 0;
          int minY = 0;
          int maxX = 0;
          int maxY = 0;

          for (int x = 0; x < art.Width; x++)
          {
            for (int y = 0; y < art.Height; y++)
            {
              Color c = art.GetPixel(x, y);
              if (c.IsEmpty || (c.R + c.G + c.G < 12)) { }//Odstin moc blizky cerne { } 
              else
              {
                if (minX == 0 || x < minX)
                  minX = x;

                if (minY == 0 || y < minY)
                  minY = y;

                if (x > maxX)
                  maxX = x;

                if (y > maxY)
                  maxY = y;
              }
            }
          }

          Color borderColor = Color.LightGray;
          if (Selection)
            borderColor = Color.Yellow;
          else if (Selected)
            borderColor = Color.DeepSkyBlue;

          Color markColor = borderColor;//Color.Crimson;
          if (this.Mark1)
            markColor = Color.Crimson; 


          for (int x = minX; x < maxX; x++)
          {
            art.SetPixel(x, minY, markColor);

            art.SetPixel(x, maxY - 1, borderColor);
            art.SetPixel(x, maxY, borderColor);
          }

          for (int y = minY; y < maxY; y++)
          {
            art.SetPixel(minX, y, borderColor);

            art.SetPixel(maxX - 1, y, markColor);
            art.SetPixel(maxX, y, markColor);
          }

          if (Grayscale)
          {

            int x, y;
            for (x = 0; x < art.Width; x++)
            {
              for (y = 0; y < art.Height; y++)
              {
                Color pixelColor = art.GetPixel(x, y);
                Color newColor = Color.FromArgb(pixelColor.R, 0, 0);
                art.SetPixel(x, y, newColor); // Now greyscale
              }
            }
          }

          using (Graphics g = Graphics.FromImage(bitmap))
          {
            g.DrawImage(art, new Rectangle(0, 0, this.Width, this.Height), new Rectangle(minX, minY, maxX - minX, maxY - minY), GraphicsUnit.Pixel);
            if (this.counter.HasValue)
            {
              g.FillRectangle(Brushes.Black, 0, 0, 12, 12);
              g.DrawString(String.Format("{0}", this.counter), new Font("Arial", CounterTextFontSize), CounterBrush, 0, 1);
            }
          }
        }
      }
      else
        this.BackColor = Color.LightGray;

      Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      if (hues != null)
      {
        if (bitmap != null)
        {
          Point pt = new Point((Width - bitmap.Width) / 2, (Height - bitmap.Height) / 2);

          if ((imageAlignment & ImageAlignment.Left) != 0)
            pt.X = 0;
          else if ((imageAlignment & ImageAlignment.Right) != 0)
            pt.X = Width - bitmap.Width;

          if ((imageAlignment & ImageAlignment.Top) != 0)
            pt.Y = 0;
          else if ((imageAlignment & ImageAlignment.Bottom) != 0)
            pt.Y = Height - bitmap.Height;

          e.Graphics.DrawImageUnscaled(bitmap, pt);
        }
      }
      else {
        e.Graphics.DrawRectangle(SystemPens.WindowText, 0, 0, Width - 1, Height - 1);

        SizeF fontSize = e.Graphics.MeasureString(Name, SystemFonts.DialogFont);
        e.Graphics.DrawString(Name, SystemFonts.DialogFont, SystemBrushes.WindowText, (Width - fontSize.Width) / 2, (Height - fontSize.Height) / 2);
      }

      base.OnPaint(e);
    }


    #region Disabled properties

    [Browsable(false)]
    [DefaultValue("")]
    public override Image BackgroundImage
    {
      get { return null; }
      set { }
    }

    [Browsable(false)]
    [DefaultValue("")]
    public override ImageLayout BackgroundImageLayout
    {
      get { return ImageLayout.None; }
      set { }
    }

    [Browsable(false)]
    [DefaultValue("")]
    public override string Text
    {
      get { return Name; }
      set { }
    }

    [Browsable(false)]
    [DefaultValue("")]
    public override Font Font
    {
      get { return SystemFonts.DefaultFont; }
      set { }
    }

    [Browsable(false)]
    [DefaultValue("")]
    public override RightToLeft RightToLeft
    {
      get { return RightToLeft.No; }
      set { }
    }

    #endregion
  }
}
