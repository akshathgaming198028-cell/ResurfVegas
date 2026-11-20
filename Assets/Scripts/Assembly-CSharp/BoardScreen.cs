using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class BoardScreen : UIScreen
{
	private class BoardState
	{
		public Quaternion defaultRotation;

		public GameObject boardRoot;

		public Transform boardTransform;
	}

	private const float DEFAULT_SCALE_FACTOR = 7f;

	private const float FOCUSED_SCALE_FACTOR = 10f;

	[SerializeField]
	private GameObject scrollAnchor;

	[SerializeField]
	private UIGrid scrollGrid;

	[SerializeField]
	private GameObject overlayParent;

	[SerializeField]
	private UIPanel scrollPanel;

	[SerializeField]
	private GameObject scroll2dElements;

	[SerializeField]
	private GameObject _centerAnchor;

	[SerializeField]
	private GameObject dummyObject;

	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private UILabel staticPowerupLabel;

	[SerializeField]
	private UILabel descriptionLabel;

	[SerializeField]
	private UISprite powerIcon;

	[SerializeField]
	private UILabel themeLimitedLabel;

	[SerializeField]
	private GameObject eventRibbon;

	[SerializeField]
	private UILabel[] ribbonLabels;

	[SerializeField]
	private UIHoverboardThemeShifter _hoverboardThemeShifter;

	[SerializeField]
	private GameObject _hoverboardSelectButtonPrefab;

	[SerializeField]
	private AudioClip selectSound;

	[SerializeField]
	private GameObject themePower;

	[SerializeField]
	private UILabel themePowerLabel;

	private bool _hasInited;

	private CenterOnChild _centerer;

	private HoverboardSelectButton _hoverboardSelectButton;

	private bool _refreshingThemeInScrollList;

	private SSEventType _cachedActiveEventType;

	private bool _hasSetupScroll;

	private float _cellWidth;

	private List<KeyValuePair<Hoverboards.BoardType, Hoverboards.Board>> boardList;

	private Quaternion[] _previousBoardQuaternions;

	private bool hasEnabledScrollElements;

	private float _cachedScrollPosition;

	private List<BoardState> scrollBoards = new List<BoardState>();

	private List<HoverboardOverlayHelper> boardHelpers = new List<HoverboardOverlayHelper>();

	private List<OverlayIndex> boardIndices = new List<OverlayIndex>();

	private bool _inappOverlayActive;

	private bool _popupActive;

	private bool _modelsEnabled;

	public override void Init()
	{
		base.Init();
		InitializeSelectButton();
		_cellWidth = scrollGrid.cellWidth;
		_centerer = scrollGrid.GetComponent<CenterOnChild>();
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
		_cachedActiveEventType = ((activeEventManager != null) ? activeEventManager.GetEventType() : SSEventType.None);
		_hoverboardThemeShifter.Init(HoverboardManager.ScreenContext.BoardScreen);
		_hoverboardThemeShifter.OnClickedTheme += ClickedHoverboardTheme;
		InitBoards();
		_hoverboardSelectButton.SetStickerColors(HoverboardSelectButton.StickerColors.Discount);
		_hasInited = true;
		InitializeCoinbox(null, true, true, true);
	}

	public override void Show()
	{
		base.Show();
		UIScreenController.OnApplicationResumed += RefreshBoards;
		UIScreenController.OnApplicationResumed += RefreshEventBoardAndUI;
		SSEventsManagerController.OnSSEventStartedHandler += RefreshEventBoardAndUI;
		SSEventsManagerController.OnSSEventFinishedHandler += RefreshEventBoardAndUI;
		HoverboardManager.OnBoardSelected += UpdateUIElements;
		HoverboardManager.OnThemeActiveStateChanged += OnThemeActiveStateChanged;
		RefreshBoards();
		RefreshEventBoardAndUI();
	}

	public override void Hide()
	{
		base.Hide();
		UIScreenController.OnApplicationResumed -= RefreshBoards;
		UIScreenController.OnApplicationResumed -= RefreshEventBoardAndUI;
		SSEventsManagerController.OnSSEventStartedHandler -= RefreshEventBoardAndUI;
		SSEventsManagerController.OnSSEventFinishedHandler -= RefreshEventBoardAndUI;
		HoverboardManager.OnBoardSelected -= UpdateUIElements;
		HoverboardManager.OnThemeActiveStateChanged -= OnThemeActiveStateChanged;
		UIModelController.Instance.ClearModels();
	}

	private void InitializeSelectButton()
	{
		GameObject gameObject = NGUITools.AddChild(_centerAnchor, _hoverboardSelectButtonPrefab);
		_hoverboardSelectButton = gameObject.GetComponent<HoverboardSelectButton>();
		gameObject.transform.localPosition = new Vector3(62f, 14f, 0f);
		_hoverboardSelectButton.InitButton(HoverboardManager.ScreenContext.BoardScreen);
	}

	private void RefreshEventBoardAndUI()
	{
		SSEventsManagerController.instance.RestoreBoardAfterEventIfNeeded();
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
		ActivateEventRibbon(activeEventManager != null && (activeEventManager.GetEventType() == SSEventType.SpecialPowerBoard || activeEventManager.GetEventType() == SSEventType.ExpiredLimitedBoard));
		UIModelController.Instance.ActivateHoverboardModel();
		ShowHoverboardInMenu(true, false);
	}

	private void OnThemeActiveStateChanged(Hoverboard.BoardAbility boardAbility)
	{
		ShowHoverboardInMenu(true, true, boardAbility);
		ShowHoverboardInScrollList();
	}

	public void ShowHoverboardInMenu(bool willUpdateAnimation, bool willAllowSignatureAnimation, Hoverboard.BoardAbility ability = Hoverboard.BoardAbility.None)
	{
		UIModelController.Instance.ShowHoverboardMenuModel(BoardScreenManager.Instance.currentlyShownBoard, willUpdateAnimation, willAllowSignatureAnimation, ability);
		UpdateUIElements();
	}

	public void ShowHoverboardInScrollList()
	{
		_refreshingThemeInScrollList = true;
		RefreshBoards();
		_refreshingThemeInScrollList = false;
	}

	private void UpdateUIElements()
	{
		if (BoardScreenManager.Instance.shouldResetThemeSelection)
		{
			BoardScreenManager.Instance.shouldResetThemeSelection = false;
			BoardScreenManager.Instance.currentlyShownThemeIndex = 0;
			_hoverboardThemeShifter.ResetThemeSelection();
		}
		if (BoardScreenManager.Instance.currentlyShownThemeIndex != 0)
		{
			List<HoverboardThemes.HoverboardTheme> list = HoverboardThemes.TryGetCustomThemesForBoard(BoardScreenManager.Instance.currentlyShownBoard);
			if (list == null || BoardScreenManager.Instance.currentlyShownThemeIndex > list.Count)
			{
				BoardScreenManager.Instance.currentlyShownThemeIndex = 0;
				_hoverboardThemeShifter.ResetThemeSelection();
			}
		}
		Hoverboards.Board board = Hoverboards.boardData[BoardScreenManager.Instance.currentlyShownBoard];
		nameLabel.text = Strings.Get(board.name);
		descriptionLabel.text = Strings.Get(board.description);
		themeLimitedLabel.text = Strings.Get(board.limitedDescription);
		staticPowerupLabel.text = Strings.Get(StringID.BOARD_SCREEN_SPECIAL_POWER);
		bool flag = !string.IsNullOrEmpty(Strings.Get(board.limitedDescription));
		bool flag2 = !string.IsNullOrEmpty(Strings.Get(board.description));
		powerIcon.enabled = flag2;
		staticPowerupLabel.enabled = flag2;
		themeLimitedLabel.enabled = flag;
		if (!flag || flag2)
		{
		}
		if (BoardScreenManager.Instance.currentlyShownThemeIndex == 0)
		{
			themePower.SetActive(false);
			nameLabel.gameObject.SetActive(true);
		}
		else
		{
			themePower.SetActive(true);
			nameLabel.gameObject.SetActive(false);
			themePowerLabel.text = Strings.Get(HoverboardThemes.hoverboardCustomThemes[BoardScreenManager.Instance.currentlyShownBoard][BoardScreenManager.Instance.currentlyShownThemeIndex - 1].title);
		}
		_hoverboardSelectButton.UpdateSticker(BoardScreenManager.Instance.currentlyShownBoard, BoardScreenManager.Instance.currentlyShownThemeIndex);
		SetupThemes();
	}

	private void ActivateEventRibbon(bool active)
	{
		eventRibbon.SetActive(active);
		if (!active)
		{
			return;
		}
		string firstLine = string.Empty;
		string secondLine = string.Empty;
		string thirdLine = string.Empty;
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
		if (activeEventManager != null)
		{
			SSEventType eventType = activeEventManager.GetEventType();
			if (eventType == SSEventType.ExpiredLimitedBoard || eventType == SSEventType.SpecialPowerBoard)
			{
				activeEventManager.GetRibbonDescription(out firstLine, out secondLine, out thirdLine);
			}
		}
		if (!string.IsNullOrEmpty(firstLine) && !string.IsNullOrEmpty(secondLine) && !string.IsNullOrEmpty(thirdLine))
		{
			ribbonLabels[0].text = firstLine;
			ribbonLabels[1].text = secondLine;
			ribbonLabels[2].text = thirdLine;
			Vector3 localPosition = ribbonLabels[2].transform.localPosition;
			float x = ribbonLabels[2].localSize.x;
			float num = 136f;
			if (x > num)
			{
				float num2 = Mathf.Abs(x - num);
				localPosition.x = num2 * 0.5f;
			}
			else
			{
				localPosition.x = 0f;
			}
			ribbonLabels[2].transform.localPosition = localPosition;
		}
	}

	private void InitBoards()
	{
		boardList = new List<KeyValuePair<Hoverboards.BoardType, Hoverboards.Board>>();
		Hoverboards.BoardType? eventBoardType = HoverboardManager.Instance.GetEventBoardType();
		foreach (Hoverboards.BoardType item in Hoverboards.boardOrder)
		{
			Hoverboards.Board value = Hoverboards.boardData[item];
			if (PlayerInfo.Instance.isHoverboardUnlocked(item) || PlayerInfo.Instance.isBoardActive(item) || (eventBoardType.HasValue && eventBoardType.Value == item))
			{
				boardList.Add(new KeyValuePair<Hoverboards.BoardType, Hoverboards.Board>(item, value));
			}
		}
		int num = 0;
		foreach (KeyValuePair<Hoverboards.BoardType, Hoverboards.Board> board in boardList)
		{
			BoardState boardState = new BoardState();
			boardState.boardRoot = HoverboardModelPreviewFactory.Instance.GetHoverboardModelPreview(board.Value.boardModelName, board.Key);
			if (!(boardState.boardRoot == null))
			{
				boardState.boardRoot.name = string.Format("{0:000}{1}", num, board.Value.name);
				boardState.boardTransform = boardState.boardRoot.transform.GetChild(0);
				boardState.defaultRotation = HoverboardModelPreviewFactory.Instance.GetHoverboardDefaultRotation(board.Value.boardModelName);
				scrollBoards.Add(boardState);
				Transform transform = boardState.boardRoot.transform;
				transform.parent = scrollAnchor.transform;
				transform.localPosition = new Vector3((float)num * _cellWidth, 0f, 50f);
				transform.localScale = Vector3.one * 7f;
				transform.localEulerAngles = new Vector3(53f, 183f, 360f);
				GameObject gameObject = NGUITools.AddChild(overlayParent, scroll2dElements);
				gameObject.name = string.Format("{0:000}{1}", num, board.Value.name);
				boardHelpers.Add(gameObject.GetComponent<HoverboardOverlayHelper>());
				boardHelpers[num].Init(num, board.Key, transform, _hoverboardSelectButton);
				GameObject gameObject2 = NGUITools.AddChild(scrollGrid.gameObject, dummyObject);
				boardIndices.Add(gameObject2.AddComponent<OverlayIndex>());
				boardIndices[num].index = num;
				gameObject2.name = string.Format("{0:000}{1}", num, board.Key.ToString());
				num++;
			}
		}
		Utility.SetLayerRecursively(scrollAnchor.transform, 29);
		Utility.SetLayerRecursively(overlayParent.transform, 26);
		scrollGrid.SendMessage("Start");
		if (!_refreshingThemeInScrollList)
		{
			hasEnabledScrollElements = false;
			StartCoroutine(DelayedInitCenterBoard());
			UIModelController.Instance.ActivateHoverboardModel();
			Hoverboards.BoardType currentlyShownBoard = BoardTypeToCenterOn();
			BoardScreenManager.Instance.currentlyShownBoard = currentlyShownBoard;
			ToggleScrollElements(false);
			return;
		}
		Hoverboards.BoardType currentlyShownBoard2 = BoardScreenManager.Instance.currentlyShownBoard;
		OverlayIndex overlayIndex = ElementToCenterOn(boardIndices, currentlyShownBoard2);
		_centerer.CenterOnTransform(overlayIndex.transform, true);
		BoardScreenManager.Instance.ChangedDisplayedHoverboard(currentlyShownBoard2);
		for (int i = 0; i < scrollBoards.Count; i++)
		{
			scrollBoards[i].boardTransform.rotation = _previousBoardQuaternions[i];
		}
		ToggleScrollElements(true);
		ScaleScrollModels(Mathf.Abs(scrollPanel.transform.localPosition.x));
	}

	private IEnumerator DelayedInitCenterBoard()
	{
		yield return new WaitForEndOfFrame();
		Hoverboards.BoardType relevantBoard = BoardTypeToCenterOn();
		OverlayIndex elementOverlayIndex = ElementToCenterOn(boardIndices, relevantBoard);
		_centerer.CenterOnTransform(elementOverlayIndex.transform, true);
		BoardScreenManager.Instance.ChangedDisplayedHoverboard(relevantBoard);
	}

	private void ToggleScrollElements(bool enable)
	{
		scrollAnchor.SetActive(enable);
		overlayParent.SetActive(enable);
	}

	private void ClickedHoverboardTheme(int index)
	{
		bool flag = PlayerInfo.Instance.HoverboardCustomizationData.IsThemeOwned(BoardScreenManager.Instance.currentlyShownBoard, HoverboardThemes.GetThemeIdentifier(BoardScreenManager.Instance.currentlyShownBoard, index - 1));
		if (BoardScreenManager.Instance.currentlyShownThemeIndex == index)
		{
			if (flag)
			{
				_hoverboardSelectButton.OnClick();
			}
			else if (index != 0)
			{
				return;
			}
		}
		BoardScreenManager.Instance.currentlyShownThemeIndex = index;
		_hoverboardSelectButton.UpdateCurrentThemeIndex();
		UpdateDisplayedModelWithTheme(BoardScreenManager.Instance.currentlyShownBoard, BoardScreenManager.Instance.currentlyShownThemeIndex);
		if (flag)
		{
			return;
		}
		CharacterModel activeCharacterModel = UIModelController.Instance.GetActiveCharacterModel();
		if (!(activeCharacterModel != null))
		{
			return;
		}
		GameObject hoverboardGameObject = activeCharacterModel.GetHoverboardGameObject();
		Animation characterAnimation = activeCharacterModel.CharacterAnimation;
		if (hoverboardGameObject != null && characterAnimation != null)
		{
			if (BoardScreenManager.Instance.currentlyShownThemeIndex > 0)
			{
				HoverboardModelPreviewFactory.Instance.ChangeHoverboard(BoardScreenManager.Instance.currentlyShownBoard, hoverboardGameObject, characterAnimation, true, true, HoverboardThemes.hoverboardCustomThemes[BoardScreenManager.Instance.currentlyShownBoard][BoardScreenManager.Instance.currentlyShownThemeIndex - 1].power);
			}
			else
			{
				HoverboardModelPreviewFactory.Instance.ChangeHoverboard(BoardScreenManager.Instance.currentlyShownBoard, hoverboardGameObject, characterAnimation, true, true, Hoverboard.BoardAbility.None);
			}
		}
	}

	private void UpdateDisplayedModelWithTheme(Hoverboards.BoardType boardType, int themeIndex)
	{
		HoverboardManager instance = HoverboardManager.Instance;
		HoverboardCustomizationData hoverboardCustomizationData = PlayerInfo.Instance.HoverboardCustomizationData;
		if (themeIndex != 0)
		{
			bool flag = hoverboardCustomizationData.IsThemeOwned(boardType, HoverboardThemes.GetThemeIdentifier(boardType, themeIndex - 1)) && !hoverboardCustomizationData.IsThemeActive(boardType, HoverboardThemes.GetThemeIdentifier(boardType, themeIndex - 1));
			instance.DisplayCustomSet(themeIndex - 1, !flag);
			for (int i = 0; i < 2; i++)
			{
				if (i != themeIndex - 1)
				{
					instance.DisplayCustomSet(i, hoverboardCustomizationData.IsThemeActive(boardType, HoverboardThemes.GetThemeIdentifier(boardType, i)));
				}
			}
		}
		else
		{
			for (int j = 0; j < 2; j++)
			{
				instance.DisplayCustomSet(j, hoverboardCustomizationData.IsThemeActive(boardType, HoverboardThemes.GetThemeIdentifier(boardType, j)));
			}
		}
		ShowHoverboardInMenu(false, true);
	}

	private void RefreshBoards()
	{
		boardList = new List<KeyValuePair<Hoverboards.BoardType, Hoverboards.Board>>();
		bool flag = false;
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
		SSEventType sSEventType = ((activeEventManager != null) ? activeEventManager.GetEventType() : SSEventType.None);
		if (_cachedActiveEventType != sSEventType)
		{
			_cachedActiveEventType = sSEventType;
			flag = true;
		}
		if (BoardScreenManager.Instance.forceRefreshOnBoardScreen)
		{
			BoardScreenManager.Instance.forceRefreshOnBoardScreen = false;
			flag = true;
		}
		if (_refreshingThemeInScrollList)
		{
			flag = true;
		}
		Hoverboards.BoardType? eventBoardType = HoverboardManager.Instance.GetEventBoardType();
		foreach (Hoverboards.BoardType item in Hoverboards.boardOrder)
		{
			Hoverboards.Board value = Hoverboards.boardData[item];
			if (PlayerInfo.Instance.isHoverboardUnlocked(item) || PlayerInfo.Instance.isBoardActive(item) || (eventBoardType.HasValue && eventBoardType.Value == item))
			{
				boardList.Add(new KeyValuePair<Hoverboards.BoardType, Hoverboards.Board>(item, value));
			}
		}
		if (!flag)
		{
			for (int i = 0; i < boardHelpers.Count; i++)
			{
				if (boardList[i].Key != boardHelpers[i].GetBoardType())
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			_previousBoardQuaternions = new Quaternion[scrollBoards.Count];
			for (int j = 0; j < scrollBoards.Count; j++)
			{
				_previousBoardQuaternions[j] = scrollBoards[j].boardTransform.rotation;
			}
			foreach (BoardState scrollBoard in scrollBoards)
			{
				Object.Destroy(scrollBoard.boardRoot);
			}
			scrollBoards.Clear();
			foreach (HoverboardOverlayHelper boardHelper in boardHelpers)
			{
				Object.Destroy(boardHelper.gameObject);
			}
			boardHelpers.Clear();
			foreach (OverlayIndex boardIndex in boardIndices)
			{
				Object.Destroy(boardIndex.gameObject);
			}
			boardIndices.Clear();
			foreach (Transform item2 in scrollGrid.transform)
			{
				item2.gameObject.SetActive(false);
				Object.Destroy(item2);
			}
			overlayParent.transform.localPosition = Vector3.zero;
			scrollAnchor.transform.localPosition = new Vector3(0f, scrollAnchor.transform.localPosition.y, scrollAnchor.transform.localPosition.z);
			scrollPanel.cachedTransform.localPosition = Vector3.zero;
			scrollPanel.clipOffset = new Vector2(0f, 0f);
			_centerer.ClearCenterObject();
			SpringPanel component = scrollPanel.GetComponent<SpringPanel>();
			if (component != null)
			{
				Object.Destroy(component);
			}
			InitBoards();
		}
		else if (BoardScreenManager.Instance.forceCenterOnSpecificBoard)
		{
			BoardScreenManager.Instance.forceCenterOnSpecificBoard = false;
			Hoverboards.BoardType boardType = BoardTypeToCenterOn();
			OverlayIndex overlayIndex = ElementToCenterOn(boardIndices, boardType);
			_centerer.CenterOnTransform(overlayIndex.transform);
			UIModelController.Instance.ActivateHoverboardModel();
			BoardScreenManager.Instance.ChangedDisplayedHoverboard(boardType);
		}
		BoardScreenManager.Instance.comingFromHoverboardCustomizationPopup = false;
	}

	private void SetupThemes()
	{
		if (HoverboardThemes.hoverboardCustomThemes.ContainsKey(BoardScreenManager.Instance.currentlyShownBoard))
		{
			_hoverboardThemeShifter.MakeCustomThemesAvailable(HoverboardThemes.hoverboardCustomThemes[BoardScreenManager.Instance.currentlyShownBoard]);
		}
		else
		{
			_hoverboardThemeShifter.DisableThemeShifter();
		}
	}

	public void ScrollClicked(Vector2 pos)
	{
		if (_centerer.CenterOnClosestChildAtPosition(pos) && PlayerInfo.Instance.isHoverboardUnlocked(BoardScreenManager.Instance.currentlyShownBoard))
		{
			HoverboardManager.Instance.SelectCurrentlyShownBoard(HoverboardManager.ScreenContext.BoardScreen);
			NGUITools.PlaySound(selectSound);
		}
	}

	private Hoverboards.BoardType BoardTypeToCenterOn()
	{
		Hoverboards.BoardType? eventBoardType = HoverboardManager.Instance.GetEventBoardType();
		if (BoardScreenManager.Instance.comingFromHoverboardCustomizationPopup)
		{
			return Hoverboards.BoardType.theoriginal;
		}
		if (eventBoardType.HasValue)
		{
			return eventBoardType.Value;
		}
		SSEventsManagerController.instance.RestoreBoardAfterEventIfNeeded();
		return PlayerInfo.Instance.currentHoverboard;
	}

	private T ElementToCenterOn<T>(List<T> elements, Hoverboards.BoardType board)
	{
		T result = default(T);
		int num = 0;
		using (List<KeyValuePair<Hoverboards.BoardType, Hoverboards.Board>>.Enumerator enumerator = boardList.GetEnumerator())
		{
			while (enumerator.MoveNext() && board != enumerator.Current.Key)
			{
				num++;
			}
		}
		if (num < elements.Count)
		{
			result = elements[num];
		}
		return result;
	}

	private void ScaleScrollModels(float xCenter)
	{
		for (int i = 0; i < scrollBoards.Count; i++)
		{
			float num = Mathf.Abs(xCenter - (float)i * _cellWidth);
			float num2 = 1.5f * _cellWidth;
			float num3 = Mathf.SmoothStep(10f, 7f, num / num2);
			boardHelpers[i].currentDistFromCenter = num / num2;
			scrollBoards[i].boardRoot.transform.localScale = Vector3.one * num3;
		}
	}

	private void Update()
	{
		if (!_hasInited || boardList == null)
		{
			return;
		}
		if (_centerer.centeredObject != null)
		{
			int index = _centerer.centeredObject.GetComponent<OverlayIndex>().index;
			Transform boardTransform = scrollBoards[index].boardTransform;
			boardTransform.RotateAround(boardTransform.right, 0f - Time.deltaTime);
			Hoverboards.BoardType boardType = boardHelpers[index].GetBoardType();
			if (!_hasSetupScroll)
			{
				BoardScreenManager.Instance.currentlyShownBoard = boardType;
				_hasSetupScroll = true;
			}
			else if (BoardScreenManager.Instance.currentlyShownBoard != boardType || BoardScreenManager.Instance.willResetBoardAnimation)
			{
				if (!BoardScreenManager.Instance.willResetBoardAnimation)
				{
					BoardScreenManager.Instance.currentlyShownThemeIndex = 0;
					_hoverboardSelectButton.UpdateCurrentThemeIndex();
				}
				if (BoardScreenManager.Instance.currentlyShownBoard != boardType)
				{
					BoardScreenManager.Instance.ChangedDisplayedHoverboard(boardType);
				}
				bool willAllowSignatureAnimation = !BoardScreenManager.Instance.willResetBoardAnimation;
				ShowHoverboardInMenu(true, willAllowSignatureAnimation);
				BoardScreenManager.Instance.willResetBoardAnimation = false;
				boardHelpers[index].SelectedInMenu();
				foreach (HoverboardOverlayHelper boardHelper in boardHelpers)
				{
					if (boardHelper.GetIndex() != index)
					{
						boardHelper.RefreshBubbles(false);
					}
				}
			}
			index = ((!(_centerer.centeredObject != null)) ? (-1) : _centerer.centeredObject.GetComponent<OverlayIndex>().index);
			for (int i = 0; i < scrollBoards.Count; i++)
			{
				if (i != index)
				{
					BoardState boardState = scrollBoards[i];
					Quaternion b = boardState.boardTransform.parent.rotation * boardState.defaultRotation;
					boardState.boardTransform.rotation = Quaternion.Lerp(boardState.boardTransform.rotation, b, Time.deltaTime);
				}
			}
		}
		float num = Mathf.Abs(scrollPanel.transform.localPosition.x);
		if (num != _cachedScrollPosition)
		{
			ScaleScrollModels(num);
			_cachedScrollPosition = num;
		}
		if (!hasEnabledScrollElements)
		{
			ToggleScrollElements(true);
			hasEnabledScrollElements = true;
		}
		if (UIScreenController.Instance.isShowingPopup)
		{
			if (!_popupActive || !_inappOverlayActive)
			{
				_popupActive = true;
			}
		}
		else if (_popupActive && !_inappOverlayActive)
		{
			_popupActive = false;
		}
		if (UIScreenController.Instance.inAppPurchaseOverlay.activeInHierarchy)
		{
			if (!_inappOverlayActive)
			{
				_inappOverlayActive = true;
			}
		}
		else if (_inappOverlayActive)
		{
			_inappOverlayActive = false;
		}
		if (_inappOverlayActive || _popupActive)
		{
			if (_modelsEnabled)
			{
				scrollAnchor.SetActive(false);
				overlayParent.SetActive(false);
				_modelsEnabled = false;
			}
		}
		else if (!_modelsEnabled)
		{
			scrollAnchor.SetActive(true);
			overlayParent.SetActive(true);
			_modelsEnabled = true;
		}
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	private static void LogError(string msg, Object context = null)
	{
		Debug.LogError(msg, context);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	private static void LogWarning(string msg, Object context = null)
	{
		Debug.LogWarning(msg, context);
	}
}
