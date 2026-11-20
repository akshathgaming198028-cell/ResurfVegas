using UnityEngine;

public class SaveMeCancelButton : MonoBehaviour
{
	public SaveMePopup saveMePopup;

	private void OnClick()
	{
		if (saveMePopup != null && saveMePopup.getAnimationTimeLeft() < saveMePopup.getAnimationDuration() - 1f)
		{
			CloseSaveMe();
		}
	}

	public static void CloseSaveMe()
	{
		UIScreenController.Instance.CloseActivePopups("SaveMePopup");
		Revive.Instance.SendSkipRevive();
		SaveMeManager.ResetSaveMeForNewRun();
	}
}
