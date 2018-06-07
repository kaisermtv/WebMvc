using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMvc.Domain.Interfaces;
using WebMvc.Domain.Interfaces.Services;
using WebMvc.Services.Data.Context;

namespace WebMvc.Services
{
    public partial class ShoppingCartProductService : IShoppingCartProductService
    {
        private readonly WebMvcContext _context;
        private readonly ICacheService _cacheService;

        public ShoppingCartProductService(IWebMvcContext context, ICacheService cacheService)
        {
            _cacheService = cacheService;
            _context = context as WebMvcContext;
        }
    }
}
