using System.Security.Cryptography;
using System.Text;

namespace Kiloo.Common
{
	public static class Checksum
	{
		public static byte[] ComputeSHA1(byte[] data)
		{
			using (SHA1 sHA = SHA1.Create())
			{
				return sHA.ComputeHash(data);
			}
		}

		public static byte[] ComputeMD5(byte[] data)
		{
			using (MD5 mD = MD5.Create())
			{
				return mD.ComputeHash(data);
			}
		}

		public static byte[] ComputeSHA256(byte[] data)
		{
			using (SHA256 sHA = SHA256.Create())
			{
				return sHA.ComputeHash(data);
			}
		}

		public static byte[] ComputeSHA1(string data, Encoding encoding = null)
		{
			if (encoding == null)
			{
				encoding = Encoding.UTF8;
			}
			return ComputeSHA1(encoding.GetBytes(data));
		}

		public static byte[] ComputeMD5(string data, Encoding encoding = null)
		{
			if (encoding == null)
			{
				encoding = Encoding.UTF8;
			}
			return ComputeMD5(encoding.GetBytes(data));
		}

		public static byte[] ComputeSHA256(string data, Encoding encoding = null)
		{
			if (encoding == null)
			{
				encoding = Encoding.UTF8;
			}
			return ComputeSHA256(encoding.GetBytes(data));
		}
	}
}
