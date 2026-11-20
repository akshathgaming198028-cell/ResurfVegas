using System.Collections.Generic;
using SYBO.Subway;
using SYBO.Subway.Level;
using SYBO.Subway.Pickups;
using SYBO.Subway.Pickups.Migration;
using UnityEngine;

public class SpawnPointEgg : Selector
{
	private const float SPAWN_MINIMUM_TIME = 5f;

	private const float SPAWN_MAXIMUM_TIME = 240f;

	private const float SPAWN_PROBABILTY = 0.06f;

	public GameObject easterEgg;

	private static float lastSpawn;

	public override void PerformSelection(List<GameObject> objectsToVisit)
	{
		base.PerformSelection(objectsToVisit);
		bool flag = false;
		if (Game.Instance.ElapsedGameTime < lastSpawn + 5f)
		{
			flag = false;
		}
		else if (Game.Instance.ElapsedGameTime > lastSpawn + 240f)
		{
			flag = true;
		}
		else if (Random.value <= 0.06f)
		{
			flag = true;
		}
		if (flag && WeeklyHuntManager.IsAnyPeriodActive())
		{
			PickupsMigration.Log2014PickupSpawnInNewLogger(PickupType.TokenHunt);
			if (Globals.DEBUG_PRINT_SPAWNED_POWER_UPS)
			{
				SpawnedObjects.countUP("huntToken");
			}
			objectsToVisit.Add(easterEgg);
			lastSpawn = Game.Instance.ElapsedGameTime;
		}
		else
		{
			easterEgg.SetActive(false);
		}
	}
}
