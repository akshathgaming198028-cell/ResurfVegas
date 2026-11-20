using SYBO.Subway;
using UnityEngine;

[AddComponentMenu("GUI/Interaction/Change Screen Button")]
public class UIButtonChangeScreen : UIBasicButton
{
	public enum ScreenChangeType
	{
		PushScreen = 0,
		SwitchScreen = 1,
		BackToPrevious = 2,
		QueuePopup = 3,
		ClosePopup = 4,
		PushPopup = 5
	}

	private GameObject target;

	private string functionName = string.Empty;

	public ScreenChangeType screenChangeType;

	private string popupToReturnTo;

	public string ScreenNameToOpen;

	private bool useSend = true;

	public void SetScreenNameAndDoNotUseSendMessage(string sendScreenNameToOpen)
	{
		ScreenNameToOpen = sendScreenNameToOpen;
		useSend = false;
	}

	private void Awake()
	{
		if (useSend)
		{
			if (screenChangeType == ScreenChangeType.PushScreen)
			{
				functionName = "PushScreen";
			}
			else if (screenChangeType == ScreenChangeType.SwitchScreen)
			{
				functionName = "SwitchScreen";
			}
			else if (screenChangeType == ScreenChangeType.BackToPrevious)
			{
				functionName = "BackToPrevious";
			}
			else if (screenChangeType == ScreenChangeType.QueuePopup)
			{
				functionName = "QueuePopup";
			}
			else if (screenChangeType == ScreenChangeType.PushPopup)
			{
				functionName = "PushPopup";
			}
			if (screenChangeType == ScreenChangeType.ClosePopup)
			{
				functionName = "ClosePopup";
			}
		}
	}

	protected override void Send()
	{
		if (!useSend)
		{
			return;
		}
		if (ScreenNameToOpen == "CoinsUI_quick")
		{
			string topScreenName = UIScreenController.Instance.GetTopScreenName();
			if (!string.IsNullOrEmpty(topScreenName))
			{
				Flurry.LogEventWithAParameter("More coins button clicked", "Screen Name", topScreenName);
			}
			else
			{
				Flurry.LogEvent("More coins button clicked");
			}
		}
		else if (ScreenNameToOpen == "PauseUI")
		{
			Game.Instance.wasButtonClicked = true;
		}
		if (base.enabled && base.gameObject.activeInHierarchy)
		{
			if (string.IsNullOrEmpty(ScreenNameToOpen) && (screenChangeType == ScreenChangeType.PushScreen || screenChangeType == ScreenChangeType.SwitchScreen || screenChangeType == ScreenChangeType.QueuePopup))
			{
				Debug.LogError(base.name + " tried to send an empty Change Screen message");
			}
			if (target == null)
			{
				target = MessageCenter.Instance.gameObject;
			}
			Transform[] componentsInChildren = target.GetComponentsInChildren<Transform>();
			Transform[] array = componentsInChildren;
			foreach (Transform transform in array)
			{
				transform.gameObject.SendMessage(functionName, base.gameObject, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
