using UnityEngine;

public class BoardsNotificationHelper : MonoBehaviour
{
	[SerializeField]
	private UISprite notificationIcon;

	[SerializeField]
	private UILabel notificationLabel;

	private bool _hasGoneToZero;

	private void OnEnable()
	{
		RefreshNotification();
	}

	private void Update()
	{
		if (!_hasGoneToZero && UIScreenController.Instance.GetTopScreenName() == "BoardScreen")
		{
			RefreshNotification();
		}
	}

	private void RefreshNotification()
	{
		int numberOfNewHoverboards = HoverboardStickerManager.getNumberOfNewHoverboards();
		int num = PlayerInfo.Instance.HoverboardCustomizationData.GetNumberOfNewUnseenBoardThemes();
		if (PlayerInfo.Instance.isFreshInstall)
		{
			num = 0;
		}
		if (numberOfNewHoverboards + num > 0)
		{
			notificationIcon.enabled = true;
			notificationLabel.enabled = true;
			notificationLabel.text = (numberOfNewHoverboards + num).ToString();
		}
		else
		{
			notificationIcon.enabled = false;
			notificationLabel.enabled = false;
			_hasGoneToZero = true;
		}
	}
}
