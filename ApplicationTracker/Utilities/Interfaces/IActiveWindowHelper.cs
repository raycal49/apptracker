using ApplicationTracker.Models;

namespace ApplicationTracker.Utilities.Interfaces
{
    public interface IActiveWindowHelper
    {
        nint WrapperGetForegroundWindow();
        int WrapperGetWindowThreadProcessId(nint handle, out int processId);
        ProcessWrapper WrapperGetProcessById(int processId);
    }
}