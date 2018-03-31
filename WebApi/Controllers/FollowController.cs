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
	public class FollowController : Controller
	{
		private readonly FollowDataSource _followDataSource;

		public FollowController(FollowDataSource fds)
		{
			_followDataSource = fds ?? throw new ArgumentNullException(nameof(fds));
		}

		[HttpPost]
		[Route("me/followings/{userId}")]
		public async Task<IActionResult> Add(int userId)
		{
			int currentUserId = this.GetCurrentUserId();

			await _followDataSource.Create(currentUserId, userId);

			return Ok();
		}

		[HttpDelete]
		[Route("me/followings/{userId}")]
		public async Task<IActionResult> Remove(int userId)
		{
			int currentUserId = this.GetCurrentUserId();

			await _followDataSource.Delete(currentUserId, userId);

			return Ok();
		}

		[HttpGet]
		[Route("{userId}/followings")]
		public async Task<IActionResult> GetFollowings(int userId)
		{
			IEnumerable<User> users = await _followDataSource.ReadFollowings(userId);

			return Ok(users);
		}

		[HttpGet]
		[Route("{userId}/followers")]
		public async Task<IActionResult> GetFollowers(int userId)
		{
			IEnumerable<User> users = await _followDataSource.ReadFollowers(userId);

			return Ok(users);
		}
	}
}
