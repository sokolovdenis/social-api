using System;

namespace WebApi.Models
{
	/// <summary>
	/// User's Post.
	/// </summary>
	public class Post
	{
		/// <summary>
		/// Post ID.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Post Text.
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		/// Post Image URL.
		/// </summary>
		public string ImageUrl { get; set; }

		/// <summary>
		/// Created.
		/// </summary>
		public DateTime DateTime { get; set; }

		/// <summary>
		/// User's info.
		/// </summary>
		public User User { get; set; }
	}
}
