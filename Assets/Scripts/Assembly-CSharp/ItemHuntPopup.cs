using System;
using System.Collections;
using UnityEngine;

public class ItemHuntPopup : UIScreen
{
	private const float SHORT_OUTLINE_Y = 373f;

	private const float SHORT_FILL_Y = 360f;

	[SerializeField]
	private UIAnchor popupAnchor;

	[SerializeField]
	private UILabel mainSubtitle;

	[SerializeField]
	private UILabel mainTitle;

	[SerializeField]
	private GameObject periodPrefab;

	[SerializeField]
	private UISprite bgOutline;

	[SerializeField]
	private UISprite bgFill;

	[SerializeField]
	private GameObject bubbleDoll;

	[SerializeField]
	private UISprite bubbleSprite;

	[SerializeField]
	private UISprite bubbleSpriteShadow;

	[SerializeField]
	private UILabel bottomSubtitle;

	[SerializeField]
	private UILabel bottomTitle;

	[SerializeField]
	private GameObject bottomtext;

	[SerializeField]
	private GameObject presentPrefab_green;

	[SerializeField]
	private GameObject presentPrefab_blue;

	[SerializeField]
	private GameObject presentPrefab_red;

	[SerializeField]
	private PrizeIconPrefabReferences iconReferences;

	private GameObject huntIcon;

	private ItemHuntPopupCell currentActiveCell;

	private UIModelController uimodelController;

	private float FIRST_CELL_STARTING_Y = 145f;

	private float DEFAULT_CELL_HEIGHT = 73f;

	private float ACTIVE_CELL_HEIGHT = 113f;

	private float GAP_CELLS = 7f;

	private float _currentHeight;

	private int _originalBGOutlineScale;

	private int _originalBGFillScale;

	private int _shortBGOutlineScale;

	private int _shortBGFillScale;

	private float THREE_PERIODS_ANCHOR_OFFSET = -0.065f;

	private Vector3 _bottomTextOriginalPosition = Vector3.zero;

	private Vector3 _bottomTextShortPosition = new Vector3(0f, 87f, 0f);

	private WeeklyHuntManager.WeeklyHuntPeriodData prizeOne;

	private WeeklyHuntManager.WeeklyHuntPeriodData prizeTwo;

	private WeeklyHuntManager.WeeklyHuntPeriodData prizeThree;

	private WeeklyHuntManager.WeeklyHuntPeriodData prizeFour;

	private bool _instantiatedPeriodCells;

	private ItemHuntPopupCell periodOne;

	private ItemHuntPopupCell periodTwo;

	private ItemHuntPopupCell periodThree;

	private ItemHuntPopupCell periodFour;

	private DateTime endDate = DateTime.MinValue;

	private void Awake()
	{
		_originalBGOutlineScale = bgOutline.height;
		_originalBGFillScale = bgFill.height;
		_shortBGOutlineScale = (int)Mathf.Round(373f);
		_shortBGFillScale = (int)Mathf.Round(360f);
	}

	public override void Show()
	{
		base.Show();
		uimodelController = UIModelController.Instance;
		_currentHeight = FIRST_CELL_STARTING_Y;
		PlaceHuntIcon();
		HandleData();
	}

	public override void Hide()
	{
		base.Hide();
		huntIcon.SetActive(false);
		periodOne.HidePrizeIcon();
		periodTwo.HidePrizeIcon();
		periodThree.HidePrizeIcon();
		if ((bool)periodFour)
		{
			periodFour.HidePrizeIcon();
		}
		ResetCell(periodOne);
		ResetCell(periodTwo);
		ResetCell(periodThree);
		if ((bool)periodFour)
		{
			ResetCell(periodFour);
		}
		currentActiveCell = null;
	}

	private void ResetCell(ItemHuntPopupCell cell)
	{
		if (!cell)
		{
			Debug.LogError("Attempted to reset a null cell", base.gameObject);
			return;
		}
		cell.activeLabels.SetActive(false);
		cell.activeProgressSlider.SetActive(false);
		cell.inactiveLabels.SetActive(true);
		cell.stickerGroup.SetActive(false);
		cell.inactiveProgressGroup.SetActive(true);
		if (cell == currentActiveCell)
		{
			cell.cellBackground.height = (int)Mathf.Round(DEFAULT_CELL_HEIGHT);
			Vector3 localPosition = cell.iconsGroup.transform.localPosition;
			localPosition.y += 7f;
			cell.iconsGroup.transform.localPosition = localPosition;
		}
	}

	private void PlaceHuntIcon()
	{
		if (huntIcon != null)
		{
			huntIcon.SetActive(true);
		}
		else
		{
			huntIcon = NGUITools.AddChild(uimodelController.TutorialPopupAnchor, bubbleDoll);
		}
		huntIcon.layer = uimodelController.TutorialPopupAnchor.layer;
		StartCoroutine("DelayHuntIconPositioning");
	}

	private IEnumerator DelayHuntIconPositioning()
	{
		yield return new WaitForEndOfFrame();
		Vector3 trackerPos = bubbleSprite.transform.position;
		Vector3 pixelPos = UIScreenController.Instance.Camera2DNormal.WorldToScreenPoint(trackerPos);
		pixelPos = new Vector3(pixelPos.x, pixelPos.y, 3f);
		ResetTransform(newPosition: UIScreenController.Instance.Camera3D.ScreenToWorldPoint(pixelPos), trans: huntIcon.transform, positionOffset: WeeklyHuntManager.HUNT_ITEM_POSITION_OFFSET, newRotation: WeeklyHuntManager.HUNT_ITEM_ROTATION, newScale: WeeklyHuntManager.HUNT_ITEM_SCALE);
	}

	private void ResetTransform(Transform trans, Vector3 newPosition, Vector3 positionOffset, Vector3 newRotation, Vector3 newScale)
	{
		trans.position = newPosition;
		trans.localPosition += positionOffset;
		trans.localRotation = Quaternion.Euler(newRotation);
		trans.localScale = newScale;
	}

	private void HandleData()
	{
		mainTitle.text = Strings.Get(StringID.WEEKLY_HUNT_TITLE);
		mainSubtitle.text = Strings.Get(StringID.WEEKLY_HUNT_SUBTITLE);
		mainTitle.color = GlobalColors.WEEKLYHUNT_COLOR_TOPTITLE;
		mainSubtitle.color = GlobalColors.WEEKLYHUNT_COLOR_TOPSUBTITLE;
		bubbleSprite.color = GlobalColorsGenerated.WEEKLYHUNT_COLOR_SPLASH;
		bubbleSpriteShadow.enabled = true;
		if (bubbleSpriteShadow.enabled)
		{
			bubbleSpriteShadow.color = GlobalColorsGenerated.WEEKLYHUNT_COLOR_SPLASH_SHADOW;
		}
		prizeOne = WeeklyHuntManager.GetPrizeDataForPeriod(WeeklyHuntManager.PrizePeriodId.PeriodOne);
		prizeTwo = WeeklyHuntManager.GetPrizeDataForPeriod(WeeklyHuntManager.PrizePeriodId.PeriodTwo);
		prizeThree = WeeklyHuntManager.GetPrizeDataForPeriod(WeeklyHuntManager.PrizePeriodId.PeriodThree);
		prizeFour = WeeklyHuntManager.GetPrizeDataForPeriod(WeeklyHuntManager.PrizePeriodId.PeriodFour);
		if (!_instantiatedPeriodCells)
		{
			_instantiatedPeriodCells = true;
			periodOne = NGUITools.AddChild(popupAnchor.gameObject, periodPrefab).GetComponent<ItemHuntPopupCell>();
			periodTwo = NGUITools.AddChild(popupAnchor.gameObject, periodPrefab).GetComponent<ItemHuntPopupCell>();
			periodThree = NGUITools.AddChild(popupAnchor.gameObject, periodPrefab).GetComponent<ItemHuntPopupCell>();
			if (!WeeklyHuntManager.HIDE_LAST_PERIOD)
			{
				periodFour = NGUITools.AddChild(popupAnchor.gameObject, periodPrefab).GetComponent<ItemHuntPopupCell>();
			}
		}
		periodOne.iconPrefab = iconReferences.GetPrefab(WeeklyHuntManager.GetPrizeForPeriod(WeeklyHuntManager.PrizePeriodId.PeriodOne));
		periodTwo.iconPrefab = iconReferences.GetPrefab(WeeklyHuntManager.GetPrizeForPeriod(WeeklyHuntManager.PrizePeriodId.PeriodTwo));
		periodThree.iconPrefab = iconReferences.GetPrefab(WeeklyHuntManager.GetPrizeForPeriod(WeeklyHuntManager.PrizePeriodId.PeriodThree));
		if ((bool)periodFour)
		{
			periodFour.iconPrefab = iconReferences.GetPrefab(WeeklyHuntManager.GetPrizeForPeriod(WeeklyHuntManager.PrizePeriodId.PeriodFour));
		}
		switch (WeeklyHuntManager.GetActivePeriode())
		{
		case WeeklyHuntManager.PrizePeriodId.PeriodOne:
			currentActiveCell = periodOne;
			break;
		case WeeklyHuntManager.PrizePeriodId.PeriodTwo:
			currentActiveCell = periodTwo;
			break;
		case WeeklyHuntManager.PrizePeriodId.PeriodThree:
			currentActiveCell = periodThree;
			break;
		case WeeklyHuntManager.PrizePeriodId.PeriodFour:
			currentActiveCell = periodFour;
			break;
		default:
			currentActiveCell = null;
			break;
		case WeeklyHuntManager.PrizePeriodId.None:
			break;
		}
		UpdateCell(periodOne, prizeOne, WeeklyHuntManager.PrizePeriodId.PeriodOne);
		UpdateCell(periodTwo, prizeTwo, WeeklyHuntManager.PrizePeriodId.PeriodTwo);
		UpdateCell(periodThree, prizeThree, WeeklyHuntManager.PrizePeriodId.PeriodThree);
		if (!WeeklyHuntManager.HIDE_LAST_PERIOD)
		{
			UpdateCell(periodFour, prizeFour, WeeklyHuntManager.PrizePeriodId.PeriodFour);
			bgOutline.height = _originalBGOutlineScale;
			bgFill.height = _originalBGFillScale;
			popupAnchor.relativeOffset = Vector2.zero;
		}
		else
		{
			bgOutline.height = _shortBGOutlineScale;
			bgFill.height = _shortBGFillScale;
			popupAnchor.relativeOffset = new Vector2(0f, THREE_PERIODS_ANCHOR_OFFSET);
		}
		if (!WeeklyHuntManager.IsAnyPeriodActive() && WeeklyHuntManager.GetActivePeriode() == WeeklyHuntManager.PrizePeriodId.None)
		{
			bottomTitle.text = Strings.Get(StringID.WEEKLY_HUNT_ENDED_TITLE);
			bottomSubtitle.text = Strings.Get(StringID.WEEKLY_HUNT_ENDED_SUBTITLE);
			bottomTitle.color = GlobalColors.WEEKLYHUNT_COLOR_BOTTITLE;
			bottomSubtitle.color = GlobalColors.WEEKLYHUNT_COLOR_BOTSUBTITLE;
			bottomSubtitle.gameObject.SetActive(true);
			bottomTitle.gameObject.SetActive(true);
			if (WeeklyHuntManager.HIDE_LAST_PERIOD)
			{
				bottomtext.transform.localPosition = _bottomTextShortPosition;
			}
			else
			{
				bottomtext.transform.localPosition = _bottomTextOriginalPosition;
			}
		}
		else
		{
			bottomSubtitle.gameObject.SetActive(false);
			bottomTitle.gameObject.SetActive(false);
		}
	}

	private void SetShadowColor(GameObject go, Color32 frontColor, Color32 shadowColor)
	{
		UITextShadowHelper component = go.GetComponent<UITextShadowHelper>();
		if (component != null)
		{
			component.frontColor = frontColor;
			component.shadowColor = shadowColor;
		}
	}

	private void UpdateCell(ItemHuntPopupCell cell, WeeklyHuntManager.WeeklyHuntPeriodData periodData, WeeklyHuntManager.PrizePeriodId periodId)
	{
		bool flag = cell == currentActiveCell;
		Transform transform = cell.transform;
		Vector3 localPosition = transform.localPosition;
		localPosition.y = _currentHeight;
		cell.transform.localPosition = localPosition;
		WeeklyHuntManager.PrizeData prizeForPeriod = WeeklyHuntManager.GetPrizeForPeriod(periodId);
		if (flag)
		{
			_currentHeight = _currentHeight - GAP_CELLS - ACTIVE_CELL_HEIGHT;
		}
		else
		{
			_currentHeight = _currentHeight - GAP_CELLS - DEFAULT_CELL_HEIGHT;
		}
		WeeklyHuntManager.PeriodStatus periodStatus = WeeklyHuntManager.GetPeriodStatus(periodId);
		bool hiddenUntilUnlockedForPeriod = WeeklyHuntManager.GetHiddenUntilUnlockedForPeriod(periodId);
		if (flag)
		{
			bool showPresentIcon = hiddenUntilUnlockedForPeriod && periodData.itemsCollected != periodData.itemsToColect;
			Show3dPrizeIcon(cell, periodId, uimodelController.TutorialPopupAnchor, uimodelController.HuntPrizeAnchor, flag, showPresentIcon);
			cell.cellBackground.height = (int)Mathf.Round(ACTIVE_CELL_HEIGHT);
			Vector3 localPosition2 = cell.iconsGroup.transform.localPosition;
			localPosition2.y -= 7f;
			cell.iconsGroup.transform.localPosition = localPosition2;
			cell.cellBackground.color = GlobalColorsGenerated.WEEKLYHUNT_ACTIVE_BACKGROUND;
			cell.spriteCircleBg.color = GlobalColorsGenerated.WEEKLYHUNT_ACTIVE_CIRCLE;
			SetShadowColor(cell.activePeriodTitle.gameObject, GlobalColorsGenerated.WEEKLYHUNT_ACTIVE_PERIODTITLE, GlobalColorsGenerated.WEEKLYHUNT_ACTIVE_PERIODTITLE_SHADOW);
			SetShadowColor(cell.activeMainTitle.gameObject, GlobalColorsGenerated.WEEKLYHUNT_ACTIVE_MAINTITLE, GlobalColorsGenerated.WEEKLYHUNT_ACTIVE_MAINTITLE_SHADOW);
			cell.activeProgressText.color = GlobalColorsGenerated.WEEKLYHUNT_ACTIVE_PROGRES_TITLE;
			cell.activeProgressBg.color = GlobalColorsGenerated.WEEKLYHUNT_ACTIVE_PROGRES_BG;
			cell.activeProgressFg.color = GlobalColorsGenerated.WEEKLYHUNT_ACTIVE_PROGRES_FG;
			cell.stickerSprite.color = GlobalColorsGenerated.WEEKLYHUNT_ACTIVE_STICKER_BG;
			cell.stickerTextBot.color = GlobalColorsGenerated.WEEKLYHUNT_ACTIVE_STICKER_TEXT;
			cell.stickerTextTop.color = GlobalColorsGenerated.WEEKLYHUNT_ACTIVE_STICKER_TEXT;
			cell.activeLabels.SetActive(true);
			cell.activeProgressSlider.SetActive(true);
			cell.inactiveLabels.SetActive(false);
			cell.stickerGroup.SetActive(true);
			cell.inactiveProgressGroup.SetActive(false);
			cell.spriteCirclePrizeUnknown.enabled = false;
			cell.spriteCirclePrizeFailed.enabled = false;
			if (periodData.itemsCollected == periodData.itemsToColect)
			{
				cell.spriteCirclePrizeCompleted.enabled = true;
				cell.activeMainTitle.text = string.Format(WeeklyHuntManager.GetPrizeTitle(prizeForPeriod), prizeForPeriod.amount);
			}
			else
			{
				cell.spriteCirclePrizeCompleted.enabled = false;
				cell.activeMainTitle.text = ((!hiddenUntilUnlockedForPeriod) ? string.Format(WeeklyHuntManager.GetPrizeTitle(prizeForPeriod), prizeForPeriod.amount) : string.Format(Strings.Get(StringID.WEEKLY_HUNT_HIDDEN_PRIZE_TITLE), prizeForPeriod.amount));
			}
			cell.stickerGroup.SetActive(true);
			endDate = periodData.periodEndDate;
			TimeSpan theStickerContent = endDate - DateTime.Now;
			cell.SetTheStickerContent(theStickerContent);
			cell.activePeriodTitle.text = Strings.Get(periodData.periodTitle);
			cell.RefreshProgressBar(periodData.itemsCollected, periodData.itemsToColect);
			return;
		}
		cell.cellBackground.color = GlobalColorsGenerated.WEEKLYHUNT_INACTIVE_BACKGROUND;
		SetShadowColor(cell.inactivePeriodTitle.gameObject, GlobalColorsGenerated.WEEKLYHUNT_INACTIVE_PERIODTITLE, GlobalColorsGenerated.WEEKLYHUNT_INACTIVE_PERIODTITLE_SHADOW);
		SetShadowColor(cell.inactiveMainTitle.gameObject, GlobalColorsGenerated.WEEKLYHUNT_INACTIVE_MAINTITLE, GlobalColorsGenerated.WEEKLYHUNT_INACTIVE_MAINTITLE_SHADOW);
		cell.inactiveProgressText.color = GlobalColorsGenerated.WEEKLYHUNT_INACTIVE_PROGRES_TITLE;
		cell.spriteCircleBg.color = GlobalColorsGenerated.WEEKLYHUNT_INACTIVE_CIRCLE;
		cell.spriteCirclePrizeFailed.color = GlobalColorsGenerated.WEEKLYHUNT_INACTIVE_ICONS;
		cell.spriteCirclePrizeUnknown.color = GlobalColorsGenerated.WEEKLYHUNT_INACTIVE_ICONS;
		cell.activeLabels.SetActive(false);
		cell.activeProgressSlider.SetActive(false);
		cell.inactiveLabels.SetActive(true);
		cell.stickerGroup.SetActive(false);
		cell.inactiveProgressGroup.SetActive(true);
		cell.inactivePeriodTitle.text = Strings.Get(periodData.periodTitle);
		switch (periodStatus)
		{
		case WeeklyHuntManager.PeriodStatus.Expired:
			cell.spriteCirclePrizeUnknown.enabled = false;
			if (periodData.itemsCollected == periodData.itemsToColect)
			{
				cell.spriteCirclePrizeCompleted.enabled = true;
				cell.spriteCirclePrizeFailed.enabled = false;
				Show3dPrizeIcon(cell, periodId, uimodelController.TutorialPopupAnchor, uimodelController.HuntPrizeAnchor, flag, false);
			}
			else
			{
				cell.spriteCirclePrizeCompleted.enabled = false;
				cell.spriteCirclePrizeFailed.enabled = true;
			}
			cell.inactiveMainTitle.text = string.Format(WeeklyHuntManager.GetPrizeTitleShort(prizeForPeriod), prizeForPeriod.amount);
			cell.inactiveProgressText.text = string.Format("{0} / {1}", periodData.itemsCollected, periodData.itemsToColect);
			break;
		case WeeklyHuntManager.PeriodStatus.Unstarted:
			if (hiddenUntilUnlockedForPeriod)
			{
				cell.spriteCirclePrizeUnknown.enabled = false;
				cell.spriteCirclePrizeCompleted.enabled = false;
				cell.spriteCirclePrizeFailed.enabled = false;
				cell.inactiveMainTitle.text = Strings.Get(StringID.WEEKLY_HUNT_HIDDEN_PRIZE_TITLE_SHORT);
				cell.inactiveProgressText.text = string.Format(Strings.Get(StringID.WEEKLY_HUNT_PERIOD_NOTSTARTED_SUBTITLE), periodData.itemsToColect);
				Show3dPrizeIcon(cell, periodId, uimodelController.TutorialPopupAnchor, uimodelController.HuntPrizeAnchor, false, true);
			}
			else
			{
				cell.spriteCirclePrizeUnknown.enabled = true;
				cell.spriteCirclePrizeCompleted.enabled = false;
				cell.spriteCirclePrizeFailed.enabled = false;
				cell.inactiveMainTitle.text = Strings.Get(StringID.WEEKLY_HUNT_PERIOD_NOTSTARTED_TITLE);
				cell.inactiveProgressText.text = string.Format(Strings.Get(StringID.WEEKLY_HUNT_PERIOD_NOTSTARTED_SUBTITLE), periodData.itemsToColect);
			}
			break;
		}
	}

	private void Update()
	{
		TimeSpan theStickerContent = endDate - DateTime.Now;
		if (theStickerContent.Days < 1 && currentActiveCell != null)
		{
			currentActiveCell.SetTheStickerContent(theStickerContent);
		}
	}

	private void Show3dPrizeIcon(ItemHuntPopupCell cell, WeeklyHuntManager.PrizePeriodId periodId, GameObject parent, GameObject presentParent, bool isCellActive, bool showPresentIcon)
	{
		if (cell.prizeIcon != null)
		{
			cell.prizeIcon.SetActive(true);
		}
		else
		{
			cell.prizeIcon = NGUITools.AddChild(parent, cell.iconPrefab);
		}
		if (cell.presentIcon != null)
		{
			cell.presentIcon.SetActive(true);
		}
		else
		{
			GameObject gameObject = null;
			switch (periodId)
			{
			case WeeklyHuntManager.PrizePeriodId.PeriodOne:
				gameObject = presentPrefab_blue;
				break;
			case WeeklyHuntManager.PrizePeriodId.PeriodTwo:
				gameObject = presentPrefab_green;
				break;
			case WeeklyHuntManager.PrizePeriodId.PeriodThree:
				gameObject = presentPrefab_red;
				break;
			case WeeklyHuntManager.PrizePeriodId.PeriodFour:
				gameObject = presentPrefab_red;
				break;
			default:
				Debug.LogError("Hunt Period not available for present!");
				gameObject = presentPrefab_green;
				break;
			}
			cell.presentIcon = NGUITools.AddChild(presentParent, gameObject);
		}
		cell.prizeIcon.layer = parent.layer;
		cell.presentIcon.layer = presentParent.layer;
		if (!showPresentIcon)
		{
			cell.presentIcon.SetActive(false);
		}
		else
		{
			cell.prizeIcon.SetActive(false);
		}
		if (!showPresentIcon)
		{
			WeeklyHuntManager.PrizeData prizeForPeriod = WeeklyHuntManager.GetPrizeForPeriod(periodId);
			PrizePositionData.TransformValues? prizePositioningData = PrizePositionData.GetPrizePositioningData(prizeForPeriod);
			if (prizePositioningData.HasValue)
			{
				cell.PlaceModel(prizePositioningData.Value.PositionOffset, prizePositioningData.Value.Scale, prizePositioningData.Value.Rotation, showPresentIcon);
			}
		}
		else
		{
			cell.PlaceModel(PrizePositionData.PRESENT_POSITION_OFFSET, PrizePositionData.PRESENT_SCALE, PrizePositionData.PRESENT_ROTATION, showPresentIcon);
		}
	}
}
