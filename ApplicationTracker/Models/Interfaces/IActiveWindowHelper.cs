namespace ApplicationTracker.Models.Interfaces
{
    public interface IActiveWindowHelper
    {
        nint WrapperGetForegroundWindow();
        int WrapperGetWindowThreadProcessId(nint handle, out int processId);
        ProcessWrapper WrapperGetProcessById(int processId);
    }
}