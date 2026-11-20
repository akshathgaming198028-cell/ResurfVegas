using TopRun;
using UnityEngine;

public class FBLogOutButton : MonoBehaviour
{
	[SerializeField]
	private UILabel _description;

	[SerializeField]
	private UILabel _title;

	[SerializeField]
	private GameObject _overlay;

	[SerializeField]
	private UISprite _fill;

	[SerializeField]
	private SettingsPopup settingsPopup;

	private void OnClick()
	{
		SocialManager.instance.FacebookLogout();
		CachedLeaderboard.RemoveFacebookFriends();
		UIScreenController.Instance.QueueSlideIn(new UIScreenController.SlideIn(UIScreenController.SlideInType.Facebook, UISlideInFacebook.FacebookSlideInState.LoggedOut));
		IManager manager = TopRunRoot.manager;
		if (manager != null)
		{
			manager.RefreshTournament();
		}
		settingsPopup.LoggedOutOfFacebook();
	}

	private void Disable()
	{
		_description.text = string.Empty;
		_title.text = Strings.Get(StringID.SETTINGS_POPUP_FB_LOGGED_OUT);
		_overlay.SetActive(false);
		TweenColor component = _fill.gameObject.GetComponent<TweenColor>();
		if (component != null)
		{
			component.enabled = false;
		}
		_fill.color = GlobalColors.TERTIARY_ACTION_COLOR_UNAVAILABLE;
		GetComponent<Collider>().enabled = false;
	}

	private void Enable()
	{
		_description.text = Strings.Get(StringID.SETTINGS_POPUP_FB_LOG_OUT_DESC);
		_title.text = Strings.Get(StringID.SETTINGS_POPUP_FB_LOG_OUT_TITLE);
		_overlay.SetActive(true);
		_fill.color = GlobalColors.TERTIARY_ACTION_COLOR;
		GetComponent<Collider>().enabled = true;
	}

	private void OnEnable()
	{
		Enable();
	}
}
