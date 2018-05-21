using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMvc.Domain.Interfaces;
using WebMvc.Domain.Interfaces.UnitOfWork;

namespace WebMvc.Services.Data.UnitOfWork
{
    public partial class UnitOfWorkManager : IUnitOfWorkManager
    {
        private bool _isDisposed;
        private readonly IWebMvcContext context;
        public UnitOfWorkManager(IWebMvcContext _context)
        {
            context = _context;
        }

        public IUnitOfWork NewUnitOfWork()
        {
            return new UnitOfWork(context);
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                context.Dispose();
                _isDisposed = true;
            }
        }
    }
}
