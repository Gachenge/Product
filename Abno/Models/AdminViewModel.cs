using System.Collections.Generic;

namespace Abno.Models
{
    public class AdminViewModel
    {
        public Dictionary<Product, List<User>> UsersPerProduct { get; set; }
        public Product Product { get; set; }
        public int TotalProducts {  get; set; }
        public int TotalUsers {  get; set; }
        public List<UserProduct> UserProducts { get; set; }
        public string DataPoints { get; internal set; }
        public string LineGraphData { get; internal set; }
    }
}
