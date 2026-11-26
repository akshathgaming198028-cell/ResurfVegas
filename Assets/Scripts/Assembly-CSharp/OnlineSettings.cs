#define ENABLE_ERROR_LOGS
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using Kiloo.Common;
using Kiloo.Plugins.DeviceUtility;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class OnlineSettings
{
	public const string DISABLED_SETTING = "disabled";

	private const string PLATFORM = "android";

	private const string URL = "/newonlinesettings.php";

	private const string FILE_SECRET = "pdvshbhkndf92k19zvbckawd92fjk";

	private const int FILEFORMAT_VERSION = 1;

	private static readonly char[] LINEBREAK_CHARS = new char[2] { '\n', '\r' };

	private static OnlineSettings _instance;

	private Dictionary<string, string> _settings = new Dictionary<string, string>();

	private bool _isDownloading;

	private GameObject _downloadGameObject;

	private Action _onFinishedDownloadingOnlineSettingsHandler;

	public Action<bool> onOnlineSettingsDownloadResult;

	private string _checksumToShow;

	public static bool isInstanced
	{
		get
		{
			return _instance != null;
		}
	}

	public static OnlineSettings instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new OnlineSettings();
			}
			return _instance;
		}
	}

	public bool isDownloading
	{
		get
		{
			return _isDownloading;
		}
	}

	private OnlineSettings()
	{
		if (Globals.DEBUG_USE_FAKE_ONLINE_SETTINGS)
		{
			// Fake server: always use the built-in snapshot, never an old saved file.
			_settings = StringUtility.ParseProperties(Globals.fakeOnlineSettings);
			LocalEventSchedule.Apply(_settings);
			SaveToFile();
		}
		else
		{
			LoadFromFile();
		}
	}

	public void AddOnFinishedDownloadingOnlineSettingsHandler(Action handler)
	{
		_onFinishedDownloadingOnlineSettingsHandler = (Action)Delegate.Combine(_onFinishedDownloadingOnlineSettingsHandler, handler);
	}

	public void RemoveOnFinishedDownloadingOnlineSettingsHandler(Action handler)
	{
		_onFinishedDownloadingOnlineSettingsHandler = (Action)Delegate.Remove(_onFinishedDownloadingOnlineSettingsHandler, handler);
	}

	public void DownloadNow()
	{
		if (!_isDownloading)
		{
			if (_downloadGameObject != null)
			{
				UnityEngine.Object.Destroy(_downloadGameObject);
			}
			_downloadGameObject = new GameObject("OnlineSettingsDownloader");
			UnityEngine.Object.DontDestroyOnLoad(_downloadGameObject);
			_downloadGameObject.AddComponent<ResurfCoroutineRunner>().StartCoroutine(DownloadCoroutine());
		}
		else
		{
			LogWarning("OnlineSettings.DownloadNow(): Already downloading");
		}
	}

	public double GetValue(string key, double defaultValue)
	{
		string value;
		if (_settings.TryGetValue(key, out value))
		{
			try
			{
				return double.Parse(value, NumberFormatInfo.InvariantInfo);
			}
			catch
			{
				LogError("OnlineSettings.GetValue() " + key + " returns the defaultValue because send string was " + value);
			}
		}
		return defaultValue;
	}

	public float GetValue(string key, float defaultValue)
	{
		string value;
		if (_settings.TryGetValue(key, out value))
		{
			try
			{
				return float.Parse(value, NumberFormatInfo.InvariantInfo);
			}
			catch
			{
				LogError("OnlineSettings.GetValue() " + key + " returns the defaultValue because send string was " + value);
			}
		}
		return defaultValue;
	}

	public int GetValue(string key, int defaultValue)
	{
		string value;
		if (_settings.TryGetValue(key, out value))
		{
			try
			{
				return int.Parse(value, NumberFormatInfo.InvariantInfo);
			}
			catch
			{
				LogError("OnlineSettings.GetValue() " + key + " returns the defaultValue because send string was " + value);
			}
		}
		return defaultValue;
	}

	public uint GetValue(string key, uint defaultValue)
	{
		string value;
		if (_settings.TryGetValue(key, out value))
		{
			try
			{
				return uint.Parse(value, NumberFormatInfo.InvariantInfo);
			}
			catch
			{
				Debug.LogError("OnlineSettings.GetValue() " + key + " returns the defaultValue because send string was " + value);
			}
		}
		return defaultValue;
	}

	public string GetValue(string key, string defaultValue)
	{
		string value;
		if (_settings.TryGetValue(key, out value))
		{
			return value;
		}
		return defaultValue;
	}

	public bool GetValue(string key, bool defaultValue)
	{
		string value;
		if (_settings.TryGetValue(key, out value))
		{
			try
			{
				return bool.Parse(value);
			}
			catch
			{
				LogError("OnlineSettings.GetValue() " + key + " returns the defaultValue because send string was " + value);
			}
		}
		return defaultValue;
	}

	public bool TryGetValue(string key, out string valueString)
	{
		return _settings.TryGetValue(key, out valueString);
	}

	public bool HasValue(string key)
	{
		return _settings.ContainsKey(key);
	}

	public string GetCurrentPlatform()
	{
		return "android";
	}

	public string GetChecksumToShow()
	{
		return _checksumToShow;
	}

	private IEnumerator DownloadCoroutine()
	{
		if (Globals.DEBUG_USE_FAKE_ONLINE_SETTINGS)
		{
			_settings = StringUtility.ParseProperties("efe40d19b11ddd485b4188664e9969f4bc89de0d\n[latestversion] 1.56.0\n[latestversion_changelist] \n\n\n* Subway Surfers in the Snow!\n* Limited Holiday character\n* Discover special Boards\n* Receive a free GIFT!\n* Optimized for iPhone 5!\n[in_app_tier_2] 20000\n[discount_end_time] 1814313600\n[videoads_providerlist] vungleclips\n[hoverboard_expiretime_liberty] 23-12-2012 22:00:00\n[hoverboard_starttime_liberty] 20-11-2012 22:00:00\n[character_expiretime_nychar] 23-12-2012 22:00:00\n[character_starttime_nychar] 13-10-2012 22:00:00\n[character_expiretime_elftricky] 23-11-2012 22:00:00\n[character_starttime_elftricky] 23-11-2012 22:00:00\n[enable_local_notifications] true[save_me_locales] en_US;en_UK;[offerwall_number_of_offers] 4[offerwall_session_timeout] 24[offerwall_offers_timeout] 24[offerwall_balance_timeout] 24[offerwal_min_reward_allowed] 10[offerwall_max_reward_allowed] 10000[offerwall_filter_free] true[save_me_locales] all[event_id]1496318400[event_value] 0;0[event_type] CharacterName[event_start] 1764547200[event_end] 1830000000[event_ribbon_text]ad3;3;6[number_of_fails_for_interstitial] 1[interstitials_limit_per_hour] 10[interstitials_limit_per_day] 40[intesrtitials_delay_seconds] 10[keys_rewarded_for_video_ad] 2");
			LocalEventSchedule.Apply(_settings);
			SaveToFile();
			Action<bool> handler = onOnlineSettingsDownloadResult;
			if (handler != null)
			{
				handler(true);
			}
		}
		else
		{
			bool downloadSucceded = false;
			StringBuilder url = ((!Globals.DEBUG_OVERRIDE_ONLINE_SETTINGS_URL) ? new StringBuilder("http://hoodrunner.kiloo.com") : new StringBuilder("http://10.0.0.169/hoodrunner"));
			url.Append("/newonlinesettings.php");
			url.Append("?platform=").Append("android");
			url.Append("&locale=").Append(DeviceUtility.GetLocale());
			string timezone = DateTime.Now.GetCurrentTimeZoneUTCOffset().TotalHours.ToString("F2", CultureInfo.InvariantCulture);
			url.Append("&timezone=").Append(timezone);
			string bundleVersion = DeviceUtility.GetBundleVersion();
			if (!string.IsNullOrEmpty(bundleVersion))
			{
				url.Append("&version=").Append(bundleVersion);
			}
			if (Globals.DEBUG_USE_ONLINESETTINGS_SET && !string.IsNullOrEmpty(Globals.ONLINESETTINGS_SET))
			{
				url.Append("&set=").Append(WWW.EscapeURL(Globals.ONLINESETTINGS_SET));
			}
			if (Globals.DEBUG_USE_ONLINESETTINGS_TESTTIME)
			{
				url.Append("&testtime=").Append(WWW.EscapeURL(DateTime.UtcNow.ToString("s", CultureInfo.InvariantCulture)));
			}
			WWW www = new WWW(url.ToString());
			yield return www;
			if (www.error != null)
			{
				LogWarning("OnlineSettings download failed: " + www.error);
			}
			else
			{
				downloadSucceded = true;
				string response = www.text;
				if (response.Contains("<html"))
				{
					LogError("OnlineSettings download failed: " + response);
				}
				else
				{
					SetCurrentChecksum(response);
					if (ValidateChecksum(response))
					{
						_settings = StringUtility.ParseProperties(www.text);
						SaveToFile();
					}
					else
					{
						Debug.LogError("OnlineSettings checksum verification failed");
					}
				}
			}
			Action<bool> handler2 = onOnlineSettingsDownloadResult;
			if (handler2 != null)
			{
				handler2(downloadSucceded);
			}
		}
		if (_onFinishedDownloadingOnlineSettingsHandler != null)
		{
			_onFinishedDownloadingOnlineSettingsHandler();
		}
		UnityEngine.Object.Destroy(_downloadGameObject);
		_downloadGameObject = null;
		_isDownloading = false;
	}

	private void SetCurrentChecksum(string response)
	{
		int num = response.IndexOfAny(LINEBREAK_CHARS);
		if (num > 0)
		{
			_checksumToShow = response.Substring(0, num);
		}
		else
		{
			_checksumToShow = null;
		}
	}

	private static bool ValidateChecksum(string response)
	{
		if (Globals.DEBUG_USE_FAKE_ONLINE_SETTINGS)
		{
			return true;
		}
		int num = response.IndexOfAny(LINEBREAK_CHARS);
		if (num > 0)
		{
			string strA = response.Substring(0, num);
			string text = response.Substring(num + 1).Trim();
			string strB = StringUtility.ToHexString(Checksum.ComputeSHA1(text + "resxrctrv7tgv7gb8h9h9u0909kllfmolkjnhghgjjkhjghg"));
			if (string.Compare(strA, strB, StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				return true;
			}
		}
		return false;
	}

	private void LoadFromFile()
	{
		try
		{
			string saveDataPath = GetSaveDataPath();
			byte[] buffer = FileUtil.Load(saveDataPath, "pdvshbhkndf92k19zvbckawd92fjk");
			MemoryStream memoryStream = new MemoryStream(buffer);
			BinaryReader binaryReader = new BinaryReader(memoryStream);
			int num = binaryReader.ReadInt32();
			_settings = FileUtil.ReadStringStringDictionary(binaryReader);
			memoryStream.Close();
		}
		catch (FileNotFoundException)
		{
		}
		catch (Exception)
		{
		}
	}

	private void SaveToFile()
	{
		try
		{
			MemoryStream memoryStream = new MemoryStream(4096);
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			binaryWriter.Write(1);
			FileUtil.WriteStringStringDictionary(binaryWriter, _settings);
			string saveDataPath = GetSaveDataPath();
			FileUtil.Save(saveDataPath, memoryStream.GetBuffer(), "pdvshbhkndf92k19zvbckawd92fjk", 0, (int)memoryStream.Length);
			memoryStream.Close();
		}
		catch (Exception ex)
		{
			LogError("Failed to save OnlineSettings to file: " + ex);
		}
	}

	private static string GetSaveDataPath()
	{
		return DeviceUtility.GetUserDataPath() + "/onlinesettings";
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	private static void LogError(string msg, UnityEngine.Object context = null)
	{
		Debug.LogError(msg, context);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	private static void LogWarning(string msg, UnityEngine.Object context = null)
	{
		Debug.LogWarning(msg, context);
	}

	[Conditional("ENABLE_ONLINESETTINGS_LOGS")]
	private static void Log(string msg, UnityEngine.Object context = null)
	{
		Debug.Log(msg, context);
	}
}
