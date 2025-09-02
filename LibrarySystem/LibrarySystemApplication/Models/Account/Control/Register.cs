using System.ComponentModel.DataAnnotations;

namespace LibrarySystemApplication.Models.Account.Control
{
    public class Register
    {

        [Required]
       public string? UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
       public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
       public string? Password { get; set; }


        [Compare("Password", ErrorMessage = "Password dont matched") ]
       public string? ConfirmPassword { get; set; } 

        public string? Address { get; set; }

    }
}
