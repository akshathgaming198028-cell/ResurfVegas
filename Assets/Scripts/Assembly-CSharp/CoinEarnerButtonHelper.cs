using System;
using UnityEngine;

public class CoinEarnerButtonHelper : MonoBehaviour
{
	[SerializeField]
	private GameObject _normalContent;

	[SerializeField]
	private GameObject _connectingContent;

	[SerializeField]
	private UIAnchor _priceAnchor;

	[SerializeField]
	private BoxCollider _collider;

	public UISprite icon;

	public UILabel title;

	public UILabel description;

	public UILabel priceLabel;

	private int earnCurrencyProfileIndex;

	private Action _onClickDelegate;

	public void Init(int earnCurrencyProfileIndex, string title, string desc, string iconName, int price, Action onClickDelegate)
	{
		this.earnCurrencyProfileIndex = earnCurrencyProfileIndex;
		this.title.text = title;
		_onClickDelegate = onClickDelegate;
		icon.spriteName = iconName;
		description.text = desc;
		if (priceLabel != null)
		{
			priceLabel.text = price.ToString();
		}
		icon.MakePixelPerfect();
		_priceAnchor.pixelOffset = new Vector2(priceLabel.width / 2 + 8, -10f);
		GoToNormalState();
	}

	private void OnClick()
	{
		if (EarnCoinsData.profiles[earnCurrencyProfileIndex].id == EarnCoinsData.Id.FacebookLogin)
		{
			EarnCoinsScreen.FacebookLoginClicked();
			GoToConnectingState();
		}
		else
		{
			EarnCoinsData.Trigger(earnCurrencyProfileIndex);
		}
		if (_onClickDelegate != null)
		{
			_onClickDelegate();
		}
	}

	private void GoToNormalState()
	{
		_normalContent.SetActive(true);
		_connectingContent.SetActive(false);
		_collider.enabled = true;
	}

	private void GoToConnectingState()
	{
		_normalContent.SetActive(false);
		_connectingContent.SetActive(true);
		_collider.enabled = false;
	}
}
