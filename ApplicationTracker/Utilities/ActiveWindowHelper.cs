using ApplicationTracker.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTracker.Utilities
{
    // probably need to have the below wrapper implement IDisposable
    // To properly get rid of pointers.
    public class ActiveWindowHelper : IActiveWindowHelper
    {

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern nint GetForegroundWindow();

        public nint WrapperGetForegroundWindow()
        {
            return GetForegroundWindow();
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowThreadProcessId(nint handle, out int processId);

        public int WrapperGetWindowThreadProcessId(nint handle, out int processId)
        {
            return GetWindowThreadProcessId(handle, out processId);
        }

        public ProcessWrapper WrapperGetProcessById(int processId)
        {
            Process temp = Process.GetProcessById(processId);
            ProcessWrapper process = new ProcessWrapper() { ProcessName = temp.ProcessName };
            return process;
        }
    }
}