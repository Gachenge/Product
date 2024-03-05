using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Abno.Models
{
 
    public class User : IdentityUser
    {
        public string Avatar { get; set; }
        public string Primary { get; set; }
        public string Secondary { get; set; }

        // Navigation property for many-to-many relationship
        public ICollection<UserProduct> UserProducts { get; set; }

        public User()
        {
            this.Avatar = "/Avatars/default.jpg";
            this.Primary = "blue";
            this.Secondary= "#4dea39";
        }

    }
}
