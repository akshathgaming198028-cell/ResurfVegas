using System;
using System.Collections.Generic;
using SYBO.Subway.Info;

public class CharacterScreenManager
{
	public enum CharacterPurchaseInitiator
	{
		none = 0,
		characterScreen = 1,
		characterNameEvent = 2,
		limitedCharacterPopup = 3
	}

	private static CharacterScreenManager _instance;

	private List<KeyValuePair<Characters.CharacterType, Characters.Model>> _characterList;

	private PlayerInfo _playerInfoInstance;

	private bool _purchaseInProgress;

	private bool _facebookLoginRunning;

	private CharacterPurchaseInitiator _characterPurchaseInitiator;

	private Action<Characters.CharacterType, int> _onCharacterUnlocked;

	public Action OnFacebookStatusChanged;

	public Characters.CharacterType currenlyShownCharacter { get; set; }

	public int currentlyShownThemeIndex { get; set; }

	public static CharacterScreenManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new CharacterScreenManager();
			}
			return _instance;
		}
	}

	public static bool isInitialized
	{
		get
		{
			return _instance != null;
		}
	}

	public static event Action OnCharacterSelected;

	private CharacterScreenManager()
	{
		_characterList = new List<KeyValuePair<Characters.CharacterType, Characters.Model>>();
		_playerInfoInstance = PlayerInfo.Instance;
	}

	public void PurchaseCharacter(CharacterPurchaseInitiator initiator, Characters.CharacterType characterType, int themeIndex)
	{
		if (!_purchaseInProgress)
		{
			_purchaseInProgress = true;
			_characterPurchaseInitiator = initiator;
			PurchaseHandler.Instance.PurchaseCharacter(characterType, themeIndex);
		}
	}

	public void PurchaseCharacterWithInapp(CharacterPurchaseInitiator initiator, Characters.CharacterType characterType)
	{
		if (!_purchaseInProgress)
		{
			_purchaseInProgress = true;
			_characterPurchaseInitiator = initiator;
			PurchaseHandler.Instance.StartPurchaseCharacterWithInapp(characterType);
		}
	}

	public void CharacterPurchaseSuccessful(Characters.CharacterType purchasedCharacter, int themeIndex)
	{
		if (_purchaseInProgress)
		{
			if (_characterPurchaseInitiator == CharacterPurchaseInitiator.characterNameEvent && themeIndex == 0 && Characters.characterData[purchasedCharacter].unlockType == Characters.UnlockType.coins)
			{
				Flurry.LogEventWithSeveralParameters("Unlocked character for Wordy Weekend", "Character;Price", string.Concat(purchasedCharacter, ";", Characters.characterData[purchasedCharacter].Price));
				Flurry.LogEvent(string.Format("{0} unlocked for Wordy Weekend", purchasedCharacter));
			}
			_purchaseInProgress = false;
			_characterPurchaseInitiator = CharacterPurchaseInitiator.none;
			OnCharacterUnlocked(purchasedCharacter, themeIndex);
			UIScreenController.Instance.ShowUnlockAnimationForCharacter(purchasedCharacter, themeIndex);
			PlayerInfo.Instance.SaveIfDirty();
		}
	}

	public void CharacterPurchaseFailure()
	{
		if (_purchaseInProgress)
		{
			_characterPurchaseInitiator = CharacterPurchaseInitiator.none;
			_purchaseInProgress = false;
		}
	}

	public void AddOnCharacterUnlockedListener(Action<Characters.CharacterType, int> handler)
	{
		if (handler != null)
		{
			_onCharacterUnlocked = (Action<Characters.CharacterType, int>)Delegate.Combine(_onCharacterUnlocked, handler);
		}
	}

	public void RemoveOnCharacterUnlockedListener(Action<Characters.CharacterType, int> handler)
	{
		if (handler != null)
		{
			_onCharacterUnlocked = (Action<Characters.CharacterType, int>)Delegate.Remove(_onCharacterUnlocked, handler);
		}
	}

	private void OnCharacterUnlocked(Characters.CharacterType character, int version)
	{
		if (_onCharacterUnlocked != null)
		{
			SelectCharacter(character, version);
			_onCharacterUnlocked(character, version);
		}
	}

	public void SelectCharacter(Characters.CharacterType charType, int themeIndex)
	{
		UIModelController.Instance.SelectCharacterForPlay(charType, themeIndex);
		CharacterScreen.forceCenterOnCurrentlySelectedCharacter = true;
		if (CharacterScreenManager.OnCharacterSelected != null)
		{
			CharacterScreenManager.OnCharacterSelected();
		}
	}

	public void StartLoginToFacebook()
	{
		HouseKeeper.SetFacebookPayoutAction(FacebookStatusChanged);
		_facebookLoginRunning = true;
		if (SocialManager.instance.FacebookLogin(OnFacebookLoginResult))
		{
			UIScreenController.Instance.FacebookLogIn(true);
		}
	}

	public void OnFacebookLoginResult(bool loggedIn)
	{
		if (!loggedIn)
		{
			QueueFacebookFailSlideIn(OnFacebookLoginStopped);
		}
		else
		{
			_facebookLoginRunning = false;
		}
	}

	private void OnFacebookLoginStopped()
	{
		_facebookLoginRunning = false;
		Action onFacebookStatusChanged = OnFacebookStatusChanged;
		if (onFacebookStatusChanged != null)
		{
			onFacebookStatusChanged();
		}
	}

	private void QueueFacebookFailSlideIn(Action onFinishCallback)
	{
		UIScreenController.Instance.QueueSlideIn(new UIScreenController.SlideIn(UIScreenController.SlideInType.Facebook, UISlideInFacebook.FacebookSlideInState.ConnectFailed, onFinishCallback));
	}

	private void FacebookStatusChanged(bool success)
	{
		HouseKeeper.SetFacebookPayoutActionToDefault();
		if (success)
		{
			_facebookLoginRunning = false;
			if (!FacebookRewardsInfo.HasReceivedAllAvailableRewards())
			{
				UIScreenController.Instance.PushPopup("FacebookRewardsPopup");
				return;
			}
			OnFacebookCharacterUnlocked();
			UIScreenController.Instance.QueueSlideIn(new UIScreenController.SlideIn(UIScreenController.SlideInType.Facebook, UISlideInFacebook.FacebookSlideInState.ConnectSucceeded));
		}
	}

	public void OnFacebookCharacterUnlocked()
	{
		OnCharacterUnlocked(Characters.CharacterType.facebookRewardCharacter01, 0);
	}

	public bool isFacebookLoginRunning()
	{
		return _facebookLoginRunning;
	}

	public void OnFacebookLogout()
	{
		PlayerInfo instance = PlayerInfo.Instance;
		if (instance.currentCharacter == 36)
		{
			UIModelController.Instance.SelectCharacterForPlay(Characters.CharacterType.slick, 0);
		}
		Action onFacebookStatusChanged = OnFacebookStatusChanged;
		if (onFacebookStatusChanged != null)
		{
			onFacebookStatusChanged();
		}
	}

	public void OnCharacterScreenHide()
	{
		if (_facebookLoginRunning)
		{
			HouseKeeper.SetFacebookPayoutActionToDefault();
		}
	}

	public Characters.CharacterType GetCurrenlySelectedCharacter()
	{
		return (Characters.CharacterType)PlayerInfo.Instance.currentCharacter;
	}

	public Characters.CharacterType? GetCharacterTypeActiveInEvent()
	{
		Characters.CharacterType? result = null;
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
		if (activeEventManager != null && (activeEventManager.GetEventType() == SSEventType.CharacterDay || activeEventManager.GetEventType() == SSEventType.ExpiredLimitedCharacter))
		{
			bool found = false;
			result = activeEventManager.GetEventValue<Characters.CharacterType>(out found);
		}
		return result;
	}

	public int GetLastSelectedThemeForCharacterType(Characters.CharacterType charType)
	{
		int result = 0;
		if (CharacterThemes.GetThemeForCharacter(charType, _playerInfoInstance.GetIndexForLastSelectedTheme(charType)).HasValue)
		{
			result = _playerInfoInstance.GetIndexForLastSelectedTheme(charType);
		}
		return result;
	}

	public void InitCharacters()
	{
		_characterList = new List<KeyValuePair<Characters.CharacterType, Characters.Model>>();
		foreach (Characters.CharacterType item in Characters.characterOrder)
		{
			Characters.Model value = Characters.characterData[item];
			if (_playerInfoInstance.IsCollectionComplete(item) || _playerInfoInstance.isCharacterActive(item) || !value.hasOnlineSettings)
			{
				_characterList.Add(new KeyValuePair<Characters.CharacterType, Characters.Model>(item, value));
			}
		}
	}

	public List<KeyValuePair<Characters.CharacterType, Characters.Model>> GetCharacterList()
	{
		if (_characterList.Count == 0)
		{
			InitCharacters();
		}
		return _characterList;
	}
}
