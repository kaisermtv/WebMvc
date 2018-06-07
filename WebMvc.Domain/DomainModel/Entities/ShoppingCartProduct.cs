using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMvc.Utilities;

namespace WebMvc.Domain.DomainModel.Entities
{
    public partial class ShoppingCartProduct : Entity
    {
        public ShoppingCartProduct()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public Guid ShoppingCartId { get; set; }
        public Guid ProductId { get; set; }
        public string Price { get; set; }
        public int CountProduct { get; set; }
        
    }
}
