using System;
using System.IO;
using UnityEngine;

namespace SYBO.Subway.Coins
{
	public class CoinSystemSpawnLog
	{
		public static CoinSystemSpawnLogPair Old;

		public static CoinSystemSpawnLogPair New;

		private StreamWriter _writer;

		private string _path;

		private CoinSystemSpawnLog(StreamWriter writer, string path)
		{
			_writer = writer;
			_path = path;
		}

		public static void CreateLogs()
		{
			string text = DateTime.Now.ToLongTimeString().Replace(":", "-");
			DirectoryInfo directoryInfo = new DirectoryInfo(Application.dataPath);
			string text2 = Path.Combine(directoryInfo.Parent.FullName, "CoinSpawnLog_" + text);
			if (!Directory.Exists(text2))
			{
				Directory.CreateDirectory(text2);
			}
			CoinSystemSpawnLogPair coinSystemSpawnLogPair = new CoinSystemSpawnLogPair();
			coinSystemSpawnLogPair.Simple = CreateNew("OldSimple", text2);
			coinSystemSpawnLogPair.Detailed = CreateNew("OldDetailed", text2);
			coinSystemSpawnLogPair.Stats = CoinStats.Old;
			Old = coinSystemSpawnLogPair;
			coinSystemSpawnLogPair = new CoinSystemSpawnLogPair();
			coinSystemSpawnLogPair.Simple = CreateNew("NewSimple", text2);
			coinSystemSpawnLogPair.Detailed = CreateNew("NewDetailed", text2);
			coinSystemSpawnLogPair.Stats = CoinStats.New;
			New = coinSystemSpawnLogPair;
		}

		private static CoinSystemSpawnLog CreateNew(string runName, string directory)
		{
			string path = string.Format("CoinSpawnLog-{0}.txt", runName);
			string text = Path.Combine(directory, path);
			StreamWriter writer = File.CreateText(text);
			CoinSystemSpawnLog result = new CoinSystemSpawnLog(writer, text);
			Debug.Log("Created Coin System Spawn Log at: " + text);
			return result;
		}

		public void Log(string str)
		{
			_writer.WriteLine(str);
			_writer.Flush();
		}

		public void Close()
		{
			Debug.Log("Wrote log to: " + _path);
			_writer.Close();
		}
	}
}
