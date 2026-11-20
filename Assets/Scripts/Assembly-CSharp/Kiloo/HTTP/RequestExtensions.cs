using System.Collections.Generic;
using System.Text;
using MiniJSON;
using UnityEngine;

namespace Kiloo.HTTP
{
	public static class RequestExtensions
	{
		public static IRequest WithContent(this IRequest request, byte[] content, string contentType)
		{
			if (content != null)
			{
				request.bytes = content;
				if (!string.IsNullOrEmpty(contentType))
				{
					request.headers["Content-Type"] = contentType;
				}
			}
			return request;
		}

		public static IRequest WithContent(this IRequest request, string content, string contentType = "text/plain; charset=utf-8", Encoding encoding = null)
		{
			if (encoding == null)
			{
				encoding = Encoding.UTF8;
			}
			return request.WithContent(encoding.GetBytes(content), contentType);
		}

		public static IRequest WithJsonContent(this IRequest request, object jsonData)
		{
			return request.WithContent(Encoding.UTF8.GetBytes(Json.Serialize(jsonData)), "application/json");
		}

		public static IRequest WithHeaders(this IRequest request, IDictionary<string, string> headers)
		{
			if (headers != null)
			{
				foreach (KeyValuePair<string, string> header in headers)
				{
					request.headers[header.Key] = header.Value;
				}
			}
			return request;
		}

		public static IRequest WithHeader(this IRequest request, string key, string value)
		{
			request.headers[key] = value;
			return request;
		}

		public static IRequest WithTimeout(this IRequest request, float timeout)
		{
			request.timeout = timeout;
			return request;
		}

		public static IRequest WithMinBackOffTime(this IRequest request, float minBackOffTime)
		{
			request.minBackOffTime = minBackOffTime;
			return request;
		}

		public static IRequest WithMaxBackOffTime(this IRequest request, float maxBackOffTime)
		{
			request.maxBackOffTime = maxBackOffTime;
			return request;
		}

		public static IRequest WithIgnoreCertificateErrors(this IRequest request, bool ignoreCertificateErrors)
		{
			request.ignoreCertificateErrors = ignoreCertificateErrors;
			return request;
		}

		public static IRequest WithIgnoreContentMD5(this IRequest request, bool ignoreContentMD5)
		{
			request.ignoreContentMD5 = ignoreContentMD5;
			return request;
		}

		public static IRequest WithSimulatedRequestLatency(this IRequest request, float simulatedRequestLatency)
		{
			request.simulatedRequestLatency = simulatedRequestLatency;
			return request;
		}

		public static IRequest WithSimulatedRequestResults(this IRequest request, IEnumerable<SimulatedRequestResult> simulatedRequestResults)
		{
			request.simulatedRequestResults = simulatedRequestResults;
			return request;
		}

		public static IRequest ThenMakeTheRequest(this IRequest request, bool enableCache = true, bool useCacheOnError = false)
		{
			return WebClient.SendRequest(request, enableCache, useCacheOnError);
		}

		public static IRequest Send(this IRequest request, bool enableCache = true, bool useCacheOnError = false)
		{
			return WebClient.SendRequest(request, enableCache, useCacheOnError);
		}

		public static Coroutine ThenMakeTheRequestAsync(this IRequest request, WebClient.RequestCompleteHandler onComplete = null, bool enableCache = true, bool useCacheOnError = false)
		{
			return WebClient.SendRequestAsync(request, onComplete, enableCache, useCacheOnError);
		}

		public static Coroutine SendAsync(this IRequest request, WebClient.RequestCompleteHandler onComplete = null, bool enableCache = true, bool useCacheOnError = false)
		{
			return WebClient.SendRequestAsync(request, onComplete, enableCache, useCacheOnError);
		}

		public static IRequest WithKilooAuth(this IRequest request, string authentication)
		{
			if (!string.IsNullOrEmpty(authentication))
			{
				if (request.uri.Scheme == "https")
				{
					request.headers["Kiloo-Auth"] = authentication;
				}
				else
				{
					LogError("Adding Kiloo-Auth header is only allowed when using https");
				}
			}
			return request;
		}

		private static void LogError(string msg)
		{
			Debug.LogError("[RequestExtensions] " + msg);
		}
	}
}
