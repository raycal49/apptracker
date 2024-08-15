using ApplicationTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTracker.Repositories
{
    public interface IProcessDataRepository : IRepository<ProcessData>
    {
        ProcessData? FindFirstOrDef(Expression<Func<ProcessData, bool>> predicate);
    }
}
