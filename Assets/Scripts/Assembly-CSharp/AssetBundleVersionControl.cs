using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Kiloo.Plugins.DeviceUtility;
using MiniJSON;
using SYBO.Core.Components.Singleton;
using SYBO.Subway.Info;
using UnityEngine;

public class AssetBundleVersionControl
{
	private const string VERSION_INFO_SAVE_PATH = "AssetBundleVersion";

	private const string VERSION_INFO_FILE_NAME = "AssetBundleVersionInfo";

	private const string ASSET_BUNDLE_MANIFESTS_RESOURCES_PATH = "AssetBundleManifests/";

	public const string DOWNLOADABLE_BUNDLE_DATA_FILENAME = "DownloadableAssetBundlesManifest";

	public const string SHIPPED_BUNDLE_DATA_FILENAME = "ShippedAssetBundlesManifest";

	private const string DEBUG_TAG = "[AssetBundleVersionControl]: ";

	private const string ONLINE_SETTINGS_BUNDLES_VERSION_KEY = "asset_bundles_version";

	private const string ONLINE_SETTINGS_UPDATED_ASSET_BUNDLES = "updated_bundles_list";

	private const char UPDATED_BUNDLE_SEPARATOR = ';';

	private const char UPDATED_BUNDLE_VALUE_SEPARATOR = ':';

	private const string MANIFESTS_IOS_FOLDER = "ios";

	private const string MANIFESTS_ANDROID_FOLDER = "android";

	private const string MANIFESTS_WP_FOLDER = "wp";

	private const string MANIFESTS_EDITOR_FOLDER = "editor";

	private const string MANIFESTS_DEAULT_FOLDER = "editor";

	private const int DEFAULT_BUNDLES_VERSION = 1;

	private bool _initialized;

	private bool _eventsChanged;

	private string _versionInfoFilePath = string.Empty;

	private Dictionary<string, int> _availableBundlesData;

	private Dictionary<string, int> _shippedBundlesData;

	private Dictionary<string, int> _downloadableBundlesData;

	private Dictionary<string, int> _neededBundles;

	public Action OnDownloadableAssetBundlesUpdated;

	private static AssetBundleVersionControl _instance;

	public static AssetBundleVersionControl Instance
	{
		get
		{
			return _instance ?? (_instance = new AssetBundleVersionControl());
		}
		private set
		{
		}
	}

	private AssetBundleVersionControl()
	{
		Initialize();
	}

	public void Initialize()
	{
		if (_initialized)
		{
			return;
		}
		_versionInfoFilePath = Path.Combine(Path.Combine(DeviceUtility.GetUserDataPath(), "AssetBundleVersion"), "AssetBundleVersionInfo");
		DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(DeviceUtility.GetUserDataPath(), "AssetBundleVersion"));
		if (!directoryInfo.Exists)
		{
			directoryInfo.Create();
		}
		_shippedBundlesData = ReadAssetBundleManifest(false);
		_downloadableBundlesData = ReadAssetBundleManifest(true);
		string lastSeenVersion = PlayerPrefs.GetString("lastSeenBundleVersionKey", string.Empty);
		if (HasAppBeenUpdated(lastSeenVersion) || IsFreshInstall(lastSeenVersion))
		{
			PlayerInfo.Instance.LatestAvailableAssetBundlesVersion = OnlineSettings.instance.GetValue("asset_bundles_version", 1);
			PlayerInfo.Instance.SaveIfDirty();
			if (_shippedBundlesData == null || _downloadableBundlesData == null)
			{
				LogError("Unable to read manifests. Aborting initialization.");
				return;
			}
			CreateAvailableBundlesVersionInfo(_shippedBundlesData, _downloadableBundlesData);
		}
		else
		{
			LoadAvailableBundlesData();
		}
		InitializeNeededBundles();
		OnlineSettings.instance.AddOnFinishedDownloadingOnlineSettingsHandler(OnOnlineSettingsDownloaded);
		SSEventsManagerController.OnSSEventStartedHandler += OnEventChanged;
		SSEventsManagerController.OnSSEventFinishedHandler += OnEventChanged;
		SSEventsManagerController.OnSSEventChangedHandler += OnEventChanged;
		_initialized = true;
	}

	private void OnEventChanged()
	{
		_eventsChanged = true;
	}

	private bool IsFreshInstall(string lastSeenVersion)
	{
		return string.IsNullOrEmpty(lastSeenVersion);
	}

	private bool HasAppBeenUpdated(string lastSeenVersion)
	{
		string bundleVersion = DeviceUtility.GetBundleVersion();
		return !lastSeenVersion.Equals(bundleVersion);
	}

	private void OnOnlineSettingsDownloaded()
	{
		OnlineSettings instance = OnlineSettings.instance;
		PlayerInfo instance2 = PlayerInfo.Instance;
		if (!instance.HasValue("asset_bundles_version"))
		{
			return;
		}
		int value = instance.GetValue("asset_bundles_version", instance2.LatestAvailableAssetBundlesVersion);
		if (value > instance2.LatestAvailableAssetBundlesVersion)
		{
			instance2.LatestAvailableAssetBundlesVersion = value;
			instance2.SaveIfDirty();
		}
		if (!instance.HasValue("updated_bundles_list"))
		{
			return;
		}
		Dictionary<string, int> dictionary = ParseUpdatedBundles(instance.GetValue("updated_bundles_list", string.Empty));
		if (dictionary == null || !dictionary.Any())
		{
			return;
		}
		foreach (KeyValuePair<string, int> item in dictionary)
		{
			if (_availableBundlesData.ContainsKey(item.Key) && item.Value > _availableBundlesData[item.Key])
			{
				_availableBundlesData[item.Key] = item.Value;
			}
		}
		SaveAvailableBundlesData();
		if (OnDownloadableAssetBundlesUpdated != null)
		{
			OnDownloadableAssetBundlesUpdated();
		}
	}

	private Dictionary<string, int> ParseUpdatedBundles(string serializedUpdatedBundles)
	{
		if (string.IsNullOrEmpty(serializedUpdatedBundles))
		{
			return null;
		}
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		string[] array = serializedUpdatedBundles.Split(';');
		string[] array2 = array;
		foreach (string text in array2)
		{
			string[] array3 = text.Split(':');
			if (array3.Length != 2)
			{
				LogError("Updated bundle data formatting is wrong. Aborting bundle data update.");
			}
			else
			{
				dictionary.Add(array3[0], int.Parse(array3[1]));
			}
		}
		return dictionary;
	}

	private void CreateAvailableBundlesVersionInfo(Dictionary<string, int> shippedData, Dictionary<string, int> downloadableData)
	{
		DeleteAvailableBundlesData();
		_availableBundlesData = new Dictionary<string, int>(shippedData);
		foreach (KeyValuePair<string, int> downloadableDatum in downloadableData)
		{
			if (_availableBundlesData.ContainsKey(downloadableDatum.Key))
			{
				LogError("Repeated key " + downloadableDatum.Key);
				if (downloadableDatum.Value > _availableBundlesData[downloadableDatum.Key])
				{
					_availableBundlesData[downloadableDatum.Key] = downloadableDatum.Value;
				}
			}
			else
			{
				_availableBundlesData.Add(downloadableDatum.Key, downloadableDatum.Value);
			}
		}
		SaveAvailableBundlesData();
	}

	public Dictionary<string, int> GetLatestAvailableVersionForAllAssetBundles()
	{
		return _availableBundlesData;
	}

	public Dictionary<string, int> GetShippedBundles()
	{
		return _shippedBundlesData;
	}

	private void InitializeNeededBundles()
	{
		if (_neededBundles == null)
		{
			_neededBundles = new Dictionary<string, int>();
		}
		_neededBundles.Clear();
		List<KeyValuePair<Characters.CharacterType, Characters.Model>> characterList = CharacterScreenManager.Instance.GetCharacterList();
		if (_neededBundles != null)
		{
			_neededBundles.Clear();
		}
		else
		{
			_neededBundles = new Dictionary<string, int>();
		}
		foreach (KeyValuePair<Characters.CharacterType, Characters.Model> item in characterList)
		{
			AddNeededBundles(_neededBundles, item.Key);
		}
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
		if (activeEventManager == null)
		{
			return;
		}
		switch (activeEventManager.GetEventType())
		{
		case SSEventType.CharacterDay:
		{
			CharacterDayEventManager typeOfActiveEventManager2 = SSEventsManagerController.instance.GetTypeOfActiveEventManager<CharacterDayEventManager>();
			if (typeOfActiveEventManager2 != null)
			{
				bool found = false;
				Characters.CharacterType eventValue = typeOfActiveEventManager2.GetEventValue<Characters.CharacterType>(out found);
				if (found)
				{
					AddNeededBundles(_neededBundles, eventValue);
				}
			}
			break;
		}
		case SSEventType.ExpiredLimitedCharacter:
		{
			ExpiredLimitedCharacterEventManager typeOfActiveEventManager3 = SSEventsManagerController.instance.GetTypeOfActiveEventManager<ExpiredLimitedCharacterEventManager>();
			if (typeOfActiveEventManager3 != null)
			{
				bool found2 = false;
				Characters.CharacterType eventValue2 = typeOfActiveEventManager3.GetEventValue<Characters.CharacterType>(out found2);
				if (found2)
				{
					AddNeededBundles(_neededBundles, eventValue2);
				}
			}
			break;
		}
		case SSEventType.CharacterName:
		{
			CharacterNameEventManager typeOfActiveEventManager = SSEventsManagerController.instance.GetTypeOfActiveEventManager<CharacterNameEventManager>();
			if (typeOfActiveEventManager == null)
			{
				break;
			}
			List<Characters.CharacterType> characterList2 = typeOfActiveEventManager.GetCharacterList();
			{
				foreach (Characters.CharacterType item2 in characterList2)
				{
					AddNeededBundles(_neededBundles, item2);
				}
				break;
			}
		}
		case SSEventType.BiggerJackpot:
		case SSEventType.MysteryPowerup:
		case SSEventType.ExpiredLimitedBoard:
			break;
		}
	}

	private void AddNeededBundles(Dictionary<string, int> neededBundles, Characters.CharacterType character)
	{
		string[] referencedAssetBundleNames = SingletonMonobehavior<Factories>.Instance.GetReferencedAssetBundleNames(character);
		string[] array = referencedAssetBundleNames;
		foreach (string text in array)
		{
			int availableShippedVersionForBundle = GetAvailableShippedVersionForBundle(text);
			if (availableShippedVersionForBundle != -1 && !neededBundles.ContainsKey(text))
			{
				neededBundles.Add(text, availableShippedVersionForBundle);
			}
		}
	}

	public Dictionary<string, int> GetNeededBundles()
	{
		if (_eventsChanged)
		{
			InitializeNeededBundles();
		}
		return _neededBundles;
	}

	public int GetLatestVersionForBundle(string bundleName)
	{
		return (!_availableBundlesData.ContainsKey(bundleName)) ? (-1) : _availableBundlesData[bundleName];
	}

	public int GetAvailableShippedVersionForBundle(string bundleName)
	{
		return (!_shippedBundlesData.ContainsKey(bundleName)) ? (-1) : _shippedBundlesData[bundleName];
	}

	public bool IsBundleVersionShipped(string bundleName, int version)
	{
		return _shippedBundlesData.ContainsKey(bundleName) && _shippedBundlesData[bundleName] == version;
	}

	public int GetCurrentlyAvailableBundleVersion()
	{
		return PlayerInfo.Instance.LatestAvailableAssetBundlesVersion;
	}

	private Dictionary<string, int> ReadAssetBundleManifest(bool downloadables)
	{
		string filename = ((!downloadables) ? "ShippedAssetBundlesManifest" : "DownloadableAssetBundlesManifest");
		string manifestPath = GetManifestPath(filename);
		try
		{
			TextAsset textAsset = Resources.Load<TextAsset>(manifestPath);
			if ((bool)textAsset)
			{
				Dictionary<string, object> dictionary = Json.Deserialize(textAsset.text) as Dictionary<string, object>;
				Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
				if (dictionary == null)
				{
					LogError("Deserialized JSon is null. Returning empty manifest.");
					return dictionary2;
				}
				foreach (KeyValuePair<string, object> item in dictionary)
				{
					dictionary2.Add(item.Key, Convert.ToInt32(item.Value));
				}
				return dictionary2;
			}
			LogError("Attempt to load " + manifestPath + " from resources failed.");
		}
		catch (Exception ex)
		{
			LogError("Exception caught attempting to load " + manifestPath + ":" + ex);
		}
		LogError("Unable to read " + manifestPath);
		return null;
	}

	private string GetManifestPath(string filename)
	{
		string empty = string.Empty;
		switch (Application.platform)
		{
		case RuntimePlatform.Android:
			empty = "android";
			break;
		case RuntimePlatform.IPhonePlayer:
			empty = "ios";
			break;
		case RuntimePlatform.MetroPlayerARM:
			empty = "wp";
			break;
		case RuntimePlatform.OSXEditor:
		case RuntimePlatform.WindowsEditor:
			empty = "editor";
			break;
		default:
			empty = "editor";
			LogError("No manifest folder found for platform: " + Application.platform);
			break;
		}
		string path = Path.Combine("AssetBundleManifests/", empty);
		return Path.Combine(path, filename);
	}

	private void LoadAvailableBundlesData()
	{
		if (!File.Exists(_versionInfoFilePath))
		{
			LogError("Attempting to load saved version info, however the file does not exist.Recreating version info from manifests.");
			CreateAvailableBundlesVersionInfo(_shippedBundlesData, _downloadableBundlesData);
			return;
		}
		StreamReader streamReader = File.OpenText(_versionInfoFilePath);
		string serializedData = streamReader.ReadToEnd();
		_availableBundlesData = AssetBundleManager.ParseAssetBundleVersionData(serializedData);
		streamReader.Close();
	}

	private void SaveAvailableBundlesData()
	{
		File.WriteAllText(_versionInfoFilePath, AssetBundleManager.SerializeAssetBundleVersionData(_availableBundlesData));
	}

	private void DeleteAvailableBundlesData()
	{
		if (File.Exists(_versionInfoFilePath))
		{
			File.Delete(_versionInfoFilePath);
		}
	}

	[Conditional("ASSET_BUNDLES_DEBUG_LOGS")]
	private void Log(string msg)
	{
		Debug.Log("[AssetBundleVersionControl]: " + msg);
	}

	[Conditional("ASSET_BUNDLES_DEBUG_LOGS")]
	private void LogWarning(string msg)
	{
		Debug.LogWarning("[AssetBundleVersionControl]: " + msg);
	}

	private void LogError(string msg)
	{
		Debug.LogError("[AssetBundleVersionControl]: " + msg);
	}
}
