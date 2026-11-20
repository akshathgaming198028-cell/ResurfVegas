using System;
using UnityEngine;

public class LocalNotificationsManager
{
	public const int TIME_OF_NOTIFICATION = 17;

	public const int TIME_OF_NOTIFICATION_DAILY_VIDEO = 19;

	public const string ONLINE_SETTING = "enable_local_notifications";

	private static LocalNotificationsManager _instance;

	private static ILocalNotificationController _notificationCtrl;

	public ILocalNotificationController NotificationController
	{
		get
		{
			return _notificationCtrl;
		}
	}

	public static LocalNotificationsManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new LocalNotificationsManager();
				if (Application.platform == RuntimePlatform.Android)
				{
					_notificationCtrl = new AndroidLocalNotificationsController();
				}
				else if (Application.platform == RuntimePlatform.IPhonePlayer)
				{
					_notificationCtrl = new IOSLocalNotificationsController();
				}
				else
				{
					_notificationCtrl = new EditorLocalNotificationCtrl();
				}
			}
			return _instance;
		}
	}

	public DateTime GetNextNotificationTimeForVideos()
	{
		return DateTime.Today.AddHours(19.0);
	}
}
