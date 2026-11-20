using System;
using UnityEngine;

public class FacebookWeeklyGiftManager
{
	private const string SHOW_GIFT_POPUP = "show_facebook_gift_popup";

	private PlayerInfo playerInfoInstance = PlayerInfo.Instance;

	private static FacebookWeeklyGiftManager _instance;

	public static FacebookWeeklyGiftManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new FacebookWeeklyGiftManager();
			}
			return _instance;
		}
	}

	private FacebookWeeklyGiftManager()
	{
		CheckAndUpdateNextWeeklyGiftDate();
	}

	public void PopupDidShow()
	{
		PlayerPrefs.SetInt("show_facebook_gift_popup", 0);
	}

	public bool ShouldDisplayPopup()
	{
		return PlayerPrefs.GetInt("show_facebook_gift_popup", 0) != 0 && IsGiftAvailable();
	}

	public void GiftWasClaimed()
	{
		playerInfoInstance.hasCollectedFacebookWeeklyGiftThisWeek = true;
	}

	public void CheckAndUpdateNextWeeklyGiftDate()
	{
		if (!(playerInfoInstance.lastKnownServerTime == DateTime.MinValue))
		{
			TryCalculateAndSetNextGiftDeadlineIfNeeded();
		}
	}

	public void TryCalculateAndSetNextGiftDeadlineIfNeeded()
	{
		DateTime nextWeekday = GetNextWeekday(playerInfoInstance.lastKnownServerTime, DayOfWeek.Saturday);
		DateTime dateTime = new DateTime(nextWeekday.Year, nextWeekday.Month, nextWeekday.Day, 11, 59, 59);
		if (playerInfoInstance.weeklyGiftEndTime != dateTime)
		{
			playerInfoInstance.weeklyGiftEndTime = dateTime;
			playerInfoInstance.hasCollectedFacebookWeeklyGiftThisWeek = false;
			PlayerPrefs.SetInt("show_facebook_gift_popup", 1);
		}
	}

	public static DateTime GetNextWeekday(DateTime startDate, DayOfWeek weekday)
	{
		int num = (weekday - startDate.DayOfWeek + 7) % 7;
		if (num == 0 && DateTime.Compare(t2: new DateTime(startDate.Year, startDate.Month, startDate.Day, 12, 0, 0), t1: startDate) >= 0)
		{
			num = 7;
		}
		return startDate.AddDays(num);
	}

	public bool IsGiftAvailable()
	{
		return playerInfoInstance.weeklyGiftEndTime != DateTime.MinValue && !playerInfoInstance.hasCollectedFacebookWeeklyGiftThisWeek && SocialManager.instance.facebookIsLoggedIn;
	}
}
