using UnityEngine;

public class HoverboardThemeButton : MonoBehaviour
{
	public enum ThemeState
	{
		NotOwned = 0,
		OwnedAndInactive = 1,
		OwnedAndActive = 2
	}

	public delegate void OnPressEvent(int index);

	[SerializeField]
	private UISprite _background;

	[SerializeField]
	private UISprite _icon;

	[SerializeField]
	private GameObject giftBuble;

	[SerializeField]
	private GameObject newBuble;

	[SerializeField]
	private GameObject selectedCircle;

	[SerializeField]
	private GameObject ownedCircle;

	private int _index;

	private HoverboardThemes.ThemeIdentifier _identifier;

	public int ThemeIndex
	{
		get
		{
			return _index;
		}
		private set
		{
		}
	}

	public event OnPressEvent OnPress;

	public void InitButton(string iconName, Color32 bgColor, Color32 iconColor, int index)
	{
		if (!string.IsNullOrEmpty(iconName))
		{
			_icon.spriteName = iconName;
		}
		_icon.MakePixelPerfect();
		_icon.color = iconColor;
		_background.color = bgColor;
		_index = index;
	}

	public void setActiveBubles(bool enableGiftBuble, bool enableNewBuble)
	{
		if (giftBuble != null)
		{
			giftBuble.SetActive(enableGiftBuble);
		}
		if (newBuble != null)
		{
			newBuble.SetActive(enableNewBuble);
		}
	}

	public void SetCurrentState(ThemeState newState)
	{
		switch (newState)
		{
		case ThemeState.NotOwned:
			selectedCircle.SetActive(false);
			ownedCircle.SetActive(false);
			break;
		case ThemeState.OwnedAndActive:
			selectedCircle.SetActive(true);
			ownedCircle.SetActive(false);
			break;
		case ThemeState.OwnedAndInactive:
			selectedCircle.SetActive(false);
			ownedCircle.SetActive(true);
			break;
		}
	}

	private void ButtonPressed()
	{
		if (newBuble != null && newBuble.activeSelf)
		{
			newBuble.SetActive(false);
		}
		OnPressEvent onPress = this.OnPress;
		if (onPress != null)
		{
			onPress(_index);
		}
	}
}
