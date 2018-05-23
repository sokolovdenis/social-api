using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
	/// <summary>
	/// Sign In Request.
	/// </summary>
	public class SignInRequest
	{
		/// <summary>
		/// Email.
		/// </summary>
		[Required]
		[MaxLength(50)]
		[EmailAddress]
		public string Email { get; set; }

		/// <summary>
		/// Password.
		/// </summary>
		[Required]
		[MaxLength(50)]
		public string Password { get; set; }
	}
}
