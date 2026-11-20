using System;
using System.Collections.Generic;

public static class InterstitialLocations
{
	private static Location[] _showLocations;

	public static readonly List<string> ALLOWED_CACHE_SCREENS;

	static InterstitialLocations()
	{
		ALLOWED_CACHE_SCREENS = new List<string> { "CoinsUI_shop", "CharacterScreen", "GameoverUI", "AwardsScreen", "UpgradesUI_shop", "BoardScreen", "EarnCoinsScreen", "FriendsUI", "LeaderboardUI", "TopRunUI" };
		LocationID[] array = (LocationID[])Enum.GetValues(typeof(LocationID));
		int num = (int)array[array.Length - 1];
		_showLocations = new Location[num + 1];
		_showLocations[0] = new Location
		{
			id = LocationID.DisableShow,
			lastShownAdTimeStamp = DateTime.MinValue,
			defaultTimeLimitToNextShow = 0u,
			defaultEnabled = false
		};
		_showLocations[1] = new Location
		{
			id = LocationID.front_screen_after_tweens,
			lastShownAdTimeStamp = DateTime.MinValue,
			defaultTimeLimitToNextShow = 0u,
			defaultEnabled = true
		};
		_showLocations[2] = new Location
		{
			id = LocationID.gameover_after_countup,
			lastShownAdTimeStamp = DateTime.MinValue,
			defaultTimeLimitToNextShow = 300u,
			defaultEnabled = false
		};
	}

	public static Location GetLocation(LocationID locationID)
	{
		return _showLocations[(int)locationID];
	}

	public static Location SetLocationLastShownAdTimeStamp(Location location, DateTime timeStamp)
	{
		Location location2 = _showLocations[(int)location.id];
		location2.lastShownAdTimeStamp = timeStamp;
		_showLocations[(int)location.id] = location2;
		return location2;
	}
}
