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
		private readonly FollowDataSource _followDataSource;

		public UsersController(UserDataSource uds, FollowDataSource fds)
		{
			_userDataSource = uds ?? throw new ArgumentNullException(nameof(uds));
			_followDataSource = fds ?? throw new ArgumentNullException(nameof(fds));
		}

		[HttpGet]
		[Route("me")]
		public async Task<IActionResult> GetMe()
		{
			int currentUserId = this.GetCurrentUserId();
			User user = await _userDataSource.Read(currentUserId);
			return Ok(user);
		}

		[HttpDelete]
		[Route("me")]
		public async Task<IActionResult> DeleteMe()
		{
			int currentUserId = this.GetCurrentUserId();
			await _userDataSource.Delete(currentUserId);
			return Ok();
		}

		[HttpPut]
		[Route("me")]
		public async Task<IActionResult> ModifyMe(ModifyMeRequest request)
		{
			int currentUserId = this.GetCurrentUserId();
			User user = await _userDataSource.Update(
				currentUserId, request.Name, request.Info, request.Birthday);
			return Ok(user);
		}

		[HttpGet]
		[Route("{id}")]
		public async Task<IActionResult> Get(int id)
		{
			User user = await _userDataSource.Read(id);
			return Ok(user);
		}

		[HttpGet]
		[Route("")]
		public async Task<IActionResult> GetAll()
		{
			IEnumerable<User> users = await _userDataSource.Read();
			return Ok(users);
		}
	}
}
