using System.Diagnostics;
using Kiloo.Monetization.Tapjoy.Internal;
using UnityEngine;

namespace Kiloo.Monetization.Tapjoy
{
	public static class TapJoy
	{
		private enum TapJoyConnectionState
		{
			Connecting = 0,
			Connected = 1,
			Disconnected = 2
		}

		public const string INITIALIZE_SUCCESS_RETURN_VALUE = "success";

		private static TapJoyConnectionState _connectionState = TapJoyConnectionState.Disconnected;

		private static ITapjoyBridge _platformBridge;

		public static bool isInitialized
		{
			get
			{
				return _platformBridge != null && _connectionState == TapJoyConnectionState.Connected;
			}
		}

		public static event TapjoyConnectEvent onInitializeFinished;

		public static event TapjoyGetFullScreenAdResponseEvent onFullScreenAdResponse;

		public static event TapjoyEarnedPointsEvent onEarnPoints;

		public static event TapjoyGetPointsEvent onGetPoints;

		public static event TapjoyGetPointsErrorEvent onGetPointsError;

		public static event TapjoySpendPointsEvent onSpendPoints;

		public static event TapjoySpendPointsErrorEvent onSpendPointsError;

		public static event TapjoyViewWillAppearEvent onViewWillAppear;

		public static event TapjoyViewDidAppearEvent onViewDidAppear;

		public static event TapjoyViewWillDisappearEvent onViewWillDisappear;

		public static event TapjoyViewDidDisappearEvent onViewDidDisappear;

		public static event TapjoyVideoStartEvent onVideoStart;

		public static event TapjoyVideoErrorEvent onVideoError;

		public static event TapjoyVideoCompleteEvent onVideoComplete;

		public static bool IsSupportedOnPlatform()
		{
			switch (Application.platform)
			{
			case RuntimePlatform.IPhonePlayer:
				return true;
			case RuntimePlatform.Android:
				return true;
			case RuntimePlatform.WindowsEditor:
				return false;
			case RuntimePlatform.OSXEditor:
				return false;
			default:
				return false;
			}
		}

		public static void Initialize(string appId, string appKey, string eventName)
		{
			if (_connectionState == TapJoyConnectionState.Connecting || isInitialized)
			{
				LogError("TapJoy has already been configured. Ignoring new parameters.");
				return;
			}
			_connectionState = TapJoyConnectionState.Connecting;
			if (_platformBridge == null)
			{
				switch (Application.platform)
				{
				case RuntimePlatform.IPhonePlayer:
					_platformBridge = TapjoyIOS.CreatePlugin();
					break;
				case RuntimePlatform.Android:
					_platformBridge = TapjoyAndroid.CreatePlugin();
					break;
				default:
					LogError("There's no platform bridge for the current platform: " + Application.platform);
					return;
				}
				_platformBridge.onConnect += OnInitializeFinished;
				_platformBridge.onEarnedPoints += OnEarnPoints;
				_platformBridge.onGetFullScreenAdResponse += OnFullScreenAdResponse;
				_platformBridge.onGetPoints += OnGetPoints;
				_platformBridge.onGetPointsError += OnGetPointsError;
				_platformBridge.onSpendPoints += OnSpendPoints;
				_platformBridge.onSpendPointsError += OnSpendPointsError;
				_platformBridge.onViewWillDisappear += OnViewWillDisappear;
				_platformBridge.onViewDidDisappear += OnViewDidDisappear;
				_platformBridge.onViewWillAppear += OnViewWillAppear;
				_platformBridge.onViewDidAppear += OnViewDidAppear;
				_platformBridge.onVideoStart += OnVideoStart;
				_platformBridge.onVideoError += OnVideoError;
				_platformBridge.onVideoComplete += OnVideoComplete;
			}
			_platformBridge.Initialize(appId, appKey, eventName);
		}

		public static void GetFullScreenAd()
		{
			if (isInitialized)
			{
				_platformBridge.GetFullScreenAd();
			}
		}

		public static void ShowFullScreenAd()
		{
			if (isInitialized)
			{
				_platformBridge.ShowFullScreenAd();
			}
		}

		public static void GetPoints()
		{
			if (isInitialized)
			{
				_platformBridge.GetPoints();
			}
		}

		public static void SpendPoints(int amount)
		{
			if (isInitialized)
			{
				_platformBridge.SpendPoints(amount);
			}
		}

		private static void OnInitializeFinished(string result)
		{
			if (result == "success")
			{
				_connectionState = TapJoyConnectionState.Connected;
			}
			else
			{
				_connectionState = TapJoyConnectionState.Disconnected;
			}
			if (TapJoy.onInitializeFinished != null)
			{
				TapJoy.onInitializeFinished(result);
			}
		}

		private static void OnFullScreenAdResponse(string result)
		{
			if (TapJoy.onFullScreenAdResponse != null)
			{
				TapJoy.onFullScreenAdResponse(result);
			}
		}

		private static void OnEarnPoints(string amount)
		{
			if (TapJoy.onEarnPoints != null)
			{
				TapJoy.onEarnPoints(amount);
			}
		}

		private static void OnGetPoints(string amount)
		{
			if (TapJoy.onGetPoints != null)
			{
				TapJoy.onGetPoints(amount);
			}
		}

		private static void OnGetPointsError()
		{
			if (TapJoy.onGetPointsError != null)
			{
				TapJoy.onGetPointsError();
			}
		}

		private static void OnSpendPoints(string amount)
		{
			if (TapJoy.onSpendPoints != null)
			{
				TapJoy.onSpendPoints(amount);
			}
		}

		private static void OnSpendPointsError()
		{
			if (TapJoy.onSpendPointsError != null)
			{
				TapJoy.onSpendPointsError();
			}
		}

		private static void OnViewWillAppear(string viewType)
		{
			if (TapJoy.onViewWillAppear != null)
			{
				TapJoy.onViewWillAppear(viewType);
			}
		}

		private static void OnViewDidAppear(string viewType)
		{
			if (TapJoy.onViewDidAppear != null)
			{
				TapJoy.onViewDidAppear(viewType);
			}
		}

		private static void OnViewWillDisappear(string viewType)
		{
			if (TapJoy.onViewWillDisappear != null)
			{
				TapJoy.onViewWillDisappear(viewType);
			}
		}

		private static void OnViewDidDisappear(string viewType)
		{
			if (TapJoy.onViewDidDisappear != null)
			{
				TapJoy.onViewDidDisappear(viewType);
			}
		}

		private static void OnVideoStart()
		{
			if (TapJoy.onVideoStart != null)
			{
				TapJoy.onVideoStart();
			}
		}

		private static void OnVideoError(string error)
		{
			if (TapJoy.onVideoError != null)
			{
				TapJoy.onVideoError(error);
			}
		}

		private static void OnVideoComplete()
		{
			if (TapJoy.onVideoComplete != null)
			{
				TapJoy.onVideoComplete();
			}
		}

		private static void LogError(string msg, Object context = null)
		{
			Debug.LogError("TapJoy: " + msg, context);
		}

		private static void LogWarning(string msg, Object context = null)
		{
			Debug.LogWarning("TapJoy: " + msg, context);
		}

		[Conditional("ENABLE_TAPJOY_LOGS")]
		private static void Log(string msg, Object context = null)
		{
			Debug.Log("TapJoy: " + msg, context);
		}
	}
}
