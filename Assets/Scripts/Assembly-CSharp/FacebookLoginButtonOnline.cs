using System.Collections;
using UnityEngine;

public class FacebookLoginButtonOnline : MonoBehaviour
{
	[SerializeField]
	private GameObject _connectingToGameObject;

	[SerializeField]
	private GameObject _couldNotConnectTryAgainGameObject;

	[SerializeField]
	private GameObject _couldNotConnectGameObject;

	[SerializeField]
	private GameObject _loginToFbWithBonusGameObject;

	[SerializeField]
	private GameObject _loginToFbNoBonusGameObject;

	[SerializeField]
	private GameObject _loginToFbForAwardsGameObject;

	[SerializeField]
	private GameObject _loginToFbForTopRunGameObject;

	[SerializeField]
	private UIButtonOverlayOff _btnOverlayOffDefault;

	[SerializeField]
	private UIButtonOverlayOff _btnOverlayOffTryAgain;

	[SerializeField]
	private UILabel _connectingTo;

	[SerializeField]
	private UILabel _couldNotConnect1;

	[SerializeField]
	private UILabel _couldNotConnect2;

	[SerializeField]
	private UILabel _tryAgain1;

	[SerializeField]
	private UILabel _tryAgain2;

	[SerializeField]
	private UILabel _loginToFb1;

	[SerializeField]
	private UILabel _loginToFb2;

	[SerializeField]
	private UILabel _loginToFb3;

	[SerializeField]
	private UILabel _loginToFb4;

	[SerializeField]
	private UILabel _loginToFb5;

	[SerializeField]
	private UILabel _loginToFb6;

	[SerializeField]
	private UILabel _loginToFb7;

	private bool _isNoBonus;

	private bool _isGameOverScreen;

	private bool _shouldDisplayAwardsSpecific;

	private bool _shouldDisplayTopRunSpecific;

	private bool _enabled = true;

	public void Init(bool isLoggedIntoFacebook, bool isNoBonus, bool isAwardsSpecific = false, bool isLoggedIntoGC = false, bool isTopRunSpecific = false, bool noGCFriends = false, bool isGameOverScreen = false)
	{
		_isNoBonus = isNoBonus;
		_isGameOverScreen = isGameOverScreen;
		_shouldDisplayAwardsSpecific = isAwardsSpecific && !isLoggedIntoGC;
		_shouldDisplayTopRunSpecific = isTopRunSpecific && isNoBonus;
		SetupButton(noGCFriends);
		if (isLoggedIntoFacebook)
		{
			SetStateGameObjectActive(_couldNotConnectTryAgainGameObject);
			EnableTouch(true);
			return;
		}
		if (_shouldDisplayTopRunSpecific)
		{
			SetStateGameObjectActive(_loginToFbForTopRunGameObject);
		}
		else if (_shouldDisplayAwardsSpecific)
		{
			SetStateGameObjectActive(_loginToFbForAwardsGameObject);
		}
		else
		{
			SetStateGameObjectActive((!isNoBonus) ? _loginToFbWithBonusGameObject : _loginToFbNoBonusGameObject);
		}
		EnableTouch(true);
	}

	private void SetupButton(bool noGCFriends)
	{
		TrySetTextToLabel(_connectingTo, Strings.Get(StringID.SOCIAL_BUTTON_FEEDBACK_CONNECTING));
		TrySetTextToLabel(_couldNotConnect1, Strings.Get(StringID.SOCIAL_BUTTON_FEEDBACK_COULD_NOT_CONNECT_1));
		TrySetTextToLabel(_couldNotConnect2, Strings.Get(StringID.SOCIAL_BUTTON_FEEDBACK_COULD_NOT_CONNECT_2));
		TrySetTextToLabel(_tryAgain1, Strings.Get(StringID.SOCIAL_BUTTON_FEEDBACK_TRY_AGAIN_1));
		TrySetTextToLabel(_tryAgain2, Strings.Get(StringID.SOCIAL_BUTTON_FEEDBACK_TRY_AGAIN_2));
		if (_shouldDisplayAwardsSpecific)
		{
			TrySetTextToLabel(_loginToFb6, Strings.Get(StringID.SOCIAL_BUTTON_FEEDBACK_LOGIN_1_AWARDS));
			TrySetTextToLabel(_loginToFb7, Strings.Get(StringID.SOCIAL_BUTTON_FEEDBACK_LOGIN_2_AWARDS));
		}
		else
		{
			StringID stringID = ((!noGCFriends) ? StringID.FACEBOOK_BUTTON_FEEDBACK_LOGIN_2_NO_BONUS : StringID.FACEBOOK_BUTTON_FEEDBACK_LOGIN_2_NO_BONUS_NO_GC_FRIENDS);
			TrySetTextToLabel((!_isNoBonus) ? _loginToFb1 : _loginToFb4, Strings.Get((!_isNoBonus) ? StringID.SOCIAL_BUTTON_FEEDBACK_LOGIN_1 : StringID.FACEBOOK_BUTTON_FEEDBACK_LOGIN_1_NO_BONUS));
			TrySetTextToLabel((!_isNoBonus) ? _loginToFb2 : _loginToFb5, Strings.Get((!_isNoBonus) ? StringID.SOCIAL_BUTTON_FEEDBACK_LOGIN_2 : stringID));
			TrySetTextToLabel(_loginToFb3, string.Format(Strings.Get(StringID.SOCIAL_BUTTON_FEEDBACK_LOGIN_3), 5000));
			Vector2 zero = Vector2.zero;
			zero = ((!noGCFriends) ? new Vector2(13f, -1f) : new Vector2(30f, -1f));
			UIAnchor component = _loginToFb4.GetComponent<UIAnchor>();
			if (component != null)
			{
				component.pixelOffset = zero;
			}
		}
		SocialManager.instance.AddFacebookStateChangedHandler(FacebookStateChanged);
		SocialManager.instance.AddFriendsConsolidatedHandler(FriendsConsolidated);
		FacebookStateChanged();
	}

	private void TrySetTextToLabel(UILabel lbl, string text)
	{
		if (lbl != null)
		{
			lbl.text = text;
		}
	}

	private void OnDestroy()
	{
		SocialManager.instance.RemoveFacebookStateChangedHandler(FacebookStateChanged);
		SocialManager.instance.RemoveFriendsConsolidatedHandler(FriendsConsolidated);
	}

	private void OnPress(bool pressed)
	{
		if (_enabled)
		{
			if (_btnOverlayOffDefault != null)
			{
				_btnOverlayOffDefault.ButtonPressed(pressed);
			}
			if (_btnOverlayOffTryAgain != null && _couldNotConnectTryAgainGameObject != null && _couldNotConnectTryAgainGameObject.activeInHierarchy)
			{
				_btnOverlayOffTryAgain.ButtonPressed(pressed);
			}
		}
	}

	private void OnClick()
	{
		if (_enabled)
		{
			SetStateGameObjectActive(_connectingToGameObject);
			EnableTouch(false);
			SocialManager.instance.FacebookLogin(FacebookLoggedIn);
		}
	}

	private void FacebookLoggedIn(bool success)
	{
		if ((bool)this)
		{
			if (!success)
			{
				StartCoroutine(UserFeedback());
			}
			else
			{
				UIScreenController.Instance.FacebookLogIn(true);
				EnableTouch(false);
			}
		}
		QueueSlideIn(success);
	}

	private IEnumerator UserFeedback()
	{
		yield return new WaitForSeconds(2f);
		SetStateGameObjectActive(_couldNotConnectTryAgainGameObject);
		EnableTouch(true);
	}

	private void QueueSlideIn(bool success)
	{
		if (success)
		{
			UIScreenController.Instance.QueueSlideIn(new UIScreenController.SlideIn(UIScreenController.SlideInType.Facebook, UISlideInFacebook.FacebookSlideInState.ConnectSucceeded));
		}
		else
		{
			UIScreenController.Instance.QueueSlideIn(new UIScreenController.SlideIn(UIScreenController.SlideInType.Facebook, UISlideInFacebook.FacebookSlideInState.ConnectFailed));
		}
	}

	private void FacebookStateChanged()
	{
		switch (SocialManager.instance.facebookState)
		{
		case SocialManager.FacebookState.LoggingIn:
			SetStateGameObjectActive(_connectingToGameObject);
			EnableTouch(false);
			break;
		case SocialManager.FacebookState.Error:
			SetStateGameObjectActive(_couldNotConnectTryAgainGameObject);
			EnableTouch(true);
			break;
		case SocialManager.FacebookState.LoggedIn:
			break;
		case SocialManager.FacebookState.None:
			break;
		}
	}

	private bool AreFacebookFriendsConsolidated()
	{
		int consolidationState = SocialManager.instance.consolidationState;
		return (consolidationState & 1) == 1;
	}

	private void SetStateGameObjectActive(GameObject go)
	{
		if (_connectingToGameObject != null)
		{
			_connectingToGameObject.SetActive(false);
		}
		if (_couldNotConnectGameObject != null)
		{
			_couldNotConnectGameObject.SetActive(false);
		}
		if (_couldNotConnectTryAgainGameObject != null)
		{
			_couldNotConnectTryAgainGameObject.SetActive(false);
		}
		if (_loginToFbWithBonusGameObject != null)
		{
			_loginToFbWithBonusGameObject.SetActive(false);
		}
		if (_loginToFbNoBonusGameObject != null)
		{
			_loginToFbNoBonusGameObject.SetActive(false);
		}
		if (_loginToFbForAwardsGameObject != null)
		{
			_loginToFbForAwardsGameObject.SetActive(false);
		}
		if (_loginToFbForTopRunGameObject != null)
		{
			_loginToFbForTopRunGameObject.SetActive(false);
		}
		if (go != null)
		{
			go.SetActive(true);
		}
		if (go != null && (go == _loginToFbWithBonusGameObject || go == _loginToFbNoBonusGameObject || go == _loginToFbForAwardsGameObject || go == _loginToFbForTopRunGameObject))
		{
			_btnOverlayOffDefault.SetButtonType(UIButtonOverlayOff.ButtonType.Secondary);
			if (go == _loginToFbForTopRunGameObject && !_isGameOverScreen)
			{
				_btnOverlayOffDefault.SetButtonType(UIButtonOverlayOff.ButtonType.Primary);
			}
		}
		else if (go != null)
		{
			_btnOverlayOffDefault.SetButtonType(UIButtonOverlayOff.ButtonType.SocialGrey);
		}
	}

	private void FriendsConsolidated(bool success)
	{
		if (SocialManager.instance.facebookIsLoggedIn && !AreFacebookFriendsConsolidated())
		{
			SetStateGameObjectActive(_couldNotConnectGameObject);
			EnableTouch(false);
		}
	}

	private void EnableTouch(bool enable)
	{
		_enabled = enable;
		if (_btnOverlayOffDefault != null && _loginToFbWithBonusGameObject != null && _loginToFbNoBonusGameObject != null && _loginToFbForAwardsGameObject != null && _loginToFbForTopRunGameObject != null && _couldNotConnectGameObject != null)
		{
			if (enable)
			{
				_btnOverlayOffDefault.GoToNormalState();
			}
			else
			{
				_btnOverlayOffDefault.GoToNotAvailableState();
			}
		}
	}
}
