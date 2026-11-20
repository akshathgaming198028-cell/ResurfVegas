using SYBO.Subway.Info;
using UnityEngine;

public class WelcomePopup : CharacterPopup
{
	[SerializeField]
	private UILabel titleLine;

	[SerializeField]
	private UILabel line1Big;

	[SerializeField]
	private UILabel line1Small;

	[SerializeField]
	private UILabel line1Dot;

	[SerializeField]
	private UILabel line2Big;

	[SerializeField]
	private UILabel line2Small;

	[SerializeField]
	private UILabel line2Dot;

	[SerializeField]
	private UILabel line3Big;

	[SerializeField]
	private UILabel line3Small;

	[SerializeField]
	private UILabel line3Dot;

	[SerializeField]
	private UILabel buttonText;

	[SerializeField]
	private UISprite calloutBackgroundSprite;

	[SerializeField]
	private UISprite calloutBackgroundTriangleSprite;

	[SerializeField]
	private UILabel calloutText;

	public override void Show()
	{
		base.Show();
		SetCharacter(Characters.CharacterType.lasVegasCharacter01, new Vector2(43f, 5f), null, 19f);
		titleLine.text = Strings.Get(StringID.POPUP_WELCOME_TITLE_LINE_1);
		buttonText.text = Strings.Get(StringID.OK);
		line1Big.text = Strings.Get(StringID.POPUP_WELCOME_LINE1BIG);
		line1Small.text = Strings.Get(StringID.POPUP_WELCOME_LINE1SMALL);
		line2Big.text = Strings.Get(StringID.POPUP_WELCOME_LINE2BIG);
		line2Small.text = Strings.Get(StringID.POPUP_WELCOME_LINE2SMALL);
		line3Big.text = Strings.Get(StringID.POPUP_WELCOME_LINE3BIG);
		line3Small.text = Strings.Get(StringID.POPUP_WELCOME_LINE3SMALL);
		calloutBackgroundSprite.color = GlobalColors.WELCOMEPOPUP_BUBBLE_COLOR;
		calloutBackgroundTriangleSprite.color = GlobalColors.WELCOMEPOPUP_BUBBLE_COLOR;
		calloutText.color = GlobalColors.WELCOMEPOPUP_BUBBLE_TEXT_COLOR;
	}

	public void OkClicked()
	{
		Flurry.LogOkPressedOnPopup(UIScreens.GetFlurryPopUpWithOkAndCloseName(base.gameObject.name));
		UIScreenController.Instance.CloseActivePopups(screenName);
	}

	private void CloseClicked()
	{
		Flurry.LogClosePressedOnPopup(UIScreens.GetFlurryPopUpWithOkAndCloseName(base.gameObject.name));
		UIScreenController.Instance.CloseActivePopups(screenName);
	}
}
