using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
	/// <summary>
	/// Post Create Request.
	/// </summary>
	public class PostCreateRequest
	{
		/// <summary>
		/// Text.
		/// </summary>
		[Required]
		[MaxLength(500)]
		public string Text { get; set; }
	}
}