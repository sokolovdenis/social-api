using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
	public class SignInRequest
	{
		[Required]
		[MaxLength(50)]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		[MaxLength(50)]
		public string Password { get; set; }
	}
}
