using System;
using UnityEngine;

public class EditorLocalNotificationCtrl : ILocalNotificationController
{
	public void ScheduleLocalNotification(DateTime date, string title, string message, string soundName)
	{
		Debug.Log("Local notification was scheduled for " + date.ToString() + " with message: " + message);
	}

	public void CancelAllNotifications()
	{
	}

	public void EnableLogs(bool enable)
	{
	}
}
