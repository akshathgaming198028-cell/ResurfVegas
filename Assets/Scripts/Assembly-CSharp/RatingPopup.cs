using Kiloo.Plugins.DeviceUtility;
using Kiloo.Plugins.Utility;
using SYBO.Subway.Info;
using UnityEngine;

public class RatingPopup : CharacterPopup
{
	[SerializeField]
	private UILabel bigLabel;

	[SerializeField]
	private UILabel pleaseLabel;

	[SerializeField]
	private UILabel rateButtonLabel;

	[SerializeField]
	private UILabel cancelButtonLabel;

	[SerializeField]
	private UILabel notAgainLabel;

	[SerializeField]
	private UIToggle notAgainCheckBox;

	public override void Show()
	{
		base.Show();
		SetCharacter(Characters.CharacterType.tricky, null, null, 19f);
		bigLabel.text = Strings.Get(StringID.POPUP_RATING_BIG_LABEL);
		pleaseLabel.text = Strings.Get(StringID.POPUP_RATING_PLEASE_LABEL);
		rateButtonLabel.text = Strings.Get(StringID.POPUP_RATING_RATE_BUTTON_LABEL);
		cancelButtonLabel.text = Strings.Get(StringID.POPUP_RATING_CANCEL_BUTTON_LABEL);
		notAgainLabel.text = Strings.Get(StringID.POPUP_RATING_NOTAGAIN_LABEL);
	}

	private void OkClicked()
	{
		if (UIScreenController.Instance.GetCurrentPopupName() == "RatingPopup")
		{
			UIScreenController.Instance.CloseActivePopups(screenName);
			PlayerInfo.Instance.neverAskForRating = true;
			PlayerInfo.Instance.lastVersionRated = DeviceUtility.GetBundleVersion();
			Flurry.LogEventWithSeveralParameters("Rate the app popup", "Result;Should ask again", "Ok;Do not ask again");
		}
		if (Application.platform == RuntimePlatform.WP8Player)
		{
			UtilWP8.RateTheApp();
		}
		else
		{
			Application.OpenURL(Globals.RatingURL);
		}
	}

	private void CancelClicked()
	{
		if (UIScreenController.Instance.GetCurrentPopupName() == "RatingPopup")
		{
			UIScreenController.Instance.CloseActivePopups(screenName);
			PlayerInfo.Instance.neverAskForRating = notAgainCheckBox.value;
			PlayerInfo.Instance.lastVersionRated = DeviceUtility.GetBundleVersion();
			if (notAgainCheckBox.value)
			{
				Flurry.LogEventWithSeveralParameters("Rate the app popup", "Result;Should ask again", "Cancel;Do not ask again");
			}
			else
			{
				Flurry.LogEventWithSeveralParameters("Rate the app popup", "Result;Should ask again", "Cancel;Ask again");
			}
		}
	}
}
