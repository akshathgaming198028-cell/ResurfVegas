using System.Collections;
using Kiloo.Plugins.DeviceUtility;
using UnityEngine;

public class GameCenterButton : MonoBehaviour
{
	[SerializeField]
	private GameObject _connectingToGameObject;

	[SerializeField]
	private GameObject _couldNotConnectTryAgainGameObject;

	[SerializeField]
	private GameObject _couldNotConnectGameObject;

	[SerializeField]
	private GameObject _loginToGcWithBonusGameObject;

	[SerializeField]
	private GameObject _loginToGcNoBonusGameObject;

	[SerializeField]
	private GameObject _loginToGcForAwardsGameObject;

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
	private UILabel _loginToGc1;

	[SerializeField]
	private UILabel _loginToGc2;

	[SerializeField]
	private UILabel _loginToGc3;

	[SerializeField]
	private UILabel _loginToGc4;

	[SerializeField]
	private UILabel _loginToGc5;

	[SerializeField]
	private UILabel _loginToGc6;

	[SerializeField]
	private UILabel _loginToGc7;

	private bool _enabled = true;

	private bool _isNoBonus;

	private bool _shouldDisplayAwardsSpecific;

	private void SetupText()
	{
		TrySetTextToLabel(_connectingTo, Strings.Get(StringID.SOCIAL_BUTTON_FEEDBACK_CONNECTING));
		TrySetTextToLabel(_couldNotConnect1, Strings.Get(StringID.SOCIAL_BUTTON_FEEDBACK_COULD_NOT_CONNECT_1));
		TrySetTextToLabel(_couldNotConnect2, Strings.Get(StringID.SOCIAL_BUTTON_FEEDBACK_COULD_NOT_CONNECT_2));
		TrySetTextToLabel(_tryAgain1, Strings.Get(StringID.SOCIAL_BUTTON_FEEDBACK_TRY_AGAIN_1));
		TrySetTextToLabel(_tryAgain2, Strings.Get(StringID.SOCIAL_BUTTON_FEEDBACK_TRY_AGAIN_2));
		if (_shouldDisplayAwardsSpecific)
		{
			TrySetTextToLabel(_loginToGc6, Strings.Get(StringID.SOCIAL_BUTTON_FEEDBACK_LOGIN_1_AWARDS));
			TrySetTextToLabel(_loginToGc7, Strings.Get(StringID.SOCIAL_BUTTON_FEEDBACK_LOGIN_2_AWARDS));
		}
		else
		{
			TrySetTextToLabel((!_isNoBonus) ? _loginToGc1 : _loginToGc4, Strings.Get((!_isNoBonus) ? StringID.SOCIAL_BUTTON_FEEDBACK_LOGIN_1 : StringID.GAMECENTER_BUTTON_FEEDBACK_LOGIN_1_NO_BONUS));
			TrySetTextToLabel((!_isNoBonus) ? _loginToGc1 : _loginToGc4, Strings.Get((!_isNoBonus) ? StringID.SOCIAL_BUTTON_FEEDBACK_LOGIN_1 : StringID.GAMECENTER_BUTTON_FEEDBACK_LOGIN_1_NO_BONUS));
			TrySetTextToLabel((!_isNoBonus) ? _loginToGc2 : _loginToGc5, Strings.Get((!_isNoBonus) ? StringID.SOCIAL_BUTTON_FEEDBACK_LOGIN_2 : StringID.GAMECENTER_BUTTON_FEEDBACK_LOGIN_2_NO_BONUS));
		}
		TrySetTextToLabel(_loginToGc3, string.Format(Strings.Get(StringID.SOCIAL_BUTTON_FEEDBACK_LOGIN_3), 250));
	}

	private void TrySetTextToLabel(UILabel lbl, string text)
	{
		if (lbl != null)
		{
			lbl.text = text;
		}
	}

	private void OnEnable()
	{
		SocialManager.instance.AddFriendsConsolidatedHandler(FriendsConsolidated);
		SocialManagerIOS.instance.AddGameCenterStateChangedHandler(GameCenterStateChanged);
		GameCenterStateChanged();
	}

	private void OnDisable()
	{
		SocialManager.instance.RemoveFriendsConsolidatedHandler(FriendsConsolidated);
		SocialManagerIOS.instance.RemoveGameCenterStateChangedHandler(GameCenterStateChanged);
	}

	public void Init(bool isLoggedIntoGameCenter, bool isNoBonus, bool isAwardsSpecific = false, bool isLoggedIntoFB = false)
	{
		_isNoBonus = isNoBonus;
		_shouldDisplayAwardsSpecific = isAwardsSpecific && !isLoggedIntoFB;
		SetupText();
		if (isLoggedIntoGameCenter)
		{
			SetStateGameObjectActive(_couldNotConnectTryAgainGameObject);
			EnableTouch(true);
			return;
		}
		if (_shouldDisplayAwardsSpecific)
		{
			SetStateGameObjectActive(_loginToGcForAwardsGameObject);
		}
		else
		{
			SetStateGameObjectActive((!isNoBonus) ? _loginToGcWithBonusGameObject : _loginToGcNoBonusGameObject);
		}
		EnableTouch(true);
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
		if (!_enabled)
		{
			return;
		}
		SocialManager instance = SocialManager.instance;
		if (!instance.gamecenterIsLoggedIn)
		{
			if (!instance.hasGameCenterShowLoginView)
			{
				DeviceUtility.ShowNativePopup(Strings.Get(StringID.GAME_CENTER_BUTTON_NATIVE_POPUP_TITILE), Strings.Get(StringID.GAME_CENTER_BUTTON_NATIVE_POPUP_MESSAGE), Strings.Get(StringID.GAME_CENTER_BUTTON_NATIVE_POPUP_OK));
			}
		}
		else
		{
			SetStateGameObjectActive(_connectingToGameObject);
			EnableTouch(false);
			StartCoroutine("UserFeedback");
		}
	}

	private IEnumerator UserFeedback()
	{
		yield return new WaitForSeconds(2f);
		SetStateGameObjectActive(_couldNotConnectTryAgainGameObject);
		EnableTouch(true);
	}

	private void GameCenterStateChanged()
	{
		switch (SocialManagerIOS.instance.gameCenterState)
		{
		case SocialManagerIOS.GCState.LoggedOut:
			if (_shouldDisplayAwardsSpecific)
			{
				SetStateGameObjectActive(_loginToGcForAwardsGameObject);
			}
			else
			{
				SetStateGameObjectActive((!_isNoBonus) ? _loginToGcWithBonusGameObject : _loginToGcNoBonusGameObject);
			}
			EnableTouch(true);
			break;
		case SocialManagerIOS.GCState.Authenticating:
		case SocialManagerIOS.GCState.LoadingFriends:
			SetStateGameObjectActive(_connectingToGameObject);
			EnableTouch(false);
			break;
		case SocialManagerIOS.GCState.LoggedIn:
			break;
		}
	}

	private void FriendsConsolidated(bool success)
	{
		if (SocialManager.instance.gamecenterIsLoggedIn && !AreGamecenterFriendsConsolidated())
		{
			SetStateGameObjectActive(_couldNotConnectGameObject);
			EnableTouch(false);
		}
	}

	private bool AreGamecenterFriendsConsolidated()
	{
		int consolidationState = SocialManager.instance.consolidationState;
		return (consolidationState & 2) == 2;
	}

	private void SetStateGameObjectActive(GameObject go)
	{
		TrySetGameObjectActive(_connectingToGameObject, false);
		TrySetGameObjectActive(_couldNotConnectGameObject, false);
		TrySetGameObjectActive(_couldNotConnectTryAgainGameObject, false);
		TrySetGameObjectActive(_loginToGcWithBonusGameObject, false);
		TrySetGameObjectActive(_loginToGcNoBonusGameObject, false);
		TrySetGameObjectActive(_loginToGcForAwardsGameObject, false);
		TrySetGameObjectActive(go, true);
		if (go != null && (go == _loginToGcWithBonusGameObject || go == _loginToGcNoBonusGameObject || go == _loginToGcForAwardsGameObject))
		{
			_btnOverlayOffDefault.SetButtonType(UIButtonOverlayOff.ButtonType.Secondary);
		}
		else if (go != null)
		{
			_btnOverlayOffDefault.SetButtonType(UIButtonOverlayOff.ButtonType.SocialGrey);
		}
	}

	private void TrySetGameObjectActive(GameObject go, bool active)
	{
		if (go != null)
		{
			go.SetActive(active);
		}
	}

	private void EnableTouch(bool enable)
	{
		_enabled = enable;
		if (_btnOverlayOffDefault != null && _loginToGcWithBonusGameObject != null && _loginToGcNoBonusGameObject != null && _loginToGcForAwardsGameObject != null && _connectingToGameObject != null)
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
