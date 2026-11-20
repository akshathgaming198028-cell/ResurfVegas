using System.Diagnostics;
using UnityEngine;

public class DoubleCoinUpgradeHelper : DiscountButton
{
	private Vector3 _shownDiscStickerPos = new Vector3(126f, 28f, 0f);

	private Vector3 _shownDiscLabelPos = new Vector3(125f, 28.5f, 0f);

	private Vector3 _shownDiscLabelSingleLinePos = new Vector3(127f, 29f, 0f);

	private string doubleCoinVersion;

	private bool inited;

	public bool OnSale
	{
		get
		{
			if (InAppManager.IsDoubleCoinsDiscountActive)
			{
				int value = OnlineSettings.instance.GetValue(InAppData.GetInAppOnlineSettingsId("com.kiloo.subways.doublecoins"), 0);
				if (value < 0)
				{
					return true;
				}
			}
			return false;
		}
	}

	public override void Awake()
	{
		base.Awake();
	}

	private void ForceEndDiscount()
	{
		_Setup(true);
	}

	private void _DefaultSetup(string data)
	{
		_Setup();
	}

	private void _Setup(bool forceEndDiscount = false)
	{
		if (PlayerInfo.Instance.hasDoubleCoins)
		{
			doubleCoinVersion = "com.kiloo.subways.doublecoins";
			ShowNoDiscount(doubleCoinVersion, string.Empty);
			fillColorSprite.spriteName = "button_check_grey";
			fillColorSprite.pivot = UIWidget.Pivot.Center;
			if (DeviceInfo.Instance.isHighres)
			{
				fillColorSprite.width = Mathf.RoundToInt((float)fillColorSprite.GetAtlasSprite().width / 2f);
				fillColorSprite.height = Mathf.RoundToInt((float)fillColorSprite.GetAtlasSprite().height / 2f);
			}
			else
			{
				fillColorSprite.width = Mathf.RoundToInt(fillColorSprite.GetAtlasSprite().width);
				fillColorSprite.height = Mathf.RoundToInt(fillColorSprite.GetAtlasSprite().height);
			}
			fillColorSprite.cachedTransform.localPosition = new Vector3(97f, 2f, 0f);
			price.enabled = false;
			GetComponent<BoxCollider>().enabled = false;
		}
		else
		{
			if (OnSale && !forceEndDiscount)
			{
				doubleCoinVersion = "com.kiloo.subways.doublecoinsdiscount";
				ShowOnSale("com.kiloo.subways.doublecoinsdiscount", string.Empty);
				if (!(base.transform.parent.name != "000"))
				{
					return;
				}
				{
					foreach (Transform item in base.transform.parent)
					{
						if (item.name == "000")
						{
							base.transform.parent = item;
							base.transform.localPosition = new Vector3(0f, -75f, 0f);
						}
					}
					return;
				}
			}
			doubleCoinVersion = "com.kiloo.subways.doublecoins";
			ShowNoDiscount("com.kiloo.subways.doublecoins", string.Empty);
			if (base.transform.parent.name == "000")
			{
				base.transform.parent = base.transform.parent.parent;
			}
		}
	}

	public void Init()
	{
		inited = true;
		_Setup();
	}

	private void OnEnable()
	{
		if (InAppManager.instance != null)
		{
			InAppManager.instance.onPurchaseSuccess += _DefaultSetup;
		}
		if (inited)
		{
			_Setup();
		}
	}

	private void OnDisable()
	{
		if (InAppManager.instance != null)
		{
			InAppManager.instance.onPurchaseSuccess -= _DefaultSetup;
		}
	}

	private void UpdatePrice()
	{
		price.text = InAppData.inAppData[doubleCoinVersion].LocalizedPrice;
	}

	private void OnClick()
	{
		if (!PlayerInfo.Instance.hasDoubleCoins)
		{
			InAppManager.instance.StartPurchase(doubleCoinVersion);
		}
	}

	protected override void ShowOnPromotion(string productString, string sendDescription, string sendDescription2)
	{
		base.ShowOnPromotion(productString, sendDescription, sendDescription2);
		EnableDiscount();
	}

	protected override void ShowOnSale(string productString, string sendDescription = "")
	{
		base.ShowOnSale(productString, sendDescription);
		EnableDiscount();
	}

	protected override void ShowNoDiscount(string productString, string backupDescription = "")
	{
		base.ShowNoDiscount(productString, backupDescription);
		DisableDiscount();
	}

	private void EnableDiscount()
	{
		discountSticker.cachedTransform.localPosition = _shownDiscStickerPos;
		discountLabel.cachedTransform.localPosition = _shownDiscLabelPos;
		discountLabelSingleLine.cachedTransform.localPosition = _shownDiscLabelSingleLinePos;
	}

	private void DisableDiscount()
	{
		discountSticker.cachedTransform.localPosition = Vector3.zero;
		discountLabel.cachedTransform.localPosition = new Vector3(0f, 0f, -1f);
		discountLabelSingleLine.cachedTransform.localPosition = new Vector3(0f, 0f, -1f);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	public static void LogError(string msg, Object context = null)
	{
		Debug.LogError(msg, context);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	public static void LogWarning(string msg, Object context = null)
	{
		Debug.LogWarning(msg, context);
	}

	[Conditional("ENABLE_DOUBLECOINS_LOGS")]
	public static void Log(string msg, Object context = null)
	{
		Debug.Log(msg, context);
	}
}
