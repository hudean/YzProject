using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YzProject.Domain.UnitOfWorks
{
    //public class EFUnitOfWork : IUnitOfWork
    //{
    //    private readonly DbContext _context;

    //    public EFUnitOfWork(DbContext context)
    //    {
    //        _context = context;
    //    }

    //    public DbTransaction BeginTransaction()
    //    {
    //        _context.Database.BeginTransaction();
    //        return _context.Database.CurrentTransaction.GetDbTransaction();
    //    }

    //    public void CommitTransaction()
    //    {
    //        _context.SaveChanges();
    //        _context.Database.CommitTransaction();
    //    }

    //    public void RollbackTransaction()
    //    {
    //        _context.Database.RollbackTransaction();
    //    }
    //}
}
