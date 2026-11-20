using UnityEngine;

public class BragPopup : UIScreen
{
	public BragSendHelper SendMessageButton;

	public BragSendHelper FacebookBragButton;

	private FriendHandlerBrag _bragHandler;

	public override void Init()
	{
		base.Init();
		_bragHandler = UIScreenController.Instance.screenAnchor.GetComponentInChildren(typeof(FriendHandlerBrag)) as FriendHandlerBrag;
	}

	public override void Show()
	{
		base.Show();
		SetupNotifyButton();
		SetupFacebookButton();
	}

	private void SetupNotifyButton()
	{
		if (GameStats.Instance.bragNotifyDone)
		{
			if (SendMessageButton.gameObject.GetComponent<Collider>() != null)
			{
				Object.Destroy(SendMessageButton.gameObject.GetComponent<Collider>());
			}
			SendMessageButton.line1.text = Strings.Get(StringID.BRAG_POPUP_SEND_MESSAGE_LINE4);
			SendMessageButton.line2.text = string.Empty;
			SendMessageButton.line3.text = string.Empty;
		}
		else
		{
			NGUITools.AddWidgetCollider(SendMessageButton.gameObject);
			SendMessageButton.line1.text = Strings.Get(StringID.BRAG_POPUP_SEND_MESSAGE_LINE5);
			SendMessageButton.line2.text = Strings.Get(StringID.BRAG_POPUP_SEND_MESSAGE_LINE6) + " (" + _bragHandler.bragList.Count + ")";
			SendMessageButton.line3.text = Strings.Get(StringID.BRAG_POPUP_SEND_MESSAGE_LINE7);
		}
	}

	private void SetupFacebookButton()
	{
		if (SocialManager.instance.facebookIsLoggedIn)
		{
			if (GameStats.Instance.bragFacebookDone)
			{
				if (FacebookBragButton.gameObject.GetComponent<Collider>() != null)
				{
					Object.Destroy(FacebookBragButton.gameObject.GetComponent<Collider>());
				}
				FacebookBragButton.line1.text = Strings.Get(StringID.BRAG_POPUP_FACEBOOK_BRAG_1);
				FacebookBragButton.line2.text = Strings.Get(StringID.BRAG_POPUP_FACEBOOK_BRAG_2);
			}
			else
			{
				NGUITools.AddWidgetCollider(FacebookBragButton.gameObject);
				FacebookBragButton.line1.text = Strings.Get(StringID.BRAG_POPUP_FACEBOOK_BRAG_3);
				FacebookBragButton.line2.text = Strings.Get(StringID.BRAG_POPUP_FACEBOOK_BRAG_4);
			}
		}
		else
		{
			NGUITools.AddWidgetCollider(FacebookBragButton.gameObject);
			FacebookBragButton.line1.text = Strings.Get(StringID.BRAG_POPUP_FACEBOOK_BRAG_5);
			FacebookBragButton.line2.text = Strings.Get(StringID.BRAG_POPUP_FACEBOOK_BRAG_6);
		}
	}

	private void SendMessageButtonClicked()
	{
		if (SocialManager.instance != null)
		{
			SocialManager.instance.BragNotify(PlayerInfo.Instance.oldHighestScore, _bragHandler.bragList);
		}
		if (SendMessageButton.gameObject.GetComponent<Collider>() != null)
		{
			Object.Destroy(SendMessageButton.gameObject.GetComponent<Collider>());
		}
		SendMessageButton.line1.text = Strings.Get(StringID.BRAG_POPUP_MESSAGE_BUTTON);
		GameStats.Instance.bragNotifyDone = true;
		CheckIfCompleted();
	}

	private void FacebookBragButtonClicked()
	{
		if (SocialManager.instance.facebookIsLoggedIn)
		{
			SocialManager.instance.BragFacebook(_bragHandler.bragList);
			if (FacebookBragButton.gameObject.GetComponent<Collider>() != null)
			{
				Object.Destroy(FacebookBragButton.gameObject.GetComponent<Collider>());
			}
			GameStats.Instance.bragFacebookDone = true;
		}
		else
		{
			SocialManager.instance.FacebookLogin(FacebookLoggedIn);
		}
		SetupFacebookButton();
		CheckIfCompleted();
	}

	private void CheckIfCompleted()
	{
		GameStats instance = GameStats.Instance;
		if (instance.bragFacebookDone && instance.bragNotifyDone)
		{
			UIScreenController.Instance.CloseActivePopups(screenName);
			_bragHandler.CompletedBrag();
		}
	}

	private void FacebookLoggedIn(bool loggedIn)
	{
		if (loggedIn)
		{
			SetupFacebookButton();
		}
	}
}
