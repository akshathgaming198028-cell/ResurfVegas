using UnityEngine;

public class TopRunChallengersButton : MonoBehaviour
{
	[SerializeField]
	private UISprite iconOff;

	[SerializeField]
	private UILabel topRunChallengersLabel;

	private void Awake()
	{
		_SetupButton(Settings.optionTopRunChallengers);
	}

	public void Click()
	{
		Settings.optionTopRunChallengers = !Settings.optionTopRunChallengers;
		_SetupButton(Settings.optionTopRunChallengers);
	}

	private void _SetupButton(bool enabled)
	{
		if (enabled)
		{
			topRunChallengersLabel.text = Strings.Get(StringID.SETTINGS_POPUP_TOPRUNCHALLENGERS_ON);
			iconOff.enabled = false;
		}
		else
		{
			topRunChallengersLabel.text = Strings.Get(StringID.SETTINGS_POPUP_TOPRUNCHALLENGERS_OFF);
			iconOff.enabled = true;
		}
	}
}
