using System;
using System.Collections;
using SYBO.Core.Utilities;
using UnityEngine;
using com.kiloo.awards;

public class SSEventsManagerController
{
	public const string ONLINE_SETTING_EVENT_ID = "event_id";

	public const string ONLINE_SETTING_EVENT_START = "event_start";

	public const string ONLINE_SETTING_EVENT_END = "event_end";

	public const string ONLINE_SETTING_EVENT_VALUE = "event_value";

	public const string ONLINE_SETTING_EVENT_TYPE = "event_type";

	public const string ONLINE_SETTING_TITLE_FIRST = "event_popup_title_first";

	public const string ONLINE_SETTING_TITLE_SECOND = "event_popup_title_second";

	public const string ONLINE_SETTING_DESCRIPTION_FIRST = "event_popup_description_first";

	public const string ONLINE_SETTING_DESCRIPTION_SECOND = "event_popup_description_second";

	public const string ONLINE_SETTING_EVENT_RIBBON_TEXT = "event_ribbon_text";

	public const string ONLINE_SETTING_EVENT_MEGA_JACKPOT_MB_VALUE = "event_jackpot_mysterybox_value";

	public const string ONLINE_SETTING_EVENT_MEGA_JACKPOT_SMB_VALUE = "event_jackpot_supermysterybox_value";

	private IEventManager _currentEventManager;

	private static GameObject _eventTimerGameObject;

	private static MonoBehaviour _eventTimerMono;

	private bool _isInitialized;

	private static SSEventsManagerController _instance;

	public static SSEventsManagerController instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new SSEventsManagerController();
			}
			return _instance;
		}
	}

	public static event Action OnSSEventStartedHandler;

	public static event Action OnSSEventFinishedHandler;

	public static event Action OnSSEventChangedHandler;

	private SSEventsManagerController()
	{
		if (!_isInitialized)
		{
			Initialize();
		}
	}

	public void Initialize()
	{
		if (!_isInitialized)
		{
			OnlineSettings.instance.AddOnFinishedDownloadingOnlineSettingsHandler(StartEventTimer);
			UIScreenController.OnApplicationResumed += StartEventTimer;
			if (Globals.DEBUG_USE_EVENT_TESTER)
			{
				EventTesterManager.OnEventTesterInfoChanged += StartEventTimer;
			}
			StartEventTimer();
			_isInitialized = true;
		}
	}

	public IEventManager GetActiveEventManager(SSEventType desiredEventType = SSEventType.None)
	{
		IEventManager result = null;
		if (_currentEventManager != null && _currentEventManager.IsEventActive())
		{
			if (desiredEventType != SSEventType.None)
			{
				if (_currentEventManager.GetEventType() == desiredEventType)
				{
					result = _currentEventManager;
				}
			}
			else
			{
				result = _currentEventManager;
			}
		}
		return result;
	}

	public T GetTypeOfActiveEventManager<T>()
	{
		T result = default(T);
		if (_currentEventManager != null && _currentEventManager.IsEventActive())
		{
			try
			{
				result = (T)_currentEventManager;
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Concat("The type of specified manager could not be found: ", typeof(T), ", stack trace: ", ex.StackTrace));
			}
		}
		return result;
	}

	public void RestoreBoardAfterEventIfNeeded()
	{
		PlayerInfo playerInfo = PlayerInfo.Instance;
		if (!playerInfo.shouldRestoreAfterEvent || (TimeLeftInSeconds().CompareTo(0L) > 0 && PlayerInfo.Instance.lastEventID == GetCurrentEventID()))
		{
			return;
		}
		Hoverboards.BoardType currentHoverboard = playerInfo.currentHoverboard;
		Hoverboards.BoardType previousHoverboard = playerInfo.previousHoverboard;
		if (!string.IsNullOrEmpty(playerInfo.lastEventItemID))
		{
			Hoverboards.BoardType boardType = (Hoverboards.BoardType)(int)Enum.Parse(typeof(Hoverboards.BoardType), playerInfo.lastEventItemID);
			if (currentHoverboard == boardType)
			{
				playerInfo.LockHoverboard(currentHoverboard);
				UIModelController.Instance.SelectCurrentlyShownBoard(previousHoverboard);
			}
		}
		playerInfo.shouldRestoreAfterEvent = false;
	}

	public long TimeLeftInSeconds()
	{
		double unixCurrentTimestamp = GetUnixCurrentTimestamp();
		if (_currentEventManager != null && _currentEventManager.IsEventActive())
		{
			long num = EndTimestampForCurrentEventType();
			if (num > 0)
			{
				return num - (long)unixCurrentTimestamp;
			}
			Debug.LogWarning("Online setting had a wrong format for an end event");
		}
		return 0L;
	}

	public void CurrentEventHasBeenTriedNow()
	{
		if (_currentEventManager != null)
		{
			PlayerInfo.Instance.lastTimeAnEventHasBeenTried = EndTimestampForCurrentEventType();
		}
	}

	public bool HasTheCurrentEventBeenTried()
	{
		long lastTimeAnEventHasBeenTried = PlayerInfo.Instance.lastTimeAnEventHasBeenTried;
		if (_currentEventManager != null)
		{
			return lastTimeAnEventHasBeenTried == EndTimestampForCurrentEventType();
		}
		return false;
	}

	private void StartEventTimer()
	{
		long num = EndTimestampForCurrentEventType();
		long num2 = StartTimestampForCurrentEventType();
		long num3 = (long)GetUnixCurrentTimestamp();
		long timeLeft = 0L;
		if (_eventTimerGameObject != null)
		{
			UnityEngine.Object.Destroy(_eventTimerGameObject);
		}
		_eventTimerGameObject = new GameObject("EventTimers");
		UnityEngine.Object.DontDestroyOnLoad(_eventTimerGameObject);
		_eventTimerMono = _eventTimerGameObject.AddComponent<ResurfCoroutineRunner>();
		HierarchyUtility.ReparentByHierarchyPathType(_eventTimerGameObject.transform, HierarchyPathType.DynamicManagers);
		if (PlayerInfo.Instance.lastTimeAnEventWasStarted.CompareTo(num2) < 0)
		{
			if (num3.CompareTo(num2) < 0)
			{
				timeLeft = num2 - num3;
			}
			_eventTimerMono.StartCoroutine(TimerToEventStart(timeLeft));
		}
		if (num.CompareTo(0L) > 0)
		{
			timeLeft = num - num3;
			if (timeLeft.CompareTo(0L) > 0)
			{
				_eventTimerMono.StartCoroutine(TimerToEventFinish(timeLeft));
			}
		}
		SetCurrentEventManager();
	}

	private IEnumerator TimerToEventStart(long timeLeft)
	{
		float startPoint = 0f;
		while (startPoint < (float)timeLeft)
		{
			startPoint += RealTimeTracker.deltaTime;
			yield return null;
		}
		PlayerInfo.Instance.lastTimeAnEventWasStarted = (long)GetUnixCurrentTimestamp();
		SetCurrentEventManager();
		if (SSEventsManagerController.OnSSEventStartedHandler != null)
		{
			SSEventsManagerController.OnSSEventStartedHandler();
		}
	}

	private IEnumerator TimerToEventFinish(long timeLeft)
	{
		float startPoint = 0f;
		while (startPoint < (float)timeLeft)
		{
			startPoint += RealTimeTracker.deltaTime;
			yield return null;
		}
		SetCurrentEventManager();
		if (SSEventsManagerController.OnSSEventFinishedHandler != null)
		{
			SSEventsManagerController.OnSSEventFinishedHandler();
		}
	}

	private long GetCurrentEventID()
	{
		if (Globals.DEBUG_USE_EVENT_TESTER)
		{
			return EventTesterManager.Instance.GetEventID();
		}
		string valueString = string.Empty;
		if (OnlineSettings.instance.TryGetValue("event_id", out valueString))
		{
			long result = 0L;
			if (long.TryParse(valueString, out result))
			{
				return result;
			}
		}
		return 0L;
	}

	private long EndTimestampForCurrentEventType()
	{
		if (Globals.DEBUG_USE_EVENT_TESTER)
		{
			return EventTesterManager.Instance.GetEventEndTime();
		}
		string valueString = string.Empty;
		if (OnlineSettings.instance.TryGetValue("event_end", out valueString))
		{
			long result = 0L;
			if (long.TryParse(valueString, out result))
			{
				return result;
			}
		}
		return 0L;
	}

	private long StartTimestampForCurrentEventType()
	{
		if (Globals.DEBUG_USE_EVENT_TESTER)
		{
			return EventTesterManager.Instance.GetEventStartTime();
		}
		string valueString = string.Empty;
		if (OnlineSettings.instance.TryGetValue("event_start", out valueString))
		{
			long result = 0L;
			if (long.TryParse(valueString, out result))
			{
				return result;
			}
		}
		return 0L;
	}

	private double GetUnixCurrentTimestamp()
	{
		return (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
	}

	private bool IsAnyEventEnabled()
	{
		long num = EndTimestampForCurrentEventType();
		if (num > 0)
		{
			long num2 = StartTimestampForCurrentEventType();
			double unixCurrentTimestamp = GetUnixCurrentTimestamp();
			if (num2 > 0)
			{
				return (double)num2 < unixCurrentTimestamp && unixCurrentTimestamp < (double)num;
			}
			return unixCurrentTimestamp < (double)num;
		}
		return false;
	}

	private SSEventType GetEnabledEventType()
	{
		if (IsAnyEventEnabled())
		{
			if (Globals.DEBUG_USE_EVENT_TESTER)
			{
				return EventTesterManager.Instance.GetEventType();
			}
			string valueString = string.Empty;
			if (OnlineSettings.instance.TryGetValue("event_type", out valueString))
			{
				try
				{
					return (SSEventType)(int)Enum.Parse(typeof(SSEventType), valueString, true);
				}
				catch (ArgumentException ex)
				{
					Debug.LogWarning("Wrong online setting: " + ex.StackTrace);
				}
			}
		}
		return SSEventType.None;
	}

	private void SetCurrentEventManager()
	{
		if (_currentEventManager != null)
		{
			_currentEventManager.UnHookFromGameEvents();
		}
		SSEventType enabledEventType = GetEnabledEventType();
		long currentEventID = GetCurrentEventID();
		if (_currentEventManager == null || _currentEventManager.GetEventID() != currentEventID || enabledEventType != _currentEventManager.GetEventType())
		{
			if (enabledEventType != SSEventType.CharacterName)
			{
				AwardsData.ResetWordyWeekendChallengeOneWeekend();
			}
			switch (enabledEventType)
			{
			case SSEventType.BiggerJackpot:
				_currentEventManager = new BiggerJackpotEventManager(enabledEventType, currentEventID);
				break;
			case SSEventType.CharacterDay:
				_currentEventManager = new CharacterDayEventManager(enabledEventType, currentEventID);
				break;
			case SSEventType.CharacterName:
				_currentEventManager = new CharacterNameEventManager(enabledEventType, currentEventID);
				break;
			case SSEventType.ExpiredLimitedBoard:
				_currentEventManager = new ExpiredLimitedBoardEventManager(enabledEventType, currentEventID);
				break;
			case SSEventType.ExpiredLimitedCharacter:
				_currentEventManager = new ExpiredLimitedCharacterEventManager(enabledEventType, currentEventID);
				break;
			case SSEventType.MysteryPowerup:
				_currentEventManager = new MysteryPowerupEventManager(enabledEventType, currentEventID);
				break;
			case SSEventType.SpecialPowerBoard:
				_currentEventManager = new SpecialPowerBoardEventManager(enabledEventType, currentEventID);
				break;
			case SSEventType.SuperMysteryBox:
				_currentEventManager = new SuperMysteryBoxEventManager(enabledEventType, currentEventID);
				break;
			default:
				_currentEventManager = null;
				break;
			}
		}
		if (SSEventsManagerController.OnSSEventChangedHandler != null)
		{
			SSEventsManagerController.OnSSEventChangedHandler();
		}
	}
}
