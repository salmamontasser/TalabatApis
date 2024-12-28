using System.ComponentModel.DataAnnotations;

namespace Talabat.API.DTO
{
	public class RegisterDto
	{
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string DisplayName { get; set; }
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
        [Required]
		[RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[!@#$%^&()_+])[A-Za-z\\d!@#$%^&()_+]{6,10}$",
	ErrorMessage = "Password must contain 1 uppercase letter, 1 lowercase letter, 1 digit, and 1 special character.")]

		public string Password { get; set; }
    }
}
