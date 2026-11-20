using System;
using UnityEngine;

public class UISlideInCharacterEventRewardHelper : UISlideIn
{
	private const int NOTIFICATION_EDGE_MARGIN = 90;

	private const int MINIMUM_SLIDE_IN_WIDTH = 170;

	[SerializeField]
	private UILabel _label;

	[SerializeField]
	private UISprite _slideInBackground;

	[SerializeField]
	private UISprite _slideInBackgroundDark;

	public void SetupCharacterEventReward(string payload)
	{
		base.gameObject.SetActive(true);
		SetLabel(payload);
		AdjustSlideInNotificationWidth();
		SlideIn();
	}

	private void SetLabel(string payload)
	{
		string empty = string.Empty;
		empty = payload;
		string text = string.Empty;
		for (int i = 0; i < empty.Length; i++)
		{
			text = text + empty[i] + " ";
		}
		if (text.Length > 0)
		{
			text.Substring(0, text.Length - 1);
		}
		_label.text = text + Environment.NewLine + "complete";
	}

	private void AdjustSlideInNotificationWidth()
	{
		_slideInBackground.width = _slideInBackgroundDark.width + _label.width + 90;
		_slideInBackground.width = _label.width + 90;
		if (_slideInBackground.width < 170)
		{
			_slideInBackground.width = 170;
		}
	}
}
