using System.Collections.Generic;
using System.Data.SqlClient;
using WebMvc.Domain.Interfaces;
using WebMvc.Domain.Interfaces.Services;
using WebMvc.Domain.Interfaces.UnitOfWork;
using WebMvc.Services.Data.Context;

namespace WebMvc.Services.Data.UnitOfWork
{
    class UnitOfWork : IUnitOfWork
    {
        private readonly WebMvcContext context;
        private readonly SqlTransaction transaction;

        public UnitOfWork(IWebMvcContext _context)
        {
            context = _context as WebMvcContext;

            transaction = context.BeginTransaction();
        }
        
        public void Commit()
        {
            transaction.Commit();
        }

        public void Commit(List<string> cacheStartsWithToClear, ICacheService cacheService)
        {
            Commit();
            cacheService.ClearStartsWith(cacheStartsWithToClear);
        }

        public void Dispose()
        {
            context.Dispose();
        }
        
        public void Rollback()
        {
            transaction.Rollback();
        }
    }
}
