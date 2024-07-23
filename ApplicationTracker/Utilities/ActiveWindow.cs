using ApplicationTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTracker.Utilities
{
    public class ActiveWindow
    {
        IActiveWindowHelper helper;

        public ActiveWindow() { }

        public ActiveWindow(IActiveWindowHelper helper)
        {
            this.helper = helper;
        }

        public virtual bool IsActive(string argProcName)
        {

            nint activatedHandle = helper.WrapperGetForegroundWindow();

            if (activatedHandle == nint.Zero)
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