using System;
using JetBrains.Annotations;
using Kiloo.Common;
using MiniJSON;
using UnityEngine;

namespace Kiloo.HTTP
{
	public static class ResponseExtensions
	{
		public static bool IsJSON(this IResponse response)
		{
			string text = response.headers["Content-Type"];
			return text.Contains("json") || text.Contains("javascript");
		}

		[CanBeNull]
		public static object AsJSON(this IResponse response)
		{
			try
			{
				return (response.bytes != null) ? Json.Deserialize(response.ToString()) : null;
			}
			catch (Exception)
			{
				return null;
			}
		}

		[NotNull]
		public static JsonObject AsJSONObject(this IResponse response)
		{
			return new JsonObject(response.AsJSON());
		}

		public static bool IsTexture(this IResponse response)
		{
			string text = response.headers["Content-Type"];
			return text.Equals("image/jpeg") || text.Equals("image/png");
		}

		[CanBeNull]
		public static Texture2D AsTexture(this IResponse response)
		{
			byte[] bytes = response.bytes;
			if (bytes != null && bytes.Length > 0)
			{
				Texture2D texture2D = new Texture2D(4, 4);
				if (texture2D.LoadImage(bytes))
				{
					return texture2D;
				}
			}
			return null;
		}
	}
}
