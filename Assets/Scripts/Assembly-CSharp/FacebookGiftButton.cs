using System;
using UnityEngine;

public class FacebookGiftButton : MonoBehaviour
{
	[SerializeField]
	private UILabel title;

	[SerializeField]
	private UILabel description;

	private int earnCurrencyProfileIndex;

	private Action _onClickDelegate;

	public void Init(int earnCurrencyProfileIndex, string earnerTitle, string earnerDesc, Action onClickDelegate, UIScrollView _panel)
	{
		this.earnCurrencyProfileIndex = earnCurrencyProfileIndex;
		title.text = earnerTitle;
		description.text = earnerDesc;
		_onClickDelegate = onClickDelegate;
		base.gameObject.GetComponent<UIDragScrollView>().scrollView = _panel;
	}

	private void OnClick()
	{
		EarnCoinsData.Trigger(earnCurrencyProfileIndex);
		if (_onClickDelegate != null)
		{
			_onClickDelegate();
		}
	}
}
