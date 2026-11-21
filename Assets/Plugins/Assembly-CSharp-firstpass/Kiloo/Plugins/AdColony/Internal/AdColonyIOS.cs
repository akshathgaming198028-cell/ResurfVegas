using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kiloo.Plugins.AdColony.Internal
{
	public class AdColonyIOS : MonoBehaviour, IAdColonyPlatformBridge
	{
		private const string CALLBACK_RECEIVER_NAME = "AdColonyBridgeReceiver";

		public event AdColonyV4VCRewardEvent onV4VCReward;

		public event AdColonyAdAttemptFinishEvent onAdAttemptFinished;

		public event AdColonyAdStartEvent onAdStarted;

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogError(string msg, Object context = null)
		{
			Debug.LogError("AdColonyIOS: " + msg, context);
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogWarning(string msg, Object context = null)
		{
			Debug.LogWarning("AdColonyIOS: " + msg, context);
		}

		[Conditional("ENABLE_ADCOLONY_LOGS")]
		private static void Log(string msg, Object context = null)
		{
			Debug.Log("AdColonyIOS: " + msg, context);
		}

		[DllImport("__Internal")]
		private static extern void _adcolonyplugin_configure(string appID, int zoneIDcount, string[] zoneIDs, bool debug);

		[DllImport("__Internal")]
		private static extern void _adcolonyplugin_playVideoAdForZoneWithPopups(string zoneID, bool prePopupV4VC, bool postPopupV4VC);

		[DllImport("__Internal")]
		private static extern bool _adcolonyplugin_isVirtualCurrencyRewardAvailableForZone(string zoneID);

		public void Configure(string appID, string[] zoneIDs)
		{
			bool debug = false;
			_adcolonyplugin_configure(appID, zoneIDs.Length, zoneIDs, debug);
		}

		public void PlayVideoAdForZoneWithPopups(string zoneID, bool prePopupV4VC, bool postPopupV4VC)
		{
			_adcolonyplugin_playVideoAdForZoneWithPopups(zoneID, prePopupV4VC, postPopupV4VC);
		}

		public bool IsVirtualCurrencyRewardAvailable(string zoneID)
		{
			return _adcolonyplugin_isVirtualCurrencyRewardAvailableForZone(zoneID);
		}

		private void _callback_onAdAttemptFinished(string empty)
		{
			if (this.onAdAttemptFinished != null)
			{
				this.onAdAttemptFinished();
			}
		}

		private void _callback_onAdStartedInZone(string empty)
		{
			if (this.onAdStarted != null)
			{
				this.onAdStarted();
			}
		}

		private void _callback_onV4VCReward(string msg)
		{
			if (this.onV4VCReward != null)
			{
				this.onV4VCReward(msg == "1");
			}
		}

		public static AdColonyIOS CreatePlugin()
		{
			GameObject gameObject = new GameObject("AdColonyBridgeReceiver");
			Object.DontDestroyOnLoad(gameObject);
			return gameObject.AddComponent<AdColonyIOS>();
		}
	}
}
