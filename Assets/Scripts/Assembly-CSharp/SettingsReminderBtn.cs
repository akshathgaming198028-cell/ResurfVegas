using UnityEngine;

public class SettingsReminderBtn : MonoBehaviour
{
	[SerializeField]
	private UISprite iconOff;

	[SerializeField]
	private UILabel reminderLabel;

	private void Awake()
	{
		SetupButton();
	}

	public void Click()
	{
		Settings.optionReminder = !Settings.optionReminder;
		SetupButton();
	}

	private void SetupButton()
	{
		if (Settings.optionReminder)
		{
			reminderLabel.text = Strings.Get(StringID.SETTINGS_POPUP_REMINDER_ON);
			iconOff.enabled = false;
		}
		else
		{
			reminderLabel.text = Strings.Get(StringID.SETTINGS_POPUP_REMINDER_OFF);
			iconOff.enabled = true;
		}
	}
}
