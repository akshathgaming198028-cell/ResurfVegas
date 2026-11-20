using UnityEngine;

public class CharactersNotificationHelper : MonoBehaviour
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
		if (!_hasGoneToZero && UIScreenController.Instance.GetTopScreenName() == "CharacterScreen")
		{
			RefreshNotification();
		}
	}

	private void RefreshNotification()
	{
		int numberOfNewCharactersOrThemes = CharacterStickerManager.getNumberOfNewCharactersOrThemes();
		if (numberOfNewCharactersOrThemes > 0)
		{
			notificationIcon.enabled = true;
			notificationLabel.enabled = true;
			notificationLabel.text = numberOfNewCharactersOrThemes.ToString();
		}
		else
		{
			notificationIcon.enabled = false;
			notificationLabel.enabled = false;
			_hasGoneToZero = true;
		}
	}
}
