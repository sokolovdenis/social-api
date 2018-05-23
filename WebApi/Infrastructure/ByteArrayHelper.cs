namespace WebApi.Infrastructure
{
	public static class ByteArrayHelper
	{
		public static bool AreEqual(byte[] a, byte[] b)
		{
			if (a == null && b == null)
			{
				return true;
			}
			if (a == null || b == null || a.Length != b.Length)
			{
				return false;
			}
			var areSame = true;
			for (var i = 0; i < a.Length; i++)
			{
				areSame &= (a[i] == b[i]);
			}
			return areSame;
		}
	}
}
