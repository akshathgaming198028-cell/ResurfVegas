using System;
using System.Collections.Generic;

namespace Kiloo.Monetization.Offerwall
{
	public interface IOfferWallProvider
	{
		OfferWallProviderID providerID { get; }

		event Action<List<IDisplayableOffer>, bool> onOffersReceived;

		event Action onOffersRequestFailed;

		event Action<int> onPendingRewardsReceived;

		event Action onPendingRewardsRequestFailed;

		event Action<OffersResponseData> notifyOffersReceivedVSOffersRequested;

		void InitProvider(IOfferWallDataManager dataManager);

		void RefreshOffers(bool bypassTimeLimitsEnforcedByOutside = false);

		void GetPendingRewards(bool bypassTimeLimitsEnforcedByOutside = false);

		void UnloadProvider();
	}
}
