using UnityEngine;

public class NotEnoughCurencyPopup : UIScreen
{
	private enum PopupClosingAction
	{
		none = 0,
		buy = 1,
		cancel = 2,
		ok = 3
	}

	[SerializeField]
	private UILabel popupTitle;

	[SerializeField]
	private UILabel popupDescription;

	[SerializeField]
	private GameObject buyButton;

	[SerializeField]
	private GameObject cancelButton;

	[SerializeField]
	private GameObject okButton;

	[SerializeField]
	private GameObject uiContentCoins;

	[SerializeField]
	private GameObject uiContentKeys;

	private PopupClosingAction _closingAction;

	private InAppManagerPopupData _popupData;

	public override void Show()
	{
		base.Show();
		_popupData = InAppManager.instance.GetPopupData();
		if (_popupData == null)
		{
			return;
		}
		popupTitle.text = _popupData.popupTitle;
		popupDescription.text = _popupData.popupDescription;
		if (_popupData.isErrorMessage)
		{
			uiContentCoins.SetActive(false);
			uiContentKeys.SetActive(false);
		}
		else if (_popupData.isCoinsPack)
		{
			uiContentCoins.SetActive(true);
			uiContentKeys.SetActive(false);
			if (base.parentScreen == "CharacterNameEventPopup")
			{
				Flurry.LogEvent("Attempt to buy coins for Wordy Weekend");
			}
		}
		else
		{
			uiContentCoins.SetActive(false);
			uiContentKeys.SetActive(true);
			if (base.parentScreen == "CharacterNameEventPopup")
			{
				Flurry.LogEvent("Attempt to buy keys for the Wordy Weekend");
			}
		}
		if (_popupData.canMakePurchase)
		{
			buyButton.SetActive(true);
			cancelButton.SetActive(true);
			okButton.SetActive(false);
		}
		else
		{
			buyButton.SetActive(false);
			cancelButton.SetActive(false);
			okButton.SetActive(true);
		}
	}

	public override void Hide()
	{
		base.Hide();
		if (_closingAction == PopupClosingAction.none)
		{
			Payout(PopupClosingAction.cancel);
		}
		_closingAction = PopupClosingAction.none;
		_popupData = null;
	}

	public void OnOkClicked()
	{
		_closingAction = PopupClosingAction.ok;
		Payout(_closingAction);
		if (UIScreenController.IsInstanced)
		{
			UIScreenController.Instance.CloseActivePopups(screenName);
		}
	}

	public void OnBuyClicked()
	{
		_closingAction = PopupClosingAction.buy;
		Payout(_closingAction);
		if (UIScreenController.IsInstanced)
		{
			UIScreenController.Instance.CloseActivePopups(screenName);
		}
	}

	public void OnCancelClicked()
	{
		_closingAction = PopupClosingAction.cancel;
		Payout(_closingAction);
		if (UIScreenController.IsInstanced)
		{
			UIScreenController.Instance.CloseActivePopups(screenName);
		}
	}

	private void Payout(PopupClosingAction action)
	{
		if (_popupData == null)
		{
			return;
		}
		switch (action)
		{
		case PopupClosingAction.buy:
			InAppManager.instance.NativePurchaseInappPack("1");
			InAppManager.instance.DeletePopupData();
			if (base.parentScreen == "CharacterNameEventPopup" && !_popupData.isErrorMessage)
			{
				if (_popupData.isCoinsPack)
				{
					Flurry.LogEvent("Bought coins for Wordy Weekend");
				}
				else
				{
					Flurry.LogEvent("Bought keys for Wordy Weekend");
				}
			}
			break;
		case PopupClosingAction.ok:
			InAppManager.instance.NativePurchaseFail("0");
			InAppManager.instance.DeletePopupData();
			if (base.parentScreen == "CharacterNameEventPopup" && !_popupData.isErrorMessage)
			{
				if (_popupData.isCoinsPack)
				{
					Flurry.LogEvent("Unable to buy coins for Wordy Weekend");
				}
				else
				{
					Flurry.LogEvent("Unable to buy keys for Wordy Weekend");
				}
			}
			break;
		case PopupClosingAction.cancel:
			InAppManager.instance.NativePurchaseInappPack("0");
			InAppManager.instance.DeletePopupData();
			if (base.parentScreen == "CharacterNameEventPopup" && !_popupData.isErrorMessage)
			{
				if (_popupData.isCoinsPack)
				{
					Flurry.LogEvent("Canceled buying coins for Wordy Weekend");
				}
				else
				{
					Flurry.LogEvent("Canceled buying keys for Wordy Weekend");
				}
			}
			break;
		}
	}
}
