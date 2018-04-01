using System;
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
	[Route("api/users/me/posts")]
	public class SelfPostsController : Controller
	{
		private PostDataSource _postDataSource;
		private UserDataSource _userDataSource;
		private readonly ImageProcessingService _imageService;
		private readonly AzureStorage _storageService;

		public SelfPostsController(PostDataSource pds, UserDataSource uds, 
			ImageProcessingService imgSrv, AzureStorage stSrv)
		{
			_postDataSource = pds ?? throw new ArgumentNullException(nameof(pds));
			_userDataSource = uds ?? throw new ArgumentNullException(nameof(uds));
			_imageService = imgSrv ?? throw new ArgumentNullException(nameof(imgSrv));
			_storageService = stSrv ?? throw new ArgumentNullException(nameof(stSrv));
		}

		/// <summary>
		/// Create post.
		/// </summary>
		/// <param name="request">Post Creation Request.</param>
		[HttpPost]
		[Route("")]
		[ProducesResponseType(typeof(Post), 200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> CreatePost(PostCreateRequest request)
		{
			int currentUserId = this.GetCurrentUserId();

			if (await _userDataSource.Read(currentUserId) == null)
			{
				return NotFound();
			}

			Post post = await _postDataSource.Create(currentUserId, request.Text);

			return Ok(post);
		}

		/// <summary>
		/// Attach image to your post.
		/// </summary>
		/// <param name="postId">Post ID.</param>
		/// <param name="imageFile">Image file.</param>
		[HttpPut]
		[Route("{postId}/image")]
		[ProducesResponseType(typeof(Post), 200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> PutPostImage(int postId, [FromForm]IFormFile imageFile)
		{
			int currentUserId = this.GetCurrentUserId();

			if (await _userDataSource.Read(currentUserId) == null)
			{
				return NotFound();
			}

			if (!_imageService.IsSupportedFormat(imageFile.ContentType, imageFile.FileName))
			{
				return BadRequest();
			}

			string fileName = $"{Guid.NewGuid()}.jpg";
			string url;

			using (Stream originalImageStream = imageFile.OpenReadStream())
			using (Stream resizedImageStream = new MemoryStream())
			{
				_imageService.ResizePostImage(originalImageStream, resizedImageStream);
				url = await _storageService.UploadPostImageAsync(resizedImageStream, fileName);
			}

			Post post = await _postDataSource.UpdateImage(postId, currentUserId, url);

			if (post == null)
			{
				return NotFound();
			}

			return Ok(post);
		}
	}
}
