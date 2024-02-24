using System.Collections.Generic;

namespace Abno.Models
{
    public class UserProductViewModel
    {
        public UserProduct userProduct { get; internal set; }
        public List<User> subscribers { get; internal set; }
    }
}
