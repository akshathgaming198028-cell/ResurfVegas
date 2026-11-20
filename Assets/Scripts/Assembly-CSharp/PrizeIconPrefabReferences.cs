using System;
using UnityEngine;

public class PrizeIconPrefabReferences : MonoBehaviour
{
	[Serializable]
	private struct PrizeModel
	{
		public WeeklyHuntManager.WeeklyHuntPrize rewardType;

		public GameObject iconPrefab;
	}

	[SerializeField]
	private PrizeModel[] modelReferences = new PrizeModel[0];

	public GameObject GetPrefab(WeeklyHuntManager.PrizeData prizeData)
	{
		WeeklyHuntManager.WeeklyHuntPrize weeklyHuntPrize = WeeklyHuntManager.DeterminePrize(prizeData);
		for (int i = 0; i < modelReferences.Length; i++)
		{
			if (modelReferences[i].rewardType == weeklyHuntPrize)
			{
				GameObject iconPrefab = modelReferences[i].iconPrefab;
				if (!iconPrefab)
				{
					Debug.LogError(string.Format("Prize '{0}' doesn't have a model set", weeklyHuntPrize.ToString()), base.gameObject);
				}
				return iconPrefab;
			}
		}
		Debug.LogError(string.Format("Unable to find icon prefab for prize '{0}'", prizeData.rewardType.ToString()), base.gameObject);
		return null;
	}
}
