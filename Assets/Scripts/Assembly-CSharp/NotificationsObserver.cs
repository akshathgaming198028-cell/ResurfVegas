#define ENABLE_ERROR_LOGS
using System.Collections.Generic;
using System.Diagnostics;
using MiniJSON;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class NotificationsObserver : MonoBehaviour
{
	public delegate void NotificationHandler(Notification notification);

	private const string TITLE_DICT_KEY = "title";

	private const string MESSAGE_DICT_KEY = "message";

	private const string TYPE_DICT_KEY = "type";

	private const string SOUNDNAME_DICT_KEY = "soundName";

	private static NotificationsObserver _instance;

	public static NotificationsObserver instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject gameObject = new GameObject("NotificationsObserver");
				gameObject.AddComponent<NotificationsObserver>();
				Object.DontDestroyOnLoad(gameObject);
			}
			return _instance;
		}
	}

	public event NotificationHandler onNotificationReceived;

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
			return;
		}
		LogError("There is more than one NotificationsObserver in the scene!");
		Object.Destroy(base.gameObject);
	}

	private void Start()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			GCMPlugin.onNotificationReceived += onAndroidNotificationReceived;
		}
	}

	private void onAndroidNotificationReceived(string payload)
	{
		Dictionary<string, object> dictionary = Json.Deserialize(payload) as Dictionary<string, object>;
		if (dictionary != null)
		{
			object value;
			dictionary.TryGetValue("title", out value);
			string title = value as string;
			dictionary.TryGetValue("message", out value);
			string message = value as string;
			dictionary.TryGetValue("type", out value);
			string type = value as string;
			alertListeners(title, message, type, string.Empty);
		}
	}

	private void alertListeners(string title, string message, string type, string soundName = "")
	{
		Notification notification = new Notification();
		notification.type = type;
		notification.message = message;
		NotificationHandler notificationHandler = this.onNotificationReceived;
		if (notificationHandler != null)
		{
			notificationHandler(notification);
		}
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	public static void LogError(string msg, Object context = null)
	{
		Debug.LogError(msg, context);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	public static void LogWarning(string msg, Object context = null)
	{
		Debug.LogWarning(msg, context);
	}

	[Conditional("ENABLE_NOTIFICATIONSOBSERVER_LOGS")]
	public static void Log(string msg, Object context = null)
	{
		Debug.Log(msg, context);
	}
}
