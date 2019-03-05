using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using Phoenix;
using System.IO;
using System.Timers;

namespace ScreenCapture
{
    /// <summary>
    /// Provides functions to capture the entire screen, or a particular window, and save it to a file.
    /// </summary>
    public class ScreenCapture
    {
        private Timer timer = new Timer();
        private bool fullScreen = false;
        private bool running = false;

        /// <summary>
        /// Creates an Image object containing a screen shot of the entire desktop
        /// </summary>
        /// <returns></returns>
        public Image CaptureScreen()
        {
            return CaptureWindow(User32.GetDesktopWindow());
        }

        /// <summary>
        /// Creates an Image object containing a screen shot of a specific window
        /// </summary>
        /// <param name="handle">The handle to the window. (In windows forms, this is obtained by the Handle property)</param>
        /// <returns></returns>
        public Image CaptureWindow(IntPtr handle)
        {
            // get te hDC of the target window
            IntPtr hdcSrc = User32.GetWindowDC(handle);
            // get the size
            User32.RECT windowRect = new User32.RECT();
            User32.GetWindowRect(handle, ref windowRect);
            int width = windowRect.right - windowRect.left;
            int height = windowRect.bottom - windowRect.top;
            // create a device context we can copy to
            IntPtr hdcDest = GDI32.CreateCompatibleDC(hdcSrc);
            // create a bitmap we can copy it to,
            // using GetDeviceCaps to get the width/height
            IntPtr hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc, width, height);
            // select the bitmap object
            IntPtr hOld = GDI32.SelectObject(hdcDest, hBitmap);
            // bitblt over
            GDI32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, GDI32.SRCCOPY);
            // restore selection
            GDI32.SelectObject(hdcDest, hOld);
            // clean up 
            GDI32.DeleteDC(hdcDest);
            User32.ReleaseDC(handle, hdcSrc);

            // get a .NET image object for it
            Image img = Image.FromHbitmap(hBitmap);
            // free up the Bitmap object
            GDI32.DeleteObject(hBitmap);

            return img;
        }

        public Image CaptureWindow(IntPtr handle, int width, int height)
        {
            //width += 3;
            //height += 20;
            // get te hDC of the target window
            IntPtr hdcSrc = User32.GetWindowDC(handle);
            // get the size
            User32.RECT windowRect = new User32.RECT();
            User32.GetWindowRect(handle, ref windowRect);
            // create a device context we can copy to
            IntPtr hdcDest = GDI32.CreateCompatibleDC(hdcSrc);
            // create a bitmap we can copy it to,
            // using GetDeviceCaps to get the width/height
            IntPtr hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc, width, height);
            // select the bitmap object
            IntPtr hOld = GDI32.SelectObject(hdcDest, hBitmap);
            // bitblt over
            GDI32.BitBlt(hdcDest, -5, -22, width + 5, height + 22, hdcSrc, 0, 0, GDI32.SRCCOPY);
            // restore selection
            GDI32.SelectObject(hdcDest, hOld);
            // clean up 
            GDI32.DeleteDC(hdcDest);
            User32.ReleaseDC(handle, hdcSrc);

            // get a .NET image object for it
            Image img = Image.FromHbitmap(hBitmap);
            // free up the Bitmap object
            GDI32.DeleteObject(hBitmap);

            return img;
        }

        /// <summary>
        /// Captures a screen shot of a specific window, and saves it to a file
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="filename"></param>
        /// <param name="format"></param>
        public void CaptureWindowToFile(IntPtr handle, string filename, ImageFormat format)
        {
            Image img = CaptureWindow(handle);
            img.Save(filename, format);
        }


        /// <summary>
        /// Captures a screen shot of the entire desktop, and saves it to a file
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="format"></param>
        public void CaptureWindowRectToFile(IntPtr handle, string filename, ImageFormat format, int width, int height)
        {
            Image img = CaptureWindow(handle, width, height);
            img.Save(filename, format);
        }

        /// <summary>
        /// Captures a screen shot of the entire desktop, and saves it to a file
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="format"></param>
        public void CaptureScreenToFile(string filename, ImageFormat format)
        {
            Image img = CaptureScreen();
            img.Save(filename, format);
        }

        /// <summary>
        /// Helper class containing Gdi32 API functions
        /// </summary>
        private class GDI32
        {

            public const int SRCCOPY = 0x00CC0020; // BitBlt dwRop parameter

            [DllImport("gdi32.dll")]
            public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest,
                int nWidth, int nHeight, IntPtr hObjectSource,
                int nXSrc, int nYSrc, int dwRop);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth,
                int nHeight);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteObject(IntPtr hObject);
            [DllImport("gdi32.dll")]
            public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
        }

        /// <summary>
        /// Helper class containing User32 API functions
        /// </summary>
        private class User32
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }

            [DllImport("user32.dll")]
            public static extern IntPtr GetDesktopWindow();
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowDC(IntPtr hWnd);
            [DllImport("user32.dll")]
            public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);

        }

        [Command]
        public void snap()
        {
            string obrName = "";
            int lastObr = 0;
            string lastObrStr = "";
            string name;
            int obrNum;
            DirectoryInfo dir = new DirectoryInfo(Phoenix.Core.Directory + "\\Obrazky");
            if (!dir.Exists) dir.Create();
            FileInfo[] obrazky = dir.GetFiles();
            foreach (FileInfo info in obrazky)
            {
                name = info.Name;
                if (name.Contains("Obrazek"))
                    try
                    {
                        obrNum = Convert.ToInt32(name.Substring(7, 5));
                        if (lastObr < obrNum)
                        {
                            lastObr = obrNum;
                        }
                    }
                    catch
                    {
                    }
            }
            lastObr++;
            for (int i = lastObr.ToString().Length; i < 5; i++)
            {
                lastObrStr += "0";
            }
            lastObrStr += lastObr.ToString();
            obrName = "Obrazek" + lastObrStr + ".jpg";
            ScreenCapture sc = new ScreenCapture();
            sc.CaptureWindowToFile(Client.HWND, Phoenix.Core.Directory + "\\Obrazky\\" + obrName, ImageFormat.Jpeg);
            UO.PrintWarning("Screenshot: " + obrName);
        }

        [Command]
        public void snap(string filename)
        {
            string obrName = "";
            int lastObr = 0;
            string lastObrStr = "";
            string name;
            int obrNum;
            DirectoryInfo dir = new DirectoryInfo(Phoenix.Core.Directory + "\\Obrazky");
            if (!dir.Exists) dir.Create();
            FileInfo[] obrazky = dir.GetFiles();
            foreach (FileInfo info in obrazky)
            {
                name = info.Name;
                if (name.Contains(filename))
                    try
                    {
                        obrNum = Convert.ToInt32(name.Substring(filename.Length, 5));
                        if (lastObr < obrNum)
                        {
                            lastObr = obrNum;
                        }
                    }
                    catch
                    {
                    }
            }
            lastObr++;
            for (int i = lastObr.ToString().Length; i < 5; i++)
            {
                lastObrStr += "0";
            }
            lastObrStr += lastObr.ToString();
            obrName = filename + lastObrStr + ".jpg";
            ScreenCapture sc = new ScreenCapture();
            sc.CaptureWindowToFile(Client.HWND, Phoenix.Core.Directory + "\\Obrazky\\" + obrName, ImageFormat.Jpeg);
            UO.PrintWarning("Screenshot: " + obrName);
        }

        [Command]
        public void snapScreen()
        {
            string obrName = "";
            int lastObr = 0;
            string lastObrStr = "";
            string name;
            int obrNum;
            DirectoryInfo dir = new DirectoryInfo(Phoenix.Core.Directory + "\\Obrazky");
            if (!dir.Exists) dir.Create();
            FileInfo[] obrazky = dir.GetFiles();
            foreach (FileInfo info in obrazky)
            {
                name = info.Name;
                if (name.Contains("Obrazek"))
                    try
                    {
                        obrNum = Convert.ToInt32(name.Substring(7, 5));
                        if (lastObr < obrNum)
                        {
                            lastObr = obrNum;
                        }
                    }
                    catch
                    {
                    }
            }
            lastObr++;
            for (int i = lastObr.ToString().Length; i < 5; i++)
            {
                lastObrStr += "0";
            }
            lastObrStr += lastObr.ToString();
            obrName = "Obrazek" + lastObrStr + ".jpg";
            ScreenCapture sc = new ScreenCapture();
            sc.CaptureWindowRectToFile(Client.HWND, Phoenix.Core.Directory + "\\Obrazky\\" + obrName, ImageFormat.Jpeg, 800, 600);
            UO.PrintWarning("Screenshot: " + obrName);
        }

        [Command]
        public void autosnap()
        {
            if (running)
            {
                timer.Elapsed -= timer_Elapsed;
                running = false;
                UO.PrintWarning("Autosnap vypnut!");
            }
            else
            {
                UO.PrintWarning("Autosnap se zapina ,autosnap interval true/false! [true - fullscreen, false - vyrez hry (800x600)]");
            }
        }

        [Command]
        public void autosnap(int interval, bool fullscreen)
        {
            if (running)
            {
                timer.Elapsed -= timer_Elapsed;
                running = false;
                UO.PrintWarning("Autosnap vypnut!");
            }
            else
            {
                timer.Interval = interval;
                this.fullScreen = fullscreen;
                timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
                timer.Start();
                running = true;
                UO.PrintWarning("Autosnap zapnut!");
            }
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (fullScreen)
            {
                snap();
            }
            else
            {
                snapScreen();
            }
        }
    }
}

