using ApplicationTracker.Models;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ApplicationTracker.Utilities
{
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