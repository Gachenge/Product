using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Abno.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public bool IsAvailable { get; set; }
        [Required]
        public string Image { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ICollection<UserProduct> UserProducts { get; set; }
    }
}
