using System;
using System.Collections.Generic;
using System.Text;
using SYBO.Subway.Info;
using UnityEngine;

public class LimitedCharacterPopup : UIScreen
{
	[SerializeField]
	private GameObject _characterSelectButtonPrefab;

	[SerializeField]
	private GameObject _centerAnchor;

	[SerializeField]
	private GameObject _characterAnchor;

	[SerializeField]
	private UIWidget _coinBoxAnchor;

	[SerializeField]
	private UILabel _nameLabel;

	[SerializeField]
	private UILabel _themeLimitedLabel;

	[SerializeField]
	private UICharacterThemeShifter _characterThemeShifter;

	private CharacterScreenSelectButton _characterSelectButton;

	private List<string> _boughtThemeNames = new List<string>();

	private bool _characterWasBought;

	public override void Init()
	{
		base.Init();
		if (UIModelController.Instance.limitedCharacterPopupAnchor == null)
		{
			UIModelController.Instance.limitedCharacterPopupAnchor = _characterAnchor;
		}
		InitializeSelectButton();
		InitializeCoinbox(_coinBoxAnchor, true, true, true, 3f, -1f);
	}

	public override void Show()
	{
		base.Show();
		LimitedCharacterPopupManager.Instance.currentThemeIndex = 0;
		UIModelController.Instance.ShowMenuModel(LimitedCharacterPopupManager.Instance.currentCharacter, LimitedCharacterPopupManager.Instance.currentThemeIndex, UIModelController.ModelScreen.LimitedCharacterPopup);
		UpdateUIElements();
		InitThemeShifter(LimitedCharacterPopupManager.Instance.currentCharacter, LimitedCharacterPopupManager.Instance.currentThemeIndex);
		_characterThemeShifter.AddOnChangeThemeListener(ThemeButtonClicked);
		_characterThemeShifter.gameObject.SetActive(CharacterThemes.characterCustomThemes.ContainsKey(LimitedCharacterPopupManager.Instance.currentCharacter));
		CharacterScreenManager.OnCharacterSelected += UpdateSelectedCharacter;
		CharacterScreenManager.Instance.AddOnCharacterUnlockedListener(OnCharacterUnlocked);
		UIScreenController.OnApplicationResumed += HidePopupIfNeeded;
		_characterWasBought = false;
		_boughtThemeNames.Clear();
	}

	public override void Hide()
	{
		base.Hide();
		_characterThemeShifter.RemoveOnChangeThemeListener(ThemeButtonClicked);
		CharacterScreenManager.OnCharacterSelected -= UpdateSelectedCharacter;
		CharacterScreenManager.Instance.RemoveOnCharacterUnlockedListener(OnCharacterUnlocked);
		UIScreenController.OnApplicationResumed -= HidePopupIfNeeded;
		UIModelController.Instance.ClearModels();
		LogFlurryEvent();
	}

	public override void GainFocus()
	{
		base.GainFocus();
		UIModelController.Instance.ShowMenuModel(LimitedCharacterPopupManager.Instance.currentCharacter, LimitedCharacterPopupManager.Instance.currentThemeIndex, UIModelController.ModelScreen.LimitedCharacterPopup);
	}

	public override void LooseFocus()
	{
		base.LooseFocus();
		UIModelController.Instance.ClearModels();
	}

	private void LogFlurryEvent()
	{
		Characters.CharacterType currentCharacter = LimitedCharacterPopupManager.Instance.currentCharacter;
		string arg = Strings.Get(Characters.characterData[currentCharacter].name).ToLower();
		StringBuilder stringBuilder = new StringBuilder(string.Format("Character {0} bought", arg));
		stringBuilder.Append(";");
		StringBuilder stringBuilder2 = new StringBuilder(GetStateString(_characterWasBought));
		stringBuilder2.Append(";");
		List<CharacterTheme> list = CharacterThemes.TryGetCustomThemesForChar(currentCharacter);
		if (list != null)
		{
			string[] array = new string[list.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = GetThemeName(list[i]);
				bool state = _boughtThemeNames.Contains(array[i]);
				stringBuilder.AppendFormat("{0}_{1} bought", arg, array[i]);
				stringBuilder.Append(";");
				stringBuilder2.AppendFormat("{0};", GetStateString(state));
			}
		}
		Flurry.LogEventWithSeveralParameters("Limited character popup closed", stringBuilder.ToString(), stringBuilder2.ToString());
	}

	private string GetThemeName(CharacterTheme theme)
	{
		return Strings.Get(theme.title);
	}

	private string GetStateString(bool state)
	{
		return (!state) ? "no" : "yes";
	}

	private void ThemeButtonClicked(int index)
	{
		if (LimitedCharacterPopupManager.Instance.currentThemeIndex != index)
		{
			LimitedCharacterPopupManager.Instance.currentThemeIndex = index;
			UIModelController.Instance.ShowMenuModel(LimitedCharacterPopupManager.Instance.currentCharacter, index, UIModelController.ModelScreen.LimitedCharacterPopup);
			UpdateUIElements();
			_characterThemeShifter.UpdateUIForCharacter(LimitedCharacterPopupManager.Instance.currentCharacter, index);
		}
	}

	private void HidePopupIfNeeded()
	{
		if (!LimitedCharacterPopupManager.Instance.IsCharacterAvailable())
		{
			UIScreenController.Instance.CloseActivePopups("LimitedCharacterPopup");
		}
	}

	private void InitializeSelectButton()
	{
		GameObject gameObject = NGUITools.AddChild(_centerAnchor, _characterSelectButtonPrefab);
		_characterSelectButton = gameObject.GetComponent<CharacterScreenSelectButton>();
		gameObject.transform.localPosition = new Vector3(62f, -42f, 0f);
		_characterThemeShifter.InitValues(LimitedCharacterPopupManager.Instance.currentCharacter, LimitedCharacterPopupManager.Instance.currentThemeIndex);
		_characterSelectButton.SetStickerColors(CharacterScreenSelectButton.StickerColors.Discount);
		_characterSelectButton.InitButton(CharacterScreenSelectButton.ButtonContext.limitedCharacterPopup);
	}

	private void ChangePositionOnYAxis(Transform objectTransform, float newYPosition)
	{
		Vector3 localPosition = objectTransform.localPosition;
		localPosition.y = newYPosition;
		objectTransform.localPosition = localPosition;
	}

	private void InitThemeShifter(Characters.CharacterType charType, int index)
	{
		bool flag = CharacterThemes.characterCustomThemes.ContainsKey(charType);
		_characterThemeShifter.gameObject.SetActive(flag);
		if (flag)
		{
			List<CharacterTheme> themes = CharacterThemes.TryGetCustomThemesForChar(charType);
			_characterThemeShifter.MakeCustomThemesAvailable(themes);
			_characterThemeShifter.UpdateUIForCharacter(charType, index);
		}
	}

	public void UpdateUIElements()
	{
		Characters.CharacterType currentlyShownModel = UIModelController.Instance.currentlyShownModel;
		Characters.Model model = Characters.characterData[currentlyShownModel];
		bool flag = PlayerInfo.Instance.IsCollectionComplete(currentlyShownModel);
		CharacterTheme? themeForCharacter = CharacterThemes.GetThemeForCharacter(currentlyShownModel, LimitedCharacterPopupManager.Instance.currentThemeIndex);
		if (flag)
		{
			if (model.hasOnlineSettings && !string.IsNullOrEmpty(Strings.Get(model.characterLimitedDescription)))
			{
				_themeLimitedLabel.text = Strings.Get(model.characterLimitedDescription);
				_themeLimitedLabel.enabled = true;
			}
			else
			{
				_themeLimitedLabel.text = string.Empty;
				_themeLimitedLabel.enabled = false;
			}
		}
		else if (model.hasOnlineSettings)
		{
			if (!themeForCharacter.HasValue)
			{
				_characterSelectButton.SetStickerColors(CharacterScreenSelectButton.StickerColors.Discount);
				TimeSpan timeSpan = PlayerInfo.Instance.CharacterExpirationTimespan(currentlyShownModel);
				if (timeSpan.Days < 1)
				{
					_characterSelectButton.stickerDayLabel.text = Strings.Get(StringID.CHARACTER_SCREEN_LAST);
					_characterSelectButton.stickerLeftLabel.text = Strings.Get(StringID.CHARACTER_SCREEN_DAY);
				}
				else
				{
					_characterSelectButton.stickerDayLabel.text = string.Format(Strings.Get(StringID.CHARACTER_SCREEN_DAYS_LEFT), timeSpan.Days, (timeSpan.Days == 1) ? string.Empty : "S");
					_characterSelectButton.stickerLeftLabel.text = Strings.Get(StringID.CHARACTER_SCREEN_LEFT);
				}
				_characterSelectButton.stickerDayLabel.fontSize = 10;
				ChangePositionOnYAxis(_characterSelectButton.stickerDayLabel.transform, 6f);
				ChangePositionOnYAxis(_characterSelectButton.stickerLeftLabel.transform, -5f);
			}
			if (!string.IsNullOrEmpty(Strings.Get(model.characterLimitedDescription)))
			{
				_themeLimitedLabel.text = Strings.Get(model.characterLimitedDescription);
			}
			else
			{
				_themeLimitedLabel.text = string.Empty;
			}
			_themeLimitedLabel.enabled = true;
		}
		else
		{
			_themeLimitedLabel.text = string.Empty;
			_characterSelectButton.stickerDayLabel.text = string.Empty;
			_characterSelectButton.stickerLeftLabel.text = string.Empty;
			_characterSelectButton.stickerDayLabel.enabled = false;
			_themeLimitedLabel.enabled = false;
			_characterSelectButton.stickerLeftLabel.enabled = false;
			_characterSelectButton.backgroundSticker.enabled = false;
		}
		if (themeForCharacter.HasValue)
		{
			CharacterTheme value = themeForCharacter.Value;
			_nameLabel.text = Strings.Get(value.title);
			string text = Strings.Get(value.themeDescription);
			if (!string.IsNullOrEmpty(text))
			{
				_themeLimitedLabel.text = text;
			}
		}
		else
		{
			_nameLabel.text = Strings.Get(model.name);
		}
		UpdateCharacterSticker(LimitedCharacterPopupManager.Instance.currentCharacter, LimitedCharacterPopupManager.Instance.currentThemeIndex);
		_characterSelectButton.ReloadButton();
	}

	public void UpdateSelectedCharacter()
	{
		Characters.CharacterType currentlyShownModel = UIModelController.Instance.currentlyShownModel;
		int currentlyShownThemeIndex = UIModelController.Instance.currentlyShownThemeIndex;
		bool flag = PlayerInfo.Instance.IsCollectionComplete(currentlyShownModel);
		bool flag2 = PlayerInfo.Instance.IsThemeUnlockedForCharacter(currentlyShownModel, currentlyShownThemeIndex);
		if (flag && flag2)
		{
			PlayerInfo.Instance.SetLastSelectedTheme(currentlyShownModel, currentlyShownThemeIndex);
			_characterThemeShifter.UpdateUIForCharacter(currentlyShownModel, currentlyShownThemeIndex);
			_characterSelectButton.ReloadButton();
		}
		else
		{
			Debug.LogWarning("Trying to select a character or theme we don't currently own.");
		}
	}

	public void UpdateCharacterSticker(Characters.CharacterType charType, int index)
	{
		Characters.Model model = Characters.characterData[charType];
		CharacterTheme? themeForCharacter = CharacterThemes.GetThemeForCharacter(charType, index);
		bool flag = PlayerInfo.Instance.IsCollectionComplete(charType);
		bool hasValue = themeForCharacter.HasValue;
		bool flag2 = hasValue && themeForCharacter.Value.price == 0 && !CharacterStickerManager.isThemeUnlockedForCharacter(charType, index);
		bool flag3 = !hasValue && model.hasOnlineSettings && !flag;
		bool flag4 = flag2 || flag3;
		bool flag5 = LimitedCharacterPopupManager.Instance.CurrentlyShownCharacterMatchesEventItem();
		_characterSelectButton.stickerDayLabel.enabled = flag4 && !flag5;
		_characterSelectButton.stickerLeftLabel.enabled = flag4 && !flag5;
		_characterSelectButton.backgroundSticker.enabled = flag4 || flag5;
		_characterSelectButton.eventIcon.enabled = flag5;
		if (flag2)
		{
			_characterSelectButton.stickerDayLabel.text = Strings.Get(StringID.BOARD_SCREEN_FREE);
			_characterSelectButton.stickerLeftLabel.text = Strings.Get(StringID.BOARD_SCREEN_GIFT);
			_characterSelectButton.SetStickerColors(CharacterScreenSelectButton.StickerColors.Gift);
			_characterSelectButton.stickerDayLabel.fontSize = 15;
			ChangePositionOnYAxis(_characterSelectButton.stickerDayLabel.transform, 5.5f);
			ChangePositionOnYAxis(_characterSelectButton.stickerLeftLabel.transform, -7f);
		}
		else if (flag5)
		{
			_characterSelectButton.backgroundSticker.color = GlobalColors.GIFT_COLOR;
		}
	}

	private void OnCharacterUnlocked(Characters.CharacterType charType, int version)
	{
		LimitedCharacterPopupManager.Instance.currentThemeIndex = version;
		UpdateSelectedCharacter();
		UpdateUIElements();
		_characterWasBought = true;
		if (version > 0)
		{
			CharacterTheme? themeForCharacter = CharacterThemes.GetThemeForCharacter(charType, version);
			if (themeForCharacter.HasValue)
			{
				_boughtThemeNames.Add(GetThemeName(themeForCharacter.Value));
			}
		}
	}
}
