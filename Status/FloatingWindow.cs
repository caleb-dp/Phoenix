using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phoenix.Gui.Controls;
using Phoenix;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using System.ComponentModel;

namespace CalExtension.UI.Status
{
    public class FloatingWindow : FormEx
    {
        #region Native

        protected const int WM_KEYDOWN = 0x100;
        protected const int WM_KEYUP = 0x101;
        protected const int WM_SYSKEYDOWN = 0x104;
        protected const int WM_SYSKEYUP = 0x102;

        protected const int WS_EX_NOACTIVATE = 0x8000000;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        #endregion

        private Point offset;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:HelperForm"/> class.
        /// </summary>
        public FloatingWindow()
        {
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            TopMost = true; // Show it on the top by default
            ActivateClient = true;
        }

        #region Visibility

        protected override void Dispose(bool disposing)
        {
            Client.WindowFocusChanged -= Client_WindowFocusChanged;

            base.Dispose(disposing);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (Visible)
                Client.WindowFocusChanged += Client_WindowFocusChanged;
            else
                Client.WindowFocusChanged -= Client_WindowFocusChanged;
        }

        void Client_WindowFocusChanged(object sender, EventArgs e)
        {
            if (InvokeRequired) {
                BeginInvoke(new EventHandler(Client_WindowFocusChanged), sender, e);
                return;
            }

            if (!IsDisposed) {
                SetTopMost(Client.WindowFocus);
            }
        }

        #endregion

        /// <summary>
        /// Gets or sets, whether client window should be automatically activated on click.
        /// </summary>
        [DefaultValue(true)]
        public bool ActivateClient { get; set; }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams p = base.CreateParams;
                p.ExStyle |= WS_EX_NOACTIVATE;
                return p;
            }
        }

        #region Moving window

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (Capture) {
                Point loc = PointToScreen(e.Location);
                loc.Offset(offset);

                Location = loc;
                // Don't fire events
                return;
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) {
                offset = new Point(-e.X, -e.Y);
                Capture = true;
                // Don't fire events
                return;
            }
            else if (e.Button == MouseButtons.Right) {
                Close();
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && Capture) {
                Capture = false;
                // Don't fire events
                return;
            }

            base.OnMouseUp(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (ActivateClient) {
                // Let's be sure the client is active
                SetForegroundWindow(Client.HWND);
                // And if the client is active, we are top-most window
                SetTopMost(true);
            }
        }

        #endregion

        #region Cursors helper

        [DllImport("user32.dll")]
        private static extern IntPtr LoadCursorFromFile(string fileName);

        protected static Cursor LoadCursor(string filename)
        {
            IntPtr colorCursorHandle = LoadCursorFromFile(filename);
            return new Cursor(colorCursorHandle);
        }

        #endregion

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg) {
                case WM_KEYDOWN:
                case WM_KEYUP:
                case WM_SYSKEYDOWN:
                case WM_SYSKEYUP:
                    // Forward message
                    Client.Window.SendMessage(m.Msg, m.WParam.ToInt32(), m.LParam.ToInt32());
                    // Dont process it further
                    m.Result = IntPtr.Zero;
                    return;
            }

            base.WndProc(ref m);

        }
    }
}
