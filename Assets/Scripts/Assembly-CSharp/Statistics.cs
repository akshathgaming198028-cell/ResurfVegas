using System;
using System.Diagnostics;
using UnityEngine;

public class Statistics
{
	public delegate void StatChangedDelegate(Stat stat, int newValue);

	public delegate void TempStatChangedDelegate(TempStat tempStat, int newValue);

	public bool dirty;

	private int[] _values;

	private StatChangedDelegate[] _statChangedHandlers;

	private int[] _tempStatValues;

	private TempStatChangedDelegate[] _tempStatChangedHandlers;

	public int this[Stat stat]
	{
		get
		{
			return _values[(int)stat];
		}
		set
		{
			if (_values[(int)stat] == value)
			{
				return;
			}
			_values[(int)stat] = value;
			dirty = true;
			if (_statChangedHandlers != null)
			{
				StatChangedDelegate statChangedDelegate = _statChangedHandlers[(int)stat];
				if (statChangedDelegate != null)
				{
					statChangedDelegate(stat, value);
				}
			}
		}
	}

	public int this[TempStat tempStat]
	{
		get
		{
			return _tempStatValues[(int)tempStat];
		}
		set
		{
			if (_values[(int)tempStat] == value)
			{
				return;
			}
			_tempStatValues[(int)tempStat] = value;
			dirty = true;
			if (_tempStatChangedHandlers != null)
			{
				TempStatChangedDelegate tempStatChangedDelegate = _tempStatChangedHandlers[(int)tempStat];
				if (tempStatChangedDelegate != null)
				{
					tempStatChangedDelegate(tempStat, value);
				}
			}
		}
	}

	public int this[string stat]
	{
		get
		{
			return _values[(int)Enum.Parse(typeof(Stat), stat, true)];
		}
		set
		{
			this[(Stat)(int)Enum.Parse(typeof(Stat), stat, true)] = value;
		}
	}

	public Statistics()
	{
		_values = new int[Enum.GetNames(typeof(Stat)).Length];
		dirty = false;
	}

	public bool AnyListenerForStat(Stat stat)
	{
		if (_statChangedHandlers != null)
		{
			StatChangedDelegate statChangedDelegate = _statChangedHandlers[(int)stat];
			if (statChangedDelegate != null)
			{
				return true;
			}
		}
		return false;
	}

	public void AddStatChangedHandler(Stat stat, StatChangedDelegate handler)
	{
		if (_statChangedHandlers == null)
		{
			_statChangedHandlers = new StatChangedDelegate[_values.Length];
		}
		StatChangedDelegate[] statChangedHandlers = _statChangedHandlers;
		Stat num = stat;
		statChangedHandlers[(int)num] = (StatChangedDelegate)Delegate.Combine(statChangedHandlers[(int)num], handler);
	}

	public void RemoveStatChangedHandler(Stat stat, StatChangedDelegate handler)
	{
		if (_statChangedHandlers != null)
		{
			StatChangedDelegate[] statChangedHandlers = _statChangedHandlers;
			Stat num = stat;
			statChangedHandlers[(int)num] = (StatChangedDelegate)Delegate.Remove(statChangedHandlers[(int)num], handler);
		}
	}

	public void Copy(Statistics src)
	{
		foreach (int value in Enum.GetValues(typeof(Stat)))
		{
			this[(Stat)value] = src[(Stat)value];
		}
	}

	public void Clear()
	{
		foreach (int value in Enum.GetValues(typeof(Stat)))
		{
			this[(Stat)value] = 0;
		}
	}

	public override string ToString()
	{
		int num = 0;
		int[] values = _values;
		for (int i = 0; i < values.Length; i++)
		{
			if (values[i] != 0)
			{
				num++;
			}
		}
		Stat[] array = (Stat[])Enum.GetValues(typeof(Stat));
		string[] array2 = new string[num];
		int num2 = 0;
		Stat[] array3 = array;
		foreach (Stat stat in array3)
		{
			if (this[stat] != 0)
			{
				array2[num2] = string.Format("{0}={1}", stat, this[stat]);
				num2++;
			}
		}
		return string.Join(";", array2);
	}

	public static Statistics Parse(string s)
	{
		Statistics statistics = new Statistics();
		if (s.Length > 0)
		{
			string[] array = s.Split(';');
			string[] array2 = array;
			foreach (string text in array2)
			{
				int num = text.IndexOf('=');
				string text2 = text.Substring(0, num);
				try
				{
					statistics._values[(int)Enum.Parse(typeof(Stat), text2, true)] = int.Parse(text.Substring(num + 1));
				}
				catch (ArgumentException)
				{
					Debug.LogWarning(string.Format("Unknown int stat \"{0}\"encountered while reading statistics: ", text2));
				}
			}
		}
		return statistics;
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	private static void LogError(string msg, UnityEngine.Object context = null)
	{
		Debug.LogError(msg, context);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	private static void LogWarning(string msg, UnityEngine.Object context = null)
	{
		Debug.LogWarning(msg, context);
	}

	[Conditional("STATISTICS_DEBUG_OUTPUT")]
	private static void Log(string msg, UnityEngine.Object context = null)
	{
		Debug.Log(msg, context);
	}
}
