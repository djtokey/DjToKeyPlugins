using Ktos.DjToKey.Plugins.Scripts;
using System.ComponentModel.Composition;

namespace Ktos.DjToKey.Plugins.WindowsInterop
{
    [Export(typeof(IScriptObject))]
    public class Process : IScriptObject
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

    public class ProcessImpl
    {
        public void Start(string path)
        {
            System.Diagnostics.Process.Start(path);
        }
    }
}