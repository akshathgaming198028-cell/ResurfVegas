namespace Kiloo.HTTP
{
	public enum RequestState
	{
		Waiting = 0,
		Reading = 1,
		Done = 2,
		Canceled = 3,
		TimedOut = 4,
		Error = 5
	}
}
