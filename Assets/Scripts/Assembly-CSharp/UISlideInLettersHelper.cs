using UnityEngine;

public class UISlideInLettersHelper : UISlideIn
{
	private const int NOTIFICATION_EDGE_MARGIN = 22;

	private const int MINIMUM_SLIDE_IN_WIDTH = 155;

	[SerializeField]
	private UILabel _timerLabel;

	[SerializeField]
	private UISprite _slideInBackground;

	public void SetupLetters()
	{
		base.gameObject.SetActive(true);
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
}
