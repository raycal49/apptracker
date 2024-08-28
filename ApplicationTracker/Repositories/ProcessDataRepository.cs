using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using ApplicationTracker.Models;

namespace ApplicationTracker.Repositories
{
    public class ProcessDataRepository : Repository<ProcessData>, IProcessDataRepository
    {
        public ProcessDataRepository(DbContext context) : base(context)
        {
        }

        public new IEnumerable<ProcessData> Find(Expression<Func<ProcessData, bool>> predicate)
        {
            return Context.Set<ProcessData>().Where(predicate).ToList();
        }

        ProcessData? IProcessDataRepository.FindFirstOrDef(Expression<Func<ProcessData, bool>> predicate)
        {
            return Context.Set<ProcessData>().FirstOrDefault(predicate);
        }
    }
}
