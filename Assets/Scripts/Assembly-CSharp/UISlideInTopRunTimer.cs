using UnityEngine;

public class UISlideInTopRunTimer : UISlideIn
{
	[SerializeField]
	private UILabel _timerLabel;

	[SerializeField]
	private UISprite _slideInBackground;

	[SerializeField]
	private UISprite _slideInBackgroundDark;

	private int _NotificationEdgeMargin = 35;

	public void SetupSlideTopRunTimer(int TimeLeftInTopRunWeekInMinutes)
	{
		base.gameObject.SetActive(true);
		if (TimeLeftInTopRunWeekInMinutes > 1)
		{
			_timerLabel.text = TimeLeftInTopRunWeekInMinutes + " minutes left!";
		}
		else if (TimeLeftInTopRunWeekInMinutes == 1)
		{
			_timerLabel.text = TimeLeftInTopRunWeekInMinutes + " minute left!";
		}
		else
		{
			_timerLabel.text = "Ending Soon!";
		}
		AdjustSlideInNotificationWidth(_timerLabel);
		SlideIn();
	}

	private void AdjustSlideInNotificationWidth(UILabel timeLabel)
	{
		_slideInBackground.width = _slideInBackgroundDark.width + _timerLabel.width + _NotificationEdgeMargin;
	}
}
