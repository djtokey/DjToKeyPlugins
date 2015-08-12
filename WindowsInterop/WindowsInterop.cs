using System;
using Ktos.DjToKey.Plugins.Contracts;
using System.Runtime.InteropServices;
using System.Text;

namespace Ktos.DjToKey.Plugins.WindowsInterop
{
    class WindowsInterop : IScriptObject
    {
        private const string objName = "Windows";

        public string Name
        {
            get
            {
                return objName;
            }
        }

        public object Object
        {
            get
            {
                return w;
            }
        }

        private WindowsImpl w;

        public WindowsInterop()
        {
            w = new WindowsImpl();
        }
    }

    class WindowsImpl
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        /// <summary>
        /// Returns title of active window in operating system
        /// </summary>
        /// <returns>Title of foreground window</returns>
        private string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }
    }
}
