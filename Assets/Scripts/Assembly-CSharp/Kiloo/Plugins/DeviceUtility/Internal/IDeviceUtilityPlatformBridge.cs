using System;

namespace Kiloo.Plugins.DeviceUtility.Internal
{
	internal interface IDeviceUtilityPlatformBridge
	{
		bool hasPhysicalBackButton { get; }

		bool isKindleDevice { get; }

		bool isFlexionDevice { get; }

		void SetDeviceUtilityDebugEnabled(bool val);

		string GetUserDataPath();

		string GetCachePath();

		string GetDeviceIdentifier();

		string GetBundleVersion();

		string GetBundleIdentifier();

		string GetLocale();

		string GetOSVersion();

		string GetOSIdentifier();

		string GetBuildIdentifier();

		void SetKeyboardReturnType(int returnKeyType);

		void ShowNativePopup(string title, string message, string dismissButton, string optionalButton1, string optionalButton2, Action<string> onDismissed);

		void SetDeviceClipBoard(string text);

		int GetDeviceFreeMemory();

		int GetCurrentMemoryUsage();

		int GetPeakMemoryUsage();

		int GetMemoryUsageLimit();

		int GetBatteryLevel();

		int GetFreeDiskSpace();

		int GetTotalDiskSpace();

		int GetFreeSDCardSpace();

		int GetTotalSDCardSpace();

		string GetAndroidExternalSavePath();

		int GetAndroidApiLevel();

		int GetAndroidVersionCode();

		string GetAndroidAdvertisingID();

		bool GetIOSOtherAudioPlaying();
	}
}
