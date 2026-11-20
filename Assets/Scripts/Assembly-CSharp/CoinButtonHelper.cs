using UnityEngine;

public class CoinButtonHelper : DiscountButton
{
	private bool inited;

	private Transform onDisplayParent;

	private CoinScreen coinScreen;

	private InAppItemModel itemData;

	private bool scheduleSetup = true;

	private void OnClick()
	{
		InAppManager.instance.StartPurchase(itemData.ItemId);
	}

	public void Init(InAppItemModel inAppItem)
	{
		inited = true;
		itemData = inAppItem;
		foreach (Transform item in base.transform.parent)
		{
			if (item.name == "000")
			{
				onDisplayParent = item;
			}
		}
		coinScreen = NGUITools.FindInParents<CoinScreen>(base.transform.parent.gameObject);
		Setup();
		icon.MakePixelPerfect();
	}

	private void ForceEndDiscount()
	{
		Setup(true);
		coinScreen.counterForElementsOnDisplay = 0;
	}

	private void Setup(bool forceEndDiscount = false)
	{
		if (itemData.ItemOfferType == InAppItemModel.OfferType.Sale && !forceEndDiscount)
		{
			if (itemData.InAppItemType == InAppItemModel.ItemType.Coins)
			{
				ShowOnSale(itemData.ItemId, string.Format(Strings.Get(StringID.COIN_BUTTON_HELPER_NUMBER_OF_COINS), itemData.PayoutAmount));
			}
			else if (itemData.InAppItemType == InAppItemModel.ItemType.Keys)
			{
				ShowOnSale(itemData.ItemId, string.Format(Strings.Get(StringID.COIN_BUTTON_HELPER_NUMBER_OF_KEYS), itemData.PayoutAmount));
			}
			else
			{
				Debug.LogError("Item type not supported in CoinButtonHelper: " + itemData.InAppItemType);
			}
			if (base.transform.parent != onDisplayParent)
			{
				coinScreen.counterForElementsOnDisplay++;
				base.transform.parent = onDisplayParent;
				base.transform.localPosition = new Vector3(0f, -75 * coinScreen.counterForElementsOnDisplay, 0f);
			}
		}
		else if (itemData.ItemOfferType == InAppItemModel.OfferType.Promotion && !forceEndDiscount)
		{
			if (itemData.InAppItemType == InAppItemModel.ItemType.Coins)
			{
				ShowOnPromotion(itemData.ItemId, string.Format(Strings.Get(StringID.COIN_BUTTON_HELPER_NUMBER_OF_COINS), InAppData.inAppData[itemData.NormalOfferId].PayoutAmount), string.Format(Strings.Get(StringID.COIN_BUTTON_HELPER_NUMBER_OF_COINS_SALE), itemData.PayoutAmount));
			}
			else if (itemData.InAppItemType == InAppItemModel.ItemType.Keys)
			{
				ShowOnPromotion(itemData.ItemId, string.Format(Strings.Get(StringID.COIN_BUTTON_HELPER_NUMBER_OF_KEYS), InAppData.inAppData[itemData.NormalOfferId].PayoutAmount), string.Format(Strings.Get(StringID.COIN_BUTTON_HELPER_NUMBER_OF_KEYS_SALE), itemData.PayoutAmount));
			}
			else
			{
				Debug.LogError("Item type not supported in CoinButtonHelper: " + itemData.InAppItemType);
			}
			if (base.transform.parent != onDisplayParent)
			{
				coinScreen.counterForElementsOnDisplay++;
				base.transform.parent = onDisplayParent;
				base.transform.localPosition = new Vector3(0f, -75 * coinScreen.counterForElementsOnDisplay, 0f);
			}
		}
		else
		{
			if (itemData.InAppItemType == InAppItemModel.ItemType.Coins)
			{
				ShowNoDiscount(itemData.ItemId, string.Format(Strings.Get(StringID.COIN_BUTTON_HELPER_NUMBER_OF_COINS), itemData.PayoutAmount));
			}
			else if (itemData.InAppItemType == InAppItemModel.ItemType.Keys)
			{
				ShowNoDiscount(itemData.ItemId, string.Format(Strings.Get(StringID.COIN_BUTTON_HELPER_NUMBER_OF_KEYS), itemData.PayoutAmount));
			}
			else
			{
				Debug.LogError("Item type not supported in CoinButtonHelper: " + itemData.InAppItemType);
			}
			if (base.transform.parent == onDisplayParent)
			{
				base.transform.parent = onDisplayParent.parent;
			}
		}
	}

	private void OnEnable()
	{
		if (inited)
		{
			scheduleSetup = true;
		}
	}

	private void Update()
	{
		if (scheduleSetup)
		{
			Setup();
			scheduleSetup = false;
		}
	}
}
