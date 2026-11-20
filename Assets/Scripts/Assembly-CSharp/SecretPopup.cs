using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Kiloo.Common;
using Kiloo.Common.UI;
using Kiloo.Plugins.DeviceUtility;
using UnityEngine;

public class SecretPopup : UIScreen
{
	public class SecretListAdapter : ListAdapter<SecretCell>
	{
		private const int SECRET_INFO_PREFAB = 0;

		private const int SECRET_SUBTITLE_PREFAB = 1;

		private const int SECRET_ENDLIST_PREFAB = 2;

		public SecretPopup secretScreen;

		public GameObject secretPrefab;

		public GameObject secretSubtitlePrefab;

		public GameObject secretEndList;

		public SecretListAdapter(List<SecretCell> contentList, GameObject newSecretPrefab, GameObject newSecretSubtitlePrefab, GameObject newSecretEndList, SecretPopup screenReference)
			: base(contentList)
		{
			secretPrefab = newSecretPrefab;
			secretSubtitlePrefab = newSecretSubtitlePrefab;
			secretEndList = newSecretEndList;
			secretScreen = screenReference;
		}

		public void setNewDataList(List<SecretCell> leaderboardListT)
		{
			base.dataList = leaderboardListT;
			NotifyDataSetChanged();
		}

		public override int GetModelType(int index)
		{
			switch (GetItem(index).cellType)
			{
			case SecretCellType.Info:
				return 0;
			case SecretCellType.Subtitle:
				return 1;
			case SecretCellType.EndList:
				return 2;
			default:
				return 0;
			}
		}

		public override Container GetContainer(Container container, int dataIndex, bool isMeasurePass)
		{
			Container container2 = container;
			SecretCell item = GetItem(dataIndex);
			if (container2 == null)
			{
				SecretCellReferences secretCellReferences = new SecretCellReferences();
				switch (item.cellType)
				{
				case SecretCellType.Info:
				{
					container2 = new Container(secretPrefab);
					SecretHelper component3 = container2.model.GetComponent<SecretHelper>();
					secretCellReferences.secretHelper = component3;
					container2.tag = secretCellReferences;
					break;
				}
				case SecretCellType.Subtitle:
				{
					container2 = new Container(secretSubtitlePrefab);
					SecretHelper component2 = container2.model.GetComponent<SecretHelper>();
					secretCellReferences.secretHelper = component2;
					container2.tag = secretCellReferences;
					break;
				}
				case SecretCellType.EndList:
				{
					container2 = new Container(secretEndList);
					SecretHelper component = container2.model.GetComponent<SecretHelper>();
					secretCellReferences.secretHelper = component;
					container2.tag = secretCellReferences;
					break;
				}
				}
			}
			SecretCellReferences secretCellReferences2 = (SecretCellReferences)container2.tag;
			switch (item.cellType)
			{
			case SecretCellType.Info:
				secretCellReferences2.secretHelper.UpdateInfo(item.cellInfo, item.cellValue);
				break;
			case SecretCellType.Subtitle:
				secretCellReferences2.secretHelper.UpdateTitle(item.cellTitle);
				break;
			default:
				Debug.LogError("No cell type match for: " + item.cellType);
				break;
			case SecretCellType.EndList:
				break;
			}
			return container2;
		}
	}

	[SerializeField]
	private UILabel _popupTitle;

	[SerializeField]
	private GameObject _copyButton;

	[SerializeField]
	private GameObject _copiedTextGO;

	[SerializeField]
	private UILabel _copiedTextLabel;

	[SerializeField]
	private GameObject SecretCellPrefab;

	[SerializeField]
	private GameObject SecretCellSubtitlePrefab;

	[SerializeField]
	private GameObject SecretCellEndList;

	private List<SecretCell> _contentList = new List<SecretCell>();

	private SecretListAdapter _adapter;

	[SerializeField]
	private ScrollView _scrollView;

	private StringBuilder _infoToBeCopiedToClipboard;

	private int lastIndex;

	public override void Init()
	{
		base.Init();
		if (_contentList != null)
		{
			_adapter = new SecretListAdapter(_contentList, SecretCellPrefab, SecretCellSubtitlePrefab, SecretCellEndList, this);
		}
		_popupTitle.color = GlobalColors.COUNTRY_POPUP_TITLE_COLOUR;
	}

	public override void Show()
	{
		base.Show();
		ReloadList();
		_copiedTextGO.SetActive(false);
		if (IsWPDevice())
		{
			_copyButton.SetActive(false);
		}
		else
		{
			_copyButton.SetActive(true);
		}
		ScrollToLastIndex(lastIndex);
	}

	public override void Hide()
	{
		base.Hide();
		SaveIndex();
	}

	private void OnEnable()
	{
		VideoAdsManager.instance.AddOnVideoStateChangedHandler(VideoStateOrProviderChangedHandling);
		VideoAdsManager.instance.AddOnVideoProviderChangedHandler(VideoStateOrProviderChangedHandling);
	}

	private void OnDisable()
	{
		VideoAdsManager.instance.RemoveOnVideoStateChangedHandler(VideoStateOrProviderChangedHandling);
		VideoAdsManager.instance.RemoveOnVideoProviderChangedHandler(VideoStateOrProviderChangedHandling);
	}

	private bool IsAndroidDevice()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			return true;
		}
		return false;
	}

	private bool IsiOSDevice()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return true;
		}
		return false;
	}

	private bool IsWPDevice()
	{
		if (Application.platform == RuntimePlatform.WP8Player)
		{
			return true;
		}
		return false;
	}

	private void VideoStateOrProviderChangedHandling()
	{
		SaveIndex();
		ReloadList();
		ScrollToLastIndex(lastIndex);
	}

	private void ReloadList()
	{
		SetContentList();
		if (_scrollView != null && _adapter != null)
		{
			_scrollView.SetAdapter(_adapter);
		}
	}

	private void SetContentList()
	{
		ResetList();
		InsertTitle("Plugin Information", SecretCellType.Subtitle);
		InsertInfo("Facebook ID", GetFacebookID(), SecretCellType.Info);
		if (IsiOSDevice())
		{
			InsertInfo("GID", GetGameCenterID(), SecretCellType.Info);
		}
		InsertInfo("LIP", GetLastIntertialProvider(), SecretCellType.Info);
		InsertInfo("CIP", GetCurrentIntertialProvider(), SecretCellType.Info);
		InsertInfo("LVP", GetLastVideoProvider(), SecretCellType.Info);
		InsertInfo("CVP", GetCurrentVideoProvider(), SecretCellType.Info);
		InsertInfo("OS", GetOnlineSettings(), SecretCellType.Info);
		InsertInfo("OSP", GetOnlineSettingsPlatform(), SecretCellType.Info);
		InsertInfo("OSC", GetOnlineSettingsChecksum(), SecretCellType.Info);
		InsertTitle("General Information", SecretCellType.Subtitle);
		InsertInfo("Pre-cached Bundles", AssetBundleManager.Instance.GetPreCachedBundles(), SecretCellType.Info);
		InsertInfo("Downloaded Bundles", AssetBundleManager.Instance.GetDownloadedBundles(), SecretCellType.Info);
		InsertInfo("Bundle Version", DeviceUtility.GetBundleVersion(), SecretCellType.Info);
		InsertInfo("Debug Mode", GetDebugMode(), SecretCellType.Info);
		InsertInfo("IAP Legacy", CheckForInAppLegacyPurchase(), SecretCellType.Info);
		InsertInfo("IAP Cons", CheckForInAppConsumablePurchase(), SecretCellType.Info);
		InsertInfo("IAP Noncons", CheckForInAppNonconsumablePurchase(), SecretCellType.Info);
		InsertInfo("IAP Restore", CheckForInAppRestoredPurchase(), SecretCellType.Info);
		InsertInfo("Initial Date Installed", GetInitialInstallDate(), SecretCellType.Info);
		InsertInfo("Most Recent Date Updated", PlayerInfo.Instance.lastTimeAppWasUpdatedOrInstalled.ToString(), SecretCellType.Info);
		InsertInfo("PUS version", PlayerInfo.Instance.CurrentPickupSystemVersion.ToString(), SecretCellType.Info);
		InsertTitle("Device Information", SecretCellType.Subtitle);
		InsertInfo("Device Model", SystemInfo.deviceModel, SecretCellType.Info);
		InsertInfo("Device Name", SystemInfo.deviceName, SecretCellType.Info);
		InsertInfo("Operating System", SystemInfo.operatingSystem, SecretCellType.Info);
		InsertInfo("Device Locale", DeviceUtility.GetLocale(), SecretCellType.Info);
		InsertInfo("System Language", Application.systemLanguage.ToString(), SecretCellType.Info);
		if (IsAndroidDevice())
		{
			InsertInfo("RD", CheckForRootedDevice(), SecretCellType.Info);
		}
		if (IsiOSDevice())
		{
			InsertInfo("JD", CheckForJailbrokenDevice(), SecretCellType.Info);
		}
		if (Globals.DEBUG)
		{
			InsertInfo("Performance Level", DeviceInfo.Instance.performanceLevel.ToString(), SecretCellType.Info);
		}
		InsertInfo("High Resolution", DeviceInfo.Instance.isHighres.ToString(), SecretCellType.Info);
		InsertTitle("Time", SecretCellType.Subtitle);
		InsertInfo("Device Timezone", GetTimezone(), SecretCellType.Info);
		InsertInfo("Device Time", DateTime.Now.ToString(), SecretCellType.Info);
		InsertInfo("Server Time", PlayerInfo.Instance.lastKnownServerTime.ToString(), SecretCellType.Info);
		InsertTitle("Hardware", SecretCellType.Subtitle);
		InsertInfo("Device RAM (Used/Total)", GetDeviceMemory(), SecretCellType.Info);
		InsertInfo("App RAM (Used/Allowed)", GetApplicationMemoryUsage(), SecretCellType.Info);
		if (IsWPDevice())
		{
			InsertInfo("App RAM Peak Used", GetPeakMemoryUsage(), SecretCellType.Info);
		}
		if (IsiOSDevice() || IsAndroidDevice())
		{
			InsertInfo("Int. Storage (Used/Total)", GetDiskSpace(), SecretCellType.Info);
		}
		if (IsAndroidDevice())
		{
			InsertInfo("SD Card (Used/Total)", GetSDCardSpace(), SecretCellType.Info);
		}
		InsertInfo("CPU", SystemInfo.processorType, SecretCellType.Info);
		InsertInfo("GPU", SystemInfo.graphicsDeviceName, SecretCellType.Info);
		InsertInfo("Screen DPI", GetScreenDPI(), SecretCellType.Info);
		InsertInfo("Battery Level", GetBatteryLevel(), SecretCellType.Info);
		if (Globals.DEBUG_SHOW_PERFORMANCE_INFO)
		{
			InsertTitle("Performance", SecretCellType.Subtitle);
			InsertInfo("GPU score", DeviceInfo.Instance.gpuScore.ToString(), SecretCellType.Info);
			InsertInfo("GPU category", DeviceInfo.Instance.gpuCategory.ToString(), SecretCellType.Info);
			InsertInfo("FPS", InRunAverageFPSCounter.runAverageFPS.ToString(), SecretCellType.Info);
			InsertInfo("Run Length", InRunAverageFPSCounter.runLength.ToString(), SecretCellType.Info);
		}
		_contentList.Add(new SecretCell
		{
			cellType = SecretCellType.EndList
		});
	}

	private void InsertInfo(string label, string value, SecretCellType secretCellType)
	{
		_contentList.Add(new SecretCell
		{
			cellInfo = label,
			cellValue = value,
			cellType = secretCellType
		});
	}

	private void InsertTitle(string title, SecretCellType secretCellType)
	{
		_contentList.Add(new SecretCell
		{
			cellTitle = title,
			cellType = secretCellType
		});
	}

	private void ResetList()
	{
		_contentList.Clear();
	}

	private void SaveIndex()
	{
		Container firstVisibleContainerOnScreen = _scrollView.GetFirstVisibleContainerOnScreen();
		lastIndex = firstVisibleContainerOnScreen.dataIndex;
	}

	private int GetSubtitleIndexInTheList(string subtitle)
	{
		int result = -1;
		if (_contentList != null)
		{
			for (int i = 0; i < _contentList.Count; i++)
			{
				if (_contentList[i].cellType == SecretCellType.Subtitle && _contentList[i].cellTitle == subtitle)
				{
					result = i;
				}
			}
		}
		return result;
	}

	private void ScrollToLastIndex(int index)
	{
		_scrollView.SetSelection(index, ContainerPivot.Top, 0f);
	}

	public void ScrollToPlugins()
	{
		int subtitleIndexInTheList = GetSubtitleIndexInTheList("Plugin Information");
		_scrollView.SetSelection(subtitleIndexInTheList, ContainerPivot.Top, 0f);
	}

	public void ScrollToGeneral()
	{
		int subtitleIndexInTheList = GetSubtitleIndexInTheList("General Information");
		_scrollView.SetSelection(subtitleIndexInTheList, ContainerPivot.Top, 0f);
	}

	public void ScrollToDeviceInformation()
	{
		int subtitleIndexInTheList = GetSubtitleIndexInTheList("Device Information");
		_scrollView.SetSelection(subtitleIndexInTheList, ContainerPivot.Top, 0f);
	}

	public void ScrollToTime()
	{
		int subtitleIndexInTheList = GetSubtitleIndexInTheList("Time");
		_scrollView.SetSelection(subtitleIndexInTheList, ContainerPivot.Top, 0f);
	}

	public void ScrollToHardware()
	{
		int subtitleIndexInTheList = GetSubtitleIndexInTheList("Hardware");
		_scrollView.SetSelection(subtitleIndexInTheList, ContainerPivot.Top, 0f);
	}

	private IEnumerator ShowTextCopied()
	{
		_copiedTextGO.SetActive(true);
		_copiedTextLabel.alpha = 1f;
		_copyButton.SetActive(false);
		yield return new WaitForSeconds(0.6f);
		float duration = 0.7f;
		float fromAlpha = 1f;
		float toAlpha = 0f;
		float factor = 0f;
		while (factor < 1f)
		{
			factor += Time.deltaTime / duration;
			factor = Mathf.Clamp01(factor);
			_copiedTextLabel.alpha = Mathf.Lerp(fromAlpha, toAlpha, factor);
			yield return null;
		}
		yield return new WaitForSeconds(0.05f);
		_copiedTextGO.SetActive(false);
		_copyButton.SetActive(true);
	}

	private string GetInitialInstallDate()
	{
		if (PlayerInfo.Instance.firstInstallDate == DateTime.MinValue)
		{
			return "PreUpdateInstallDate";
		}
		return PlayerInfo.Instance.firstInstallDate.ToString();
	}

	private string GetDeviceMemory()
	{
		int systemMemorySize = SystemInfo.systemMemorySize;
		int deviceFreeMemory = DeviceUtility.GetDeviceFreeMemory();
		int num = systemMemorySize - deviceFreeMemory;
		string text = systemMemorySize + " MB";
		string text2 = num + " MB";
		if (deviceFreeMemory == 0)
		{
			text2 = "Unknown";
		}
		return text2 + " / " + text;
	}

	private string GetApplicationMemoryUsage()
	{
		int currentMemoryUsage = DeviceUtility.GetCurrentMemoryUsage();
		int memoryUsageLimit = DeviceUtility.GetMemoryUsageLimit();
		string text = currentMemoryUsage + " MB";
		string text2 = memoryUsageLimit + " MB";
		if (currentMemoryUsage == 0)
		{
			text = "Unknown";
		}
		if (memoryUsageLimit == 0)
		{
			text2 = "Unknown";
		}
		return text + " / " + text2;
	}

	private string GetPeakMemoryUsage()
	{
		int peakMemoryUsage = DeviceUtility.GetPeakMemoryUsage();
		if (peakMemoryUsage == 0)
		{
			return "Unknown";
		}
		return peakMemoryUsage + " MB";
	}

	private string GetDebugMode()
	{
		if (Globals.DEBUG)
		{
			return "On";
		}
		return "Off";
	}

	private string GetScreenDPI()
	{
		if (Screen.dpi == 0f)
		{
			return "Unknown";
		}
		return Screen.dpi + " DPI";
	}

	private string GetBatteryLevel()
	{
		int batteryLevel = DeviceUtility.GetBatteryLevel();
		if (batteryLevel == -1)
		{
			return "Unknown";
		}
		return batteryLevel + "%";
	}

	private string GetDiskSpace()
	{
		float num = (float)DeviceUtility.GetFreeDiskSpace() / 1024f;
		float num2 = (float)DeviceUtility.GetTotalDiskSpace() / 1024f;
		float num3 = num2 - num;
		string text = string.Format("{0:F2}", num3);
		string text2 = string.Format("{0:F2}", num2);
		text += " GB";
		text2 += " GB";
		if (num == -1f)
		{
			text = "Unknown";
		}
		if (num2 == -1f)
		{
			text2 = "Unknown";
		}
		return text + " / " + text2;
	}

	private string GetSDCardSpace()
	{
		float num = (float)DeviceUtility.GetFreeSDCardSpace() / 1024f;
		float num2 = (float)DeviceUtility.GetTotalSDCardSpace() / 1024f;
		float num3 = num2 - num;
		string text = string.Format("{0:F2}", num3);
		string text2 = string.Format("{0:F2}", num2);
		text += " GB";
		text2 += " GB";
		if (num == -1f)
		{
			text = "Unknown";
		}
		if (num2 == -1f)
		{
			text2 = "Unknown";
		}
		return text + " / " + text2;
	}

	private string GetOnlineSettings()
	{
		if (Globals.DEBUG_USE_ONLINESETTINGS_SET)
		{
			string oNLINESETTINGS_SET = Globals.ONLINESETTINGS_SET;
			string text = oNLINESETTINGS_SET[0].ToString();
			return (oNLINESETTINGS_SET.Length <= 0) ? oNLINESETTINGS_SET : (text.ToUpper() + oNLINESETTINGS_SET.Substring(1));
		}
		return "Live";
	}

	private string GetOnlineSettingsPlatform()
	{
		string currentPlatform = OnlineSettings.instance.GetCurrentPlatform();
		string text = currentPlatform[0].ToString();
		if (currentPlatform == "ios")
		{
			return "iOS";
		}
		if (currentPlatform == "wp8")
		{
			return "WP8";
		}
		return (currentPlatform.Length <= 0) ? currentPlatform : (text.ToUpper() + currentPlatform.Substring(1));
	}

	private string GetOnlineSettingsChecksum()
	{
		string checksumToShow = OnlineSettings.instance.GetChecksumToShow();
		if (checksumToShow == null)
		{
			return "No";
		}
		return checksumToShow;
	}

	private string GetTimezone()
	{
		double totalHours = DateTime.Now.GetCurrentTimeZoneUTCOffset().TotalHours;
		if (totalHours > 0.0)
		{
			return "GMT+" + totalHours.ToString("00") + ":00";
		}
		return "GMT" + totalHours.ToString("00") + ":00";
	}

	private string CheckForInAppLegacyPurchase()
	{
		return PlayerInfo.Instance.inAppLegacyPurchaseCount.ToString();
	}

	private string CheckForInAppConsumablePurchase()
	{
		return PlayerInfo.Instance.inAppConsumablePurchaseCount.ToString();
	}

	private string CheckForInAppNonconsumablePurchase()
	{
		return PlayerInfo.Instance.inAppNonconsumablePurchaseCount.ToString();
	}

	private string CheckForInAppRestoredPurchase()
	{
		return PlayerInfo.Instance.inAppRestoredPurchaseCount.ToString();
	}

	private string GetFacebookID()
	{
		if (SocialManager.instance.facebookIsLoggedIn && SocialManager.instance.facebookProfile != null && SocialManager.instance.facebookProfile.fullName != null && SocialManager.instance.facebookProfile.id != null)
		{
			return SocialManager.instance.facebookProfile.fullName + " (id: " + SocialManager.instance.facebookProfile.id + ")";
		}
		return "Not logged in to Facebook";
	}

	private string GetGameCenterID()
	{
		if (SocialManager.instance.gamecenterIsLoggedIn)
		{
			string id = SocialManager.instance.gamecenterProfile.id;
			byte[] bytes = Encoding.ASCII.GetBytes(id);
			string text = string.Empty;
			byte[] array = bytes;
			foreach (byte b in array)
			{
				text = text + b + " ";
			}
			return string.Empty + text;
		}
		return "Not logged in.";
	}

	private string CheckForRootedDevice()
	{
		bool flag = false;
		string[] array = new string[8] { "/sbin/", "/system/bin/", "/system/xbin/", "/data/local/xbin/", "/data/local/bin/", "/system/sd/xbin/", "/system/bin/failsafe/", "/data/local/" };
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (File.Exists(text + "su"))
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			return "Yes";
		}
		return "No";
	}

	private string GetCurrentIntertialProvider()
	{
		bool hasCached;
		bool isCaching;
		string currentProvider = ScreenAdsManager.instance.GetCurrentProvider(out hasCached, out isCaching);
		if (currentProvider == null)
		{
			return "None";
		}
		string text = ((!hasCached) ? "Uncached" : "Cached");
		if (isCaching)
		{
			text = "Chaching";
		}
		return currentProvider.Substring(0, 2) + " (" + text + ")";
	}

	private string GetCurrentVideoProvider()
	{
		VideoAdsManager.VideoState currentVideoState = VideoAdsManager.instance.currentVideoState;
		VideoAdsManager.State state = VideoAdsManager.instance.state;
		VideoAdProvider currentProvider = VideoAdsManager.instance.GetCurrentProvider();
		if (currentProvider == null)
		{
			return "None";
		}
		string text = currentVideoState.ToString();
		text = text.Substring(text.IndexOf('.') + 1);
		string text2 = state.ToString().Substring(text.IndexOf('.') + 1);
		return currentProvider.id.Substring(0, 2) + " (" + text + ") (" + text2 + ")";
	}

	private string GetLastIntertialProvider()
	{
		string lastShownProvider = ScreenAdsManager.instance.GetLastShownProvider();
		if (lastShownProvider == null)
		{
			return "None";
		}
		return lastShownProvider.Substring(0, 2);
	}

	private string GetLastVideoProvider()
	{
		VideoAdProvider lastShownProvider = VideoAdsManager.instance.GetLastShownProvider();
		if (lastShownProvider == null)
		{
			return "None";
		}
		return lastShownProvider.id.Substring(0, 2);
	}

	private string CheckForJailbrokenDevice()
	{
		if (Directory.Exists("/bin/bash"))
		{
			return "Yes";
		}
		return "No";
	}

	public void CopyToClipboard()
	{
		_infoToBeCopiedToClipboard = new StringBuilder();
		foreach (SecretCell content in _contentList)
		{
			if (content.cellType == SecretCellType.Info)
			{
				_infoToBeCopiedToClipboard.Append(content.cellInfo + ": " + content.cellValue + Environment.NewLine);
			}
		}
		DeviceUtility.SetDeviceClipBoard(_infoToBeCopiedToClipboard.ToString());
		StartCoroutine(ShowTextCopied());
	}
}
