using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTracker.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;
        public IDailyTotalRepository DailyTotals { get; set; }

        public UnitOfWork(DbContext context)
        {
            _context = context;
            DailyTotals = new DailyTotalRepository(_context);
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

    }
}
