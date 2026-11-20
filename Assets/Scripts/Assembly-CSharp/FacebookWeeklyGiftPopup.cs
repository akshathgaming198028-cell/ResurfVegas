using System.Collections;
using UnityEngine;

public class FacebookWeeklyGiftPopup : UIScreen
{
	[SerializeField]
	private UILabel title;

	[SerializeField]
	private UILabel subtitle;

	[SerializeField]
	private UIWidget centerWidget;

	[SerializeField]
	private GameObject superMysteryBoxPrefab;

	private GameObject superMysteryBoxPrize;

	private Vector3 ITEM_POSITION_OFFSET = new Vector3(0f, 20f, 0f);

	private Vector3 ITEM_ROTATION = new Vector3(-16f, -17f, 5f);

	private Vector3 ITEM_SCALE = new Vector3(2000f, 2000f, 2000f);

	public override void Show()
	{
		base.Show();
		StartCoroutine(Place3DModel());
	}

	private IEnumerator Place3DModel()
	{
		yield return new WaitForEndOfFrame();
		if (superMysteryBoxPrize != null)
		{
			superMysteryBoxPrize.SetActive(true);
		}
		else
		{
			superMysteryBoxPrize = NGUITools.AddChild(UIModelController.Instance.TutorialPopupAnchor, superMysteryBoxPrefab);
		}
		superMysteryBoxPrize.layer = UIModelController.Instance.TutorialPopupAnchor.layer;
		Vector3 trackerPos = centerWidget.transform.position;
		Vector3 pixelPos = UIScreenController.Instance.Camera2DNormal.WorldToScreenPoint(trackerPos);
		pixelPos = new Vector3(pixelPos.x, pixelPos.y, 3f);
		ResetTransform(newPosition: UIScreenController.Instance.Camera3D.ScreenToWorldPoint(pixelPos), trans: superMysteryBoxPrize.transform, positionOffset: ITEM_POSITION_OFFSET, newRotation: ITEM_ROTATION, newScale: ITEM_SCALE);
	}

	private void ResetTransform(Transform trans, Vector3 newPosition, Vector3 positionOffset, Vector3 newRotation, Vector3 newScale)
	{
		trans.position = newPosition;
		trans.localPosition += positionOffset;
		trans.localRotation = Quaternion.Euler(newRotation);
		trans.localScale = newScale;
	}

	public override void Hide()
	{
		base.Hide();
		superMysteryBoxPrize.SetActive(false);
	}

	public void OnCheckoutClicked()
	{
		if (UIScreenController.IsInstanced)
		{
			UIScreenController.Instance.ignoreLastPopupClosed = true;
			UIScreenController.Instance.doSaveAndDelayRemainingPopups = true;
			UIScreenController.Instance.CloseActivePopups(screenName);
			UIScreenController.Instance.PushScreen("EarnCoinsScreen");
		}
	}

	public static void ShowIfNeeded()
	{
		if (FacebookWeeklyGiftManager.Instance.ShouldDisplayPopup())
		{
			UIScreenController.Instance.QueuePopup("FacebookWeeklyGiftPopup");
			FacebookWeeklyGiftManager.Instance.PopupDidShow();
		}
	}
}
