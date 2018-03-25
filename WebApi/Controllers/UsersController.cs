using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.DataSources;
using WebApi.Infrastructure;
using WebApi.Models;

namespace WebApi.Controllers
{
	[Authorize]
	[Route("api/users")]
	public class UsersController : Controller
	{
		private readonly UserDataSource _userDataSource;

		public UsersController(UserDataSource uds)
		{
			_userDataSource = uds ?? throw new ArgumentNullException(nameof(uds));
		}

		[Route("me")]
		[HttpGet]
		public async Task<IActionResult> GetMe()
		{
			int userId = this.GetCurrentUserId();
			User user = await _userDataSource.Read(userId);
			return Ok(user);
		}

		[Route("me")]
		[HttpDelete]
		public async Task<IActionResult> DeleteMe()
		{
			int userId = this.GetCurrentUserId();
			User user = await _userDataSource.Delete(userId);
			return Ok(user);
		}

		[Route("{id}")]
		[HttpGet]
		public async Task<IActionResult> Get(int id)
		{
			User user = await _userDataSource.Read(id);
			return Ok(user);
		}

		[Route("")]
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			IEnumerable<User> users = await _userDataSource.Read();
			return Ok(users);
		}
	}
}
