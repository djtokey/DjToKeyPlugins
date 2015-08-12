using System;
using Ktos.DjToKey.Plugins.Contracts;

namespace Ktos.DjToKey.Plugins.WindowsInterop
{
    class WindowsInterop : IScriptObject
    {
        public string Name
        {
            get
            {
                return "Windows";
            }
        }

        public object Object
        {
            get
            {
                return w;
            }
        }

        private Windows w;

        public WindowsInterop()
        {
            w = new Windows();
        }
    }
}
