using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
	/// <summary>
	/// Sign Up Request.
	/// </summary>
	public class SignUpRequest
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

		/// <summary>
		/// Name.
		/// </summary>
		[Required]
		[MaxLength(50)]
		public string Name { get; set; }

		/// <summary>
		/// Birthday.
		/// </summary>
		[Required]
		public DateTime Birthday { get; set; }
	}
}
