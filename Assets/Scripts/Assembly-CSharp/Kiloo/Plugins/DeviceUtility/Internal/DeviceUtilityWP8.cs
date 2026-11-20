using System;
using System.Diagnostics;
using System.Globalization;
using Kiloo.Plugins.Utility;
using UnityEngine;

namespace Kiloo.Plugins.DeviceUtility.Internal
{
	internal class DeviceUtilityWP8 : IDeviceUtilityPlatformBridge
	{
		private const string OS_IDENTIFIER = "windows_phone_8";

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
			Debug.LogError("[DeviceUtilityWP8] " + msg);
		}

		private static void LogWarning(string msg)
		{
			Debug.LogWarning("[DeviceUtilityWP8] " + msg);
		}

		[Conditional("DEVICE_UTILITY_DEBUG_LOGS")]
		private static void Log(string msg)
		{
			Debug.Log("[DeviceUtilityWP8] " + msg);
		}

		public string GetBuildIdentifier()
		{
			return GetOSIdentifier();
		}

		public void SetKeyboardReturnType(int returnKeyType)
		{
		}

		public void SetDeviceUtilityDebugEnabled(bool val)
		{
		}

		public string GetUserDataPath()
		{
			return Application.persistentDataPath;
		}

		public string GetCachePath()
		{
			return Application.temporaryCachePath;
		}

		public string GetDeviceIdentifier()
		{
			return SystemInfo.deviceUniqueIdentifier;
		}

		public string GetBundleVersion()
		{
			if (Application.platform == RuntimePlatform.WP8Player)
			{
				string text = UtilWP8.GetVersion();
				int num = 0;
				for (int i = 0; i < text.Length; i++)
				{
					if (text[i] == '.')
					{
						num++;
						if (num > 2)
						{
							text = text.Substring(0, i);
							break;
						}
					}
				}
				return text;
			}
			return string.Empty;
		}

		public string GetBundleIdentifier()
		{
			return string.Empty;
		}

		public string GetLocale()
		{
			CultureInfo currentCulture = CultureInfo.CurrentCulture;
			if (!CultureInfo.InvariantCulture.Equals(currentCulture))
			{
				string text = currentCulture.Name;
				if (text.Length != 5 || text[2] != '-')
				{
					text = currentCulture.TwoLetterISOLanguageName;
					if (text.Length != 2)
					{
						text = "en-US";
					}
				}
				return text;
			}
			return "en-US";
		}

		public string GetOSVersion()
		{
			return Environment.OSVersion.Version.ToString(4);
		}

		public string GetOSIdentifier()
		{
			return "windows_phone_8";
		}

		public void ShowNativePopup(string title, string message, string cancelButton, string optionalButton1, string optionalButton2, Action<string> onDismissed)
		{
			if (optionalButton2 != null)
			{
				LogError("ShowNativePopup: optionalButton2 is not supported on this platform, ignored");
			}
			if (optionalButton1 == null)
			{
				if (onDismissed == null)
				{
					UtilWP8.ShowOKMessageBox(title, message);
					return;
				}
				UtilWP8.ShowOKMessageBoxWithCallback(title, message, delegate
				{
					onDismissed(cancelButton);
				});
				return;
			}
			UtilWP8.ShowOKCancelMessageBoxWithCallback(title, message, delegate(string s)
			{
				if (s == "1")
				{
					onDismissed(optionalButton1);
				}
				else
				{
					onDismissed(cancelButton);
				}
			});
		}

		public void SetDeviceClipBoard(string text)
		{
		}

		public int GetDeviceFreeMemory()
		{
			return 0;
		}

		public int GetCurrentMemoryUsage()
		{
			return (int)UtilWP8.GetCurrentMemoryUsage() / 1024 / 1024;
		}

		public int GetPeakMemoryUsage()
		{
			return (int)UtilWP8.GetPeakMemoryUsage() / 1024 / 1024;
		}

		public int GetMemoryUsageLimit()
		{
			return (int)UtilWP8.GetMemoryUsageLimit() / 1024 / 1024;
		}

		public int GetBatteryLevel()
		{
			return UtilWP8.GetBatteryPercentage();
		}

		public int GetFreeDiskSpace()
		{
			return -1;
		}

		public int GetTotalDiskSpace()
		{
			return -1;
		}

		public int GetFreeSDCardSpace()
		{
			return -1;
		}

		public int GetTotalSDCardSpace()
		{
			return -1;
		}

		public string GetAndroidExternalSavePath()
		{
			return string.Empty;
		}

		public int GetAndroidApiLevel()
		{
			return -1;
		}

		public int GetAndroidVersionCode()
		{
			return -1;
		}

		public string GetAndroidAdvertisingID()
		{
			return string.Empty;
		}

		public bool GetIOSOtherAudioPlaying()
		{
			return false;
		}

		public static DeviceUtilityWP8 CreatePlugin()
		{
			return new DeviceUtilityWP8();
		}
	}
}
