using System.Diagnostics;
using UnityEngine;

namespace Kiloo.Monetization.Inmobi.Internal
{
	public class InmobiSimulated : MonoBehaviour, IInmobiPlatformBridge
	{
		private const string CALLBACK_RECEIVER_NAME = "InmobiBridgeReceiver";

		public event InmobiCallback didCacheAd;

		public event InmobiCallback didFailToLoadAd;

		public event InmobiCallback didDismissAd;

		public event InmobiCallback willShowAd;

		public event InmobiCallback didCompleteIncentive;

		private InmobiSimulated()
		{
		}

		private static void LogError(string msg, Object context = null)
		{
			Debug.LogError("[InmobiSimulated] " + msg, context);
		}

		private static void LogWarning(string msg, Object context = null)
		{
			Debug.LogWarning("[InmobiSimulated] " + msg, context);
		}

		[Conditional("INMOBI_LOGS")]
		private static void Log(string msg, Object context = null)
		{
			Debug.Log("[InmobiSimulated] " + msg, context);
		}

		public void Init(string appId)
		{
		}

		public void CacheAd(string key, string appId)
		{
		}

		public void ShowAd(string key, string appId)
		{
		}

		public bool HasCachedAd(string key, string appId)
		{
			return false;
		}

		public static InmobiSimulated CreatePlugin()
		{
			GameObject gameObject = new GameObject("InmobiBridgeReceiver");
			Object.DontDestroyOnLoad(gameObject);
			return gameObject.AddComponent<InmobiSimulated>();
		}
	}
}
