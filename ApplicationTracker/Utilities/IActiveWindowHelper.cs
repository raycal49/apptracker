using ApplicationTracker.Models;

namespace ApplicationTracker.Utilities
{
    public interface IActiveWindowHelper
    {
        //IntPtr GetForegroundWindow();
        nint WrapperGetForegroundWindow();

        //int GetWindowThreadProcessId(IntPtr handle, out int processId);
        int WrapperGetWindowThreadProcessId(nint handle, out int processId);

        ProcessWrapper WrapperGetProcessById(int processId);

        //bool GetLastInputInfo(ref LASTINPUTINFO plii);

        //bool IsActive(string argProcName);
    }
}