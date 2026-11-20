using SYBO.Subway.Info;
using UnityEngine;

public class TurboHeadstartIntroductionPopup : CharacterPopup
{
	private const int NUMBER_OF_FREE_HEADSTARTS = 3;

	[SerializeField]
	private UILabel topTitle;

	[SerializeField]
	private UILabel line1Big;

	[SerializeField]
	private UILabel line1Small;

	[SerializeField]
	private UILabel line2Big;

	[SerializeField]
	private UILabel line2Small;

	[SerializeField]
	private UILabel line3Big;

	[SerializeField]
	private UILabel line3Small;

	[SerializeField]
	private UILabel buttonText;

	public override void Show()
	{
		base.Show();
		SetCharacter(Characters.CharacterType.slick, null, null, 19f);
		topTitle.text = Strings.Get(StringID.TURBO_HEADSTART_INTRODUCTION_POPUP_TITLE);
		line1Big.text = Strings.Get(StringID.TURBO_HEADSTART_INTRODUCTION_POPUP_LINE1BIG);
		line1Small.text = Strings.Get(StringID.TURBO_HEADSTART_INTRODUCTION_POPUP_LINE1SMALL);
		line2Big.text = Strings.Get(StringID.TURBO_HEADSTART_INTRODUCTION_POPUP_LINE2BIG);
		line2Small.text = Strings.Get(StringID.TURBO_HEADSTART_INTRODUCTION_POPUP_LINE2SMALL);
		line3Big.text = Strings.Get(StringID.TURBO_HEADSTART_INTRODUCTION_POPUP_LINE3BIG);
		line3Small.text = Strings.Get(StringID.TURBO_HEADSTART_INTRODUCTION_POPUP_LINE3SMALL);
		buttonText.text = Strings.Get(StringID.TURBO_HEADSTART_INTRODUCTION_POPUP_BUTTON_TEXT);
	}

	private void OkClicked()
	{
		Close();
		Flurry.LogOkPressedOnPopup("POPUP Screen TurboHeadstartIntroductionPopup");
	}

	private void CloseClicked()
	{
		Close();
		Flurry.LogClosePressedOnPopup("POPUP Screen TurboHeadstartIntroductionPopup");
	}

	private void Close()
	{
		PlayerInfo.Instance.IncreaseUpgradeAmount(PowerupType.headstart2000, 3);
		PlayerInfo.Instance.hasShownTurboHeadstartIntroductionPopup = true;
		UIScreenController.Instance.ignoreLastPopupClosed = true;
		UIScreenController.Instance.CloseActivePopups(screenName);
		UIScreenController.Instance.PushScreen("UpgradesUI_shop");
	}
}
