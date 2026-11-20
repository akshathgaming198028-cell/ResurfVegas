using UnityEngine;

public class FacebookRewardsButton : MonoBehaviour
{
	public UpgradeScreen upgradeScreen;

	[SerializeField]
	private GameObject activeContent;

	[SerializeField]
	private GameObject connectingContent;

	[SerializeField]
	private UIAnchor contentAnchor;

	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private UILabel priceLabel;

	[SerializeField]
	private BundleContentSlot coinsSlot;

	[SerializeField]
	private BundleContentSlot hoverboardsSlot;

	[SerializeField]
	private BundleContentSlot keysSlot;

	[SerializeField]
	private BundleContentSlot headstartSlot;

	[SerializeField]
	private BundleContentSlot characterSlot;

	private void OnEnable()
	{
		if (FacebookRewardsInfo.HasReceivedAllAvailableRewards())
		{
			Object.Destroy(base.gameObject);
			return;
		}
		InitializeButton();
		GoToActiveState();
	}

	private void InitializeButton()
	{
		titleLabel.text = Strings.Get(StringID.FACEBOOK_REWARDS_BUTTON_TITLE);
		priceLabel.text = Strings.Get(StringID.FACEBOOK_REWARDS_BUTTON_LOG_IN);
		coinsSlot.UpdateAmount(string.Empty + FacebookRewardsInfo.facebookRewardPack[FacebookRewardsInfo.FacebookRewardType.Coins]);
		hoverboardsSlot.UpdateAmount(string.Empty + FacebookRewardsInfo.facebookRewardPack[FacebookRewardsInfo.FacebookRewardType.Hoverboards]);
		keysSlot.UpdateAmount(string.Empty + FacebookRewardsInfo.facebookRewardPack[FacebookRewardsInfo.FacebookRewardType.Keys]);
		headstartSlot.UpdateAmount(string.Empty + FacebookRewardsInfo.facebookRewardPack[FacebookRewardsInfo.FacebookRewardType.Headstarts]);
		characterSlot.UpdateAmount("1");
		if (PlayerInfo.Instance.hasPayedOutFacebook)
		{
			contentAnchor.pixelOffset = new Vector2(34f, -2f);
			coinsSlot.gameObject.SetActive(false);
		}
		else
		{
			contentAnchor.pixelOffset = new Vector2(64f, -2f);
			coinsSlot.gameObject.SetActive(true);
		}
	}

	private void OnClick()
	{
		GoToConnectingState();
		upgradeScreen.FacebookLoginClicked();
	}

	public void GoToActiveState()
	{
		activeContent.SetActive(true);
		connectingContent.SetActive(false);
		GetComponent<BoxCollider>().enabled = true;
	}

	public void GoToConnectingState()
	{
		activeContent.SetActive(false);
		connectingContent.SetActive(true);
		GetComponent<BoxCollider>().enabled = false;
	}
}
