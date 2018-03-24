using System;

namespace WebApi.Models
{
	public class Identity
	{
		public Guid Id { get; set; }

		public int UserId { get; set; }

		public byte[] Salt { get; set; }

		public byte[] Hash { get; set; }
	}
}
