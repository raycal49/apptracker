using ApplicationTracker.Models;
using DetectLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTracker
{
    public interface IActiveWindowHelper
    {
        //IntPtr GetForegroundWindow();
        IntPtr WrapperGetForegroundWindow();

        //int GetWindowThreadProcessId(IntPtr handle, out int processId);
        int WrapperGetWindowThreadProcessId(IntPtr handle, out int processId);

        MyProcess WrapperGetProcessById(int processId);

        //bool GetLastInputInfo(ref LASTINPUTINFO plii);

        //bool IsActive(string argProcName);
    }
}