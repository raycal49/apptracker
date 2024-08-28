using System.Linq.Expressions;
using ApplicationTracker.Models;

namespace ApplicationTracker.Repositories
{
    public interface IProcessDataRepository : IRepository<ProcessData>
    {
        ProcessData? FindFirstOrDef(Expression<Func<ProcessData, bool>> predicate);
    }
}
