using ApplicationTracker.Models;
using DetectLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTracker
{
    // probably need to have the below wrapper implement IDisposable
    // To properly get rid of pointers.
    public class ActiveWindowHelper : IActiveWindowHelper
    {

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();

        public IntPtr WrapperGetForegroundWindow()
        {
            //IntPtr temp = GetForegroundWindow();

            //return temp;

            return GetForegroundWindow();
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

        public int WrapperGetWindowThreadProcessId(IntPtr handle, out int processId)
        {
            return GetWindowThreadProcessId(handle, out processId);
        }

        public MyProcess WrapperGetProcessById(int processId)
        {
            Process temp = Process.GetProcessById(processId);
            MyProcess process = new MyProcess() { ProcessName = temp.ProcessName };
            //return Process.GetProcessById(processId);
            return process;
        }

        /*
        [DllImport("user32.dll")]
        static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        
        public bool GetLastInputInfo(ref LASTINPUTINFO plii)
        {
            return GetLastInputInfo(ref plii);
        }
        */

    }
}