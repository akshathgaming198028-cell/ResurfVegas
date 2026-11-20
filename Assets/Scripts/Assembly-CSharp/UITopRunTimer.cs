using System;
using TopRun;
using UnityEngine;

public class UITopRunTimer : MonoBehaviour
{
	[SerializeField]
	private UILabel _currentTopRunWeek;

	[SerializeField]
	private UILabel _timerLabel;

	[SerializeField]
	private string _prefix = string.Empty;

	[SerializeField]
	private PauseScreen pauseScreen;

	private bool _lessThanAdayLeft;

	private int _totalSeconds = -1;

	private IManager _tournamentManager;

	private ITournament _currentTournament;

	public void DoTimer()
	{
		_tournamentManager = TopRunRoot.manager;
		if (_tournamentManager != null)
		{
			if (_tournamentManager.tournament != null)
			{
				SetTextTopRunTimerLabels();
			}
			else
			{
				pauseScreen.FetchingTopRunDataTimedOut();
			}
		}
		else
		{
			pauseScreen.FetchingTopRunDataTimedOut();
		}
	}

	private void SetTextTopRunTimerLabels()
	{
		if (_tournamentManager != null && _tournamentManager.tournament != null)
		{
			_currentTournament = _tournamentManager.tournament;
			TimeSpan timeSpan = _currentTournament.officialEndTimeUTC - DateTime.UtcNow;
			if (DateTime.UtcNow > _currentTournament.officialEndTimeUTC)
			{
				_lessThanAdayLeft = false;
				_timerLabel.text = Strings.Get(StringID.TOPRUN_STATE_NEXT_SOON);
				_timerLabel.fontSize = 15;
			}
			else if (timeSpan.Days > 1)
			{
				_lessThanAdayLeft = false;
				_timerLabel.text = timeSpan.Days + " " + Strings.Get(StringID.TOPRUN_STATE_DAYS_LEFT);
				_timerLabel.fontSize = 24;
			}
			else if (timeSpan.Days == 1)
			{
				_lessThanAdayLeft = false;
				_timerLabel.text = timeSpan.Days + " " + Strings.Get(StringID.TOPRUN_STATE_DAYS_LEFT_SINGLE);
				_timerLabel.fontSize = 24;
			}
			else
			{
				_timerLabel.fontSize = 24;
				_lessThanAdayLeft = true;
			}
			_currentTopRunWeek.text = Strings.Get(StringID.TOPRUN_STATE_WEEK) + " " + _tournamentManager.tournament.sequenceId;
		}
		else
		{
			pauseScreen.FetchingTopRunDataTimedOut();
		}
	}

	private void Update()
	{
		if (!_lessThanAdayLeft)
		{
			return;
		}
		TimeSpan timeSpan = _currentTournament.officialEndTimeUTC - DateTime.UtcNow;
		int num = (int)timeSpan.TotalSeconds;
		if (_totalSeconds != num)
		{
			_totalSeconds = num;
			if (timeSpan.Ticks > 0)
			{
				_timerLabel.text = string.Format("{0}{1:00}:{2:00}:{3:00}", _prefix, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
			}
			else
			{
				_lessThanAdayLeft = false;
				_timerLabel.text = Strings.Get(StringID.TOPRUN_STATE_NEXT_SOON);
				_timerLabel.fontSize = 15;
			}
		}
	}
}
