using Ktos.DjToKey.Plugins.Scripts;
using System.ComponentModel.Composition;
using System.Diagnostics;

namespace Ktos.DjToKey.Plugins.WindowsInterop
{
    [Export(typeof(IScriptObject))]
    public class Process : IScriptObject
    {
        private const string objName = "Process";

        public string Name
        {
            get { return objName; }
        }

        public object Object
        {
            get { return w; }
        }

        private ProcessImpl w;

        public Process()
        {
            w = new ProcessImpl();
        }
    }

    public class ProcessImpl
    {
        public void Start(string path)
        {
            var psi = new ProcessStartInfo { FileName = path, UseShellExecute = true };
            System.Diagnostics.Process.Start(psi);
        }

        public void Start(string path, string args)
        {
            var psi = new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true,
                Arguments = args
            };
            System.Diagnostics.Process.Start(psi);
        }
    }
}
