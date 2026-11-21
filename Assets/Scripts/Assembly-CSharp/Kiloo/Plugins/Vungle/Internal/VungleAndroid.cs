using System.Collections;
using System.Diagnostics;
using MiniJSON;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kiloo.Plugins.Vungle.Internal
{
	public class VungleAndroid : MonoBehaviour, IVunglePlatformBridge
	{
		public enum AndroidOrientationOptions
		{
			autoRotate = 0,
			matchVideo = 1
		}

		private const string CALLBACK_RECEIVER_NAME = "VungleAndroidReceiver";

		private AndroidJavaObject _androidBridge;

		public event VungleVideoViewFinishedEvent onVideoViewFinished;

		public event VungleAdStartEvent onAdStart;

		public event VungleCachedAdAvailableEvent onCachedAdAvailable;

		public event VungleAdUnavailableEvent onAdUnavailable;

		public event VungleWillCloseProductSheet onWillCloseProductSheet;

		private void _callback_onVideoView(string result)
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

		private void _callback_onCachedAdAvailable(string result)
		{
			if (this.onAdUnavailable != null)
			{
				this.onAdUnavailable(result);
			}
		}

		private void _callback_onWillCloseProductSheet()
		{
			if (Application.platform != RuntimePlatform.Android)
			{
			}
		}

		public bool InitVungle(string appId)
		{
			bool flag = false;
			if (_androidBridge == null)
			{
				AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
				_androidBridge = new AndroidJavaObject("com.kiloo.vungleplugin.VungleMain", androidJavaObject);
			}
			_androidBridge.Set("enableDebugLogs", false);
			return _androidBridge.Call<bool>("initialize", new object[1] { appId });
		}

		public void PlayAd()
		{
			_androidBridge.Call("playAd");
		}

		public void PlayAdWithOptions(IDictionary optionsData)
		{
			_androidBridge.Call("playAdWithOptions", Json.Serialize(optionsData));
		}

		public bool IsCachedAdAvailable()
		{
			bool flag = false;
			return _androidBridge.Call<bool>("isCachedAdAvailable", new object[0]);
		}

		public void OnPause()
		{
			_androidBridge.Call("onPause");
		}

		public void OnResume()
		{
			_androidBridge.Call("onResume");
		}

		public static VungleAndroid CreatePlugin()
		{
			GameObject gameObject = new GameObject("VungleAndroidReceiver");
			Object.DontDestroyOnLoad(gameObject);
			return gameObject.AddComponent<VungleAndroid>();
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogError(string msg, Object context = null)
		{
			Debug.LogError("VungleAndroid: " + msg, context);
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogWarning(string msg, Object context = null)
		{
			Debug.LogWarning("VungleAndroid: " + msg, context);
		}

		[Conditional("ENABLE_VUNGLE_LOGS")]
		private static void Log(string msg, Object context = null)
		{
			Debug.Log("VungleAndroid: " + msg, context);
		}
	}
}
