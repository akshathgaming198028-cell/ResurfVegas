using System;
using UnityEngine;

public class CharacterThemeButton : MonoBehaviour
{
	[SerializeField]
	private UISprite _background;

	[SerializeField]
	private UISprite _icon;

	[SerializeField]
	private GameObject giftBuble;

	[SerializeField]
	private GameObject newBuble;

	private int _index;

	private Action<int> _onPress;

	public void AddOnPressListener(Action<int> handler)
	{
		if (handler != null)
		{
			_onPress = (Action<int>)Delegate.Combine(_onPress, handler);
		}
	}

	public void RemoveOnPressListener(Action<int> handler)
	{
		if (handler != null)
		{
			_onPress = (Action<int>)Delegate.Remove(_onPress, handler);
		}
	}

	public void SetColors(Color32 bgColor, Color32 iconColor, int index)
	{
		_background.color = bgColor;
		_icon.color = iconColor;
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

	private void ButtonPressed()
	{
		if (newBuble != null && newBuble.activeSelf)
		{
			newBuble.SetActive(false);
		}
		Action<int> onPress = _onPress;
		if (onPress != null)
		{
			onPress(_index);
		}
	}
}
