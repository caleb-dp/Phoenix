using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Phoenix;
using Phoenix.WorldData;
using System.Runtime.CompilerServices;

namespace CalExtension.UI.Status
{
    public class ItemCount : UserControl
    {
        private readonly ISupplyCounter counter;
        private readonly Graphic graphic;
        private readonly UOColor color;

        private Phoenix.Gui.Controls.ArtImageControl image;
        private Label amountText;

        public ItemCount(ISupplyCounter counter, Graphic graphic, UOColor color, bool stock, Point offset)
        {
            if (counter == null)
                throw new ArgumentNullException("counter");
            if (graphic == 0)
                throw new ArgumentOutOfRangeException("type");

            this.counter = counter;
            this.graphic = graphic;
            this.color = color;

            InitializeComponent();

            image.Stocked = stock;
            image.DataIndex = graphic;
            if (color > 0 && color.IsConstant) {
                image.UseHue = true;
                image.HueIndex = color;
            }

            Point loc = image.Location;
            loc.Offset(offset);
            image.Location = loc;
        }

        private void InitializeComponent()
        {
            image = new Phoenix.Gui.Controls.ArtImageControl();
            image.ArtData = DataFiles.Art.Items;
            image.Hues = DataFiles.Hues;
            image.Enabled = false;
            image.Location = new Point(0, 0);
            image.ImageAlignment = Phoenix.Gui.Controls.ImageAlignment.TopLeft;
            Controls.Add(image);

            amountText = new Label();
            amountText.Enabled = false;
            amountText.Text = "0";
            amountText.Location = new Point(30, 3);
            amountText.AutoSize = true;
            Controls.Add(amountText);

            Size = new Size(50, 26);
            Enabled = false;
            Padding = new Padding(0);
            Margin = new Padding(0);
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            counter.AmountChanged += new EventHandler(counter_AmountChanged);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            image.Size = new Size(Height - image.Top, Height - image.Left);
            amountText.Left = image.Right + 1;
        }

        protected override void Dispose(bool disposing)
        {
            counter.AmountChanged -= new EventHandler(counter_AmountChanged);
            counter.Dispose();
            base.Dispose(disposing);
        }

        void counter_AmountChanged(object sender, EventArgs e)
        {
            if (InvokeRequired) {
                BeginInvoke(new EventHandler(counter_AmountChanged), sender, e);
                return;
            }

            amountText.Text = counter.CurrentAmount.ToString();
        }
    }
}
