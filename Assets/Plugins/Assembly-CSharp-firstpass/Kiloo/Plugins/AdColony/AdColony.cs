#define ENABLE_ERROR_LOGS
using System.Diagnostics;
using Kiloo.Plugins.AdColony.Internal;
using UnityEngine;

namespace Kiloo.Plugins.AdColony
{
	public static class AdColony
	{
		private static IAdColonyPlatformBridge _platformBridge;

		public static event AdColonyV4VCRewardEvent onV4VCReward;

		public static event AdColonyAdAttemptFinishEvent onAdAttemptFinished;

		public static event AdColonyAdStartEvent onAdStarted;

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogError(string msg, Object context = null)
		{
			Debug.LogError("AdColony: " + msg, context);
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogWarning(string msg, Object context = null)
		{
			Debug.LogWarning("AdColony: " + msg, context);
		}

		[Conditional("ENABLE_ADCOLONY_LOGS")]
		private static void Log(string msg, Object context = null)
		{
			Debug.Log("AdColony: " + msg, context);
		}

		private static void OnV4VCRewardCallback(bool success)
		{
			if (AdColony.onV4VCReward != null)
			{
				AdColony.onV4VCReward(success);
			}
		}

		private static void OnAdAttemptFinishedCallback()
		{
			if (AdColony.onAdAttemptFinished != null)
			{
				AdColony.onAdAttemptFinished();
			}
		}

		private static void OnAdStartedCallback()
		{
			if (AdColony.onAdStarted != null)
			{
				AdColony.onAdStarted();
			}
		}

		private static bool CheckConfigured()
		{
			if (_platformBridge == null)
			{
				LogError("AdColony must be configured before this call.");
				return false;
			}
			return true;
		}

		public static bool IsSupportedOnPlatform()
		{
			switch (Application.platform)
			{
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.IPhonePlayer:
			case RuntimePlatform.Android:
				return true;
			default:
				return false;
			}
		}

		public static void Configure(string appID, string[] zoneIDs)
		{
			if (_platformBridge != null)
			{
				LogError("AdColony has already been configured. Ignoring new parameters.");
				return;
			}
			switch (Application.platform)
			{
			case RuntimePlatform.IPhonePlayer:
				_platformBridge = AdColonyIOS.CreatePlugin();
				break;
			case RuntimePlatform.Android:
				_platformBridge = AdColonyAndroid.CreatePlugin();
				break;
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.WindowsEditor:
				_platformBridge = AdColonySimulated.CreatePlugin();
				break;
			default:
				LogError("There's no platform bridge for the current platform.");
				return;
			}
			_platformBridge.onV4VCReward += OnV4VCRewardCallback;
			_platformBridge.onAdStarted += OnAdStartedCallback;
			_platformBridge.onAdAttemptFinished += OnAdAttemptFinishedCallback;
			_platformBridge.Configure(appID, zoneIDs);
		}

		public static void PlayVideoAdForZoneWithPopups(string zoneID, bool prePopupV4VC = false, bool postPopupV4VC = false)
		{
			if (CheckConfigured())
			{
				_platformBridge.PlayVideoAdForZoneWithPopups(zoneID, prePopupV4VC, postPopupV4VC);
			}
		}

		public static bool IsVirtualCurrencyAwardAvailable(string zoneID)
		{
			if (!CheckConfigured())
			{
				return false;
			}
			return _platformBridge.IsVirtualCurrencyRewardAvailable(zoneID);
		}
	}
}
