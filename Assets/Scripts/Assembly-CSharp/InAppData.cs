using System;
using System.Collections.Generic;
using System.Text;
using SYBO.Subway.Info;
using UnityEngine;

public static class InAppData
{
	public enum BundleTier
	{
		None = 0,
		BundleTier1 = 1,
		BundleTier2 = 2,
		BundleTier3 = 3,
		BundleTier4 = 4,
		BundleTier5 = 5
	}

	public const string inAppCharacterPrefix = "com.kiloo.subwaycharacter.";

	public const string inAppBoardPrefix = "com.kiloo.subwayboard.";

	public const string inAppTier1 = "com.kiloo.subways.coinstier1";

	public const string inAppTier2 = "com.kiloo.subways.coinstier2";

	public const string inAppTier3 = "com.kiloo.subways.coinstier3";

	public const string inAppTier4 = "com.kiloo.subways.coinstier4";

	public const string inAppTier5 = "com.kiloo.subways.coinstier5";

	public const string inAppTier6 = "com.kiloo.subways.keystier1";

	public const string inAppTier7 = "com.kiloo.subways.keystier2";

	public const string inAppTier8 = "com.kiloo.subways.keystier3";

	public const string inAppTier9 = "com.kiloo.subways.keystier4";

	public const string inAppDoubleCoins = "com.kiloo.subways.doublecoins";

	public const string inAppDoubleCoinsDiscount = "com.kiloo.subways.doublecoinsdiscount";

	public const string inAppTier1Discount = "com.kiloo.subways.coinstier1discount";

	public const string inAppTier2Discount = "com.kiloo.subways.coinstier2_discount";

	public const string inAppTier3Discount = "com.kiloo.subways.coinstier3discount";

	public const string inAppTier4Discount = "com.kiloo.subways.coinstier4discount";

	public const string inAppTier5Discount = "com.kiloo.subways.coinstier5discount";

	public const string inAppTier6Discount = "com.kiloo.subways.keystier1discount";

	public const string inAppTier7Discount = "com.kiloo.subways.keystier2discount";

	public const string inAppTier8Discount = "com.kiloo.subways.keystier3discount";

	public const string inAppTier9Discount = "com.kiloo.subways.keystier4discount";

	public const string inAppBundleTier1 = "com.kiloo.subways.bundletier1";

	public const string inAppBundleTier2 = "com.kiloo.subways.bundletier2";

	public const string inAppBundleTier3 = "com.kiloo.subways.bundletier3";

	public const string inAppBundleTier4 = "com.kiloo.subways.bundletier4";

	public const string inAppBundleTier5 = "com.kiloo.subways.bundletier5";

	private static readonly string[] inAppCurrencyTiers = new string[9] { "com.kiloo.subways.coinstier1", "com.kiloo.subways.coinstier2", "com.kiloo.subways.coinstier3", "com.kiloo.subways.coinstier4", "com.kiloo.subways.coinstier5", "com.kiloo.subways.keystier1", "com.kiloo.subways.keystier2", "com.kiloo.subways.keystier3", "com.kiloo.subways.keystier4" };

	private static readonly string[] inAppCurrencyDiscountTiers = new string[9] { "com.kiloo.subways.coinstier1discount", "com.kiloo.subways.coinstier2_discount", "com.kiloo.subways.coinstier3discount", "com.kiloo.subways.coinstier4discount", "com.kiloo.subways.coinstier5discount", "com.kiloo.subways.keystier1discount", "com.kiloo.subways.keystier2discount", "com.kiloo.subways.keystier3discount", "com.kiloo.subways.keystier4discount" };

	private static readonly string[] inAppBundleTiers = new string[5] { "com.kiloo.subways.bundletier1", "com.kiloo.subways.bundletier2", "com.kiloo.subways.bundletier3", "com.kiloo.subways.bundletier4", "com.kiloo.subways.bundletier5" };

	private static readonly Dictionary<string, string> inAppTiersOnlineSettingsIds = new Dictionary<string, string>
	{
		{ "com.kiloo.subways.coinstier1", "in_app_tier_1" },
		{ "com.kiloo.subways.coinstier2", "in_app_tier_2" },
		{ "com.kiloo.subways.coinstier3", "in_app_tier_3" },
		{ "com.kiloo.subways.coinstier4", "in_app_tier_4" },
		{ "com.kiloo.subways.coinstier5", "in_app_tier_5" },
		{ "com.kiloo.subways.keystier1", "in_app_tier_6" },
		{ "com.kiloo.subways.keystier2", "in_app_tier_7" },
		{ "com.kiloo.subways.keystier3", "in_app_tier_8" },
		{ "com.kiloo.subways.keystier4", "in_app_tier_9" },
		{ "com.kiloo.subways.doublecoins", "in_app_double_coins" }
	};

	private static readonly Dictionary<string, string> ignoredInAppTiersOnlineSettingsIds = new Dictionary<string, string>
	{
		{ "com.kiloo.subways.coinstier1", "ignore_in_app_tier_1" },
		{ "com.kiloo.subways.coinstier2", "ignore_in_app_tier_2" },
		{ "com.kiloo.subways.coinstier3", "ignore_in_app_tier_3" },
		{ "com.kiloo.subways.coinstier4", "ignore_in_app_tier_4" },
		{ "com.kiloo.subways.coinstier5", "ignore_in_app_tier_5" },
		{ "com.kiloo.subways.keystier1", "ignore_in_app_tier_6" },
		{ "com.kiloo.subways.keystier2", "ignore_in_app_tier_7" },
		{ "com.kiloo.subways.keystier3", "ignore_in_app_tier_8" },
		{ "com.kiloo.subways.keystier4", "ignore_in_app_tier_9" }
	};

	public static readonly List<string> LIST_OF_MANAGED_INAPP_IDS = new List<string> { "com.kiloo.subways.doublecoins", "com.kiloo.subways.doublecoinsdiscount" };

	public static Dictionary<string, InAppItemModel> inAppData = new Dictionary<string, InAppItemModel>
	{
		{
			"com.kiloo.subways.coinstier1",
			new InAppItemModel(string.Empty, string.Empty, "com.kiloo.subways.coinstier1", Strings.Get(StringID.INAPP_DATA_STACK_OF_COINS), string.Empty, "icon_coinPack_1", 7500, true, InAppItemModel.ItemType.Coins, InAppItemModel.OfferType.Normal, null, string.Empty)
		},
		{
			"com.kiloo.subways.coinstier2",
			new InAppItemModel(string.Empty, string.Empty, "com.kiloo.subways.coinstier2", Strings.Get(StringID.INAPP_DATA_PILE_OF_COINS), string.Empty, "icon_coinPack_2", 45000, true, InAppItemModel.ItemType.Coins, InAppItemModel.OfferType.Normal, null, string.Empty)
		},
		{
			"com.kiloo.subways.coinstier3",
			new InAppItemModel(string.Empty, string.Empty, "com.kiloo.subways.coinstier3", Strings.Get(StringID.INAPP_DATA_BAG_OF_COINS), string.Empty, "icon_coinPack_3", 180000, true, InAppItemModel.ItemType.Coins, InAppItemModel.OfferType.Normal, null, string.Empty)
		},
		{
			"com.kiloo.subways.coinstier4",
			new InAppItemModel(string.Empty, string.Empty, "com.kiloo.subways.coinstier4", Strings.Get(StringID.INAPP_DATA_CHEST_OF_COINS), string.Empty, "icon_coinPack_4", 500000, true, InAppItemModel.ItemType.Coins, InAppItemModel.OfferType.Normal, null, string.Empty)
		},
		{
			"com.kiloo.subways.coinstier5",
			new InAppItemModel(string.Empty, string.Empty, "com.kiloo.subways.coinstier5", Strings.Get(StringID.INAPP_DATA_VAULT_OF_COINS), string.Empty, "icon_coinPack_5", 1200000, true, InAppItemModel.ItemType.Coins, InAppItemModel.OfferType.Normal, null, string.Empty)
		},
		{
			"com.kiloo.subways.keystier1",
			new InAppItemModel(string.Empty, string.Empty, "com.kiloo.subways.keystier1", Strings.Get(StringID.INAPP_DATA_BUNCH_OF_KEYS), string.Empty, "icon_keyPack_01", 25, true, InAppItemModel.ItemType.Keys, InAppItemModel.OfferType.Normal, null, string.Empty)
		},
		{
			"com.kiloo.subways.keystier2",
			new InAppItemModel(string.Empty, string.Empty, "com.kiloo.subways.keystier2", Strings.Get(StringID.INAPP_DATA_HEAP_OF_KEYS), string.Empty, "icon_keyPack_02", 110, true, InAppItemModel.ItemType.Keys, InAppItemModel.OfferType.Normal, null, string.Empty)
		},
		{
			"com.kiloo.subways.keystier3",
			new InAppItemModel(string.Empty, string.Empty, "com.kiloo.subways.keystier3", Strings.Get(StringID.INAPP_DATA_SACK_OF_KEYS), string.Empty, "icon_keyPack_03", 300, true, InAppItemModel.ItemType.Keys, InAppItemModel.OfferType.Normal, null, string.Empty)
		},
		{
			"com.kiloo.subways.keystier4",
			new InAppItemModel(string.Empty, string.Empty, "com.kiloo.subways.keystier4", Strings.Get(StringID.INAPP_DATA_CRATE_OF_KEYS), string.Empty, "icon_keyPack_04", 650, true, InAppItemModel.ItemType.Keys, InAppItemModel.OfferType.Normal, null, string.Empty)
		},
		{
			"com.kiloo.subways.coinstier1discount",
			new InAppItemModel(string.Empty, string.Empty, "com.kiloo.subways.coinstier1discount", Strings.Get(StringID.INAPP_DATA_STACK_OF_COINS), string.Empty, "icon_coinPack_1", 7500, true, InAppItemModel.ItemType.Coins, InAppItemModel.OfferType.Sale, null, "com.kiloo.subways.coinstier1")
		},
		{
			"com.kiloo.subways.coinstier2_discount",
			new InAppItemModel(string.Empty, string.Empty, "com.kiloo.subways.coinstier2_discount", Strings.Get(StringID.INAPP_DATA_PILE_OF_COINS), string.Empty, "icon_coinPack_2", 45000, true, InAppItemModel.ItemType.Coins, InAppItemModel.OfferType.Sale, null, "com.kiloo.subways.coinstier2")
		},
		{
			"com.kiloo.subways.coinstier3discount",
			new InAppItemModel(string.Empty, string.Empty, "com.kiloo.subways.coinstier3discount", Strings.Get(StringID.INAPP_DATA_BAG_OF_COINS), string.Empty, "icon_coinPack_3", 180000, true, InAppItemModel.ItemType.Coins, InAppItemModel.OfferType.Sale, null, "com.kiloo.subways.coinstier3")
		},
		{
			"com.kiloo.subways.coinstier4discount",
			new InAppItemModel(string.Empty, string.Empty, "com.kiloo.subways.coinstier4discount", Strings.Get(StringID.INAPP_DATA_CHEST_OF_COINS), string.Empty, "icon_coinPack_4", 500000, true, InAppItemModel.ItemType.Coins, InAppItemModel.OfferType.Sale, null, "com.kiloo.subways.coinstier4")
		},
		{
			"com.kiloo.subways.coinstier5discount",
			new InAppItemModel(string.Empty, string.Empty, "com.kiloo.subways.coinstier5discount", Strings.Get(StringID.INAPP_DATA_VAULT_OF_COINS), string.Empty, "icon_coinPack_5", 1200000, true, InAppItemModel.ItemType.Coins, InAppItemModel.OfferType.Sale, null, "com.kiloo.subways.coinstier5")
		},
		{
			"com.kiloo.subways.keystier1discount",
			new InAppItemModel(string.Empty, string.Empty, "com.kiloo.subways.keystier1discount", Strings.Get(StringID.INAPP_DATA_BUNCH_OF_KEYS), string.Empty, "icon_keyPack_01", 25, true, InAppItemModel.ItemType.Keys, InAppItemModel.OfferType.Sale, null, "com.kiloo.subways.keystier1")
		},
		{
			"com.kiloo.subways.keystier2discount",
			new InAppItemModel(string.Empty, string.Empty, "com.kiloo.subways.keystier2discount", Strings.Get(StringID.INAPP_DATA_HEAP_OF_KEYS), string.Empty, "icon_keyPack_02", 110, true, InAppItemModel.ItemType.Keys, InAppItemModel.OfferType.Sale, null, "com.kiloo.subways.keystier2")
		},
		{
			"com.kiloo.subways.keystier3discount",
			new InAppItemModel(string.Empty, string.Empty, "com.kiloo.subways.keystier3discount", Strings.Get(StringID.INAPP_DATA_SACK_OF_KEYS), string.Empty, "icon_keyPack_03", 300, true, InAppItemModel.ItemType.Keys, InAppItemModel.OfferType.Sale, null, "com.kiloo.subways.keystier3")
		},
		{
			"com.kiloo.subways.keystier4discount",
			new InAppItemModel(string.Empty, string.Empty, "com.kiloo.subways.keystier4discount", Strings.Get(StringID.INAPP_DATA_CRATE_OF_KEYS), string.Empty, "icon_keyPack_04", 650, true, InAppItemModel.ItemType.Keys, InAppItemModel.OfferType.Sale, null, "com.kiloo.subways.keystier4")
		},
		{
			"com.kiloo.subways.doublecoins",
			new InAppItemModel(string.Empty, string.Empty, "com.kiloo.subways.doublecoins", Strings.Get(StringID.INAPP_DATA_DOUBLE_COINS), Strings.Get(StringID.INAPP_DATA_DOUBLE_COINS_DESCRIPTION), "icon_doubleCoins", 1, false, InAppItemModel.ItemType.DoubleCoins, InAppItemModel.OfferType.Normal, null, string.Empty)
		},
		{
			"com.kiloo.subways.doublecoinsdiscount",
			new InAppItemModel(string.Empty, string.Empty, "com.kiloo.subways.doublecoinsdiscount", Strings.Get(StringID.INAPP_DATA_DOUBLE_COINS), Strings.Get(StringID.INAPP_DATA_DOUBLE_COINS_DESCRIPTION), "icon_doubleCoins", 1, false, InAppItemModel.ItemType.DoubleCoins, InAppItemModel.OfferType.Sale, null, "com.kiloo.subways.doublecoins")
		},
		{
			"com.kiloo.subways.bundletier1",
			new InAppItemModel(string.Empty, string.Empty, "com.kiloo.subways.bundletier1", Strings.Get(StringID.INAPP_DATA_BUNDLE_TIER_ONE), string.Empty, "icon_bundle_01", 1, true, InAppItemModel.ItemType.Bundle, InAppItemModel.OfferType.Normal, new Dictionary<InAppItemModel.ItemType, int>
			{
				{
					InAppItemModel.ItemType.Coins,
					4000
				},
				{
					InAppItemModel.ItemType.Hoverboards,
					20
				},
				{
					InAppItemModel.ItemType.Keys,
					5
				}
			}, string.Empty)
		},
		{
			"com.kiloo.subways.bundletier2",
			new InAppItemModel(string.Empty, string.Empty, "com.kiloo.subways.bundletier2", Strings.Get(StringID.INAPP_DATA_BUNDLE_TIER_TWO), string.Empty, "icon_bundle_02", 1, true, InAppItemModel.ItemType.Bundle, InAppItemModel.OfferType.Normal, new Dictionary<InAppItemModel.ItemType, int>
			{
				{
					InAppItemModel.ItemType.Coins,
					30000
				},
				{
					InAppItemModel.ItemType.Hoverboards,
					50
				},
				{
					InAppItemModel.ItemType.Keys,
					10
				},
				{
					InAppItemModel.ItemType.Headstarts,
					10
				}
			}, string.Empty)
		},
		{
			"com.kiloo.subways.bundletier3",
			new InAppItemModel(string.Empty, string.Empty, "com.kiloo.subways.bundletier3", Strings.Get(StringID.INAPP_DATA_BUNDLE_TIER_THREE), string.Empty, "icon_bundle_03", 1, true, InAppItemModel.ItemType.Bundle, InAppItemModel.OfferType.Normal, new Dictionary<InAppItemModel.ItemType, int>
			{
				{
					InAppItemModel.ItemType.Coins,
					100000
				},
				{
					InAppItemModel.ItemType.Hoverboards,
					100
				},
				{
					InAppItemModel.ItemType.Keys,
					50
				},
				{
					InAppItemModel.ItemType.Headstarts,
					20
				}
			}, string.Empty)
		},
		{
			"com.kiloo.subways.bundletier4",
			new InAppItemModel(string.Empty, string.Empty, "com.kiloo.subways.bundletier4", Strings.Get(StringID.INAPP_DATA_BUNDLE_TIER_FOUR), string.Empty, "icon_bundle_04", 1, true, InAppItemModel.ItemType.Bundle, InAppItemModel.OfferType.Normal, new Dictionary<InAppItemModel.ItemType, int>
			{
				{
					InAppItemModel.ItemType.Coins,
					300000
				},
				{
					InAppItemModel.ItemType.Hoverboards,
					200
				},
				{
					InAppItemModel.ItemType.Keys,
					150
				},
				{
					InAppItemModel.ItemType.Headstarts,
					30
				},
				{
					InAppItemModel.ItemType.Scoreboosters,
					30
				}
			}, string.Empty)
		},
		{
			"com.kiloo.subways.bundletier5",
			new InAppItemModel(string.Empty, string.Empty, "com.kiloo.subways.bundletier5", Strings.Get(StringID.INAPP_DATA_BUNDLE_TIER_FIVE), string.Empty, "icon_bundle_05", 1, true, InAppItemModel.ItemType.Bundle, InAppItemModel.OfferType.Normal, new Dictionary<InAppItemModel.ItemType, int>
			{
				{
					InAppItemModel.ItemType.Coins,
					600000
				},
				{
					InAppItemModel.ItemType.Hoverboards,
					300
				},
				{
					InAppItemModel.ItemType.Keys,
					300
				},
				{
					InAppItemModel.ItemType.Headstarts,
					50
				},
				{
					InAppItemModel.ItemType.Scoreboosters,
					50
				}
			}, string.Empty)
		}
	};

	public static int InAppPurchaseCount
	{
		get
		{
			return inAppData.Count;
		}
	}

	public static string CommaSeparatedProductIds
	{
		get
		{
			string text = string.Empty;
			int num = 0;
			foreach (KeyValuePair<string, InAppItemModel> inAppDatum in inAppData)
			{
				if (num > 0)
				{
					text += ",";
				}
				text += inAppDatum.Key;
				num++;
			}
			return text;
		}
	}

	public static string GetBundleId(BundleTier tier)
	{
		switch (tier)
		{
		case BundleTier.None:
			Debug.LogError("Bundle ID could notbe found. BundleTier: " + tier);
			return "TierNotFound";
		case BundleTier.BundleTier1:
			return "com.kiloo.subways.bundletier1";
		case BundleTier.BundleTier2:
			return "com.kiloo.subways.bundletier2";
		case BundleTier.BundleTier3:
			return "com.kiloo.subways.bundletier3";
		case BundleTier.BundleTier4:
			return "com.kiloo.subways.bundletier4";
		case BundleTier.BundleTier5:
			return "com.kiloo.subways.bundletier5";
		default:
			Debug.LogError("Bundle ID could notbe found. BundleTier: " + tier);
			return "TierNotFound";
		}
	}

	public static string GetInAppOnlineSettingsId(string inAppId)
	{
		if (inAppTiersOnlineSettingsIds.ContainsKey(inAppId))
		{
			return inAppTiersOnlineSettingsIds[inAppId];
		}
		return null;
	}

	public static string GetIgnoredInAppOnlineSettingsId(string inAppId)
	{
		if (ignoredInAppTiersOnlineSettingsIds.ContainsKey(inAppId))
		{
			return ignoredInAppTiersOnlineSettingsIds[inAppId];
		}
		return null;
	}

	public static string GetDiscountIdForNormalOffer(string normalOffer)
	{
		string[] currencyDiscountTiers = GetCurrencyDiscountTiers();
		string[] array = currencyDiscountTiers;
		foreach (string text in array)
		{
			if (inAppData[text].NormalOfferId == normalOffer)
			{
				return text;
			}
		}
		Debug.LogWarning("No discount Id found for normal offer: " + normalOffer);
		return string.Empty;
	}

	public static string[] getInAppBundleTiers()
	{
		return inAppBundleTiers;
	}

	public static bool InappidIsDynamicLimitedCharacterPurchase(string inappId)
	{
		if (inappId.StartsWith("com.kiloo.subwaycharacter."))
		{
			return true;
		}
		return false;
	}

	public static bool InappidIsDynamicLimitedBoardPurchase(string inappId)
	{
		if (inappId.StartsWith("com.kiloo.subwayboard."))
		{
			return true;
		}
		return false;
	}

	public static string[] GetCurrencyTiers()
	{
		return inAppCurrencyTiers;
	}

	public static string[] GetCurrencyDiscountTiers()
	{
		return inAppCurrencyDiscountTiers;
	}

	public static string[] GetAllProductIds()
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, InAppItemModel> inAppDatum in inAppData)
		{
			list.Add(inAppDatum.Key);
		}
		foreach (Characters.CharacterType item3 in Characters.characterOrder)
		{
			if (Characters.characterData[item3].hasOnlineSettings)
			{
				string item = ConvertCharacterTypeToDynamicPurchaseId(item3);
				list.Add(item);
			}
		}
		foreach (Hoverboards.BoardType item4 in Hoverboards.boardOrder)
		{
			if (Hoverboards.boardData[item4].hasOnlineTimeLimit || Hoverboards.boardData[item4].unlockType == Hoverboards.UnlockType.hiddenUntillUnlocked)
			{
				string item2 = ConvertBoardTypeToDynamicPurchaseId(item4);
				list.Add(item2);
			}
		}
		return list.ToArray();
	}

	public static bool TryConvertDynamicPurchaseIdToCharacterType(string purchaseId, out Characters.CharacterType charReturned)
	{
		string value = purchaseId.Replace("com.kiloo.subwaycharacter.", string.Empty);
		try
		{
			charReturned = (Characters.CharacterType)(int)Enum.Parse(typeof(Characters.CharacterType), value, true);
			return true;
		}
		catch (Exception ex)
		{
			Debug.LogWarning("InAppData: ConvertDynamicPurchaseIdToCharacterType conversion failed. Must be a custom dynamic ID: " + ex.ToString());
			foreach (KeyValuePair<Characters.CharacterType, Characters.Model> characterDatum in Characters.characterData)
			{
				Characters.Model value2 = characterDatum.Value;
				string dynamicIapId = value2.dynamicIapId;
				if (!string.IsNullOrEmpty(dynamicIapId) && dynamicIapId.ToLower() == purchaseId.ToLower() && IsUsingCustomIapIdForCharacter(value2))
				{
					charReturned = characterDatum.Key;
					return true;
				}
			}
		}
		charReturned = Characters.CharacterType.slick;
		return false;
	}

	public static string ConvertCharacterTypeToDynamicPurchaseId(Characters.CharacterType charType)
	{
		Characters.Model model = Characters.characterData[charType];
		string text = ((!IsUsingCustomIapIdForCharacter(model)) ? ("com.kiloo.subwaycharacter." + charType) : model.dynamicIapId);
		if (Application.platform == RuntimePlatform.Android && !DeviceInfo.isKindleDevice())
		{
			text = text.ToLower();
		}
		return text;
	}

	private static bool IsUsingCustomIapIdForCharacter(Characters.Model model)
	{
		bool result = false;
		switch (Application.platform)
		{
		case RuntimePlatform.IPhonePlayer:
			if (model.overrideDynamicIapIdiOS)
			{
				result = true;
			}
			break;
		case RuntimePlatform.Android:
			if (DeviceInfo.isKindleDevice())
			{
				if (model.overrideDynamicIapIdKindle)
				{
					result = true;
				}
			}
			else if (model.overrideDynamicIapIdAndroid)
			{
				result = true;
			}
			break;
		case RuntimePlatform.WP8Player:
			if (model.overrideDynamicIapIdWP8)
			{
				result = true;
			}
			break;
		}
		return result;
	}

	public static void PrintDynamicIdsForCharacterTypeForAllPlatforms(Characters.CharacterType characterType)
	{
		StringBuilder stringBuilder = new StringBuilder();
		Characters.Model model = Characters.characterData[characterType];
		string text = Strings.Get(model.name);
		string dynamicIapId = model.dynamicIapId;
		string text2 = "com.kiloo.subwaycharacter." + characterType;
		stringBuilder.AppendLine(string.Format("iOS Id for " + text + ": {0}", (!model.overrideDynamicIapIdiOS) ? text2 : dynamicIapId));
		stringBuilder.AppendLine(string.Format("Android Id for " + text + ": {0}", (!model.overrideDynamicIapIdAndroid) ? text2.ToLower() : dynamicIapId.ToLower()));
		stringBuilder.AppendLine(string.Format("Kindle Id for " + text + ": {0}", (!model.overrideDynamicIapIdKindle) ? text2 : dynamicIapId));
		stringBuilder.AppendLine(string.Format("Windows Phone8 Id for " + text + ": {0}", (!model.overrideDynamicIapIdWP8) ? text2 : dynamicIapId));
		Debug.Log(stringBuilder.ToString());
	}

	public static bool TryConvertDynamicBoardIdToBoardType(string purchaseId, out Hoverboards.BoardType boardReturned)
	{
		string value = purchaseId.Replace("com.kiloo.subwayboard.", string.Empty);
		try
		{
			boardReturned = (Hoverboards.BoardType)(int)Enum.Parse(typeof(Hoverboards.BoardType), value, true);
			return true;
		}
		catch (Exception ex)
		{
			Debug.LogError("InAppData: ConvertDynamicBoardIdToBoardType conversion failed. Must be a custom dynamic ID: " + ex.ToString());
			foreach (KeyValuePair<Hoverboards.BoardType, Hoverboards.Board> boardDatum in Hoverboards.boardData)
			{
				Hoverboards.Board value2 = boardDatum.Value;
				string dynamicIapId = value2.dynamicIapId;
				if (!string.IsNullOrEmpty(dynamicIapId) && dynamicIapId.ToLower() == purchaseId.ToLower() && IsUsingCustomIapIdForBoard(value2))
				{
					boardReturned = boardDatum.Key;
					return true;
				}
			}
		}
		boardReturned = Hoverboards.BoardType.normal;
		return false;
	}

	public static string ConvertBoardTypeToDynamicPurchaseId(Hoverboards.BoardType boardType)
	{
		string empty = string.Empty;
		Hoverboards.Board board = Hoverboards.boardData[boardType];
		empty = ((!IsUsingCustomIapIdForBoard(board)) ? ("com.kiloo.subwayboard." + boardType) : board.dynamicIapId);
		if (Application.platform == RuntimePlatform.Android && !DeviceInfo.isKindleDevice())
		{
			empty = empty.ToLower();
		}
		return empty;
	}

	private static bool IsUsingCustomIapIdForBoard(Hoverboards.Board board)
	{
		bool result = false;
		switch (Application.platform)
		{
		case RuntimePlatform.IPhonePlayer:
			if (board.overrideDynamicIapIdiOS)
			{
				result = true;
			}
			break;
		case RuntimePlatform.Android:
			if (DeviceInfo.isKindleDevice())
			{
				if (board.overrideDynamicIapIdKindle)
				{
					result = true;
				}
			}
			else if (board.overrideDynamicIapIdAndroid)
			{
				result = true;
			}
			break;
		case RuntimePlatform.WP8Player:
			if (board.overrideDynamicIapIdWP8)
			{
				result = true;
			}
			break;
		}
		return result;
	}

	public static void PrintDynamicIdsForBoardTypeForAllPlatforms(Hoverboards.BoardType boardType)
	{
		StringBuilder stringBuilder = new StringBuilder();
		Hoverboards.Board board = Hoverboards.boardData[boardType];
		string text = Strings.Get(board.name);
		string dynamicIapId = board.dynamicIapId;
		string text2 = "com.kiloo.subwayboard." + boardType;
		stringBuilder.AppendLine(string.Format("iOS Id for " + text + ": {0}", (!board.overrideDynamicIapIdiOS) ? text2 : dynamicIapId));
		stringBuilder.AppendLine(string.Format("Android Id for " + text + ": {0}", (!board.overrideDynamicIapIdAndroid) ? text2.ToLower() : dynamicIapId.ToLower()));
		stringBuilder.AppendLine(string.Format("Kindle Id for " + text + ": {0}", (!board.overrideDynamicIapIdKindle) ? text2 : dynamicIapId));
		stringBuilder.AppendLine(string.Format("Windows Phone8 Id for " + text + ": {0}", (!board.overrideDynamicIapIdWP8) ? text2 : dynamicIapId));
		Debug.Log(stringBuilder.ToString());
	}
}
