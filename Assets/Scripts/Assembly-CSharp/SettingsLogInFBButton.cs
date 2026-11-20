using System;
using UnityEngine;

public class SettingsLogInFBButton : MonoBehaviour
{
	[SerializeField]
	private UILabel _description;

	[SerializeField]
	private UILabel _title;

	[SerializeField]
	private UILabel _connecting;

	[SerializeField]
	private UISprite _fbIcon;

	[SerializeField]
	private UISprite _spinner;

	[SerializeField]
	private GameObject _overlay;

	[SerializeField]
	private UISprite _fill;

	[SerializeField]
	private SettingsPopup settingsPopup;

	private bool _facebookLoginRunning;

	private void OnClick()
	{
		HouseKeeper.SetFacebookPayoutAction(FacebookStatusChanged);
		_facebookLoginRunning = true;
		if (SocialManager.instance.FacebookLogin(OnFacebookLoginResult))
		{
			UIScreenController.Instance.FacebookLogIn(true);
		}
		SetToConnectingState();
	}

	public void OnFacebookLoginResult(bool loggedIn)
	{
		if (!loggedIn && _facebookLoginRunning)
		{
			_facebookLoginRunning = false;
			QueueFacebookFailSlideIn(OnFailSlideInFinish);
		}
	}

	private void QueueFacebookFailSlideIn(Action onFinishCallback)
	{
		UIScreenController.Instance.QueueSlideIn(new UIScreenController.SlideIn(UIScreenController.SlideInType.Facebook, UISlideInFacebook.FacebookSlideInState.ConnectFailed, onFinishCallback));
	}

	private void OnFailSlideInFinish()
	{
		FacebookStatusChanged(false);
	}

	private void FacebookStatusChanged(bool success)
	{
		if (success)
		{
			if (!FacebookRewardsInfo.HasReceivedAllAvailableRewards())
			{
				UIScreenController.Instance.PushPopup("FacebookRewardsPopup");
			}
			else
			{
				UIScreenController.Instance.QueueSlideIn(new UIScreenController.SlideIn(UIScreenController.SlideInType.Facebook, UISlideInFacebook.FacebookSlideInState.ConnectSucceeded));
			}
			settingsPopup.ConnectedToFacebook();
		}
		else
		{
			if (_facebookLoginRunning)
			{
				OnFacebookLoginResult(false);
			}
			SetToLogInState();
		}
		HouseKeeper.SetFacebookPayoutActionToDefault();
		_facebookLoginRunning = false;
	}

	private void OnEnable()
	{
		_description.text = Strings.Get(StringID.SETTINGS_POPUP_FB_LOG_IN_DESC);
		_title.text = Strings.Get(StringID.SETTINGS_POPUP_FB_LOG_IN_TITLE);
		_connecting.text = Strings.Get(StringID.SETTINGS_POPUP_FB_LOG_IN_CONNECTING);
		SetToLogInState();
	}

	private void OnDisable()
	{
		if (_facebookLoginRunning)
		{
			HouseKeeper.SetFacebookPayoutActionToDefault();
			_facebookLoginRunning = false;
		}
	}

	private void SetToLogInState()
	{
		_overlay.SetActive(true);
		_fill.color = GlobalColors.TERTIARY_ACTION_COLOR;
		_title.gameObject.SetActive(true);
		_description.gameObject.SetActive(true);
		_fbIcon.gameObject.SetActive(true);
		_connecting.gameObject.SetActive(false);
		_spinner.gameObject.SetActive(false);
		GetComponent<Collider>().enabled = true;
	}

	private void SetToConnectingState()
	{
		_overlay.SetActive(false);
		_fill.color = GlobalColors.SOCIAL_GREY;
		_title.gameObject.SetActive(false);
		_description.gameObject.SetActive(false);
		_fbIcon.gameObject.SetActive(false);
		_connecting.gameObject.SetActive(true);
		_spinner.gameObject.SetActive(true);
		GetComponent<Collider>().enabled = false;
	}
}
