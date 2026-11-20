using UnityEngine;

public class UIButtonOverlayOff : MonoBehaviour
{
	public enum ButtonType
	{
		Custom = 0,
		Primary = 1,
		Secondary = 2,
		Tertiary = 3,
		Facebook = 4,
		GameCenter = 5,
		Primary_inactive = 6,
		Tertiary_inactive = 7,
		Footer_back = 8,
		Footer_me = 9,
		Footer_friends = 10,
		Footer_shop = 11,
		Seasonal = 12,
		SocialGrey = 13,
		WeeklyHunt = 14,
		LimitedCharacterPopup = 15,
		LimitedBoardPopup = 16
	}

	public GameObject fillWithPrimeColor;

	private Color32 originalFillColor;

	private UIWidget fillSprite;

	private float DURATION;

	public UISprite overlay;

	protected bool _initDone;

	private bool _buttonStateLocked;

	public ButtonType buttonType;

	private ButtonColorScheme buttonColorScheme;

	private void Awake()
	{
		if (!_initDone)
		{
			Init();
		}
	}

	private void OnDisable()
	{
		if (fillSprite.color != originalFillColor)
		{
			GoToNormalState();
		}
	}

	public void SetButtonType(ButtonType type)
	{
		buttonType = type;
		Init();
		ButtonPressed(false);
	}

	protected void Init()
	{
		_initDone = true;
		if (fillSprite == null)
		{
			fillSprite = fillWithPrimeColor.GetComponent<UIWidget>();
		}
		buttonColorScheme = GlobalColors.GetButtonColorScheme(buttonType, base.gameObject);
		Color32? original = buttonColorScheme.original;
		if (original.HasValue)
		{
			UIWidget uIWidget = fillSprite;
			Color32? original2 = buttonColorScheme.original;
			uIWidget.color = original2.Value;
		}
		originalFillColor = fillSprite.color;
		if (overlay != null)
		{
			overlay.enabled = true;
			Color32? light = buttonColorScheme.light;
			if (light.HasValue)
			{
				UISprite uISprite = overlay;
				Color32? light2 = buttonColorScheme.light;
				uISprite.color = light2.Value;
			}
		}
	}

	public void GoToNormalState()
	{
		if (!_buttonStateLocked)
		{
			if ((bool)GetComponent<BoxCollider>())
			{
				GetComponent<BoxCollider>().enabled = true;
			}
			fillSprite.color = originalFillColor;
			Color32? light = buttonColorScheme.light;
			if (light.HasValue)
			{
				UISprite uISprite = overlay;
				Color32? light2 = buttonColorScheme.light;
				uISprite.color = light2.Value;
			}
			if (overlay != null)
			{
				overlay.enabled = true;
			}
		}
	}

	public void GoToNotAvailableState()
	{
		if (!_buttonStateLocked)
		{
			if ((bool)GetComponent<BoxCollider>())
			{
				GetComponent<BoxCollider>().enabled = false;
			}
			overlay.enabled = false;
			Color32? unavailable = buttonColorScheme.unavailable;
			if (unavailable.HasValue)
			{
				UIWidget uIWidget = fillSprite;
				Color32? unavailable2 = buttonColorScheme.unavailable;
				uIWidget.color = unavailable2.Value;
			}
		}
	}

	public void GoToSelectedState()
	{
		if (!_buttonStateLocked)
		{
			if ((bool)GetComponent<BoxCollider>())
			{
				GetComponent<BoxCollider>().enabled = false;
			}
			overlay.enabled = false;
			Color32? selected = buttonColorScheme.selected;
			if (selected.HasValue)
			{
				UIWidget uIWidget = fillSprite;
				Color32? selected2 = buttonColorScheme.selected;
				uIWidget.color = selected2.Value;
			}
			else
			{
				Debug.LogWarning("No 'selected' color found for button type: " + buttonType, base.gameObject);
			}
		}
	}

	public void LockState(bool locked)
	{
		_buttonStateLocked = locked;
	}

	protected virtual void OnPress(bool isPressed)
	{
		ButtonPressed(isPressed);
	}

	public void ButtonPressed(bool isPressed)
	{
		if (!base.enabled)
		{
			return;
		}
		if (overlay != null)
		{
			Color32? light = buttonColorScheme.light;
			if (light.HasValue)
			{
				Color32? light2 = buttonColorScheme.light;
				Color32 value = light2.Value;
				TweenColor.Begin(overlay.gameObject, DURATION, (!isPressed) ? value : new Color32(value.r, value.g, value.b, 0));
			}
			else if (isPressed)
			{
				overlay.gameObject.SetActive(false);
			}
			else
			{
				overlay.gameObject.SetActive(true);
			}
		}
		if (isPressed)
		{
			byte r = (byte)Mathf.Clamp(originalFillColor.r - 30, 0, 255);
			byte g = (byte)Mathf.Clamp(originalFillColor.g - 30, 0, 255);
			byte b = (byte)Mathf.Clamp(originalFillColor.b - 30, 0, 255);
			TweenColor.Begin(fillSprite.gameObject, DURATION, new Color32(r, g, b, originalFillColor.a));
		}
		else
		{
			TweenColor.Begin(fillSprite.gameObject, DURATION, originalFillColor);
		}
	}

	public void ResetTweens()
	{
		if (!base.enabled)
		{
			return;
		}
		if (overlay != null)
		{
			Color32? light = buttonColorScheme.light;
			if (light.HasValue)
			{
				GameObject go = overlay.gameObject;
				float dURATION = DURATION;
				Color32? light2 = buttonColorScheme.light;
				TweenColor.Begin(go, dURATION, light2.Value);
			}
		}
		TweenColor.Begin(fillSprite.gameObject, DURATION, originalFillColor);
	}
}
