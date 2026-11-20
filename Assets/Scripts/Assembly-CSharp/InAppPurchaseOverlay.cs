using UnityEngine;

public class InAppPurchaseOverlay : MonoBehaviour
{
	public UILabel titleLabel;

	private void OnEnable()
	{
		if (titleLabel != null)
		{
			titleLabel.text = Strings.Get(StringID.INAPP_MANAGER_OVERLAY_TEXT_GOOGLEPLAY);
		}
		else
		{
			Debug.Log("In App Purchase Overlay: Title Label not set");
		}
	}
}
