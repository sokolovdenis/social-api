using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
	public class SignUpRequest
	{
		[Required]
		[MaxLength(50)]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		[MaxLength(50)]
		public string Password { get; set; }

		[Required]
		[MaxLength(50)]
		public string Name { get; set; }

		[Required]
		public DateTime Birthday { get; set; }
	}
}
