using System.Collections.Generic;
using UnityEngine;

public static class PrizePositionData
{
	public struct TransformValues
	{
		public Vector3 PositionOffset;

		public Vector3 Rotation;

		public Vector3 Scale;

		public TransformValues(Vector3 positionOffset, Vector3 rotation, Vector3 scale)
		{
			PositionOffset = positionOffset;
			Rotation = rotation;
			Scale = scale;
		}
	}

	public static Vector3 HOVERBOARD_POSITION = new Vector3(0f, 0f, 0f);

	public static Vector3 HOVERBOARD_SCALE = new Vector3(11f, 11f, 11f);

	public static Vector3 HOVERBOARD_ROTATION = new Vector3(320.3f, 314.3f, 66.2f);

	public static Vector3 KEY_POSITION = new Vector3(0f, 2f, 0f);

	public static Vector3 KEY_SCALE = new Vector3(10f, 10f, 10f);

	public static Vector3 KEY_ROTATION = new Vector3(11f, 15f, 5.5f);

	public static Vector3 SCORE_BOOSTER_POSITION = new Vector3(2f, 0f, 0f);

	public static Vector3 SCORE_BOOSTER_SCALE = new Vector3(13f, 13f, 13f);

	public static Vector3 SCORE_BOOSTER_ROTATION = new Vector3(14f, 22f, 326.8f);

	public static Vector3 HEADSTART_POSITION = new Vector3(0f, 0f, 0f);

	public static Vector3 HEADSTART_SCALE = new Vector3(14f, 14f, 14f);

	public static Vector3 HEADSTART_ROTATION = new Vector3(13f, 24f, 319f);

	public static Vector3 MYSTERY_BOX_POSITION = new Vector3(0f, 0f, 0f);

	public static Vector3 MYSTERY_BOX_SCALE = new Vector3(0f, 0f, 0f);

	public static Vector3 MYSTERY_BOX_ROTATION = new Vector3(0f, 0f, 0f);

	public static Vector3 SUPER_MYSTERY_BOX_POSITION = new Vector3(-2.3f, 0f, 0f);

	public static Vector3 SUPER_MYSTERY_BOX_SCALE = new Vector3(7f, 7f, 7f);

	public static Vector3 SUPER_MYSTERY_BOX_ROTATION = new Vector3(343f, 50f, 345f);

	public static Vector3 DRAGON_BOARD_POSITION = new Vector3(4f, 0f, 0f);

	public static Vector3 DRAGON_BOARD_SCALE = new Vector3(12f, 12f, 12f);

	public static Vector3 DRAGON_BOARD_ROTATION = new Vector3(324.7f, 304.17f, 66.86f);

	public static Vector3 SLEIGH_BOARD_POSITION = new Vector3(7f, 5f, 0f);

	public static Vector3 SLEIGH_BOARD_SCALE = new Vector3(14f, 14f, 14f);

	public static Vector3 SLEIGH_BOARD_ROTATION = new Vector3(0.2f, 313.6f, 6.6f);

	public static Vector3 CHRISTMAS2014_BOARD_POSITION = new Vector3(7f, 5f, 0f);

	public static Vector3 CHRISTMAS2014_BOARD_SCALE = new Vector3(14f, 14f, 14f);

	public static Vector3 CHRISTMAS2014_BOARD_ROTATION = new Vector3(0.2f, 313.6f, 6.6f);

	public static Vector3 PRESENT_POSITION_OFFSET = new Vector3(0f, 0f, 0f);

	public static Vector3 PRESENT_ROTATION = new Vector3(21f, -138f, 21f);

	public static Vector3 PRESENT_SCALE = new Vector3(400f, 400f, 400f);

	private static Dictionary<WeeklyHuntManager.WeeklyHuntPrize, TransformValues?> _cachedPositionData = new Dictionary<WeeklyHuntManager.WeeklyHuntPrize, TransformValues?>();

	public static TransformValues? GetPrizePositioningData(WeeklyHuntManager.PrizeData prize)
	{
		WeeklyHuntManager.WeeklyHuntPrize weeklyHuntPrize = WeeklyHuntManager.DeterminePrize(prize);
		if (_cachedPositionData.ContainsKey(weeklyHuntPrize))
		{
			return _cachedPositionData[weeklyHuntPrize];
		}
		TransformValues? transformValues = null;
		switch (weeklyHuntPrize)
		{
		case WeeklyHuntManager.WeeklyHuntPrize.Key:
			transformValues = new TransformValues(KEY_POSITION, KEY_ROTATION, KEY_SCALE);
			break;
		case WeeklyHuntManager.WeeklyHuntPrize.SuperMysteryBox:
			transformValues = new TransformValues(SUPER_MYSTERY_BOX_POSITION, SUPER_MYSTERY_BOX_ROTATION, SUPER_MYSTERY_BOX_SCALE);
			break;
		case WeeklyHuntManager.WeeklyHuntPrize.Hoverboard:
			transformValues = new TransformValues(HOVERBOARD_POSITION, HOVERBOARD_ROTATION, HOVERBOARD_SCALE);
			break;
		case WeeklyHuntManager.WeeklyHuntPrize.ScoreBooster:
			transformValues = new TransformValues(SCORE_BOOSTER_POSITION, SCORE_BOOSTER_ROTATION, SCORE_BOOSTER_SCALE);
			break;
		case WeeklyHuntManager.WeeklyHuntPrize.Headstart:
			transformValues = new TransformValues(HEADSTART_POSITION, HEADSTART_ROTATION, HEADSTART_SCALE);
			break;
		case WeeklyHuntManager.WeeklyHuntPrize.ChristmasBoard:
			transformValues = new TransformValues(CHRISTMAS2014_BOARD_POSITION, CHRISTMAS2014_BOARD_ROTATION, CHRISTMAS2014_BOARD_SCALE);
			break;
		}
		if (transformValues.HasValue)
		{
			_cachedPositionData.Add(weeklyHuntPrize, transformValues);
		}
		else
		{
			Debug.LogError("Unable to find prize position data for type: " + weeklyHuntPrize);
		}
		return transformValues;
	}
}
