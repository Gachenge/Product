using System.ComponentModel.DataAnnotations;

namespace Abno.Models
{
    public class Credentials
    {
        public int Id { get; set; }
        [Required]
        public int ProductId { get; set; } 

        [Required]
        [MinLength(5)]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(7)]
        public string Password { get; set; }

        public Product Product { get; set; }
    }
}
