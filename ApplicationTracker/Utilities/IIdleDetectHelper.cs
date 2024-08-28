
namespace ApplicationTracker.Utilities
{
    public interface IIdleDetectHelper
    {
        public int GetSystemUptime();

        public bool WrapperGetLastInputInfo(ref LASTINPUTINFO plii);

        public bool TryWrapperGetLastInputInfo(out LASTINPUTINFO lastInputInfo);
    }
}
