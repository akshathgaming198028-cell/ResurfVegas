using SYBO.Subway;
using UnityEngine;

public class SaveMeButton : MonoBehaviour
{
	[SerializeField]
	private UILabel saveMePrice;

	public SaveMePopup saveMePopup;

	[SerializeField]
	private GameObject _saveMePriceParent;

	private void OnEnable()
	{
		saveMePrice.text = SaveMeManager.GetNumberOfKeysToSaveMe() + string.Empty;
		CenterUnit(_saveMePriceParent.transform, 2f);
	}

	public void CenterUnit(Transform targetTransform, float aditionalOffset = 0f)
	{
		Vector3 localPosition = (targetTransform.localPosition = Vector3.zero);
		Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(targetTransform);
		localPosition.x = localPosition.x - bounds.extents.x / 2f + aditionalOffset;
		targetTransform.localPosition = localPosition;
	}

	private void OnClick()
	{
		Game.Instance.wasButtonClicked = true;
		int numberOfKeysToSaveMe = SaveMeManager.GetNumberOfKeysToSaveMe();
		if (numberOfKeysToSaveMe <= PlayerInfo.Instance.amountOfKeys)
		{
			UIScreenController instance = UIScreenController.Instance;
			IngameScreen ingameScreen = instance.GetScreenFromCache(instance.GetTopScreenName()) as IngameScreen;
			if (ingameScreen == null)
			{
				Debug.LogError("IngameScreen == NULL");
			}
			else
			{
				ingameScreen.SetPauseButtonVisibility(true);
			}
			PlayerInfo.Instance.amountOfKeys -= numberOfKeysToSaveMe;
			if (PlayerInfo.Instance.amountOfKeys < 0)
			{
				PlayerInfo.Instance.amountOfKeys = 0;
			}
			Revive.Instance.SendRevive();
			SaveMeManager.IncrementNumberOfUsedKeys();
			UIScreenController.Instance.CloseActivePopups("SaveMePopup");
			SaveMeManager.IS_PURCHASE_MADE_FROM_INGAME = false;
			SaveMeManager.IS_PURCHASE_RUNNING_INGAME = false;
			Flurry.LogEventWithAParameter("Save Me performed", "Amount", numberOfKeysToSaveMe.ToString());
			MissionsManager.Instance.PlayerDidThis(MissionTarget.SpendKeys, numberOfKeysToSaveMe);
		}
		else
		{
			SaveMeManager.IS_PURCHASE_MADE_FROM_INGAME = true;
			SaveMeManager.IS_PURCHASE_RUNNING_INGAME = true;
			UIScreenController.Instance.ClosePopup();
			PurchaseHandler.Instance.PurchaseKeysIfNeeded(numberOfKeysToSaveMe);
		}
	}
}
