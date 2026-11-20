namespace Kiloo.Plugins.AdColony.Internal
{
	public interface IAdColonyPlatformBridge
	{
		event AdColonyV4VCRewardEvent onV4VCReward;

		event AdColonyAdAttemptFinishEvent onAdAttemptFinished;

		event AdColonyAdStartEvent onAdStarted;

		void Configure(string appID, string[] zoneIDs);

		void PlayVideoAdForZoneWithPopups(string zoneID, bool prePopupV4VC, bool postPopupV4VC);

		bool IsVirtualCurrencyRewardAvailable(string zoneID);
	}
}
