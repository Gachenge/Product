using System.Collections.Generic;

namespace Abno.Models
{
    public class AdminViewModel
    {
        public Dictionary<Product, List<User>> UsersPerProduct { get; set; }
        public Product Product { get; set; }
        public int totalProducts {  get; set; }
        public int totalUsers {  get; set; }
        public List<UserProduct> UserProducts { get; set; }
        public List<object> LineDataPoints { get; internal set; }
    }
}
