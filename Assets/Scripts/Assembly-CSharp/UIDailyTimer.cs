using System;
using UnityEngine;

public class UIDailyTimer : MonoBehaviour
{
	[SerializeField]
	private UILabel _timerLabel;

	[SerializeField]
	private string prefix = string.Empty;

	private bool doARefresh;

	private int _totalSeconds = -1;

	private void Update()
	{
		TimeSpan timeSpan = PlayerInfo.Instance.dailyWordTimeLeft;
		if (timeSpan.Ticks <= 0)
		{
			timeSpan = PlayerInfo.Instance.dailyWordDeviceTiemLeft;
		}
		int num = (int)timeSpan.TotalSeconds;
		if (_totalSeconds == num)
		{
			return;
		}
		_totalSeconds = num;
		if (timeSpan.Ticks > 0)
		{
			_timerLabel.text = string.Format("{0} {1:00}:{2:00}:{3:00}", prefix, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
			return;
		}
		_timerLabel.text = string.Format("{0} {1:00}:{2:00}:{3:00}", prefix, 0, 0, 0);
		if (doARefresh)
		{
			SendMessageUpwards("RefreshDailyChallenge", SendMessageOptions.DontRequireReceiver);
			doARefresh = false;
		}
	}

	private void OnApplicationPause(bool paused)
	{
		if (!paused)
		{
			doARefresh = true;
		}
	}
}
