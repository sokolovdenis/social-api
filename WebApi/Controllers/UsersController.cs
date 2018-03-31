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
	public class UsersController : Controller
	{
		private readonly UserDataSource _userDataSource;
		private readonly FollowDataSource _followDataSource;
		private readonly ImageProcessingService _imageService;
		private readonly AzureStorage _storageService;

		public UsersController(UserDataSource uds, FollowDataSource fds, ImageProcessingService imgSrv, AzureStorage stSrv)
		{
			_userDataSource = uds ?? throw new ArgumentNullException(nameof(uds));
			_followDataSource = fds ?? throw new ArgumentNullException(nameof(fds));
			_imageService = imgSrv ?? throw new ArgumentNullException(nameof(imgSrv));
			_storageService = stSrv ?? throw new ArgumentNullException(nameof(stSrv));
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
		public async Task<IActionResult> ModifyMe([FromBody]ModifyMeRequest request)
		{
			int currentUserId = this.GetCurrentUserId();
			User user = await _userDataSource.Update(
				currentUserId, request.Name, request.Info, request.Birthday);
			return Ok(user);
		}

		[HttpPut]
		[Route("me/photo")]
		public async Task<IActionResult> ModifyPhotoMe(IFormFile file)
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

			await _userDataSource.UpdateImageUrl(currentUserId, url);

			return Ok();
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
