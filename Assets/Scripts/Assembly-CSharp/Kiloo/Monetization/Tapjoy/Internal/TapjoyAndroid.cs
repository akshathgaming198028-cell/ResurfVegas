using System.Diagnostics;
using UnityEngine;

namespace Kiloo.Monetization.Tapjoy.Internal
{
	public class TapjoyAndroid : MonoBehaviour, ITapjoyBridge
	{
		private const string CALLBACK_RECEIVER_NAME = "TapjoyAndroidReceiver";

		private AndroidJavaObject _androidBridge;

		public event TapjoyConnectEvent onConnect;

		public event TapjoyGetFullScreenAdResponseEvent onGetFullScreenAdResponse;

		public event TapjoyViewDidAppearEvent onViewDidAppear;

		public event TapjoyViewDidDisappearEvent onViewDidDisappear;

		public event TapjoyVideoStartEvent onVideoStart;

		public event TapjoyVideoErrorEvent onVideoError;

		public event TapjoyVideoCompleteEvent onVideoComplete;

		public event TapjoyEarnedPointsEvent onEarnedPoints;

		public event TapjoyGetPointsEvent onGetPoints;

		public event TapjoyGetPointsErrorEvent onGetPointsError;

		public event TapjoySpendPointsEvent onSpendPoints;

		public event TapjoySpendPointsErrorEvent onSpendPointsError;

		public event TapjoyViewWillAppearEvent onViewWillAppear;

		public event TapjoyViewWillDisappearEvent onViewWillDisappear;

		public void Initialize(string appID, string appKey, string eventName)
		{
			_androidBridge = new AndroidJavaObject("com.tapjoy.TapjoyBridge.TapjoyBridge");
			_androidBridge.Set("enableDebugLogs", false);
			_androidBridge.Call("Initialize", appID, appKey, eventName);
		}

		public void GetFullScreenAd()
		{
			_androidBridge.Call("GetFullScreenAd");
		}

		public void ShowFullScreenAd()
		{
			_androidBridge.Call("ShowFullScreenAd");
		}

		public void GetPoints()
		{
			_androidBridge.Call("GetPoints");
		}

		public void SpendPoints(int amount)
		{
			_androidBridge.Call("SpendPoints", amount);
		}

		private void _callback_onConnect(string result)
		{
			if (this.onConnect != null)
			{
				this.onConnect(result);
			}
		}

		private void _callback_onGetPoints(string amount)
		{
			if (this.onGetPoints != null)
			{
				this.onGetPoints(amount);
			}
		}

		private void _callback_onGetPointsError()
		{
			if (this.onGetPointsError != null)
			{
				this.onGetPointsError();
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

		private void _callback_onEarnedPoints(string amount)
		{
			if (this.onEarnedPoints != null)
			{
				this.onEarnedPoints(amount);
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

		private void _callback_onGetFullScreenAdResponse(string result)
		{
			if (this.onGetFullScreenAdResponse != null)
			{
				this.onGetFullScreenAdResponse(result);
			}
		}

		private void _callback_onVideoStart()
		{
			if (this.onVideoStart != null)
			{
				this.onVideoStart();
			}
		}

		private void _callback_onVideoError(string error)
		{
			if (this.onVideoError != null)
			{
				this.onVideoError(error);
			}
		}

		private void _callback_onVideoComplete()
		{
			if (this.onVideoComplete != null)
			{
				this.onVideoComplete();
			}
		}

		public static TapjoyAndroid CreatePlugin()
		{
			GameObject gameObject = new GameObject("TapjoyAndroidReceiver");
			Object.DontDestroyOnLoad(gameObject);
			return gameObject.AddComponent<TapjoyAndroid>();
		}

		private static void LogError(string msg, Object context = null)
		{
			Debug.LogError("TapjoyAndroid: " + msg, context);
		}

		private static void LogWarning(string msg, Object context = null)
		{
			Debug.LogWarning("TapjoyAndroid: " + msg, context);
		}

		[Conditional("ENABLE_TAPJOY_LOGS")]
		private static void Log(string msg, Object context = null)
		{
			Debug.Log("TapjoyAndroid: " + msg, context);
		}
	}
}
