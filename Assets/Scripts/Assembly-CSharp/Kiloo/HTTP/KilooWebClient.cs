using System;
using System.Collections.Generic;
using System.Diagnostics;
using Kiloo.Common;
using Kiloo.Plugins.DeviceUtility;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kiloo.HTTP
{
	public static class KilooWebClient
	{
		private static Dictionary<string, string> _clientInfoHeaders;

		public static bool disableCache { get; set; }

		public static void InitAutomatically(string appId)
		{
			string language;
			string country;
			GetLanguageAndCountry(out language, out country);
			float deviceTimeZone = (float)DateTime.Now.GetCurrentTimeZoneUTCOffset().TotalHours;
			string text = DeviceUtility.GetBundleVersion();
			if (string.IsNullOrEmpty(text))
			{
				text = "0.0";
			}
			InitWithClientInfo(appId, text, GetPlatformIdentifier(), SystemInfo.deviceModel, GetOSIdentifier(), SystemInfo.operatingSystem, language, country, deviceTimeZone);
		}

		public static void InitWithClientInfo(string appId, string appVersion, string platformId, string deviceModel, string osId, string osVersion, string deviceLanguage, string deviceCountry, float deviceTimeZone)
		{
			if (string.IsNullOrEmpty(appId) || string.IsNullOrEmpty(appVersion) || string.IsNullOrEmpty(platformId) || string.IsNullOrEmpty(deviceModel) || string.IsNullOrEmpty(osId) || string.IsNullOrEmpty(osVersion) || string.IsNullOrEmpty(deviceLanguage) || string.IsNullOrEmpty(deviceCountry) || deviceTimeZone < -12f || deviceTimeZone > 14f)
			{
				LogError("Initialize: Invalid initialization parameters");
				return;
			}
			int num = (int)deviceTimeZone;
			float num2 = deviceTimeZone - (float)num;
			int num3 = Mathf.Abs(Mathf.RoundToInt(num2 * 60f));
			string value = string.Format("{0:+00;-00}:{1:00}", num, num3);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("Kiloo-App-Identifier", appId);
			dictionary.Add("Kiloo-App-Version", appVersion);
			dictionary.Add("Kiloo-Build-Identifier", platformId);
			dictionary.Add("Kiloo-Device-Model", deviceModel);
			dictionary.Add("Kiloo-Os-Identifier", osId);
			dictionary.Add("Kiloo-Os-Version", osVersion);
			dictionary.Add("Kiloo-Device-Language", deviceLanguage);
			dictionary.Add("Kiloo-Device-Country", deviceCountry);
			dictionary.Add("Kiloo-Device-Time-Zone", value);
			_clientInfoHeaders = dictionary;
		}

		public static IRequest CreatePost(Uri uri)
		{
			return CreateRequest("POST", uri);
		}

		public static IRequest CreateGet(Uri uri)
		{
			return CreateRequest("GET", uri);
		}

		public static IRequest CreatePut(Uri uri)
		{
			return CreateRequest("PUT", uri);
		}

		public static IRequest CreateDelete(Uri uri)
		{
			return CreateRequest("DELETE", uri);
		}

		private static IRequest CreateRequest(string method, Uri uri)
		{
			IRequest request = HTTPFactory.CreateRequest(method, uri);
			Headers headers = request.headers;
			headers["Connection"] = "close";
			if (_clientInfoHeaders != null)
			{
				headers.AddAll(_clientInfoHeaders);
			}
			else
			{
				LogError("Client info not set. Call SetClientInfo before making requests.");
			}
			return request;
		}

		private static void GetLanguageAndCountry(out string language, out string country)
		{
			string locale = DeviceUtility.GetLocale();
			string[] array = locale.Split('-');
			language = array[0].ToLowerInvariant();
			if (string.IsNullOrEmpty(language))
			{
				language = "en";
			}
			country = ((array.Length <= 1) ? null : array[1].ToUpperInvariant());
			if (string.IsNullOrEmpty(country))
			{
				country = "US";
			}
		}

		private static string GetOSIdentifier()
		{
			switch (Application.platform)
			{
			case RuntimePlatform.Android:
				return "android";
			case RuntimePlatform.IPhonePlayer:
				return "ios";
			case RuntimePlatform.WP8Player:
				return "windows_phone_8";
			default:
				return "other";
			}
		}

		private static bool isKindleDevice()
		{
			return false;
		}

		private static string GetPlatformIdentifier()
		{
			return (!isKindleDevice()) ? GetOSIdentifier() : "kindle";
		}

		private static void LogError(string msg)
		{
			Debug.LogError("[KilooWebClient] " + msg);
		}

		[Conditional("HTTP_DEBUG_LOGS")]
		private static void Log(string msg)
		{
			Debug.Log("[KilooWebClient] " + msg);
		}
	}
}
