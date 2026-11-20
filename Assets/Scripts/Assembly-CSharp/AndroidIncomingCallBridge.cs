using System.Diagnostics;
using UnityEngine;

public class AndroidIncomingCallBridge : MonoBehaviour
{
	public delegate void IncomingCallEvent(string state);

	private const string CALLBACK_RECEIVER_NAME = "IncomingCallReceiverBridge";

	private AndroidJavaObject _androidBridge;

	public event IncomingCallEvent onPhoneStateChanged;

	public static AndroidIncomingCallBridge CreatePlugin()
	{
		GameObject gameObject = new GameObject("IncomingCallReceiverBridge");
		Object.DontDestroyOnLoad(gameObject);
		return gameObject.AddComponent<AndroidIncomingCallBridge>();
	}

	public void Init()
	{
		if (_androidBridge == null && Application.platform == RuntimePlatform.Android)
		{
			_androidBridge = new AndroidJavaObject("com.kiloo.unityplugins.IncomingCallReceiver");
		}
	}

	private void _callback_onAudioStateChanged(string isModeNormal)
	{
		if (this.onPhoneStateChanged != null)
		{
			this.onPhoneStateChanged(isModeNormal);
		}
	}

	[Conditional("ENABLE_INCOMINGCALL_LOGS")]
	private static void LogError(string msg, Object context = null)
	{
		Debug.LogError("AndroidIncomingCallBridge: " + msg, context);
	}

	[Conditional("ENABLE_INCOMINGCALL_LOGS")]
	private static void LogWarning(string msg, Object context = null)
	{
		Debug.LogWarning("AndroidIncomingCallBridge: " + msg, context);
	}

	[Conditional("ENABLE_INCOMINGCALL_LOGS")]
	private static void Log(string msg, Object context = null)
	{
		Debug.Log("AndroidIncomingCallBridge: " + msg, context);
	}
}
