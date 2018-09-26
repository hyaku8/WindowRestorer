using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Yukari
{
    public class WindowSetter
    {
        [DllImport("USER32.DLL")]
        private static extern bool MoveWindow(IntPtr hwnd, int x, int y, int w, int h, bool repaint);

        [DllImport("USER32.DLL")]
        private static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowTextLength(IntPtr IntPtr);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        [DllImport("USER32.DLL")]
        private static extern bool IsWindowVisible(IntPtr IntPtr);

        [DllImport("USER32.DLL")]
        private static extern bool GetWindowPlacement(IntPtr hwnd, ref tagWINDOWPLACEMENT tagWINDOWPLACEMENT);

        [DllImport("USER32.DLL")]
        private static extern IntPtr GetShellWindow();

        [DllImport("USER32.DLL")]
        private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

        private delegate bool EnumWindowsProc(IntPtr IntPtr, int lParam);


        public System.Timers.Timer Timer { get; set; }


        static WindowSetter()
        {
            instance = new WindowSetter();
            Microsoft.Win32.SystemEvents.DisplaySettingsChanging += instance.onDisplayChanging;

        }
        private static readonly WindowSetter instance;
        public static WindowSetter Instance
        {
            get
            {
                return instance;
            }
        }

        const int SW_SHOWMAXIMIZED = 3;

        private IDictionary<IntPtr, SavedPosition> windows = new Dictionary<IntPtr, SavedPosition>();
        private class SavedPosition
        {
            public Rect Rect { get; set; }
            public bool Maximized { get; set; }
        }

        struct tagWINDOWPLACEMENT
        {
            public uint length;
            public uint flags;
            public uint showCmd;
            public Point ptMinPosition;
            public Point ptMaxPosition;
            public Rect rcNormalPosition;
            public Rect rcDevice;
        }


        ~WindowSetter()
        {
            try
            {
                Microsoft.Win32.SystemEvents.DisplaySettingsChanging -= this.onDisplayChanging;
                Timer?.Stop();
                Timer?.Dispose();
            }
            catch (Exception e) { }
        }

    
        public void RestoreWindows()
        {
            foreach (KeyValuePair<IntPtr, SavedPosition> w in this.windows)
            {
                Rect rect = w.Value.Rect;
                int x = rect.Left;
                int y = rect.Top;
                MoveWindow(w.Key, x, y, -1 *(x - rect.Right), -1 * (y - rect.Bottom), true);
                if (w.Value.Maximized)
                {
                    ShowWindow(w.Key, SW_SHOWMAXIMIZED);
                }
            }
            Timer?.Start();
        }

        private void onDisplayChanging(object sender, EventArgs e)
        {
            Timer?.Stop();
        }

        public void SaveWindowPositions()
        {
            IntPtr shellWindow = GetShellWindow();
            this.windows.Clear();

            EnumWindows(delegate (IntPtr _ptr, int lParam)
            {
                if (_ptr == shellWindow) return true;
                if (!IsWindowVisible(_ptr)) return true;

                int length = GetWindowTextLength(_ptr);
                if (length == 0) return true;

                Rect getRect = new Rect();
                GetWindowRect(_ptr, ref getRect);
                tagWINDOWPLACEMENT tagWINDOWPLACEMENT = new tagWINDOWPLACEMENT();
                GetWindowPlacement(_ptr, ref tagWINDOWPLACEMENT);
                SavedPosition savedPosition = new SavedPosition()
                {
                    Rect = getRect,
                    Maximized = tagWINDOWPLACEMENT.showCmd == SW_SHOWMAXIMIZED
                };
                this.windows.Add(_ptr, savedPosition);
                return true;
            }, 0);
        }
    }

    public struct Rect
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }

        public override string ToString()
        {
            return "Left: " + this.Left + " Top: " + this.Top + " Right: " + this.Right + " Bottom: " + this.Bottom;
        }
    }

    public struct Point
    {
        public int x;
        public int y;

        public override string ToString()
        {
            return "x: " + x + " y: " + y;
        }
    }
}
