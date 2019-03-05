using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phoenix;
using System.Windows.Forms;
using Phoenix.WorldData;
using System.Threading;

namespace CalExtension.UI.Status
{
    public class InGameWindow : FloatingWindow
    {
        private Cursor cursorDefault;
        private Cursor cursorWar;
        private Cursor cursorTarget;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:InGameWindow"/> class.
        /// </summary>
        public InGameWindow()
        {
            if (!DesignMode) {
                cursorDefault = LoadCursor(Core.Directory + @"\Scripts\Status\default.cur");
                cursorWar = LoadCursor(Core.Directory + @"\Scripts\Status\war.cur");
                cursorTarget = LoadCursor(Core.Directory + @"\Scripts\Status\target32.cur");

                Cursor = cursorDefault;
            }

            UIManager.StateChanged += new EventHandler(UIManager_StateChanged);
        }

        /// <summary>
        /// Urcuje, zdali se zmeni kurzor pri targetu, a muze byt zacileno.
        /// </summary>
        protected virtual bool Targettable
        {
            get { return false; }
        }

        /// <summary>
        /// Server ocekaval target a byl zacilen na tomto okne.
        /// </summary>
        public event EventHandler Target;

        protected void OnTarget(EventArgs e)
        {
            EventHandler h = Target;
            if (h != null)
                h(this, e);
        }

        protected override void Dispose(bool disposing)
        {
            UIManager.StateChanged -= new EventHandler(UIManager_StateChanged);

            // Dispose cursor
            Cursor = Cursors.Default;

            if (cursorDefault != null) cursorDefault.Dispose();
            if (cursorWar != null) cursorWar.Dispose();
            if (cursorTarget != null) cursorTarget.Dispose();
            cursorDefault = cursorTarget = cursorWar = null;

            base.Dispose(disposing);
        }

        void UIManager_StateChanged(object sender, EventArgs e)
        {
            UpdateCursor();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            UpdateCursor();

            base.OnMouseEnter(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) {
                if (UIManager.CurrentState == UIManager.State.ServerTarget || UIManager.CurrentState == UIManager.State.Target) {
                    OnTarget(EventArgs.Empty);
                    return;
                }
            }

            base.OnMouseClick(e);
        }

        protected void UpdateCursor()
        {
            if (InvokeRequired) {
                BeginInvoke(new ThreadStart(UpdateCursorImpl));
                return;
            }

            UpdateCursorImpl();
        }

        /// <summary>
        /// Do no call this method directly, use <see cref="UpdateCursor"/>, which is thread-safe.
        /// </summary>
        protected virtual void UpdateCursorImpl()
        {
            if (Targettable && (UIManager.CurrentState == UIManager.State.ServerTarget || UIManager.CurrentState == UIManager.State.Target)) {
                Cursor = cursorTarget;
            }
            else {
                if (World.Player.Warmode)
                    Cursor = cursorWar;
                else
                    Cursor = cursorDefault;
            }
        }
    }
}
