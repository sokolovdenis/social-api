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
	/// <summary>
	/// Yourself management methods.
	/// </summary>
	/// <response code="404">If provided token issued for deleted user.</response> 
	[Authorize]
	[Route("api/users/me")]
	public class SelfUserController : Controller
	{
		private readonly UserDataSource _userDataSource;
		private readonly ImageProcessingService _imageService;
		private readonly AzureStorage _storageService;

		public SelfUserController(UserDataSource uds, ImageProcessingService imgSrv, AzureStorage stSrv)
		{
			_userDataSource = uds ?? throw new ArgumentNullException(nameof(uds));
			_imageService = imgSrv ?? throw new ArgumentNullException(nameof(imgSrv));
			_storageService = stSrv ?? throw new ArgumentNullException(nameof(stSrv));
		}

		/// <summary>
		/// Get yourself info.
		/// </summary>
		[HttpGet]
		[Route("")]
		[ProducesResponseType(typeof(User), 200)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> Get()
		{
			int currentUserId = this.GetCurrentUserId();

			User user = await _userDataSource.Read(currentUserId);

			if (user == null)
			{
				return NotFound();
			}

			return Ok(user);
		}

		/// <summary>
		/// Delete yourself.
		/// </summary>
		[HttpDelete]
		[Route("")]
		public async Task<IActionResult> Delete()
		{
			int currentUserId = this.GetCurrentUserId();

			User user = await _userDataSource.Delete(currentUserId);

			if (user == null)
			{
				return NotFound();
			}

			return Ok();
		}

		/// <summary>
		/// Edit yourself info.
		/// </summary>
		/// <param name="request">New yourself info.</param>
		[HttpPut]
		[Route("")]
		[ProducesResponseType(typeof(User), 200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> Put([FromBody]ModifyMeRequest request)
		{
			int currentUserId = this.GetCurrentUserId();

			User user = await _userDataSource.Update(
				currentUserId, request.Name, request.Info, request.Birthday);

			if (user == null)
			{
				return NotFound();
			}

			return Ok(user);
		}

		/// <summary>
		/// Edit yourself photo.
		/// </summary>
		/// <param name="file">Photo.</param>
		[HttpPut]
		[Route("photo")]
		[ProducesResponseType(400)]
		public async Task<IActionResult> PutPhoto([FromForm]IFormFile file)
		{
			if (!_imageService.IsSupportedFormat(file.ContentType, file.FileName))
			{
				return BadRequest();
			}

			string fileName = $"{Guid.NewGuid()}.jpg";
			string url;

			using (Stream originalImageStream = file.OpenReadStream())
			using (Stream resizedImageStream = new MemoryStream())
			{
				_imageService.ResizeUserImage(originalImageStream, resizedImageStream);
				url = await _storageService.UploadUserImage(resizedImageStream, fileName);
			}

			int currentUserId = this.GetCurrentUserId();

			User user = await _userDataSource.UpdateImageUrl(currentUserId, url);

			if (user == null)
			{
				return NotFound();
			}

			return Ok(user);
		}
	}
}
