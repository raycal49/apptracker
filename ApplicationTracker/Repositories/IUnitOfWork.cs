using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTracker.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IDailyTotalRepository DailyTotals { get;  }

        int Complete();
    }
}
