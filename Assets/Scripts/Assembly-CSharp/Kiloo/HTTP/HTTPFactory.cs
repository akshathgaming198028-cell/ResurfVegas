using System;
using Kiloo.HTTP.Internal;

namespace Kiloo.HTTP
{
	public static class HTTPFactory
	{
		public static IRequest CreateGet(Uri uri)
		{
			return CreateRequest("GET", uri);
		}

		public static IRequest CreatePost(Uri uri)
		{
			return CreateRequest("POST", uri);
		}

		public static IRequest CreatePut(Uri uri)
		{
			return CreateRequest("PUT", uri);
		}

		public static IRequest CreateDelete(Uri uri)
		{
			return CreateRequest("DELETE", uri);
		}

		public static IRequest CreateRequest(string method, Uri uri)
		{
			return new Request(method, uri);
		}

		public static IRequest CreateRequest(string method, Uri uri, byte[] bytes)
		{
			return new Request(method, uri, bytes);
		}

		public static IRequest CreateRequest(string method, string uri)
		{
			return new Request(method, uri);
		}

		public static IRequest CreateRequest(string method, string uri, byte[] bytes)
		{
			return new Request(method, uri, bytes);
		}

		public static IResponse CreateResponse()
		{
			return new Response();
		}

		public static IResponse CreateResponse(Headers headers, byte[] bytes)
		{
			return new Response(headers, bytes);
		}

		public static IResponse CreateResponse(int status, string message, Headers headers, byte[] bytes)
		{
			return new Response(status, message, headers, bytes);
		}
	}
}
