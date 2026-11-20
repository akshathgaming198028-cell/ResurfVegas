using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Kiloo.Monetization.Tapjoy.Internal
{
	public class TapjoyIOS : MonoBehaviour, ITapjoyBridge
	{
		private const string CALLBACK_RECEIVER_NAME = "TapJoyIOSReceiver";

		public event TapjoyConnectEvent onConnect;

		public event TapjoyGetFullScreenAdResponseEvent onGetFullScreenAdResponse;

		public event TapjoySpendPointsEvent onSpendPoints;

		public event TapjoySpendPointsErrorEvent onSpendPointsError;

		public event TapjoyViewWillAppearEvent onViewWillAppear;

		public event TapjoyViewDidAppearEvent onViewDidAppear;

		public event TapjoyViewWillDisappearEvent onViewWillDisappear;

		public event TapjoyViewDidDisappearEvent onViewDidDisappear;

		public event TapjoyVideoCompleteEvent onVideoComplete;

		public event TapjoyVideoStartEvent onVideoStart;

		public event TapjoyVideoErrorEvent onVideoError;

		public event TapjoyEarnedPointsEvent onEarnedPoints;

		public event TapjoyGetPointsEvent onGetPoints;

		public event TapjoyGetPointsErrorEvent onGetPointsError;

		[DllImport("__Internal")]
		private static extern void _tapjoyplugin_Initialize(string appID, string key, string eventName);

		[DllImport("__Internal")]
		private static extern void _tapjoyplugin_GetFullScreenAd();

		[DllImport("__Internal")]
		private static extern void _tapjoyplugin_ShowFullScreenAd();

		[DllImport("__Internal")]
		private static extern void _tapjoyplugin_EnableDebugLogs(bool enabled);

		public void Initialize(string appID, string appKey, string eventName)
		{
			_tapjoyplugin_Initialize(appID, appKey, eventName);
		}

		public void GetFullScreenAd()
		{
			_tapjoyplugin_GetFullScreenAd();
		}

		public void ShowFullScreenAd()
		{
			_tapjoyplugin_ShowFullScreenAd();
		}

		public void GetPoints()
		{
		}

		public void SpendPoints(int amount)
		{
		}

		private void _callback_onConnect(string result)
		{
			if (this.onConnect != null)
			{
				this.onConnect(result);
			}
		}

		private void _callback_onGetFullScreenAdResponse(string result)
		{
			if (this.onGetFullScreenAdResponse != null)
			{
				this.onGetFullScreenAdResponse(result);
			}
		}

		private void _callback_onSpendPoints(string amount)
		{
			if (this.onSpendPoints != null)
			{
				this.onSpendPoints(amount);
			}
		}

		private void _callback_onSpendPointsError()
		{
			if (this.onSpendPointsError != null)
			{
				this.onSpendPointsError();
			}
		}

		private void _callback_onViewWillAppear(string viewType)
		{
			if (this.onViewWillAppear != null)
			{
				this.onViewWillAppear(viewType);
			}
		}

		private void _callback_onViewDidAppear(string viewType)
		{
			if (this.onViewDidAppear != null)
			{
				this.onViewDidAppear(viewType);
			}
		}

		private void _callback_onViewWillDisappear(string viewType)
		{
			if (this.onViewWillDisappear != null)
			{
				this.onViewWillDisappear(viewType);
			}
		}

		private void _callback_onViewDidDisappear(string viewType)
		{
			if (this.onViewDidDisappear != null)
			{
				this.onViewDidDisappear(viewType);
			}
		}

		private void _callback_onVideoAdCompleted()
		{
			if (this.onVideoComplete != null)
			{
				this.onVideoComplete();
			}
		}

		private void _callback_onVideoAdError(string error)
		{
			if (this.onVideoError != null)
			{
				this.onVideoError(error);
			}
		}

		public static TapjoyIOS CreatePlugin()
		{
			_tapjoyplugin_EnableDebugLogs(false);
			GameObject gameObject = new GameObject("TapJoyIOSReceiver");
			Object.DontDestroyOnLoad(gameObject);
			return gameObject.AddComponent<TapjoyIOS>();
		}

		private static void LogError(string msg, Object context = null)
		{
			Debug.LogError("TapJoyIOS: " + msg, context);
		}

		private static void LogWarning(string msg, Object context = null)
		{
			Debug.LogWarning("TapJoyIOS: " + msg, context);
		}

		[Conditional("ENABLE_TAPJOY_LOGS")]
		private static void Log(string msg, Object context = null)
		{
			Debug.Log("TapJoyIOS: " + msg, context);
		}
	}
}
