using UnityEngine;

public class UISlideInBundlesPack : UISlideIn
{
	[SerializeField]
	private UILabel _packNameLabel;

	[SerializeField]
	private UISprite _iconPack;

	public void SetupSlideInOffer(string sum)
	{
		if (sum.Equals("pack1"))
		{
			_packNameLabel.text = Strings.Get(StringID.INAPP_DATA_BUNDLE_TIER_ONE);
			_iconPack.spriteName = "icon_pack1";
		}
		else if (sum.Equals("pack2"))
		{
			_packNameLabel.text = Strings.Get(StringID.INAPP_DATA_BUNDLE_TIER_TWO);
			_iconPack.spriteName = "icon_pack2";
		}
		else if (sum.Equals("pack3"))
		{
			_packNameLabel.text = Strings.Get(StringID.INAPP_DATA_BUNDLE_TIER_THREE);
			_iconPack.spriteName = "icon_pack3";
		}
		else if (sum.Equals("pack4"))
		{
			_packNameLabel.text = Strings.Get(StringID.INAPP_DATA_BUNDLE_TIER_FOUR);
			_iconPack.spriteName = "icon_pack4";
		}
		else if (sum.Equals("pack5"))
		{
			_packNameLabel.text = Strings.Get(StringID.INAPP_DATA_BUNDLE_TIER_FIVE);
			_iconPack.spriteName = "icon_pack5";
		}
		base.gameObject.SetActive(true);
		SlideIn();
	}
}
