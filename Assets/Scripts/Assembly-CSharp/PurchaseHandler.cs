using System;
using SYBO.Subway.Info;
using UnityEngine;

public class PurchaseHandler
{
	private HoverboardSelectButton btnBuyItemWithInapp;

	private static PurchaseHandler _instance;

	public static PurchaseHandler Instance
	{
		get
		{
			return _instance ?? (_instance = new PurchaseHandler());
		}
	}

	public event Action<PowerupType, int> onUpgradePurchase;

	private event Action _onFailedToPurchaseEventItemWithInapp;

	public void TriggerOnFailedToPurchaseEventItemWithInapp()
	{
		if (this._onFailedToPurchaseEventItemWithInapp != null)
		{
			this._onFailedToPurchaseEventItemWithInapp();
			ResetOnFailedToPurchaseEventItemWithInapp();
		}
	}

	public void ResetOnFailedToPurchaseEventItemWithInapp()
	{
		this._onFailedToPurchaseEventItemWithInapp = null;
	}

	public void PurchaseCharacter(Characters.CharacterType characterType, int themeVersion)
	{
		CharacterScreenManager instance = CharacterScreenManager.Instance;
		Characters.Model model = Characters.characterData[characterType];
		CharacterTheme? themeForCharacter = CharacterThemes.GetThemeForCharacter(characterType, themeVersion);
		bool hasValue = themeForCharacter.HasValue;
		Characters.UnlockType unlockType = model.unlockType;
		int price = model.Price;
		if (hasValue)
		{
			CharacterTheme value = themeForCharacter.Value;
			unlockType = value.unlockType;
			price = value.price;
		}
		if (unlockType != Characters.UnlockType.coins && unlockType != Characters.UnlockType.keys)
		{
			Debug.LogWarning("Cannot buy character with unlocktype: " + model.unlockType);
			instance.CharacterPurchaseFailure();
			return;
		}
		CheckForDebugFreePurchases(unlockType, price);
		int num = PlayerInfo.Instance.amountOfCoins;
		InAppItemModel.ItemType type = InAppItemModel.ItemType.Coins;
		if (unlockType == Characters.UnlockType.keys)
		{
			num = PlayerInfo.Instance.amountOfKeys;
			type = InAppItemModel.ItemType.Keys;
		}
		if (num >= price)
		{
			switch (unlockType)
			{
			case Characters.UnlockType.coins:
				MissionsManager.Instance.PlayerDidThis(MissionTarget.SpendCoin, price);
				PlayerInfo.Instance.amountOfCoins -= price;
				if (Characters.characterData[characterType].missionTargetKey != MissionTarget.none)
				{
					MissionsManager.Instance.PlayerDidThis(Characters.characterData[characterType].missionTargetKey);
				}
				break;
			case Characters.UnlockType.keys:
				MissionsManager.Instance.PlayerDidThis(MissionTarget.SpendKeys, price);
				PlayerInfo.Instance.amountOfKeys -= price;
				break;
			}
			if (hasValue)
			{
				PlayerInfo.Instance.UnlockTheme(characterType, themeVersion);
			}
			else
			{
				PlayerInfo.Instance.CollectToken(characterType, price);
			}
			instance.CharacterPurchaseSuccessful(characterType, themeVersion);
		}
		else
		{
			InAppManager.instance.SuggestInAppPopup(price, Strings.Get(model.name), type);
			instance.CharacterPurchaseFailure();
		}
	}

	public void StartPurchaseCharacterWithInapp(Characters.CharacterType characterType)
	{
		this._onFailedToPurchaseEventItemWithInapp = OnPurchaseCharacterWithInappFailed;
		Characters.Model model = Characters.characterData[characterType];
		string inappId = InAppData.ConvertCharacterTypeToDynamicPurchaseId(characterType);
		string characterName = Strings.Get(model.name);
		if (Globals.DEBUG_USE_EVENT_TESTER)
		{
			InAppData.PrintDynamicIdsForCharacterTypeForAllPlatforms(characterType);
		}
		InAppManager.instance.BuyExpiredLimitedCharacter(inappId, characterName, this);
	}

	public void OnPurchaseCharacterWithInappSucceded(Characters.CharacterType characterPurchased, string inappId)
	{
		CharacterScreenManager.Instance.CharacterPurchaseSuccessful(characterPurchased, 0);
	}

	public void OnPurchaseCharacterWithInappFailed()
	{
		CharacterScreenManager.Instance.CharacterPurchaseFailure();
	}

	private static void CheckForDebugFreePurchases(Characters.UnlockType unlockType, int price)
	{
		if (!Globals.DEBUG_FREE_PURCHASES)
		{
			return;
		}
		switch (unlockType)
		{
		case Characters.UnlockType.coins:
			if (PlayerInfo.Instance.amountOfCoins < price)
			{
				PlayerInfo.Instance.amountOfCoins += price;
			}
			break;
		case Characters.UnlockType.keys:
			if (PlayerInfo.Instance.amountOfKeys < price)
			{
				PlayerInfo.Instance.amountOfKeys += price;
			}
			break;
		}
	}

	public void OnPurchaseBoardWithInappSucceded(string inappId)
	{
		if (btnBuyItemWithInapp != null)
		{
			btnBuyItemWithInapp.PurchaseSuccessful();
		}
	}

	public void OnPurchaseBoardWithInappFailed()
	{
		if (btnBuyItemWithInapp != null)
		{
			btnBuyItemWithInapp.PurchaseFailure();
		}
	}

	public void StartPurchaseBoardWithInapp(Hoverboards.BoardType boardType, HoverboardSelectButton sender)
	{
		this._onFailedToPurchaseEventItemWithInapp = OnPurchaseBoardWithInappFailed;
		btnBuyItemWithInapp = sender;
		string inappId = InAppData.ConvertBoardTypeToDynamicPurchaseId(boardType);
		string boardName = Strings.Get(Hoverboards.boardData[boardType].name);
		if (Globals.DEBUG_USE_EVENT_TESTER)
		{
			InAppData.PrintDynamicIdsForBoardTypeForAllPlatforms(boardType);
		}
		InAppManager.instance.BuyExpiredLimitedBoard(inappId, boardName, this);
	}

	public void PurchaseHoverboard(BuyHoverboardButton sender)
	{
		int price = Upgrades.upgrades[PowerupType.hoverboard].getPrice(0);
		if (PlayerInfo.Instance.amountOfCoins >= price)
		{
			PlayerInfo.Instance.IncreaseUpgradeAmount(PowerupType.hoverboard);
			MissionsManager.Instance.PlayerDidThis(MissionTarget.SpendCoin, price);
			PlayerInfo.Instance.amountOfCoins -= price;
			sender.PurchaseSuccessful();
			Action<PowerupType, int> action = this.onUpgradePurchase;
			if (action != null)
			{
				action(PowerupType.hoverboard, PlayerInfo.Instance.GetUpgradeAmount(PowerupType.hoverboard));
			}
		}
		else
		{
			InAppManager.instance.SuggestInAppPopup(price, "Hoverboard", InAppItemModel.ItemType.Coins);
			sender.PurchaseFailure();
		}
	}

	public void PurchaseKeysIfNeeded(int amountToSaveMe)
	{
		InAppManager.instance.SuggestInAppPopup(amountToSaveMe, "Keys popup", InAppItemModel.ItemType.Keys);
	}

	public void PurchaseHoverboardType(Hoverboards.BoardType boardType, HoverboardSelectButton sender)
	{
		Hoverboards.Board board = Hoverboards.boardData[boardType];
		if (board.unlockType == Hoverboards.UnlockType.free)
		{
			PlayerInfo.Instance.UnlockHoverboard(boardType);
			sender.PurchaseSuccessful();
		}
		else if (board.unlockType == Hoverboards.UnlockType.coins)
		{
			int price = board.price;
			if (Globals.DEBUG_FREE_PURCHASES && PlayerInfo.Instance.amountOfCoins < price)
			{
				PlayerInfo.Instance.amountOfCoins += price;
			}
			if (PlayerInfo.Instance.amountOfCoins >= price)
			{
				MissionsManager.Instance.PlayerDidThis(MissionTarget.SpendCoin, price);
				PlayerInfo.Instance.UnlockHoverboard(boardType);
				PlayerInfo.Instance.amountOfCoins -= price;
				sender.PurchaseSuccessful();
			}
			else
			{
				InAppManager.instance.SuggestInAppPopup(price, Strings.Get(board.name), InAppItemModel.ItemType.Coins);
				sender.PurchaseFailure();
			}
		}
		else if (board.unlockType == Hoverboards.UnlockType.eggHunt)
		{
			if (!EggHuntManager.TryToUnlockEggBoard())
			{
				sender.PurchaseFailure();
			}
			else
			{
				sender.PurchaseSuccessful();
			}
		}
	}

	public void PurchaseHoverboardTheme(Hoverboards.BoardType boardType, int themeIndex, HoverboardSelectButton sender)
	{
		HoverboardThemes.HoverboardTheme hoverboardTheme = HoverboardThemes.hoverboardCustomThemes[boardType][themeIndex];
		if (hoverboardTheme.unlockType == Hoverboards.UnlockType.free)
		{
			PlayerInfo.Instance.UnlockHoverboard(boardType);
			PlayerInfo.Instance.HoverboardCustomizationData.SetThemeAcquired(boardType, HoverboardThemes.GetThemeIdentifier(boardType, themeIndex));
			PlayerInfo.Instance.HoverboardCustomizationData.SetThemeActive(boardType, HoverboardThemes.GetThemeIdentifier(boardType, themeIndex), true);
			sender.PurchaseSuccessful(true, themeIndex);
		}
		else if (hoverboardTheme.unlockType == Hoverboards.UnlockType.coins)
		{
			int price = hoverboardTheme.price;
			if (Globals.DEBUG_FREE_PURCHASES && PlayerInfo.Instance.amountOfCoins < price)
			{
				PlayerInfo.Instance.amountOfCoins += price;
			}
			if (PlayerInfo.Instance.amountOfCoins >= price)
			{
				MissionsManager.Instance.PlayerDidThis(MissionTarget.SpendCoin, price);
				PlayerInfo.Instance.HoverboardCustomizationData.SetThemeAcquired(boardType, HoverboardThemes.GetThemeIdentifier(boardType, themeIndex));
				PlayerInfo.Instance.HoverboardCustomizationData.SetThemeActive(boardType, HoverboardThemes.GetThemeIdentifier(boardType, themeIndex), true);
				PlayerInfo.Instance.amountOfCoins -= price;
				sender.PurchaseSuccessful(true, themeIndex);
			}
			else
			{
				InAppManager.instance.SuggestInAppPopup(price, Strings.Get(hoverboardTheme.title), InAppItemModel.ItemType.Coins);
				sender.PurchaseFailure();
			}
		}
		else if (hoverboardTheme.unlockType == Hoverboards.UnlockType.keys)
		{
			int price2 = hoverboardTheme.price;
			if (Globals.DEBUG_FREE_PURCHASES && PlayerInfo.Instance.amountOfKeys < price2)
			{
				PlayerInfo.Instance.amountOfKeys += price2;
			}
			if (PlayerInfo.Instance.amountOfKeys >= price2)
			{
				MissionsManager.Instance.PlayerDidThis(MissionTarget.SpendKeys, price2);
				PlayerInfo.Instance.HoverboardCustomizationData.SetThemeAcquired(boardType, HoverboardThemes.GetThemeIdentifier(boardType, themeIndex));
				PlayerInfo.Instance.amountOfKeys -= price2;
				sender.PurchaseSuccessful(true, themeIndex);
			}
			else
			{
				InAppManager.instance.SuggestInAppPopup(price2, Strings.Get(hoverboardTheme.title), InAppItemModel.ItemType.Keys);
				sender.PurchaseFailure();
			}
		}
	}

	public void PurchaseUpgrade(PowerupType type, BuyButtonIngame sender)
	{
		int num = ((Upgrades.upgrades[type].numberOfTiers != 0) ? Upgrades.upgrades[type].getPrice(PlayerInfo.Instance.GetCurrentTier(type) + 1) : Upgrades.upgrades[type].getPrice(0));
		if (Globals.DEBUG_FREE_PURCHASES && PlayerInfo.Instance.amountOfCoins < num)
		{
			PlayerInfo.Instance.amountOfCoins += num;
		}
		if (PlayerInfo.Instance.amountOfCoins >= num)
		{
			switch (type)
			{
			case PowerupType.hoverboard:
			case PowerupType.headstart500:
			case PowerupType.headstart2000:
			case PowerupType.scorebooster:
				PlayerInfo.Instance.IncreaseUpgradeAmount(type);
				MissionsManager.Instance.PlayerDidThis(MissionTarget.SpendCoin, num);
				if (type == PowerupType.headstart2000)
				{
					MissionsManager.Instance.PlayerDidThis(MissionTarget.HaveHeadStartLarge);
				}
				break;
			case PowerupType.jetpack:
			case PowerupType.supersneakers:
			case PowerupType.coinmagnet:
			case PowerupType.letters:
			case PowerupType.doubleMultiplier:
				PlayerInfo.Instance.IncreasePowerupTier(type);
				MissionsManager.Instance.PlayerDidThis(MissionTarget.SpendCoin, num);
				MissionsManager.Instance.PlayerDidThis(MissionTarget.HaveUpgrades);
				break;
			case PowerupType.mysterybox:
				MissionsManager.Instance.PlayerDidThis(MissionTarget.SpendCoin, num);
				MissionsManager.Instance.PlayerDidThis(MissionTarget.BuyMysterybox);
				break;
			case PowerupType.skipmission1:
				MissionsManager.Instance.PlayerDidThis(MissionTarget.SpendCoin, num, 0);
				break;
			case PowerupType.skipmission2:
				MissionsManager.Instance.PlayerDidThis(MissionTarget.SpendCoin, num, 1);
				break;
			case PowerupType.skipmission3:
				MissionsManager.Instance.PlayerDidThis(MissionTarget.SpendCoin, num, 2);
				break;
			}
			PlayerInfo.Instance.amountOfCoins -= num;
			sender.PurchaseSuccessful();
			Action<PowerupType, int> action = this.onUpgradePurchase;
			if (action != null)
			{
				if (PlayerInfo.Instance.IsUpgradeStackable(type))
				{
					action(type, PlayerInfo.Instance.GetUpgradeAmount(type));
				}
				else
				{
					action(type, 0);
				}
			}
		}
		else
		{
			InAppManager.instance.SuggestInAppPopup(num, Strings.Get(Upgrades.upgrades[type].name), InAppItemModel.ItemType.Coins);
			sender.PurchaseFailure();
		}
	}
}
