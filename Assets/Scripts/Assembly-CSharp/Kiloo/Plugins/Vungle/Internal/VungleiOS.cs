using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using MiniJSON;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kiloo.Plugins.Vungle.Internal
{
	public class VungleiOS : MonoBehaviour, IVunglePlatformBridge
	{
		public enum IOSOrientationMaskTypes
		{
			portraitOrientation = 0,
			landscapeLeftOrientation = 1,
			landscapeRightOrientation = 2,
			portraitUpsideDownOrientation = 3,
			landscapeOrientation = 4,
			allOrientation = 5,
			allButUpsideDownOrientation = 6
		}

		private const string CALLBACK_RECEIVER_NAME = "VungleIOSReceiver";

		public event VungleAdStartEvent onAdStart;

		public event VungleCachedAdAvailableEvent onCachedAdAvailable;

		public event VungleVideoViewFinishedEvent onVideoViewFinished;

		public event VungleAdUnavailableEvent onAdUnavailable;

		public event VungleWillCloseProductSheet onWillCloseProductSheet;

		private static void _vungleplugin_initialize(string appId) { }

		private static void _vungleplugin_playAd() { }

		private static void _vungleplugin_playAdWithOptions(string options) { }

		private static bool _vungleplugin_isCachedAdAvailable() { return false; }

		private static void _vungleplugin_EnableDebugLogs(bool val) { }

		public bool InitVungle(string appId)
		{
			bool result = false;
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				_vungleplugin_initialize(appId);
				result = true;
			}
			return result;
		}

		public void PlayAd()
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				_vungleplugin_playAd();
			}
		}

		public void PlayAdWithOptions(IDictionary optionsData)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				_vungleplugin_playAdWithOptions(Json.Serialize(optionsData));
			}
		}

		public bool IsCachedAdAvailable()
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				return _vungleplugin_isCachedAdAvailable();
			}
			return false;
		}

		public void OnPause()
		{
		}

		public void OnResume()
		{
		}

		private void _callback_onVideoViewFinished(string result)
		{
			if (this.onVideoViewFinished != null)
			{
				this.onVideoViewFinished(result);
			}
		}

		private void _callback_onAdStart()
		{
			if (this.onAdStart != null)
			{
				this.onAdStart();
			}
		}

		private void _callback_onCachedAdAvailable()
		{
			if (this.onCachedAdAvailable != null)
			{
				this.onCachedAdAvailable();
			}
		}

		private void _callback_onAdUnavailable(string result)
		{
			if (Application.platform != RuntimePlatform.IPhonePlayer)
			{
			}
		}

		private void _callback_willCloseProductSheet()
		{
			if (Application.platform != RuntimePlatform.Android)
			{
			}
		}

		public static VungleiOS CreatePlugin()
		{
			_vungleplugin_EnableDebugLogs(false);
			GameObject gameObject = new GameObject("VungleIOSReceiver");
			Object.DontDestroyOnLoad(gameObject);
			return gameObject.AddComponent<VungleiOS>();
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogError(string msg, Object context = null)
		{
			Debug.LogError("VungleIOS: " + msg, context);
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogWarning(string msg, Object context = null)
		{
			Debug.LogWarning("VungleIOS: " + msg, context);
		}

		[Conditional("ENABLE_VUNGLE_LOGS")]
		private static void Log(string msg, Object context = null)
		{
			Debug.Log("VungleIOS: " + msg, context);
		}
	}
}
