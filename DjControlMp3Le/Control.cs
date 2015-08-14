using Ktos.DjToKey.Plugins.Scripts;
using Midi;
using System;

namespace Ktos.DjToKey.Plugins.DjControlMp3Le
{
    public enum Button
    {
        Magic = 46,
        Vinyl = 45,
        Up = 41,
        Down = 42,
        Load = 44,
        Files = 43,

        ListenA = 16,
        LoadA = 17,
        LoadB = 37,
        ListenB = 36,

        RevB = 32,
        FfvB = 33,
        CueB = 34,
        EjectB = 35,

        PitchBMinus = 30,
        PitchBPlus = 31,
        PitchBSync = 38,
        PitchBReset = 39,

        EffectB1 = 21,
        EffectB2 = 22,
        EffectB3 = 23,
        EffectB4 = 24,
        EffectB5 = 25,
        EffectB6 = 26,
        EffectB7 = 27,
        EffectB8 = 28,

        RevA = 12,
        FfvA = 13,
        CueA = 14,
        EjectA = 15,

        PitchAMinus = 10,
        PitchAPlus = 11,
        PitchASync = 18,
        PitchAReset = 19,

        EffectA1 = 1,
        EffectA2 = 2,
        EffectA3 = 3,
        EffectA4 = 4,
        EffectA5 = 5,
        EffectA6 = 6,
        EffectA7 = 7,
        EffectA8 = 8
    }

    public class DjButton : IScriptType
    {
        private const string objName = "DjButton";

        public string Name
        {
            get
            {
                return objName;
            }
        }

        public Type Type
        {
            get
            {
                return typeof(Button);
            }
        }
    }

    public class DjControl : IScriptObject
    {
        private const string objName = "DjControl";

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

        private DjControlMp3LeImpl w;

        public DjControl()
        {
            try
            {
                w = new DjControlMp3LeImpl();
            }
            catch (Exception)
            {
                w = null;
            }
        }
    }

    public class DjControlMp3LeImpl : IDisposable
    {
        private readonly OutputDevice device;

        public DjControlMp3LeImpl()
        {
            device = null;
            foreach (var d in OutputDevice.InstalledDevices)
            {
                if (d.Name == "DJControl MP3 LE MIDI")
                    device = d;
            }

            if (device == null)
                throw new Exception("No DjControl MP3 LE device available");

            device.Open();
        }

        public void TurnAllOff()
        {
            for (byte i = 1; i < 95; i++)
            {
                turnOff(i);
            }
        }

        public void TurnOn(Button button)
        {
            turnOn((byte)button);
        }

        public void TurnOnBlink(Button button)
        {
            turnOn((byte)(button + 48));
            //turnOn(66);
        }

        public void TurnOff(Button button)
        {
            turnOff((byte)button);
        }

        private void turnOn(byte number)
        {
            device.SendShortMsg(new byte[] { 0x90, number, 127 });
        }

        private void turnOff(byte number)
        {
            device.SendShortMsg(new byte[] { 0x90, number, 0 });
        }

        public void Dispose()
        {
            if (device != null) device.Close();
        }
    }
}