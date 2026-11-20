using System;
using Kiloo.Plugins.DeviceUtility;
using SYBO.Subway.Info;
using UnityEngine;

public class UpdateApp : CharacterPopup
{
	private const string ONLINESETTINGS_LATESTVERSION_KEY = "latestversion";

	private const string ONLINESETTINGS_LATESTVERSION_CHANGELIST_KEY = "latestversion_changelist";

	private static bool isUpdateNowOnScreen;

	private static bool _hasShownThisSession;

	[SerializeField]
	private UILabel updateLabel;

	[SerializeField]
	private UILabel theLabel;

	[SerializeField]
	private UILabel appLabel;

	[SerializeField]
	private UILabel changeListLabel;

	private void Awake()
	{
		if (updateLabel != null && theLabel != null && appLabel != null)
		{
			updateLabel.text = Strings.Get(StringID.UPDATE_APP_POPUP_UPDATE);
			theLabel.text = Strings.Get(StringID.UPDATE_APP_POPUP_THE);
			appLabel.text = Strings.Get(StringID.UPDATE_APP_POPUP_APP);
		}
	}

	public override void Show()
	{
		base.Show();
		SetCharacter(Characters.CharacterType.slick, null, null, 19f);
		string valueString;
		if (OnlineSettings.instance.TryGetValue("latestversion_changelist", out valueString))
		{
			changeListLabel.text = valueString + "\n";
			return;
		}
		Debug.LogError("Showing NewVersion popup, but no changeList found in OnlineSettings", this);
		changeListLabel.text = "New Content\n";
	}

	private void CloseClicked()
	{
		UIScreenController.Instance.CloseActivePopups(screenName);
		isUpdateNowOnScreen = false;
		Flurry.LogClosePressedOnPopup(UIScreens.GetFlurryPopUpWithOkAndCloseName(base.gameObject.name));
	}

	private void OkClicked()
	{
		Application.OpenURL(Globals.AppStoreURL);
		UIScreenController.Instance.CloseActivePopups(screenName);
		isUpdateNowOnScreen = false;
		Flurry.LogOkPressedOnPopup(UIScreens.GetFlurryPopUpWithOkAndCloseName(base.gameObject.name));
	}

	public static void AllowAgainThisSession()
	{
		_hasShownThisSession = false;
	}

	public static void ShowIfNeeded()
	{
		string valueString;
		if (isUpdateNowOnScreen || !(UIScreenController.Instance.GetCurrentPopupName() != "EventsPopup") || string.IsNullOrEmpty(Globals.AppStoreURL) || !OnlineSettings.instance.TryGetValue("latestversion", out valueString))
		{
			return;
		}
		string bundleVersion = DeviceUtility.GetBundleVersion();
		bool flag = false;
		try
		{
			if (Utility.CompareVersions(bundleVersion, valueString) < 0)
			{
				flag = true;
			}
		}
		catch (FormatException ex)
		{
			Debug.LogError("Failed to parse versions for comparison: " + bundleVersion + " and " + valueString + "  : " + ex);
		}
		if (flag && !_hasShownThisSession)
		{
			UIScreenController.Instance.QueuePopup("UpdateAppPopup");
			isUpdateNowOnScreen = true;
			_hasShownThisSession = true;
		}
	}
}
