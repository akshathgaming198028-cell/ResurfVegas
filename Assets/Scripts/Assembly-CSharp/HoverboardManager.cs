using System;

public class HoverboardManager
{
	public enum ScreenContext
	{
		BoardScreen = 0,
		LimitedBoardPopup = 1
	}

	private HoverboardCustomizationData _customizationData;

	private bool[] _displayCustomSets;

	private static HoverboardManager instance;

	public static HoverboardManager Instance
	{
		get
		{
			return instance ?? (instance = new HoverboardManager());
		}
	}

	public static event Action OnBoardSelected;

	public static event Action<Hoverboard.BoardAbility> OnThemeActiveStateChanged;

	public HoverboardManager()
	{
		_customizationData = PlayerInfo.Instance.HoverboardCustomizationData;
		_displayCustomSets = new bool[2];
		ResetDisplayedCustomSets(PlayerInfo.Instance.currentHoverboard);
	}

	public Hoverboards.BoardType GetCurrentlyShownHoverboard(ScreenContext context)
	{
		switch (context)
		{
		case ScreenContext.BoardScreen:
			return BoardScreenManager.Instance.currentlyShownBoard;
		case ScreenContext.LimitedBoardPopup:
			return LimitedBoardPopupManager.Instance.currentBoardType;
		default:
			return Hoverboards.BoardType.normal;
		}
	}

	public int GetCurrentlyShownHoverboardTheme(ScreenContext context)
	{
		switch (context)
		{
		case ScreenContext.BoardScreen:
			return BoardScreenManager.Instance.currentlyShownThemeIndex;
		case ScreenContext.LimitedBoardPopup:
			return LimitedBoardPopupManager.Instance.currentlyShownThemeIndex;
		default:
			return 0;
		}
	}

	public void SelectCurrentlyShownBoard(ScreenContext context)
	{
		UIModelController.Instance.SelectCurrentlyShownBoard(GetCurrentlyShownHoverboard(context));
		Action onBoardSelected = HoverboardManager.OnBoardSelected;
		if (onBoardSelected != null)
		{
			onBoardSelected();
		}
	}

	public void ChangeThemeActiveState(ScreenContext context, Hoverboard.BoardAbility boardAbility = Hoverboard.BoardAbility.None)
	{
		Hoverboards.BoardType currentlyShownHoverboard = GetCurrentlyShownHoverboard(context);
		BoardScreenManager.Instance.ChangedDisplayedHoverboard(currentlyShownHoverboard);
		Action<Hoverboard.BoardAbility> onThemeActiveStateChanged = HoverboardManager.OnThemeActiveStateChanged;
		if (onThemeActiveStateChanged != null)
		{
			onThemeActiveStateChanged(boardAbility);
		}
	}

	public void ResetDisplayedCustomSets(Hoverboards.BoardType newBoard)
	{
		if (HoverboardThemes.hoverboardCustomThemes.ContainsKey(newBoard))
		{
			for (int i = 0; i < 2; i++)
			{
				_displayCustomSets[i] = IsThemeActiveForUIIndex(newBoard, i);
			}
		}
		else
		{
			for (int j = 0; j < 2; j++)
			{
				_displayCustomSets[j] = false;
			}
		}
	}

	private bool IsThemeActiveForUIIndex(Hoverboards.BoardType board, int uiIndex)
	{
		if (HoverboardThemes.hoverboardCustomThemes.ContainsKey(board))
		{
			foreach (HoverboardThemes.HoverboardTheme item in HoverboardThemes.hoverboardCustomThemes[board])
			{
				if (item.uiOrder == (HoverboardThemes.UIOrder)uiIndex)
				{
					return IsCustomSetEquippedForBoard(board, uiIndex);
				}
			}
			return false;
		}
		return false;
	}

	public bool IsCustomSetEquippedForBoard(Hoverboards.BoardType boardType, int themeIndex)
	{
		if (HoverboardThemes.hoverboardCustomThemes.ContainsKey(boardType))
		{
			foreach (HoverboardThemes.HoverboardTheme item in HoverboardThemes.hoverboardCustomThemes[boardType])
			{
				if (themeIndex == (int)item.uiOrder)
				{
					return _customizationData.IsThemeActive(boardType, item.identifier);
				}
			}
			return false;
		}
		return false;
	}

	public bool ShouldCustomSetBeDisplayed(int themeIndex)
	{
		return _displayCustomSets[themeIndex];
	}

	public void DisplayCustomSet(int themeIndex, bool display)
	{
		_displayCustomSets[themeIndex] = display;
	}

	public Hoverboards.BoardType? GetEventBoardType()
	{
		bool found = false;
		Hoverboards.BoardType? boardType = null;
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
		if (activeEventManager != null)
		{
			SSEventType eventType = activeEventManager.GetEventType();
			if (eventType == SSEventType.ExpiredLimitedBoard || eventType == SSEventType.SpecialPowerBoard)
			{
				boardType = activeEventManager.GetEventValue<Hoverboards.BoardType>(out found);
			}
		}
		return (!found) ? ((Hoverboards.BoardType?)null) : boardType;
	}
}
