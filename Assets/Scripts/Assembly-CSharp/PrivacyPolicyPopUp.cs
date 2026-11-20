using System.Collections;
using Kiloo.Common;
using UnityEngine;

public class PrivacyPolicyPopUp : UIScreen
{
	[SerializeField]
	private UILabel popupTitle;

	[SerializeField]
	private UILabel noButtonLabel;

	[SerializeField]
	private UILabel yesButtonLabel;

	private void Awake()
	{
		popupTitle.text = Strings.Get(StringID.PRIVACY_POLICY_PUOPUP_TITLE);
		noButtonLabel.text = Strings.Get(StringID.PRIVACY_POLICY_PUOPUP_NO);
		yesButtonLabel.text = Strings.Get(StringID.PRIVACY_POLICY_PUOPUP_YES);
	}

	public void ClosePopup()
	{
		Flurry.LogClosePressedOnPopup(UIScreens.GetFlurryPopUpWithOkAndCloseName(base.gameObject.name));
		UIScreenController.Instance.CloseActivePopups(screenName);
	}

	public void ReadPrivacyPolicy()
	{
		Flurry.LogOkPressedOnPopup(UIScreens.GetFlurryPopUpWithOkAndCloseName(base.gameObject.name));
		CoroutineUtil.StartCoroutine(DelayShowingPolicy());
		UIScreenController.Instance.CloseActivePopups(screenName);
	}

	private IEnumerator DelayShowingPolicy()
	{
		yield return null;
		Application.OpenURL("http://www.kiloo.com/privacy/");
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			ClosePopup();
		}
	}
}
