using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Entities
{
    public class AppUser : IdentityUser
    {
        [Required]
        [StringLength(10, MinimumLength = 3, ErrorMessage = "Name field must be between 3 ~ 10")]
        public string Name { get; set; }
    }
}
