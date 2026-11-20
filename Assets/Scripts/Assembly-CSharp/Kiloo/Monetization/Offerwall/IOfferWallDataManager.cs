using System.Collections.Generic;

namespace Kiloo.Monetization.Offerwall
{
	public interface IOfferWallDataManager
	{
		bool isOfferWallDisabled { get; }

		int offersRefreshIntervalAfterSuccess { get; }

		int offersRefreshIntervalAfterFail { get; }

		int rewardsRefreshIntervalAfterSuccess { get; }

		int rewardsRefreshIntervalAfterFail { get; }

		int requestTimeout { get; }

		string externalIpUrl { get; }

		int GetNumberOfOffersToRequest();

		string GetSDKAppID(OfferWallProviderID providerID);

		string GetSDKAppKey(OfferWallProviderID providerID);

		string GetSDKEventName(OfferWallProviderID providerID);

		Dictionary<OfferWallFilterType, string> GetFilters();
	}
}
