using System;
using System.Collections.Generic;
using UnityEngine;

public class Friend
{
	public enum Medal
	{
		none = 0,
		bronze = 1,
		silver = 2,
		gold = 3
	}

	public class Status
	{
		public DateTime lastPokeTime = DateTime.MinValue;

		public DateTime lastDuelPokeTime = DateTime.MinValue;

		public int gamesCashedIn;
	}

	public const int RELATION_FACEBOOK_MASK = 1;

	public const int RELATION_GAMECENTER_MASK = 2;

	public static readonly string[] romanRanks = new string[21]
	{
		string.Empty,
		"I",
		"II",
		"III",
		"IV",
		"V",
		"VI",
		"VII",
		"VIII",
		"IX",
		"X",
		"XI",
		"XII",
		"XIII",
		"XIV",
		"XV",
		"XVI",
		"XVII",
		"XVIII",
		"XIX",
		"XX"
	};

	public static readonly Dictionary<Medal, string> medalSpriteString = new Dictionary<Medal, string>
	{
		{
			Medal.none,
			string.Empty
		},
		{
			Medal.bronze,
			"icon_medal_bronze"
		},
		{
			Medal.silver,
			"icon_medal_silver"
		},
		{
			Medal.gold,
			"icon_medal_gold"
		}
	};

	public int userid;

	public int score;

	public int meters;

	public int games;

	public int rank;

	public GameCenterProfile gcProfile;

	public FacebookProfile fbProfile;

	public OfflineProfile offlineProfile;

	public Status status;

	private Action _updatedHandler;

	public string name
	{
		get
		{
			if (fbProfile != null)
			{
				return fbProfile.name;
			}
			if (gcProfile != null)
			{
				return gcProfile.alias;
			}
			if (offlineProfile != null)
			{
				return offlineProfile.name;
			}
			Debug.LogError("Friend not initialized");
			return null;
		}
		set
		{
			if (fbProfile == null && gcProfile == null)
			{
				if (offlineProfile == null)
				{
					offlineProfile = new OfflineProfile();
				}
				offlineProfile.name = value;
			}
		}
	}

	public Texture2D image
	{
		get
		{
			Texture2D result = null;
			if (fbProfile != null)
			{
				result = fbProfile.image;
			}
			else if (gcProfile != null)
			{
				result = gcProfile.image;
			}
			return result;
		}
	}

	public int relation
	{
		get
		{
			int num = 0;
			if (fbProfile != null)
			{
				num |= 1;
			}
			if (gcProfile != null)
			{
				num |= 2;
			}
			return num;
		}
	}

	public string id
	{
		get
		{
			if (fbProfile != null)
			{
				return fbProfile.id;
			}
			if (gcProfile != null)
			{
				return gcProfile.id;
			}
			return null;
		}
	}

	public int gamesToCashIn
	{
		get
		{
			if (games < status.gamesCashedIn)
			{
				Debug.LogWarning("CashedIn more runs that games run. " + games + " <-GAMES,gamesToCashIn-> " + status.gamesCashedIn);
				status.gamesCashedIn = games;
			}
			return games - status.gamesCashedIn;
		}
	}

	public bool shouldBeDuelPoked
	{
		get
		{
			return isPokeTime(status.lastDuelPokeTime);
		}
	}

	public bool shouldBePoked
	{
		get
		{
			return isPokeTime(status.lastPokeTime);
		}
	}

	private bool isPokeTime(DateTime lastPokeTime)
	{
		if ((DateTime.UtcNow - lastPokeTime).Days > 0 && status.lastPokeTime != DateTime.MinValue)
		{
			return true;
		}
		return false;
	}

	public void AddUpdatedHandler(Action handler)
	{
		_updatedHandler = (Action)Delegate.Combine(_updatedHandler, handler);
	}

	public void RemoveUpdatedHandler(Action handler)
	{
		_updatedHandler = (Action)Delegate.Remove(_updatedHandler, handler);
	}

	public void InvokeUpdatedHandler()
	{
		if (_updatedHandler != null)
		{
			_updatedHandler();
		}
	}
}
