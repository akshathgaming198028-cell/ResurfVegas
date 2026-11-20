using System;
using TopRun;
using UnityEngine;

public class TopRunNormalStateHelper : MonoBehaviour
{
	public enum NormalStateType
	{
		Ongoing = 0,
		WaitingForResults = 1,
		Over = 2
	}

	private const string zeroClock = "00:00:00";

	[SerializeField]
	private UISprite _background;

	[SerializeField]
	private UILabel _weekLabel;

	[SerializeField]
	private UILabel _statusLabel;

	[SerializeField]
	private UILabel _numberLabel;

	[SerializeField]
	private UILabel _nextSoonLabel;

	private ITournament currentTournament;

	private int hoursThreshold = 100;

	public NormalStateType currentState { get; private set; }

	public void InitNormalState()
	{
		IManager manager = TopRunRoot.manager;
		if (manager != null && manager.tournament != null)
		{
			ITournament tournament = (currentTournament = manager.tournament);
			if (DateTime.UtcNow > tournament.officialEndTimeUTC)
			{
				if (!tournament.hasResult)
				{
					if (manager.score <= 0)
					{
						SetOverState(tournament);
					}
					else
					{
						SetWaitingForResultsState(tournament);
					}
				}
				else if (manager.score <= 0 || (manager.score > 0 && !TopRunRewardHelper.ShouldShowResults()))
				{
					SetOverState(tournament);
				}
				else
				{
					SetOverState(tournament);
				}
			}
			else if (tournament.isFinished)
			{
				SetOverState(tournament);
			}
			else
			{
				SetOngoingState(tournament);
			}
		}
		else
		{
			Debug.LogError("Normal State triggered while no Tournament Data is available.");
		}
	}

	private void Update()
	{
		if (currentTournament == null)
		{
			return;
		}
		switch (currentState)
		{
		case NormalStateType.Ongoing:
		{
			bool lessThanAdayLeft = false;
			_numberLabel.text = string.Empty + TournamentTimeLeft(currentTournament.officialEndTimeUTC, _statusLabel, out lessThanAdayLeft);
			_numberLabel.pivot = UIWidget.Pivot.Right;
			UIAnchor component = _numberLabel.GetComponent<UIAnchor>();
			component.container = _statusLabel.gameObject;
			component.side = UIAnchor.Side.Left;
			if (lessThanAdayLeft)
			{
				_numberLabel.pivot = UIWidget.Pivot.Left;
				component.pixelOffset = new Vector2(-66f, 0f);
			}
			else
			{
				component.pixelOffset = new Vector2(-2f, 0f);
			}
			DateTime officialEndTimeUTC = currentTournament.officialEndTimeUTC;
			if (DateTime.UtcNow > officialEndTimeUTC)
			{
				if (TopRunRoot.manager.score <= 0)
				{
					SetOverState(currentTournament);
				}
				else
				{
					SetWaitingForResultsState(currentTournament);
				}
			}
			break;
		}
		case NormalStateType.WaitingForResults:
			if (currentTournament.hasResult)
			{
				TopRunRewardHelper.RefreshPendingResults();
			}
			break;
		case NormalStateType.Over:
		{
			bool pastNextTournamentStart = false;
			_numberLabel.text = TimeUntilNextTournament(TopRunRoot.manager.nextTournamentFetchTime, out pastNextTournamentStart);
			if (pastNextTournamentStart)
			{
				SetNextTournamentSoon();
				break;
			}
			_nextSoonLabel.gameObject.SetActive(false);
			_numberLabel.gameObject.SetActive(true);
			_statusLabel.gameObject.SetActive(true);
			break;
		}
		default:
			Debug.LogError("No avilable handler for case " + currentState);
			break;
		}
	}

	private void SetOngoingState(ITournament tournament)
	{
		currentState = NormalStateType.Ongoing;
		_nextSoonLabel.gameObject.SetActive(false);
		_weekLabel.text = Strings.Get(StringID.TOPRUN_STATE_WEEK) + " " + tournament.sequenceId;
		_statusLabel.gameObject.SetActive(true);
		_statusLabel.pivot = UIWidget.Pivot.Right;
		UIAnchor component = _statusLabel.GetComponent<UIAnchor>();
		component.container = _background.gameObject;
		component.side = UIAnchor.Side.Right;
		component.pixelOffset = new Vector2(-7f, -1f);
		bool lessThanAdayLeft = false;
		_numberLabel.gameObject.SetActive(true);
		_numberLabel.text = string.Empty + TournamentTimeLeft(tournament.officialEndTimeUTC, _statusLabel, out lessThanAdayLeft);
		_numberLabel.pivot = UIWidget.Pivot.Right;
		UIAnchor component2 = _numberLabel.GetComponent<UIAnchor>();
		component2.container = _statusLabel.gameObject;
		component2.side = UIAnchor.Side.Left;
		if (lessThanAdayLeft)
		{
			_numberLabel.pivot = UIWidget.Pivot.Left;
			component2.pixelOffset = new Vector2(-66f, 0f);
		}
		else
		{
			component2.pixelOffset = new Vector2(-2f, 0f);
		}
	}

	private void SetWaitingForResultsState(ITournament tournament)
	{
		currentState = NormalStateType.WaitingForResults;
		_nextSoonLabel.gameObject.SetActive(false);
		_weekLabel.text = Strings.Get(StringID.TOPRUN_STATE_WEEK) + " " + tournament.sequenceId;
		_statusLabel.gameObject.SetActive(true);
		_statusLabel.text = Strings.Get(StringID.TOPRUN_STATE_WAIT_FOR_RESULTS);
		_statusLabel.pivot = UIWidget.Pivot.Right;
		UIAnchor component = _statusLabel.GetComponent<UIAnchor>();
		component.container = _background.gameObject;
		component.side = UIAnchor.Side.Right;
		component.pixelOffset = new Vector2(-7f, -1f);
		_numberLabel.gameObject.SetActive(false);
	}

	private void SetOverState(ITournament tournament)
	{
		currentState = NormalStateType.Over;
		_nextSoonLabel.gameObject.SetActive(false);
		_weekLabel.text = Strings.Get(StringID.TOPRUN_STATE_WEEK) + " " + tournament.sequenceId;
		_numberLabel.gameObject.SetActive(true);
		bool pastNextTournamentStart = false;
		_numberLabel.text = TimeUntilNextTournament(TopRunRoot.manager.nextTournamentFetchTime, out pastNextTournamentStart);
		_numberLabel.pivot = UIWidget.Pivot.Left;
		UIAnchor component = _numberLabel.GetComponent<UIAnchor>();
		component.container = _background.gameObject;
		component.side = UIAnchor.Side.Right;
		component.pixelOffset = new Vector2(-68f, -1f);
		TimeSpan timeSpan = TopRunRoot.manager.nextTournamentFetchTime - DateTime.UtcNow;
		if (pastNextTournamentStart || timeSpan.TotalHours > (double)hoursThreshold)
		{
			SetNextTournamentSoon();
			return;
		}
		_statusLabel.text = Strings.Get(StringID.TOPRUN_STATE_NEXT_IN);
		_statusLabel.pivot = UIWidget.Pivot.Right;
		UIAnchor component2 = _statusLabel.GetComponent<UIAnchor>();
		component2.container = _background.gameObject;
		component2.side = UIAnchor.Side.Right;
		component2.pixelOffset = new Vector2(-74f, -1f);
	}

	private void SetNextTournamentSoon()
	{
		_numberLabel.gameObject.SetActive(false);
		_statusLabel.gameObject.SetActive(false);
		_nextSoonLabel.gameObject.SetActive(true);
		_nextSoonLabel.text = Strings.Get(StringID.TOPRUN_STATE_NEXT_SOON);
	}

	private string TournamentTimeLeft(DateTime officialTournamentEnd, UILabel statusLabel, out bool lessThanAdayLeft)
	{
		TimeSpan timeSpan = officialTournamentEnd - DateTime.UtcNow;
		if (DateTime.UtcNow > officialTournamentEnd)
		{
			lessThanAdayLeft = false;
			return "00:00:00";
		}
		if (timeSpan.Days > 1)
		{
			lessThanAdayLeft = false;
			statusLabel.text = Strings.Get(StringID.TOPRUN_STATE_DAYS_LEFT);
			return string.Empty + timeSpan.Days;
		}
		if (timeSpan.Days == 1)
		{
			lessThanAdayLeft = false;
			statusLabel.text = Strings.Get(StringID.TOPRUN_STATE_DAYS_LEFT_SINGLE);
			return string.Empty + timeSpan.Days;
		}
		lessThanAdayLeft = true;
		statusLabel.text = Strings.Get(StringID.TOPRUN_STATE_TIME_LEFT);
		return GetTimeLeftInHHMMSS(timeSpan);
	}

	private string TimeUntilNextTournament(DateTime nextTournamentStartTime, out bool pastNextTournamentStart)
	{
		TimeSpan timeSpan = nextTournamentStartTime - DateTime.UtcNow;
		if (timeSpan.Days > 99)
		{
			pastNextTournamentStart = true;
			return string.Empty;
		}
		if (DateTime.UtcNow > nextTournamentStartTime)
		{
			pastNextTournamentStart = true;
			return "00:00:00";
		}
		pastNextTournamentStart = false;
		return GetTimeLeftInHHMMSS(timeSpan);
	}

	private string GetTimeLeftInHHMMSS(TimeSpan timeSpan)
	{
		int num = timeSpan.Hours + timeSpan.Days * 24;
		string text = ((num >= 10) ? (string.Empty + num) : ("0" + num));
		string text2 = ((timeSpan.Minutes >= 10) ? (string.Empty + timeSpan.Minutes) : ("0" + timeSpan.Minutes));
		string text3 = ((timeSpan.Seconds >= 10) ? (string.Empty + timeSpan.Seconds) : ("0" + timeSpan.Seconds));
		return text + ":" + text2 + ":" + text3;
	}
}
