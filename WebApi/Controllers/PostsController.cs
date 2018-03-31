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
	public class PostsController : Controller
	{
		private PostDataSource _pds;

		public PostsController(PostDataSource pds)
		{
			_pds = pds ?? throw new ArgumentNullException(nameof(pds));
		}

		[HttpGet]
		[Route("{userId}/feed")]
		public async Task<IActionResult> GetFeed(int userId, int skip = 0, int count = 20)
		{
			IEnumerable<Post> posts = await _pds.ReadFeedAsync(userId, skip, count);

			return Ok(posts);
		}

		[HttpGet]
		[Route("{userId}/posts")]
		public async Task<IActionResult> GetPosts(int userId, int skip = 0, int count = 20)
		{
			IEnumerable<Post> posts = await _pds.ReadWallAsync(userId, skip, count);

			return Ok(posts);
		}

		[HttpPost]
		[Route("me/posts")]
		public async Task<IActionResult> CreatePost([FromBody]string text)
		{
			int currentUserId = this.GetCurrentUserId();

			Post post = await _pds.Create(currentUserId, text);

			return Ok();
		}
	}
}