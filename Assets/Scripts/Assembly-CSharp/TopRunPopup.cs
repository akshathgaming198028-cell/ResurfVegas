using UnityEngine;

public class TopRunPopup : UIScreen
{
	public const int NEWEST_TOPRUN_POPUP_VERSION_NUMBER = 2;

	[SerializeField]
	private UILabel _title;

	[SerializeField]
	private UILabel line1Big;

	[SerializeField]
	private UILabel line1Small;

	[SerializeField]
	private UILabel buttonText;

	[SerializeField]
	private UISprite diamondBackground;

	[SerializeField]
	private UISprite goldBackground;

	[SerializeField]
	private UISprite silverBackground;

	[SerializeField]
	private UISprite bronzeBackground;

	[SerializeField]
	private UISprite _topCountryFlag;

	[SerializeField]
	private UILabel _topCountryName;

	[SerializeField]
	private UISprite _infoTextBackground;

	public override void Show()
	{
		base.Show();
		_title.text = Strings.Get(StringID.POPUP_TOPRUN_TITLE);
		line1Big.text = Strings.Get(StringID.POPUP_TOPRUN_V2_LINE1);
		line1Small.text = Strings.Get(StringID.POPUP_TOPRUN_V2_LINE2);
		buttonText.text = Strings.Get(StringID.OK);
		_infoTextBackground.color = GlobalColors.TOPRUN_POPUP_TEXT_BACKGROUND_PURPLE;
		diamondBackground.color = GlobalColors.TOPRUN_POPUP_DIAMOND_BACKGROUND;
		goldBackground.color = GlobalColors.TOPRUN_POPUP_GOLD_BACKGROUND;
		silverBackground.color = GlobalColors.TOPRUN_POPUP_SILVER_BACKGROUND;
		bronzeBackground.color = GlobalColors.TOPRUN_POPUP_BRONZE_BACKGROUND;
		_title.color = GlobalColors.TOPRUN_SCREEN_TITLE_COLOUR;
		_topCountryName.color = GlobalColors.TOPRUN_COUNTRY_NAME_COLOUR;
		_topCountryFlag.spriteName = CountryManager.instance.GetCurrentTournamentCountry().flag;
		_topCountryName.text = CountryManager.instance.GetCurrentTournamentCountry().name.ToUpper();
		PlayerInfo.Instance.hasShownTopRunIntroductionPopup = true;
	}

	public void OKClicked()
	{
		UIScreenController.Instance.ignoreLastPopupClosed = true;
		UIScreenController.Instance.PushScreen("TopRunUI");
		UIScreenController.Instance.CloseActivePopups(screenName);
	}
}
