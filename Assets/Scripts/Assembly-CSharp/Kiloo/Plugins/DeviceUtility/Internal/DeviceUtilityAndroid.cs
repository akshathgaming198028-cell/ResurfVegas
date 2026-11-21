using System;
using System.Diagnostics;
using JetBrains.Annotations;
using Kiloo.Common;
using UnityEngine;
using Debug = UnityEngine.Debug;

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

		private bool _bridgeOk;

		private T SafeCall<T>(string method, T fallback)
		{
			if (!_bridgeOk)
			{
				return fallback;
			}
			try
			{
				return _androidBridge.Call<T>(method, new object[0]);
			}
			catch (Exception)
			{
				return fallback;
			}
		}

		private void SafeCallVoid(string method, params object[] args)
		{
			if (!_bridgeOk)
			{
				return;
			}
			try
			{
				_androidBridge.Call(method, args);
			}
			catch (Exception)
			{
			}
		}

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
			try
			{
				_androidBridge = new AndroidJavaObject("com.kiloo.unityplugin.deviceutility.DeviceUtilityBridgeAndroid");
				_bridgeOk = true;
			}
			catch (Exception)
			{
				_bridgeOk = false;
			}
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
			if (_bridgeOk) { try { _androidBridge.Set("debugLogEnabled", val); } catch (Exception) { } }
		}

		public string GetUserDataPath()
		{
			string text = null;
			text = SafeCall<string>("getUserDataPath", Application.persistentDataPath);
			if (string.IsNullOrEmpty(text))
			{
				throw new InvalidOperationException("Could not get user data path");
			}
			return text;
		}

		public string GetCachePath()
		{
			string text = null;
			text = SafeCall<string>("getCachePath", Application.temporaryCachePath);
			if (string.IsNullOrEmpty(text))
			{
				throw new InvalidOperationException("Could not get cache path");
			}
			return text;
		}

		public string GetDeviceIdentifier()
		{
			return SafeCall<string>("getDeviceIdentifier", SystemInfo.deviceUniqueIdentifier);
		}

		public string GetBundleVersion()
		{
			return SafeCall<string>("getBundleVersion", Application.version);
		}

		public string GetBundleIdentifier()
		{
			return SafeCall<string>("getBundleIdentifier", Application.identifier);
		}

		public string GetLocale()
		{
			return SafeCall<string>("getLocale", "en_US");
		}

		public string GetOSVersion()
		{
			return SafeCall<string>("getOSVersion", SystemInfo.operatingSystem);
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
			SafeCallVoid("showNativePopup", num, title, message, cancelButton, optionalButton1, optionalButton2);
		}

		public void SetDeviceClipBoard(string text)
		{
			SafeCallVoid("setDeviceClipBoard", text);
		}

		public int GetDeviceFreeMemory()
		{
			return SafeCall<int>("getDeviceFreeMemory", 0);
		}

		public int GetCurrentMemoryUsage()
		{
			return SafeCall<int>("getDeviceCurrentMemoryUsage", 0);
		}

		public int GetPeakMemoryUsage()
		{
			return 0;
		}

		public int GetMemoryUsageLimit()
		{
			return SafeCall<int>("getDeviceMemoryLimit", 0);
		}

		public int GetBatteryLevel()
		{
			return SafeCall<int>("getBatteryLevel", 100);
		}

		public int GetFreeDiskSpace()
		{
			return SafeCall<int>("getFreeDiskSpace", 0);
		}

		public int GetTotalDiskSpace()
		{
			return SafeCall<int>("getTotalDiskSpace", 0);
		}

		public int GetFreeSDCardSpace()
		{
			return SafeCall<int>("getFreeSDCardSpace", 0);
		}

		public int GetTotalSDCardSpace()
		{
			return SafeCall<int>("getTotalSDCardSpace", 0);
		}

		public string GetAndroidExternalSavePath()
		{
			return SafeCall<string>("getAndroidExternalSavePath", Application.persistentDataPath);
		}

		public int GetAndroidApiLevel()
		{
			return SafeCall<int>("getAndroidApiLevel", 0);
		}

		public int GetAndroidVersionCode()
		{
			return SafeCall<int>("getAndroidVersionCode", 0);
		}

		public string GetAndroidAdvertisingID()
		{
			return SafeCall<string>("getAndroidAdvertisingID", "");
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
