using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Kiloo;
using Kiloo.Common;
using Kiloo.HTTP;
using Kiloo.Plugins.DeviceUtility;
using SYBO.Core.Components.Singleton;
using SYBO.Subway;
using SYBO.Subway.Info;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class AssetBundleManager
{
	public const string COMPRESSION_SIGNATURE = "subway_bundles";

	private const string ASSET_BUNDLE_RESOURCES_PATH = "AssetBundles";

	private const string DEBUG_TAG = "[AssetBundleManager]: ";

	private const char VALUE_SEPARATOR = '-';

	private const char ITEM_SEPARATOR = ',';

	private const string ASSET_BUNDLE_SERVER_URL_OS_KEY = "asset_bundle_server";

	private const string ASSET_BUNDLE_SERVER_IOS_FOLDER_OS_KEY = "asset_bundle_server_ios_folder";

	private const string ASSET_BUNDLE_SERVER_ANDROID_FOLDER_OS_KEY = "asset_bundle_server_android_folder";

	private const string ASSET_BUNDLE_SERVER_WP_FOLDER_OS_KEY = "asset_bundle_server_wp_folder";

	private const string ASSET_BUNDLE_REQUEST_TIMEOUT_OS_KEY = "asset_bundle_request_timeout";

	private const string ASSET_BUNDLE_REQUEST_MAX_BACKOFF_TIME_OS_KEY = "asset_bundle_request_max_backoff_time";

	private const string ASSET_BUNDLE_DOWNLOAD_REST_TIME_OS_KEY = "asset_bundle_download_rest_time";

	private const string ASSET_BUNDLE_IS_USING_EXTENSION_OS_KEY = "asset_bundle_using_extension";

	private const string ASSET_BUNDLE_EXTENSION_OS_KEY = "asset_bundle_extension";

	private static string _assetBundleBaseServerUrl = "http://az827068.vo.msecnd.net/subway/";

	private static string _serverDirIos = "ios";

	private static string _serverDirAndroid = "android";

	private static string _serverDirWindowsPhone = "wp";

	private static float _requestDownloadTimeout = 30f;

	private static float _requestMaxBackoffTime = 300f;

	private static float _downloadAttemptRestTime = 10f;

	private static bool _usingBundleExtension = true;

	private static string _assetBundleExtension = ".bytes";

	private AssetBundleVersionControl _versionControl;

	private List<string> _assetBundlesLoading = new List<string>();

	private Dictionary<string, AssetBundle> _loadedAssetBundles = new Dictionary<string, AssetBundle>();

	private Dictionary<string, bool> _pendingUnloads = new Dictionary<string, bool>();

	private Dictionary<string, int> _pendingDownloads;

	private bool _isDownloading;

	private float _lastInitializedDownloads = 0f - _downloadAttemptRestTime;

	private bool _downloadableBundlesUpdated;

	private IEnumerator _currentDownloaderCoroutine;

	private static AssetBundleManager _instance;

	private int attemptingToPrecacheAmount;

	private int succesfullyPrecached;

	private int attemptingToDownloadAmount;

	private int succesfullyDownloaded;

	private bool IsDownloading
	{
		get
		{
			return _isDownloading;
		}
		set
		{
			_isDownloading = value;
			if (!value)
			{
				StartDownloadIfPossible();
			}
		}
	}

	public static AssetBundleManager Instance
	{
		get
		{
			return _instance ?? (_instance = new AssetBundleManager());
		}
		private set
		{
		}
	}

	private AssetBundleManager()
	{
		AssetBundleCache.Initialize();
		_versionControl = AssetBundleVersionControl.Instance;
		AssetBundleVersionControl versionControl = _versionControl;
		versionControl.OnDownloadableAssetBundlesUpdated = (Action)Delegate.Combine(versionControl.OnDownloadableAssetBundlesUpdated, new Action(OnDownloadableAssetBundlesUpdated));
		Game.OnRunStarted = (Action)Delegate.Combine(Game.OnRunStarted, new Action(StopDownloading));
		Game.OnMenuScreenShown = (Action)Delegate.Combine(Game.OnMenuScreenShown, new Action(StartDownloadIfPossible));
		InitializeDownloadRequests();
	}

	private string GetAssetBundlesServerURL(string bundleName)
	{
		OnlineSettings instance = OnlineSettings.instance;
		string text = Application.version + "/v" + _versionControl.GetCurrentlyAvailableBundleVersion() + "/";
		switch (Application.platform)
		{
		case RuntimePlatform.Android:
			text += instance.GetValue("asset_bundle_server_android_folder", _serverDirAndroid);
			break;
		case RuntimePlatform.IPhonePlayer:
			text += instance.GetValue("asset_bundle_server_ios_folder", _serverDirIos);
			break;
		case RuntimePlatform.MetroPlayerARM:
			text += instance.GetValue("asset_bundle_server_wp_folder", _serverDirWindowsPhone);
			break;
		case RuntimePlatform.OSXEditor:
		case RuntimePlatform.WindowsEditor:
			text += _serverDirIos;
			break;
		}
		if (string.IsNullOrEmpty(text))
		{
			LogError("No Asset Bundle directory found for platform: " + Application.platform);
		}
		string path = Path.Combine(instance.GetValue("asset_bundle_server", _assetBundleBaseServerUrl), text);
		return Path.Combine(path, bundleName.ToLower() + ((!instance.GetValue("asset_bundle_using_extension", _usingBundleExtension)) ? string.Empty : instance.GetValue("asset_bundle_extension", _assetBundleExtension)));
	}

	public string GetPreCachedBundles()
	{
		if (attemptingToPrecacheAmount == succesfullyPrecached)
		{
			return "Complete(" + attemptingToPrecacheAmount + ")";
		}
		return string.Empty + succesfullyPrecached + "/" + attemptingToPrecacheAmount;
	}

	public string GetDownloadedBundles()
	{
		if (attemptingToDownloadAmount == succesfullyDownloaded)
		{
			return "Complete(" + attemptingToDownloadAmount + ")";
		}
		return string.Empty + succesfullyDownloaded + "/" + attemptingToDownloadAmount;
	}

	public AssetBundle GetAssetBundle(string bundleName)
	{
		AssetBundle value = null;
		if (_loadedAssetBundles.TryGetValue(bundleName, out value))
		{
			return value;
		}
		if (!_assetBundlesLoading.Contains(bundleName))
		{
			_assetBundlesLoading.Add(bundleName);
			try
			{
				return RequestAssetBundle(bundleName);
			}
			catch (Exception ex)
			{
				LogError("Exception caught while trying to retrieve an AssetBundle: " + bundleName + "Error: " + ex);
			}
		}
		return null;
	}

	private AssetBundle RequestAssetBundle(string bundleName)
	{
		AssetBundle assetBundle = null;
		int latestVersionForBundle = _versionControl.GetLatestVersionForBundle(bundleName);
		int versionForCachedBundle = AssetBundleCache.GetVersionForCachedBundle(bundleName);
		if (latestVersionForBundle < 0)
		{
			LogError("No version information exists for bundle :" + bundleName);
		}
		else if (latestVersionForBundle > versionForCachedBundle && !_versionControl.IsBundleVersionShipped(bundleName, latestVersionForBundle))
		{
			AddAssetBundleDownloadRequest(bundleName, latestVersionForBundle);
		}
		if (versionForCachedBundle < 0)
		{
			int availableShippedVersionForBundle = _versionControl.GetAvailableShippedVersionForBundle(bundleName);
			if (availableShippedVersionForBundle >= 0)
			{
				CacheAssetBundle(bundleName, availableShippedVersionForBundle);
				versionForCachedBundle = AssetBundleCache.GetVersionForCachedBundle(bundleName);
			}
		}
		if (versionForCachedBundle >= 0)
		{
			assetBundle = AssetBundleCache.LoadAssetBundle(bundleName, versionForCachedBundle);
		}
		if (assetBundle != null)
		{
			if (_loadedAssetBundles.ContainsKey(bundleName))
			{
				if (_loadedAssetBundles[bundleName] != null)
				{
				}
				_loadedAssetBundles[bundleName] = assetBundle;
			}
			else
			{
				_loadedAssetBundles.Add(bundleName, assetBundle);
			}
		}
		else
		{
			LogError("AssetBundle request finished unsuccessfully for bundle: " + bundleName);
		}
		if (_assetBundlesLoading.Contains(bundleName))
		{
			_assetBundlesLoading.Remove(bundleName);
		}
		return assetBundle;
	}

	public IEnumerator<float> CacheNeededAssetBundles()
	{
		Debug.Log("Caching needed asset bundles.");
		Dictionary<string, int> neededBundles = _versionControl.GetNeededBundles();
		int nShippedBundles = neededBundles.Count;
		int nBundlesProcessed = 0;
		succesfullyPrecached = 0;
		attemptingToPrecacheAmount = 0;
		foreach (string bundleName in neededBundles.Keys)
		{
			int shippedVersion = _versionControl.GetAvailableShippedVersionForBundle(bundleName);
			if (shippedVersion >= 0)
			{
				if (!AssetBundleCache.IsAssetBundleCached(bundleName, neededBundles[bundleName]))
				{
					attemptingToPrecacheAmount++;
					if (CacheAssetBundle(bundleName, _versionControl.GetAvailableShippedVersionForBundle(bundleName)))
					{
						succesfullyPrecached++;
					}
				}
			}
			else if (!AssetBundleCache.IsAssetBundleCached(bundleName, AssetBundleVersionControl.Instance.GetLatestVersionForBundle(bundleName)))
			{
				AddAssetBundleDownloadRequest(bundleName, _versionControl.GetLatestVersionForBundle(bundleName));
			}
			nBundlesProcessed++;
			yield return (float)nBundlesProcessed / (float)nShippedBundles;
		}
	}

	public void PerformAvailabilityCheck()
	{
		Dictionary<string, int> neededBundles = _versionControl.GetNeededBundles();
		bool flag = true;
		foreach (KeyValuePair<string, int> item in neededBundles)
		{
			if (!flag)
			{
				break;
			}
			int versionForCachedBundle = AssetBundleCache.GetVersionForCachedBundle(item.Key);
			if (versionForCachedBundle == -1)
			{
				flag = CacheAssetBundle(item.Key, item.Value);
			}
		}
		if (!flag)
		{
			LogError("We do not have all the necessary bundles cached. Please free some disk space and restart the App.");
			Flurry.LogEvent("Asset Bundles Availability Check Failed");
		}
	}

	private bool CacheAssetBundle(string bundleName, int version, byte[] bundleBytes = null)
	{
		if (bundleBytes != null)
		{
			return AssetBundleCache.CacheAssetBundle(bundleName, version, bundleBytes);
		}
		TextAsset textAsset = Resources.Load<TextAsset>(Path.Combine("AssetBundles", bundleName));
		if ((bool)textAsset)
		{
			byte[] bytes = textAsset.bytes.Decompress("subway_bundles");
			return AssetBundleCache.CacheAssetBundle(bundleName, version, bytes);
		}
		LogError("Cannot load bundle from resources: " + Path.Combine("AssetBundles", bundleName));
		return false;
	}

	public void UnloadAssetBundle(string bundleName, bool unloadAllLoadedObjects)
	{
		if (_pendingUnloads.ContainsKey(bundleName))
		{
			_pendingUnloads[bundleName] = unloadAllLoadedObjects;
		}
		else
		{
			_pendingUnloads.Add(bundleName, unloadAllLoadedObjects);
		}
		CoroutineUtil.StartCoroutine(UnloadBundlesCoroutine());
	}

	private IEnumerator UnloadBundlesCoroutine()
	{
		yield return new WaitForEndOfFrame();
		if (_pendingUnloads.Count <= 0)
		{
			yield break;
		}
		List<string> successfullyUnloadedBundles = new List<string>();
		foreach (KeyValuePair<string, bool> bundleToUnload in _pendingUnloads)
		{
			string bundleName = bundleToUnload.Key;
			bool unloadAllLoadedObjects = bundleToUnload.Value;
			if (!_assetBundlesLoading.Contains(bundleName))
			{
				if (_loadedAssetBundles.ContainsKey(bundleName) && _loadedAssetBundles[bundleName] != null)
				{
					_loadedAssetBundles[bundleName].Unload(unloadAllLoadedObjects);
					_loadedAssetBundles.Remove(bundleName);
				}
				successfullyUnloadedBundles.Add(bundleName);
			}
		}
		foreach (string unloadedBundle in successfullyUnloadedBundles)
		{
			_pendingUnloads.Remove(unloadedBundle);
		}
	}

	private void InitializeDownloadRequests()
	{
		_lastInitializedDownloads = Time.timeSinceLevelLoad;
		_pendingDownloads = new Dictionary<string, int>();
		List<KeyValuePair<Characters.CharacterType, Characters.Model>> characterList = CharacterScreenManager.Instance.GetCharacterList();
		foreach (KeyValuePair<Characters.CharacterType, Characters.Model> item in characterList)
		{
			string[] referencedAssetBundleNames = SingletonMonobehavior<Factories>.Instance.GetReferencedAssetBundleNames(item.Key);
			string[] array = referencedAssetBundleNames;
			foreach (string bundleName in array)
			{
				int latestVersionForBundle = _versionControl.GetLatestVersionForBundle(bundleName);
				if (!AssetBundleCache.IsAssetBundleCached(bundleName, latestVersionForBundle) && !_versionControl.IsBundleVersionShipped(bundleName, latestVersionForBundle))
				{
					AddAssetBundleDownloadRequest(bundleName, latestVersionForBundle);
				}
			}
		}
		_downloadableBundlesUpdated = false;
		PlayerInfo.Instance.AllAssetBundlesDownloaded = !_pendingDownloads.Any();
		attemptingToDownloadAmount = ((_pendingDownloads.Count > 0) ? _pendingDownloads.Count : 0);
		succesfullyDownloaded = 0;
	}

	private void OnDownloadableAssetBundlesUpdated()
	{
		_downloadableBundlesUpdated = true;
	}

	private void AddAssetBundleDownloadRequest(string bundleName, int version)
	{
		if (_pendingDownloads.ContainsKey(bundleName))
		{
			if (_pendingDownloads[bundleName] > version)
			{
				_pendingDownloads[bundleName] = version;
			}
		}
		else
		{
			_pendingDownloads.Add(bundleName, version);
		}
	}

	public void StartDownloadIfPossible()
	{
		if (_pendingDownloads == null || !_pendingDownloads.Any() || _downloadableBundlesUpdated)
		{
			if (AllowedToInitializeDownloads())
			{
				InitializeDownloadRequests();
				if (_pendingDownloads.Any())
				{
					StartDownloadIfPossible();
				}
			}
		}
		else if (!IsDownloading && !Game.Instance.IsInGame.Value)
		{
			KeyValuePair<string, int> keyValuePair = _pendingDownloads.First();
			_currentDownloaderCoroutine = DownloadAssetBundle(keyValuePair.Key, keyValuePair.Value);
			CoroutineUtil.StartCoroutine(_currentDownloaderCoroutine);
		}
	}

	private bool AllowedToInitializeDownloads()
	{
		return _lastInitializedDownloads + OnlineSettings.instance.GetValue("asset_bundle_download_rest_time", _downloadAttemptRestTime) < Time.timeSinceLevelLoad;
	}

	private void StopDownloading()
	{
		if (IsDownloading)
		{
			CoroutineUtil.StopCoroutine(_currentDownloaderCoroutine);
			IsDownloading = false;
		}
	}

	private IEnumerator DownloadAssetBundle(string bundleName, int version)
	{
		OnlineSettings onlineSettings = OnlineSettings.instance;
		IsDownloading = true;
		bool downloadSuccessful = false;
		string serverUrl = GetAssetBundlesServerURL(bundleName);
		float timeout = onlineSettings.GetValue("asset_bundle_request_timeout", _requestDownloadTimeout);
		IRequest request = RequestExtensions.WithMaxBackOffTime(maxBackOffTime: onlineSettings.GetValue("asset_bundle_request_max_backoff_time", _requestMaxBackoffTime), request: WebClient.CreateGet(new Uri(serverUrl)).WithTimeout(timeout));
		yield return request.SendAsync();
		if (request.isFinished && request.isSuccessful)
		{
			byte[] decompressedBundleBytes = request.response.bytes.Decompress("subway_bundles");
			if (CacheAssetBundle(bundleName, version, decompressedBundleBytes))
			{
				downloadSuccessful = true;
				succesfullyDownloaded++;
				ResolveDownloadRequest(bundleName, version);
			}
			else
			{
				LogError("Attempt to cache downloaded bundle from the server failed.");
			}
		}
		else
		{
			if (request.isFinished)
			{
				string country = Utils.GetContryCodeFromLocale(DeviceUtility.GetLocale());
				Flurry.LogEventWithSeveralParameters("Asset Bundle Failed to Download", "Asset Bundle Name;Asset Bundle Version;Asset Bundle Download Error;Country", string.Concat(bundleName, ";", version, ";", request.exception, ";", country));
			}
			LogError("Attempt to download from the server failed. Error message: " + request.exception);
		}
		if (!downloadSuccessful)
		{
			DownloadFailedFor(bundleName, version);
		}
		IsDownloading = false;
	}

	private void DownloadFailedFor(string bundleName, int version)
	{
		if (_pendingDownloads.ContainsKey(bundleName))
		{
			_pendingDownloads.Remove(bundleName);
			return;
		}
		LogError("No pending download found for failed download: " + bundleName + " version " + version);
	}

	private void ResolveDownloadRequest(string bundleName, int version)
	{
		if (_pendingDownloads.ContainsKey(bundleName) && _pendingDownloads[bundleName] <= version)
		{
			_pendingDownloads.Remove(bundleName);
		}
	}

	public static string SerializeAssetBundleVersionData(Dictionary<string, int> versionData)
	{
		string text = string.Empty;
		if (versionData != null)
		{
			foreach (KeyValuePair<string, int> versionDatum in versionData)
			{
				string text2 = text;
				text = text2 + versionDatum.Key + '-' + versionDatum.Value + ',';
			}
		}
		return text;
	}

	public static Dictionary<string, int> ParseAssetBundleVersionData(string serializedData)
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		string[] array = serializedData.Split(',');
		string[] array2 = array;
		foreach (string text in array2)
		{
			string[] array3 = text.Split('-');
			if (array3.Length == 2)
			{
				dictionary.Add(array3[0], int.Parse(array3[1]));
			}
		}
		return dictionary;
	}

	[Conditional("ASSET_BUNDLES_DEBUG_LOGS")]
	private void Log(string msg)
	{
		Debug.Log("[AssetBundleManager]: " + msg);
	}

	[Conditional("ASSET_BUNDLES_DEBUG_LOGS")]
	private void LogWarning(string msg)
	{
		Debug.LogWarning("[AssetBundleManager]: " + msg);
	}

	private void LogError(string msg)
	{
		Debug.LogError("[AssetBundleManager]: " + msg);
	}

	[Conditional("ASSET_BUNDLES_REQUESTS_DEBUG_LOGS")]
	private void DebugRequestStatus(string bundleName)
	{
		string text = "REQUESTED: " + bundleName + "\n";
		text += "LOADED:\n";
		foreach (string key in _loadedAssetBundles.Keys)
		{
			text = text + "     " + key + "\n";
		}
		text += "LOADING:\n";
		foreach (string item in _assetBundlesLoading)
		{
			text = text + "     " + item + "\n";
		}
		text += "UNLOADING:\n";
		foreach (string key2 in _pendingUnloads.Keys)
		{
			text = text + "     " + key2 + "\n";
		}
	}
}
