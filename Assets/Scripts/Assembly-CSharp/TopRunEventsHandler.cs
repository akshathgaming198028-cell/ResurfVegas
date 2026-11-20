using System;
using TopRun;

public class TopRunEventsHandler
{
	private static TopRunEventsHandler _instance;

	public static TopRunEventsHandler instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new TopRunEventsHandler();
				TopRunRoot.manager.tournamentChanged += instance.TriggerOnTopRunChanged;
			}
			return _instance;
		}
	}

	public event EventHandler<TournamentChangedEventArgs> OnTopRunChangedEvent;

	private TopRunEventsHandler()
	{
	}

	public virtual void TriggerOnTopRunChanged(object o, TournamentChangedEventArgs eventArg = null)
	{
		if (eventArg == null)
		{
			TournamentChangedEventArgs e = new TournamentChangedEventArgs();
			e.tournamentWasUpdated = true;
			eventArg = e;
		}
		if (this.OnTopRunChangedEvent != null)
		{
			this.OnTopRunChangedEvent(this, eventArg);
		}
	}
}
