using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace SafeVault.Models
{
    public class UserModel : IdentityUser
    {
        [Required]
        public string Role { get; set; }
    }
}