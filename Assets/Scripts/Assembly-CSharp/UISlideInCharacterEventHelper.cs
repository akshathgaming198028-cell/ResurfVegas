using UnityEngine;

public class UISlideInCharacterEventHelper : UISlideIn
{
	private const int NOTIFICATION_EDGE_MARGIN = 22;

	private const int MINIMUM_SLIDE_IN_WIDTH = 155;

	[SerializeField]
	private UILabel _timerLabel;

	[SerializeField]
	private UILabel _textLabel;

	[SerializeField]
	private UILabel shadowLabel;

	[SerializeField]
	private UISprite _slideInBackground;

	public Color MyColorActive;

	public Color MyColorInactive;

	public Color shadowColorActive;

	public Color shadowColorInactive;

	private string _MyColorActive;

	private string _MyColorInactive;

	private string _shadowColorActive;

	private string _shadowColorInactive;

	private void Awake()
	{
		StoreColors();
	}

	public void SetupCharacterEventLetters(int letterMask, string extraInfo)
	{
		base.gameObject.SetActive(true);
		UpdateCharacterEventWord(letterMask, extraInfo);
		AdjustSlideInNotificationWidth();
		SlideIn();
	}

	private void AdjustSlideInNotificationWidth()
	{
		_slideInBackground.width = _timerLabel.width + 22;
		if (_slideInBackground.width < 155)
		{
			_slideInBackground.width = 155;
		}
	}

	private void StoreColors()
	{
		_MyColorActive = NGUIText.EncodeColor(MyColorActive);
		_MyColorInactive = NGUIText.EncodeColor(MyColorInactive);
		_shadowColorActive = NGUIText.EncodeColor(shadowColorActive);
		_shadowColorInactive = NGUIText.EncodeColor(shadowColorInactive);
	}

	private void UpdateCharacterEventWord(int letterMask, string extraInfo)
	{
		char[] array = null;
		int num = 0;
		string text = string.Empty;
		string text2 = string.Empty;
		array = extraInfo.ToCharArray();
		num = letterMask;
		num++;
		for (int i = 0; i < array.Length; i++)
		{
			if (IsFilteredCharacter(array[i]))
			{
				num++;
			}
			else if (num > i)
			{
				text = text + "[" + _MyColorActive + "]";
				text2 = text2 + "[" + _shadowColorActive + "]";
			}
			else
			{
				text = text + "[" + _MyColorInactive + "]";
				text2 = text2 + "[" + _shadowColorInactive + "]";
			}
			text = text + array[i] + " ";
			text2 = text2 + array[i] + " ";
		}
		if (array.Length > 0)
		{
			_textLabel.text = text.Substring(0, text.Length - 1);
			shadowLabel.text = text2.Substring(0, text2.Length - 1);
		}
	}

	private bool IsFilteredCharacter(char newChar)
	{
		char[] fILTERED_CHARACTERS = CharacterNameEventManager.FILTERED_CHARACTERS;
		foreach (char c in fILTERED_CHARACTERS)
		{
			if (newChar == c)
			{
				return true;
			}
		}
		return false;
	}
}
