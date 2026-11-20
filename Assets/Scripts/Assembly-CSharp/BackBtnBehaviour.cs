using JetBrains.Annotations;
using Kiloo.Plugins.DeviceUtility;
using SYBO.Subway;
using UnityEngine;

public class BackBtnBehaviour : MonoBehaviour
{
	public enum ScreenChangeType
	{
		PushScreen = 0,
		SwitchScreen = 1,
		BackToPrevious = 2,
		QueuePopup = 3,
		ClosePopup = 4,
		ExitGame = 5,
		SaveMePopup = 6,
		NotEnoughCurrency = 7
	}

	private GameObject target;

	private string functionName = string.Empty;

	public ScreenChangeType screenChangeType;

	public GameObject popupLayerAnchor;

	public string ScreenNameToOpen = string.Empty;

	[UsedImplicitly]
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Send();
		}
	}

	private void CheckForFunctionToExecute()
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
		else if (screenChangeType == ScreenChangeType.ClosePopup)
		{
			functionName = "ClosePopup";
		}
		else if (screenChangeType == ScreenChangeType.ExitGame)
		{
			functionName = "ExitGame";
		}
		else if (screenChangeType == ScreenChangeType.SaveMePopup)
		{
			functionName = "CloseSaveMe";
		}
		else if (screenChangeType == ScreenChangeType.NotEnoughCurrency)
		{
			functionName = "CloseNotEnoughCurrency";
		}
	}

	protected void Send()
	{
		if (UIScreenController.Instance.GetCurrentPopupName() == "CelebrationPopup" || UIScreenController.Instance.IsInAppPurchaseOverlayVisible())
		{
			return;
		}
		if (UIScreenController.Instance.IsAdsOverlayVisible())
		{
			if (UIScreenController.IsInstanced)
			{
				UIScreenController.Instance.HideAdsOverlay();
			}
			return;
		}
		CheckForFunctionToExecute();
		if (!base.enabled || !base.gameObject.activeInHierarchy)
		{
			return;
		}
		if (string.IsNullOrEmpty(ScreenNameToOpen) && (screenChangeType == ScreenChangeType.PushScreen || screenChangeType == ScreenChangeType.SwitchScreen || screenChangeType == ScreenChangeType.QueuePopup))
		{
			Debug.LogError(base.name + " tried to send an empty Change Screen message");
		}
		if (functionName.Equals("ExitGame"))
		{
			if (UIScreenController.Instance.IsPopupQueueEmpty())
			{
				ShowExitGamePopup();
			}
		}
		else if (functionName.Equals("CloseSaveMe"))
		{
			SaveMeCancelButton.CloseSaveMe();
		}
		else if (functionName.Equals("CloseNotEnoughCurrency"))
		{
			UIScreenController.Instance.CloseActivePopups("NotEnoughCurencyPopup");
			if (Game.Instance.IsInGame.Value)
			{
				Revive.Instance.SendSkipRevive();
				SaveMeManager.ResetSaveMeForNewRun();
			}
		}
		else
		{
			if (target == null)
			{
				target = MessageCenter.Instance.gameObject;
			}
			Transform[] componentsInChildren = target.gameObject.GetComponentsInChildren<Transform>();
			Transform[] array = componentsInChildren;
			foreach (Transform transform in array)
			{
				transform.SendMessage(functionName, base.gameObject, SendMessageOptions.DontRequireReceiver);
			}
		}
		LogFlurry();
	}

	private void ShowExitGamePopup()
	{
		DeviceUtility.ShowNativePopup(Strings.Get(StringID.QUIT_NATIVE_POPUP_TITLE), Strings.Get(StringID.QUIT_NATIVE_POPUP_TEXT), Strings.Get(StringID.QUIT_NATIVE_POPUP_BTN_RETURN), Strings.Get(StringID.QUIT_NATIVE_POPUP_BTN_QUIT), delegate(string buttonPressed)
		{
			if (buttonPressed == Strings.Get(StringID.QUIT_NATIVE_POPUP_BTN_QUIT))
			{
				PlayerInfo.Instance.SaveIfDirty();
				SocialManager.instance.SaveIfDirty();
				Application.Quit();
			}
		});
	}

	private void LogFlurry()
	{
		string currentPopupName = UIScreenController.Instance.GetCurrentPopupName();
		string text = (string.IsNullOrEmpty(currentPopupName) ? UIScreenController.Instance.GetTopScreenName() : currentPopupName);
		if (string.IsNullOrEmpty(text))
		{
			Flurry.LogEventWithAParameter("Back button pressed", "Screen Name", "Null");
		}
		else
		{
			Flurry.LogEventWithAParameter("Back button pressed", "Screen Name", text);
		}
	}
}
