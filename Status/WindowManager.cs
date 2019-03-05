using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.Runtime.InteropServices;
using System.Threading;
using Phoenix.Runtime;
using System.Reflection;
using Phoenix.Gui;
using Phoenix;
using System.Runtime.CompilerServices;

namespace CalExtension.UI.Status
{
    /// <summary>
    /// Creates new window.
    /// </summary>
    /// <returns>Newly created window.</returns>
    public delegate FloatingWindow WindowBuilder();

    /// <summary>
    /// Starts its own ui thread and manages it's windows.
    /// </summary>
    /// <remarks>
    /// To add new window, use <see cref="CreateWindow"/> method.
    /// </remarks>
    public class WindowManager
    {
        #region Native functions

        [StructLayout(LayoutKind.Sequential)]
        struct MSG
        {
            IntPtr hwnd;
            uint message;
            IntPtr wParam;
            IntPtr lParam;
            uint time;
            int ptX;
            int ptY;
        }

        const int PM_NOREMOVE = 0;
        const int PM_REMOVE = 1;


        [DllImport("user32.dll")]
        [SuppressUnmanagedCodeSecurityAttribute]
        static extern bool GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

        [DllImport("user32.dll")]
        [SuppressUnmanagedCodeSecurityAttribute]
        static extern bool PeekMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, int wRemoveMsg);

        [DllImport("user32.dll")]
        [SuppressUnmanagedCodeSecurityAttribute]
        static extern bool TranslateMessage(ref MSG lpmsg);

        [DllImport("user32.dll")]
        [SuppressUnmanagedCodeSecurityAttribute]
        static extern IntPtr DispatchMessage(ref MSG lpmsg);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        [SuppressUnmanagedCodeSecurityAttribute]
        static extern bool PostThreadMessage(int threadId, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32")]
        [SuppressUnmanagedCodeSecurityAttribute]
        static extern int GetCurrentThreadId();

        #endregion

        private readonly Thread thread;
        private readonly List<FloatingWindow> forms = new List<FloatingWindow>();
        private readonly List<ThreadStart> requests = new List<ThreadStart>();
        private bool run = true;
        private int threadId = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:StatusBar"/> class.
        /// </summary>
        public WindowManager()
        {
            thread = new Thread(MessagePump);
            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            Core.Disconnected += new EventHandler(Core_Disconnected);
            RuntimeCore.UnregisteringAssembly += new UnregisteringAssemblyEventHandler(RuntimeCore_UnregisteringAssembly);
        }

        #region Properties

        /// <summary>
        /// Gets whether window thread is active.
        /// </summary>
        public bool Running
        {
            get { return run; }
        }

        /// <summary>
        /// Gets whether <see cref="BeginInvoke"/> or <see cref="Invoke"/> are required.
        /// </summary>
        public bool InvokeRequired
        {
            get { return threadId != GetCurrentThreadId(); }
        }

        /// <summary>
        /// Gets count of the owned windows.
        /// </summary>
        public int WindowCount
        {
            get { return forms.Count; }
        }

        /// <summary>
        /// Gets enumeration of all owned windows. Don't manipulate them from other then manager thread.
        /// </summary>
        public IEnumerable<FloatingWindow> OwnedWindows
        {
            get { return GetFormsSafe(); }
        }

        #endregion

        #region Implementation

        private FloatingWindow[] GetFormsSafe()
        {
            lock (forms) {
                return forms.ToArray();
            }
        }

        private void MessagePump()
        {
            threadId = GetCurrentThreadId();

            MSG m;

            // Create message queue
            PeekMessage(out m, IntPtr.Zero, 0, 0, PM_NOREMOVE);

            // Message pump
            while (run && GetMessage(out m, IntPtr.Zero, 0, 0)) {
                TranslateMessage(ref m);
                DispatchMessage(ref m);

                // Execute requests
                if (requests.Count > 0) {
                    // Get safely array
                    ThreadStart[] arr;
                    lock (requests) {
                        arr = requests.ToArray();
                        requests.Clear();
                    }

                    // Execute
                    foreach (var r in arr) {
                        try {
                            r();
                        }
                        catch (Exception e) {
                            ExceptionDialog.Show(e, "Unhandled exception in the WindowManager thread.");
                        }
                    }
                }
            }

            run = false;

            // Let's dispose all forms in good manner
            foreach (var f in GetFormsSafe()) {
                try {
                    f.Dispose();
                }
                catch { }
            }
        }

        void RuntimeCore_UnregisteringAssembly(object sender, UnregisteringAssemblyEventArgs e)
        {
            if (e.Assembly == Assembly.GetExecutingAssembly()) {
                // Stop thread
                Stop();
            }
        }

        void win_Disposed(object sender, EventArgs e)
        {
            lock (forms) {
                forms.Remove((FloatingWindow)sender);
            }
        }

        void Core_Disconnected(object sender, EventArgs e)
        {
            // Close all windows
            BeginInvoke(delegate()
            {
                foreach (var f in GetFormsSafe()) {
                    try {
                        f.Dispose();
                    }
                    catch { }
                }
            });
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Invokes callback asynchronously on the window thread.
        /// </summary>
        /// <param name="callback">Callback method.</param>
        public void BeginInvoke(ThreadStart callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (!run)
                throw new InvalidOperationException("Window manager is not running.");

            lock (requests) {
                requests.Add(callback);
            }

            // Poke message pump
            PostThreadMessage(threadId, 0, IntPtr.Zero, IntPtr.Zero);
        }

        /// <summary>
        /// Invokes callback synchronously on the window thread.
        /// </summary>
        /// <param name="callback">Callback method.</param>
        public void Invoke(ThreadStart callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (!run)
                throw new InvalidOperationException("Window manager is not running.");

            // Execute immediatly on the gui thread
            if (!InvokeRequired) {
                callback();
                return;
            }

            // Invoke callback
            ManualResetEvent ev = new ManualResetEvent(false);
            Exception ex = null;

            BeginInvoke(delegate()
            {
                try {
                    // Invoke real callback
                    callback();
                }
                catch (Exception e) {
                    ex = e;
                }

                // Set event
                ev.Set();
            });

            // Wait for event
            ev.WaitOne();

            if (ex != null) {
                throw new TargetInvocationException(ex);
            }
        }

        /// <summary>
        /// Creates and registers new window, that will be owned and ran by this manager.
        /// </summary>
        /// <param name="builder">Builder callback.</param>
        /// <returns>Created window. Don't forget not to manipulate it from other threads.</returns>
        public FloatingWindow CreateWindow(WindowBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException("builder");

            FloatingWindow win = null;

            Invoke(delegate()
            {
                win = builder();

                if (win != null) {
                    // Add to list
                    lock (forms) {
                        forms.Add(win);
                    }

                    // Add callback
                    win.Disposed += new EventHandler(win_Disposed);

                    // Show
                    win.Show();
                }
            });

            return win;
        }

        /// <summary>
        /// Stops the message pump.
        /// </summary>
        public void Stop()
        {
            if (run) {
                run = false;
                PostThreadMessage(threadId, 0, IntPtr.Zero, IntPtr.Zero);
            }
        }

        #endregion

        #region Default instance

        private static WindowManager defaultManager;

        public static WindowManager GetDefaultManager()
        {
            if (defaultManager == null) {
                // Enter synchronized context
                lock (typeof(WindowManager)) {
                    // Is it still null?
                    if (defaultManager == null) {
                        // Create new instance
                        defaultManager = new WindowManager();
                    }
                }
            }

            return defaultManager;
        }

        #endregion
    }
}
