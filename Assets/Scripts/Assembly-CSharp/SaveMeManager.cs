using UnityEngine;

public static class SaveMeManager
{
	private const string SAVE_ME_LOCALES_KEY = "save_me_locales";

	private static int _numberOfUsedKeysInCurrentRun = 1;

	public static bool IS_PURCHASE_MADE_FROM_INGAME;

	public static bool IS_PURCHASE_RUNNING_INGAME;

	public static void IncrementNumberOfUsedKeys()
	{
		if (_numberOfUsedKeysInCurrentRun <= 0)
		{
			_numberOfUsedKeysInCurrentRun = 1;
		}
		else
		{
			_numberOfUsedKeysInCurrentRun *= 2;
		}
	}

	public static int GetNumberOfKeysToSaveMe()
	{
		return _numberOfUsedKeysInCurrentRun;
	}

	public static void ResetSaveMeForNewRun()
	{
		_numberOfUsedKeysInCurrentRun = 1;
	}

	public static void HasShownKeysPopup()
	{
		PlayerInfo.Instance.hasShownKeysPopup = true;
	}

	public static bool IsSaveMePopupEnabled()
	{
		return PlayerInfo.Instance.isSaveMePopupEnabled;
	}

	public static void SkipReviveIfPurchaseFailed()
	{
		if (UIScreenController.IsInstanced)
		{
			if (UIScreenController.Instance.GetTopScreenName() == "IngameUI" && IS_PURCHASE_MADE_FROM_INGAME)
			{
				IS_PURCHASE_MADE_FROM_INGAME = false;
				IS_PURCHASE_RUNNING_INGAME = false;
				Revive instance = Revive.Instance;
				if (instance != null)
				{
					instance.SendSkipRevive();
				}
				else
				{
					Debug.LogWarning("SaveMeManager: reviveInstance is null");
				}
			}
		}
		else
		{
			Debug.LogWarning("SaveMeManager: UIScreenController instance null");
		}
	}

	public static void SendReviveIfPurchaseSucceeded()
	{
		if (UIScreenController.IsInstanced)
		{
			string topScreenName = UIScreenController.Instance.GetTopScreenName();
			if (!string.IsNullOrEmpty(topScreenName) && topScreenName.Equals("IngameUI") && IS_PURCHASE_MADE_FROM_INGAME)
			{
				if (PlayerInfo.Instance.amountOfKeys - GetNumberOfKeysToSaveMe() >= 0)
				{
					PlayerInfo.Instance.amountOfKeys -= GetNumberOfKeysToSaveMe();
					MissionsManager.Instance.PlayerDidThis(MissionTarget.SpendKeys, GetNumberOfKeysToSaveMe());
				}
				IS_PURCHASE_MADE_FROM_INGAME = false;
				IS_PURCHASE_RUNNING_INGAME = false;
				Revive instance = Revive.Instance;
				if (instance != null)
				{
					instance.SendRevive();
				}
				else
				{
					Debug.LogWarning("SaveMeManager: reviveInstance is null");
				}
				IncrementNumberOfUsedKeys();
				UIScreenController instance2 = UIScreenController.Instance;
				IngameScreen ingameScreen = instance2.GetScreenFromCache(instance2.GetTopScreenName()) as IngameScreen;
				if (ingameScreen == null)
				{
					Debug.LogError("IngameScreen == NULL");
				}
				else
				{
					ingameScreen.SetPauseButtonVisibility(true);
				}
			}
		}
		else
		{
			Debug.LogWarning("SaveMeManager: UIScreenController instance null");
		}
	}
}
