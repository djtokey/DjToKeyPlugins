using System;
using Ktos.DjToKey.Plugins.Contracts;
using System.Runtime.InteropServices;
using System.Text;

namespace Ktos.DjToKey.Plugins.WindowsInterop
{
    class Process : IScriptObject
    {
        private const string objName = "Process";

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

        private ProcessImpl w;

        public Process()
        {
            w = new ProcessImpl();
        }
    }

    class ProcessImpl
    {
        public void Start(string path)
        {
            System.Diagnostics.Process.Start(path);
        }
    }
}
