using System;
using UnityEngine;

public abstract class VideoAdProvider
{
	protected Action<VideoAdProvider> _onVideoAvailable;

	protected Action<VideoAdProvider, string> _onVideoUnavailable;

	protected Action<VideoAdProvider> _onTakeoverBegan;

	protected Action<VideoAdProvider> _onTakeoverEnded;

	protected Action<VideoAdProvider> _onVideoWatched;

	protected Action<VideoAdProvider> _onCachingTimedOut;

	public abstract bool isInitialized { get; }

	public abstract bool IsStopped { get; }

	public abstract string id { get; set; }

	public abstract int weight { get; set; }

	public abstract int priority { get; set; }

	protected void InvokeHandlerIfNotNull(Action handler)
	{
		if (handler != null)
		{
			handler();
		}
	}

	protected void InvokeHandlerIfNotNull<T>(Action<T> handler, T val)
	{
		if (handler != null)
		{
			handler(val);
		}
	}

	protected void InvokeHandlerIfNotNull<T>(Action<T, string> handler, T val, string extraInfo = "")
	{
		if (handler != null)
		{
			handler(val, extraInfo);
		}
	}

	protected void InvokeHandlerIfNotNull<T1, T2>(Action<T1, T2> handler, T1 val1, T2 val2)
	{
		if (handler != null)
		{
			handler(val1, val2);
		}
	}

	public abstract void Init();

	public abstract bool PlayVideoIfAvailable();

	public abstract void StopProvider();

	public abstract void ResumeProvider();

	public abstract void StartCachingVideo();

	public static void SetVideoProviderData(string OnlineSettings_ID, int Default_Priority, int Default_Weight, ref int priority, ref int weight)
	{
		string valueString = string.Empty;
		if (OnlineSettings.instance.TryGetValue(OnlineSettings_ID, out valueString))
		{
			if (string.IsNullOrEmpty(valueString))
			{
				priority = Default_Priority;
				weight = Default_Weight;
				return;
			}
			string[] array = valueString.Split(';');
			if (array.Length > 1)
			{
				if (!int.TryParse(array[0], out priority))
				{
					priority = Default_Priority;
					Debug.Log("Priority input string is not a sequence of digits");
				}
				if (!int.TryParse(array[1], out weight))
				{
					weight = Default_Weight;
					Debug.Log("Weight input string is not a sequence of digits");
				}
			}
			else
			{
				priority = Default_Priority;
				weight = Default_Weight;
			}
		}
		else
		{
			priority = Default_Priority;
			weight = Default_Weight;
		}
	}

	public void AddVideoAvailableHandler(Action<VideoAdProvider> handler)
	{
		_onVideoAvailable = (Action<VideoAdProvider>)Delegate.Combine(_onVideoAvailable, handler);
	}

	public void RemoveVideoAvailableHandler(Action<VideoAdProvider> handler)
	{
		_onVideoAvailable = (Action<VideoAdProvider>)Delegate.Remove(_onVideoAvailable, handler);
	}

	public void AddVideoUnavailableHandler(Action<VideoAdProvider, string> handler)
	{
		_onVideoUnavailable = (Action<VideoAdProvider, string>)Delegate.Combine(_onVideoUnavailable, handler);
	}

	public void RemoveVideoUnavailableHandler(Action<VideoAdProvider, string> handler)
	{
		_onVideoUnavailable = (Action<VideoAdProvider, string>)Delegate.Remove(_onVideoUnavailable, handler);
	}

	public void AddTakeoverBeganHandler(Action<VideoAdProvider> handler)
	{
		_onTakeoverBegan = (Action<VideoAdProvider>)Delegate.Combine(_onTakeoverBegan, handler);
	}

	public void RemoveTakeoverBeganHandler(Action<VideoAdProvider> handler)
	{
		_onTakeoverBegan = (Action<VideoAdProvider>)Delegate.Remove(_onTakeoverBegan, handler);
	}

	public void AddTakeoverEndedHandler(Action<VideoAdProvider> handler)
	{
		_onTakeoverEnded = (Action<VideoAdProvider>)Delegate.Combine(_onTakeoverEnded, handler);
	}

	public void RemoveTakeoverEndedHandler(Action<VideoAdProvider> handler)
	{
		_onTakeoverEnded = (Action<VideoAdProvider>)Delegate.Remove(_onTakeoverEnded, handler);
	}

	public void AddVideoWatchedHandler(Action<VideoAdProvider> handler)
	{
		_onVideoWatched = (Action<VideoAdProvider>)Delegate.Combine(_onVideoWatched, handler);
	}

	public void RemoveVideoWatchedHandler(Action<VideoAdProvider> handler)
	{
		_onVideoWatched = (Action<VideoAdProvider>)Delegate.Remove(_onVideoWatched, handler);
	}

	public void AddCachingTimedOutHandler(Action<VideoAdProvider> handler)
	{
		_onCachingTimedOut = (Action<VideoAdProvider>)Delegate.Combine(_onCachingTimedOut, handler);
	}

	public void RemoveCachingTimedOutHandler(Action<VideoAdProvider> handler)
	{
		_onCachingTimedOut = (Action<VideoAdProvider>)Delegate.Remove(_onCachingTimedOut, handler);
	}

	public virtual void OnPause()
	{
	}

	public virtual void OnResume()
	{
	}
}
