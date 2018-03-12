using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebApi.Models;

namespace WebApi.Controllers
{
	public class AuthenticationController : Controller
	{
		[HttpPost]
		[Route("api/authentication")]
		public IActionResult RequestToken([FromBody]TokenRequest request)
		{
			if (request.Username == "Denis" && request.Password == "Sokolov")
			{
				var claims = new[]
				{
					new Claim(ClaimTypes.Name, request.Username),
					new Claim(ClaimTypes.NameIdentifier, Guid.Empty.ToString())
				};

				var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("У попа была собака."));
				var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

				var token = new JwtSecurityToken(
					issuer: "yourdomain.com",
					audience: "yourdomain.com",
					claims: claims,
					expires: DateTime.Now.AddMinutes(30),
					signingCredentials: creds);

				return Ok(new
				{
					token = new JwtSecurityTokenHandler().WriteToken(token)
				});
			}

			return BadRequest("Could not verify username and password");
		}
	}
}
