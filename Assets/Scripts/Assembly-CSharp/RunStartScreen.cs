using JetBrains.Annotations;
using SYBO.Subway;
using UnityEngine;

public abstract class RunStartScreen : UIScreen
{
	[UsedImplicitly]
	public void StartRun()
	{
		if (Input.touchCount <= 1)
		{
			if (Game.Instance != null)
			{
				SSEventsManagerController.instance.RestoreBoardAfterEventIfNeeded();
				Game.Instance.StartNewRun(false);
				UIScreenController.Instance.PushScreen("IngameUI");
				SaveMeManager.ResetSaveMeForNewRun();
			}
			else
			{
				UIScreenController.Instance.PushScreen("GameoverUI");
			}
		}
	}
}
