﻿using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApi.DataSources;
using WebApi.Infrastructure;
using WebApi.Models;

namespace WebApi.Controllers
{
	[Route("api/identity")]
	public class IdentityController : Controller
	{
		private readonly UserDataSource _userDataSource;
		private readonly IdentityDataSource _identityDataSource;
		private readonly IdentityService _identityService;
		private readonly JwtAuthenticationService _jwtAuthenticationService;

		public IdentityController(UserDataSource uds, IdentityDataSource ids, IdentityService isrv,
			JwtAuthenticationService jwtAuthSrv)
		{
			_userDataSource = uds ?? throw new ArgumentNullException(nameof(uds));
			_identityDataSource = ids ?? throw new ArgumentNullException(nameof(ids));
			_identityService = isrv ?? throw new ArgumentNullException(nameof(isrv));
			_jwtAuthenticationService = jwtAuthSrv ?? throw new ArgumentNullException(nameof(jwtAuthSrv));
		}

		/// <summary>
		/// Sign in and get API token.
		/// </summary>
		/// <param name="request">Sign In Request.</param>
		[HttpPost]
		[Route("signin")]
		[ProducesResponseType(typeof(TokenResponse), 200)]
		[ProducesResponseType(400)]
		public async Task<IActionResult> SignIn([FromBody]SignInRequest request)
		{
			Guid id = _identityService.GenerateIdentityHash(request.Email);

			Identity identity = await _identityDataSource.Read(id);

			if (identity == null ||
				!_identityService.IsPasswordValid(request.Password, identity.Hash, identity.Salt))
			{
				return BadRequest("Password incorrect or email not found.");
			}

			TokenResponse tokenResponse = _jwtAuthenticationService.CreateTokenResponse(identity);

			return Ok(tokenResponse);
		}

		/// <summary>
		/// Sign up and get API token.
		/// </summary>
		/// <param name="request">Sign Up Request.</param>
		[HttpPost]
		[Route("signup")]
		[ProducesResponseType(typeof(TokenResponse), 200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(409)]
		public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
		{
			Guid id = _identityService.GenerateIdentityHash(request.Email);

			Identity identity = await _identityDataSource.Read(id);

			if (identity != null)
			{
				return StatusCode((int)HttpStatusCode.Conflict, "Email is already used.");
			}

			_identityService.GeneratePasswordHashAndSalt(
				request.Password, out byte[] hash, out byte[] salt);

			User user = await _userDataSource.Create(request.Name, "", request.Birthday);
			identity = await _identityDataSource.Create(id, user.Id, salt, hash);

			TokenResponse tokenResponse = _jwtAuthenticationService.CreateTokenResponse(identity);

			return Ok(tokenResponse);
		}
	}
}
