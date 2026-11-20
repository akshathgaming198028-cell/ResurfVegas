using System;
using System.Collections.Generic;
using UnityEngine;

public class UIHoverboardThemeShifter : MonoBehaviour
{
	private const int MAX_NUMBER_OF_CUSTOM_THEMES = 3;

	[SerializeField]
	private GameObject _defaultHoverboardThemeButton;

	[SerializeField]
	private GameObject _customHoverboardThemeButton;

	[SerializeField]
	private Transform _selectedMarker;

	private Vector3 _distanceBetweenButtons = Vector3.zero;

	private List<HoverboardThemeButton> _customButtons = new List<HoverboardThemeButton>();

	private Hoverboards.BoardType _cachedBoard;

	private int _cachedThemeIndex;

	private List<HoverboardThemes.HoverboardTheme> _currentBoardThemes = new List<HoverboardThemes.HoverboardTheme>();

	private HoverboardThemes.HoverboardTheme[] _sortedThemes;

	private HoverboardManager.ScreenContext _context;

	public event Action<int> OnClickedTheme;

	private void Awake()
	{
		Vector3 localPosition = _defaultHoverboardThemeButton.transform.localPosition;
		Vector3 localPosition2 = _customHoverboardThemeButton.transform.localPosition;
		_distanceBetweenButtons = localPosition2 - localPosition;
		_customButtons.Add(_customHoverboardThemeButton.GetComponent<HoverboardThemeButton>());
		_selectedMarker.GetComponent<UISprite>().color = GlobalColors.CHARACTERS_CUSTOMIZATION_BASE_SKIN_ICON;
		_defaultHoverboardThemeButton.GetComponent<HoverboardThemeButton>().OnPress += ClickedTheme;
		_customButtons[0].OnPress += ClickedTheme;
		_selectedMarker.transform.position = _defaultHoverboardThemeButton.transform.position;
	}

	public void Init(HoverboardManager.ScreenContext context)
	{
		_context = context;
	}

	private HoverboardThemes.HoverboardTheme[] SortThemes(List<HoverboardThemes.HoverboardTheme> themes)
	{
		HoverboardThemes.HoverboardTheme[] array = new HoverboardThemes.HoverboardTheme[themes.Count];
		List<HoverboardThemes.HoverboardTheme> list = new List<HoverboardThemes.HoverboardTheme>();
		list.AddRange(themes);
		int num = int.MaxValue;
		HoverboardThemes.HoverboardTheme hoverboardTheme = themes[0];
		for (int i = 0; i < themes.Count; i++)
		{
			foreach (HoverboardThemes.HoverboardTheme item in list)
			{
				if ((int)item.uiOrder < num)
				{
					num = (int)item.uiOrder;
					hoverboardTheme = item;
				}
			}
			array[i] = hoverboardTheme;
			list.Remove(hoverboardTheme);
			num = int.MaxValue;
		}
		return array;
	}

	public void MakeCustomThemesAvailable(List<HoverboardThemes.HoverboardTheme> themes)
	{
		base.gameObject.SetActive(true);
		_currentBoardThemes = themes;
		if (themes.Count > 0)
		{
			_sortedThemes = SortThemes(themes);
		}
		foreach (HoverboardThemeButton customButton in _customButtons)
		{
			customButton.gameObject.SetActive(false);
		}
		int num = 0;
		if (themes != null)
		{
			num = ((themes.Count <= 3) ? themes.Count : 3);
		}
		_defaultHoverboardThemeButton.GetComponent<HoverboardThemeButton>().InitButton(string.Empty, GlobalColors.HOVERBOARD_CUSTOMIZATION_BASE_SKIN_FILL, GlobalColors.HOVERBOARD_CUSTOMIZATION_BASE_SKIN_ICON, 0);
		for (int i = 0; i < num; i++)
		{
			if (_customButtons.Count > i)
			{
				HoverboardThemeButton hoverboardThemeButton = _customButtons[i];
				if (_sortedThemes != null && _sortedThemes.Length > i)
				{
					HoverboardThemes.HoverboardTheme hoverboardTheme = _sortedThemes[i];
					setActiveBubles(hoverboardThemeButton, hoverboardTheme, HoverboardManager.Instance.GetCurrentlyShownHoverboard(_context), themes.IndexOf(hoverboardTheme) + 1);
					hoverboardThemeButton.InitButton(hoverboardTheme.iconName, hoverboardTheme.buttonBgColor, hoverboardTheme.buttonIconColor, themes.IndexOf(hoverboardTheme) + 1);
				}
				hoverboardThemeButton.gameObject.SetActive(true);
				continue;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(_customHoverboardThemeButton);
			HoverboardThemeButton component = gameObject.GetComponent<HoverboardThemeButton>();
			if (component != null)
			{
				component.transform.parent = base.gameObject.transform;
				component.transform.localPosition = _customHoverboardThemeButton.transform.localPosition + _distanceBetweenButtons * i;
				component.transform.localScale = Vector3.one;
				component.name = i + 1 + "BoardThemeBtn";
				if (_sortedThemes != null && _sortedThemes.Length > i)
				{
					HoverboardThemes.HoverboardTheme hoverboardTheme2 = _sortedThemes[i];
					setActiveBubles(component, hoverboardTheme2, HoverboardManager.Instance.GetCurrentlyShownHoverboard(_context), themes.IndexOf(hoverboardTheme2) + 1);
					component.InitButton(hoverboardTheme2.iconName, hoverboardTheme2.buttonBgColor, hoverboardTheme2.buttonIconColor, themes.IndexOf(hoverboardTheme2) + 1);
				}
				_customButtons.Add(component);
				component.OnPress += ClickedTheme;
			}
		}
		UpdateThemeStates();
	}

	private void OnDisable()
	{
		foreach (HoverboardThemeButton customButton in _customButtons)
		{
			customButton.gameObject.SetActive(false);
		}
	}

	private void setActiveBubles(HoverboardThemeButton btn, HoverboardThemes.HoverboardTheme ht, Hoverboards.BoardType type, int index)
	{
		bool flag = false;
		flag = ((index == 0) ? (Hoverboards.boardData[type].unlockType == Hoverboards.UnlockType.free && !PlayerInfo.Instance.isHoverboardUnlocked(type)) : (ht.unlockType == Hoverboards.UnlockType.free && !PlayerInfo.Instance.HoverboardCustomizationData.IsThemeOwned(type, HoverboardThemes.GetThemeIdentifier(type, index - 1)) && (_cachedThemeIndex == 0 || _cachedThemeIndex != btn.ThemeIndex)));
		bool enableNewBuble = !PlayerInfo.Instance.HoverboardCustomizationData.HasThemeBeenSeen(type, HoverboardThemes.GetThemeIdentifier(type, index - 1)) && !PlayerInfo.Instance.isFreshInstall && ht.isNewInThisUpdate;
		btn.setActiveBubles(flag, enableNewBuble);
	}

	private void UpdateSelector(int index)
	{
		if (index == 0)
		{
			_selectedMarker.transform.position = _defaultHoverboardThemeButton.transform.position;
		}
		else
		{
			_selectedMarker.transform.position = _customButtons[_currentBoardThemes.IndexOf(_sortedThemes[index - 1])].transform.position;
		}
	}

	public void DisableThemeShifter()
	{
		base.gameObject.SetActive(false);
	}

	public void UpdateThemeStates()
	{
		HoverboardCustomizationData hoverboardCustomizationData = PlayerInfo.Instance.HoverboardCustomizationData;
		for (int i = 0; i < _customButtons.Count; i++)
		{
			HoverboardThemeButton.ThemeState themeState = HoverboardThemeButton.ThemeState.NotOwned;
			themeState = (hoverboardCustomizationData.IsThemeOwned(_cachedBoard, HoverboardThemes.GetThemeIdentifier(_cachedBoard, i)) ? ((!hoverboardCustomizationData.IsThemeActive(_cachedBoard, HoverboardThemes.GetThemeIdentifier(_cachedBoard, i))) ? HoverboardThemeButton.ThemeState.OwnedAndInactive : HoverboardThemeButton.ThemeState.OwnedAndActive) : HoverboardThemeButton.ThemeState.NotOwned);
			_customButtons[i].SetCurrentState(themeState);
		}
	}

	public void ClickedTheme(int index)
	{
		_cachedBoard = HoverboardManager.Instance.GetCurrentlyShownHoverboard(_context);
		_cachedThemeIndex = index;
		UpdateSelector(index);
		if (index != 0)
		{
			PlayerInfo.Instance.HoverboardCustomizationData.SetThemeSeen(_cachedBoard, HoverboardThemes.GetThemeIdentifier(_cachedBoard, index - 1));
		}
		Action<int> onClickedTheme = this.OnClickedTheme;
		if (onClickedTheme != null)
		{
			this.OnClickedTheme(index);
		}
	}

	public void ResetThemeSelection()
	{
		_cachedBoard = HoverboardManager.Instance.GetCurrentlyShownHoverboard(_context);
		UpdateSelector(0);
		UpdateThemeStates();
	}
}
