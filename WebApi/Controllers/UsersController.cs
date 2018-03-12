using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
	[Authorize]
	[Route("api/users")]
	public class UsersController : Controller
	{
		[Route("me")]
		public IActionResult Get()
		{
			string id = User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value;

			return Ok(id);
		}
	}
}
