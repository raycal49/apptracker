using ApplicationTracker.Models;

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

            ProcessWrapper activeProc = helper.WrapperGetProcessById(activeProcId);

            return activeProc.ProcessName == argProcName;
        }

    }
}