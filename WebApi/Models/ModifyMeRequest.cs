using System;

namespace WebApi.Models
{
	public class ModifyMeRequest
	{
		public string Name { get; set; }

		public string Info { get; set; }

		public DateTime Birthday { get; set; }
	}
}
