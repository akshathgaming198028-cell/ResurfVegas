public class UIScreens
{
	public const string BRAG_POPUP = "BragPopup";

	public const string CHARACTER_SCREEN = "CharacterScreen";

	public const string COINS_SCREEN = "CoinsUI_shop";

	public const string COINS_POPUP = "CoinsUI_quick";

	public const string WELCOME_POPUP = "WelcomePopup";

	public const string DAILY_CHALLENGE_POPUP = "DailyChallengePopup";

	public const string FACEBOOK_PAYOUT_POPUP = "FacebookPayoutPopup";

	public const string FACEBOOK_REWARDS_POPUP = "FacebookRewardsPopup";

	public const string FACEBOOK_WEEKLYGIFT_POPUP = "FacebookWeeklyGiftPopup";

	public const string FRIENDS_SCREEN = "FriendsUI";

	public const string FRONT_SCREEN = "FrontUI";

	public const string GAMECENTER_PAYOUT_POPUP = "GameCenterPayoutPopup";

	public const string GAMEOVER_SCREEN = "GameoverUI";

	public const string HOVERBOARD_POPUP = "HoverboardPopup";

	public const string PRIVACY_POLICY_POPUP = "PrivacyPolicyPopup";

	public const string LEADERBOARD_SCREEN = "LeaderboardUI";

	public const string TOPRUN_SCREEN = "TopRunUI";

	public const string MISSION_POPUP = "Mission_popup";

	public const string MYSTERYBOX_POPUP = "CelebrationPopup";

	public const string PAUSE_SCREEN = "PauseUI";

	public const string PRE_BRAG_POPUP = "PreBragPopup";

	public const string PRIZE_MB_POPUP_KEYS = "PrizeMBPopup_Keys";

	public const string PRIZE_SMB_POPUP_KEYS = "PrizeSMBPopup_Keys";

	public const string PRIZE_MMB_POPUP = "PrizeMMBPopup";

	public const string SETTINGS_POPUP = "SettingsPopup";

	public const string INGAME_SCREEN = "IngameUI";

	public const string AWARDS_SCREEN = "AwardsScreen";

	public const string TUTORIAL_COLLECT_FROM_FRIENDS = "TutorialCollectFromFriendsPopup";

	public const string TUTORIAL_DOUBLE_COINS = "TutorialDoubleCoinsPopup";

	public const string TUTORIAL_END_GAME_MISSIONS = "TutorialEndGameMissionsPopup";

	public const string TUTORIAL_FACEBOOK_POPUP = "TutorialFacebookPopup";

	public const string TUTORIAL_HOVERBOARDS = "TutorialHoverboardsPopup";

	public const string TUTORIAL_MISSION = "TutorialMissionPopup";

	public const string UPDATE_APP_POPUP = "UpdateAppPopup";

	public const string UPGRADE_SCREEN = "UpgradesUI_shop";

	public const string UPGRADE_POPUP = "UpgradesUI_quick";

	public const string NOTEBOOK_BACKGROUND = "NotebookPanel2";

	public const string BOARD_SCREEN = "BoardScreen";

	public const string EARN_COINS_SCREEN = "EarnCoinsScreen";

	public const string RATING_POPUP = "RatingPopup";

	public const string ITEM_HUNT_POPUP = "ItemHuntPopup";

	public const string SAVE_ME_POPUP = "SaveMePopup";

	public const string EVENTS_POPUP = "EventsPopup";

	public const string NOT_ENOUGH_CURENCY_POPUP = "NotEnoughCurencyPopup";

	public const string AWARDS_POPUP = "AwardsPopup";

	public const string COUNTRY_POPUP = "CountryPopup";

	public const string TOPRUN_POPUP = "TopRunPopup";

	public const string HOVERBOARD_CUSTOMIZATION_POPUP = "HoverboardCustomizationPopup";

	public const string TURBO_HEADSTART_INTRODUCTION_POPUP = "TurboHeadstartIntroductionPopup";

	public const string CHARACTER_NAME_EVENT_POPUP = "CharacterNameEventPopup";

	public const string EVENT_TESTER_POPUP = "EventTesterPopup";

	public const string CELEBRATION_TESTER_POPUP = "CelebrationTesterPopup";

	public const string DEBUGSETTINGSPOPUP = "DebugSettingsPopup";

	public const string SECRET_POPUP = "SecretPopup";

	public const string LIMITED_CHARACTER_POPUP = "LimitedCharacterPopup";

	public const string LIMITED_BOARD_POPUP = "LimitedBoardPopup";

	public static string friendsMenu_lastScreen = "TopRunUI";

	public static string meMenu_lastScreen = "CharacterScreen";

	public static string shopMenu_lastScreen = "UpgradesUI_shop";

	public static string GetFlurryPopUpWithOkAndCloseName(string goName)
	{
		string text = string.Empty;
		if (goName.Contains("TutorialCollectFromFriendsPopup"))
		{
			text = "GuidelineCollectPopup";
		}
		else if (goName.Contains("TutorialEndGameMissionsPopup"))
		{
			text = "GuidelineEndGameMissionPopup";
		}
		else if (goName.Contains("TutorialFacebookPopup"))
		{
			text = "GuidelineFacebook";
		}
		else if (goName.Contains("TutorialMissionPopup"))
		{
			text = "GuidelineMissionPopup";
		}
		else if (goName.Contains("PrivacyPolicyPopup"))
		{
			text = "PrivacyPolicyPopup";
		}
		else if (goName.Contains("TopRunPopup"))
		{
			text = "TopRunPopup";
		}
		else if (goName.Contains("HoverboardCustomizationPopup"))
		{
			text = "HoverboardCustomizationPopup";
		}
		else if (goName.Contains("UpdateAppPopup"))
		{
			text = "Update App";
		}
		else if (goName.Contains("TurboHeadstartIntroductionPopup"))
		{
			text = "TurboHeadstartIntroductionPopup";
		}
		if (text != string.Empty)
		{
			text = "POPUP Screen " + text;
		}
		return text;
	}

	public static string GetFlurryScreenName(string goName)
	{
		string text = string.Empty;
		if (goName.Contains("BragPopup"))
		{
			text = "BragPopup";
		}
		else if (goName.Contains("CoinsUI_quick"))
		{
			text = "CoinsUI_quick";
		}
		else if (goName.Contains("CharacterScreen"))
		{
			text = "CharacterScreen";
		}
		else if (goName.Contains("FacebookPayoutPopup"))
		{
			text = "FacebookPayoutPopup";
		}
		else if (goName.Contains("FacebookRewardsPopup"))
		{
			text = "FacebookRewardsPopup";
		}
		else if (goName.Contains("DailyChallengePopup"))
		{
			text = "DailyChallengePopup";
		}
		else if (goName.Contains("GameCenterPayoutPopup"))
		{
			text = "GameCenterPayoutPopup";
		}
		else if (goName.Contains("PreBragPopup"))
		{
			text = "PreBragPopup";
		}
		else if (goName.Contains("HoverboardPopup"))
		{
			text = "HoverboardPopup";
		}
		else if (goName.Contains("Mission_popup"))
		{
			text = "Mission_popup";
		}
		else if (goName.Contains("CelebrationPopup"))
		{
			text = "CelebrationPopup";
		}
		else if (goName.Contains("UpgradesUI_quick"))
		{
			text = "UpgradesUI_quick";
		}
		else if (goName.Contains("TutorialDoubleCoinsPopup"))
		{
			text = "TutorialDoubleCoinsPopup";
		}
		else if (goName.Contains("PrizeMBPopup_Keys"))
		{
			text = "PrizeMBPopup_Keys";
		}
		else if (goName.Contains("PrizeSMBPopup_Keys"))
		{
			text = "PrizeSMBPopup_Keys";
		}
		else if (goName.Contains("SettingsPopup"))
		{
			text = "SettingsPopup";
		}
		else if (goName.Contains("CoinsUI_shop"))
		{
			text = "CoinsUI_shop";
		}
		else if (goName.Contains("FriendsUI"))
		{
			text = "FriendsUI";
		}
		else if (goName.Contains("FrontUI"))
		{
			text = "FrontUI";
		}
		else if (goName.Contains("GameoverUI"))
		{
			text = "GameoverUI";
		}
		else if (goName.Contains("LeaderboardUI"))
		{
			text = "LeaderboardUI";
		}
		else if (goName.Contains("TopRunUI"))
		{
			text = "TopRunUI";
		}
		else if (goName.Contains("HoverboardCustomizationPopup"))
		{
			text = "HoverboardCustomizationPopup";
		}
		else if (goName.Contains("PauseUI"))
		{
			text = "PauseUI";
		}
		else if (goName.Contains("IngameUI"))
		{
			text = "IngameUI";
		}
		else if (goName.Contains("AwardsScreen"))
		{
			text = "AwardsScreen";
		}
		else if (goName.Contains("UpgradesUI_shop"))
		{
			text = "UpgradesUI_shop";
		}
		else if (goName.Contains("BoardScreen"))
		{
			text = "BoardScreen";
		}
		else if (goName.Contains("EarnCoinsScreen"))
		{
			text = "EarnCoinsScreen";
		}
		else if (goName.Contains("ItemHuntPopup"))
		{
			text = "ItemHuntPopup";
		}
		else if (goName.Contains("CountryPopup"))
		{
			text = "CountryPopup";
		}
		else if (goName.Contains("TurboHeadstartIntroductionPopup"))
		{
			text = "TurboHeadstartIntroductionPopup";
		}
		else if (goName.Contains("LimitedCharacterPopup"))
		{
			text = "LimitedCharacterPopup";
		}
		else if (goName.Contains("LimitedBoardPopup"))
		{
			text = "LimitedBoardPopup";
		}
		if (text != string.Empty)
		{
			text = "UI Screen " + text;
		}
		return text;
	}
}
