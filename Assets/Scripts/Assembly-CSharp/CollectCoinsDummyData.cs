using System;
using System.Collections.Generic;
using UnityEngine;

public class CollectCoinsDummyData
{
	public class CollectCoinsDummy
	{
		public int index;

		public string name;

		public bool hasBeenCollected;

		public int fakeProgress;

		public override string ToString()
		{
			return string.Empty + index + ':' + name + ':' + hasBeenCollected.ToString() + ':' + fakeProgress;
		}

		public void Collected()
		{
			hasBeenCollected = true;
		}

		public bool ParseDummy(string rawDummyData)
		{
			string[] array = rawDummyData.Split(':');
			bool result = false;
			if (array.Length == 4)
			{
				try
				{
					index = int.Parse(array[0]);
					name = array[1];
					hasBeenCollected = bool.Parse(array[2]);
					fakeProgress = int.Parse(array[3]);
					result = true;
				}
				catch (Exception ex)
				{
					throw ex;
				}
			}
			else
			{
				Debug.LogError("Unexpected number of parameters: " + rawDummyData + ". Aborting parsing.");
			}
			return result;
		}
	}

	private const char DATA_VALUE_SEPARATOR = ':';

	private const char DATA_CHUNK_SEPARATOR = ';';

	private static bool _dirty = false;

	private static bool _initialized = false;

	private static Dictionary<int, CollectCoinsDummy> _dummyData = null;

	private static Dictionary<int, CollectCoinsDummy> _defaultDummyList = new Dictionary<int, CollectCoinsDummy>
	{
		{
			0,
			new CollectCoinsDummy
			{
				index = 0,
				name = "Friend",
				hasBeenCollected = false,
				fakeProgress = 0
			}
		},
		{
			1,
			new CollectCoinsDummy
			{
				index = 1,
				name = "Homie",
				hasBeenCollected = false,
				fakeProgress = 0
			}
		},
		{
			2,
			new CollectCoinsDummy
			{
				index = 2,
				name = "Mate",
				hasBeenCollected = false,
				fakeProgress = 0
			}
		},
		{
			3,
			new CollectCoinsDummy
			{
				index = 3,
				name = "Friend",
				hasBeenCollected = true,
				fakeProgress = 48
			}
		},
		{
			4,
			new CollectCoinsDummy
			{
				index = 4,
				name = "Pal",
				hasBeenCollected = true,
				fakeProgress = 41
			}
		},
		{
			5,
			new CollectCoinsDummy
			{
				index = 5,
				name = "Amiga",
				hasBeenCollected = true,
				fakeProgress = 35
			}
		},
		{
			6,
			new CollectCoinsDummy
			{
				index = 6,
				name = "Homeboy",
				hasBeenCollected = true,
				fakeProgress = 26
			}
		},
		{
			7,
			new CollectCoinsDummy
			{
				index = 7,
				name = "Mate",
				hasBeenCollected = true,
				fakeProgress = 22
			}
		},
		{
			8,
			new CollectCoinsDummy
			{
				index = 8,
				name = "Neighbour",
				hasBeenCollected = true,
				fakeProgress = 14
			}
		},
		{
			9,
			new CollectCoinsDummy
			{
				index = 9,
				name = "Friend",
				hasBeenCollected = true,
				fakeProgress = 7
			}
		},
		{
			10,
			new CollectCoinsDummy
			{
				index = 10,
				name = "Schoolmate",
				hasBeenCollected = true,
				fakeProgress = 2
			}
		},
		{
			11,
			new CollectCoinsDummy
			{
				index = 10,
				name = "Homegirl",
				hasBeenCollected = true,
				fakeProgress = 2
			}
		},
		{
			12,
			new CollectCoinsDummy
			{
				index = 10,
				name = "Homie",
				hasBeenCollected = true,
				fakeProgress = 1
			}
		},
		{
			13,
			new CollectCoinsDummy
			{
				index = 10,
				name = "Mate",
				hasBeenCollected = true,
				fakeProgress = 0
			}
		}
	};

	public bool IsDirty
	{
		get
		{
			return _dirty;
		}
		private set
		{
		}
	}

	public CollectCoinsDummyData(string rawData)
	{
		LoadCustomizationData(rawData);
	}

	public static Dictionary<int, CollectCoinsDummy> GetDummyData()
	{
		if (_initialized)
		{
			return _dummyData;
		}
		Debug.LogError("Collect Coins Dummies not initialized!");
		return null;
	}

	private void LoadCustomizationData(string rawDummyData)
	{
		_dummyData = ParseDummyData(rawDummyData);
		_initialized = true;
	}

	private Dictionary<int, CollectCoinsDummy> ParseDummyData(string rawData)
	{
		Dictionary<int, CollectCoinsDummy> dictionary = new Dictionary<int, CollectCoinsDummy>();
		if (!string.IsNullOrEmpty(rawData))
		{
			string[] array = rawData.Split(';');
			int num = 0;
			string[] array2 = array;
			foreach (string rawDummyData in array2)
			{
				CollectCoinsDummy collectCoinsDummy = new CollectCoinsDummy();
				if (collectCoinsDummy.ParseDummy(rawDummyData))
				{
					dictionary.Add(num, collectCoinsDummy);
					num++;
					continue;
				}
				return _defaultDummyList;
			}
			return dictionary;
		}
		return _defaultDummyList;
	}

	public void DummyCollected(int index)
	{
		_dummyData[index].Collected();
		_dirty = true;
		PlayerInfo.Instance.SaveIfDirty();
	}

	public string ToSerializedString()
	{
		string text = string.Empty;
		foreach (KeyValuePair<int, CollectCoinsDummy> dummyDatum in _dummyData)
		{
			if (text != string.Empty)
			{
				text += ';';
			}
			text += dummyDatum.Value.ToString();
		}
		return text;
	}
}
