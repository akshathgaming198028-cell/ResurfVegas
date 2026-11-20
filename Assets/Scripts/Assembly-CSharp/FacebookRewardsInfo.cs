using System;
using System.Collections.Generic;
using SYBO.Subway.Info;
using UnityEngine;

public static class FacebookRewardsInfo
{
	public enum FacebookRewardType
	{
		Coins = 0,
		Keys = 1,
		Hoverboards = 2,
		Headstarts = 3
	}

	public static int POPUP_DISPLAY_TIME_LIMIT = 300;

	public static int POPUP_DISPLAY_RUN_LIMIT = 3;

	public static string FACEBOOK_REWARD_RUN_LIMIT_KEY = "facebook_reward_run_limit";

	public static string FACEBOOK_REWARD_TIME_LIMIT_KEY = "facebook_reward_time_limit";

	public static bool isCharacterRewarded = true;

	public static Characters.CharacterType awardedCharacter = Characters.CharacterType.facebookRewardCharacter01;

	public static int characterTheme = 0;

	public static DateTime connectPopupLastShownOnGameOver = DateTime.Now.AddDays(-1.0);

	public static DateTime connectPopupLastShownOnFriendsScreen = DateTime.Now.AddDays(-1.0);

	public static Dictionary<FacebookRewardType, int> facebookRewardPack = new Dictionary<FacebookRewardType, int>
	{
		{
			FacebookRewardType.Coins,
			5000
		},
		{
			FacebookRewardType.Keys,
			10
		},
		{
			FacebookRewardType.Hoverboards,
			25
		},
		{
			FacebookRewardType.Headstarts,
			5
		}
	};

	public static bool ShouldShowConnectPopupOnGameOver(int showCounter)
	{
		if (Application.isEditor)
		{
		}
		TimeSpan timeSpan = DateTime.Now - connectPopupLastShownOnGameOver;
		TimeSpan timeSpan2 = DateTime.Now - connectPopupLastShownOnFriendsScreen;
		int value = OnlineSettings.instance.GetValue(FACEBOOK_REWARD_TIME_LIMIT_KEY, POPUP_DISPLAY_TIME_LIMIT);
		int value2 = OnlineSettings.instance.GetValue(FACEBOOK_REWARD_RUN_LIMIT_KEY, POPUP_DISPLAY_RUN_LIMIT);
		return !HasReceivedAllAvailableRewards() && Math.Abs((int)timeSpan.TotalSeconds) >= value && Math.Abs((int)timeSpan2.TotalSeconds) >= value && showCounter >= value2;
	}

	public static bool ShouldShowConnectPopupOnFriendsScreen()
	{
		if (Application.isEditor)
		{
		}
		TimeSpan timeSpan = DateTime.Now - connectPopupLastShownOnFriendsScreen;
		int value = OnlineSettings.instance.GetValue(FACEBOOK_REWARD_TIME_LIMIT_KEY, POPUP_DISPLAY_TIME_LIMIT);
		return !HasReceivedAllAvailableRewards() && Math.Abs((int)timeSpan.TotalSeconds) >= value;
	}

	public static bool HasReceivedAllAvailableRewards()
	{
		bool flag = PlayerInfo.Instance.hasPayedOutFacebook && PlayerInfo.Instance.hasPayedOutFacebookPack;
		if (!isCharacterRewarded)
		{
			return flag;
		}
		return flag && PlayerInfo.Instance.hasPayedOutFacebookCharacter;
	}
}
