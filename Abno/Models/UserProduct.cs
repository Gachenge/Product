using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Abno.Models
{
    public class UserProduct
    {
        [Key]
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public int ProductId { get; set; }

        // Navigation property for the associated Product
        public Product Product { get; set; }
        public User ProductUser { get; set; }
    }
}
