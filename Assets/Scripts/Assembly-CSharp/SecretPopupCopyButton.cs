using UnityEngine;

public class SecretPopupCopyButton : MonoBehaviour
{
	[SerializeField]
	private SecretPopup _secretPopUpScreen;

	private void OnClick()
	{
		_secretPopUpScreen.CopyToClipboard();
	}
}
