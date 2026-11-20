#define ENABLE_ERROR_LOGS
using System;
using System.Diagnostics;
using UnityEngine;

public class GCMPlugin : MonoBehaviour
{
	public delegate void AndroidNotificationReceivedDelegate(string payload);

	private static string _deviceToken;

	private Action<string> _registrationCompleteHandler;

	private static GameObject callbackGameObject;

	public static string deviceToken
	{
		get
		{
			if (_deviceToken == null)
			{
				CreateCallbackGameObjectIfNecesarry();
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.kiloo.subwaysurf.GCMIntentService"))
				{
					string value = androidJavaClass.CallStatic<string>("getDeviceToken", new object[0]);
					if (!string.IsNullOrEmpty(value))
					{
						_deviceToken = value;
					}
				}
			}
			return _deviceToken;
		}
	}

	public static event AndroidNotificationReceivedDelegate onNotificationReceived;

	public static void UnregisterDevice()
	{
		_deviceToken = string.Empty;
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.kiloo.subwaysurf.GCMIntentService"))
		{
			if (!androidJavaClass.CallStatic<bool>("unregisterDevice", new object[0]))
			{
			}
		}
	}

	public static void RequestDeviceToken(Action<string> completionHandler, string soundToPlay)
	{
		if (string.IsNullOrEmpty(deviceToken))
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.kiloo.subwaysurf.GCMIntentService"))
			{
				CreateCallbackGameObjectIfNecesarry();
				GCMPlugin component = callbackGameObject.GetComponent<GCMPlugin>();
				component._registrationCompleteHandler = completionHandler;
				if (!androidJavaClass.CallStatic<bool>("registerDevice", new object[2] { callbackGameObject.name, soundToPlay }))
				{
					_deviceToken = string.Empty;
					LogError("GCMPlugin failed to register device. Something is setup wrong.");
					CompleteRegistration(completionHandler);
					UnityEngine.Object.Destroy(callbackGameObject);
				}
				return;
			}
		}
		CompleteRegistration(completionHandler);
	}

	private void OnGCMRegistrationComplete(string regId)
	{
		_deviceToken = regId;
		CompleteRegistration(_registrationCompleteHandler);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void OnGCMRegistrationError(string errorId)
	{
		LogError("GCMPlugin.OnGCMRegistrationError: " + errorId);
		_deviceToken = string.Empty;
		CompleteRegistration(_registrationCompleteHandler);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void OnNotificationReceived(string payload)
	{
		AndroidNotificationReceivedDelegate androidNotificationReceivedDelegate = GCMPlugin.onNotificationReceived;
		if (androidNotificationReceivedDelegate != null)
		{
			androidNotificationReceivedDelegate(payload);
		}
	}

	private static void CreateCallbackGameObjectIfNecesarry()
	{
		if (callbackGameObject == null)
		{
			callbackGameObject = new GameObject("GCMPluginRegistration");
			GCMPlugin gCMPlugin = callbackGameObject.AddComponent<GCMPlugin>();
			UnityEngine.Object.DontDestroyOnLoad(callbackGameObject);
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.kiloo.subwaysurf.GCMIntentService"))
			{
				androidJavaClass.CallStatic("SetCallbackObjectName", callbackGameObject.name);
			}
		}
	}

	private static void CompleteRegistration(Action<string> completionHandler)
	{
		if (completionHandler != null)
		{
			completionHandler(_deviceToken);
		}
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	public static void LogError(string msg, UnityEngine.Object context = null)
	{
		Debug.LogError(msg, context);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	public static void LogWarning(string msg, UnityEngine.Object context = null)
	{
		Debug.LogWarning(msg, context);
	}

	[Conditional("ENABLE_GCM_LOGS")]
	public static void Log(string msg, UnityEngine.Object context = null)
	{
		Debug.Log(msg, context);
	}
}
