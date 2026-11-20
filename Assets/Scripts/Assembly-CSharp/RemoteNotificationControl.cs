using UnityEngine;

public class RemoteNotificationControl
{
	public delegate void RemoteNotificationsEnabledHandler(string token, string oldToken);

	private const string CACHED_DEVICE_TOKEN_KEY = "remote_notifications_cached_device_token";

	private static bool _cachedDeviceTokenLoaded;

	private static string _cachedDeviceToken;

	public static void Disable()
	{
		PlayerInfo.Instance.remoteNotificationsEnabled = false;
	}

	public static void Enable(RemoteNotificationsEnabledHandler completionHandler, string soundToPlay)
	{
		PlayerInfo.Instance.remoteNotificationsEnabled = true;
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			if (completionHandler != null)
			{
				completionHandler(GetDeviceToken(), null);
			}
		}
		else
		{
			if (Application.platform != RuntimePlatform.Android)
			{
				return;
			}
			string deviceToken = GCMPlugin.deviceToken;
			if (string.IsNullOrEmpty(deviceToken))
			{
				GCMPlugin.RequestDeviceToken(delegate(string token)
				{
					string oldToken2 = CacheTokenAndGetOldIfChanged(token);
					completionHandler(token, oldToken2);
				}, soundToPlay);
			}
			else if (completionHandler != null)
			{
				string oldToken = CacheTokenAndGetOldIfChanged(deviceToken);
				completionHandler(deviceToken, oldToken);
			}
		}
	}

	private static string CacheTokenAndGetOldIfChanged(string token)
	{
		if (!_cachedDeviceTokenLoaded)
		{
			_cachedDeviceToken = PlayerPrefs.GetString("remote_notifications_cached_device_token", null);
			_cachedDeviceTokenLoaded = true;
		}
		if (token != _cachedDeviceToken)
		{
			string cachedDeviceToken = _cachedDeviceToken;
			if (token == null)
			{
				PlayerPrefs.DeleteKey("remote_notifications_cached_device_token");
			}
			else
			{
				PlayerPrefs.SetString("remote_notifications_cached_device_token", token);
			}
			_cachedDeviceToken = token;
			return cachedDeviceToken;
		}
		return null;
	}

	public static string GetDeviceToken()
	{
		if (PlayerInfo.Instance.remoteNotificationsEnabled && Application.platform != RuntimePlatform.IPhonePlayer && Application.platform == RuntimePlatform.Android)
		{
			return GCMPlugin.deviceToken;
		}
		return string.Empty;
	}

	private static string ByteArrayToHex(byte[] barray)
	{
		char[] array = new char[barray.Length * 2];
		for (int i = 0; i < barray.Length; i++)
		{
			byte b = (byte)(barray[i] >> 4);
			array[i * 2] = (char)((b <= 9) ? (b + 48) : (b + 55));
			b = (byte)(barray[i] & 0xF);
			array[i * 2 + 1] = (char)((b <= 9) ? (b + 48) : (b + 55));
		}
		return new string(array);
	}
}
