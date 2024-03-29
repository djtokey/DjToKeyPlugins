﻿// Copyright (c) 2009, Tom Lokovic
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//
//     * Redistributions of source code must retain the above copyright notice,
//       this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
// POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections.ObjectModel;
using System.Text;

namespace Midi
{
    /// <summary>
    /// A MIDI output device.
    /// </summary>
    /// <remarks>
    /// <para>Each instance of this class describes a MIDI output device installed on the system.
    /// You cannot create your own instances, but instead must go through the
    /// <see cref="InstalledDevices"/> property to find which devices are available.  You may wish
    /// to examine the <see cref="DeviceBase.Name"/> property of each one and present the user with
    /// a choice of which device to use.
    /// </para>
    /// <para>Open an output device with <see cref="Open"/> and close it with <see cref="Close"/>.
    /// While it is open, you may send MIDI messages with functions such as
    /// <see cref="SendNoteOn"/>, <see cref="SendNoteOff"/> and <see cref="SendProgramChange"/>.
    /// All notes may be silenced on the device by calling <see cref="SilenceAllNotes"/>.</para>
    /// <para>Note that the above methods send their messages immediately.  If you wish to arrange
    /// for a message to be sent at a specific future time, you'll need to instantiate some subclass
    /// of <see cref="Message"/> (eg <see cref="NoteOnMessage"/>) and then pass it to
    /// <see cref="Clock.Schedule(Midi.Message)">Clock.Schedule</see>.
    /// </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    /// <seealso cref="Clock"/>
    /// <seealso cref="InputDevice"/>
    public class OutputDevice : DeviceBase
    {
        #region Public Methods and Properties

        /// <summary>
        /// List of devices installed on this system.
        /// </summary>
        public static ReadOnlyCollection<OutputDevice> InstalledDevices
        {
            get
            {
                lock (staticLock)
                {
                    if (installedDevices == null)
                    {
                        installedDevices = MakeDeviceList();
                    }
                    return new ReadOnlyCollection<OutputDevice>(installedDevices);
                }
            }
        }

        /// <summary>
        /// True if this device is open.
        /// </summary>
        public bool IsOpen
        {
            get
            {
                lock (this)
                {
                    return isOpen;
                }
            }
        }

        /// <summary>
        /// Opens this output device.
        /// </summary>
        /// <exception cref="InvalidOperationException">The device is already open.</exception>
        /// <exception cref="DeviceException">The device cannot be opened.</exception>
        public void Open()
        {
            lock (this)
            {
                CheckNotOpen();
                CheckReturnCode(Win32API.midiOutOpen(out handle, deviceId, null, (UIntPtr)0));
                isOpen = true;
            }
        }

        /// <summary>
        /// Closes this output device.
        /// </summary>
        /// <exception cref="InvalidOperationException">The device is not open.</exception>
        /// <exception cref="DeviceException">The device cannot be closed.</exception>
        public void Close()
        {
            lock (this)
            {
                CheckOpen();
                CheckReturnCode(Win32API.midiOutClose(handle));
                isOpen = false;
            }
        }              

        #region ShortMsg
        public void SendShortMsg(byte[] message)
        {
            lock (this)
            {
                CheckOpen();

                const int StatusMask = 16776960;
                const int Data1Mask = 16711935;
                const int Data2Mask = 65535;

                int m = 0;

                m &= StatusMask;
                m |= message[0];

                m &= Data1Mask;
                m |= message[1] << 8;

                m &= Data2Mask;
                m |= message[2] << 16;           

                //UInt32 m = (UInt32)(message[0] | message[1] >> 8 | message[2] >> 16);
                CheckReturnCode(Win32API.midiOutShortMsg(handle, (UInt32)m));
            }
        }
        #endregion
        

        #endregion

        #region Private Methods

        /// <summary>
        /// Makes sure rc is MidiWin32Wrapper.MMSYSERR_NOERROR.  If not, throws an exception with an
        /// appropriate error message.
        /// </summary>
        /// <param name="rc"></param>
        private static void CheckReturnCode(Win32API.MMRESULT rc)
        {
            if (rc != Win32API.MMRESULT.MMSYSERR_NOERROR)
            {
                StringBuilder errorMsg = new StringBuilder(128);
                rc = Win32API.midiOutGetErrorText(rc, errorMsg);
                if (rc != Win32API.MMRESULT.MMSYSERR_NOERROR)
                {
                    throw new DeviceException("no error details");
                }
                throw new DeviceException(errorMsg.ToString());
            }
        }

        /// <summary>
        /// Throws a MidiDeviceException if this device is not open.
        /// </summary>
        private void CheckOpen()
        {
            if (!isOpen)
            {
                throw new InvalidOperationException("device not open");
            }
        }

        /// <summary>
        /// Throws a MidiDeviceException if this device is open.
        /// </summary>
        private void CheckNotOpen()
        {
            if (isOpen)
            {
                throw new InvalidOperationException("device open");
            }
        }

        /// <summary>
        /// Private Constructor, only called by the getter for the InstalledDevices property.
        /// </summary>
        /// <param name="deviceId">Position of this device in the list of all devices.</param>
        /// <param name="caps">Win32 Struct with device metadata</param>
        private OutputDevice(UIntPtr deviceId, Win32API.MIDIOUTCAPS caps)
            : base(caps.szPname)
        {
            this.deviceId = deviceId;
            this.caps = caps;
            this.isOpen = false;
        }

        /// <summary>
        /// Private method for constructing the array of MidiOutputDevices by calling the Win32 api.
        /// </summary>
        /// <returns></returns>
        private static OutputDevice[] MakeDeviceList()
        {
            uint outDevs = Win32API.midiOutGetNumDevs();
            OutputDevice[] result = new OutputDevice[outDevs];
            for (uint deviceId = 0; deviceId < outDevs; deviceId++)
            {
                Win32API.MIDIOUTCAPS caps = new Win32API.MIDIOUTCAPS();
                Win32API.midiOutGetDevCaps((UIntPtr)deviceId, out caps);
                result[deviceId] = new OutputDevice((UIntPtr)deviceId, caps);
            }
            return result;
        }

        #endregion

        #region Private Fields

        // Access to the global state is guarded by lock(staticLock).
        private static Object staticLock = new Object();
        private static OutputDevice[] installedDevices = null;

        // The fields initialized in the constructor never change after construction,
        // so they don't need to be guarded by a lock.
        private UIntPtr deviceId;
        private Win32API.MIDIOUTCAPS caps;

        // Access to the Open/Close state is guarded by lock(this).
        private bool isOpen;
        private Win32API.HMIDIOUT handle;

        #endregion
    }
}
