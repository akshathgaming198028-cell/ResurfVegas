using UnityEngine;

public struct Upgrade
{
	public StringID name;

	public StringID nameTwoLines;

	public StringID description;

	public StringID mysteryBoxDescription;

	public int numberOfTiers;

	public float[] durations;

	public float speed;

	public float landSpeed;

	public int spawnProbability;

	public int minimumMeters;

	public int coinmagnetRange;

	public int[] pricesRaw;

	public int levelPriceMultiplyer;

	public string iconName;

	public int weight;

	public StringID GetName()
	{
		if (!string.IsNullOrEmpty(Strings.Get(nameTwoLines)))
		{
			return nameTwoLines;
		}
		return name;
	}

	public int getPrice(int tier)
	{
		if (pricesRaw == null)
		{
			Debug.LogWarning("Prices is not initialized");
			return -1;
		}
		return pricesRaw[tier] + levelPriceMultiplyer * Mathf.Clamp(MissionsManager.Instance.currentMissionSet, 0, MissionsManager.Instance.missionSetStoryCount);
	}
}
