using System;
using System.Collections.Generic;
using Kiloo.Plugins.FlurryAnalyticsWP8;

namespace Kiloo.Common.FlurryAnalytics.Internal
{
	internal class FlurryAnalyticsWP8 : IFlurryAnalyticsBridge
	{
		private static FlurryAnalyticsWP8 _instance;

		private IFlurryAnalyticsNative _plugin;

		private static FlurryAnalyticsWP8 instance
		{
			get
			{
				return _instance ?? (_instance = new FlurryAnalyticsWP8());
			}
		}

		private FlurryAnalyticsWP8()
		{
			if (FlurryAnalyticsNative.isSupportedOnPlatform)
			{
				_plugin = FlurryAnalyticsNative.Create();
			}
		}

		public static FlurryAnalyticsWP8 CreatePlugin()
		{
			return instance;
		}

		public void StartSession(string apiKey)
		{
			if (_plugin != null)
			{
				_plugin.StartSession(apiKey);
			}
		}

		public void EndSession()
		{
			if (_plugin != null)
			{
				_plugin.EndSession();
			}
		}

		public void LogEvent(string eventName)
		{
			if (_plugin != null)
			{
				_plugin.LogEvent(eventName);
			}
		}

		public void LogEventWithParams(string eventName, Dictionary<string, string> parameters)
		{
			if (_plugin != null)
			{
				_plugin.LogEventWithParams(eventName, parameters);
			}
		}

		public void LogTimedEvent(string eventName)
		{
			if (_plugin != null)
			{
				_plugin.LogTimedEvent(eventName);
			}
		}

		public void LogTimedEventWithParams(string eventName, Dictionary<string, string> parameters)
		{
			if (_plugin != null)
			{
				_plugin.LogEventWithParams(eventName, parameters);
			}
		}

		public void EndTimedEvent(string eventName)
		{
			if (_plugin != null)
			{
				_plugin.EndTimedEvent(eventName);
			}
		}

		public void LogPageView()
		{
			if (_plugin != null)
			{
				_plugin.OnPageView();
			}
		}

		public void LogError(string errorId, string message, string errorClass)
		{
			if (_plugin != null)
			{
				_plugin.OnError(errorId, new Exception(message));
			}
		}

		public void SetUserId(string userId)
		{
			if (_plugin != null)
			{
				_plugin.SetUserId(userId);
			}
		}

		public void SetAge(int age)
		{
			if (_plugin != null)
			{
				_plugin.SetAge(age);
			}
		}

		public void SetGender(Gender gender)
		{
			if (_plugin != null)
			{
				_plugin.SetGender((int)gender);
			}
		}

		public void SetContinueSessionMillis(long milliseconds)
		{
			if (_plugin != null)
			{
				_plugin.SetContinueSessionMillis((int)milliseconds);
			}
		}

		public void SetCaptureUncaughtExceptions(bool value)
		{
		}

		public void SetDebugLogsEnabled(bool value)
		{
		}
	}
}
