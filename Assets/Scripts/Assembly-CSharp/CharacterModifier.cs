using System.Collections;
using UnityEngine;

public abstract class CharacterModifier : MonoBehaviour
{
	public enum StopSignal
	{
		DONT_STOP = 0,
		STOP = 1,
		STOP_NO_ENDING = 2
	}

	protected StopSignal stop;

	private bool _paused;

	private IEnumerator current;

	public bool Paused
	{
		get
		{
			return _paused;
		}
		protected set
		{
			_paused = value;
		}
	}

	public StopSignal Stop
	{
		get
		{
			return stop;
		}
		set
		{
			stop = value;
		}
	}

	public virtual bool ShouldPauseInJetpack
	{
		get
		{
			return false;
		}
	}

	public IEnumerator Current
	{
		get
		{
			return current;
		}
		set
		{
			current = value;
		}
	}

	public virtual void Prepare()
	{
	}

	public virtual IEnumerator Begin()
	{
		yield break;
	}

	public virtual void Pause()
	{
		_paused = true;
	}

	public virtual void Resume()
	{
		_paused = false;
	}

	public virtual void Reset()
	{
	}

	public virtual void ExpireSoon()
	{
	}
}
