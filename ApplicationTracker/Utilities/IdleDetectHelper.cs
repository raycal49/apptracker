﻿using System.Runtime.InteropServices;
using ApplicationTracker.Utilities.Interfaces;

namespace ApplicationTracker.Utilities
{
    public class IdleDetectHelper : IIdleDetectHelper
    {
        public int GetSystemUptime()
        {
            return Environment.TickCount;
        }

        public bool TryWrapperGetLastInputInfo(out LASTINPUTINFO lastInputInfo)
        {
            lastInputInfo = new LASTINPUTINFO
            {
                cbSize = (uint)Marshal.SizeOf(typeof(LASTINPUTINFO)),
                dwTime = 0
            };
            return GetLastInputInfo(ref lastInputInfo);
        }

        [DllImport("user32.dll")]
        public static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        public bool WrapperGetLastInputInfo(ref LASTINPUTINFO plii)
        {
            return GetLastInputInfo(ref plii);
        }
    }
}
