using ApplicationTracker.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTracker.Repositories
{
    public class ProcessDataRepository : Repository<ProcessData>, IProcessDataRepository
    {
        public ProcessDataRepository(DbContext context) : base(context)
        {

        }

        public new IEnumerable<ProcessData> Find(Expression<Func<ProcessData, bool>> predicate)
        {
            //return Context.Set<ProcessTableEntry>().Where(predicate).ToList();
            return Context.Set<ProcessData>().Where(predicate).ToList();
        }

        ProcessData? IProcessDataRepository.FindFirstOrDef(Expression<Func<ProcessData, bool>> predicate)
        {
            return Context.Set<ProcessData>().FirstOrDefault(predicate);
        }
    }
}
