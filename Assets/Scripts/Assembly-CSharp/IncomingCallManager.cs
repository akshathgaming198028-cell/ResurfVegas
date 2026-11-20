using System.Diagnostics;
using UnityEngine;

public class IncomingCallManager : MonoBehaviour
{
	private static AndroidIncomingCallBridge _platformBridge;

	private static bool _gameSoundEnabledBeforeCall;

	private static bool _hasMutedSound;

	private static bool _isInCall;

	public static bool IsInCall
	{
		get
		{
			return _isInCall;
		}
	}

	public static void Init()
	{
		if (_platformBridge == null && Application.platform == RuntimePlatform.Android)
		{
			_platformBridge = AndroidIncomingCallBridge.CreatePlugin();
			if (_platformBridge != null)
			{
				_platformBridge.onPhoneStateChanged += PhoneStateChanged;
				_platformBridge.Init();
			}
		}
	}

	private static void PhoneStateChanged(string state)
	{
		bool result = false;
		bool.TryParse(state, out result);
		if (result)
		{
			RestoreSound();
		}
		else
		{
			MuteSound();
		}
	}

	private static void RestoreSound()
	{
		Settings.optionSound = _gameSoundEnabledBeforeCall;
		HouseKeeper.DeleteSoundOptionBeforeVideoAds();
		_isInCall = false;
		_hasMutedSound = false;
	}

	private static void MuteSound()
	{
		_isInCall = true;
		if (!_hasMutedSound)
		{
			_hasMutedSound = true;
			_gameSoundEnabledBeforeCall = Settings.optionSound;
			HouseKeeper.SaveSoundOptionBeforeVideoAds(_gameSoundEnabledBeforeCall);
		}
		Settings.optionSound = false;
	}

	[Conditional("ENABLE_INCOMINGCALL_LOGS")]
	private static void LogError(string msg, Object context = null)
	{
		Debug.LogError("IncomingCallManager: " + msg, context);
	}

	[Conditional("ENABLE_INCOMINGCALL_LOGS")]
	private static void LogWarning(string msg, Object context = null)
	{
		Debug.LogWarning("IncomingCallManager: " + msg, context);
	}

	[Conditional("ENABLE_INCOMINGCALL_LOGS")]
	private static void Log(string msg, Object context = null)
	{
		Debug.Log("IncomingCallManager: " + msg, context);
	}
}
