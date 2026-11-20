using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Kiloo.Common;
using Kiloo.Plugins.DeviceUtility;
using UnityEngine;

public static class CachedLeaderboard
{
	private const string FILENAME = "leaderboard";

	private const string PLAYERPREFS_KEY = "HasCachedLeaderboard";

	private static TimeSpan expirationDate = new TimeSpan(1, 0, 0, 0);

	public static Friend[] Load()
	{
		Friend[] array = null;
		if (IsAvailable())
		{
			try
			{
				string savePath = GetSavePath();
				byte[] buffer = FileUtil.Load(savePath);
				MemoryStream memoryStream = new MemoryStream(buffer);
				BinaryReader reader = new BinaryReader(memoryStream);
				Dictionary<string, string> dictionary = FileUtil.ReadStringStringDictionary(reader);
				array = new Friend[dictionary.Count];
				int num = 0;
				foreach (KeyValuePair<string, string> item in dictionary)
				{
					if (!string.IsNullOrEmpty(item.Key) && !string.IsNullOrEmpty(item.Value))
					{
						array[num] = ParseFriend(item.Key, item.Value);
						num++;
					}
					else
					{
						Debug.LogWarning("Friend parsed contains null or empty string.");
					}
				}
				memoryStream.Close();
			}
			catch (Exception ex)
			{
				Debug.LogError("Error loading leaderboard: " + ex.ToString());
			}
		}
		return array;
	}

	public static void Save(Friend[] friends)
	{
		if (friends != null && friends.Length > 0)
		{
			SaveLeaderboardToFile(friends);
			PlayerPrefs.SetString("HasCachedLeaderboard", DateTime.Now.ToString(CultureInfo.InvariantCulture));
		}
		else if (!Application.isEditor)
		{
			Debug.LogWarning("Couldn't save leaderboard, friend list was null or empty. Save aborted.");
		}
	}

	private static void SaveLeaderboardToFile(Friend[] friends)
	{
		try
		{
			MemoryStream memoryStream = new MemoryStream(8192);
			BinaryWriter writer = new BinaryWriter(memoryStream);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			int num = 0;
			foreach (Friend friend in friends)
			{
				string key = num.ToString();
				string text = friend.name + "," + friend.score + "," + friend.games;
				text = ((friend.fbProfile == null) ? ((friend.gcProfile == null) ? (text + "," + OfflineProfile.ProfileOrigin.Facebook) : (text + "," + OfflineProfile.ProfileOrigin.GameCenter)) : (text + "," + OfflineProfile.ProfileOrigin.Facebook));
				dictionary.Add(key, text);
				num++;
			}
			FileUtil.WriteStringStringDictionary(writer, dictionary);
			FileUtil.Save(GetSavePath(), memoryStream.GetBuffer(), null, 0, (int)memoryStream.Length, 1, 5);
			memoryStream.Close();
		}
		catch (Exception ex)
		{
			Debug.LogError("Error saving leaderboard: " + ex.ToString());
		}
	}

	private static Friend ParseFriend(string id, string values)
	{
		Friend friend = new Friend();
		string[] array = values.Split(',');
		if (array.Length == 4)
		{
			friend.name = array[0];
			friend.score = int.Parse(array[1]);
			friend.games = int.Parse(array[2]);
			friend.offlineProfile.origin = (OfflineProfile.ProfileOrigin)(int)Enum.Parse(typeof(OfflineProfile.ProfileOrigin), array[3]);
		}
		else
		{
			Debug.LogWarning("Parsed friend's values incorrect.");
		}
		return friend;
	}

	private static string GetSavePath()
	{
		return DeviceUtility.GetUserDataPath() + "/leaderboard";
	}

	public static bool IsAvailable()
	{
		return !HasExpired();
	}

	public static void DeleteFile()
	{
		if (Exists())
		{
			try
			{
				string savePath = GetSavePath();
				File.Delete(savePath);
				PlayerPrefs.DeleteKey("HasCachedLeaderboard");
			}
			catch (Exception ex)
			{
				Debug.LogError("Error saving leaderboard: " + ex.ToString());
			}
		}
	}

	public static void RemoveFacebookFriends()
	{
		Friend[] array = Load();
		List<Friend> list = new List<Friend>();
		if (array != null)
		{
			Friend[] array2 = array;
			foreach (Friend friend in array2)
			{
				if (friend.offlineProfile.origin != OfflineProfile.ProfileOrigin.Facebook)
				{
					list.Add(friend);
				}
			}
		}
		if (list.Count > 0)
		{
			Save(list.ToArray());
		}
		else
		{
			DeleteFile();
		}
	}

	private static bool Exists()
	{
		return PlayerPrefs.HasKey("HasCachedLeaderboard");
	}

	private static bool HasExpired()
	{
		if (Exists())
		{
			DateTime dateTime = Utils.StringToDateTime(PlayerPrefs.GetString("HasCachedLeaderboard"), DateTime.MinValue);
			if ((DateTime.Now - dateTime).TotalSeconds < expirationDate.TotalSeconds && dateTime < DateTime.Now)
			{
				return false;
			}
			DeleteFile();
			return true;
		}
		return true;
	}
}
