using System;

namespace WebApi.Models
{
	public class Post
	{
		public int Id { get; set; }

		public string Text { get; set; }

		public DateTime DateTime { get; set; }

		public User User { get; set; }
	}
}
