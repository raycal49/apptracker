using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using ApplicationTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTracker.Repositories
{
    public class DailyTotalRepository : Repository<DailyTotal>, IDailyTotalRepository
    {
        public DailyTotalRepository(DbContext context) : base(context)
        {

        }

        DailyTotal? IDailyTotalRepository.FindFirstOrDef(Expression<Func<DailyTotal, bool>> predicate)
        {
            return Context.Set<DailyTotal>().FirstOrDefault(predicate);
        }
    }
}
