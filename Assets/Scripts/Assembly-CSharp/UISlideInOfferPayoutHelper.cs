using UnityEngine;

public class UISlideInOfferPayoutHelper : UISlideIn
{
	[SerializeField]
	private UILabel _price;

	private void Awake()
	{
		if (_price == null)
		{
			Debug.LogError("Price label is not set in the UISlideInOfferPayouHelper");
		}
	}

	public void SetupSlideInOffer(string sum)
	{
		base.gameObject.SetActive(true);
		_price.text = sum.ToString();
		SlideIn();
	}
}
