using System;
using System.Collections;
using Kiloo.Common;

public class LimitedBoardPopupManager
{
	private const Hoverboards.BoardType _currentBoardType = Hoverboards.BoardType.lasVegasBoard01;

	private static LimitedBoardPopupManager _instance;

	private bool _isInitialized;

	private IEnumerator _startTimerCoroutine;

	private IEnumerator _endTimerCoroutine;

	public Hoverboards.BoardType currentBoardType
	{
		get
		{
			if (Globals.DEBUG_LIMITED_BOARD_POPUP)
			{
				return Globals.DEBUG_LIMITED_BOARD_POPUP_TYPE;
			}
			return Hoverboards.BoardType.lasVegasBoard01;
		}
	}

	public int currentlyShownThemeIndex { get; set; }

	public static LimitedBoardPopupManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new LimitedBoardPopupManager();
			}
			return _instance;
		}
	}

	public static event Action OnLimitedBoardStartedHandler;

	public static event Action OnLimitedBoardExpiredHandler;

	private LimitedBoardPopupManager()
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

	public bool IsBoardAvailable()
	{
		if (Globals.DEBUG_LIMITED_BOARD_POPUP_IGNORE_EXPIRE)
		{
			return true;
		}
		DateTime value = PlayerInfo.Instance.BoardExpirationDate(currentBoardType);
		DateTime value2 = PlayerInfo.Instance.BoardStartAvailabilityDate(currentBoardType);
		DateTime now = PlayerInfo.Instance.LimitedBoardNow(currentBoardType);
		return now.CompareTo(value2) > 0 && now.CompareTo(value) < 0;
	}

	public bool ShouldButtonBeVisible()
	{
		bool flag = LimitedCharacterPopupManager.Instance.ShouldButtonBeVisible();
		bool flag2 = UIScreenController.Instance.GetCurrentPopupName() == "LimitedCharacterPopup";
		bool flag3 = PlayerInfo.Instance.isHoverboardUnlocked(currentBoardType);
		return !flag && !flag2 && !flag3 && IsBoardAvailable();
	}

	private void StartTimer()
	{
		DateTime dateTime = PlayerInfo.Instance.BoardStartAvailabilityDate(currentBoardType);
		DateTime dateTime2 = PlayerInfo.Instance.BoardExpirationDate(currentBoardType);
		DateTime now = PlayerInfo.Instance.LimitedBoardNow(currentBoardType);
		CoroutineUtil.StopCoroutine(_startTimerCoroutine);
		CoroutineUtil.StopCoroutine(_endTimerCoroutine);
		if (now.CompareTo(dateTime) < 0)
		{
			float timeLeft = (float)(dateTime - now).TotalSeconds;
			_startTimerCoroutine = Timer(timeLeft, LimitedBoardPopupManager.OnLimitedBoardStartedHandler);
			CoroutineUtil.StartCoroutine(_startTimerCoroutine);
		}
		if (now.CompareTo(dateTime2) < 0)
		{
			float timeLeft2 = (float)(dateTime2 - now).TotalSeconds;
			_endTimerCoroutine = Timer(timeLeft2, LimitedBoardPopupManager.OnLimitedBoardExpiredHandler);
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
