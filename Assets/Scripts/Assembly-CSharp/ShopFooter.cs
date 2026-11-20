using UnityEngine;

public class ShopFooter : MonoBehaviour
{
	[SerializeField]
	private UILabel menuButtonLabel;

	[SerializeField]
	private UILabel boostsButtonLabel;

	[SerializeField]
	private UILabel boostsCoolDealLabel;

	[SerializeField]
	private UILabel boostsCoolDealSingleLineLabel;

	[SerializeField]
	private UILabel earnCoinsButtonLabel;

	[SerializeField]
	private UILabel storeButtonLabel;

	[SerializeField]
	private UILabel storeCoolDealLabel;

	[SerializeField]
	private UILabel storeCoolDealSingleLineLabel;

	private void Awake()
	{
		menuButtonLabel.text = Strings.Get(StringID.SHOP_FOOTER_MENU);
		boostsButtonLabel.text = Strings.Get(StringID.SHOP_FOOTER_BOOSTS);
		boostsCoolDealSingleLineLabel.text = Strings.Get(StringID.SHOP_FOOTER_COOL_DEAL_SINGLE_LINE);
		earnCoinsButtonLabel.text = Strings.Get(StringID.SHOP_FOOTER_EARN_COINS);
		storeButtonLabel.text = Strings.Get(StringID.SHOP_FOOTER_STORE);
		storeCoolDealSingleLineLabel.text = Strings.Get(StringID.SHOP_FOOTER_COOL_DEAL_SINGLE_LINE);
	}
}
