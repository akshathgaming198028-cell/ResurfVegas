using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kiloo.Plugins.AdColony.Internal
{
	public class AdColonyAndroid : MonoBehaviour, IAdColonyPlatformBridge
	{
		private const string CALLBACK_RECEIVER_NAME = "AdColonyBridgeReceiver";

		private AndroidJavaObject _androidBridge;

		public event AdColonyV4VCRewardEvent onV4VCReward;

		public event AdColonyAdAttemptFinishEvent onAdAttemptFinished;

		public event AdColonyAdStartEvent onAdStarted;

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogError(string msg, Object context = null)
		{
			Debug.LogError("AdColonyAndroid: " + msg, context);
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogWarning(string msg, Object context = null)
		{
			Debug.LogWarning("AdColonyAndroid: " + msg, context);
		}

		[Conditional("ENABLE_ADCOLONY_LOGS")]
		private static void Log(string msg, Object context = null)
		{
			Debug.Log("AdColonyAndroid: " + msg, context);
		}

		public void Configure(string appID, string[] zoneIDs)
		{
			bool val = false;
			_androidBridge = new AndroidJavaObject("com.kiloo.unityplugin.adcolony.AdColonyBridgeAndroid");
			_androidBridge.Set("debugLogEnabled", val);
			_androidBridge.Call("configure", appID, zoneIDs);
		}

		public void PlayVideoAdForZoneWithPopups(string zoneID, bool prePopupV4VC, bool postPopupV4VC)
		{
			_androidBridge.Call("playV4VCAd", zoneID, prePopupV4VC, postPopupV4VC);
		}

		public bool IsVirtualCurrencyRewardAvailable(string zoneID)
		{
			return _androidBridge.Call<bool>("isV4VCReady", new object[1] { zoneID });
		}

		private void _callback_onV4VCStarted(string empty)
		{
			if (this.onAdStarted != null)
			{
				this.onAdStarted();
			}
		}

		private void _callback_onV4VCFinished(string empty)
		{
			if (this.onAdAttemptFinished != null)
			{
				this.onAdAttemptFinished();
			}
		}

		private void _callback_onV4VCWatched(string msg)
		{
			if (this.onV4VCReward != null)
			{
				this.onV4VCReward(msg == "1");
			}
		}

		public static AdColonyAndroid CreatePlugin()
		{
			GameObject gameObject = new GameObject("AdColonyBridgeReceiver");
			Object.DontDestroyOnLoad(gameObject);
			return gameObject.AddComponent<AdColonyAndroid>();
		}
	}
}
