using UnityEngine;

public class SoundButton : MonoBehaviour
{
	[SerializeField]
	private UISprite iconOff;

	[SerializeField]
	private UILabel soundLabel;

	private void Awake()
	{
		_SetupButton(Settings.optionSound);
	}

	private void OnEnable()
	{
		_SetupButton(Settings.optionSound && !Settings.anotherSoundIsPlayingOnDevice);
	}

	public void Click()
	{
		if (Settings.anotherSoundIsPlayingOnDevice)
		{
			Settings.ToggleCurrentSoundOption();
		}
		else
		{
			Settings.optionSound = !Settings.optionSound;
		}
		_SetupButton(Settings.optionSound);
	}

	private void _SetupButton(bool enabled)
	{
		if (enabled)
		{
			soundLabel.text = Strings.Get(StringID.SETTINGS_POPUP_SOUND_ON);
			iconOff.enabled = false;
		}
		else
		{
			soundLabel.text = Strings.Get(StringID.SETTINGS_POPUP_SOUND_OFF);
			iconOff.enabled = true;
		}
	}
}
