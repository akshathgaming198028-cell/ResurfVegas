using System;
using System.Collections.Generic;
using SYBO.Core.Components.Singleton;
using SYBO.Core.Managers;
using SYBO.Subway.Info;
using UnityEngine;
using com.kiloo.awards;

public class DebugSettingsHelper : MonoBehaviour
{
	public UILabel Title;

	public UILabel Description;

	public GameObject EventTesterIcon;

	public GameObject CelebrationTesterIcon;

	public GameObject OffIcon;

	public GameObject OnIcon;

	public SelectPopupButton PopupList;

	public UILabel PopupListTitle;

	private bool currentIconState;

	private DebugSetting _settingType;

	public bool multiSelect;

	public void UpdateTitle(string title)
	{
		Title.text = title;
	}

	public void UpdateDescription(string description)
	{
		Description.text = description;
	}

	public void SetToggleIcon(DebugSetting settingType)
	{
		bool flag = false;
		switch (settingType)
		{
		case DebugSetting.Characters:
			flag = Globals.DEBUG_ALL_CHARS;
			break;
		case DebugSetting.Boards:
			flag = Globals.DEBUG_ALL_BOARDS;
			break;
		case DebugSetting.Purchases:
			flag = Globals.DEBUG_FREE_INAPP_PURCHASE;
			break;
		case DebugSetting.Letters:
			flag = Globals.DEBUG_SKIP_DAILY_LETTERS;
			break;
		case DebugSetting.Powerups:
			flag = Globals.DEBUG_PRINT_SPAWNED_POWER_UPS;
			break;
		case DebugSetting.FrontScreen:
			flag = Globals.DEBUG_SHOW_FRONT_SCREEN_IF_NEW_USER;
			break;
		case DebugSetting.UseOSSet:
			flag = Globals.DEBUG_USE_ONLINESETTINGS_SET;
			break;
		case DebugSetting.OSTesttime:
			flag = Globals.DEBUG_USE_ONLINESETTINGS_TESTTIME;
			break;
		case DebugSetting.Interstitials:
			flag = Globals.DEBUG_INTERSTITIAL_PROVIDERS;
			break;
		case DebugSetting.VideoAds:
			flag = Globals.DEBUG_VIDEOADS_PROVIDERS;
			break;
		case DebugSetting.InterstitialTimelimit:
			flag = Globals.DEBUG_INTERSTITIAL_PROVIDERS_IGNORE_TIME_LIMIT;
			break;
		case DebugSetting.InterstitialInApp:
			flag = Globals.DEBUG_INTERSTITIAL_PROVIDERS_IGNORE_IN_APP_PURCHASES;
			break;
		case DebugSetting.FPS:
			flag = DebugSettingsPopup.IsShowingFPS;
			break;
		case DebugSetting.RecordFPS:
			flag = FPSLog.instance.IsLoggingFPS;
			break;
		case DebugSetting.UploadFPS:
			flag = FPSLog.instance.IsUploadingFPS;
			break;
		case DebugSetting.CharacterNoHitCollission:
			flag = Globals.DEBUG_CHARACTER_MOVE_THROUGH_COLLIDERS;
			break;
		case DebugSetting.CharacterShaderDebug:
			flag = Globals.DEBUG_SHADERS;
			break;
		case DebugSetting.TimescaleButton:
			flag = Globals.DEBUG_USE_TIMESCALE_PAUSE_BUTTON;
			break;
		case DebugSetting.SkipVideoAds:
			flag = Globals.DEBUG_SKIP_VIDEO_ADS;
			break;
		case DebugSetting.LimitedCharacterPopup:
			flag = Globals.DEBUG_LIMITED_CHARACTER_POPUP;
			break;
		case DebugSetting.LimitedCharacterPopupIgnoreExpire:
			flag = Globals.DEBUG_LIMITED_CHARACTER_POPUP_IGNORE_EXPIRE;
			break;
		case DebugSetting.LimitedBoardPopup:
			flag = Globals.DEBUG_LIMITED_BOARD_POPUP;
			break;
		case DebugSetting.LimitedBoardPopupIgnoreExpire:
			flag = Globals.DEBUG_LIMITED_BOARD_POPUP_IGNORE_EXPIRE;
			break;
		case DebugSetting.PerformanceInfo:
			flag = Globals.DEBUG_SHOW_PERFORMANCE_INFO;
			break;
		default:
			Debug.LogWarning("DebugSettingsHelper: Attempting to use incorrect DebugSetting type!");
			break;
		}
		OffIcon.SetActive(!flag);
		OnIcon.SetActive(flag);
		currentIconState = flag;
	}

	public void ToggleIcon()
	{
		currentIconState = !currentIconState;
		OffIcon.SetActive(!currentIconState);
		OnIcon.SetActive(currentIconState);
	}

	private List<string> GetCurrentlySelected()
	{
		List<string> list = new List<string>();
		if (multiSelect)
		{
			if (_settingType == DebugSetting.VideoAdsProvider)
			{
				if (Globals.DEBUG_VIDEOADS_PROVIDERS_FLURRY)
				{
					list.Add("Flurry");
				}
				if (Globals.DEBUG_VIDEOADS_PROVIDERS_INMOBI)
				{
					list.Add("InMobi");
				}
				if (Globals.DEBUG_VIDEOADS_PROVIDERS_TABJOY)
				{
					list.Add("TabJoy");
				}
				if (Globals.DEBUG_VIDEOADS_PROVIDERS_VUNGLE)
				{
					list.Add("Vungle");
				}
				if (Globals.DEBUG_VIDEOADS_PROVIDERS_ADCOLONY)
				{
					list.Add("AdColony");
				}
				if (Globals.DEBUG_VIDEOADS_PROVIDERS_CHARTBOOST)
				{
					list.Add("Chartboost");
				}
			}
			else if (_settingType == DebugSetting.InterstitialProvider)
			{
				if (Globals.DEBUG_INTERSTITIAL_PROVIDER_CHARTBOOST)
				{
					list.Add("Chartboost");
				}
				if (Globals.DEBUG_INTERSTITIAL_PROVIDER_MDOTM)
				{
					list.Add("MdotM");
				}
				if (Globals.DEBUG_INTERSTITIAL_PROVIDER_INMOBI)
				{
					list.Add("InMobi");
				}
			}
			else if (_settingType == DebugSetting.AwardTestAward)
			{
				if (DebugSettingsPopup.completeAwardsTiersSelectedItems.Keys != null)
				{
					list.AddRange(DebugSettingsPopup.completeAwardsTiersSelectedItems.Keys);
				}
			}
			else if (_settingType == DebugSetting.AwardTestTier)
			{
				list.Add(DebugSettingsPopup.completeAwardsTiersSelectedTiers.ToString());
			}
			else if (_settingType == DebugSetting.TimeScaleInGameOverride)
			{
				list.Add(DebugSettingsPopup.TimeScaleInGameOverrideSelectedValue.ToString());
			}
			else if (_settingType == DebugSetting.TimeScaleInMenuOverride)
			{
				list.Add(DebugSettingsPopup.TimeScaleInMenuOverrideSelectedValue.ToString());
			}
		}
		else
		{
			list.Add(PopupListTitle.text);
		}
		return list;
	}

	public void SetTesterIcon(ButtonIcon icon)
	{
		EventTesterIcon.SetActive(icon == ButtonIcon.EventTester);
		CelebrationTesterIcon.SetActive(icon == ButtonIcon.CelebrationTester);
		if (icon == ButtonIcon.None)
		{
			Title.transform.localPosition = new Vector3(-111f, -6f, 0f);
			Description.transform.localPosition = new Vector3(-111f, -24f, 0f);
		}
		else
		{
			Title.transform.localPosition = new Vector3(-87f, -6f, 0f);
			Description.transform.localPosition = new Vector3(-87f, -24f, 0f);
		}
	}

	public void SetExpandableList(DebugSetting settingType)
	{
		_settingType = settingType;
		PopupList.multiSelect = false;
		switch (settingType)
		{
		case DebugSetting.OSSet:
			PopupList.SetItems(new List<string> { "qa", "testology" });
			PopupListTitle.text = Globals.ONLINESETTINGS_SET;
			break;
		case DebugSetting.InterstitialProvider:
			PopupList.multiSelect = true;
			PopupList.SetItems(new List<string> { "InMobi", "Chartboost", "MdotM" });
			PopupListTitle.text = "None";
			break;
		case DebugSetting.VideoAdsProvider:
			PopupList.multiSelect = true;
			PopupList.SetItems(new List<string> { "AdColony", "Flurry", "InMobi", "TabJoy", "Vungle", "Chartboost" });
			PopupListTitle.text = "None";
			break;
		case DebugSetting.FPSTargetFramerate:
			PopupList.SetItems(new List<string> { "Default", "10", "20", "30", "40", "50", "60" });
			PopupListTitle.text = "Default";
			break;
		case DebugSetting.AwardTestAward:
		{
			PopupList.multiSelect = true;
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			bool containsCollectibleAwards;
			foreach (Award sortedListAward in AwardsManager.Instance.GetSortedListAwards(out containsCollectibleAwards))
			{
				dictionary.Add(sortedListAward.trophyData.id.ToString(), Strings.Get(sortedListAward.trophyData.title));
			}
			PopupList.SetItems(dictionary);
			PopupListTitle.text = AwardsManager.Instance.GetSortedListAwards(out containsCollectibleAwards)[0].trophyData.id.ToString();
			break;
		}
		case DebugSetting.AwardTestTier:
			PopupList.SetItems(new List<string> { "1", "2", "3" });
			PopupListTitle.text = ((DebugSettingsPopup.completeAwardsTiersSelectedTiers != 0) ? DebugSettingsPopup.completeAwardsTiersSelectedTiers.ToString() : "1");
			break;
		case DebugSetting.TimeScaleInMenuOverride:
			PopupList.SetItems(new List<string> { "0.1", "0.25", "0.5", "1", "2", "5" }, 3);
			PopupListTitle.text = SingletonMonobehavior<TimeScaleManager>.Instance.TimeScaleInMenuDefault.ToString();
			break;
		case DebugSetting.TimeScaleInGameOverride:
			PopupList.SetItems(new List<string> { "0.1", "0.25", "0.5", "1", "2", "5" }, 3);
			PopupListTitle.text = SingletonMonobehavior<TimeScaleManager>.Instance.TimeScaleInGameDefault.ToString();
			break;
		case DebugSetting.LimitedCharacterPopupType:
			PopupList.SetItems(new List<string>(Enum.GetNames(typeof(Characters.CharacterType))));
			PopupListTitle.text = Globals.DEBUG_LIMITED_CHARACTER_POPUP_TYPE.ToString();
			break;
		case DebugSetting.LimitedBoardPopupType:
			PopupList.SetItems(new List<string>(Enum.GetNames(typeof(Hoverboards.BoardType))));
			PopupListTitle.text = Globals.DEBUG_LIMITED_BOARD_POPUP_TYPE.ToString();
			break;
		default:
			Debug.LogWarning(string.Format("DebugSettingsHelper: Attempting to set expandable list of type {0}, but behaviour is not defined", settingType.ToString()));
			break;
		}
		if (multiSelect)
		{
			PopupListTitle.text = ((GetCurrentlySelected().Count != 1) ? string.Format("{0} selected", GetCurrentlySelected().Count) : GetCurrentlySelected()[0]);
		}
	}

	public void ExpandableListItemSelected()
	{
		switch (_settingType)
		{
		case DebugSetting.OSSet:
			Globals.ONLINESETTINGS_SET = PopupList.value;
			DebugSettingsPopup.DownloadNewOS = true;
			break;
		case DebugSetting.InterstitialProvider:
			Globals.DEBUG_INTERSTITIAL_PROVIDER_INMOBI = false;
			Globals.DEBUG_INTERSTITIAL_PROVIDER_CHARTBOOST = false;
			Globals.DEBUG_INTERSTITIAL_PROVIDER_MDOTM = false;
			foreach (string value in PopupList.multipleValues.Values)
			{
				switch (value)
				{
				case "InMobi":
					Globals.DEBUG_INTERSTITIAL_PROVIDER_INMOBI = true;
					break;
				case "Chartboost":
					Globals.DEBUG_INTERSTITIAL_PROVIDER_CHARTBOOST = true;
					break;
				case "MdotM":
					Globals.DEBUG_INTERSTITIAL_PROVIDER_MDOTM = true;
					break;
				default:
					Debug.LogWarning("DebugSettingsHelper: Attempting to use incorrect provider!");
					break;
				}
			}
			DebugSettingsPopup.RefreshInterstitials = true;
			break;
		case DebugSetting.VideoAdsProvider:
			Globals.DEBUG_VIDEOADS_PROVIDERS_ADCOLONY = false;
			Globals.DEBUG_VIDEOADS_PROVIDERS_FLURRY = false;
			Globals.DEBUG_VIDEOADS_PROVIDERS_VUNGLE = false;
			Globals.DEBUG_VIDEOADS_PROVIDERS_INMOBI = false;
			Globals.DEBUG_VIDEOADS_PROVIDERS_TABJOY = false;
			Globals.DEBUG_VIDEOADS_PROVIDERS_CHARTBOOST = false;
			foreach (string value2 in PopupList.multipleValues.Values)
			{
				switch (value2)
				{
				case "AdColony":
					Globals.DEBUG_VIDEOADS_PROVIDERS_ADCOLONY = true;
					break;
				case "Flurry":
					Globals.DEBUG_VIDEOADS_PROVIDERS_FLURRY = true;
					break;
				case "InMobi":
					Globals.DEBUG_VIDEOADS_PROVIDERS_INMOBI = true;
					break;
				case "TabJoy":
					Globals.DEBUG_VIDEOADS_PROVIDERS_TABJOY = true;
					break;
				case "Vungle":
					Globals.DEBUG_VIDEOADS_PROVIDERS_VUNGLE = true;
					break;
				case "Chartboost":
					Globals.DEBUG_VIDEOADS_PROVIDERS_CHARTBOOST = true;
					break;
				default:
					Debug.LogWarning("DebugSettingsHelper: Attempting to use incorrect provider!");
					break;
				}
			}
			DebugSettingsPopup.RefreshVideoAds = true;
			break;
		case DebugSetting.FPSTargetFramerate:
			if (PopupList.value == "Default")
			{
				FPSLog.instance.SetFPSFramerate();
			}
			else
			{
				FPSLog.instance.SetFPSFramerate(int.Parse(PopupList.value));
			}
			break;
		case DebugSetting.LimitedCharacterPopupType:
		{
			string[] names2 = Enum.GetNames(typeof(Characters.CharacterType));
			int num2 = Array.IndexOf(names2, PopupList.value);
			if (num2 != -1)
			{
				Globals.DEBUG_LIMITED_CHARACTER_POPUP_TYPE = (Characters.CharacterType)num2;
			}
			break;
		}
		case DebugSetting.LimitedBoardPopupType:
		{
			string[] names = Enum.GetNames(typeof(Hoverboards.BoardType));
			int num = Array.IndexOf(names, PopupList.value);
			if (num != -1)
			{
				Globals.DEBUG_LIMITED_BOARD_POPUP_TYPE = (Hoverboards.BoardType)num;
			}
			break;
		}
		case DebugSetting.AwardTestAward:
			DebugSettingsPopup.completeAwardsTiersSelectedItems = PopupList.multipleValues;
			break;
		case DebugSetting.AwardTestTier:
			DebugSettingsPopup.completeAwardsTiersSelectedTiers = int.Parse(PopupList.value);
			break;
		case DebugSetting.TimeScaleInMenuOverride:
			DebugSettingsPopup.TimeScaleInMenuOverrideSelectedValue = float.Parse(PopupList.value);
			if (DebugSettingsPopup.TimeScaleInMenuOverrideSelectedValue != SingletonMonobehavior<TimeScaleManager>.Instance.TimeScaleInMenuDefault)
			{
				SingletonMonobehavior<TimeScaleManager>.Instance.TimeScaleInMenuDefault = DebugSettingsPopup.TimeScaleInMenuOverrideSelectedValue;
			}
			break;
		case DebugSetting.TimeScaleInGameOverride:
			DebugSettingsPopup.TimeScaleInGameOverrideSelectedValue = float.Parse(PopupList.value);
			if (DebugSettingsPopup.TimeScaleInGameOverrideSelectedValue != SingletonMonobehavior<TimeScaleManager>.Instance.TimeScaleInGameDefault)
			{
				SingletonMonobehavior<TimeScaleManager>.Instance.TimeScaleInGameDefault = DebugSettingsPopup.TimeScaleInGameOverrideSelectedValue;
			}
			break;
		default:
			Debug.LogWarning(string.Format("DebugSettingsHelper: Attempting to use expandable list of type [{0}], but behaviour is not defined", _settingType.ToString()));
			break;
		}
	}
}
