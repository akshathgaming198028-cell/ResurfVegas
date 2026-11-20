using UnityEngine;

public class UISlideInVideoPayoutHelper : UISlideIn
{
	[SerializeField]
	private UILabel _price;

	public void SetupSlideInVideoPayout()
	{
		SetupSlideIn();
		if (_price != null)
		{
			_price.text = VideoAdsManager.instance.GetAmountOfKeysRewardedForVideo().ToString();
		}
	}
}
