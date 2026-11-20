using SYBO.Core.Components.Singleton;
using SYBO.Subway.Pickups.Log;
using UnityEngine;

namespace SYBO.Subway.Pickups.Migration
{
	public class PickupsMigration : MonoBehaviour
	{
		public static bool Migration_IsActivePickupSystemVersion2015
		{
			get
			{
				return PickupManager2.ActivePickupSystemVersion == ActivePickupSystemVersion.Version2015 || PickupManager2.ActivePickupSystemVersion == ActivePickupSystemVersion.Version2014And2015;
			}
		}

		public static bool Migration_IsActivePickupSystemVersion2014
		{
			get
			{
				return PickupManager2.ActivePickupSystemVersion == ActivePickupSystemVersion.Version2014 || PickupManager2.ActivePickupSystemVersion == ActivePickupSystemVersion.Version2014And2015;
			}
		}

		public static void Log2014PickupSpawnInNewLogger(PickupType pickupType)
		{
			if (SingletonMonobehavior<PickupLogManager>.IsInstantiated() && Migration_IsActivePickupSystemVersion2014 && SingletonMonobehavior<PickupManager2>.Instance.HasPickupDataForPickupType(pickupType))
			{
				SingletonMonobehavior<PickupLogManager>.Instance.Log(new PickupLogData(PickupLogType.PICKUP_SPAWN, SingletonMonobehavior<PickupManager2>.Instance.GetPickupDataForPickupType(pickupType), SpawnPointMode.Normal));
			}
		}

		public static PickupType Migration_ConvertFromPickupTypeName2014(string pickupTypeName)
		{
			PickupType pickupType = PickupType.LetterHuntDaily;
			switch (pickupTypeName)
			{
			case "doubleScoreMultiplier":
				return PickupType.MultiplierScore2x;
			case "jumpBooster":
				return PickupType.SuperSneakers;
			case "jetpackPickup":
			case "jetpack pickup":
				return PickupType.JetPack;
			case "dailyLetter":
				return PickupType.LetterHuntDaily;
			case "characterLetter":
				return PickupType.LetterHuntCharacter;
			case "magnetBooster":
				return PickupType.CoinMagnet;
			case "mysteryBox":
				return PickupType.MysteryBox;
			case "mysteryPowerup":
				return PickupType.MysteryPowerup;
			case "saveMeToken":
				return PickupType.Key;
			default:
				throw new UnityException("No PickupType found for pickupTypeName: " + pickupTypeName);
			}
		}
	}
}
