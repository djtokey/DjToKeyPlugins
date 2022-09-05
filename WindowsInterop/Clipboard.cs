using Ktos.DjToKey.Plugins.Scripts;
using System.ComponentModel.Composition;
using System.Diagnostics;

namespace Ktos.DjToKey.Plugins.WindowsInterop
{
    [Export(typeof(IScriptObject))]
    public class Clipboard : IScriptObject
    {
        private const string objName = "Clipboard";

        public string Name
        {
            get { return objName; }
        }

        public object Object
        {
            get { return w; }
        }

        private ClipboardImpl w;

        public Clipboard()
        {
            w = new ClipboardImpl();
        }
    }

    public class ClipboardImpl
    {
        public void SetText(string text)
        {
            System.Windows.Forms.Clipboard.SetText(text);
        }

        public string GetText()
        {
            return System.Windows.Forms.Clipboard.GetText();
        }
    }
}
