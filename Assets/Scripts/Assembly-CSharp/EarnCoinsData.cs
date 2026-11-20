using System;
using System.Text;
using Kiloo.Plugins.DeviceUtility;
using UnityEngine;

public static class EarnCoinsData
{
	public enum Id
	{
		FacebookKiloo = 0,
		FacebookSybo = 1,
		TwitterKiloo = 2,
		TwitterSybo = 3,
		YoutubeKiloo = 4,
		YoutubeSybo = 5,
		Review = 6,
		AdColony = 7,
		VideoAd = 8,
		FacebookLogin = 9,
		FacebookWeeklyGift = 10
	}

	public enum Type
	{
		OpenURL = 0,
		VideoAd = 1,
		ClaimReward = 2
	}

	public enum Repeatability
	{
		Once = 0,
		OncePerVersion = 1,
		Forever = 2
	}

	public class EarnCurrencyProfile
	{
		public Id id;

		public Type type;

		public Repeatability repeatability;

		public int defaultAmountOfCoins;

		public StringID title;

		public StringID desc;

		public StringID iOSDesc;

		public string iconName = string.Empty;

		public string url;

		public string overrideAmountOfCoinsOnlineSettingKey;

		public int GetAmountOfCoins()
		{
			string valueString;
			if (!string.IsNullOrEmpty(overrideAmountOfCoinsOnlineSettingKey) && OnlineSettings.instance.TryGetValue(overrideAmountOfCoinsOnlineSettingKey, out valueString))
			{
				int result;
				if (int.TryParse(valueString, out result))
				{
					if (result > 0)
					{
						return result;
					}
				}
				else
				{
					Debug.LogError("EarnCurrencyProfile ERROR: Unable to parse int from onlinesetting " + overrideAmountOfCoinsOnlineSettingKey + ": " + valueString);
				}
			}
			return defaultAmountOfCoins;
		}
	}

	private const string ONLINESETTINGS_VIDEOADS_DEFAULTREWARD_KEY = "videoads_defaultreward";

	private const string DATA_PROFILE_MAIN_SPLIT = ";";

	private const string DATA_PROFILE_SUB_SPLIT = "=";

	public static readonly EarnCurrencyProfile[] profiles = new EarnCurrencyProfile[9]
	{
		new EarnCurrencyProfile
		{
			id = Id.VideoAd,
			type = Type.VideoAd,
			repeatability = Repeatability.Forever,
			title = StringID.EARN_COINS_SPONSORED_VIDEO,
			desc = StringID.EARN_COINS_SPONSORED_VIDEO_DESCRIPTION,
			iOSDesc = StringID.EARN_COINS_SPONSORED_VIDEO_DESCRIPTION,
			iconName = "icon_small_mini_mysteryBox_glow",
			overrideAmountOfCoinsOnlineSettingKey = "videoads_defaultreward"
		},
		new EarnCurrencyProfile
		{
			id = Id.FacebookLogin,
			type = Type.OpenURL,
			repeatability = Repeatability.Once,
			defaultAmountOfCoins = 5000,
			title = StringID.EARN_COINS_FACEBOOK_LOGIN,
			desc = StringID.EARN_COINS_FACEBOOK_LOGIN_DESCRIPTION,
			iOSDesc = StringID.EARN_COINS_FACEBOOK_LOGIN_DESCRIPTION,
			iconName = "icon_coinPack_1",
			url = string.Empty
		},
		new EarnCurrencyProfile
		{
			id = Id.FacebookWeeklyGift,
			type = Type.ClaimReward,
			repeatability = Repeatability.Forever,
			title = StringID.EARN_COINS_FACEBOOKGIFT_TITLE,
			desc = StringID.EARN_COINS_FACEBOOKGIFT_DESCRIPTION,
			iOSDesc = StringID.EARN_COINS_FACEBOOKGIFT_DESCRIPTION,
			iconName = "icon_upgrades_superMysteryBox"
		},
		new EarnCurrencyProfile
		{
			id = Id.FacebookKiloo,
			type = Type.OpenURL,
			repeatability = Repeatability.Once,
			defaultAmountOfCoins = 300,
			title = StringID.EARN_COINS_KILOO_FACEBOOK,
			desc = StringID.EARN_COINS_KILOO_FACENOOK_DESCRIPTION,
			iOSDesc = StringID.EARN_COINS_KILOO_FACEBOOK_IOS_DESCRIPTION,
			iconName = "icon_coinPack_1",
			url = "http://m.facebook.com/kiloogames"
		},
		new EarnCurrencyProfile
		{
			id = Id.FacebookSybo,
			type = Type.OpenURL,
			repeatability = Repeatability.Once,
			defaultAmountOfCoins = 300,
			title = StringID.EARN_COINS_SYBO_FACEBOOK,
			desc = StringID.EARN_COINS_SYBO_FACEBOOK_DESCRIPTION,
			iOSDesc = StringID.EARN_COINS_SYBO_FACEBOOK_IOS_DESCRIPTION,
			iconName = "icon_coinPack_1",
			url = "http://m.facebook.com/SubwaySurfersSybo"
		},
		new EarnCurrencyProfile
		{
			id = Id.TwitterKiloo,
			type = Type.OpenURL,
			repeatability = Repeatability.Once,
			defaultAmountOfCoins = 300,
			title = StringID.EARN_COINS_KILOO_TWITTER,
			desc = StringID.EARN_COINS_KILOO_TWITTER_DESCRIPTION,
			iOSDesc = StringID.EARN_COINS_KILOO_TWITTER_IOS_DESCRIPTION,
			iconName = "icon_coinPack_1",
			url = "https://mobile.twitter.com/kiloogames"
		},
		new EarnCurrencyProfile
		{
			id = Id.TwitterSybo,
			type = Type.OpenURL,
			repeatability = Repeatability.Once,
			defaultAmountOfCoins = 300,
			title = StringID.EARN_COINS_SYBO_TWITTER,
			desc = StringID.EARN_COINS_SYBO_TWITTER_DESCRIPTION,
			iOSDesc = StringID.EARN_COINS_SYBO_TWITTER_IOS_DESCRIPTION,
			iconName = "icon_coinPack_1",
			url = "https://mobile.twitter.com/sybogames"
		},
		new EarnCurrencyProfile
		{
			id = Id.YoutubeKiloo,
			type = Type.OpenURL,
			repeatability = Repeatability.Once,
			defaultAmountOfCoins = 300,
			title = StringID.EARN_COINS_KILOO_YOUTUBE,
			desc = StringID.EARN_COINS_KILOO_YOUTUBE_DESCRIPTION,
			iOSDesc = StringID.EARN_COINS_KILOO_YOUTUBE_IOS_DESCRIPTION,
			iconName = "icon_coinPack_1",
			url = "http://m.youtube.com/kiloomobile"
		},
		new EarnCurrencyProfile
		{
			id = Id.YoutubeSybo,
			type = Type.OpenURL,
			repeatability = Repeatability.Once,
			defaultAmountOfCoins = 300,
			title = StringID.EARN_COINS_SYBO_YOUTUBE,
			desc = StringID.EARN_COINS_SYBO_YOUTUBE_DESCRIPTION,
			iOSDesc = StringID.EARN_COINS_SYBO_YOUTUBE_IOS_DESCRIPTION,
			iconName = "icon_coinPack_1",
			url = "http://m.youtube.com/sybogames"
		}
	};

	private static readonly string[] DATA_PROFILE_ALL_SPLITS = new string[2] { "=", ";" };

	private static string[] profileData;

	public static bool ShouldShowInGUI(int profileIndex)
	{
		EarnCurrencyProfile earnCurrencyProfile = profiles[profileIndex];
		if (earnCurrencyProfile.id == Id.FacebookLogin)
		{
			return !PlayerInfo.Instance.hasPayedOutFacebook;
		}
		if (earnCurrencyProfile.id == Id.FacebookWeeklyGift)
		{
			return FacebookWeeklyGiftManager.Instance.IsGiftAvailable();
		}
		string text = null;
		if (earnCurrencyProfile.repeatability == Repeatability.Once || earnCurrencyProfile.repeatability == Repeatability.OncePerVersion)
		{
			string text2 = GetProfileData(profileIndex);
			if (string.IsNullOrEmpty(text2))
			{
				return true;
			}
			if (earnCurrencyProfile.repeatability == Repeatability.OncePerVersion)
			{
				if (text == null)
				{
					text = DeviceUtility.GetBundleVersion();
				}
				if (text2 != text)
				{
					return true;
				}
			}
			return false;
		}
		return true;
	}

	public static int GetNumberOfDisplayedNonOfferEarners()
	{
		int num = 0;
		for (int i = 0; i < profiles.Length; i++)
		{
			if (ShouldShowInGUI(i))
			{
				num++;
			}
		}
		return num;
	}

	public static void Trigger(int profileIndex)
	{
		if (!ShouldShowInGUI(profileIndex))
		{
			Debug.LogError("An already trigerred coin earner button has been pressed!");
			return;
		}
		EarnCurrencyProfile earnCurrencyProfile = profiles[profileIndex];
		Flurry.LogEventWithAParameter("Earn Coins item clicked", "Id", earnCurrencyProfile.id.ToString());
		if (earnCurrencyProfile.id == Id.FacebookLogin && SocialManager.instance.FacebookLogin(null))
		{
			UIScreenController.Instance.FacebookLogIn(true);
		}
		if (earnCurrencyProfile.type == Type.OpenURL)
		{
			int amountOfCoins = earnCurrencyProfile.GetAmountOfCoins();
			if (amountOfCoins > 0)
			{
				PlayerInfo.Instance.amountOfCoins += amountOfCoins;
			}
		}
		else if (earnCurrencyProfile.type != Type.VideoAd)
		{
			if (earnCurrencyProfile.type == Type.ClaimReward)
			{
				if (earnCurrencyProfile.id == Id.FacebookWeeklyGift)
				{
					RewardManager.AddRewardToUnlock(CelebrationRewardOrigin.SuperMysteryBox, false);
					FacebookWeeklyGiftManager.Instance.GiftWasClaimed();
				}
			}
			else
			{
				Debug.LogError("Unhandled earner type: " + earnCurrencyProfile.type);
			}
		}
		if (earnCurrencyProfile.repeatability == Repeatability.Once || earnCurrencyProfile.repeatability == Repeatability.OncePerVersion)
		{
			string bundleVersion = DeviceUtility.GetBundleVersion();
			SetAndSaveProfileData(profileIndex, bundleVersion);
		}
		if (earnCurrencyProfile.type == Type.OpenURL)
		{
			Application.OpenURL(earnCurrencyProfile.url);
		}
		else if (earnCurrencyProfile.type == Type.VideoAd)
		{
			if (!VideoAdsManager.instance.isInitialized)
			{
				VideoAdsManager.instance.Init();
			}
			if (!VideoAdsManager.instance.PlayVideoIfAvailable())
			{
				VideoAdsManager.instance.TryCachingVideosAgain();
			}
			else
			{
				UIScreenController.Instance.ShowVideoAdsOverlay();
			}
		}
		else if (earnCurrencyProfile.type == Type.ClaimReward)
		{
			if (earnCurrencyProfile.id == Id.FacebookWeeklyGift)
			{
				UIScreenController.Instance.QueueMysteryBox();
			}
		}
		else
		{
			Debug.LogError("Unhandled earner type: " + earnCurrencyProfile.type);
		}
	}

	private static void InitProfileDataArrayIfNeeded()
	{
		if (profileData != null)
		{
			return;
		}
		profileData = new string[profiles.Length];
		string earnCurrenyData = PlayerInfo.Instance.earnCurrenyData;
		if (string.IsNullOrEmpty(earnCurrenyData))
		{
			return;
		}
		string[] array = earnCurrenyData.Split(DATA_PROFILE_ALL_SPLITS, StringSplitOptions.None);
		for (int i = 0; i < array.Length - 1; i++)
		{
			string value = array[i];
			string text = array[i + 1];
			if (!Enum.IsDefined(typeof(Id), value))
			{
				continue;
			}
			Id id = (Id)(int)Enum.Parse(typeof(Id), value, true);
			for (int j = 0; j < profiles.Length; j++)
			{
				if (profiles[j].id == id)
				{
					profileData[j] = text;
					break;
				}
			}
		}
	}

	private static string GetProfileData(int profileIndex)
	{
		InitProfileDataArrayIfNeeded();
		return profileData[profileIndex];
	}

	private static void SetAndSaveProfileData(int profileIndex, string data)
	{
		InitProfileDataArrayIfNeeded();
		profileData[profileIndex] = data;
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < profileData.Length; i++)
		{
			if (profileData[i] != null)
			{
				stringBuilder.Append(profiles[i].id.ToString());
				stringBuilder.Append("=");
				stringBuilder.Append(profileData[i]);
				stringBuilder.Append(";");
			}
		}
		Debug.Log("EarnCurrenyInfo: Saving profile data: " + stringBuilder.ToString());
		PlayerInfo.Instance.earnCurrenyData = stringBuilder.ToString();
	}
}
