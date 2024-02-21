using System.Collections.Generic;

namespace Abno.Models
{
    public class ProductsViewModel
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public List<Product> Products { get; set; }
        public Product Product { get; set; }
    }
}
