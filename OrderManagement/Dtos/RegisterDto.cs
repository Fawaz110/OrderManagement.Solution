using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Dtos
{
    public class RegisterDto
    {
        [Required]
        [StringLength(10, MinimumLength = 3, ErrorMessage = "Name field must be between 3 ~ 10")]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [RegularExpression("^(?=.*[A-Za-z])(?=.*\\d)(?=.*[@$!%*#?&])[A-Za-z\\d@$!%*#?&]{6,}$", 
            ErrorMessage = "\"Passord Must be minimum six characters, at least one letter, one number and one special character\"")]
        public string Password { get; set; }
    }
}
