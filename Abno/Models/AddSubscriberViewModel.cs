using Microsoft.AspNetCore.Mvc.Rendering;

namespace Abno.Models
{
    public class AddSubscriberViewModel
    {
        public SelectList UserSelect { get; set; }
        public Product Product { get; set; }
        public UserProduct userProduct { get; set; }
        public SelectList UserId { get; internal set; }
    }
}
