using System;
using UnityEngine;

public class RestorePurchaseHelper : MonoBehaviour
{
	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private UILabel descriptionLabel;

	public void setOnClickDelegate(Action onFinishDelegate)
	{
	}

	private void OnClick()
	{
		InAppManager.instance.RestoreNonConsumablePurchases(InAppData.GetAllProductIds());
	}

	private void Awake()
	{
		titleLabel.text = Strings.Get(StringID.RESTORE_BUTTON_TITLE);
		descriptionLabel.text = Strings.Get(StringID.RESTORE_BUTTON_DESCRIPTION);
	}
}
