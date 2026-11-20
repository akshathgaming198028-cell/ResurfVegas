using System.Collections.Generic;
using SYBO.Subway.Level;
using UnityEngine;

public class SpawnPoint : Selector
{
	public GameObject characterLetter;

	public GameObject dailyLetter;

	public GameObject doubleScoreMultiplier;

	public GameObject jetpackPickup;

	public GameObject jumpBooster;

	public GameObject magnetBooster;

	public GameObject mysteryBox;

	public GameObject saveMeToken;

	public GameObject mysteryPowerup;

	public override void PerformSelection(List<GameObject> objectsToVisit)
	{
		base.PerformSelection(objectsToVisit);
		SpawnPointManager.Instance.PerformSelection(this, objectsToVisit);
	}
}
