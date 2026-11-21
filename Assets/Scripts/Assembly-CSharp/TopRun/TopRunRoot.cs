using System;
using TopRun.Internal;
using UnityEngine;

namespace TopRun
{
	public static class TopRunRoot
	{
		private static IManager _manager;

		// Lazily created. HouseKeeper.Awake normally initialises it, but on device the UI
		// (TopRunOnlineScreen, TopRunEventsHandler) sometimes ran first and hit a null manager.
		public static IManager manager
		{
			get
			{
				if (_manager == null)
				{
					InitManager(CountryManager.instance.GetDefaultCountryCodeFromDevice(), PlayerInfo.Instance.rawMultiplier, PlayerInfo.Instance.highestScore);
				}
				return _manager;
			}
		}

		public static void Init(string defaultCountry, int multiplier, int highScore)
		{
			InitManager(defaultCountry, multiplier, highScore);
		}

		private static void InitManager(string defaultCountry, int multiplier, int highScore)
		{
			if (_manager == null)
			{
				try
				{
					_manager = new Manager(defaultCountry, multiplier, highScore);
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
