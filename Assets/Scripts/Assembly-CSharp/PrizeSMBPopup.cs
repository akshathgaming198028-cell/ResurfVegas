using UnityEngine;

public class PrizeSMBPopup : MonoBehaviour
{
	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private UILabel subtitleLabel;

	[SerializeField]
	private UILabel superLabel;

	[SerializeField]
	private UILabel superShadowLabel;

	[SerializeField]
	private UILabel jackpotLabel;

	[SerializeField]
	private UILabel jackpotShadowLabel;

	[SerializeField]
	private UILabel coinsTitleLabel;

	[SerializeField]
	private UILabel coinsTitleShadowLabel;

	[SerializeField]
	private UILabel newSuperTitleLabel;

	[SerializeField]
	private UILabel newSuperTitleShadowLabel;

	[SerializeField]
	private UILabel newJackpotTitleLabel;

	[SerializeField]
	private UILabel newJackpotTitleShadowLabel;

	[SerializeField]
	private UILabel oldJackpotLabel;

	[SerializeField]
	private UILabel oldJackpotShadowLabel;

	[SerializeField]
	private UILabel newJackpotLabel;

	[SerializeField]
	private UILabel newJackpotShadowLabel;

	[SerializeField]
	private UILabel rareLabel;

	[SerializeField]
	private UILabel keyLabel;

	[SerializeField]
	private UILabel scoreLabel;

	[SerializeField]
	private UILabel boosterLabel;

	[SerializeField]
	private UILabel trophies;

	[SerializeField]
	private UILabel amountOfCoinsLabel;

	[SerializeField]
	private UILabel coinsAmountLabel;

	[SerializeField]
	private UILabel specialLabel;

	[SerializeField]
	private UILabel megaLabel;

	[SerializeField]
	private UILabel headstartsLabel;

	[SerializeField]
	private UILabel upTo20BoardsLabel;

	[SerializeField]
	private UILabel boardsUpTo20BoardsLabel;

	[SerializeField]
	private UILabel upToCoinsLabel;

	[SerializeField]
	private UILabel coinsLabel;

	[SerializeField]
	private UILabel commonLabel;

	[SerializeField]
	private GameObject defaultJackpotElements;

	[SerializeField]
	private GameObject megaJackpotElements;

	private void Awake()
	{
		titleLabel.text = Strings.Get(StringID.PRIZE_SMB_POPUP_TITLE);
		subtitleLabel.text = Strings.Get(StringID.PRIZE_SMB_POPUP_SUBTITLE);
		coinsTitleLabel.text = Strings.Get(StringID.PRIZE_MB_POPUP_COINS_TITLE);
		coinsTitleShadowLabel.text = Strings.Get(StringID.PRIZE_MB_POPUP_COINS_TITLE);
		rareLabel.text = Strings.Get(StringID.PRIZE_SMB_RARE);
		if (keyLabel != null)
		{
			keyLabel.text = Strings.Get(StringID.PRIZE_SMB_KEYS);
		}
		scoreLabel.text = Strings.Get(StringID.PRIZE_SMB_SCORE);
		boosterLabel.text = Strings.Get(StringID.PRIZE_SMB_BOOSTER);
		trophies.text = Strings.Get(StringID.PRIZE_MB_POPUP_TROPHIES);
		amountOfCoinsLabel.text = Strings.Get(StringID.PRIZE_SMB_AMOUNT_OF_COINS);
		coinsAmountLabel.text = Strings.Get(StringID.PRIZE_SMB_COINS);
		specialLabel.text = Strings.Get(StringID.PRIZE_SMB_SPECIAL);
		megaLabel.text = Strings.Get(StringID.PRIZE_SMB_MEGA);
		headstartsLabel.text = Strings.Get(StringID.PRIZE_SMB_HEADSTARTS);
		upTo20BoardsLabel.text = Strings.Get(StringID.PRIZE_SMB_UP_TO_20_BOARDS);
		upToCoinsLabel.text = Strings.Get(StringID.PRIZE_SMB_UP_TO_COINS);
		coinsLabel.text = Strings.Get(StringID.PRIZE_SMB_COINS);
		commonLabel.text = Strings.Get(StringID.PRIZE_SMB_COMMON);
	}

	private void OnEnable()
	{
		SSEventsManagerController.OnSSEventStartedHandler += SetJackpotLabels;
		SSEventsManagerController.OnSSEventFinishedHandler += SetJackpotLabels;
		UIScreenController.OnApplicationResumed += SetJackpotLabels;
		SetJackpotLabels();
	}

	private void OnDisable()
	{
		SSEventsManagerController.OnSSEventStartedHandler -= SetJackpotLabels;
		SSEventsManagerController.OnSSEventFinishedHandler -= SetJackpotLabels;
		UIScreenController.OnApplicationResumed -= SetJackpotLabels;
	}

	private void SetJackpotLabels()
	{
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
		if (activeEventManager != null && activeEventManager.GetEventType() == SSEventType.BiggerJackpot)
		{
			defaultJackpotElements.SetActive(false);
			megaJackpotElements.SetActive(true);
			newJackpotTitleLabel.text = Strings.Get(StringID.PRIZE_MB_POPUP_JACKPOT);
			newJackpotTitleShadowLabel.text = Strings.Get(StringID.PRIZE_MB_POPUP_JACKPOT);
			newSuperTitleLabel.text = Strings.Get(StringID.PRIZE_SMB_SUPER);
			newSuperTitleShadowLabel.text = Strings.Get(StringID.PRIZE_SMB_SUPER);
			string text = string.Format(Strings.Get(StringID.PRIZE_MB_POPUP_COINS_TITLE), string.Format("{0:#,###0}", MysteryBox.superMysteryBoxJackpotAmount));
			string text2 = string.Format(Strings.Get(StringID.PRIZE_MB_POPUP_COINS_TITLE), string.Format("{0:#,###0}", MysteryBox.GetMegaJackpotAmount(MysteryBox.Type.Super)));
			oldJackpotLabel.text = text;
			oldJackpotShadowLabel.text = text;
			newJackpotLabel.text = text2;
			newJackpotShadowLabel.text = text2;
		}
		else
		{
			defaultJackpotElements.SetActive(true);
			megaJackpotElements.SetActive(false);
			string text3 = string.Format(Strings.Get(StringID.PRIZE_MB_POPUP_COINS_TITLE), string.Format("{0:#,###0}", MysteryBox.superMysteryBoxJackpotAmount));
			coinsTitleLabel.text = text3;
			coinsTitleShadowLabel.text = text3;
			jackpotLabel.text = Strings.Get(StringID.PRIZE_MB_POPUP_JACKPOT);
			jackpotShadowLabel.text = Strings.Get(StringID.PRIZE_MB_POPUP_JACKPOT);
			superLabel.text = Strings.Get(StringID.PRIZE_SMB_SUPER);
			superShadowLabel.text = Strings.Get(StringID.PRIZE_SMB_SUPER);
		}
	}
}
