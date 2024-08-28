using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

// Dispose of IntPtr peoperly.
namespace ApplicationTracker.Utilities
{
    public class IdleDetect
    {
        public virtual IdleTimeInfo GetIdleTimeInfo(IIdleDetectHelper idleDetectHelper)
        {
            int systemUptime = idleDetectHelper.GetSystemUptime(),
                lastInputTicks = 0,
                idleTicks = 0;

            if (idleDetectHelper.TryWrapperGetLastInputInfo(out LASTINPUTINFO lastInputInfo))
            {
                lastInputTicks = (int)lastInputInfo.dwTime;

                idleTicks = systemUptime - lastInputTicks;
            }

            IdleTimeInfo idleTime = new IdleTimeInfo(idleTicks, systemUptime);

            return idleTime;
        }
    }

    public class IdleTimeInfo
    {
        public IdleTimeInfo(int idleTicks, int systemUptime)
        {
            LastInputTime = DateTime.Now.AddMilliseconds(-1 * idleTicks);
            IdleTime = new TimeSpan(0, 0, 0, 0, idleTicks);
            SystemUptimeMilliseconds = systemUptime;
        }

        public DateTime LastInputTime { get; internal set; }

        public TimeSpan IdleTime { get; internal set; }

        public int SystemUptimeMilliseconds { get; internal set; }
    }

    public struct LASTINPUTINFO
    {
        public uint cbSize;
        public uint dwTime;
    }
}