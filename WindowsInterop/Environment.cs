using Ktos.DjToKey.Plugins.Scripts;
using System;
using System.ComponentModel.Composition;
using System.Diagnostics;

namespace Ktos.DjToKey.Plugins.WindowsInterop
{
    [Export(typeof(IScriptType))]
    public class File : IScriptType
    {
        public string Name => "File";

        public Type Type => typeof(System.IO.File);
    }

    [Export(typeof(IScriptType))]
    public class Directory : IScriptType
    {
        public string Name => "Directory";

        public Type Type => typeof(System.IO.Directory);
    }

    [Export(typeof(IScriptObject))]
    public class Environment : IScriptObject
    {
        private const string objName = "Environment";

        public string Name
        {
            get { return objName; }
        }

        public object Object
        {
            get { return w; }
        }

        private EnvironmentImpl w;

        public Environment()
        {
            w = new EnvironmentImpl();
        }
    }

    public class EnvironmentImpl
    {
        public string GetVariable(string name)
        {
            return System.Environment.GetEnvironmentVariable(name);
        }
    }
}
