using UnityEngine;

namespace SYBO.Subway.Coins
{
	public class CoinSystemSpawnLogPair
	{
		public CoinStats Stats;

		public CoinSystemSpawnLog Simple;

		public CoinSystemSpawnLog Detailed;

		public void LogSpawnPoint(GameObject spawnObj, int coins)
		{
			string str = string.Format("SpawnPoint - {0}:", coins);
			Simple.Log(str);
			string str2 = string.Format("SpawnPoint [Name = {0}, Position = {1}, InstanceID = {2}, ActivatedSpawnPoints = {3}] - {4}:", Utils.GetLongName(spawnObj.transform), spawnObj.transform.position, spawnObj.GetInstanceID(), Stats.ActivatedSpawnPoints, coins);
			Detailed.Log(str2);
		}

		public void LogCoin(GameObject coinObj)
		{
			string str = string.Format("    Coin");
			Simple.Log(str);
			string str2 = string.Format("    Coin [Position = {0}, InstanceID = {1}, ParentID = {2}, ActivatedCoins = {3}, Name={4}]", coinObj.transform.position, coinObj.GetInstanceID(), coinObj.transform.parent.gameObject.GetInstanceID(), Stats.ActivatedCoins, Utils.GetLongName(coinObj.transform));
			Detailed.Log(str2);
		}

		public void Close()
		{
			Simple.Close();
			Detailed.Close();
		}
	}
}
