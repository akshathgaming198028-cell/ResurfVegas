using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using InAppPurchase;
using JetBrains.Annotations;
using SYBO.Core.Utilities;
using SYBO.Subway.Info;
using UnityEngine;

public class InAppManager : MonoBehaviour, IInAppManager
{
	public delegate void OnProductRequestDone();

	public delegate void OnPurchaseSuccess(string productId);

	public delegate void OnPurchaseFailure(string productId, PurchaseResult result);

	public delegate void OnPurchaseCanceled(string productId, PurchaseResult result);

	public delegate void OnPurchaseItemNotAvailable(string productId, PurchaseResult result);

	public delegate void OnInitializeFinished(bool success);

	public delegate void OnItemRestored(string productId);

	public delegate void OnItemRestoreFinished(RestoreFinishedResult result);

	public delegate void OnItemRefunded(string productId);

	private const string INAPP_API_KEY_ANDROID = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAnV3V5Ht4U8dR2R/R7Fvk4aIyNtuTZk90drulZJrQ8a7v3FtSENNAHqj8v104Snrc+4ICGGixqYlf73m2jlDYjTF5N0fKfWiujyJrnL9+FEOYOudgwQCx9qmrIBbICnodqecNJeWnxwutuiFdQ2/WOwC1bqHVVws8Qz+KQNqQ86gFQRM1kBLZaEg8tl5n2WsEddqJ7T+/TheYtmimoyEe9xxXr8vB0KynXez3RwIyUjvbVDWW7tlAbUjROeIELDUWMkARfDCem8xlbAoDUiepNF3LDHOYUDcCkgcdi6HK0SSj4FrWW9hrrhxMjRxlMnhKuKP2n1ws7b4cwOjMPCaZUwIDAQAB";

	private const string GAME_OBJECT_NAME = "InAppManager";

	private static InAppManager _instance;

	private static PurchaseHandler _purchaseHandler;

	private string _itemRequested = string.Empty;

	private string _inAppPurchaseKey = string.Empty;

	private bool _productRequestSucceeded;

	private static bool _isRestoredManually;

	private InAppManagerPopupData _popupData;

	[NonSerialized]
	public AudioClip purchaseSuccessSound;

	private static bool _hasSuccessfullyInitialized;

	private static bool _applicationIsQuiting;

	private bool _isQueryingProductInfo;

	public static bool isInitialized
	{
		get
		{
			return _instance != null && _hasSuccessfullyInitialized;
		}
	}

	public static InAppManager instance
	{
		get
		{
			Init();
			return _instance;
		}
	}

	public static bool IsAnyDiscountAvailable
	{
		get
		{
			double value = OnlineSettings.instance.GetValue("discount_end_time", 0.0);
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(value);
			if ((dateTime - DateTime.UtcNow).Ticks > 0)
			{
				return true;
			}
			return false;
		}
	}

	public static bool IsDoubleCoinsDiscountActive
	{
		get
		{
			if (!PlayerInfo.Instance.hasDoubleCoins && IsAnyDiscountAvailable && OnlineSettings.instance.GetValue(InAppData.GetInAppOnlineSettingsId("com.kiloo.subways.doublecoins"), 0) < 0 && InAppData.inAppData["com.kiloo.subways.doublecoinsdiscount"].IsValidated)
			{
				return true;
			}
			return false;
		}
	}

	public static bool IsAnyCurrencyDiscountActive
	{
		get
		{
			string[] currencyTiers = InAppData.GetCurrencyTiers();
			foreach (string text in currencyTiers)
			{
				if (!IsAnyDiscountAvailable)
				{
					continue;
				}
				string inAppOnlineSettingsId = InAppData.GetInAppOnlineSettingsId(text);
				if (!string.IsNullOrEmpty(inAppOnlineSettingsId) && OnlineSettings.instance.HasValue(inAppOnlineSettingsId))
				{
					string discountIdForNormalOffer = InAppData.GetDiscountIdForNormalOffer(text);
					if (InAppData.inAppData[discountIdForNormalOffer].IsValidated)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	public event OnProductRequestDone onProductRequestDone;

	public event OnPurchaseSuccess onPurchaseSuccess;

	public event OnPurchaseFailure onPurchaseFailure;

	public event OnPurchaseCanceled onPurchaseCanceled;

	public event OnPurchaseItemNotAvailable onPurchaseItemNotAvailable;

	public event OnItemRestored onItemRestored;

	public event OnItemRefunded onItemRefunded;

	public event OnItemRestoreFinished onItemRestoreFinished;

	public static event OnInitializeFinished onInitializeFinished;

	public static void Init()
	{
		if (_instance == null && !_applicationIsQuiting)
		{
			GameObject gameObject = new GameObject();
			gameObject.name = "InAppManager";
			GameObject gameObject2 = gameObject;
			UnityEngine.Object.DontDestroyOnLoad(gameObject2);
			gameObject2.AddComponent<InAppManager>();
			HierarchyUtility.ReparentByHierarchyPathType(gameObject2.transform, HierarchyPathType.DynamicManagers);
		}
	}

	private void Awake()
	{
		if (_instance != null)
		{
			UnityEngine.Object.DestroyImmediate(this);
			return;
		}
		_instance = this;
		CreateDynamicInAppData();
		InAppPurchaseProvider.instance.Init(this);
	}

	[UsedImplicitly]
	private void OnApplicationQuit()
	{
		_applicationIsQuiting = true;
		_instance = null;
	}

	private void CreateDynamicInAppData()
	{
		string[] allProductIds = InAppData.GetAllProductIds();
		string[] array = allProductIds;
		foreach (string key in array)
		{
			if (!InAppData.inAppData.ContainsKey(key))
			{
				InAppItemModel inAppItemModel = InAppItemModel.CreateEmpty();
				inAppItemModel.IsDynamic = true;
				inAppItemModel.IsValidated = false;
				InAppData.inAppData.Add(key, inAppItemModel);
			}
		}
	}

	public void RequestIAPInfo()
	{
		if (_isQueryingProductInfo)
		{
			return;
		}
		_isQueryingProductInfo = true;
		foreach (KeyValuePair<string, InAppItemModel> inAppDatum in InAppData.inAppData)
		{
			inAppDatum.Value.IsValidated = false;
		}
		InAppPurchaseProvider.instance.RequestProductInfo(InAppData.GetAllProductIds());
	}

	public void StartPurchase(string productId)
	{
		if (Globals.DEBUG_FREE_INAPP_PURCHASE)
		{
			PayoutInAppPurchase(productId);
			return;
		}
		if (UIScreenController.IsInstanced)
		{
			UIScreenController.Instance.ShowInAppPurchaseOverlay(GetContactingStoreOverlayText());
		}
		InAppPurchaseProvider.instance.StartPurchase(productId);
	}

	public bool RestoreNonConsumablePurchases(string[] productIds)
	{
		if (UIScreenController.IsInstanced)
		{
			UIScreenController.Instance.ShowInAppPurchaseOverlay(GetContactingStoreOverlayText());
		}
		_isRestoredManually = true;
		return InAppPurchaseProvider.instance.RestoreNonConsumablePurchases(productIds);
	}

	public bool ProductRequestSucceeded()
	{
		return _productRequestSucceeded;
	}

	public void InitFinishedCallback(bool success)
	{
		_hasSuccessfullyInitialized = success;
		OnInitializeFinished onInitializeFinished = InAppManager.onInitializeFinished;
		if (onInitializeFinished != null)
		{
			onInitializeFinished(success);
		}
		if (success)
		{
			RequestIAPInfo();
		}
	}

	public string getInAppAPIKey()
	{
		return "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAnV3V5Ht4U8dR2R/R7Fvk4aIyNtuTZk90drulZJrQ8a7v3FtSENNAHqj8v104Snrc+4ICGGixqYlf73m2jlDYjTF5N0fKfWiujyJrnL9+FEOYOudgwQCx9qmrIBbICnodqecNJeWnxwutuiFdQ2/WOwC1bqHVVws8Qz+KQNqQ86gFQRM1kBLZaEg8tl5n2WsEddqJ7T+/TheYtmimoyEe9xxXr8vB0KynXez3RwIyUjvbVDWW7tlAbUjROeIELDUWMkARfDCem8xlbAoDUiepNF3LDHOYUDcCkgcdi6HK0SSj4FrWW9hrrhxMjRxlMnhKuKP2n1ws7b4cwOjMPCaZUwIDAQAB";
	}

	public IsIAPConsumableValue IsProductConsumable(string productID)
	{
		return (!InAppData.inAppData[productID].IsConsumable) ? IsIAPConsumableValue.NotConsumable : IsIAPConsumableValue.Consumable;
	}

	public void ProductInfoRequestCallback(bool success, ProductRequestResultInfo[] productInfo)
	{
		if (success)
		{
			_productRequestSucceeded = true;
			for (int i = 0; i < productInfo.Length; i++)
			{
				ProductRequestResultInfo productRequestResultInfo = productInfo[i];
				string productId = productRequestResultInfo.productId;
				if (InAppData.inAppData.ContainsKey(productId))
				{
					InAppData.inAppData[productId].IsValidated = true;
					InAppData.inAppData[productId].LocalizedPrice = productRequestResultInfo.localizedPrice;
					if (!string.IsNullOrEmpty(InAppData.inAppData[productId].LocalizedPrice))
					{
						InAppData.inAppData[productId].LocalizedPrice = Regex.Replace(InAppData.inAppData[productId].LocalizedPrice, "\\u00A0", " ");
					}
					else
					{
						LogError("InAppManager.ProductInfoRequestCallback(): Null or Empty Localized Price");
					}
					continue;
				}
				InAppItemModel inAppItemModel = InAppItemModel.CreateEmpty();
				inAppItemModel.IsDynamic = true;
				inAppItemModel.LocalizedPrice = productRequestResultInfo.localizedPrice;
				if (!string.IsNullOrEmpty(inAppItemModel.LocalizedPrice))
				{
					inAppItemModel.LocalizedPrice = Regex.Replace(inAppItemModel.LocalizedPrice, "\\u00A0", " ");
				}
				else
				{
					LogError("InAppManager.ProductInfoRequestCallback(): Null or Empty Localized Price");
				}
				inAppItemModel.IsValidated = true;
				InAppData.inAppData.Add(productId, inAppItemModel);
			}
		}
		else
		{
			_productRequestSucceeded = false;
		}
		OnProductRequestDone onProductRequestDone = this.onProductRequestDone;
		if (onProductRequestDone != null)
		{
			onProductRequestDone();
		}
		_isQueryingProductInfo = false;
	}

	public void PurchaseRequestCallback(string productId, string transactionId, string transactionIdBase64, string cookie, PurchaseResult result, ValidationResult validationResult)
	{
		switch (result)
		{
		case PurchaseResult.Completed:
			if (PlayerInfo.Instance.AddTransactionToHistory(transactionId, productId))
			{
				PayoutInAppPurchase(productId);
				InAppPurchaseProvider.instance.FinishPurchase(productId, transactionId);
				OnPurchaseSuccess onPurchaseSuccess = this.onPurchaseSuccess;
				if (onPurchaseSuccess != null)
				{
					onPurchaseSuccess(productId);
				}
			}
			break;
		case PurchaseResult.Canceled:
		{
			PurchaseCanceled(productId);
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				InAppPurchaseProvider.instance.FinishPurchase(productId, transactionId);
			}
			OnPurchaseCanceled onPurchaseCanceled = this.onPurchaseCanceled;
			if (onPurchaseCanceled != null)
			{
				onPurchaseCanceled(productId, result);
			}
			break;
		}
		case PurchaseResult.ItemUnavailable:
		{
			PurchaseItemUnavailable(productId);
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				InAppPurchaseProvider.instance.FinishPurchase(productId, transactionId);
			}
			OnPurchaseItemNotAvailable onPurchaseItemNotAvailable = this.onPurchaseItemNotAvailable;
			if (onPurchaseItemNotAvailable != null)
			{
				onPurchaseItemNotAvailable(productId, result);
			}
			break;
		}
		default:
		{
			PurchaseFailed(productId);
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				InAppPurchaseProvider.instance.FinishPurchase(productId, transactionId);
			}
			OnPurchaseFailure onPurchaseFailure = this.onPurchaseFailure;
			if (onPurchaseFailure != null)
			{
				onPurchaseFailure(productId, result);
			}
			break;
		}
		}
		if (UIScreenController.IsInstanced)
		{
			UIScreenController.Instance.HideInAppPurchaseOverlay();
		}
	}

	public void ItemRestoredCallback(string productId, string transactionId, string cookie, ValidationResult validationResult)
	{
		PurchasedRestored(productId);
		OnItemRestored onItemRestored = this.onItemRestored;
		if (onItemRestored != null)
		{
			onItemRestored(productId);
		}
		OnPurchaseSuccess onPurchaseSuccess = this.onPurchaseSuccess;
		if (onPurchaseSuccess != null)
		{
			onPurchaseSuccess(productId);
		}
		if (UIScreenController.IsInstanced)
		{
			UIScreenController.Instance.HideInAppPurchaseOverlay();
		}
	}

	public void ItemRefundedCallback(string productid, string transactionid, string cookie, ValidationResult validationResult)
	{
		OnItemRefunded onItemRefunded = this.onItemRefunded;
		if (onItemRefunded != null)
		{
			onItemRefunded(productid);
		}
		if (UIScreenController.IsInstanced)
		{
			UIScreenController.Instance.HideInAppPurchaseOverlay();
		}
	}

	public void RestoreRequestFinishedCallback(RestoreFinishedResult result)
	{
		if (UIScreenController.IsInstanced)
		{
			UIScreenController.Instance.HideInAppPurchaseOverlay();
		}
		switch (result)
		{
		case RestoreFinishedResult.Failed:
			if (_isRestoredManually)
			{
				UIScreenController.Instance.QueueErrorMessageSlidein(Strings.Get(StringID.SLIDEIN_RESTORE_NETWORK_PROBLEM));
			}
			break;
		case RestoreFinishedResult.NothingToRestore:
			if (_isRestoredManually)
			{
				UIScreenController.Instance.QueueErrorMessageSlidein(Strings.Get(StringID.SLIDEIN_RESTORE_NO_PREVIOUS_PURCHASES));
			}
			break;
		}
		OnItemRestoreFinished onItemRestoreFinished = this.onItemRestoreFinished;
		if (onItemRestoreFinished != null)
		{
			onItemRestoreFinished(result);
		}
		_isRestoredManually = false;
	}

	private void PurchasedRefunded(string productId)
	{
	}

	private void PurchasedRestored(string productId)
	{
		if (productId == "com.kiloo.subways.doublecoins" || productId == "com.kiloo.subways.doublecoinsdiscount")
		{
			if (!PlayerInfo.Instance.hasDoubleCoins)
			{
				PlayerInfo.Instance.hasDoubleCoins = true;
				if (UIScreenController.IsInstanced)
				{
					UIScreenController.Instance.QueueSlideIn(new UIScreenController.SlideIn(UIScreenController.SlideInType.Unlock, "Double Coins"));
				}
				PlayerInfo.Instance.inAppRestoredPurchaseCount++;
			}
		}
		else if (InAppData.InappidIsDynamicLimitedCharacterPurchase(productId))
		{
			LimitedCharacterPurchaseSuccesful(productId, true);
			PlayerInfo.Instance.inAppRestoredPurchaseCount++;
		}
		else if (InAppData.InappidIsDynamicLimitedBoardPurchase(productId))
		{
			LimitedBoardPurchaseSuccesful(productId, true);
			PlayerInfo.Instance.inAppRestoredPurchaseCount++;
		}
		PlayerInfo.Instance.SaveIfDirty();
	}

	private void PurchaseItemUnavailable(string productId)
	{
		TriggerFailForPurchaseHandler();
		NativePurchaseFinished(false);
		if (UIScreenController.IsInstanced)
		{
			_popupData = new InAppManagerPopupData
			{
				popupTitle = Strings.Get(StringID.PURCHASE_FAILED_TITLE),
				popupDescription = Strings.Get(StringID.PURCHASE_FAILED_BODY),
				canMakePurchase = false,
				isCoinsPack = false,
				isErrorMessage = true
			};
			UIScreenController.Instance.PushPopup("NotEnoughCurencyPopup");
		}
	}

	private void PurchaseCanceled(string productId)
	{
		TriggerFailForPurchaseHandler();
		NativePurchaseFinished(false);
	}

	private void PurchaseFailed(string productId)
	{
		TriggerFailForPurchaseHandler();
		if (UIScreenController.IsInstanced)
		{
			_popupData = new InAppManagerPopupData
			{
				popupTitle = Strings.Get(StringID.PURCHASE_FAILED_TITLE),
				popupDescription = Strings.Get(StringID.PURCHASE_FAILED_BODY),
				canMakePurchase = false,
				isCoinsPack = false,
				isErrorMessage = true
			};
			UIScreenController.Instance.PushPopup("NotEnoughCurencyPopup");
		}
	}

	private void PayoutInAppPurchase(string productId)
	{
		bool flag = false;
		string[] currencyTiers = InAppData.GetCurrencyTiers();
		string[] currencyDiscountTiers = InAppData.GetCurrencyDiscountTiers();
		if (!flag)
		{
			string[] array = currencyTiers;
			foreach (string text in array)
			{
				if (text == productId)
				{
					if (InAppData.inAppData[productId].InAppItemType == InAppItemModel.ItemType.Coins)
					{
						PlayerInfo.Instance.amountOfCoins += InAppData.inAppData[productId].PayoutAmount;
					}
					else
					{
						PlayerInfo.Instance.amountOfKeys += InAppData.inAppData[productId].PayoutAmount;
					}
					PlayerInfo.Instance.inAppConsumablePurchaseCount++;
					PlayPurchaseSucessSound();
					flag = true;
					break;
				}
			}
		}
		if (!flag)
		{
			string[] array2 = currencyDiscountTiers;
			foreach (string text2 in array2)
			{
				if (!(text2 == productId))
				{
					continue;
				}
				string inAppOnlineSettingsId = InAppData.GetInAppOnlineSettingsId(InAppData.inAppData[text2].NormalOfferId);
				if (string.IsNullOrEmpty(inAppOnlineSettingsId))
				{
					LogError("Could not find a discout id for the pricepont: " + text2);
					inAppOnlineSettingsId = string.Empty;
					return;
				}
				if (InAppData.inAppData[productId].InAppItemType == InAppItemModel.ItemType.Coins)
				{
					PlayerInfo.Instance.amountOfCoins += InAppData.inAppData[productId].PayoutAmount + Mathf.Clamp(OnlineSettings.instance.GetValue(inAppOnlineSettingsId, 0), 0, int.MaxValue);
				}
				else
				{
					PlayerInfo.Instance.amountOfKeys += InAppData.inAppData[productId].PayoutAmount + Mathf.Clamp(OnlineSettings.instance.GetValue(inAppOnlineSettingsId, 0), 0, int.MaxValue);
				}
				PlayerInfo.Instance.inAppConsumablePurchaseCount++;
				PlayPurchaseSucessSound();
				flag = true;
				break;
			}
		}
		if (!flag && (productId == "com.kiloo.subways.doublecoins" || productId == "com.kiloo.subways.doublecoinsdiscount"))
		{
			if (!PlayerInfo.Instance.hasDoubleCoins)
			{
				PlayerInfo.Instance.hasDoubleCoins = true;
				if (UIScreenController.IsInstanced)
				{
					UIScreenController.Instance.QueueSlideIn(new UIScreenController.SlideIn(UIScreenController.SlideInType.Unlock, "Double Coins"));
				}
			}
			PlayerInfo.Instance.inAppNonconsumablePurchaseCount++;
			flag = true;
		}
		if (!flag)
		{
			if (InAppData.InappidIsDynamicLimitedCharacterPurchase(productId))
			{
				LimitedCharacterPurchaseSuccesful(productId, false);
				PlayerInfo.Instance.inAppNonconsumablePurchaseCount++;
				flag = true;
			}
			else if (InAppData.InappidIsDynamicLimitedBoardPurchase(productId))
			{
				LimitedBoardPurchaseSuccesful(productId, false);
				PlayerInfo.Instance.inAppNonconsumablePurchaseCount++;
				flag = true;
			}
		}
		if (!flag)
		{
			string[] inAppBundleTiers = InAppData.getInAppBundleTiers();
			foreach (string text3 in inAppBundleTiers)
			{
				if (text3 == productId)
				{
					DeliverBundle(productId);
					PlayerInfo.Instance.UpdateLastPurchasedBundle();
					PlayPurchaseSucessSound();
					PlayerInfo.Instance.inAppConsumablePurchaseCount++;
					flag = true;
					break;
				}
			}
		}
		PlayerInfo.Instance.SaveIfDirty();
		NativePurchaseFinished(true);
		LogFlurry(productId);
		_itemRequested = string.Empty;
	}

	private void PlayPurchaseSucessSound()
	{
		if (purchaseSuccessSound != null)
		{
			NGUITools.PlaySound(purchaseSuccessSound);
		}
	}

	private void LimitedCharacterPurchaseSuccesful(string itemId, bool unlockedByRestore)
	{
		Characters.CharacterType charReturned;
		if (InAppData.TryConvertDynamicPurchaseIdToCharacterType(itemId, out charReturned))
		{
			PlayerInfo playerInfo = PlayerInfo.Instance;
			if (!playerInfo.IsCollectionComplete(charReturned))
			{
				Characters.Model model = Characters.characterData[charReturned];
				int price = model.Price;
				playerInfo.CollectToken(charReturned, price);
				if (_purchaseHandler != null)
				{
					_purchaseHandler.OnPurchaseCharacterWithInappSucceded(charReturned, itemId);
				}
				if (unlockedByRestore && UIScreenController.IsInstanced)
				{
					CharacterScreen.forceRefreshOnCharacterScreen = true;
					UIScreenController.Instance.QueueSlideIn(new UIScreenController.SlideIn(UIScreenController.SlideInType.Unlock, Strings.Get(model.name)));
				}
			}
		}
		if (_purchaseHandler != null)
		{
			_purchaseHandler.ResetOnFailedToPurchaseEventItemWithInapp();
		}
		_purchaseHandler = null;
	}

	private void LimitedBoardPurchaseSuccesful(string itemId, bool unlockedByRestore)
	{
		Hoverboards.BoardType boardReturned;
		if (InAppData.TryConvertDynamicBoardIdToBoardType(itemId, out boardReturned))
		{
			PlayerInfo playerInfo = PlayerInfo.Instance;
			if (!playerInfo.isHoverboardUnlocked(boardReturned))
			{
				playerInfo.UnlockHoverboard(boardReturned);
				if (_purchaseHandler != null)
				{
					_purchaseHandler.OnPurchaseBoardWithInappSucceded(itemId);
				}
				if (unlockedByRestore && UIScreenController.IsInstanced)
				{
					BoardScreenManager.Instance.forceRefreshOnBoardScreen = true;
					UIScreenController.Instance.QueueSlideIn(new UIScreenController.SlideIn(UIScreenController.SlideInType.Unlock, Strings.Get(Hoverboards.boardData[boardReturned].name)));
				}
			}
		}
		if (_purchaseHandler != null)
		{
			_purchaseHandler.ResetOnFailedToPurchaseEventItemWithInapp();
		}
		_purchaseHandler = null;
	}

	private void TriggerFailForPurchaseHandler()
	{
		if (_purchaseHandler != null)
		{
			_purchaseHandler.TriggerOnFailedToPurchaseEventItemWithInapp();
			_purchaseHandler = null;
		}
	}

	public void NativePurchaseFinished(bool success)
	{
		if (UIScreenController.IsInstanced)
		{
			UIScreenController.Instance.HideInAppPurchaseOverlay();
		}
		if (success)
		{
			SaveMeManager.SendReviveIfPurchaseSucceeded();
		}
		else
		{
			SaveMeManager.SkipReviveIfPurchaseFailed();
		}
	}

	public void NativePurchaseInappPack(string message)
	{
		if (message == "0")
		{
			_inAppPurchaseKey = string.Empty;
			SaveMeManager.SkipReviveIfPurchaseFailed();
		}
		else if (_productRequestSucceeded)
		{
			StartPurchase(_inAppPurchaseKey);
		}
		else
		{
			_inAppPurchaseKey = string.Empty;
		}
	}

	public void NativePurchaseFail(string message)
	{
		_inAppPurchaseKey = string.Empty;
		if (message == "0")
		{
			NativePurchaseFinished(false);
		}
	}

	public void BuyExpiredLimitedCharacter(string inappId, string characterName, PurchaseHandler sender)
	{
		_purchaseHandler = sender;
		if (!string.IsNullOrEmpty(inappId))
		{
			StartPurchase(inappId);
		}
	}

	public void BuyExpiredLimitedBoard(string inappId, string boardName, PurchaseHandler sender)
	{
		_purchaseHandler = sender;
		if (!string.IsNullOrEmpty(inappId))
		{
			StartPurchase(inappId);
		}
	}

	private void DeliverBundle(string bundleId)
	{
		InAppItemModel inAppItemModel = InAppData.inAppData[bundleId];
		int value = 0;
		inAppItemModel.BundleContents.TryGetValue(InAppItemModel.ItemType.Coins, out value);
		if (value > 0)
		{
			PlayerInfo.Instance.amountOfCoins += value;
		}
		int value2 = 0;
		inAppItemModel.BundleContents.TryGetValue(InAppItemModel.ItemType.Keys, out value2);
		if (value2 > 0)
		{
			PlayerInfo.Instance.amountOfKeys += value2;
		}
		int value3 = 0;
		inAppItemModel.BundleContents.TryGetValue(InAppItemModel.ItemType.Hoverboards, out value3);
		if (value3 > 0)
		{
			PlayerInfo.Instance.IncreaseUpgradeAmount(PowerupType.hoverboard, value3);
		}
		int value4 = 0;
		inAppItemModel.BundleContents.TryGetValue(InAppItemModel.ItemType.Scoreboosters, out value4);
		if (value4 > 0)
		{
			PlayerInfo.Instance.IncreaseUpgradeAmount(PowerupType.scorebooster, value4);
		}
		int value5 = 0;
		inAppItemModel.BundleContents.TryGetValue(InAppItemModel.ItemType.Headstarts, out value5);
		if (value5 > 0)
		{
			PlayerInfo.Instance.IncreaseUpgradeAmount(PowerupType.headstart2000, value5);
		}
	}

	public void SuggestInAppPopup(int cost, string senderName, InAppItemModel.ItemType type)
	{
		_itemRequested = senderName;
		int num = 0;
		num = ((type != InAppItemModel.ItemType.Coins) ? (cost - PlayerInfo.Instance.amountOfKeys) : (cost - PlayerInfo.Instance.amountOfCoins));
		string text = string.Empty;
		int num2 = 0;
		if (_productRequestSucceeded)
		{
			foreach (KeyValuePair<string, InAppItemModel> activeInAppOffer in GetActiveInAppOffers())
			{
				if (!activeInAppOffer.Value.IsValidated || activeInAppOffer.Value.InAppItemType != type || activeInAppOffer.Value.PayoutAmount < num)
				{
					continue;
				}
				if (!string.IsNullOrEmpty(text))
				{
					if (InAppData.inAppData[text].PayoutAmount > activeInAppOffer.Value.PayoutAmount)
					{
						text = activeInAppOffer.Key;
						num2 = activeInAppOffer.Value.PayoutAmount;
					}
				}
				else
				{
					text = activeInAppOffer.Key;
					num2 = activeInAppOffer.Value.PayoutAmount;
				}
			}
		}
		_inAppPurchaseKey = text;
		string popupTitle = string.Empty;
		bool isCoinsPack = false;
		switch (type)
		{
		case InAppItemModel.ItemType.Coins:
			isCoinsPack = true;
			popupTitle = Strings.Get(StringID.NOT_ENOUGH_COINS);
			break;
		case InAppItemModel.ItemType.Keys:
			popupTitle = Strings.Get(StringID.NOT_ENOUGH_KEYS);
			break;
		default:
			LogError("No handling implemented for item type: " + type);
			break;
		}
		if (!string.IsNullOrEmpty(_inAppPurchaseKey))
		{
			string popupDescription = string.Empty;
			switch (type)
			{
			case InAppItemModel.ItemType.Coins:
			{
				string empty2 = string.Empty;
				empty2 = ((num >= 2) ? Strings.Get(StringID.PURCHASE_COINS_MULTIPLE) : Strings.Get(StringID.PURCHASE_COINS_ONE));
				popupDescription = string.Format(empty2, num, num2, InAppData.inAppData[text].LocalizedPrice);
				break;
			}
			case InAppItemModel.ItemType.Keys:
			{
				string empty = string.Empty;
				empty = ((num >= 2) ? Strings.Get(StringID.PURCHASE_KEYS_MULTIPLE) : Strings.Get(StringID.PURCHASE_KEYS_ONE));
				popupDescription = string.Format(empty, num, num2, InAppData.inAppData[text].LocalizedPrice);
				break;
			}
			default:
				LogError("No handling implemented for item type: " + type);
				break;
			}
			if (UIScreenController.IsInstanced)
			{
				_popupData = new InAppManagerPopupData
				{
					popupTitle = popupTitle,
					popupDescription = popupDescription,
					canMakePurchase = true,
					isCoinsPack = isCoinsPack,
					isErrorMessage = false
				};
				UIScreenController.Instance.PushPopup("NotEnoughCurencyPopup");
			}
			return;
		}
		switch (type)
		{
		case InAppItemModel.ItemType.Coins:
		{
			string empty4 = string.Empty;
			empty4 = ((num >= 2) ? Strings.Get(StringID.PURCHASE_COINS_MULTIPLE_NO_INTERNET) : Strings.Get(StringID.PURCHASE_COINS_ONE_NO_INTERNET));
			string popupDescription3 = string.Format(empty4, num);
			if (UIScreenController.IsInstanced)
			{
				_popupData = new InAppManagerPopupData
				{
					popupTitle = popupTitle,
					popupDescription = popupDescription3,
					canMakePurchase = false,
					isCoinsPack = isCoinsPack,
					isErrorMessage = false
				};
				UIScreenController.Instance.PushPopup("NotEnoughCurencyPopup");
			}
			break;
		}
		case InAppItemModel.ItemType.Keys:
		{
			string empty3 = string.Empty;
			empty3 = ((num >= 2) ? Strings.Get(StringID.PURCHASE_KEYS_MULTIPLE_NO_INTERNET) : Strings.Get(StringID.PURCHASE_KEYS_ONE_NO_INTERNET));
			string popupDescription2 = string.Format(empty3, num);
			if (UIScreenController.IsInstanced)
			{
				_popupData = new InAppManagerPopupData
				{
					popupTitle = popupTitle,
					popupDescription = popupDescription2,
					canMakePurchase = false,
					isCoinsPack = isCoinsPack,
					isErrorMessage = false
				};
				UIScreenController.Instance.PushPopup("NotEnoughCurencyPopup");
			}
			break;
		}
		default:
			LogError("No handling implemented for item type: " + type);
			break;
		}
	}

	public static Dictionary<string, InAppItemModel> GetActiveInAppOffers()
	{
		Dictionary<string, InAppItemModel> dictionary = new Dictionary<string, InAppItemModel>();
		string[] currencyTiers = InAppData.GetCurrencyTiers();
		string[] array = currencyTiers;
		foreach (string text in array)
		{
			string text2 = InAppData.GetIgnoredInAppOnlineSettingsId(text);
			if (string.IsNullOrEmpty(text2))
			{
				LogError("could not find an ignored inApp id for the pricepont: " + text);
				text2 = string.Empty;
			}
			if (OnlineSettings.instance.HasValue(text2))
			{
				continue;
			}
			string text3 = InAppData.GetInAppOnlineSettingsId(text);
			if (string.IsNullOrEmpty(text3))
			{
				LogError("could not find a discout id for the pricepont: " + text);
				text3 = string.Empty;
			}
			InAppItemModel value = InAppItemModel.Clone(InAppData.inAppData[text]);
			if (OnlineSettings.instance.HasValue(text3) && IsAnyDiscountAvailable)
			{
				string discountIdForNormalOffer = InAppData.GetDiscountIdForNormalOffer(text);
				InAppItemModel inAppItemModel = InAppItemModel.Clone(InAppData.inAppData[discountIdForNormalOffer]);
				int value2 = OnlineSettings.instance.GetValue(text3, 0);
				if (value2 < 0)
				{
					dictionary.Add(discountIdForNormalOffer, inAppItemModel);
				}
				else if (value2 > 0)
				{
					if (inAppItemModel.InAppItemType == InAppItemModel.ItemType.Keys || inAppItemModel.InAppItemType == InAppItemModel.ItemType.Coins)
					{
						inAppItemModel.ConvertToPromotion(value2);
						dictionary.Add(discountIdForNormalOffer, inAppItemModel);
					}
					else
					{
						LogError(string.Concat("Invalid item type: ", inAppItemModel.InAppItemType, ". No sales are implemented for this item type."));
						dictionary.Add(text, value);
					}
				}
				else
				{
					LogError("Invalid discount value for item: " + text);
					dictionary.Add(text, value);
				}
			}
			else
			{
				dictionary.Add(text, value);
			}
		}
		return dictionary;
	}

	private void LogFlurry(string productId)
	{
		Flurry.LogEventWithAParameter("InApp purchase completed", "Id", productId);
		switch (productId)
		{
		case "com.kiloo.subways.coinstier1":
		case "com.kiloo.subways.coinstier1discount":
			if (string.IsNullOrEmpty(_itemRequested))
			{
				Flurry.LogEventWithAParameter("InApp Coin Pack 1 purchased", "Mission Set", PlayerInfo.Instance.currentMissionSet.ToString());
			}
			else
			{
				Flurry.LogEventWithSeveralParameters("InApp Coin Pack 1 purchased", "Mission Set;Item tiggered iap", PlayerInfo.Instance.currentMissionSet + ";" + _itemRequested);
			}
			break;
		case "com.kiloo.subways.coinstier2":
		case "com.kiloo.subways.coinstier2_discount":
			if (string.IsNullOrEmpty(_itemRequested))
			{
				Flurry.LogEventWithAParameter("InApp Coin Pack 2 purchased", "Mission Set", PlayerInfo.Instance.currentMissionSet.ToString());
			}
			else
			{
				Flurry.LogEventWithSeveralParameters("InApp Coin Pack 2 purchased", "Mission Set;Item tiggered iap", PlayerInfo.Instance.currentMissionSet + ";" + _itemRequested);
			}
			break;
		case "com.kiloo.subways.coinstier3":
		case "com.kiloo.subways.coinstier3discount":
			if (string.IsNullOrEmpty(_itemRequested))
			{
				Flurry.LogEventWithAParameter("InApp Coin Pack 3 purchased", "Mission Set", PlayerInfo.Instance.currentMissionSet.ToString());
			}
			else
			{
				Flurry.LogEventWithSeveralParameters("InApp Coin Pack 3 purchased", "Mission Set;Item tiggered iap", PlayerInfo.Instance.currentMissionSet + ";" + _itemRequested);
			}
			break;
		case "com.kiloo.subways.coinstier4":
		case "com.kiloo.subways.coinstier4discount":
			if (string.IsNullOrEmpty(_itemRequested))
			{
				Flurry.LogEventWithAParameter("InApp Coin Pack 4 purchased", "Mission Set", PlayerInfo.Instance.currentMissionSet.ToString());
			}
			else
			{
				Flurry.LogEventWithSeveralParameters("InApp Coin Pack 4 purchased", "Mission Set;Item tiggered iap", PlayerInfo.Instance.currentMissionSet + ";" + _itemRequested);
			}
			break;
		case "com.kiloo.subways.coinstier5":
		case "com.kiloo.subways.coinstier5discount":
			if (string.IsNullOrEmpty(_itemRequested))
			{
				Flurry.LogEventWithAParameter("InApp Coin Pack 5 purchased", "Mission Set", PlayerInfo.Instance.currentMissionSet.ToString());
			}
			else
			{
				Flurry.LogEventWithSeveralParameters("InApp Coin Pack 5 purchased", "Mission Set;Item tiggered iap", PlayerInfo.Instance.currentMissionSet + ";" + _itemRequested);
			}
			break;
		case "com.kiloo.subways.keystier1":
		case "com.kiloo.subways.keystier1discount":
			if (string.IsNullOrEmpty(_itemRequested))
			{
				Flurry.LogEventWithAParameter("InApp Key Pack 1 purchased", "Mission Set", PlayerInfo.Instance.currentMissionSet.ToString());
			}
			else
			{
				Flurry.LogEventWithSeveralParameters("InApp Key Pack 1 purchased", "Mission Set;Item tiggered iap", PlayerInfo.Instance.currentMissionSet + ";" + _itemRequested);
			}
			break;
		case "com.kiloo.subways.keystier2":
		case "com.kiloo.subways.keystier2discount":
			if (string.IsNullOrEmpty(_itemRequested))
			{
				Flurry.LogEventWithAParameter("InApp Key Pack 2 purchased", "Mission Set", PlayerInfo.Instance.currentMissionSet.ToString());
			}
			else
			{
				Flurry.LogEventWithSeveralParameters("InApp Key Pack 2 purchased", "Mission Set;Item tiggered iap", PlayerInfo.Instance.currentMissionSet + ";" + _itemRequested);
			}
			break;
		case "com.kiloo.subways.keystier3":
		case "com.kiloo.subways.keystier3discount":
			if (string.IsNullOrEmpty(_itemRequested))
			{
				Flurry.LogEventWithAParameter("InApp Key Pack 3 purchased", "Mission Set", PlayerInfo.Instance.currentMissionSet.ToString());
			}
			else
			{
				Flurry.LogEventWithSeveralParameters("InApp Key Pack 3 purchased", "Mission Set;Item tiggered iap", PlayerInfo.Instance.currentMissionSet + ";" + _itemRequested);
			}
			break;
		case "com.kiloo.subways.keystier4":
		case "com.kiloo.subways.keystier4discount":
			if (string.IsNullOrEmpty(_itemRequested))
			{
				Flurry.LogEventWithAParameter("InApp Key Pack 4 purchased", "Mission Set", PlayerInfo.Instance.currentMissionSet.ToString());
			}
			else
			{
				Flurry.LogEventWithSeveralParameters("InApp Key Pack 4 purchased", "Mission Set;Item tiggered iap", PlayerInfo.Instance.currentMissionSet + ";" + _itemRequested);
			}
			break;
		case "com.kiloo.subways.doublecoins":
		case "com.kiloo.subways.doublecoinsdiscount":
			if (UIScreenController.IsInstanced)
			{
				if (string.IsNullOrEmpty(UIScreenController.Instance.GetCurrentPopupName()))
				{
					Flurry.LogEventWithAParameter("Double Coin purchased", "Mission Set", PlayerInfo.Instance.currentMissionSet.ToString());
				}
				else if (UIScreenController.Instance.GetCurrentPopupName() == "TutorialDoubleCoinsPopup")
				{
					Flurry.LogEventWithAParameter("Double Coin purchased GameOver", "Mission Set", PlayerInfo.Instance.currentMissionSet.ToString());
				}
				else
				{
					Flurry.LogEventWithAParameter("Double Coin purchased", "Mission Set", PlayerInfo.Instance.currentMissionSet.ToString());
				}
			}
			else
			{
				Flurry.LogEventWithAParameter("Double Coin purchased", "Mission Set", PlayerInfo.Instance.currentMissionSet.ToString());
			}
			break;
		case "com.kiloo.subways.bundletier1":
			if (string.IsNullOrEmpty(_itemRequested))
			{
				Flurry.LogEventWithAParameter("InApp Bundle 1 purchased", "Mission Set", PlayerInfo.Instance.currentMissionSet.ToString());
			}
			else
			{
				Flurry.LogEventWithSeveralParameters("InApp Bundle 1 purchased", "Mission Set;Item tiggered iap", PlayerInfo.Instance.currentMissionSet + ";" + _itemRequested);
			}
			break;
		case "com.kiloo.subways.bundletier2":
			if (string.IsNullOrEmpty(_itemRequested))
			{
				Flurry.LogEventWithAParameter("InApp Bundle 2 purchased", "Mission Set", PlayerInfo.Instance.currentMissionSet.ToString());
			}
			else
			{
				Flurry.LogEventWithSeveralParameters("InApp Bundle 2 purchased", "Mission Set;Item tiggered iap", PlayerInfo.Instance.currentMissionSet + ";" + _itemRequested);
			}
			break;
		case "com.kiloo.subways.bundletier3":
			if (string.IsNullOrEmpty(_itemRequested))
			{
				Flurry.LogEventWithAParameter("InApp Bundle 3 purchased", "Mission Set", PlayerInfo.Instance.currentMissionSet.ToString());
			}
			else
			{
				Flurry.LogEventWithSeveralParameters("InApp Bundle 3 purchased", "Mission Set;Item tiggered iap", PlayerInfo.Instance.currentMissionSet + ";" + _itemRequested);
			}
			break;
		case "com.kiloo.subways.bundletier4":
			if (string.IsNullOrEmpty(_itemRequested))
			{
				Flurry.LogEventWithAParameter("InApp Bundle 4 purchased", "Mission Set", PlayerInfo.Instance.currentMissionSet.ToString());
			}
			else
			{
				Flurry.LogEventWithSeveralParameters("InApp Bundle 4 purchased", "Mission Set;Item tiggered iap", PlayerInfo.Instance.currentMissionSet + ";" + _itemRequested);
			}
			break;
		case "com.kiloo.subways.bundletier5":
			if (string.IsNullOrEmpty(_itemRequested))
			{
				Flurry.LogEventWithAParameter("InApp Bundle 5 purchased", "Mission Set", PlayerInfo.Instance.currentMissionSet.ToString());
			}
			else
			{
				Flurry.LogEventWithSeveralParameters("InApp Bundle 5 purchased", "Mission Set;Item tiggered iap", PlayerInfo.Instance.currentMissionSet + ";" + _itemRequested);
			}
			break;
		}
	}

	private string GetContactingStoreOverlayText()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			return Strings.Get(StringID.INAPP_MANAGER_OVERLAY_TEXT_GOOGLEPLAY);
		}
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return Strings.Get(StringID.INAPP_MANAGER_OVERLAY_TEXT_APPSTORE);
		}
		if (Application.platform == RuntimePlatform.WP8Player)
		{
			return Strings.Get(StringID.INAPP_MANAGER_OVERLAY_TEXT_WINDOWSSTORE);
		}
		return string.Empty;
	}

	public static void LogError(string msg, UnityEngine.Object context = null)
	{
		Debug.LogError(msg, context);
	}

	public static void LogWarning(string msg, UnityEngine.Object context = null)
	{
		Debug.LogWarning(msg, context);
	}

	[Conditional("ENABLE_INAPPMANAGER_LOGS")]
	public static void Log(string msg, UnityEngine.Object context = null)
	{
		Debug.Log(msg, context);
	}

	public InAppManagerPopupData GetPopupData()
	{
		return _popupData;
	}

	public void DeletePopupData()
	{
		_popupData = null;
	}
}
