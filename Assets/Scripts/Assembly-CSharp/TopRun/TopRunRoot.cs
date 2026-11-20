using System;
using TopRun.Internal;
using UnityEngine;

namespace TopRun
{
	public static class TopRunRoot
	{
		public static IManager manager { get; private set; }

		public static void Init(string defaultCountry, int multiplier, int highScore)
		{
			if (manager == null)
			{
				try
				{
					manager = new Manager(defaultCountry, multiplier, highScore);
				}
				catch (Exception ex)
				{
					Debug.LogError("[TopRun.Manager] Init Failed: " + ex);
				}
			}
		}

		public static void ClearSavedData()
		{
			Manager.ClearSavedData();
		}
	}
}
