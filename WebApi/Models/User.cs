using System;

namespace WebApi.Models
{
	public class User
	{
		/// <summary>
		/// ID.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Info.
		/// </summary>
		public string Info { get; set; }

		/// <summary>
		/// Profile Image URL.
		/// </summary>
		public string ImageUrl { get; set; }

		/// <summary>
		/// Birthday.
		/// </summary>
		public DateTime Birthday { get; set; }
	}
}
