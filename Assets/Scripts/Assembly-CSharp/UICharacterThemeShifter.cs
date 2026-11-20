using System;
using System.Collections.Generic;
using SYBO.Subway.Info;
using UnityEngine;

public class UICharacterThemeShifter : MonoBehaviour
{
	private const int MAX_NUMBER_OF_CUSTOM_THEMES = 3;

	[SerializeField]
	private GameObject _defaultCharacterThemeButton;

	[SerializeField]
	private GameObject _customCharacterThemeButton;

	[SerializeField]
	private Transform _selectedMarker;

	private Vector3 _distanceBetweenButtons = Vector3.zero;

	private List<CharacterThemeButton> _customButtons = new List<CharacterThemeButton>();

	private Action<int> _onThemeButtonPressed;

	private int _activeTheme;

	private Characters.CharacterType _cachedCharacter;

	private List<CharacterTheme> _currentCharacterThemes = new List<CharacterTheme>();

	private CharacterTheme[] _sortedThemes;

	public void AddOnChangeThemeListener(Action<int> handler)
	{
		if (handler != null)
		{
			_onThemeButtonPressed = (Action<int>)Delegate.Combine(_onThemeButtonPressed, handler);
		}
	}

	public void RemoveOnChangeThemeListener(Action<int> handler)
	{
		if (handler != null)
		{
			_onThemeButtonPressed = (Action<int>)Delegate.Remove(_onThemeButtonPressed, handler);
		}
	}

	public void InitValues(Characters.CharacterType charType, int index)
	{
		_cachedCharacter = charType;
		_activeTheme = index;
	}

	private void Awake()
	{
		Vector3 localPosition = _defaultCharacterThemeButton.transform.localPosition;
		Vector3 localPosition2 = _customCharacterThemeButton.transform.localPosition;
		_distanceBetweenButtons = localPosition2 - localPosition;
		_customButtons.Add(_customCharacterThemeButton.GetComponent<CharacterThemeButton>());
		_selectedMarker.GetComponent<UISprite>().color = GlobalColors.CHARACTERS_CUSTOMIZATION_BASE_SKIN_ICON;
		_defaultCharacterThemeButton.GetComponent<CharacterThemeButton>().AddOnPressListener(ThemeButtonPressed);
		_customButtons[0].AddOnPressListener(ThemeButtonPressed);
		_selectedMarker.transform.position = _defaultCharacterThemeButton.transform.position;
	}

	private CharacterTheme[] SortThemes(List<CharacterTheme> themes)
	{
		CharacterTheme[] array = new CharacterTheme[themes.Count];
		List<CharacterTheme> list = new List<CharacterTheme>();
		list.AddRange(themes);
		int num = int.MaxValue;
		CharacterTheme characterTheme = themes[0];
		for (int i = 0; i < themes.Count; i++)
		{
			foreach (CharacterTheme item in list)
			{
				if (item.uiPriority < num)
				{
					num = item.uiPriority;
					characterTheme = item;
				}
			}
			array[i] = characterTheme;
			list.Remove(characterTheme);
			num = int.MaxValue;
		}
		return array;
	}

	public void MakeCustomThemesAvailable(List<CharacterTheme> themes)
	{
		_currentCharacterThemes = themes;
		if (themes.Count > 0)
		{
			_sortedThemes = SortThemes(themes);
		}
		foreach (CharacterThemeButton customButton in _customButtons)
		{
			customButton.gameObject.SetActive(false);
		}
		int num = 0;
		if (themes != null)
		{
			num = ((themes.Count <= 3) ? themes.Count : 3);
		}
		_defaultCharacterThemeButton.GetComponent<CharacterThemeButton>().SetColors(GlobalColors.CHARACTERS_CUSTOMIZATION_BASE_SKIN_FILL, GlobalColors.CHARACTERS_CUSTOMIZATION_BASE_SKIN_ICON, 0);
		for (int i = 0; i < num; i++)
		{
			if (_customButtons.Count > i)
			{
				CharacterThemeButton characterThemeButton = _customButtons[i];
				if (_sortedThemes != null && _sortedThemes.Length > i)
				{
					CharacterTheme characterTheme = _sortedThemes[i];
					setActiveBubles(characterThemeButton, characterTheme, UIModelController.Instance.currentlyShownModel, themes.IndexOf(characterTheme) + 1);
					characterThemeButton.SetColors(characterTheme.buttonBgColor, characterTheme.buttonIconColor, themes.IndexOf(characterTheme) + 1);
				}
				characterThemeButton.gameObject.SetActive(true);
				continue;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(_customCharacterThemeButton);
			CharacterThemeButton component = gameObject.GetComponent<CharacterThemeButton>();
			if (component != null)
			{
				component.transform.parent = base.gameObject.transform;
				component.transform.localPosition = _customCharacterThemeButton.transform.localPosition + _distanceBetweenButtons * i;
				component.transform.localScale = Vector3.one;
				component.name = i + 1 + "CharThemeBtn";
				if (_sortedThemes != null && _sortedThemes.Length > i)
				{
					CharacterTheme characterTheme2 = _sortedThemes[i];
					setActiveBubles(component, characterTheme2, UIModelController.Instance.currentlyShownModel, themes.IndexOf(characterTheme2) + 1);
					component.SetColors(characterTheme2.buttonBgColor, characterTheme2.buttonIconColor, themes.IndexOf(characterTheme2) + 1);
				}
				_customButtons.Add(component);
				component.AddOnPressListener(ThemeButtonPressed);
			}
		}
	}

	private void OnDisable()
	{
		foreach (CharacterThemeButton customButton in _customButtons)
		{
			customButton.gameObject.SetActive(false);
		}
	}

	private void setActiveBubles(CharacterThemeButton btn, CharacterTheme ct, Characters.CharacterType type, int index)
	{
		if (!CharacterStickerManager.hasThemeBeenSeenForCharacter(type, index) && !PlayerInfo.Instance.isFreshInstall && ct.isNewInThisUpdate && ct.price != 0)
		{
			btn.setActiveBubles(false, true);
		}
		else if (!CharacterStickerManager.isThemeUnlockedForCharacter(type, index) && ct.price == 0)
		{
			btn.setActiveBubles(true, false);
		}
		else
		{
			btn.setActiveBubles(false, false);
		}
	}

	private void SelectButton(int index)
	{
		bool flag = index == 0;
		if (_selectedMarker != null)
		{
			if (flag)
			{
				_selectedMarker.transform.position = _defaultCharacterThemeButton.transform.position;
			}
			else
			{
				_selectedMarker.transform.position = _customButtons[_currentCharacterThemes.IndexOf(_sortedThemes[index - 1])].transform.position;
			}
		}
	}

	public void DisableCharacterCustomizator()
	{
		base.gameObject.SetActive(false);
	}

	private void ThemeButtonPressed(int index)
	{
		Action<int> onThemeButtonPressed = _onThemeButtonPressed;
		if (onThemeButtonPressed != null)
		{
			_onThemeButtonPressed(index);
		}
	}

	public void UpdateUIForCharacter(Characters.CharacterType charType, int outfitIndex)
	{
		if (_activeTheme != outfitIndex || _cachedCharacter != charType)
		{
			_cachedCharacter = charType;
			_activeTheme = outfitIndex;
			PlayerInfo.Instance.ThemeSeen(_cachedCharacter, outfitIndex);
			SelectButton(outfitIndex);
		}
	}
}
