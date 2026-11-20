using System;
using System.Collections.Generic;
using SYBO.Subway.Pickups;
using SYBO.Subway.Pickups.Migration;
using UnityEngine;

public class SpawnPointManager
{
	private class PickupType
	{
		public Func<SpawnPoint, GameObject> ExtractGameObject;

		public int spawnProbability;

		public float spawnDistanceMin;

		public float spawnZ;
	}

	private static SpawnPointManager instance;

	private PickupType characterLetterPickupType;

	private PickupType dailyLetter;

	private PickupType doubleScoreMultiplier;

	private PickupType jetpackPickup;

	private PickupType jumpBooster;

	private PickupType magnetBooster;

	private PickupType mysteryBox;

	private PickupType easterEgg;

	private PickupType saveMeToken;

	private PickupType mysteryPowerup;

	private List<PickupType> pickups;

	private float spawnZ;

	private float spawnSpacing;

	private System.Random randomGen = new System.Random();

	public static bool TEMP_WILL_SHOW_EXTRA_LOG_MESSAGES;

	private float totalProbability;

	private float[] accumulatedProbability;

	public static SpawnPointManager Instance
	{
		get
		{
			return instance ?? (instance = new SpawnPointManager());
		}
	}

	public SpawnPointManager()
	{
		float num = 8f;
		Upgrade upgrade = Upgrades.upgrades[PowerupType.characterLetter];
		characterLetterPickupType = new PickupType();
		characterLetterPickupType.spawnDistanceMin *= num;
		characterLetterPickupType.spawnProbability = upgrade.spawnProbability;
		characterLetterPickupType.ExtractGameObject = (SpawnPoint spawnPoint) => spawnPoint.characterLetter;
		Upgrade upgrade2 = Upgrades.upgrades[PowerupType.letters];
		dailyLetter = new PickupType();
		dailyLetter.spawnDistanceMin *= num;
		dailyLetter.spawnProbability = upgrade2.spawnProbability;
		dailyLetter.ExtractGameObject = (SpawnPoint spawnPoint) => spawnPoint.dailyLetter;
		Upgrade upgrade3 = Upgrades.upgrades[PowerupType.doubleMultiplier];
		doubleScoreMultiplier = new PickupType();
		doubleScoreMultiplier.spawnDistanceMin = (float)upgrade3.minimumMeters * num;
		doubleScoreMultiplier.spawnProbability = upgrade3.spawnProbability;
		doubleScoreMultiplier.ExtractGameObject = (SpawnPoint spawnPoint) => spawnPoint.doubleScoreMultiplier;
		Upgrade upgrade4 = Upgrades.upgrades[PowerupType.jetpack];
		jetpackPickup = new PickupType();
		jetpackPickup.spawnDistanceMin = (float)upgrade4.minimumMeters * num;
		jetpackPickup.spawnProbability = upgrade4.spawnProbability;
		jetpackPickup.ExtractGameObject = (SpawnPoint spawnPoint) => spawnPoint.jetpackPickup;
		Upgrade upgrade5 = Upgrades.upgrades[PowerupType.supersneakers];
		jumpBooster = new PickupType();
		jumpBooster.spawnDistanceMin = (float)upgrade5.minimumMeters * num;
		jumpBooster.spawnProbability = upgrade5.spawnProbability;
		jumpBooster.ExtractGameObject = (SpawnPoint spawnPoint) => spawnPoint.jumpBooster;
		Upgrade upgrade6 = Upgrades.upgrades[PowerupType.coinmagnet];
		magnetBooster = new PickupType();
		magnetBooster.spawnDistanceMin = (float)upgrade6.minimumMeters * num;
		magnetBooster.spawnProbability = upgrade6.spawnProbability;
		magnetBooster.ExtractGameObject = (SpawnPoint spawnPoint) => spawnPoint.magnetBooster;
		Upgrade upgrade7 = Upgrades.upgrades[PowerupType.mysterybox];
		mysteryBox = new PickupType();
		mysteryBox.spawnDistanceMin = (float)upgrade7.minimumMeters * num;
		mysteryBox.spawnProbability = upgrade7.spawnProbability;
		mysteryBox.ExtractGameObject = (SpawnPoint spawnPoint) => spawnPoint.mysteryBox;
		Upgrade upgrade8 = Upgrades.upgrades[PowerupType.saveMeToken];
		saveMeToken = new PickupType();
		saveMeToken.spawnDistanceMin = (float)upgrade8.minimumMeters * num;
		saveMeToken.spawnProbability = upgrade8.spawnProbability;
		saveMeToken.ExtractGameObject = (SpawnPoint spawnPoint) => spawnPoint.saveMeToken;
		Upgrade upgrade9 = Upgrades.upgrades[PowerupType.mysteryPowerup];
		mysteryPowerup = new PickupType();
		mysteryPowerup.spawnDistanceMin = (float)upgrade9.minimumMeters * num;
		mysteryPowerup.spawnProbability = upgrade9.spawnProbability;
		mysteryPowerup.ExtractGameObject = (SpawnPoint spawnPoint) => spawnPoint.mysteryPowerup;
		pickups = new List<PickupType> { characterLetterPickupType, dailyLetter, doubleScoreMultiplier, jetpackPickup, jumpBooster, magnetBooster, saveMeToken, mysteryBox, mysteryPowerup };
	}

	public void PerformSelection(SpawnPoint spawnPoint, List<GameObject> objectsToVisit)
	{
		float z = spawnPoint.transform.position.z;
		PickupType pickupType = null;
		CheckCharacterLetterSpawnRate();
		CheckLetterSpawnRate();
		CheckMysteryPowerupSpawnRate();
		if (CanSpawnPickup(z))
		{
			List<PickupType> list = pickups.FindAll((PickupType p) => p.spawnZ < z);
			if (list.Count > 0)
			{
				int[] array = new int[list.Count];
				int num = 0;
				for (int num2 = 0; num2 < list.Count; num2++)
				{
					num = (array[num2] = num + list[num2].spawnProbability);
				}
				int num3 = randomGen.Next(0, num + 1);
				for (int num4 = 0; num4 < array.Length; num4++)
				{
					if (num3 <= array[num4])
					{
						pickupType = list[num4];
						pickupType.spawnZ = z + pickupType.spawnDistanceMin;
						_DebugLog("CLP: selected to spawn: min:" + pickupType.spawnDistanceMin + " ,prob: " + pickupType.spawnProbability);
						break;
					}
				}
				SetNextSpawnPosition(z);
			}
		}
		for (int num5 = 0; num5 < pickups.Count; num5++)
		{
			PickupType pickupType2 = pickups[num5];
			GameObject gameObject = pickupType2.ExtractGameObject(spawnPoint);
			if (pickupType2 == pickupType)
			{
				SYBO.Subway.Pickups.PickupType pickupType3 = PickupsMigration.Migration_ConvertFromPickupTypeName2014(gameObject.name);
				PickupsMigration.Log2014PickupSpawnInNewLogger(pickupType3);
				if (Globals.DEBUG_PRINT_SPAWNED_POWER_UPS)
				{
					SpawnedObjects.countUP(gameObject.name);
				}
				objectsToVisit.Add(gameObject);
			}
			else
			{
				gameObject.SetActive(false);
			}
		}
	}

	public bool CanSpawnPickup(float z)
	{
		return z > spawnZ;
	}

	public void SetNextSpawnPosition(float z)
	{
		spawnZ = z + spawnSpacing;
	}

	private void CheckMysteryPowerupSpawnRate()
	{
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager(SSEventType.MysteryPowerup);
		if (activeEventManager != null)
		{
			mysteryPowerup.spawnProbability = 23;
		}
		else
		{
			mysteryPowerup.spawnProbability = 0;
		}
	}

	private void _DebugLog(string message)
	{
		if (TEMP_WILL_SHOW_EXTRA_LOG_MESSAGES)
		{
			Debug.Log(message);
		}
	}

	private bool _IsCharacterNameEventActive()
	{
		return SSEventsManagerController.instance.GetActiveEventManager(SSEventType.CharacterName) != null;
	}

	private bool _IsCharacterLetterPickupSpawnable()
	{
		bool result = false;
		if (Globals.DEBUG_USE_ENABLE_SS_EVENT_TYPE___LETTER_HUNT_CHARACTER)
		{
			return true;
		}
		if (_IsCharacterNameEventActive())
		{
			CharacterNameEventManager typeOfActiveEventManager = SSEventsManagerController.instance.GetTypeOfActiveEventManager<CharacterNameEventManager>();
			if (typeOfActiveEventManager != null)
			{
				result = typeOfActiveEventManager.ShouldSpawnLetters();
			}
		}
		return result;
	}

	private void CheckCharacterLetterSpawnRate()
	{
		if (_IsCharacterLetterPickupSpawnable())
		{
			if (characterLetterPickupType.spawnProbability != 25)
			{
				characterLetterPickupType.spawnProbability = 25;
			}
		}
		else if (characterLetterPickupType.spawnProbability != 0)
		{
			characterLetterPickupType.spawnProbability = 0;
		}
		_DebugLog("CLP: ------------ ( SpawnPointManager ) ------------------------------");
		_DebugLog("CLP: Globals.DEBUG_USE_ENABLE_SS_EVENT_TYPE___LETTER_HUNT_CHARACTER : " + Globals.DEBUG_USE_ENABLE_SS_EVENT_TYPE___LETTER_HUNT_CHARACTER);
		_DebugLog("CLP: PlayerInfo.Instance.tutorialCompleted " + PlayerInfo.Instance.IsTutorialCompleted);
		_DebugLog("CLP: char1 event active: " + _IsCharacterNameEventActive());
		_DebugLog("CLP: char2 spawnable   : " + _IsCharacterLetterPickupSpawnable());
		_DebugLog("CLP: char4 char prob   : " + characterLetterPickupType.spawnProbability);
	}

	private void CheckLetterSpawnRate()
	{
		if (PlayerInfo.Instance.GetNewDailyLetter() == '\0')
		{
			if (dailyLetter.spawnProbability != 0)
			{
				dailyLetter.spawnProbability = 0;
			}
		}
		else if (PlayerInfo.Instance.dailyWordTimeLeft.TotalSeconds <= 0.0)
		{
			if (dailyLetter.spawnProbability != 0)
			{
				Debug.Log("time ran out sorry, no more letters for you");
				dailyLetter.spawnProbability = 0;
			}
		}
		else if (dailyLetter.spawnProbability != 1500000000)
		{
			dailyLetter.spawnProbability = 1500000000;
		}
	}

	public void Restart()
	{
		float num = 8f;
		spawnZ = 250f * num;
		spawnSpacing = 300f * num;
		foreach (PickupType pickup in pickups)
		{
			pickup.spawnZ = float.MinValue;
		}
	}
}
