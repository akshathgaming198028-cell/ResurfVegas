using System.Collections.Generic;
using System.Runtime.InteropServices;
using MiniJSON;

namespace Kiloo.Common.FlurryAnalytics.Internal
{
	internal class FlurryAnalyticsIOS : IFlurryAnalyticsBridge
	{
		private static FlurryAnalyticsIOS _instance;

		private static FlurryAnalyticsIOS instance
		{
			get
			{
				return _instance ?? (_instance = new FlurryAnalyticsIOS());
			}
		}

		private FlurryAnalyticsIOS()
		{
		}

		public static FlurryAnalyticsIOS CreatePlugin()
		{
			return instance;
		}

		[DllImport("__Internal")]
		private static extern void flurryStartSession(string apiKey, bool crashReportingEnabled);

		[DllImport("__Internal")]
		private static extern void flurrySetUserId(string userId);

		[DllImport("__Internal")]
		private static extern void flurrySetAge(int age);

		[DllImport("__Internal")]
		private static extern void flurrySetGender(int gender);

		[DllImport("__Internal")]
		private static extern void flurryLogEvent(string eventName);

		[DllImport("__Internal")]
		private static extern void flurryLogEventWithParams(string eventName, string jsonParams);

		[DllImport("__Internal")]
		private static extern void flurryLogTimedEvent(string eventName);

		[DllImport("__Internal")]
		private static extern void flurryLogTimedEventWithParams(string eventName, string jsonParams);

		[DllImport("__Internal")]
		private static extern void flurryEndTimedEvent(string eventName);

		[DllImport("__Internal")]
		private static extern void flurryOnPageView();

		[DllImport("__Internal")]
		private static extern void flurrySetContinueSessionMillis(int milliseconds);

		[DllImport("__Internal")]
		private static extern void flurrySetDebugLogsEnabled(bool value);

		[DllImport("__Internal")]
		private static extern void flurryLogError(string errorName, string message);

		public void StartSession(string apiKey)
		{
			flurryStartSession(apiKey, true);
		}

		public void EndSession()
		{
		}

		public void LogEvent(string eventName)
		{
			flurryLogEvent(eventName);
		}

		public void LogEventWithParams(string eventName, Dictionary<string, string> parameters)
		{
			string jsonParams = Json.Serialize(parameters);
			flurryLogEventWithParams(eventName, jsonParams);
		}

		public void LogTimedEvent(string eventName)
		{
			flurryLogTimedEvent(eventName);
		}

		public void LogTimedEventWithParams(string eventName, Dictionary<string, string> parameters)
		{
			string jsonParams = Json.Serialize(parameters);
			flurryLogTimedEventWithParams(eventName, jsonParams);
		}

		public void EndTimedEvent(string eventName)
		{
			flurryEndTimedEvent(eventName);
		}

		public void LogPageView()
		{
			flurryOnPageView();
		}

		public void LogError(string errorId, string message, string errorClass)
		{
			flurryLogError(errorId, message);
		}

		public void SetUserId(string userId)
		{
			flurrySetUserId(userId);
		}

		public void SetAge(int age)
		{
			flurrySetAge(age);
		}

		public void SetGender(Gender gender)
		{
			flurrySetGender((int)gender);
		}

		public void SetContinueSessionMillis(long milliseconds)
		{
			flurrySetContinueSessionMillis((int)milliseconds);
		}

		public void SetCaptureUncaughtExceptions(bool value)
		{
		}

		public void SetDebugLogsEnabled(bool value)
		{
			flurrySetDebugLogsEnabled(value);
		}
	}
}
