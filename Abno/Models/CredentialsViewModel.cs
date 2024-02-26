using System.ComponentModel.DataAnnotations;

namespace Abno.Models
{
    public class CredentialsViewModel
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        [MinLength(5)]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [MinLength(7)]
        public string Password { get; set; }
        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
