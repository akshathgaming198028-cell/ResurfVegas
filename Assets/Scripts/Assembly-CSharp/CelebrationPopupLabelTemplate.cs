using SYBO.Subway.Info;
using UnityEngine;
using com.kiloo.awards;

public class CelebrationPopupLabelTemplate : MonoBehaviour
{
	public UILabel bigLabel;

	public UILabel subLabel;

	[SerializeField]
	private GameObject normalStateGO;

	[SerializeField]
	private GameObject megaJackpotStateGO;

	[SerializeField]
	private GameObject awardStateGO;

	[SerializeField]
	private GameObject topRunStateGO;

	[SerializeField]
	private GameObject characterEventStateGO;

	[SerializeField]
	private GameObject doubleUpEventStateGO;

	[SerializeField]
	private UILabel jackpotBigLabel;

	[SerializeField]
	private UILabel jackpotSubLabel;

	[SerializeField]
	private UILabel awardTopDescriptionLabel;

	[SerializeField]
	private UILabel awardBigLabel;

	[SerializeField]
	private UILabel awardBottomDescriptionLabel;

	[SerializeField]
	private UILabel doubleUpTitleLabel;

	[SerializeField]
	private UILabel doubleUpSubLabel;

	public UILabel doubleUpItemLabel;

	[SerializeField]
	private UILabel doubleUpDescribtionLabel;

	[SerializeField]
	private UILabel topRunTopDescriptionLabel;

	[SerializeField]
	private UILabel topRunBigLabel;

	[SerializeField]
	private UILabel topRunScore;

	[SerializeField]
	private UILabel topRunWeekNr;

	[SerializeField]
	private UILabel topRunCountry;

	[SerializeField]
	private UISprite topRunCountryFlag;

	[SerializeField]
	private UILabel topRunCheckMarkText;

	[SerializeField]
	private UILabel characterEventWordNumber;

	[SerializeField]
	private UILabel characterEventWordNumberShadow;

	[SerializeField]
	private UILabel characterEventWord;

	public float Alpha
	{
		get
		{
			return bigLabel.alpha;
		}
		set
		{
			bigLabel.alpha = Mathf.Clamp01(value);
		}
	}

	private void OnEnable()
	{
		normalStateGO.SetActive(false);
		megaJackpotStateGO.SetActive(false);
		awardStateGO.SetActive(false);
		topRunStateGO.SetActive(false);
		characterEventStateGO.SetActive(false);
		doubleUpEventStateGO.SetActive(false);
	}

	public void Init(int backgroundDepth)
	{
		UIPanel component = base.gameObject.GetComponent<UIPanel>();
		component.depth = backgroundDepth + 1;
	}

	public void SetupPowerup(PowerupType powerup, int amount, bool doubled = false)
	{
		normalStateGO.SetActive(true);
		Alpha = ((!doubled) ? 0f : 1f);
		bigLabel.pivot = ((powerup != PowerupType.headstart2000) ? bigLabel.pivot : UIWidget.Pivot.Bottom);
		bigLabel.text = _GetPowerupLabel(powerup, amount);
		subLabel.text = Strings.Get(Upgrades.upgrades[powerup].mysteryBoxDescription);
	}

	public void SetupCoins()
	{
		normalStateGO.SetActive(true);
		Alpha = 0f;
		bigLabel.text = string.Empty;
		subLabel.text = string.Empty;
	}

	public void UpdateCoins(int amount, bool isMegaJackpot = false, bool doubled = false)
	{
		if (doubled)
		{
			megaJackpotStateGO.SetActive(false);
			doubleUpItemLabel.text = _GetCoinsLabel(amount);
			subLabel.text = string.Empty;
		}
		else if (isMegaJackpot)
		{
			normalStateGO.SetActive(false);
			megaJackpotStateGO.SetActive(true);
			jackpotBigLabel.text = Strings.Get(StringID.CELEBRATION_LABEL_TEMPLATE_MEGA_JACKPOT);
			jackpotSubLabel.text = _GetCoinsLabel(amount);
		}
		else
		{
			normalStateGO.SetActive(true);
			bigLabel.text = _GetCoinsLabel(amount);
			subLabel.text = string.Empty;
		}
	}

	public void SetupKeys(int amount, bool doubled = false)
	{
		normalStateGO.SetActive(true);
		Alpha = ((!doubled) ? 0f : 1f);
		bigLabel.text = _GetKeysLabel(amount);
		subLabel.text = string.Empty;
	}

	public void SetupToken(Characters.CharacterType characterType, int amount, int tokensLeft, bool doubled = false)
	{
		normalStateGO.SetActive(true);
		Alpha = ((!doubled) ? 0f : 1f);
		bigLabel.text = amount + "x " + Characters.characterData[characterType].tokenName;
		Characters.Model model = Characters.characterData[characterType];
		if (tokensLeft > 0)
		{
			subLabel.text = string.Format(Strings.Get(StringID.CELEBRATION_POPUP_COLLECT_MORE_TOKENS), tokensLeft, Strings.Get(model.name));
		}
		else
		{
			subLabel.text = string.Format(Strings.Get(StringID.CELEBRATION_POPUP_YOU_UNLOCKED_TROPHY), Strings.Get(model.name));
		}
	}

	public void SetupTrophy(OldTrophies.Trophy trophy)
	{
		normalStateGO.SetActive(true);
		Alpha = 0f;
		bigLabel.text = _GetTrophyLabel(trophy);
		subLabel.text = Strings.Get(StringID.CELEBRATION_POPUP_TROPHY_SUBLABEL_TEXT);
	}

	public void SetupMedal()
	{
		normalStateGO.SetActive(true);
		Alpha = 0f;
		bigLabel.text = Strings.Get(StringID.CELEBRATION_POPUP_MEDAL_BIG_LABEL);
		subLabel.text = string.Empty;
	}

	public void SetupCharacter(string charName)
	{
		normalStateGO.SetActive(true);
		Alpha = 0f;
		bigLabel.text = string.Format(Strings.Get(StringID.CELEBRATION_POPUP_CHARACTER_UNLOCK), charName);
		subLabel.text = string.Empty;
	}

	public void SetupSpecialBoard(string boardName)
	{
		normalStateGO.SetActive(true);
		Alpha = 0f;
		bigLabel.text = string.Format(Strings.Get(StringID.CELEBRATION_POPUP_BOARD_UNLOCK), boardName);
		subLabel.text = string.Empty;
	}

	public void SetupEventSpecialBoard(string boardName)
	{
		normalStateGO.SetActive(true);
		Alpha = 0f;
		bigLabel.text = string.Format(Strings.Get(StringID.CELEBRATION_POPUP_EVENT_BOARD_TRYOUT), boardName);
		subLabel.text = string.Empty;
	}

	public void SetupAward(string awardTitle, int rewardAmount, AwardTierType type)
	{
		Alpha = 0f;
		string text = string.Empty;
		switch (type)
		{
		case AwardTierType.Bronze:
			text = Strings.Get(StringID.AWARD_CELEBRATION_BRONZE);
			break;
		case AwardTierType.Silver:
			text = Strings.Get(StringID.AWARD_CELEBRATION_SILVER);
			break;
		case AwardTierType.Gold:
			text = Strings.Get(StringID.AWARD_CELEBRATION_GOLD);
			break;
		}
		normalStateGO.SetActive(false);
		awardStateGO.SetActive(true);
		topRunStateGO.SetActive(false);
		doubleUpEventStateGO.SetActive(false);
		awardTopDescriptionLabel.text = awardTitle;
		awardBigLabel.text = text;
		awardBottomDescriptionLabel.text = "+" + rewardAmount + " Keys!";
	}

	public void SetupTopRun(int score, int rank, int weekNumber)
	{
		Alpha = 0f;
		normalStateGO.SetActive(false);
		doubleUpEventStateGO.SetActive(false);
		topRunStateGO.SetActive(true);
		topRunTopDescriptionLabel.text = Strings.Get(StringID.TOPRUN_CELEBRATION_TITLE);
		topRunScore.text = score + string.Empty;
		topRunWeekNr.text = string.Format(Strings.Get(StringID.TOPRUN_CELEBRATION_WEEKTITLE), weekNumber);
		topRunCountryFlag.transform.gameObject.SetActive(true);
		topRunCountry.transform.gameObject.SetActive(true);
		topRunCountryFlag.spriteName = CountryManager.instance.GetCurrentTournamentCountry().flag;
		topRunCountry.text = CountryManager.instance.GetCurrentTournamentCountry().name.ToUpper();
		topRunCheckMarkText.transform.gameObject.SetActive(false);
		switch (rank)
		{
		case 1:
			topRunBigLabel.text = Strings.Get(StringID.TOPRUN_RANK_0_MEDAL);
			topRunBigLabel.fontSize = 40;
			return;
		case 2:
			topRunBigLabel.text = Strings.Get(StringID.TOPRUN_RANK_1_MEDAL);
			topRunBigLabel.fontSize = 40;
			return;
		case 3:
			topRunBigLabel.text = Strings.Get(StringID.TOPRUN_RANK_2_MEDAL);
			topRunBigLabel.fontSize = 40;
			return;
		case 4:
			topRunBigLabel.text = Strings.Get(StringID.TOPRUN_RANK_3_MEDAL);
			topRunBigLabel.fontSize = 40;
			return;
		}
		topRunCheckMarkText.transform.gameObject.SetActive(true);
		topRunCheckMarkText.text = Strings.Get(StringID.TOPRUN_RANK_CHECKMARK_SUBTEXT);
		topRunBigLabel.text = Strings.Get(StringID.TOPRUN_RANK_CHECKMARK);
		topRunCountryFlag.transform.gameObject.SetActive(false);
		topRunCountry.transform.gameObject.SetActive(false);
		topRunBigLabel.fontSize = 30;
	}

	public void SetupDoubleUp(CelebrationReward reward)
	{
		Alpha = 0f;
		normalStateGO.SetActive(false);
		megaJackpotStateGO.SetActive(false);
		topRunStateGO.SetActive(false);
		doubleUpEventStateGO.SetActive(true);
		doubleUpDescribtionLabel.gameObject.SetActive(false);
		doubleUpTitleLabel.fontSize = 30;
		doubleUpTitleLabel.text = Strings.Get(StringID.DOUBLE_UP_CELEBRATION_SCREEN_TITLE);
		doubleUpSubLabel.text = Strings.Get(StringID.DOUBLE_UP_CELEBRATION_SCREEN_SUBTITLE);
		if (reward.rewardType == CelebrationRewardType.coins)
		{
			doubleUpItemLabel.text = _GetCoinsLabel(reward.amount);
		}
		else if (reward.rewardType == CelebrationRewardType.token)
		{
			doubleUpDescribtionLabel.gameObject.SetActive(true);
			Characters.Model model = Characters.characterData[reward.characterType];
			int num = model.Price - PlayerInfo.Instance.GetCollectedTokens(reward.characterType) - reward.amount;
			doubleUpItemLabel.text = reward.amount * 2 + "x " + Characters.characterData[reward.characterType].tokenName;
			if (num > 0)
			{
				doubleUpDescribtionLabel.text = string.Format(Strings.Get(StringID.CELEBRATION_POPUP_COLLECT_MORE_TOKENS), num, Strings.Get(model.name));
			}
			else
			{
				doubleUpDescribtionLabel.text = string.Format(Strings.Get(StringID.CELEBRATION_POPUP_YOU_UNLOCKED_TROPHY), Strings.Get(model.name));
			}
		}
		else if (reward.rewardType == CelebrationRewardType.keys)
		{
			doubleUpItemLabel.text = _GetKeysLabel(reward.amount * 2);
		}
		else if (reward.rewardType == CelebrationRewardType.powerup)
		{
			doubleUpItemLabel.text = _GetPowerupLabel(reward.powerupType, reward.amount * 2);
			doubleUpDescribtionLabel.gameObject.SetActive(true);
			doubleUpDescribtionLabel.text = Strings.Get(Upgrades.upgrades[reward.powerupType].mysteryBoxDescription);
		}
	}

	public void SetupCharacterWordEvent(int wordNumber, string word)
	{
		Alpha = 0f;
		normalStateGO.SetActive(false);
		doubleUpEventStateGO.SetActive(false);
		characterEventStateGO.SetActive(true);
		string text = NGUIText.EncodeColor(GlobalColors.CHARACTERWORDEVENT_CELEBRATION_BLUE);
		string text2 = NGUIText.EncodeColor(GlobalColors.CHARACTERWORDEVENT_CELEBRATION_WHITE);
		string text3 = NGUIText.EncodeColor(GlobalColors.CHARACTERWORDEVENT_CELEBRATION_BLUESHADOW);
		string text4 = NGUIText.EncodeColor(GlobalColors.CHARACTERWORDEVENT_CELEBRATION_WHITESHADOW);
		characterEventWordNumber.text = "[" + text + "]" + Strings.Get(StringID.CELEBRATION_LABEL_TEMPLATE_CHARACTEREVENT_PART1) + " [" + text2 + "]#" + wordNumber + " [" + text + "]" + Strings.Get(StringID.CELEBRATION_LABEL_TEMPLATE_CHARACTEREVENT_PART2);
		characterEventWordNumberShadow.text = "[" + text4 + "]" + Strings.Get(StringID.CELEBRATION_LABEL_TEMPLATE_CHARACTEREVENT_PART1) + " [" + text3 + "]#" + wordNumber + " [" + text4 + "]" + Strings.Get(StringID.CELEBRATION_LABEL_TEMPLATE_CHARACTEREVENT_PART2);
		string text5 = string.Empty;
		for (int i = 0; i < word.Length; i++)
		{
			text5 = text5 + word[i] + " ";
		}
		string text6 = text5.Substring(0, text5.Length - 1);
		characterEventWord.text = text6;
	}

	private string _GetPowerupLabel(PowerupType type, int amount)
	{
		string empty = string.Empty;
		empty = empty + amount + "x ";
		return empty + Strings.Get(Upgrades.upgrades[type].GetName());
	}

	private string _GetTrophyLabel(OldTrophies.Trophy trophy)
	{
		return Strings.Get(OldTrophies.trophyData[trophy].name);
	}

	private string _GetCoinsLabel(int amount)
	{
		return string.Format(Strings.Get(StringID.CELEBRATION_POPUP_COINS_AMOUNT), string.Format("{0:#,###0}", amount));
	}

	private string _GetKeysLabel(int amount)
	{
		if (amount > 1)
		{
			return string.Format(Strings.Get(StringID.CELEBRATION_POPUP_KEYS_AMOUNT), amount.ToString());
		}
		return string.Format(Strings.Get(StringID.CELEBRATION_POPUP_KEY_AMOUNT), amount.ToString());
	}
}
