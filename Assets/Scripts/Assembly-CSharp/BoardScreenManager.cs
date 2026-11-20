public class BoardScreenManager
{
	private static BoardScreenManager _instance;

	public static BoardScreenManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new BoardScreenManager();
			}
			return _instance;
		}
	}

	public bool forceRefreshOnBoardScreen { get; set; }

	public bool forceCenterOnSpecificBoard { get; set; }

	public bool comingFromHoverboardCustomizationPopup { get; set; }

	public bool willResetBoardAnimation { get; set; }

	public bool shouldResetThemeSelection { get; set; }

	public Hoverboards.BoardType currentlyShownBoard { get; set; }

	public int currentlyShownThemeIndex { get; set; }

	private BoardScreenManager()
	{
	}

	public void ChangedDisplayedHoverboard(Hoverboards.BoardType newBoard)
	{
		currentlyShownBoard = newBoard;
		HoverboardManager.Instance.ResetDisplayedCustomSets(newBoard);
	}
}
