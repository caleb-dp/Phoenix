using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Phoenix;
using Phoenix.WorldData;

namespace CalExtension.UI.Status
{
    public class SuppliesForm : InGameWindow
    {
        private FlowLayoutPanel container;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SuppliesForm"/> class.
        /// </summary>
        public SuppliesForm()
        {
            container = new FlowLayoutPanel();
            container.FlowDirection = FlowDirection.TopDown;
            container.Enabled = false;
            container.BackColor = Color.Transparent;
            container.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            Controls.Add(container);

            Size = new Size(100, 100);
            ItemSize = new Size(50, 24);
            //BackColor = Color.Red;


      Label l = new Label();
      l.Text = "KUNDA";
      l.Size = new Size(50, 50);
      l.AutoSize = true;
      l.BackColor = System.Drawing.Color.Red;
      l.Enabled = false;
      l.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      l.ForeColor = System.Drawing.Color.Purple;
      
      l.Name = "name";
      
      l.TabIndex = 0;


      container.Controls.Add(l);

      //BackgroundImage = Image.FromFile(Core.Directory + @"\Scripts\Status\backgray.png");
      //BackgroundImageLayout = ImageLayout.Tile;
    }

        protected override bool Targettable
        {
            get { return false; }
        }

        public FlowDirection FlowDirection
        {
            get { return container.FlowDirection; }
            set { container.FlowDirection = value; }
        }

        public Size ItemSize { get; set; }

        public void Add(Graphic type, UOColor color, bool stock)
        {
            Add(type, type, color, stock, Point.Empty);
        }

        public void Add(Graphic type, UOColor color, bool stock, Point offset)
        {
            Add(type, type, color, stock, offset);
        }

        public void Add(Graphic type, Graphic graphic, UOColor color, bool stock, Point offset)
        {
            Add(type, type, color, stock, offset, false);
        }

        public void Add(Graphic type, Graphic graphic, UOColor color, bool stock, Point offset, bool flowBreak)
        {
            var sc = new SupplyCounter(World.Player.Backpack, type, color);

            var c = new ItemCount(sc, graphic, color, stock, offset);
            c.Size = ItemSize;

            container.Controls.Add(c);
            container.SetFlowBreak(c, flowBreak);
        }
    }
}
