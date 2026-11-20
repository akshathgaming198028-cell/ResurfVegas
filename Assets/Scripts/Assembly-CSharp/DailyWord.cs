#define ENABLE_ERROR_LOGS
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using Kiloo.Common;
using UnityEngine;

public class DailyWord : MonoBehaviour
{
	private enum DailyWordServerData
	{
		wordIndex = 0,
		dailyWord = 1,
		year = 2,
		checksum = 3,
		month = 4,
		day = 5,
		hour = 6,
		minute = 7,
		second = 8,
		expireSeconds = 9
	}

	public enum DailyWordRewardType
	{
		Coins = 0,
		MysteryBox = 1
	}

	public struct DailyWordReward
	{
		public DailyWordRewardType type;

		public int coins;

		public MysteryBox.Type mysteryBoxType;
	}

	public static readonly DailyWordReward[] REWARDS = new DailyWordReward[5]
	{
		new DailyWordReward
		{
			type = DailyWordRewardType.MysteryBox,
			mysteryBoxType = MysteryBox.Type.Normal
		},
		new DailyWordReward
		{
			type = DailyWordRewardType.MysteryBox,
			mysteryBoxType = MysteryBox.Type.Normal
		},
		new DailyWordReward
		{
			type = DailyWordRewardType.Coins,
			coins = 1050
		},
		new DailyWordReward
		{
			type = DailyWordRewardType.Coins,
			coins = 1500
		},
		new DailyWordReward
		{
			type = DailyWordRewardType.MysteryBox,
			mysteryBoxType = MysteryBox.Type.Super
		}
	};

	private bool stoppingFromEditor;

	private DateTime _lastRequestTime = DateTime.MinValue;

	private static DailyWord _instance;

	public static DailyWord Instance
	{
		get
		{
			return _instance ?? (_instance = UnityEngine.Object.FindObjectOfType(typeof(DailyWord)) as DailyWord);
		}
	}

	private void Start()
	{
		DownloadDaily();
		UIScreenController instance = UIScreenController.Instance;
		instance.OnChangedScreen = (Action<string>)Delegate.Combine(instance.OnChangedScreen, new Action<string>(AutoSyncer));
	}

	private void OnApplicationQuit()
	{
		stoppingFromEditor = true;
	}

	private void OnDestroy()
	{
		if (!stoppingFromEditor)
		{
			UIScreenController instance = UIScreenController.Instance;
			instance.OnChangedScreen = (Action<string>)Delegate.Remove(instance.OnChangedScreen, new Action<string>(AutoSyncer));
		}
	}

	private void AutoSyncer(string screenName)
	{
		if (screenName == "FrontUI" || screenName == "GameoverUI")
		{
			DownloadDailyIfExpired();
		}
	}

	public void DownloadDaily()
	{
		StartCoroutine(DownloadDailyCoroutine());
	}

	public void DownloadDailyIfExpired()
	{
		if (PlayerInfo.Instance.dailyWordTimeLeft.TotalSeconds <= 0.0)
		{
			StartCoroutine(DownloadDailyCoroutine());
		}
	}

	private IEnumerator DownloadDailyCoroutine()
	{
		DateTime time = DateTime.UtcNow;
		if (!((time - _lastRequestTime).TotalSeconds > 120.0))
		{
			yield break;
		}
		_lastRequestTime = time;
		string key = GenerateKey();
		WWWForm postData = new WWWForm();
		postData.AddField("key", key);
		WWW www = new WWW("http://hoodrunner.kiloo.com/hr_dailyquests2.php", postData);
		yield return www;
		if (www.error == null)
		{
			string[] rawData = www.text.Split(';');
			if (SHA1HashCheck(rawData, key))
			{
				string word = rawData[1];
				int expireSeconds = Convert.ToInt32(rawData[9]);
				CalculateAndSaveLastKnownServerTime(rawData[2], rawData[4], rawData[5], rawData[6], rawData[7], rawData[8]);
				PlayerInfo.Instance.InitDailyWord(word, expireSeconds);
			}
			else
			{
				LogError("DailyWord.DownloadDailyCoroutine: Checksum failed");
			}
		}
		else
		{
			LogWarning("DailyWord.DownloadDailyCoroutine: www.error: " + www.error);
		}
	}

	private void CalculateAndSaveLastKnownServerTime(string year, string month, string day, string hour, string minute, string second)
	{
		DateTime minValue = DateTime.MinValue;
		try
		{
			minValue = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day), int.Parse(hour), int.Parse(minute), int.Parse(second), DateTimeKind.Utc);
			PlayerInfo.Instance.lastKnownServerTime = minValue;
		}
		catch (Exception ex)
		{
			LogError("DailyWord.CalculateAndSaveLastKnownServerTime: Was not able to parse the date received from the server. Exception: " + ex.Message);
		}
	}

	private bool SHA1HashCheck(string[] rawData, string key)
	{
		if (rawData.Length < 10)
		{
			return false;
		}
		string a = rawData[3];
		string b = StringUtility.ToHexString(Checksum.ComputeSHA1(rawData[0] + rawData[1] + rawData[2] + key + "aIN0UXP4NNoANVGi5w3raGAFN1n5OLQZFDhwjs6HoX" + rawData[4] + rawData[5] + rawData[6] + rawData[7] + rawData[8] + rawData[9]), true);
		if (string.Equals(a, b, StringComparison.InvariantCultureIgnoreCase))
		{
			return true;
		}
		return false;
	}

	private static string RandomString(int size)
	{
		StringBuilder stringBuilder = new StringBuilder();
		System.Random random = new System.Random();
		for (int i = 0; i < size; i++)
		{
			char value = Convert.ToChar(Convert.ToInt32(Math.Floor(26.0 * random.NextDouble() + 65.0)));
			stringBuilder.Append(value);
		}
		return stringBuilder.ToString();
	}

	private static int RandomNumber(int min, int max)
	{
		System.Random random = new System.Random();
		return random.Next(min, max);
	}

	private static string GenerateKey()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(RandomString(4));
		stringBuilder.Append(RandomNumber(0, 999));
		stringBuilder.Append(RandomString(2));
		return stringBuilder.ToString();
	}

	public static DailyWordReward GetRewardForDay(int dayIndex)
	{
		if (dayIndex >= REWARDS.Length)
		{
			dayIndex = REWARDS.Length - 1;
		}
		return REWARDS[dayIndex];
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	public static void LogError(string msg, UnityEngine.Object context = null)
	{
		Debug.LogError(msg, context);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	public static void LogWarning(string msg, UnityEngine.Object context = null)
	{
		Debug.LogWarning(msg, context);
	}

	[Conditional("ENABLE_DAILYWORD_LOGS")]
	public static void Log(string msg, UnityEngine.Object context = null)
	{
		Debug.Log(msg, context);
	}
}
