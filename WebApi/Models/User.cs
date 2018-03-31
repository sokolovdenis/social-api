using System;

namespace WebApi.Models
{
	public class User
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public string Info { get; set; }

		public string ImageUrl { get; set; }

		public DateTime Birthday { get; set; }
	}
}
