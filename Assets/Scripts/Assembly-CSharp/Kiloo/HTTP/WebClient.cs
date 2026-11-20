using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using Kiloo.Common;
using Kiloo.Plugins.DeviceUtility;
using UnityEngine;

namespace Kiloo.HTTP
{
	public static class WebClient
	{
		private class CacheInfo
		{
			public bool expired = true;

			public Headers headers;

			public DateTime savedTimeStamp;

			public string filename;

			public string metaFilename;
		}

		[Flags]
		public enum ClearCacheFlags
		{
			All = 1,
			Expired = 2,
			Old = 4
		}

		public delegate void RequestCompleteHandler(IRequest request, bool isFromCache);

		public const string META_DATA_SUFFIX = ".metadata";

		private static string _cachePath = null;

		private static readonly string[] CACHED_HEADERS_TO_UPDATE = new string[6] { "Date", "ETag", "Content-Location", "Expires", "Cache-Control", "Vary" };

		public static float simulatedRequestLatency = 0f;

		public static IEnumerable<SimulatedRequestResult> simulatedRequestResults = null;

		public static float timeout { get; set; }

		public static string cachePath
		{
			get
			{
				if (_cachePath == null)
				{
					string text = DeviceUtility.GetCachePath();
					if (string.IsNullOrEmpty(text))
					{
						throw new Exception("Unable to get cache path");
					}
					_cachePath = Path.Combine(text, "webclientcache");
					if (!Directory.Exists(_cachePath))
					{
						Directory.CreateDirectory(_cachePath);
					}
				}
				return _cachePath;
			}
		}

		[Pure]
		public static Uri AddQueryParametersToUri(Uri uri, IEnumerable<KeyValuePair<string, string>> queryParameters)
		{
			if (queryParameters != null)
			{
				StringBuilder stringBuilder = null;
				foreach (KeyValuePair<string, string> queryParameter in queryParameters)
				{
					if (stringBuilder == null)
					{
						string query = uri.Query;
						if (query.Length > 1)
						{
							stringBuilder = new StringBuilder(query, 1, query.Length - 1, 256);
							stringBuilder.Append('&');
						}
						else
						{
							stringBuilder = new StringBuilder(256);
						}
					}
					else
					{
						stringBuilder.Append('&');
					}
					stringBuilder.Append(Uri.EscapeDataString(queryParameter.Key));
					stringBuilder.Append('=');
					stringBuilder.Append(Uri.EscapeDataString(queryParameter.Value));
				}
				if (stringBuilder != null)
				{
					UriBuilder uriBuilder = new UriBuilder(uri);
					uriBuilder.Query = stringBuilder.ToString();
					uri = uriBuilder.Uri;
				}
			}
			return uri;
		}

		public static Uri AddQueryParametersToUri(string uri, IEnumerable<KeyValuePair<string, string>> queryParameters)
		{
			return AddQueryParametersToUri(new Uri(uri), queryParameters);
		}

		public static IRequest CreatePost(Uri uri, IEnumerable<KeyValuePair<string, string>> queryParameters = null)
		{
			return HTTPFactory.CreatePost(AddQueryParametersToUri(uri, queryParameters));
		}

		public static IRequest CreateGet(Uri uri, IEnumerable<KeyValuePair<string, string>> queryParameters = null)
		{
			return HTTPFactory.CreateGet(AddQueryParametersToUri(uri, queryParameters));
		}

		public static IRequest CreatePut(Uri uri, IEnumerable<KeyValuePair<string, string>> queryParameters = null)
		{
			return HTTPFactory.CreatePut(AddQueryParametersToUri(uri, queryParameters));
		}

		public static IRequest CreateDelete(Uri uri, IEnumerable<KeyValuePair<string, string>> queryParameters = null)
		{
			return HTTPFactory.CreateDelete(AddQueryParametersToUri(uri, queryParameters));
		}

		[Obsolete("Use SendRequestAsync instead")]
		public static Coroutine RequestAsync(IRequest request, RequestCompleteHandler onComplete = null, bool enableCache = true, bool useCacheOnError = false)
		{
			return SendRequestAsync(request, onComplete, enableCache, useCacheOnError);
		}

		public static Coroutine SendRequestAsync(IRequest request, RequestCompleteHandler onComplete = null, bool enableCache = true, bool useCacheOnError = false)
		{
			return CoroutineUtil.StartCoroutine(SendRequestCoroutine(request, onComplete, enableCache, useCacheOnError));
		}

		[Obsolete("Use SendRequest instead")]
		public static IRequest Request(IRequest request, bool enableCache = true, bool useCacheOnError = false)
		{
			return SendRequest(request, enableCache, useCacheOnError);
		}

		public static IRequest SendRequest(IRequest request, bool enableCache = true, bool useCacheOnError = false)
		{
			CacheInfo cacheInfo = ((!enableCache) ? null : HandleCacheBeforeRequest(request));
			if (cacheInfo == null || cacheInfo.expired)
			{
				if (request.timeout <= 0f)
				{
					request.timeout = timeout;
				}
				request.FetchResponse();
				if (cacheInfo != null)
				{
					HandleCacheAfterRequest(request, cacheInfo, useCacheOnError);
				}
			}
			if (cacheInfo != null && cacheInfo.headers != null)
			{
				byte[] bytes = LoadCacheContents(cacheInfo.filename);
				request.response = HTTPFactory.CreateResponse(cacheInfo.headers, bytes);
				request.state = RequestState.Done;
			}
			if (!request.isSuccessful)
			{
				if (request.exception != null)
				{
					LogError("Request \"" + request.uri.AbsoluteUri + "\" exception: " + request.exception);
				}
				else
				{
					LogError("Request \"" + request.uri.AbsoluteUri + "\" failed: " + request.statusMessage);
				}
			}
			return request;
		}

		private static IEnumerator SendRequestCoroutine(IRequest request, RequestCompleteHandler onComplete, bool enableCache, bool useCacheOnError)
		{
			CacheInfo cache = ((!enableCache) ? null : HandleCacheBeforeRequest(request));
			if (cache == null || cache.expired)
			{
				if (request.timeout <= 0f)
				{
					request.timeout = timeout;
				}
				float requestTimeout = request.timeout;
				float startTime = Time.realtimeSinceStartup;
				request.FetchResponseAsync();
				while (!request.isFinished)
				{
					if (requestTimeout > 0f && Time.realtimeSinceStartup - startTime > requestTimeout)
					{
						request.state = RequestState.TimedOut;
						break;
					}
					yield return null;
				}
				if (cache != null)
				{
					HandleCacheAfterRequest(request, cache, useCacheOnError);
				}
			}
			bool useCachedResponse = cache != null && cache.headers != null;
			if (useCachedResponse)
			{
				request.response = HTTPFactory.CreateResponse(bytes: LoadCacheContents(cache.filename), headers: cache.headers);
				request.state = RequestState.Done;
			}
			if (!request.isSuccessful)
			{
				if (request.exception != null)
				{
					LogError("Request \"" + request.uri.AbsoluteUri + "\" exception: " + request.exception);
				}
				else
				{
					LogError("Request \"" + request.uri.AbsoluteUri + "\" failed: " + request.statusMessage);
				}
			}
			if (onComplete != null)
			{
				onComplete(request, useCachedResponse);
			}
		}

		private static CacheInfo HandleCacheBeforeRequest(IRequest request)
		{
			if (!"GET".Equals(request.method, StringComparison.InvariantCultureIgnoreCase))
			{
				return null;
			}
			CacheInfo cacheInfo = new CacheInfo();
			try
			{
				string text = cachePath;
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
				string path = StringUtility.ToHexString(Checksum.ComputeSHA1(request.uri.ToString()));
				cacheInfo.filename = Path.Combine(text, path);
				cacheInfo.metaFilename = cacheInfo.filename + ".metadata";
				if (File.Exists(cacheInfo.filename))
				{
					try
					{
						cacheInfo.headers = LoadCacheMetaData(cacheInfo.metaFilename, out cacheInfo.savedTimeStamp);
						DateTime dateTime = cacheInfo.headers.Get("Expires", DateTime.MinValue);
						if (DateTime.UtcNow < dateTime)
						{
							cacheInfo.expired = false;
						}
						if (cacheInfo.expired)
						{
							string value = cacheInfo.headers["ETag"];
							if (!string.IsNullOrEmpty(value))
							{
								request.headers["If-None-Match"] = value;
							}
							string value2 = cacheInfo.headers["Last-Modified"];
							if (!string.IsNullOrEmpty(value2))
							{
								request.headers["If-Modified-Since"] = value2;
							}
						}
					}
					catch (Exception ex)
					{
						LogError("Could not load headers for cache: " + ex);
						cacheInfo.headers = null;
					}
				}
			}
			catch (Exception ex2)
			{
				LogError("Error handling cache, cache disabled: " + ex2);
				return null;
			}
			return cacheInfo;
		}

		private static void HandleCacheAfterRequest(IRequest request, CacheInfo cacheInfo, bool useCacheOnError)
		{
			if (request.response != null)
			{
				IResponse response = request.response;
				int status = response.status;
				Headers headers = request.response.headers;
				DateTime dateTime = headers.Get("Expires", DateTime.MinValue);
				bool flag = dateTime > DateTime.UtcNow;
				string value = headers["ETag"];
				string value2 = headers["Last-Modified"];
				if (!string.IsNullOrEmpty(value) || !string.IsNullOrEmpty(value2))
				{
					flag = true;
				}
				switch (status)
				{
				case 200:
					cacheInfo.headers = null;
					if (flag)
					{
						SaveCacheMetaData(cacheInfo.metaFilename, response.headers);
						SaveCacheContents(cacheInfo.filename, response.bytes);
					}
					break;
				case 304:
					if (cacheInfo.headers != null)
					{
						UpdateCachedHeaders(cacheInfo.headers, headers);
						SaveCacheMetaData(cacheInfo.metaFilename, cacheInfo.headers);
					}
					break;
				default:
					if (!useCacheOnError)
					{
						cacheInfo.headers = null;
					}
					break;
				}
			}
			else if (useCacheOnError)
			{
				LogWarning("No response - using cached version");
			}
			else
			{
				cacheInfo.headers = null;
			}
		}

		private static void UpdateCachedHeaders(Headers cached, Headers updated)
		{
			string[] cACHED_HEADERS_TO_UPDATE = CACHED_HEADERS_TO_UPDATE;
			foreach (string name in cACHED_HEADERS_TO_UPDATE)
			{
				if (updated.Contains(name))
				{
					cached.Remove(name);
					IEnumerable<string> all = updated.GetAll(name);
					cached.SetAll(name, all);
				}
			}
		}

		public static void ClearCache(ClearCacheFlags flags, float olderThanSeconds)
		{
			try
			{
				string path = cachePath;
				if (!Directory.Exists(path))
				{
					return;
				}
				if ((flags & ClearCacheFlags.All) != 0)
				{
					string[] files = Directory.GetFiles(path);
					string[] array = files;
					foreach (string path2 in array)
					{
						File.Delete(path2);
					}
					return;
				}
				DateTime utcNow = DateTime.UtcNow;
				string[] files2 = Directory.GetFiles(path);
				string[] array2 = files2;
				foreach (string text in array2)
				{
					if (!text.EndsWith(".metadata"))
					{
						continue;
					}
					bool flag = false;
					try
					{
						DateTime savedTimeStamp;
						Headers headers = LoadCacheMetaData(text, out savedTimeStamp);
						if ((flags & ClearCacheFlags.Old) != 0)
						{
							double totalSeconds = (utcNow - savedTimeStamp).TotalSeconds;
							if (totalSeconds > (double)olderThanSeconds)
							{
								flag = true;
							}
						}
						if (!flag && (flags & ClearCacheFlags.Expired) != 0)
						{
							DateTime dateTime = headers.Get("Expires", DateTime.MaxValue);
							if (dateTime <= utcNow)
							{
								flag = true;
							}
						}
						if (flag)
						{
							File.Delete(text);
							string text2 = text.Remove(text.Length - ".metadata".Length);
							if (text2.Length > 0)
							{
								File.Delete(text2);
							}
						}
					}
					catch (Exception ex)
					{
						LogError("Error while clearing cache: " + ex);
					}
				}
			}
			catch (Exception ex2)
			{
				LogError("Error while clearing cache: " + ex2);
			}
		}

		private static void SaveCacheMetaData(string file, Headers headers)
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(new FileStream(file, FileMode.Create)))
			{
				binaryWriter.Write(DateTime.UtcNow.ToUnixTime());
				headers.WriteTo(binaryWriter);
			}
		}

		private static Headers LoadCacheMetaData(string file, out DateTime savedTimeStamp)
		{
			using (BinaryReader binaryReader = new BinaryReader(new FileStream(file, FileMode.Open)))
			{
				savedTimeStamp = TimeUtility.FromUnixTime(binaryReader.ReadInt64());
				Headers headers = new Headers();
				headers.ReadFrom(binaryReader);
				return headers;
			}
		}

		private static void SaveCacheContents(string file, byte[] bytes)
		{
			byte[] bytes2 = bytes ?? new byte[0];
			FileUtil.WriteAllBytes(file, bytes2);
		}

		private static byte[] LoadCacheContents(string file)
		{
			byte[] array = FileUtil.ReadAllBytes(file);
			if (array.Length == 0)
			{
				array = null;
			}
			return array;
		}

		[Conditional("HTTP_SIMULATION_ENABLED")]
		private static void ApplySimulationParametersToRequest(IRequest request)
		{
			if (request.simulatedRequestLatency <= 0f)
			{
				request.simulatedRequestLatency = simulatedRequestLatency;
			}
			if (request.simulatedRequestResults == null)
			{
				request.simulatedRequestResults = simulatedRequestResults;
			}
		}

		private static void LogError(string msg)
		{
			Debug.LogError("[WebClient] " + msg);
		}

		private static void LogWarning(string msg)
		{
			Debug.LogWarning("[WebClient] " + msg);
		}

		[Conditional("HTTP_DEBUG_LOGS")]
		private static void Log(string msg)
		{
			Debug.Log("[WebClient] " + msg);
		}
	}
}
