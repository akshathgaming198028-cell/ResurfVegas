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
		if (SocialManager.instance.requestInvolvingDevicetokenInProgress)
		{
			if (buttonOverlayGO != null)
			{
				buttonOverlayGO.SetActive(false);
			}
			normalFillSprite.color = GlobalColors.PRIMARY_ACTION_COLOR_UNAVAILABLE;
			btnNotification.GetComponent<BoxCollider>().enabled = false;
		}
		else
		{
			if (buttonOverlayGO != null)
			{
				buttonOverlayGO.SetActive(true);
			}
			normalFillSprite.color = originalColor;
			btnNotification.GetComponent<BoxCollider>().enabled = true;
		}
	}

	private void OnDisable()
	{
		Settings.UpdateOptionMessagesIfNeeded();
	}
}
