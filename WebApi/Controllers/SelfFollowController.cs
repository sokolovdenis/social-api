using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApi.DataSources;
using WebApi.Infrastructure;

namespace WebApi.Controllers
{
	[Route("api/users/me/followings")]
	public class SelfFollowController : Controller
	{
		private readonly FollowDataSource _followDataSource;
		private readonly UserDataSource _userDataSource;

		public SelfFollowController(FollowDataSource fds, UserDataSource uds)
		{
			_followDataSource = fds ?? throw new ArgumentNullException(nameof(fds));
			_userDataSource = uds ?? throw new ArgumentNullException(nameof(uds));
		}

		/// <summary>
		/// Follow User.
		/// </summary>
		/// <param name="userId">User ID you want to follow.</param>
		[HttpPost]
		[Route("{userId}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(404)]
		[ProducesResponseType(409)]
		public async Task<IActionResult> Add(int userId)
		{
			int currentUserId = this.GetCurrentUserId();

			if (await _userDataSource.Read(currentUserId) == null || 
				await _userDataSource.Read(userId) == null)
			{
				return NotFound();
			}

			if (await _followDataSource.Exists(currentUserId, userId))
			{
				return StatusCode((int)HttpStatusCode.Conflict);
			}

			await _followDataSource.Create(currentUserId, userId);

			return Ok();
		}

		/// <summary>
		/// Unfollow User.
		/// </summary>
		/// <param name="userId">User ID you want to unfollow.</param>
		[HttpDelete]
		[Route("{userId}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> Delete(int userId)
		{
			int currentUserId = this.GetCurrentUserId();

			if (!await _followDataSource.Delete(currentUserId, userId))
			{
				return NotFound();
			}

			return Ok();
		}
	}
}
