using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.DataSources;
using WebApi.Models;

namespace WebApi.Controllers
{
	[Authorize]
	[Route("api/users")]
	public class FollowController : Controller
	{
		private readonly FollowDataSource _followDataSource;
		private readonly UserDataSource _userDataSource;

		public FollowController(FollowDataSource fds, UserDataSource uds)
		{
			_followDataSource = fds ?? throw new ArgumentNullException(nameof(fds));
			_userDataSource = uds ?? throw new ArgumentNullException(nameof(uds));
		}

		/// <summary>
		/// Get User's followings.
		/// </summary>
		/// <param name="userId">User ID.</param>
		[HttpGet]
		[Route("{userId}/followings")]
		[ProducesResponseType(typeof(IEnumerable<User>), 200)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> GetFollowings(int userId)
		{
			if (await _userDataSource.Read(userId) == null)
			{
				return NotFound();
			}

			IEnumerable<User> users = await _followDataSource.ReadFollowings(userId);

			return Ok(users);
		}

		/// <summary>
		/// Get User's followers.
		/// </summary>
		/// <param name="userId">User ID.</param>
		[HttpGet]
		[Route("{userId}/followers")]
		[ProducesResponseType(typeof(IEnumerable<User>), 200)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> GetFollowers(int userId)
		{
			if (await _userDataSource.Read(userId) == null)
			{
				return NotFound();
			}

			IEnumerable<User> users = await _followDataSource.ReadFollowers(userId);

			return Ok(users);
		}
	}
}
