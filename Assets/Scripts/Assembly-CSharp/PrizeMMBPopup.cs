using UnityEngine;

public class PrizeMMBPopup : MonoBehaviour
{
	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private UILabel subtitleLabel;

	[SerializeField]
	private UILabel rareLabel;

	[SerializeField]
	private UILabel upToKeyLabel;

	[SerializeField]
	private UILabel keyLabel;

	[SerializeField]
	private UILabel scoreLabel;

	[SerializeField]
	private UILabel boosterLabel;

	[SerializeField]
	private UILabel commonLabel;

	[SerializeField]
	private UILabel upToBoardsLabel;

	[SerializeField]
	private UILabel boardsLabel;

	[SerializeField]
	private UILabel upToCoinsLabel;

	[SerializeField]
	private UILabel coinsLabel;

	private void Awake()
	{
		titleLabel.text = Strings.Get(StringID.PRIZE_MMB_POPUP_TITLE);
		subtitleLabel.text = Strings.Get(StringID.PRIZE_MMB_POPUP_SUBTITLE);
		rareLabel.text = Strings.Get(StringID.PRIZE_MMB_POPUP_RARE);
		upToKeyLabel.text = Strings.Get(StringID.PRIZE_MMB_POPUP_UP_TO_KEYS);
		keyLabel.text = Strings.Get(StringID.PRIZE_MMB_POPUP_KEY);
		scoreLabel.text = Strings.Get(StringID.PRIZE_MMB_POPUP_SCORE);
		boosterLabel.text = Strings.Get(StringID.PRIZE_MMB_POPUP_BOOSTER);
		commonLabel.text = Strings.Get(StringID.PRIZE_MMB_POPUP_COMMON);
		upToBoardsLabel.text = Strings.Get(StringID.PRIZE_MMB_POPUP_UP_TO_BOARDS);
		boardsLabel.text = Strings.Get(StringID.PRIZE_MMB_POPUP_BOARDS);
		upToCoinsLabel.text = Strings.Get(StringID.PRIZE_MMB_POPUP_UP_TO_COINS);
		coinsLabel.text = Strings.Get(StringID.PRIZE_MMB_POPUP_COINS);
	}
}
