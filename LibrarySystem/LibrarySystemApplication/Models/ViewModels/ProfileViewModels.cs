using System.ComponentModel.DataAnnotations;
using LibrarySystemApplication.Models.Account;

namespace LibrarySystemApplication.ViewModels
{
    public class ProfileViewModel
    {
        [Required]
        public Member Member { get; set; }
    }
}
