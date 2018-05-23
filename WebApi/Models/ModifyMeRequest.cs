using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
	/// <summary>
	/// Modify Self Request.
	/// </summary>
	public class ModifyMeRequest
	{
		/// <summary>
		/// Name.
		/// </summary>
		[Required]
		[MaxLength(50)]
		public string Name { get; set; }

		/// <summary>
		/// Info.
		/// </summary>
		[MaxLength(500)]
		public string Info { get; set; }

		/// <summary>
		/// Birthday.
		/// </summary>
		[Required]
		public DateTime Birthday { get; set; }
	}
}
