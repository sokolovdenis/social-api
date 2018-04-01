using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.DataSources;
using WebApi.Models;

namespace WebApi.Controllers
{
	/// <summary>
	/// User methods.
	/// </summary>
	[Authorize]
	[Route("api/users")]
	[Produces("application/json")]
	public class UsersController : Controller
	{
		private readonly UserDataSource _userDataSource;

		public UsersController(UserDataSource uds)
		{
			_userDataSource = uds ?? throw new ArgumentNullException(nameof(uds));
		}

		/// <summary>
		/// Get user info.
		/// </summary>
		/// <param name="userId">User ID.</param>
		[HttpGet]
		[Route("{userId}")]
		[ProducesResponseType(typeof(User), 200)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> Get(int userId)
		{
			User user = await _userDataSource.Read(userId);

			if (user == null)
			{
				return NotFound();
			}

			return Ok(user);
		}

		/// <summary>
		/// Get all registered users info.
		/// </summary>
		[HttpGet]
		[Route("")]
		[ProducesResponseType(typeof(IEnumerable<User>), 200)]
		public async Task<IActionResult> GetAll()
		{
			IEnumerable<User> users = await _userDataSource.Read();
			return Ok(users);
		}
	}
}
