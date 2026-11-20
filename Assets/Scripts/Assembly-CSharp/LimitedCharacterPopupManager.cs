using System;
using System.Collections;
using Kiloo.Common;
using SYBO.Subway.Info;

public class LimitedCharacterPopupManager
{
	private const Characters.CharacterType _currentCharacter = Characters.CharacterType.lasVegasCharacter01;

	private int _currentThemeIndex;

	private static LimitedCharacterPopupManager _instance;

	private bool _isInitialized;

	private IEnumerator _startTimerCoroutine;

	private IEnumerator _endTimerCoroutine;

	public Characters.CharacterType currentCharacter
	{
		get
		{
			if (Globals.DEBUG_LIMITED_CHARACTER_POPUP)
			{
				return Globals.DEBUG_LIMITED_CHARACTER_POPUP_TYPE;
			}
			return Characters.CharacterType.lasVegasCharacter01;
		}
	}

	public int currentThemeIndex
	{
		get
		{
			return _currentThemeIndex;
		}
		set
		{
			_currentThemeIndex = value;
		}
	}

	public static LimitedCharacterPopupManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new LimitedCharacterPopupManager();
			}
			return _instance;
		}
	}

	public static event Action OnLimitedCharacterStartedHandler;

	public static event Action OnLimitedCharacterExpiredHandler;

	private LimitedCharacterPopupManager()
	{
		if (!_isInitialized)
		{
			Initialize();
		}
	}

	private void Initialize()
	{
		if (!_isInitialized)
		{
			UIScreenController.OnApplicationResumed += StartTimer;
			StartTimer();
			_isInitialized = true;
		}
	}

	public bool IsCharacterAvailable()
	{
		if (Globals.DEBUG_LIMITED_CHARACTER_POPUP_IGNORE_EXPIRE)
		{
			return true;
		}
		DateTime value = PlayerInfo.Instance.CharacterStartAvailabilityDate(currentCharacter);
		DateTime value2 = PlayerInfo.Instance.CharacterExpirationDate(currentCharacter);
		DateTime now = DateTime.Now;
		return now.CompareTo(value) > 0 && now.CompareTo(value2) < 0;
	}

	public bool ShouldButtonBeVisible()
	{
		return !PlayerInfo.Instance.IsCollectionComplete(currentCharacter) && IsCharacterAvailable();
	}

	public bool CurrentlyShownCharacterMatchesEventItem()
	{
		bool result = false;
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
		if (activeEventManager != null)
		{
			SSEventType eventType = activeEventManager.GetEventType();
			if (eventType == SSEventType.ExpiredLimitedCharacter)
			{
				bool found = false;
				Characters.CharacterType? characterType = activeEventManager.GetEventValue<Characters.CharacterType>(out found);
				if (found && characterType.GetValueOrDefault() == UIModelController.Instance.currentlyShownModel && characterType.HasValue)
				{
					result = true;
				}
			}
		}
		return result;
	}

	private void StartTimer()
	{
		DateTime dateTime = PlayerInfo.Instance.CharacterStartAvailabilityDate(currentCharacter);
		DateTime dateTime2 = PlayerInfo.Instance.CharacterExpirationDate(currentCharacter);
		DateTime now = DateTime.Now;
		CoroutineUtil.StopCoroutine(_startTimerCoroutine);
		CoroutineUtil.StopCoroutine(_endTimerCoroutine);
		if (now.CompareTo(dateTime) < 0)
		{
			float timeLeft = (float)(dateTime - now).TotalSeconds;
			_startTimerCoroutine = Timer(timeLeft, LimitedCharacterPopupManager.OnLimitedCharacterStartedHandler);
			CoroutineUtil.StartCoroutine(_startTimerCoroutine);
		}
		if (now.CompareTo(dateTime2) < 0)
		{
			float timeLeft2 = (float)(dateTime2 - now).TotalSeconds;
			_endTimerCoroutine = Timer(timeLeft2, LimitedCharacterPopupManager.OnLimitedCharacterExpiredHandler);
			CoroutineUtil.StartCoroutine(_endTimerCoroutine);
		}
	}

	private IEnumerator Timer(float timeLeft, Action handler)
	{
		yield return null;
		yield return null;
		float startPoint = 0f;
		while (startPoint < timeLeft)
		{
			startPoint += RealTimeTracker.deltaTime;
			yield return null;
		}
		if (handler != null)
		{
			handler();
		}
	}
}
