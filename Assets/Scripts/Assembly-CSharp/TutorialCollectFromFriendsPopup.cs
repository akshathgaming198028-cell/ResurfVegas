using SYBO.Subway.Info;
using UnityEngine;

public class TutorialCollectFromFriendsPopup : CharacterPopup
{
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
		topTitle.text = Strings.Get(StringID.POPUP_TUTORIAL_COLLECT_FROM_FRIENDS_TITLE);
		line1Big.text = Strings.Get(StringID.POPUP_TUTORIAL_COLLECT_FROM_FRIENDS_LINE1BIG);
		line1Small.text = Strings.Get(StringID.POPUP_TUTORIAL_COLLECT_FROM_FRIENDS_LINE1SMALL);
		line2Big.text = Strings.Get(StringID.POPUP_TUTORIAL_COLLECT_FROM_FRIENDS_LINE2BIG);
		line2Small.text = Strings.Get(StringID.POPUP_TUTORIAL_COLLECT_FROM_FRIENDS_LINE2SMALL);
		line3Big.text = Strings.Get(StringID.POPUP_TUTORIAL_COLLECT_FROM_FRIENDS_LINE3BIG);
		line3Small.text = Strings.Get(StringID.POPUP_TUTORIAL_COLLECT_FROM_FRIENDS_LINE3SMALL);
		buttonText.text = Strings.Get(StringID.POPUP_TUTORIAL_COLLECT_FROM_FRIENDS_BUTTON);
	}

	private void OkClicked()
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
