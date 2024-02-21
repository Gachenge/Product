using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Abno.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        [MaxLength (1000)]
        public string Description { get; set; }
        [Required]
        public bool IsAvailable { get; set; }
        public string Image { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Link { get; set; }

        public ICollection<UserProduct> UserProducts { get; set; }
    }
}
