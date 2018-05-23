using System;
using System.Security.Cryptography;
using System.Text;

namespace WebApi.Infrastructure
{
	public class IdentityService
	{
		private const int HashSize = 32;
		private const int SaltSize = 32;
		private const int IterationsCount = 10000;

		public Guid GenerateIdentityHash(string identity)
		{
			using (MD5 md5 = MD5.Create())
			{
				byte[] hash = md5.ComputeHash(Encoding.Unicode.GetBytes(identity));
				return new Guid(hash);
			}
		}

		public void GeneratePasswordHashAndSalt(string password, out byte[] hash, out byte[] salt)
		{
			Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, SaltSize, IterationsCount);
			hash = rfc2898DeriveBytes.GetBytes(HashSize);
			salt = rfc2898DeriveBytes.Salt;
		}

		public bool IsPasswordValid(string password, byte[] savedHash, byte[] savedSalt)
		{
			Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, savedSalt, IterationsCount);
			byte[] computedHash = rfc2898DeriveBytes.GetBytes(HashSize);
			return ByteArrayHelper.AreEqual(savedHash, computedHash);
		}
	}
}
