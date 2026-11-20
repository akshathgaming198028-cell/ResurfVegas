using UnityEngine;

public class WeeklyHuntIconPositioning : MonoBehaviour
{
	public enum IconPlacement
	{
		_notSet = 0,
		FrontUIButton = 1,
		CelebrationPopup = 2,
		ItemHuntPopupActive = 3,
		ItemHuntPopupInactive = 4,
		SlideIn = 5,
		IngameCounter = 6,
		IngameCounterBackground = 7
	}

	[SerializeField]
	private IconPlacement _placement;

	private bool RUN_ONCE = true;

	private Vector3 CELEBRATION_POPUP_ICON_POSITION = new Vector2(-92f, -50f);

	private Vector3 FRONT_UI_ICON_POSITION = new Vector2(-0.5f, 0f);

	private Vector3 HUNTPOPUP_ACTIVE_ICON_POSITION = new Vector2(73f, 7f);

	private Vector3 HUNTPOPUP_INACTIVE_ICON_POSITION = new Vector2(74f, -47f);

	private Vector3 SLIDEIN_ICON_POSITION = new Vector2(-79f, -31f);

	private Vector3 INGAME_COUNTER_ICON_POSITION = new Vector2(-21f, -15f);

	private Vector3 INGAME_COUNTER_BACKGROUND_POSITION = new Vector2(-36f, 0f);

	private int HEIGHT_SLIDE_IN = 35;

	private int HEIGHT_CELEBRATION_POPUP = 44;

	private int HEIGHT_PERIOD_CELL_ACTIVE = 30;

	private int HEIGHT_PERIOD_CELL_INACTIVE = 21;

	private int HEIGHT_INGAME = 32;

	private int HEIGHT_FRONT_UI_BUTTON = 30;

	public void ForceUpdate()
	{
		SetSize();
		Update();
	}

	private void Awake()
	{
		ForceUpdate();
	}

	private void Update()
	{
		SetPosition();
		if (RUN_ONCE)
		{
			base.enabled = false;
		}
	}

	private void SetSize()
	{
		int num = 0;
		switch (_placement)
		{
		case IconPlacement.FrontUIButton:
			num = HEIGHT_FRONT_UI_BUTTON;
			break;
		case IconPlacement.ItemHuntPopupActive:
			num = HEIGHT_PERIOD_CELL_ACTIVE;
			break;
		case IconPlacement.CelebrationPopup:
			num = HEIGHT_CELEBRATION_POPUP;
			break;
		case IconPlacement.SlideIn:
			num = HEIGHT_SLIDE_IN;
			break;
		case IconPlacement.IngameCounter:
			num = HEIGHT_INGAME;
			break;
		case IconPlacement.ItemHuntPopupInactive:
			num = HEIGHT_PERIOD_CELL_INACTIVE;
			break;
		}
		if (num != 0)
		{
			UISprite component = GetComponent<UISprite>();
			if ((bool)component)
			{
				component.keepAspectRatio = UIWidget.AspectRatioSource.Free;
				component.MakePixelPerfect();
				component.keepAspectRatio = UIWidget.AspectRatioSource.BasedOnHeight;
				component.height = num;
			}
		}
	}

	private void SetPosition()
	{
		Vector3 localPosition = Vector3.zero;
		switch (_placement)
		{
		case IconPlacement.FrontUIButton:
			localPosition = FRONT_UI_ICON_POSITION;
			break;
		case IconPlacement.CelebrationPopup:
			localPosition = CELEBRATION_POPUP_ICON_POSITION;
			break;
		case IconPlacement.ItemHuntPopupActive:
			localPosition = HUNTPOPUP_ACTIVE_ICON_POSITION;
			break;
		case IconPlacement.ItemHuntPopupInactive:
			localPosition = HUNTPOPUP_INACTIVE_ICON_POSITION;
			break;
		case IconPlacement.SlideIn:
			localPosition = SLIDEIN_ICON_POSITION;
			break;
		case IconPlacement.IngameCounter:
			localPosition = INGAME_COUNTER_ICON_POSITION;
			break;
		case IconPlacement.IngameCounterBackground:
			localPosition = INGAME_COUNTER_BACKGROUND_POSITION;
			break;
		}
		base.transform.localPosition = localPosition;
	}
}
