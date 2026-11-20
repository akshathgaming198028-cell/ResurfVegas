using System.Globalization;
using Kiloo.Common.Utility;
using UnityEngine;

namespace Kiloo.Common.Extensions
{
	public static class NumberExtensions
	{
		public static float MRound(this float f, float roundFactor)
		{
			return (f / roundFactor).Round() * roundFactor;
		}

		public static int MRoundToInt(this float f, float roundFactor)
		{
			return f.MRound(roundFactor).RoundToInt();
		}

		public static bool IsBetween(this int current, int low, int high)
		{
			return current > low && current < high;
		}

		public static bool Chance(this float percentChance)
		{
			return KRandom.Range(0f, 1f) <= percentChance;
		}

		public static int Roll(this int max)
		{
			return KRandom.Range(0, max);
		}

		public static float Variance(this float baseValue, float percentVariance)
		{
			return baseValue + KRandom.Range((0f - baseValue) * percentVariance, baseValue * percentVariance);
		}

		public static string ToOrdinal(this int num)
		{
			if (num <= 0)
			{
				return num.ToString(CultureInfo.InvariantCulture);
			}
			switch (num % 100)
			{
			case 11:
			case 12:
			case 13:
				return num + "th";
			default:
				switch (num % 10)
				{
				case 1:
					return num + "st";
				case 2:
					return num + "nd";
				case 3:
					return num + "rd";
				default:
					return num + "th";
				}
			}
		}

		public static float Round(this float f)
		{
			return Mathf.Round(f);
		}

		public static int RoundToInt(this float f)
		{
			return Mathf.RoundToInt(f);
		}

		public static float Ceil(this float f)
		{
			return Mathf.Ceil(f);
		}

		public static int CeilToInt(this float f)
		{
			return Mathf.CeilToInt(f);
		}

		public static float Floor(this float f)
		{
			return Mathf.Floor(f);
		}

		public static int FloorToInt(this float f)
		{
			return Mathf.FloorToInt(f);
		}

		public static float Pow(this float f, float power)
		{
			return Mathf.Pow(f, power);
		}

		public static float Pow(this int i, float power)
		{
			return Mathf.Pow(i, power);
		}

		public static float Clamp(this float f, float min, float max)
		{
			return Mathf.Clamp(f, min, max);
		}

		public static int Clamp(this int f, int min, int max)
		{
			return Mathf.Clamp(f, min, max);
		}
	}
}
