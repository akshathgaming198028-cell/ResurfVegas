using System;
using UnityEngine;

public class BuyHoverboardButton : MonoBehaviour
{
	private const float AMOUNT_LABEL_HIGH_RES_OFFSET = 1f;

	private bool _purchaseInProgress;

	[SerializeField]
	private UILabel amountLabel;

	[SerializeField]
	private UILabel priceLabel;

	private bool _amountLabelRepositioned;

	private void Start()
	{
		priceLabel.text = Upgrades.upgrades[PowerupType.hoverboard].getPrice(0).ToString();
	}

	private void OnEnable()
	{
		PlayerInfo instance = PlayerInfo.Instance;
		instance.onPowerupAmountChanged = (Action<PowerupType, int>)Delegate.Combine(instance.onPowerupAmountChanged, new Action<PowerupType, int>(UpdateLabels));
		UpdateLabels(PowerupType.hoverboard, PlayerInfo.Instance.GetUpgradeAmount(PowerupType.hoverboard));
	}

	private void OnDisable()
	{
		PlayerInfo instance = PlayerInfo.Instance;
		instance.onPowerupAmountChanged = (Action<PowerupType, int>)Delegate.Remove(instance.onPowerupAmountChanged, new Action<PowerupType, int>(UpdateLabels));
	}

	private void OnClick()
	{
		if (!_purchaseInProgress)
		{
			PurchaseHandler.Instance.PurchaseHoverboard(this);
			Flurry.LogEvent("Purchase hoverboard from popup");
		}
	}

	private void UpdateLabels(PowerupType powerupType, int newAmount)
	{
		if (powerupType == PowerupType.hoverboard)
		{
			amountLabel.text = newAmount.ToString();
			if (DeviceInfo.Instance.isHighres && !_amountLabelRepositioned)
			{
				amountLabel.transform.localPosition = new Vector3(amountLabel.transform.localPosition.x + 1f, amountLabel.transform.localPosition.y, amountLabel.transform.localPosition.z);
				_amountLabelRepositioned = true;
			}
		}
	}

	public void PurchaseSuccessful()
	{
		_purchaseInProgress = false;
	}

	public void PurchaseFailure()
	{
		_purchaseInProgress = false;
	}
}
