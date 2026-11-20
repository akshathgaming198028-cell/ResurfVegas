using System;
using System.Linq;
using SYBO.Subway.Info;
using UnityEngine;
using com.kiloo.awards;

public class CelebrationTesterPopup : UIScreen
{
	[SerializeField]
	private SelectPopupButton _originPopupButton;

	[SerializeField]
	private SelectPopupButton _typePopupButton;

	[SerializeField]
	private SelectPopupButton _powerupPopupButton;

	[SerializeField]
	private SelectPopupButton _charPopupButton;

	[SerializeField]
	private SelectPopupButton _boardPopupButton;

	[SerializeField]
	private SelectPopupButton _trophyPopupButton;

	[SerializeField]
	private SelectPopupButton _awardPopupButton;

	[SerializeField]
	private SelectPopupButton _amountPopupButton;

	[SerializeField]
	private SelectPopupButton _topRunRankPopupButton;

	[SerializeField]
	private UILabel buttonLabel;

	[SerializeField]
	private GameObject charButton;

	[SerializeField]
	private GameObject boardsButton;

	[SerializeField]
	private GameObject powerupButtons;

	[SerializeField]
	private GameObject trrankButton;

	[SerializeField]
	private GameObject awardButton;

	private static CelebrationReward _currentCelebrationReward = new CelebrationReward();

	private void Start()
	{
		_currentCelebrationReward.characterOutfitIndex = 0;
		_currentCelebrationReward.awardTitle = "Award Title";
		EventDelegate.Add(_originPopupButton.onChange, OriginSelected);
		_originPopupButton.SetItems(Enum.GetNames(typeof(CelebrationRewardOrigin)).ToList());
		EventDelegate.Add(_typePopupButton.onChange, TypeSelected);
		_typePopupButton.SetItems(Enum.GetNames(typeof(CelebrationRewardType)).ToList());
		EventDelegate.Add(_powerupPopupButton.onChange, PowerupSelected);
		_powerupPopupButton.SetItems(Enum.GetNames(typeof(PowerupType)).ToList());
		EventDelegate.Add(_charPopupButton.onChange, CharSelected);
		_charPopupButton.SetItems(Enum.GetNames(typeof(Characters.CharacterType)).ToList());
		EventDelegate.Add(_boardPopupButton.onChange, BoardSelected);
		_boardPopupButton.SetItems(Enum.GetNames(typeof(Hoverboards.BoardType)).ToList());
		EventDelegate.Add(_trophyPopupButton.onChange, OldTrophySelected);
		_trophyPopupButton.SetItems(Enum.GetNames(typeof(OldTrophies.Trophy)).ToList());
		EventDelegate.Add(_awardPopupButton.onChange, AwardSelected);
		_awardPopupButton.SetItems(Enum.GetNames(typeof(AwardTierType)).ToList());
		EventDelegate.Add(_amountPopupButton.onChange, NewAmountSelected);
		EventDelegate.Add(_topRunRankPopupButton.onChange, NewRankSelected);
		buttonLabel.text = ((!Globals.DEBUG_USE_CELEBRATION_TESTER) ? "Enable" : "Disable");
	}

	public void OriginSelected()
	{
		string value = _originPopupButton.value;
		_currentCelebrationReward.CelebrationRewardOrigin = (CelebrationRewardOrigin)(int)Enum.Parse(typeof(CelebrationRewardOrigin), value, true);
		powerupButtons.SetActive(true);
		boardsButton.SetActive(false);
		charButton.SetActive(true);
		trrankButton.SetActive(false);
		awardButton.SetActive(false);
		switch (_currentCelebrationReward.CelebrationRewardOrigin)
		{
		case CelebrationRewardOrigin.BoardUnlock:
			boardsButton.SetActive(true);
			powerupButtons.SetActive(false);
			charButton.SetActive(false);
			break;
		case CelebrationRewardOrigin.CharacterUnlock:
			charButton.SetActive(true);
			powerupButtons.SetActive(false);
			break;
		case CelebrationRewardOrigin.TopRun:
			trrankButton.SetActive(true);
			powerupButtons.SetActive(false);
			charButton.SetActive(false);
			break;
		case CelebrationRewardOrigin.Award:
			awardButton.SetActive(true);
			powerupButtons.SetActive(false);
			charButton.SetActive(false);
			break;
		case CelebrationRewardOrigin.Notset:
			powerupButtons.SetActive(false);
			charButton.SetActive(false);
			break;
		case CelebrationRewardOrigin.NewHighScore:
			powerupButtons.SetActive(false);
			charButton.SetActive(false);
			break;
		case CelebrationRewardOrigin.MysteryBox:
		case CelebrationRewardOrigin.SuperMysteryBox:
		case CelebrationRewardOrigin.WeeklyChallengeNormal:
		case CelebrationRewardOrigin.WeeklyChallengeMysteryBox:
		case CelebrationRewardOrigin.WeeklyChallengeSuperMysteryBox:
		case CelebrationRewardOrigin.WeeklyChallengeHiddenPrize:
			break;
		}
	}

	public void TypeSelected()
	{
		string value = _typePopupButton.value;
		_currentCelebrationReward.rewardType = (CelebrationRewardType)(int)Enum.Parse(typeof(CelebrationRewardType), value, true);
	}

	public void PowerupSelected()
	{
		string value = _powerupPopupButton.value;
		_currentCelebrationReward.powerupType = (PowerupType)(int)Enum.Parse(typeof(PowerupType), value, true);
	}

	public void CharSelected()
	{
		string value = _charPopupButton.value;
		_currentCelebrationReward.characterType = (Characters.CharacterType)(int)Enum.Parse(typeof(Characters.CharacterType), value, true);
	}

	public void BoardSelected()
	{
		string value = _boardPopupButton.value;
		_currentCelebrationReward.boardType = (Hoverboards.BoardType)(int)Enum.Parse(typeof(Hoverboards.BoardType), value, true);
	}

	public void OldTrophySelected()
	{
		string value = _trophyPopupButton.value;
		_currentCelebrationReward.trophyType = (OldTrophies.Trophy)(int)Enum.Parse(typeof(OldTrophies.Trophy), value, true);
	}

	public void AwardSelected()
	{
		string value = _awardPopupButton.value;
		_currentCelebrationReward.awardTierType = (AwardTierType)(int)Enum.Parse(typeof(AwardTierType), value, true);
	}

	public void NewAmountSelected()
	{
		string value = _amountPopupButton.value;
		_currentCelebrationReward.amount = int.Parse(value);
	}

	public void NewRankSelected()
	{
		string value = _topRunRankPopupButton.value;
		_currentCelebrationReward.rank = int.Parse(value);
		_currentCelebrationReward.score = 123543510;
		_currentCelebrationReward.weekNumber = 12;
	}

	public void ToggleDebugCelebration()
	{
		Globals.DEBUG_USE_CELEBRATION_TESTER = !Globals.DEBUG_USE_CELEBRATION_TESTER;
		buttonLabel.text = ((!Globals.DEBUG_USE_CELEBRATION_TESTER) ? "Enable" : "Disable");
	}

	public static CelebrationReward GetDebugCelebrationReward()
	{
		return _currentCelebrationReward;
	}

	public override void Init()
	{
		base.Init();
	}

	public override void Show()
	{
		base.Show();
	}

	public override void Hide()
	{
		base.Hide();
	}
}
