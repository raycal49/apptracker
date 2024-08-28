using ApplicationTracker.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTracker.Models.Interfaces
{
    public interface IIdleDetectHelper
    {
        public int GetSystemUptime();
        public bool WrapperGetLastInputInfo(ref LASTINPUTINFO plii);
        public bool TryWrapperGetLastInputInfo(out LASTINPUTINFO lastInputInfo);
    }
}
