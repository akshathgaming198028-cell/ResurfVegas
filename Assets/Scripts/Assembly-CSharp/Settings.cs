using System;
using UnityEngine;

public class Settings : MonoBehaviour
{
	private const string OPTION_SOUND_KEY = "OPTION_SOUND";

	private const int OPTION_SOUND_DEFAULT = 1;

	private const string OPTION_REMINDER_KEY = "OPTION_REMINDER_WITH_DEFAULT_OFF";

	private const int OPTION_REMINDER_DEFAULT = 0;

	private const string OPTION_TOPRUNCHALLENGERS_KEY = "OPTION_TOPRUNCHALLENGERS";

	private const int OPTION_TOPRUNCHALLENGERS_DEFAULT = 1;

	private static bool _optionsLoaded;

	private static bool _optionSound;

	private static bool _anotherSoundIsPlayingOnDevice;

	private static bool _optionReminder;

	private static bool _optionMessages;

	private static bool _optionTopRunChallengers;

	private static bool optionMessageDirty;

	public static bool optionSound
	{
		get
		{
			LoadOptionsIfNeeded();
			return _optionSound;
		}
		set
		{
			_optionSound = value;
			AudioListener.volume = ((!_optionSound) ? 0f : 1f);
			PlayerPrefs.SetInt("OPTION_SOUND", _optionSound ? 1 : 0);
		}
	}

	// 0 = not chosen yet (device default), 1 = High, 2 = Low. Read once at startup in DeviceInfo.
	public static int optionGraphics
	{
		get
		{
			return PlayerPrefs.GetInt("OPTION_GRAPHICS", 0);
		}
		set
		{
			PlayerPrefs.SetInt("OPTION_GRAPHICS", value);
		}
	}

	public static bool optionTopRunChallengers
	{
		get
		{
			LoadOptionsIfNeeded();
			return _optionTopRunChallengers;
		}
		set
		{
			_optionTopRunChallengers = value;
			PlayerPrefs.SetInt("OPTION_TOPRUNCHALLENGERS", _optionTopRunChallengers ? 1 : 0);
			Action<bool> onTopRunChallengerSettingChanged = Settings.OnTopRunChallengerSettingChanged;
			if (onTopRunChallengerSettingChanged != null)
			{
				onTopRunChallengerSettingChanged(value);
			}
		}
	}

	public static bool anotherSoundIsPlayingOnDevice
	{
		get
		{
			return _anotherSoundIsPlayingOnDevice;
		}
		set
		{
			_anotherSoundIsPlayingOnDevice = value;
		}
	}

	public static bool optionReminder
	{
		get
		{
			LoadOptionsIfNeeded();
			return _optionReminder;
		}
		set
		{
			_optionReminder = value;
			PlayerPrefs.SetInt("OPTION_REMINDER_WITH_DEFAULT_OFF", _optionReminder ? 1 : 0);
		}
	}

	public static bool optionMessages
	{
		get
		{
			return PlayerPrefs.GetInt("OPTION_MESSAGES", 1) == 1;
		}
		set
		{
			PlayerPrefs.SetInt("OPTION_MESSAGES", value ? 1 : 0);
			optionMessageDirty = true;
		}
	}

	public static event Action<bool> OnTopRunChallengerSettingChanged;

	private void Awake()
	{
		LoadOptionsIfNeeded();
	}

	private static void LoadOptionsIfNeeded()
	{
		if (!_optionsLoaded)
		{
			_optionSound = PlayerPrefs.GetInt("OPTION_SOUND", 1) != 0;
			AudioListener.volume = ((!_optionSound) ? 0f : 1f);
			_optionReminder = PlayerPrefs.GetInt("OPTION_REMINDER_WITH_DEFAULT_OFF", 0) != 0;
			_optionTopRunChallengers = PlayerPrefs.GetInt("OPTION_TOPRUNCHALLENGERS", 1) != 0;
			_optionsLoaded = true;
		}
	}

	public static void ToggleCurrentSoundOption()
	{
		if (object.Equals(AudioListener.volume, 1f))
		{
			AudioListener.volume = 0f;
		}
		else
		{
			AudioListener.volume = 1f;
		}
		anotherSoundIsPlayingOnDevice = false;
	}

	public static void EnableCurrentSoundOption()
	{
		AudioListener.volume = 1f;
		anotherSoundIsPlayingOnDevice = false;
	}

	public static void UpdateOptionMessagesIfNeeded()
	{
		if (optionMessageDirty)
		{
			optionMessageDirty = false;
			SocialManager.instance.UpdateRemoteNotificationControl(optionMessages);
		}
	}
}
