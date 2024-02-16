using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Abno.Models
{
    public class SubscriberViewModel
    {
        public List<Product> Products { get; set; }
        public  List<UserProduct> UserProducts { get; set; }
        public User User { get; set; }
        public UserProduct userProduct { get; set; } 
    }
}
