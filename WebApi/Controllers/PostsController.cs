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
	[Route("api/users/{userId}")]
	public class PostsController : Controller
	{
		private readonly PostDataSource _postDataSource;
		private readonly UserDataSource _userDataSource;

		public PostsController(PostDataSource pds, UserDataSource uds)
		{
			_postDataSource = pds ?? throw new ArgumentNullException(nameof(pds));
			_userDataSource = uds ?? throw new ArgumentNullException(nameof(uds));
		}

		/// <summary>
		/// Get posts written by followings of User, from last to first in time.
		/// </summary>
		/// <param name="userId">User ID.</param>
		/// <param name="skip">How many posts to skip. Default is 0.</param>
		/// <param name="count">How many posts to get. Default is 20.</param>
		/// <returns></returns>
		[HttpGet]
		[Route("posts/feed")]
		[ProducesResponseType(typeof(IEnumerable<Post>), 200)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> GetFeed(int userId, int? skip = 0, int? count = 20)
		{
			if (await _userDataSource.Read(userId) == null)
			{
				return NotFound();
			}

			IEnumerable<Post> posts = await _postDataSource.ReadFeedAsync(
				userId, skip.Value, count.Value);

			return Ok(posts);
		}

		/// <summary>
		/// Get posts written by specified User, from last to first in time.
		/// </summary>
		/// <param name="userId">User ID.</param>
		/// <param name="skip">How many posts to skip. Default is 0.</param>
		/// <param name="count">How many posts to get. Default is 20.</param>
		[HttpGet]
		[Route("posts/wall")]
		[ProducesResponseType(typeof(IEnumerable<Post>), 200)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> GetPosts(int userId, int? skip = 0, int? count = 20)
		{
			if (await _userDataSource.Read(userId) == null)
			{
				return NotFound();
			}

			IEnumerable<Post> posts = await _postDataSource.ReadWallAsync(
				userId, skip.Value, count.Value);

			return Ok(posts);
		}
	}
}