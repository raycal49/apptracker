using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DetectLibrary
{
    public static class ProcessUtilities
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

        // Check to see if the process at the foreground and the argument process are the same
        // If they are, return true, else false.
        public static bool IsActive(string argProcName)
        {
            var activatedHandle = GetForegroundWindow();

            if (activatedHandle == IntPtr.Zero)
            {
                return false;       // No window is currently activated
            }

            int activeProcId;

            GetWindowThreadProcessId(activatedHandle, out activeProcId);

            Process activeProc = Process.GetProcessById(activeProcId);

            return activeProc.ProcessName == argProcName;
        }
    }

    public static class IdleTimeDetector
    {
        [DllImport("user32.dll")]
        static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        public static IdleTimeInfo GetIdleTimeInfo()
        {
            int systemUptime = Environment.TickCount,
                lastInputTicks = 0,
                idleTicks = 0;

            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);
            lastInputInfo.dwTime = 0;

            if (GetLastInputInfo(ref lastInputInfo))
            {
                lastInputTicks = (int)lastInputInfo.dwTime;

                idleTicks = systemUptime - lastInputTicks;
            }

            return new IdleTimeInfo
            {
                LastInputTime = DateTime.Now.AddMilliseconds(-1 * idleTicks),
                IdleTime = new TimeSpan(0, 0, 0, 0, idleTicks),
                SystemUptimeMilliseconds = systemUptime,
            };
        }
    }

    public class IdleTimeInfo
    {
        public DateTime LastInputTime { get; internal set; }

        public TimeSpan IdleTime { get; internal set; }

        public int SystemUptimeMilliseconds { get; internal set; }
    }

    internal struct LASTINPUTINFO
    {
        public uint cbSize;
        public uint dwTime;
    }
}