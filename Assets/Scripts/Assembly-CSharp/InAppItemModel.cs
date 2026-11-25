using System.Collections.Generic;
using UnityEngine;

public class InAppItemModel
{
	public enum ItemType
	{
		Undefined = 0,
		Coins = 1,
		Keys = 2,
		Bundle = 3,
		DoubleCoins = 4,
		Hoverboards = 5,
		Headstarts = 6,
		Scoreboosters = 7
	}

	public enum OfferType
	{
		Normal = 0,
		Sale = 1,
		Promotion = 2
	}

	private string _localizedPrize = "$0.99";

	public string DisplayName { get; private set; }

	public string DisplayText { get; private set; }

	public string ItemId { get; private set; }

	public int PayoutAmount { get; private set; }

	public bool IsConsumable { get; private set; }

	public string StoreItemIdentifier { get; private set; }

	public string Icon { get; private set; }

	public string NormalOfferId { get; private set; }

	public Dictionary<ItemType, int> BundleContents { get; private set; }

	public ItemType InAppItemType { get; private set; }

	public OfferType ItemOfferType { get; private set; }

	public int CategoryOrderIndex { get; set; }

	public string StoreItemToken { get; set; }

	public string LocalizedPrice
	{
		get
		{
			if (IsValidated)
			{
				return _localizedPrize;
			}
			return Strings.Get(StringID.BUY_INAPP_NOPRICE_DEFAULT);
		}
		set
		{
			_localizedPrize = value;
		}
	}

	public bool IsValidated { get; set; } = true;

	public bool IsDynamic { get; set; }

	private InAppItemModel()
	{
	}

	public InAppItemModel(string storeItemId, string storeItemToken, string itemId, string displayName, string displayText, string icon, int payoutAmount, bool isConsumable, ItemType itemType, OfferType offerType, Dictionary<ItemType, int> bundleContents = null, string normalOfferId = "")
	{
		DisplayName = displayName;
		DisplayText = displayText;
		ItemId = itemId;
		PayoutAmount = payoutAmount;
		IsConsumable = isConsumable;
		StoreItemIdentifier = storeItemId;
		Icon = icon;
		NormalOfferId = normalOfferId;
		InAppItemType = itemType;
		ItemOfferType = offerType;
		StoreItemToken = storeItemToken;
		if (bundleContents != null)
		{
			BundleContents = bundleContents;
		}
	}

	public void ConvertToPromotion(int payoutIncrement)
	{
		PayoutAmount += payoutIncrement;
		ItemOfferType = OfferType.Promotion;
	}

	public static InAppItemModel CreateEmpty()
	{
		return new InAppItemModel();
	}

	public static InAppItemModel Clone(InAppItemModel objectToClone)
	{
		InAppItemModel inAppItemModel = new InAppItemModel();
		inAppItemModel.DisplayName = objectToClone.DisplayName;
		inAppItemModel.DisplayText = objectToClone.DisplayText;
		inAppItemModel.ItemId = objectToClone.ItemId;
		inAppItemModel.PayoutAmount = objectToClone.PayoutAmount;
		inAppItemModel.IsConsumable = objectToClone.IsConsumable;
		inAppItemModel.StoreItemIdentifier = objectToClone.StoreItemIdentifier;
		inAppItemModel.Icon = objectToClone.Icon;
		inAppItemModel.NormalOfferId = objectToClone.NormalOfferId;
		inAppItemModel.BundleContents = objectToClone.BundleContents;
		inAppItemModel.InAppItemType = objectToClone.InAppItemType;
		inAppItemModel.ItemOfferType = objectToClone.ItemOfferType;
		inAppItemModel.CategoryOrderIndex = objectToClone.CategoryOrderIndex;
		inAppItemModel.StoreItemToken = objectToClone.StoreItemToken;
		inAppItemModel.LocalizedPrice = objectToClone.LocalizedPrice;
		inAppItemModel.IsValidated = objectToClone.IsValidated;
		inAppItemModel.IsDynamic = objectToClone.IsDynamic;
		return inAppItemModel;
	}

	public override string ToString()
	{
		return string.Format("currencyItemModel: storeItemId->{0}, storeItemToken->{1}, itemId->{2}", StoreItemIdentifier, StoreItemToken, ItemId);
	}
}
