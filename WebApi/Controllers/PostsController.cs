using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
		private PostDataSource _postDataSource;
		private readonly ImageProcessingService _imageService;
		private readonly AzureStorage _storageService;

		public PostsController(PostDataSource pds, ImageProcessingService imgSrv, AzureStorage stSrv)
		{
			_postDataSource = pds ?? throw new ArgumentNullException(nameof(pds));
			_imageService = imgSrv ?? throw new ArgumentNullException(nameof(imgSrv));
			_storageService = stSrv ?? throw new ArgumentNullException(nameof(stSrv));
		}

		[HttpGet]
		[Route("{userId}/feed")]
		public async Task<IActionResult> GetFeed(int userId, int skip = 0, int count = 20)
		{
			IEnumerable<Post> posts = await _postDataSource.ReadFeedAsync(userId, skip, count);

			return Ok(posts);
		}

		[HttpGet]
		[Route("{userId}/posts")]
		public async Task<IActionResult> GetPosts(int userId, int skip = 0, int count = 20)
		{
			IEnumerable<Post> posts = await _postDataSource.ReadWallAsync(userId, skip, count);

			return Ok(posts);
		}

		[HttpPost]
		[Route("me/posts")]
		public async Task<IActionResult> CreatePost([FromBody]string text)
		{
			int currentUserId = this.GetCurrentUserId();

			Post post = await _postDataSource.Create(currentUserId, text);

			return Ok();
		}

		[HttpPut]
		[Route("me/posts/{id}/image")]
		public async Task<IActionResult> PutPostImage(int id, IFormFile formFile)
		{
			if (!_imageService.IsSupportedFormat(formFile.ContentType, formFile.FileName))
			{
				return BadRequest();
			}

			string fileName = $"{Guid.NewGuid()}.jpg";
			string url;

			using (Stream originalImageStream = formFile.OpenReadStream())
			using (Stream resizedImageStream = new MemoryStream())
			{
				_imageService.ResizePostImage(originalImageStream, resizedImageStream);
				url = await _storageService.UploadPostImageAsync(resizedImageStream, fileName);
			}

			int currentUserId = this.GetCurrentUserId();

			await _postDataSource.UpdateImage(id, currentUserId, url);

			return Ok();
		}
	}
}