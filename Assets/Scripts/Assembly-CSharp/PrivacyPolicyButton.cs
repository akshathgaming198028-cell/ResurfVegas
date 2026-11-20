using UnityEngine;

public class PrivacyPolicyButton : MonoBehaviour
{
	[SerializeField]
	private UIScreen myScreen;

	private void OnClick()
	{
		UIScreenController.Instance.PushPopup("PrivacyPolicyPopup");
	}
}
