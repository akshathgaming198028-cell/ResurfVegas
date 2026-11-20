using System;
using System.Collections.Generic;
using SYBO.Subway;
using UnityEngine;

public class GameStats
{
	public delegate void CoinsChangedIngame();

	private const int SCORE_FOR_PICKUPS = 30;

	private const int AMOUNT_BY_WHICH_EXTRA_SCORE_IS_CONSUMED = 4;

	public bool bragNotifyDone;

	public bool bragFacebookDone;

	public float duration;

	public Action OnCoinsChanged;

	public Action OnCoinsIncreased;

	public Action OnScoreChanged;

	public Action OnJumpsChanged;

	public Action OnRollsChanged;

	private int _xoredNumberOfCoins;

	private int _coinsWithPogoStick;

	private int _coinsInAir;

	private int _coinCollectedOnLeftTrack;

	private int _coinsCollectedOnCenterTrack;

	private int _coinsCollectedOnRightTrack;

	private int _coinsNotTouchingGround;

	private int _coinsCoinMagnet;

	private bool _scoreBoosterActivated;

	private int _scoreBoosterTier;

	private bool _characterDayEventActivated;

	private int _score;

	private float _metersLastUsedForScore;

	private float _meterScore;

	private int _amountLeftToConsume;

	private bool pausePowerups;

	private List<ActivePowerup> _listOfActivePowerups = new List<ActivePowerup>();

	private Dictionary<PowerupType, bool> _missionEachPowerupPickupStatus = new Dictionary<PowerupType, bool>();

	private float _metersInCurrentGame;

	public float metersRunLeftTrack;

	public float metersRunCenterTrack;

	public float metersRunRightTrack;

	public float metersFly;

	public float metersRunGround;

	public float metersRunTrain;

	public float metersRunStation;

	private int _grindedTrains;

	private int _jumps;

	private int _allCoinsInJetpack;

	private int _jumpsOverTrains;

	private int _rolls;

	private int _rollsLeftTrack;

	private int _rollsCenterTrack;

	private int _rollsRightTrack;

	public int trackChanges;

	private int _dodgeBarrier;

	private int _jumpBarrier;

	private int _jumpHighBarrier;

	private int _trainHit;

	private int _movingTrainHit;

	private int _guardHitScreen;

	private int _barrierHit;

	private int _jetpackPickups;

	private int _superSneakerPickups;

	private int _powerJumperPickups;

	private int _letterPickups;

	private int _coinMagnetsPickups;

	private int _mysteryBoxPickups;

	private int _superMysteryBoxPickups;

	private int _doubleMultiplierPickups;

	private int _pickedUpPowerups;

	private int _saveMeTokenPickup;

	private int _friendsDueled;

	private int _duelWins;

	private int _duelLosses;

	private int _duelDraws;

	public CoinsChangedIngame OnChoinsChangedIngame;

	public List<KeyValuePair<int, int>> coinsSummerized = new List<KeyValuePair<int, int>>();

	private static GameStats instance;

	public int coins
	{
		get
		{
			return Utils.EncryptDecryptXORValue(_xoredNumberOfCoins);
		}
		set
		{
			_xoredNumberOfCoins = Utils.EncryptDecryptXORValue(value);
			Action onCoinsChanged = OnCoinsChanged;
			if (onCoinsChanged != null)
			{
				onCoinsChanged();
			}
			if (value != 0)
			{
				Action onCoinsIncreased = OnCoinsIncreased;
				if (onCoinsIncreased != null)
				{
					onCoinsIncreased();
				}
				MissionsManager.Instance.PlayerDidThisCoinMission(MissionTarget.EarnCoin);
				MissionsManager.Instance.RemoveProgressForThis(MissionTarget.NoCoinsWithoutScore);
			}
		}
	}

	public int coinsWithJetpack
	{
		get
		{
			return _allCoinsInJetpack;
		}
		set
		{
			_allCoinsInJetpack = value;
			if (value != 0)
			{
				MissionsManager.Instance.PlayerDidThisCoinMission(MissionTarget.CoinsWithJetpack);
			}
		}
	}

	public int coinsWithPogoStick
	{
		get
		{
			return _coinsWithPogoStick;
		}
		set
		{
			_coinsWithPogoStick = value;
			if (value != 0)
			{
				MissionsManager.Instance.PlayerDidThisCoinMission(MissionTarget.CollectCoinsWithPowerJumper);
			}
		}
	}

	public int coinsInAir
	{
		get
		{
			return _coinsInAir;
		}
		set
		{
			_coinsInAir = value;
			if (value != 0)
			{
				MissionsManager.Instance.PlayerDidThisCoinMission(MissionTarget.EarnCoinWithoutTouchingGround);
			}
		}
	}

	public int coinsCollectedOnLeftTrack
	{
		get
		{
			return _coinCollectedOnLeftTrack;
		}
		set
		{
			_coinCollectedOnLeftTrack = value;
			if (value != 0)
			{
				MissionsManager.Instance.PlayerDidThisCoinMission(MissionTarget.CollectCoinsLeftLane);
			}
		}
	}

	public int coinsCollectedOnCenterTrack
	{
		get
		{
			return _coinsCollectedOnCenterTrack;
		}
		set
		{
			_coinsCollectedOnCenterTrack = value;
			if (value != 0)
			{
				MissionsManager.Instance.PlayerDidThisCoinMission(MissionTarget.CollectCoinsCenterLane);
			}
		}
	}

	public int coinsCollectedOnRightTrack
	{
		get
		{
			return _coinsCollectedOnRightTrack;
		}
		set
		{
			_coinsCollectedOnRightTrack = value;
			if (value != 0)
			{
				MissionsManager.Instance.PlayerDidThisCoinMission(MissionTarget.CollectCoinsRightLane);
			}
		}
	}

	public int coinsNotTouchingGround
	{
		get
		{
			return _coinsNotTouchingGround;
		}
		set
		{
			_coinsNotTouchingGround = value;
		}
	}

	public int coinsCoinMagnet
	{
		get
		{
			return _coinsCoinMagnet;
		}
		set
		{
			_coinsCoinMagnet = value;
			if (value != 0)
			{
				MissionsManager.Instance.PlayerDidThisCoinMission(MissionTarget.CoinsWithMagnet);
			}
		}
	}

	public bool scoreBoosterActivated
	{
		get
		{
			return _scoreBoosterActivated;
		}
		set
		{
			_scoreBoosterActivated = value;
			PlayerInfo.Instance.TriggerOnScoreMultiplierChanged();
		}
	}

	public int scoreBoosterTier
	{
		get
		{
			return _scoreBoosterTier;
		}
		set
		{
			_scoreBoosterTier = value;
			PlayerInfo.Instance.TriggerOnScoreMultiplierChanged();
		}
	}

	public bool characterDayEventActivated
	{
		get
		{
			return _characterDayEventActivated;
		}
		set
		{
			_characterDayEventActivated = value;
			PlayerInfo.Instance.TriggerOnScoreMultiplierChanged();
		}
	}

	public bool PAUSEPOWERUPS
	{
		get
		{
			return pausePowerups;
		}
		set
		{
			pausePowerups = value;
		}
	}

	public int score
	{
		get
		{
			return _score;
		}
	}

	public Dictionary<PowerupType, bool> MissionEachPowerupPickupStatus
	{
		get
		{
			return _missionEachPowerupPickupStatus;
		}
	}

	public float MetersInCurrentGame
	{
		get
		{
			return _metersInCurrentGame;
		}
		set
		{
			_metersInCurrentGame = value;
		}
	}

	public int grindedTrains
	{
		get
		{
			return _grindedTrains;
		}
		set
		{
			_grindedTrains = value;
		}
	}

	public int jumps
	{
		get
		{
			return _jumps;
		}
		set
		{
			_jumps = value;
			Action onJumpsChanged = OnJumpsChanged;
			if (onJumpsChanged != null)
			{
				onJumpsChanged();
			}
			if (value != 0)
			{
				MissionsManager.Instance.PlayerDidThis(MissionTarget.Jump);
				MissionsManager.Instance.RemoveProgressForThis(MissionTarget.NoJumpsWithoutScore);
			}
		}
	}

	public int allCoinsInJetpack
	{
		get
		{
			return _allCoinsInJetpack;
		}
		set
		{
			_allCoinsInJetpack = value;
		}
	}

	public int jumpsOverTrains
	{
		get
		{
			return _jumpsOverTrains;
		}
		set
		{
			_jumpsOverTrains = value;
			if (value != 0)
			{
				MissionsManager.Instance.PlayerDidThis(MissionTarget.JumpTrain);
			}
		}
	}

	public int rolls
	{
		get
		{
			return _rolls;
		}
		set
		{
			_rolls = value;
			Action onRollsChanged = OnRollsChanged;
			if (onRollsChanged != null)
			{
				onRollsChanged();
			}
			if (value != 0)
			{
				MissionsManager.Instance.PlayerDidThis(MissionTarget.Roll);
				MissionsManager.Instance.RemoveProgressForThis(MissionTarget.NoRollsWithoutScore);
			}
		}
	}

	public int rollsLeftTrack
	{
		get
		{
			return _rollsLeftTrack;
		}
		set
		{
			_rollsLeftTrack = value;
			if (value != 0)
			{
				MissionsManager.Instance.PlayerDidThis(MissionTarget.RollLeft);
			}
		}
	}

	public int rollsCenterTrack
	{
		get
		{
			return _rollsCenterTrack;
		}
		set
		{
			_rollsCenterTrack = value;
			if (value != 0)
			{
				MissionsManager.Instance.PlayerDidThis(MissionTarget.RollCenter);
			}
		}
	}

	public int rollsRightTrack
	{
		get
		{
			return _rollsRightTrack;
		}
		set
		{
			_rollsRightTrack = value;
			if (value != 0)
			{
				MissionsManager.Instance.PlayerDidThis(MissionTarget.RollRight);
			}
		}
	}

	public int dodgeBarrier
	{
		get
		{
			return _dodgeBarrier;
		}
		set
		{
			_dodgeBarrier = value;
			if (value != 0 && !Game.Instance.isDead && !Game.Instance.isInStumbleDeathSequence)
			{
				MissionsManager.Instance.PlayerDidThis(MissionTarget.RollUnderBarriers);
				MissionsManager.Instance.PlayerDidThis(MissionTarget.DodgeBarriers);
			}
		}
	}

	public int jumpBarrier
	{
		get
		{
			return _jumpBarrier;
		}
		set
		{
			_jumpBarrier = value;
			if (value != 0 && !Game.Instance.isDead && !Game.Instance.isInStumbleDeathSequence)
			{
				MissionsManager.Instance.PlayerDidThis(MissionTarget.JumpBarriers);
				MissionsManager.Instance.PlayerDidThis(MissionTarget.DodgeBarriers);
			}
		}
	}

	public int jumpHighBarrier
	{
		get
		{
			return _jumpHighBarrier;
		}
		set
		{
			_jumpHighBarrier = value;
		}
	}

	public int trainHit
	{
		get
		{
			return _trainHit;
		}
		set
		{
			_trainHit = value;
			if (value != 0)
			{
				MissionsManager.Instance.PlayerDidThis(MissionTarget.CrashTrains);
			}
		}
	}

	public int movingTrainHit
	{
		get
		{
			return _movingTrainHit;
		}
		set
		{
			_movingTrainHit = value;
			if (value != 0)
			{
				MissionsManager.Instance.PlayerDidThis(MissionTarget.DieToTrain);
			}
			if (value != 0)
			{
				MissionsManager.Instance.PlayerDidThis(MissionTarget.CrashTrains);
			}
		}
	}

	public int guardHitScreen
	{
		get
		{
			return _guardHitScreen;
		}
		set
		{
			_guardHitScreen = value;
		}
	}

	public int barrierHit
	{
		get
		{
			return _barrierHit;
		}
		set
		{
			_barrierHit = value;
			if (value != 0)
			{
				MissionsManager.Instance.PlayerDidThis(MissionTarget.CrashBarriers);
			}
		}
	}

	public int jetpackPickups
	{
		get
		{
			return _jetpackPickups;
		}
		set
		{
			_jetpackPickups = value;
			if (value == 0)
			{
				if (_missionEachPowerupPickupStatus.ContainsKey(PowerupType.jetpack))
				{
					_missionEachPowerupPickupStatus[PowerupType.jetpack] = false;
				}
				else
				{
					_missionEachPowerupPickupStatus.Add(PowerupType.jetpack, true);
				}
			}
			else
			{
				ReportOneOfEachPowerupIfApplicable(PowerupType.jetpack);
				MissionsManager.Instance.PlayerDidThis(MissionTarget.Jetpack);
			}
		}
	}

	public int superSneakerPickups
	{
		get
		{
			return _superSneakerPickups;
		}
		set
		{
			_superSneakerPickups = value;
			if (value == 0)
			{
				if (_missionEachPowerupPickupStatus.ContainsKey(PowerupType.supersneakers))
				{
					_missionEachPowerupPickupStatus[PowerupType.supersneakers] = false;
				}
				else
				{
					_missionEachPowerupPickupStatus.Add(PowerupType.supersneakers, true);
				}
			}
			else
			{
				ReportOneOfEachPowerupIfApplicable(PowerupType.supersneakers);
				MissionsManager.Instance.PlayerDidThis(MissionTarget.SuperSneakers);
			}
		}
	}

	public int powerJumperPickups
	{
		get
		{
			return _powerJumperPickups;
		}
		set
		{
			_powerJumperPickups = value;
			if (value == 0)
			{
				if (_missionEachPowerupPickupStatus.ContainsKey(PowerupType.pogostick))
				{
					_missionEachPowerupPickupStatus[PowerupType.pogostick] = false;
				}
				else
				{
					_missionEachPowerupPickupStatus.Add(PowerupType.pogostick, true);
				}
			}
			else
			{
				MissionsManager.Instance.PlayerDidThis(MissionTarget.PickUpPowerJumpers);
				MissionsManager.Instance.PlayerDidThis(MissionTarget.PickUpPowerJumpersSingleRun);
				ReportOneOfEachPowerupIfApplicable(PowerupType.pogostick);
			}
		}
	}

	public int letterPickups
	{
		get
		{
			return _letterPickups;
		}
		set
		{
			_letterPickups = value;
			if (value != 0)
			{
				MissionsManager.Instance.PlayerDidThis(MissionTarget.Letters);
			}
		}
	}

	public int coinMagnetsPickups
	{
		get
		{
			return _coinMagnetsPickups;
		}
		set
		{
			_coinMagnetsPickups = value;
			if (value == 0)
			{
				if (_missionEachPowerupPickupStatus.ContainsKey(PowerupType.coinmagnet))
				{
					_missionEachPowerupPickupStatus[PowerupType.coinmagnet] = false;
				}
				else
				{
					_missionEachPowerupPickupStatus.Add(PowerupType.coinmagnet, true);
				}
			}
			else
			{
				ReportOneOfEachPowerupIfApplicable(PowerupType.coinmagnet);
				MissionsManager.Instance.PlayerDidThis(MissionTarget.Magnets);
			}
		}
	}

	public int mysteryBoxPickups
	{
		get
		{
			return _mysteryBoxPickups;
		}
		set
		{
			_mysteryBoxPickups = value;
			if (value != 0)
			{
				MissionsManager.Instance.PlayerDidThis(MissionTarget.MysteryBoxes);
			}
		}
	}

	public int superMysteryBoxPickups
	{
		get
		{
			return _superMysteryBoxPickups;
		}
		set
		{
			_superMysteryBoxPickups = value;
		}
	}

	public int doubleMultiplierPickups
	{
		get
		{
			return _doubleMultiplierPickups;
		}
		set
		{
			_doubleMultiplierPickups = value;
			if (value == 0)
			{
				if (_missionEachPowerupPickupStatus.ContainsKey(PowerupType.doubleMultiplier))
				{
					_missionEachPowerupPickupStatus[PowerupType.doubleMultiplier] = false;
				}
				else
				{
					_missionEachPowerupPickupStatus.Add(PowerupType.doubleMultiplier, true);
				}
			}
			else
			{
				ReportOneOfEachPowerupIfApplicable(PowerupType.doubleMultiplier);
				MissionsManager.Instance.PlayerDidThis(MissionTarget.DoubleMultiplier);
			}
		}
	}

	public int pickedUpPowerups
	{
		get
		{
			return _pickedUpPowerups;
		}
		set
		{
			_pickedUpPowerups = value;
			if (value != 0)
			{
				Statistics stats;
				Statistics statistics = (stats = PlayerInfo.Instance.stats);
				Stat stat2;
				Stat stat = (stat2 = Stat.PickupPowerup);
				int num = stats[stat2];
				statistics[stat] = num + 1;
				MissionsManager.Instance.PlayerDidThis(MissionTarget.Powerups);
				MissionsManager.Instance.RemoveProgressForThis(MissionTarget.NoPowerUpsWithoutScore);
			}
		}
	}

	public int saveMeTokenPickup
	{
		get
		{
			return _saveMeTokenPickup;
		}
		set
		{
			_saveMeTokenPickup = value;
			if (value != 0)
			{
				MissionsManager.Instance.PlayerDidThis(MissionTarget.PickupKeys);
			}
		}
	}

	public int friendsDueled
	{
		get
		{
			return _friendsDueled;
		}
		set
		{
			_friendsDueled = value;
			if (value != 0)
			{
				MissionsManager.Instance.PlayerDidThis(MissionTarget.DuelFriends);
			}
		}
	}

	public int duelWins
	{
		get
		{
			return _duelWins;
		}
		set
		{
			_duelWins = value;
			if (value != 0)
			{
				MissionsManager.Instance.PlayerDidThis(MissionTarget.WinDuels);
				MissionsManager.Instance.PlayerDidThis(MissionTarget.WinDuelsInARow);
			}
		}
	}

	public int duelLosses
	{
		get
		{
			return _duelLosses;
		}
		set
		{
			_duelLosses = value;
			if (value != 0)
			{
				MissionsManager.Instance.RemoveProgressForThis(MissionTarget.WinDuelsInARow);
			}
		}
	}

	public int duelDraws
	{
		get
		{
			return _duelDraws;
		}
		set
		{
			_duelDraws = value;
			if (value != 0)
			{
				MissionsManager.Instance.RemoveProgressForThis(MissionTarget.WinDuelsInARow);
			}
		}
	}

	public static GameStats Instance
	{
		get
		{
			return instance ?? (instance = new GameStats());
		}
	}

	public static int CoinToScoreConversion(int coins)
	{
		return coins * 2 * PlayerInfo.Instance.rawMultiplier;
	}

	public void CalculateScore()
	{
		if (_metersLastUsedForScore < MetersInCurrentGame)
		{
			_meterScore = MetersInCurrentGame - _metersLastUsedForScore;
			_metersLastUsedForScore = MetersInCurrentGame;
			int num = (int)(_meterScore * (float)PlayerInfo.Instance.scoreMultiplier);
			num += ConsumeAmountNeededToAdjustScoreBy(4) * PlayerInfo.Instance.scoreMultiplier;
			_score += num;
			MissionsManager.Instance.PlayerDidThis(MissionTarget.Score, num);
			MissionsManager.Instance.PlayerDidThis(MissionTarget.NoCoinsWithoutScore, num);
			MissionsManager.Instance.PlayerDidThis(MissionTarget.NoRollsWithoutScore, num);
			MissionsManager.Instance.PlayerDidThis(MissionTarget.NoJumpsWithoutScore, num);
			if (_listOfActivePowerups.Count == 0 || (_listOfActivePowerups.Count == 1 && _listOfActivePowerups[0].type == PowerupType.hoverboard))
			{
				MissionsManager.Instance.PlayerDidThis(MissionTarget.NoPowerUpsWithoutScore, num);
			}
			if (scoreBoosterActivated)
			{
				MissionsManager.Instance.PlayerDidThis(MissionTarget.ScoreWithScorebooster, num);
			}
			Action onScoreChanged = OnScoreChanged;
			if (onScoreChanged != null)
			{
				onScoreChanged();
			}
		}
	}

	public void ResetAmountNeededToAdjustScoreBy()
	{
		_amountLeftToConsume = 0;
	}

	public void IncreaseAmountNeededToAdjustScoreBy(int amount)
	{
		_amountLeftToConsume += amount;
	}

	private int ConsumeAmountNeededToAdjustScoreBy(int amount)
	{
		if (amount > 0)
		{
			if (amount > _amountLeftToConsume)
			{
				amount = _amountLeftToConsume;
			}
			if (_amountLeftToConsume > 0)
			{
				_amountLeftToConsume -= amount;
				return amount;
			}
		}
		return 0;
	}

	public void AddScoreForPickup(PowerupType type)
	{
		switch (type)
		{
		case PowerupType.jetpack:
		case PowerupType.supersneakers:
		case PowerupType.coinmagnet:
		case PowerupType.doubleMultiplier:
			_AddScoreForPickup(true);
			break;
		case PowerupType.mysterybox:
		case PowerupType.letters:
			_AddScoreForPickup(false);
			break;
		case PowerupType.supermysterybox:
			break;
		}
	}

	private void _AddScoreForPickup(bool wasPowerup)
	{
		int num = PlayerInfo.Instance.scoreMultiplier * 30;
		_score += num;
		MissionsManager.Instance.PlayerDidThis(MissionTarget.Score, num);
		MissionsManager.Instance.PlayerDidThis(MissionTarget.NoCoinsWithoutScore, num);
		MissionsManager.Instance.PlayerDidThis(MissionTarget.NoRollsWithoutScore, num);
		MissionsManager.Instance.PlayerDidThis(MissionTarget.NoJumpsWithoutScore, num);
		if (!wasPowerup)
		{
			MissionsManager.Instance.PlayerDidThis(MissionTarget.NoPowerUpsWithoutScore, num);
		}
	}

	public void ResetScore()
	{
		_score = 0;
		_metersLastUsedForScore = 0f;
		_meterScore = 0f;
	}

	public void Reset()
	{
		bragNotifyDone = false;
		bragFacebookDone = false;
		duration = 0f;
		ResetScore();
		ResetAmountNeededToAdjustScoreBy();
		coins = 0;
		coinsCoinMagnet = 0;
		coinsWithJetpack = 0;
		allCoinsInJetpack = 0;
		scoreBoosterActivated = false;
		scoreBoosterTier = 0;
		MetersInCurrentGame = 0f;
		metersRunLeftTrack = 0f;
		metersRunCenterTrack = 0f;
		metersRunRightTrack = 0f;
		metersRunGround = 0f;
		metersRunTrain = 0f;
		metersRunStation = 0f;
		metersFly = 0f;
		grindedTrains = 0;
		jumps = 0;
		jumpsOverTrains = 0;
		rolls = 0;
		rollsLeftTrack = 0;
		rollsCenterTrack = 0;
		rollsRightTrack = 0;
		trackChanges = 0;
		dodgeBarrier = 0;
		jumpBarrier = 0;
		jumpHighBarrier = 0;
		trainHit = 0;
		guardHitScreen = 0;
		barrierHit = 0;
		jetpackPickups = 0;
		superSneakerPickups = 0;
		powerJumperPickups = 0;
		letterPickups = 0;
		coinMagnetsPickups = 0;
		mysteryBoxPickups = 0;
		pickedUpPowerups = 0;
		doubleMultiplierPickups = 0;
		coinsSummerized = new List<KeyValuePair<int, int>>();
	}

	public ActivePowerup TriggerPowerup(PowerupType type)
	{
		ActivePowerup activePowerup = new ActivePowerup();
		activePowerup.type = type;
		activePowerup.timeActivated = Time.time;
		activePowerup.timeLeft = PlayerInfo.Instance.GetPowerupDuration(type);
		if (type == PowerupType.headstart2000 || type == PowerupType.headstart500 || type == PowerupType.headstartLong || type == PowerupType.headstartShort || type == PowerupType.turboHeadstart1 || type == PowerupType.turboHeadstart2 || type == PowerupType.headstart)
		{
			activePowerup.timeLeft = 0f;
		}
		for (int num = _listOfActivePowerups.Count - 1; num >= 0; num--)
		{
			if (_listOfActivePowerups[num].type == activePowerup.type)
			{
				_listOfActivePowerups.RemoveAt(num);
			}
		}
		AddScoreForPickup(type);
		_listOfActivePowerups.Add(activePowerup);
		MissionsManager.Instance.RemoveProgressForThis(MissionTarget.ActivePowerups);
		int num2 = _listOfActivePowerups.Count;
		for (int i = 0; i < _listOfActivePowerups.Count; i++)
		{
			if (_listOfActivePowerups[i].type == PowerupType.hoverboard)
			{
				num2--;
				break;
			}
		}
		MissionsManager.Instance.PlayerDidThis(MissionTarget.ActivePowerups, num2);
		return activePowerup;
	}

	public List<ActivePowerup> GetActivePowerups()
	{
		return _listOfActivePowerups;
	}

	public void UpdatePowerupTimes(float deltaTime)
	{
		if (pausePowerups)
		{
			return;
		}
		for (int num = _listOfActivePowerups.Count - 1; num >= 0; num--)
		{
			if (((Game.Instance.IsInJetpackMode || Game.Instance.IsInPogostickMode) && (_listOfActivePowerups[num].type == PowerupType.hoverboard || _listOfActivePowerups[num].type == PowerupType.supersneakers || _listOfActivePowerups[num].type == PowerupType.doubleJump)) || (_listOfActivePowerups[num].type == PowerupType.supersneakers && Game.Instance.Modifiers.Hoverboard.isActive && Hoverboard.Instance.IsJumper))
			{
				continue;
			}
			_listOfActivePowerups[num].timeLeft -= deltaTime;
			if (!(_listOfActivePowerups[num].timeLeft < 0f) || (Game.Instance.IsInJetpackMode && _listOfActivePowerups[num].type == PowerupType.jetpack))
			{
				continue;
			}
			if (_listOfActivePowerups[num].type == PowerupType.hoverboard)
			{
				float num2 = Hoverboard.Instance.WaitForParticlesDelay + PlayerInfo.Instance.GetHoverBoardCoolDown();
				if (_listOfActivePowerups[num].timeLeft > 0f - num2)
				{
					continue;
				}
				Hoverboard.Instance.HardReset();
			}
			_listOfActivePowerups.RemoveAt(num);
			MissionsManager.Instance.RemoveProgressForThis(MissionTarget.ActivePowerups);
			int num3 = _listOfActivePowerups.Count;
			for (int i = 0; i < _listOfActivePowerups.Count; i++)
			{
				if (_listOfActivePowerups[i].type == PowerupType.hoverboard)
				{
					num3--;
					break;
				}
			}
			MissionsManager.Instance.PlayerDidThis(MissionTarget.ActivePowerups, num3);
		}
	}

	public void ClearPowerups()
	{
		_listOfActivePowerups.Clear();
		MissionsManager.Instance.RemoveProgressForThis(MissionTarget.ActivePowerups);
	}

	public void RemoveHoverBoardPowerup()
	{
		for (int num = _listOfActivePowerups.Count - 1; num >= 0; num--)
		{
			if (_listOfActivePowerups[num].type == PowerupType.hoverboard)
			{
				_listOfActivePowerups[num].timeLeft = 0f;
			}
		}
	}

	public void RemoveSwapCharacter()
	{
		for (int num = _listOfActivePowerups.Count - 1; num >= 0; num--)
		{
			if (_listOfActivePowerups[num].type == PowerupType.swapCharacter)
			{
				_listOfActivePowerups[num].timeLeft = 0f;
			}
		}
	}

	private void ReportOneOfEachPowerupIfApplicable(PowerupType powerupType)
	{
		if (MissionsManager.Instance.IsMissionTargetActive(MissionTarget.OneOfEachPowerup) && _missionEachPowerupPickupStatus.ContainsKey(powerupType) && !_missionEachPowerupPickupStatus[powerupType])
		{
			_missionEachPowerupPickupStatus[powerupType] = true;
			MissionsManager.Instance.PlayerDidThis(MissionTarget.OneOfEachPowerup);
		}
	}
}
