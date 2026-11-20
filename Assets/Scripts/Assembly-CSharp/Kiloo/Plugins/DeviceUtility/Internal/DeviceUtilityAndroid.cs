using System;
using System.Diagnostics;
using JetBrains.Annotations;
using Kiloo.Common;
using UnityEngine;

namespace Kiloo.Plugins.DeviceUtility.Internal
{
	internal class DeviceUtilityAndroid : MonoBehaviour, IDeviceUtilityPlatformBridge
	{
		private const char PARAMETER_SEPARATOR = ';';

		private const string OS_IDENTIFIER = "android";

		private const int ANDROID_BUTTON_POSITIVE = -1;

		private const int ANDROID_BUTTON_NEGATIVE = -2;

		private const int ANDROID_BUTTON_NEUTRAL = -3;

		private const string CALLBACK_RECEIVER_NAME = "DeviceUtility Receiver";

		private AndroidJavaObject _androidBridge;

		public bool hasPhysicalBackButton
		{
			get
			{
				return true;
			}
		}

		public bool isKindleDevice
		{
			get
			{
				return false;
			}
		}

		public bool isFlexionDevice
		{
			get
			{
				return false;
			}
		}

		private static void LogError(string msg)
		{
			Debug.LogError("[DeviceUtilityAndroid] " + msg);
		}

		private static void LogWarning(string msg)
		{
			Debug.LogWarning("[DeviceUtilityAndroid] " + msg);
		}

		[Conditional("DEVICE_UTILITY_DEBUG_LOGS")]
		private static void Log(string msg)
		{
			Debug.Log("[DeviceUtilityAndroid] " + msg);
		}

		[UsedImplicitly]
		private void Awake()
		{
			_androidBridge = new AndroidJavaObject("com.kiloo.unityplugin.deviceutility.DeviceUtilityBridgeAndroid");
		}

		[UsedImplicitly]
		private void _callback_alertViewDidDismissWithButtonIndex(string message)
		{
			try
			{
				string[] array = message.Split(';');
				if (array.Length == 2)
				{
					Action<int> action = Depository.CheckOut(int.Parse(array[0])) as Action<int>;
					if (action != null)
					{
						action(int.Parse(array[1]));
					}
					return;
				}
			}
			catch (Exception)
			{
			}
			LogError("Malformed dialog callback message");
		}

		public string GetBuildIdentifier()
		{
			return (!isKindleDevice) ? GetOSIdentifier() : "kindle";
		}

		public void SetKeyboardReturnType(int returnKeyType)
		{
		}

		public void SetDeviceUtilityDebugEnabled(bool val)
		{
			_androidBridge.Set("debugLogEnabled", val);
		}

		public string GetUserDataPath()
		{
			string text = null;
			text = _androidBridge.Call<string>("getUserDataPath", new object[0]);
			if (string.IsNullOrEmpty(text))
			{
				throw new InvalidOperationException("Could not get user data path");
			}
			return text;
		}

		public string GetCachePath()
		{
			string text = null;
			text = _androidBridge.Call<string>("getCachePath", new object[0]);
			if (string.IsNullOrEmpty(text))
			{
				throw new InvalidOperationException("Could not get cache path");
			}
			return text;
		}

		public string GetDeviceIdentifier()
		{
			return _androidBridge.Call<string>("getDeviceIdentifier", new object[0]);
		}

		public string GetBundleVersion()
		{
			return _androidBridge.Call<string>("getBundleVersion", new object[0]);
		}

		public string GetBundleIdentifier()
		{
			return _androidBridge.Call<string>("getBundleIdentifier", new object[0]);
		}

		public string GetLocale()
		{
			return _androidBridge.Call<string>("getLocale", new object[0]);
		}

		public string GetOSVersion()
		{
			return _androidBridge.Call<string>("getOSVersion", new object[0]);
		}

		public string GetOSIdentifier()
		{
			return "android";
		}

		public void ShowNativePopup(string title, string message, string cancelButton, string optionalButton1, string optionalButton2, Action<string> onDismissed)
		{
			int num = 0;
			if (onDismissed != null)
			{
				Action<int> deposit = delegate(int buttonPressedIndex)
				{
					switch (buttonPressedIndex)
					{
					case -1:
						onDismissed(optionalButton1);
						break;
					case -3:
						onDismissed(optionalButton2);
						break;
					default:
						onDismissed(cancelButton);
						break;
					}
				};
				num = Depository.CheckIn(deposit, this);
			}
			_androidBridge.Call("showNativePopup", num, title, message, cancelButton, optionalButton1, optionalButton2);
		}

		public void SetDeviceClipBoard(string text)
		{
			_androidBridge.Call("setDeviceClipBoard", text);
		}

		public int GetDeviceFreeMemory()
		{
			return _androidBridge.Call<int>("getDeviceFreeMemory", new object[0]);
		}

		public int GetCurrentMemoryUsage()
		{
			return _androidBridge.Call<int>("getDeviceCurrentMemoryUsage", new object[0]);
		}

		public int GetPeakMemoryUsage()
		{
			return 0;
		}

		public int GetMemoryUsageLimit()
		{
			return _androidBridge.Call<int>("getDeviceMemoryLimit", new object[0]);
		}

		public int GetBatteryLevel()
		{
			return _androidBridge.Call<int>("getBatteryLevel", new object[0]);
		}

		public int GetFreeDiskSpace()
		{
			return _androidBridge.Call<int>("getFreeDiskSpace", new object[0]);
		}

		public int GetTotalDiskSpace()
		{
			return _androidBridge.Call<int>("getTotalDiskSpace", new object[0]);
		}

		public int GetFreeSDCardSpace()
		{
			return _androidBridge.Call<int>("getFreeSDCardSpace", new object[0]);
		}

		public int GetTotalSDCardSpace()
		{
			return _androidBridge.Call<int>("getTotalSDCardSpace", new object[0]);
		}

		public string GetAndroidExternalSavePath()
		{
			return _androidBridge.Call<string>("getAndroidExternalSavePath", new object[0]);
		}

		public int GetAndroidApiLevel()
		{
			return _androidBridge.Call<int>("getAndroidApiLevel", new object[0]);
		}

		public int GetAndroidVersionCode()
		{
			return _androidBridge.Call<int>("getAndroidVersionCode", new object[0]);
		}

		public string GetAndroidAdvertisingID()
		{
			return _androidBridge.Call<string>("getAndroidAdvertisingID", new object[0]);
		}

		public bool GetIOSOtherAudioPlaying()
		{
			LogWarning("GetIOSOtherAudioPlaying is not supported on this platform.");
			return false;
		}

		public static DeviceUtilityAndroid CreatePlugin()
		{
			GameObject gameObject = new GameObject("DeviceUtility Receiver");
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			return gameObject.AddComponent<DeviceUtilityAndroid>();
		}
	}
}
