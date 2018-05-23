using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Infrastructure
{
	public static class ControllerExtensions
	{
		public static int GetCurrentUserId(this Controller controller)
		{
			return int.Parse(controller.User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value);
		}
	}
}
