using UnityEngine;

public class NotificationsButton : MonoBehaviour
{
	[SerializeField]
	private UISprite normalFillSprite;

	[SerializeField]
	private UISprite iconOff;

	[SerializeField]
	private UILabel reminderLabel;

	[SerializeField]
	private Transform btnNotification;

	[SerializeField]
	private GameObject buttonOverlayGO;

	private Color originalColor;

	private void Awake()
	{
		originalColor = normalFillSprite.color;
	}

	private void OnEnable()
	{
		SetupButton();
	}

	public void Click()
	{
		Settings.optionMessages = !Settings.optionMessages;
		SetupButton();
	}

	private void SetupButton()
	{
		if (Settings.optionMessages)
		{
			reminderLabel.text = Strings.Get(StringID.SETTINGS_POPUP_MESSAGES_ON);
			iconOff.enabled = false;
		}
		else
		{
			reminderLabel.text = Strings.Get(StringID.SETTINGS_POPUP_MESSAGES_OFF);
			iconOff.enabled = true;
		}
		// Always stays tappable: the device-token request can stay pending forever (push plugin missing),
		// which used to grey the row and disable its tap area.
		if (buttonOverlayGO != null)
		{
			buttonOverlayGO.SetActive(true);
		}
		normalFillSprite.color = originalColor;
		btnNotification.GetComponent<BoxCollider>().enabled = true;
	}

	private void OnDisable()
	{
		Settings.UpdateOptionMessagesIfNeeded();
	}
}
