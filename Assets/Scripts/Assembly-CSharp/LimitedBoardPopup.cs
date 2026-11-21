using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class LimitedBoardPopup : UIScreen
{
	private const bool ALLOW_SIGNATURE_ANIMATIONS = false;

	[SerializeField]
	private GameObject _modelAnchor;

	[SerializeField]
	private GameObject _centerAnchor;

	[SerializeField]
	private UILabel _nameLabel;

	[SerializeField]
	private UILabel _staticPowerupLabel;

	[SerializeField]
	private UILabel _descriptionLabel;

	[SerializeField]
	private UISprite _powerIcon;

	[SerializeField]
	private UILabel _themeLimitedLabel;

	[SerializeField]
	private GameObject _themePower;

	[SerializeField]
	private UILabel _themePowerLabel;

	[SerializeField]
	private UIWidget _coinBoxAnchor;

	[SerializeField]
	private UIHoverboardThemeShifter _hoverboardThemeShifter;

	[SerializeField]
	private GameObject _hoverboardSelectButtonPrefab;

	private HoverboardSelectButton _hoverboardSelectButton;

	public override void Init()
	{
		base.Init();
		if (UIModelController.Instance.limitedBoardPopupAnchor == null)
		{
			UIModelController.Instance.limitedBoardPopupAnchor = _modelAnchor;
		}
		InitializeSelectButton();
		InitializeCoinbox(_coinBoxAnchor, true, true, true, 3f, -1f);
		_hoverboardThemeShifter.Init(HoverboardManager.ScreenContext.LimitedBoardPopup);
	}

	public override void Show()
	{
		base.Show();
		LimitedBoardPopupManager.Instance.currentlyShownThemeIndex = 0;
		_hoverboardSelectButton.UpdateCurrentThemeIndex();
		_hoverboardThemeShifter.OnClickedTheme += ThemeButtonClicked;
		SetupThemeShifter();
		HoverboardManager.OnThemeActiveStateChanged += OnThemeActiveStateChanged;
		UIScreenController.OnApplicationResumed += HidePopupIfNeeded;
		PlayerInfo instance = PlayerInfo.Instance;
		instance.OnBoardUnlocked = (Action<Hoverboards.BoardType>)Delegate.Combine(instance.OnBoardUnlocked, new Action<Hoverboards.BoardType>(OnBoardUnlocked));
		UIModelController.Instance.ActivateHoverboardModel(UIModelController.ModelScreen.LimitedBoardPopup);
		ShowHoverboardInMenu(true, false);
		UpdateUIElements();
	}

	public override void Hide()
	{
		base.Hide();
		_hoverboardThemeShifter.OnClickedTheme -= ThemeButtonClicked;
		HoverboardManager.OnThemeActiveStateChanged -= OnThemeActiveStateChanged;
		UIScreenController.OnApplicationResumed -= HidePopupIfNeeded;
		UIModelController.Instance.ClearModels();
		LogFlurryEvent();
	}

	public override void GainFocus()
	{
		base.GainFocus();
		UIModelController.Instance.ActivateHoverboardModel(UIModelController.ModelScreen.LimitedBoardPopup);
		ShowHoverboardInMenu(true, false);
	}

	public override void LooseFocus()
	{
		base.LooseFocus();
		UIModelController.Instance.ClearModels();
	}

	private void InitializeSelectButton()
	{
		GameObject gameObject = NGUITools.AddChild(_centerAnchor, _hoverboardSelectButtonPrefab);
		_hoverboardSelectButton = gameObject.GetComponent<HoverboardSelectButton>();
		gameObject.transform.localPosition = new Vector3(62f, -42f, 0f);
		_hoverboardSelectButton.InitButton(HoverboardManager.ScreenContext.LimitedBoardPopup);
	}

	private void UpdateUIElements()
	{
		Hoverboards.BoardType currentBoardType = LimitedBoardPopupManager.Instance.currentBoardType;
		int currentlyShownThemeIndex = LimitedBoardPopupManager.Instance.currentlyShownThemeIndex;
		_nameLabel.text = Strings.Get(Hoverboards.boardData[currentBoardType].name);
		_descriptionLabel.text = Strings.Get(Hoverboards.boardData[currentBoardType].description);
		_themeLimitedLabel.text = Strings.Get(Hoverboards.boardData[currentBoardType].limitedDescription);
		_staticPowerupLabel.text = Strings.Get(StringID.BOARD_SCREEN_SPECIAL_POWER);
		bool flag = !string.IsNullOrEmpty(Strings.Get(Hoverboards.boardData[currentBoardType].limitedDescription));
		bool flag2 = !string.IsNullOrEmpty(Strings.Get(Hoverboards.boardData[currentBoardType].description));
		_powerIcon.enabled = flag2;
		_staticPowerupLabel.enabled = flag2;
		_themeLimitedLabel.enabled = flag;
		if (!flag || flag2)
		{
		}
		if (currentlyShownThemeIndex == 0)
		{
			_themePower.SetActive(false);
			_nameLabel.gameObject.SetActive(true);
		}
		else
		{
			_themePower.SetActive(true);
			_nameLabel.gameObject.SetActive(false);
			_themePowerLabel.text = Strings.Get(HoverboardThemes.hoverboardCustomThemes[currentBoardType][currentlyShownThemeIndex - 1].title);
		}
		UpdateThemeShifter();
		_hoverboardSelectButton.UpdateSticker(currentBoardType, currentlyShownThemeIndex);
	}

	private void SetupThemeShifter()
	{
		_hoverboardThemeShifter.ResetThemeSelection();
		UpdateThemeShifter();
	}

	private void UpdateThemeShifter()
	{
		if (HoverboardThemes.hoverboardCustomThemes.ContainsKey(LimitedBoardPopupManager.Instance.currentBoardType))
		{
			_hoverboardThemeShifter.MakeCustomThemesAvailable(HoverboardThemes.hoverboardCustomThemes[LimitedBoardPopupManager.Instance.currentBoardType]);
		}
		else
		{
			_hoverboardThemeShifter.DisableThemeShifter();
		}
	}

	private void OnBoardUnlocked(Hoverboards.BoardType boardType)
	{
		BoardScreenManager.Instance.forceRefreshOnBoardScreen = true;
	}

	private void OnThemeActiveStateChanged(Hoverboard.BoardAbility boardAbility)
	{
		ShowHoverboardInMenu(true, true, boardAbility);
	}

	private void ThemeButtonClicked(int index)
	{
		Hoverboards.BoardType currentBoardType = LimitedBoardPopupManager.Instance.currentBoardType;
		bool flag = PlayerInfo.Instance.HoverboardCustomizationData.IsThemeOwned(currentBoardType, HoverboardThemes.GetThemeIdentifier(currentBoardType, index - 1));
		if (LimitedBoardPopupManager.Instance.currentlyShownThemeIndex == index)
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
		LimitedBoardPopupManager.Instance.currentlyShownThemeIndex = index;
		_hoverboardSelectButton.UpdateCurrentThemeIndex();
		UpdateDisplayedModelWithTheme(currentBoardType, index);
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
			if (index > 0)
			{
				HoverboardModelPreviewFactory.Instance.ChangeHoverboard(currentBoardType, hoverboardGameObject, characterAnimation, true, true, HoverboardThemes.hoverboardCustomThemes[currentBoardType][index - 1].power);
			}
			else
			{
				HoverboardModelPreviewFactory.Instance.ChangeHoverboard(currentBoardType, hoverboardGameObject, characterAnimation, true, true, Hoverboard.BoardAbility.None);
			}
		}
	}

	private void UpdateDisplayedModelWithTheme(Hoverboards.BoardType boardType, int themeIndex)
	{
		HoverboardCustomizationData hoverboardCustomizationData = PlayerInfo.Instance.HoverboardCustomizationData;
		if (themeIndex != 0)
		{
			bool flag = hoverboardCustomizationData.IsThemeOwned(boardType, HoverboardThemes.GetThemeIdentifier(boardType, themeIndex - 1)) && !hoverboardCustomizationData.IsThemeActive(boardType, HoverboardThemes.GetThemeIdentifier(boardType, themeIndex - 1));
			HoverboardManager.Instance.DisplayCustomSet(themeIndex - 1, !flag);
			for (int i = 0; i < 2; i++)
			{
				if (i != themeIndex - 1)
				{
					HoverboardManager.Instance.DisplayCustomSet(i, hoverboardCustomizationData.IsThemeActive(boardType, HoverboardThemes.GetThemeIdentifier(boardType, i)));
				}
			}
		}
		else
		{
			for (int j = 0; j < 2; j++)
			{
				HoverboardManager.Instance.DisplayCustomSet(j, hoverboardCustomizationData.IsThemeActive(boardType, HoverboardThemes.GetThemeIdentifier(boardType, j)));
			}
		}
		ShowHoverboardInMenu(false, true);
	}

	private void ShowHoverboardInMenu(bool willUpdateAnimation, bool willAllowSignatureAnimation, Hoverboard.BoardAbility ability = Hoverboard.BoardAbility.None)
	{
		UIModelController.Instance.ShowHoverboardMenuModel(LimitedBoardPopupManager.Instance.currentBoardType, willUpdateAnimation, willAllowSignatureAnimation, ability);
		UpdateUIElements();
	}

	private void LogFlurryEvent()
	{
		Hoverboards.BoardType currentBoardType = LimitedBoardPopupManager.Instance.currentBoardType;
		string arg = Strings.Get(Hoverboards.boardData[currentBoardType].name).ToLower();
		bool state = PlayerInfo.Instance.isHoverboardUnlocked(currentBoardType);
		StringBuilder stringBuilder = new StringBuilder(string.Format("Board {0} bought", arg));
		stringBuilder.Append(";");
		StringBuilder stringBuilder2 = new StringBuilder(GetStateString(state));
		stringBuilder2.Append(";");
		List<HoverboardThemes.HoverboardTheme> list = HoverboardThemes.TryGetCustomThemesForBoard(currentBoardType);
		if (list != null)
		{
			for (int i = 0; i < list.Count; i++)
			{
				HoverboardThemes.ThemeIdentifier identifier = list[i].identifier;
				bool state2 = PlayerInfo.Instance.HoverboardCustomizationData.IsThemeOwned(currentBoardType, identifier);
				stringBuilder.AppendFormat("{0}_{1} bought", arg, identifier.ToString());
				stringBuilder.Append(";");
				stringBuilder2.AppendFormat("{0};", GetStateString(state2));
			}
		}
		Flurry.LogEventWithSeveralParameters("Limited board popup closed", stringBuilder.ToString(), stringBuilder2.ToString());
	}

	private string GetStateString(bool state)
	{
		return (!state) ? "no" : "yes";
	}

	private void HidePopupIfNeeded()
	{
		if (!LimitedBoardPopupManager.Instance.IsBoardAvailable())
		{
			UIScreenController.Instance.CloseActivePopups("LimitedBoardPopup");
		}
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	private static void LogError(string msg, UnityEngine.Object context = null)
	{
		Debug.LogError(msg, context);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	private static void LogWarning(string msg, UnityEngine.Object context = null)
	{
		Debug.LogWarning(msg, context);
	}

	[Conditional("ENABLE_BOARD_SELECT_BUTTON_LOGS")]
	private static void Log(string msg, UnityEngine.Object context = null)
	{
		Debug.Log(msg, context);
	}
}
