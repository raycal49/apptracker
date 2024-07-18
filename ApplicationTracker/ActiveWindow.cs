using ApplicationTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTracker
{
    public class ActiveWindow
    {
        IActiveWindowHelper helper;

        public ActiveWindow(IActiveWindowHelper helper)
        {
            this.helper = helper;
        }

        public bool IsActive(string argProcName)
        {

            IntPtr activatedHandle = helper.WrapperGetForegroundWindow();

            if (activatedHandle == IntPtr.Zero)
            {
                return false;       // No window is currently activated
            }

            int activeProcId;

            helper.WrapperGetWindowThreadProcessId(activatedHandle, out activeProcId);

            MyProcess activeProc = helper.WrapperGetProcessById(activeProcId);

            //MyProcess activeProcContainer = new MyProcess() { ProcessName = activeProc.ProcessName};

            return activeProc.ProcessName == argProcName;
        }

    }
}