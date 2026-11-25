using System;
using System.Collections.Generic;
using SYBO.Subway.Info;

public static class LocalEventSchedule
{
	private static readonly DateTime FirstThursday = new DateTime(2016, 6, 2, 0, 0, 0, DateTimeKind.Utc);

	private static readonly Characters.CharacterType[] Rotation = new Characters.CharacterType[3] { Characters.CharacterType.facebookRewardCharacter01, Characters.CharacterType.lasVegasCharacter01, Characters.CharacterType.princek };

	public static void Apply(Dictionary<string, string> settings)
	{
		DateTime utcToday = DateTime.UtcNow.Date;
		int daysSinceThursday = ((int)utcToday.DayOfWeek - (int)DayOfWeek.Thursday + 7) % 7;
		DateTime thursday = utcToday.AddDays(-daysSinceThursday);
		int week = (int)((thursday - FirstThursday).TotalDays / 7.0);
		Characters.CharacterType character = Rotation[((week % Rotation.Length) + Rotation.Length) % Rotation.Length];
		long start = (long)(thursday - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
		long end = start + 7L * 86400L;
		settings["event_id"] = start.ToString();
		settings["event_start"] = start.ToString();
		settings["event_end"] = end.ToString();
		settings["event_type"] = "CharacterDay";
		settings["event_value"] = character.ToString();
	}
}
