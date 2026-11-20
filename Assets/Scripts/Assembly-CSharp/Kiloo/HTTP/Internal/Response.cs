using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Ionic.Zlib;
using Kiloo.Common;
using UnityEngine;

namespace Kiloo.HTTP.Internal
{
	internal class Response : IResponse
	{
		private const string EOL = "\r\n";

		private List<byte[]> chunks;

		public int status { get; internal set; }

		public string message { get; internal set; }

		public byte[] bytes { get; internal set; }

		public Headers headers { get; internal set; }

		public ResponseCategory category
		{
			get
			{
				int num = status / 100;
				if (Enum.IsDefined(typeof(ResponseCategory), num))
				{
					return (ResponseCategory)num;
				}
				return ResponseCategory.Unknown;
			}
		}

		public Response(int status, string message, Headers headers, byte[] bytes)
		{
			this.status = status;
			this.message = message;
			this.headers = headers;
			this.bytes = bytes;
		}

		public Response(Headers headers, byte[] bytes)
			: this(200, "OK", headers, bytes)
		{
		}

		public Response()
			: this(new Headers(), null)
		{
		}

		public static string ContentAsString(byte[] bytes, Headers headers)
		{
			if (bytes == null)
			{
				return null;
			}
			Encoding encoding = null;
			string value;
			if (headers.contentType.TryGetValue("charset", out value) && !string.IsNullOrEmpty(value))
			{
				try
				{
					encoding = Encoding.GetEncoding(value);
				}
				catch (ArgumentException)
				{
				}
			}
			if (encoding == null)
			{
				encoding = Encoding.UTF8;
			}
			return encoding.GetString(bytes, 0, bytes.Length);
		}

		public override string ToString()
		{
			return ContentAsString(bytes, headers) ?? string.Empty;
		}

		private string ReadHeaderLine(Stream stream)
		{
			MemoryStream memoryStream = new MemoryStream();
			int num;
			while ((num = stream.ReadByte()) >= 0 && num != "\r\n"[1])
			{
				if (num != "\r\n"[0])
				{
					memoryStream.WriteByte((byte)num);
				}
			}
			return ASCIIEncoding.ASCII.GetString(memoryStream.GetBuffer(), 0, (int)memoryStream.Length).Trim();
		}

		private bool ReadHeader(Stream stream, out string key, out string value)
		{
			string text = ReadHeaderLine(stream);
			if (text == string.Empty)
			{
				key = null;
				value = null;
				return false;
			}
			int num = text.IndexOf(':');
			if (num == -1)
			{
				throw new HTTPException("Invalid header in response");
			}
			key = text.Substring(0, num).Trim();
			value = text.Substring(num + 1).Trim();
			return true;
		}

		public byte[] TakeChunk()
		{
			byte[] result = null;
			lock (chunks)
			{
				if (chunks.Count > 0)
				{
					result = chunks[0];
					chunks.RemoveAt(0);
				}
			}
			return result;
		}

		public void ReadFromStream(Stream inputStream, bool ignoreContentMD5 = false)
		{
			string[] array = ReadHeaderLine(inputStream).Split(' ');
			int result;
			if (!int.TryParse(array[1], out result))
			{
				throw new HTTPException("Bad Status Code in response");
			}
			status = result;
			message = string.Join(" ", array, 2, array.Length - 2);
			headers.Clear();
			string key;
			string value;
			while (ReadHeader(inputStream, out key, out value))
			{
				headers.Add(key, value);
			}
			if (headers["Transfer-Encoding"] == "chunked")
			{
				chunks = new List<byte[]>();
				while (true)
				{
					string text = ReadHeaderLine(inputStream);
					if (text == "0")
					{
						break;
					}
					int num = int.Parse(text, NumberStyles.AllowHexSpecifier);
					byte[] array2 = new byte[num];
					inputStream.ReadFully(array2);
					lock (chunks)
					{
						if (headers["Content-Encoding"].Contains("gzip"))
						{
							chunks.Add(UnZip(array2));
						}
						else
						{
							chunks.Add(array2);
						}
					}
					inputStream.ReadByte();
					inputStream.ReadByte();
				}
				lock (chunks)
				{
					chunks.Add(new byte[0]);
				}
				string key2;
				string value2;
				while (ReadHeader(inputStream, out key2, out value2))
				{
					headers.Add(key2, value2);
				}
				List<byte> list = new List<byte>();
				foreach (byte[] chunk in chunks)
				{
					list.AddRange(chunk);
				}
				bytes = list.ToArray();
				return;
			}
			long num2 = headers.Get("Content-Length", 0L);
			if (num2 > 0)
			{
				byte[] array3 = new byte[num2];
				inputStream.ReadFully(array3);
				string text2 = headers.Get("Content-MD5");
				if (!string.IsNullOrEmpty(text2))
				{
					byte[] inArray = Checksum.ComputeMD5(array3);
					string text3 = Convert.ToBase64String(inArray);
					if (text3 != text2)
					{
						if (!ignoreContentMD5)
						{
							throw new HTTPException("Content-MD5 digest mismatch. Data is corrupted.");
						}
						LogWarning("Content-MD5 digest mismatch. Data is corrupted.");
					}
				}
				if (headers["Content-Encoding"].Contains("gzip"))
				{
					bytes = UnZip(array3);
				}
				else
				{
					bytes = array3;
				}
			}
			else
			{
				bytes = new byte[0];
			}
		}

		private static byte[] UnZip(byte[] data)
		{
			MemoryStream memoryStream = new MemoryStream();
			using (GZipStream gZipStream = new GZipStream(new MemoryStream(data, false), CompressionMode.Decompress))
			{
				byte[] array = new byte[1024];
				int count;
				while ((count = gZipStream.Read(array, 0, array.Length)) > 0)
				{
					memoryStream.Write(array, 0, count);
				}
			}
			return memoryStream.ToArray();
		}

		private static void LogWarning(string msg)
		{
			Debug.LogWarning("[Response] " + msg);
		}
	}
}
