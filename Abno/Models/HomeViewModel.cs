using System.Collections.Generic;

namespace Abno.Models
{
    public class HomeViewModel
    {
        public List<Product> Products { get; set; }
        public List<UserProduct> userProducts { get; internal set; }
    }
}
