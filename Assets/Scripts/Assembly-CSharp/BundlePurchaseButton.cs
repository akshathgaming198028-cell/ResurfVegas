using InAppPurchase;
using UnityEngine;

public class BundlePurchaseButton : MonoBehaviour
{
	[SerializeField]
	private GameObject activeButtonContents;

	[SerializeField]
	private GameObject inactiveButtonContents;

	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private UILabel priceLabel;

	[SerializeField]
	private UISprite bundle1Icon;

	[SerializeField]
	private UISprite bundle2Icon;

	[SerializeField]
	private UISprite bundle3Icon;

	[SerializeField]
	private UISprite bundle4Icon;

	[SerializeField]
	private UISprite bundle5Icon;

	[SerializeField]
	private BundleContentSlot coinsSlot;

	[SerializeField]
	private BundleContentSlot hoverboardsSlot;

	[SerializeField]
	private BundleContentSlot keysSlot;

	[SerializeField]
	private BundleContentSlot headstartSlot;

	[SerializeField]
	private BundleContentSlot scoreboosterSlot;

	private InAppData.BundleTier _lastPurchasedBundle;

	private void OnEnable()
	{
		InitializeButton();
		UpdateButton();
	}

	private void OnClick()
	{
		InAppManager.instance.StartPurchase(InAppData.GetBundleId(GetBundleToSell()));
		UpdateButton();
	}

	private void InitializeButton()
	{
		InAppManager.instance.onProductRequestDone += OnProductRequestDone;
		InAppManager.instance.onPurchaseSuccess += OnPurchaseSuccess;
		InAppManager.instance.onPurchaseFailure += OnPurchaseFailure;
	}

	private void OnDisable()
	{
		InAppManager instance = InAppManager.instance;
		if (instance != null)
		{
			instance.onProductRequestDone -= OnProductRequestDone;
			instance.onPurchaseSuccess -= OnPurchaseSuccess;
			instance.onPurchaseFailure -= OnPurchaseFailure;
		}
	}

	private void UpdatePrice()
	{
		_lastPurchasedBundle = PlayerInfo.Instance.lastPurchasedBundle;
		priceLabel.text = InAppData.inAppData[InAppData.GetBundleId(GetBundleToSell())].LocalizedPrice;
	}

	private void OnProductRequestDone()
	{
		UpdatePrice();
	}

	private void OnPurchaseSuccess(string productId)
	{
		UpdateButton();
	}

	private void OnPurchaseFailure(string data, PurchaseResult ressult)
	{
		UpdateButton();
	}

	private InAppData.BundleTier GetBundleToSell()
	{
		switch (_lastPurchasedBundle)
		{
		case InAppData.BundleTier.None:
			return InAppData.BundleTier.BundleTier1;
		case InAppData.BundleTier.BundleTier1:
			return InAppData.BundleTier.BundleTier2;
		case InAppData.BundleTier.BundleTier2:
			return InAppData.BundleTier.BundleTier3;
		case InAppData.BundleTier.BundleTier3:
			return InAppData.BundleTier.BundleTier4;
		case InAppData.BundleTier.BundleTier4:
			return InAppData.BundleTier.BundleTier5;
		case InAppData.BundleTier.BundleTier5:
			Debug.LogError("There are no more bundles to sell");
			return InAppData.BundleTier.None;
		default:
			Debug.LogError("Unidetified bundle tier: " + _lastPurchasedBundle);
			return InAppData.BundleTier.None;
		}
	}

	private void UpdateButton()
	{
		_lastPurchasedBundle = PlayerInfo.Instance.lastPurchasedBundle;
		if (_lastPurchasedBundle != InAppData.BundleTier.BundleTier5)
		{
			ShowActiveContents(true);
			InAppItemModel inAppItemModel = InAppData.inAppData[InAppData.GetBundleId(GetBundleToSell())];
			titleLabel.text = inAppItemModel.DisplayName;
			ShowBundleIcon();
			UpdatePrice();
			int value = 0;
			inAppItemModel.BundleContents.TryGetValue(InAppItemModel.ItemType.Coins, out value);
			if (value > 0)
			{
				coinsSlot.gameObject.SetActive(true);
				coinsSlot.UpdateAmount(GetFormattedAmount(value));
			}
			else
			{
				coinsSlot.gameObject.SetActive(false);
			}
			int value2 = 0;
			inAppItemModel.BundleContents.TryGetValue(InAppItemModel.ItemType.Hoverboards, out value2);
			if (value2 > 0)
			{
				hoverboardsSlot.gameObject.SetActive(true);
				hoverboardsSlot.UpdateAmount(GetFormattedAmount(value2));
			}
			else
			{
				hoverboardsSlot.gameObject.SetActive(false);
			}
			int value3 = 0;
			inAppItemModel.BundleContents.TryGetValue(InAppItemModel.ItemType.Keys, out value3);
			if (value3 > 0)
			{
				keysSlot.gameObject.SetActive(true);
				keysSlot.UpdateAmount(GetFormattedAmount(value3));
			}
			else
			{
				keysSlot.gameObject.SetActive(false);
			}
			int value4 = 0;
			inAppItemModel.BundleContents.TryGetValue(InAppItemModel.ItemType.Headstarts, out value4);
			if (value4 > 0)
			{
				headstartSlot.gameObject.SetActive(true);
				headstartSlot.UpdateAmount(GetFormattedAmount(value4));
			}
			else
			{
				headstartSlot.gameObject.SetActive(false);
			}
			int value5 = 0;
			inAppItemModel.BundleContents.TryGetValue(InAppItemModel.ItemType.Scoreboosters, out value5);
			if (value5 > 0)
			{
				scoreboosterSlot.gameObject.SetActive(true);
				scoreboosterSlot.UpdateAmount(GetFormattedAmount(value5));
			}
			else
			{
				scoreboosterSlot.gameObject.SetActive(false);
			}
		}
		else
		{
			ShowActiveContents(false);
		}
	}

	private void ShowActiveContents(bool active)
	{
		activeButtonContents.SetActive(active);
		inactiveButtonContents.SetActive(!active);
		GetComponent<Collider>().enabled = active;
	}

	private void ShowBundleIcon()
	{
		bundle1Icon.gameObject.SetActive(_lastPurchasedBundle == InAppData.BundleTier.None);
		bundle2Icon.gameObject.SetActive(_lastPurchasedBundle == InAppData.BundleTier.BundleTier1);
		bundle3Icon.gameObject.SetActive(_lastPurchasedBundle == InAppData.BundleTier.BundleTier2);
		bundle4Icon.gameObject.SetActive(_lastPurchasedBundle == InAppData.BundleTier.BundleTier3);
		bundle5Icon.gameObject.SetActive(_lastPurchasedBundle == InAppData.BundleTier.BundleTier4);
	}

	private string GetFormattedAmount(int amount)
	{
		if (amount >= 10000)
		{
			return amount / 1000 + "K";
		}
		return amount.ToString();
	}
}
