using System;
using System.Collections.Generic;
using SYBO.Subway.Info;
using UnityEngine;

public static class MysteryBox
{
	public enum Type
	{
		Normal = 0,
		Super = 1,
		Mini = 2
	}

	private class LootTableEntry
	{
		public CelebrationRewardType itemType;

		public float weight;

		public int min = 1;

		public int max = 1;

		public PowerupType powerupType;
	}

	public const int DEFAULT_MEGA_JACKPOT_MYSTERYBOX = 900000;

	public const int DEFAULT_MEGA_JACKPOT_SUPER_MYSTERYBOX = 1500000;

	public static readonly Characters.CharacterType[] tokenRewardTypes;

	public static readonly Dictionary<Characters.CharacterType, int> characterTokenWeight;

	private static readonly LootTableEntry[][] _lootTables;

	public static readonly int mysteryBoxJackpotAmount;

	public static readonly int superMysteryBoxJackpotAmount;

	private static readonly CelebrationReward MYSTERYBOX_CONSOLATION_REWARD;

	private static readonly CelebrationReward SUPERMYSTERYBOX_CONSOLATION_REWARD;

	static MysteryBox()
	{
		tokenRewardTypes = new Characters.CharacterType[4]
		{
			Characters.CharacterType.tricky,
			Characters.CharacterType.fresh,
			Characters.CharacterType.spike,
			Characters.CharacterType.yutani
		};
		characterTokenWeight = new Dictionary<Characters.CharacterType, int>
		{
			{
				Characters.CharacterType.tricky,
				50
			},
			{
				Characters.CharacterType.fresh,
				30
			},
			{
				Characters.CharacterType.spike,
				15
			},
			{
				Characters.CharacterType.yutani,
				5
			}
		};
		mysteryBoxJackpotAmount = 100000;
		superMysteryBoxJackpotAmount = 300000;
		MYSTERYBOX_CONSOLATION_REWARD = new CelebrationReward
		{
			rewardType = CelebrationRewardType.coins,
			amount = 500
		};
		SUPERMYSTERYBOX_CONSOLATION_REWARD = new CelebrationReward
		{
			rewardType = CelebrationRewardType.coins,
			amount = 1500
		};
		Type[] array = (Type[])Enum.GetValues(typeof(Type));
		_lootTables = new LootTableEntry[array.Length][];
		_lootTables[0] = new LootTableEntry[18]
		{
			new LootTableEntry
			{
				itemType = CelebrationRewardType.token,
				weight = 45f,
				min = 1,
				max = 1
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.trophy,
				weight = 1f,
				min = 1,
				max = 1
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 5.5f,
				min = 1,
				max = 1,
				powerupType = PowerupType.hoverboard
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 12f,
				min = 200,
				max = 300
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 10.36f,
				min = 500,
				max = 500
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 9f,
				min = 2,
				max = 2,
				powerupType = PowerupType.hoverboard
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 5f,
				min = 1000,
				max = 1000
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 2.5f,
				min = 1500,
				max = 1500
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 2f,
				min = 3,
				max = 9,
				powerupType = PowerupType.hoverboard
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.keys,
				weight = 1f,
				min = 1,
				max = 1
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 2.7f,
				min = 1,
				max = 1,
				powerupType = PowerupType.headstart2000
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 0.8f,
				min = 2500,
				max = 2500
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 2f,
				min = 1,
				max = 1,
				powerupType = PowerupType.scorebooster
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 0.5f,
				min = 10,
				max = 10,
				powerupType = PowerupType.hoverboard
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 0.3f,
				min = 5000,
				max = 5000
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 0.2f,
				min = 2,
				max = 2,
				powerupType = PowerupType.headstart2000
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 0.1f,
				min = 3,
				max = 3,
				powerupType = PowerupType.headstart2000
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 0.04f,
				min = 100000,
				max = 100000
			}
		};
		_lootTables[1] = new LootTableEntry[24]
		{
			new LootTableEntry
			{
				itemType = CelebrationRewardType.token,
				weight = 30f,
				min = 3,
				max = 3
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.trophy,
				weight = 6f,
				min = 1,
				max = 1
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 15.52f,
				min = 1500,
				max = 1500
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 8f,
				min = 4,
				max = 8,
				powerupType = PowerupType.hoverboard
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.keys,
				weight = 2.5f,
				min = 1,
				max = 1
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 11f,
				min = 2000,
				max = 2000
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 1.8f,
				min = 1,
				max = 1,
				powerupType = PowerupType.headstart2000
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 7f,
				min = 3000,
				max = 3000
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 3f,
				min = 1,
				max = 1,
				powerupType = PowerupType.scorebooster
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.keys,
				weight = 1.5f,
				min = 2,
				max = 2
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 1f,
				min = 2,
				max = 2,
				powerupType = PowerupType.headstart2000
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 0.7f,
				min = 10,
				max = 20,
				powerupType = PowerupType.hoverboard
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 4f,
				min = 5000,
				max = 5000
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.keys,
				weight = 1f,
				min = 3,
				max = 3
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 0.8f,
				min = 3,
				max = 3,
				powerupType = PowerupType.headstart2000
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 2f,
				min = 2,
				max = 2,
				powerupType = PowerupType.scorebooster
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 1.5f,
				min = 8000,
				max = 8000
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 1f,
				min = 3,
				max = 3,
				powerupType = PowerupType.scorebooster
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 0.4f,
				min = 5,
				max = 5,
				powerupType = PowerupType.headstart2000
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 0.3f,
				min = 40,
				max = 40,
				powerupType = PowerupType.hoverboard
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 0.4f,
				min = 15000,
				max = 15000
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 0.3f,
				min = 20000,
				max = 20000
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 0.2f,
				min = 50000,
				max = 50000
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 0.08f,
				min = 300000,
				max = 300000
			}
		};
		_lootTables[2] = new LootTableEntry[9]
		{
			new LootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 36f,
				min = 100,
				max = 100
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 25f,
				min = 150,
				max = 150
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 15f,
				min = 200,
				max = 200
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 14.5f,
				min = 1,
				max = 1,
				powerupType = PowerupType.hoverboard
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 5f,
				min = 2,
				max = 2,
				powerupType = PowerupType.hoverboard
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 2f,
				min = 3,
				max = 3,
				powerupType = PowerupType.hoverboard
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.keys,
				weight = 1.7f,
				min = 1,
				max = 1
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 0.5f,
				min = 1,
				max = 1,
				powerupType = PowerupType.scorebooster
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.keys,
				weight = 0.3f,
				min = 2,
				max = 2
			}
		};
	}

	public static void TestRolls(int numberOfRolls, Type mysteryBoxType = Type.Normal)
	{
		for (int i = 0; i < numberOfRolls; i++)
		{
			CelebrationReward celebrationReward = Roll(mysteryBoxType);
			Debug.Log("Type: " + celebrationReward.rewardType.ToString() + " Amount: " + celebrationReward.amount);
		}
	}

	public static CelebrationReward Roll(Type mysteryBoxType)
	{
		CelebrationReward celebrationReward = new CelebrationReward();
		PlayerInfo instance = PlayerInfo.Instance;
		switch (mysteryBoxType)
		{
		case Type.Normal:
			instance.amountOfMysteryBoxesOpened++;
			if (instance.amountOfMysteryBoxesOpened == 1)
			{
				celebrationReward.rewardType = CelebrationRewardType.coins;
				celebrationReward.amount = 1000;
				return celebrationReward;
			}
			if (instance.amountOfMysteryBoxesOpened == 2)
			{
				celebrationReward.rewardType = CelebrationRewardType.token;
				celebrationReward.amount = 1;
				Characters.CharacterType characterType = Characters.CharacterType.tricky;
				if (instance.IsCollectionComplete(characterType))
				{
					characterType = Characters.CharacterType.fresh;
				}
				celebrationReward.characterType = characterType;
				return celebrationReward;
			}
			if (instance.amountOfMysteryBoxesOpened == 3)
			{
				celebrationReward.rewardType = CelebrationRewardType.powerup;
				celebrationReward.amount = 3;
				celebrationReward.powerupType = PowerupType.hoverboard;
				return celebrationReward;
			}
			if (!instance.hasReceivedFirstMBKey)
			{
				celebrationReward.rewardType = CelebrationRewardType.keys;
				celebrationReward.amount = 1;
				instance.hasReceivedFirstMBKey = true;
				return celebrationReward;
			}
			break;
		case Type.Super:
			if (!instance.hasReceivedFirstSMBKey)
			{
				celebrationReward.rewardType = CelebrationRewardType.keys;
				celebrationReward.amount = 1;
				instance.hasReceivedFirstSMBKey = true;
				return celebrationReward;
			}
			instance.amountOfSuperMysteryBoxesOpened++;
			break;
		case Type.Mini:
			instance.amountOfMiniMysteryBoxesOpened++;
			if (instance.amountOfMiniMysteryBoxesOpened == 1)
			{
				celebrationReward.rewardType = CelebrationRewardType.powerup;
				celebrationReward.powerupType = PowerupType.scorebooster;
				celebrationReward.amount = 1;
				return celebrationReward;
			}
			if (instance.amountOfMiniMysteryBoxesOpened == 2)
			{
				celebrationReward.rewardType = CelebrationRewardType.keys;
				celebrationReward.amount = 1;
				return celebrationReward;
			}
			break;
		}
		List<OldTrophies.Trophy> availableTrophyList = GetAvailableTrophyList(mysteryBoxType);
		List<Characters.CharacterType> availableCharacterTokens = GetAvailableCharacterTokens();
		LootTableEntry lootTableEntry = null;
		lootTableEntry = MakeRoll(mysteryBoxType);
		while ((lootTableEntry.itemType == CelebrationRewardType.trophy && availableTrophyList.Count <= 0) || (lootTableEntry.itemType == CelebrationRewardType.token && availableCharacterTokens.Count <= 0))
		{
			lootTableEntry = MakeRoll(mysteryBoxType);
		}
		if (lootTableEntry == null)
		{
			Debug.LogError("A reward was empty, how did this happen?");
		}
		if (lootTableEntry.itemType == CelebrationRewardType.coins)
		{
			celebrationReward.rewardType = CelebrationRewardType.coins;
			celebrationReward.amount = UnityEngine.Random.Range(lootTableEntry.min, lootTableEntry.max);
			if (celebrationReward.amount >= 5000)
			{
				MissionsManager.Instance.PlayerDidThis(MissionTarget.MysteryBoxGrandPrize);
			}
			IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager(SSEventType.BiggerJackpot);
			if (((celebrationReward.amount == mysteryBoxJackpotAmount && mysteryBoxType == Type.Normal) || (celebrationReward.amount == superMysteryBoxJackpotAmount && mysteryBoxType == Type.Super)) && activeEventManager != null)
			{
				celebrationReward.amount = GetMegaJackpotAmount(mysteryBoxType);
			}
		}
		else if (lootTableEntry.itemType == CelebrationRewardType.keys)
		{
			celebrationReward.rewardType = CelebrationRewardType.keys;
			celebrationReward.amount = UnityEngine.Random.Range(lootTableEntry.min, lootTableEntry.max);
		}
		else if (lootTableEntry.itemType == CelebrationRewardType.powerup)
		{
			celebrationReward.rewardType = CelebrationRewardType.powerup;
			celebrationReward.powerupType = lootTableEntry.powerupType;
			celebrationReward.amount = UnityEngine.Random.Range(lootTableEntry.min, lootTableEntry.max);
		}
		else if (lootTableEntry.itemType == CelebrationRewardType.token)
		{
			if (availableCharacterTokens.Count > 0)
			{
				celebrationReward.rewardType = CelebrationRewardType.token;
				celebrationReward.amount = UnityEngine.Random.Range(lootTableEntry.min, lootTableEntry.max);
				celebrationReward.characterType = GetRandomCharacterTokenTypeUsingWeights(availableCharacterTokens);
			}
			else
			{
				Debug.Log("Mystery box consolation reward");
				celebrationReward = ((mysteryBoxType != Type.Normal) ? SUPERMYSTERYBOX_CONSOLATION_REWARD : MYSTERYBOX_CONSOLATION_REWARD);
			}
		}
		else if (lootTableEntry.itemType == CelebrationRewardType.trophy)
		{
			if (availableTrophyList.Count > 0)
			{
				celebrationReward.rewardType = CelebrationRewardType.trophy;
				celebrationReward.amount = UnityEngine.Random.Range(lootTableEntry.min, lootTableEntry.max);
				celebrationReward.trophyType = availableTrophyList[UnityEngine.Random.Range(0, availableTrophyList.Count)];
			}
			else
			{
				Debug.Log("Mystery box consolation reward because there are no trophies left");
				celebrationReward = ((mysteryBoxType != Type.Normal) ? SUPERMYSTERYBOX_CONSOLATION_REWARD : MYSTERYBOX_CONSOLATION_REWARD);
			}
		}
		else if (lootTableEntry.itemType == CelebrationRewardType.medal)
		{
			celebrationReward.rewardType = CelebrationRewardType.medal;
			celebrationReward.amount = 1;
		}
		return celebrationReward;
	}

	public static int GetMegaJackpotAmount(Type mysteryBoxType)
	{
		int num = 0;
		num = ((mysteryBoxType != Type.Normal) ? OnlineSettings.instance.GetValue("event_jackpot_supermysterybox_value", 1500000) : OnlineSettings.instance.GetValue("event_jackpot_mysterybox_value", 900000));
		if (num <= 0)
		{
			num = ((mysteryBoxType != Type.Normal) ? 1500000 : 900000);
		}
		return num;
	}

	private static LootTableEntry MakeRoll(Type mysteryBoxType)
	{
		float num = 0f;
		for (int i = 0; i < _lootTables[(int)mysteryBoxType].Length; i++)
		{
			LootTableEntry lootTableEntry = _lootTables[(int)mysteryBoxType][i];
			num += lootTableEntry.weight;
		}
		float num2 = UnityEngine.Random.Range(0f, 1f) * num;
		float num3 = 0f;
		LootTableEntry result = null;
		for (int j = 0; j < _lootTables[(int)mysteryBoxType].Length; j++)
		{
			LootTableEntry lootTableEntry2 = _lootTables[(int)mysteryBoxType][j];
			num3 += lootTableEntry2.weight;
			if (num2 <= num3)
			{
				result = lootTableEntry2;
				break;
			}
		}
		return result;
	}

	private static Characters.CharacterType GetRandomCharacterTokenTypeUsingWeights(List<Characters.CharacterType> validCharacters)
	{
		Characters.CharacterType result = Characters.CharacterType.tricky;
		int num = 0;
		foreach (Characters.CharacterType validCharacter in validCharacters)
		{
			num += characterTokenWeight[validCharacter];
		}
		int num2 = new System.Random().Next(0, num) + 1;
		int num3 = 0;
		for (int i = 0; i < validCharacters.Count; i++)
		{
			num3 += characterTokenWeight[validCharacters[i]];
			if (num2 <= num3)
			{
				result = validCharacters[i];
				break;
			}
		}
		return result;
	}

	private static List<OldTrophies.Trophy> GetAvailableTrophyList(Type mysteryBoxType)
	{
		OldTrophies.Trophy[] array = Enum.GetValues(typeof(OldTrophies.Trophy)) as OldTrophies.Trophy[];
		List<OldTrophies.Trophy> list = new List<OldTrophies.Trophy>();
		OldTrophies.Trophy[] array2 = array;
		foreach (OldTrophies.Trophy trophy in array2)
		{
			TrophyData value;
			if (PlayerInfo.Instance.shouldSpawnTrophy(trophy) && OldTrophies.trophyData.TryGetValue(trophy, out value) && value.obtainSource == mysteryBoxType)
			{
				list.Add(trophy);
			}
		}
		return list;
	}

	private static List<Characters.CharacterType> GetAvailableCharacterTokens()
	{
		List<Characters.CharacterType> list = new List<Characters.CharacterType>();
		Characters.CharacterType[] array = tokenRewardTypes;
		foreach (Characters.CharacterType characterType in array)
		{
			if (PlayerInfo.Instance.IsTokenUseful(characterType))
			{
				list.Add(characterType);
			}
		}
		return list;
	}
}
