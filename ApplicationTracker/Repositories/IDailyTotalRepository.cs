using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ApplicationTracker.Models;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTracker.Repositories
{
    public interface IDailyTotalRepository : IRepository<DailyTotal>
    {
        DailyTotal? FindFirstOrDef(Expression<Func<DailyTotal, bool>> predicate);
    }
}
