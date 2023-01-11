using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YzProject.Domain.UnitOfWorks
{
    public class EfCoreUnitOfWork<TDbContext> : YzProject.Domain.UnitOfWorks.IUnitOfWork where TDbContext : DbContext
    {
        private readonly TDbContext _dbContext;

        public EfCoreUnitOfWork(TDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public TDbContext DbContext => _dbContext;

        public void Commit()
        {
            _dbContext.SaveChanges();
        }

        public Task CommitAsync(CancellationToken cancellationToken = default)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
