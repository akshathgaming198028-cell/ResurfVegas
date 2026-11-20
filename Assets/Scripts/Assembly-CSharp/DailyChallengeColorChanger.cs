using UnityEngine;

public class DailyChallengeColorChanger : MonoBehaviour
{
	public UILabel shadowLabel;

	public Color MyColorActive;

	public Color MyColorInactive;

	public Color shadowColorActive;

	public Color shadowColorInactive;

	private string _MyColorActive;

	private string _MyColorInactive;

	private string _shadowColorActive;

	private string _shadowColorInactive;

	private UILabel _myLabel;

	private string _cachedText = string.Empty;

	private string _cachedDailyWord = string.Empty;

	private IntMask _cachedDailyMask = -1;

	private void Awake()
	{
		_myLabel = base.gameObject.GetComponent<UILabel>();
		_MyColorActive = NGUIText.EncodeColor(MyColorActive);
		_MyColorInactive = NGUIText.EncodeColor(MyColorInactive);
		_shadowColorActive = NGUIText.EncodeColor(shadowColorActive);
		_shadowColorInactive = NGUIText.EncodeColor(shadowColorInactive);
	}

	private void OnEnable()
	{
		UpdateDailyWord();
	}

	private void Update()
	{
		UpdateDailyWord();
	}

	private void UpdateDailyWord()
	{
		if (_myLabel == null)
		{
			_myLabel = base.gameObject.GetComponent<UILabel>();
		}
		string text = PlayerInfo.Instance.dailyWord;
		if (string.IsNullOrEmpty(text))
		{
			text = string.Empty;
		}
		int length = text.Length;
		IntMask dailyWordUnlockedMask = PlayerInfo.Instance.dailyWordUnlockedMask;
		if (!(text == _cachedDailyWord) || (int)dailyWordUnlockedMask != (int)_cachedDailyMask)
		{
			_cachedDailyWord = text;
			_cachedDailyMask = dailyWordUnlockedMask;
			string text2 = string.Empty;
			for (int i = 0; i < length; i++)
			{
				text2 = ((!dailyWordUnlockedMask[i]) ? (text2 + "[" + _MyColorInactive + "]") : (text2 + "[" + _MyColorActive + "]"));
				text2 = text2 + text[i] + " ";
			}
			if (length > 0)
			{
				_cachedText = text2.Substring(0, text2.Length - 1);
				_myLabel.text = _cachedText;
			}
			if (length > 0)
			{
				_cachedText = text2.Substring(0, text2.Length - 1);
				_myLabel.text = _cachedText;
			}
			string text3 = string.Empty;
			for (int j = 0; j < length; j++)
			{
				text3 = ((!dailyWordUnlockedMask[j]) ? (text3 + "[" + _shadowColorInactive + "]") : (text3 + "[" + _shadowColorActive + "]"));
				text3 = text3 + text[j] + " ";
			}
			if (length > 0)
			{
				shadowLabel.text = text3.Substring(0, text3.Length - 1);
			}
		}
	}
}
