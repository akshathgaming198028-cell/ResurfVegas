using System.Collections;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kiloo.Plugins.AdColony.Internal
{
	public class AdColonySimulated : MonoBehaviour, IAdColonyPlatformBridge
	{
		private const bool V4VC_AVAILABLE = true;

		private const string CURRENCY_NAME = "Kredits";

		private const int CURRENCY_AMOUNT = 1;

		private const float ERROR_PROBABILITY = 0.25f;

		private const string CALLBACK_RECEIVER_NAME = "AdColonyBridgeReceiver";

		private bool _adsReady;

		private bool _showingAd;

		public event AdColonyV4VCRewardEvent onV4VCReward;

		public event AdColonyAdAttemptFinishEvent onAdAttemptFinished;

		public event AdColonyAdStartEvent onAdStarted;

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogError(string msg, Object context = null)
		{
			Debug.LogError("AdColonySimulated: " + msg, context);
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogWarning(string msg, Object context = null)
		{
			Debug.LogWarning("AdColonySimulated: " + msg, context);
		}

		[Conditional("ENABLE_ADCOLONY_LOGS")]
		private static void Log(string msg, Object context = null)
		{
			Debug.Log("AdColonySimulated: " + msg, context);
		}

		public void Configure(string appID, string[] zoneIDs)
		{
			StartCoroutine(SimConfigure(zoneIDs));
		}

		public void PlayVideoAdForZoneWithPopups(string zoneID, bool prePopupV4VC, bool postPopupV4VC)
		{
			SimPlayVideo(zoneID);
		}

		public bool IsVirtualCurrencyRewardAvailable(string zoneID)
		{
			return true && _adsReady;
		}

		private IEnumerator SimConfigure(string[] zoneIDs)
		{
			yield return new WaitForSeconds(Random.Range(0.5f, 3f));
			_adsReady = true;
		}

		private void SimPlayVideo(string zoneID)
		{
			if (_adsReady)
			{
				if (Random.Range(0f, 1f) <= 0.25f)
				{
					if (this.onAdAttemptFinished != null)
					{
						this.onAdAttemptFinished();
					}
					return;
				}
				_showingAd = true;
				if (this.onAdStarted != null)
				{
					this.onAdStarted();
				}
			}
			else if (this.onAdAttemptFinished != null)
			{
				this.onAdAttemptFinished();
			}
		}

		private void OnGUI()
		{
			if (!_showingAd)
			{
				return;
			}
			GUI.Box(new Rect(0f, 0f, Screen.width, Screen.height), "AdColony Video");
			if (GUI.Button(new Rect(0f, Screen.height / 2 + 55, 300f, 50f), "Close"))
			{
				_showingAd = false;
				if (this.onAdAttemptFinished != null)
				{
					this.onAdAttemptFinished();
				}
				if (this.onV4VCReward != null)
				{
					this.onV4VCReward(true);
				}
			}
		}

		public static AdColonySimulated CreatePlugin()
		{
			GameObject gameObject = new GameObject("AdColonyBridgeReceiver");
			Object.DontDestroyOnLoad(gameObject);
			return gameObject.AddComponent<AdColonySimulated>();
		}
	}
}
