using System;
using System.Collections.Generic;
using Kiloo.Monetization.Offerwall.TapjoyOfferwall;

namespace Kiloo.Monetization.Offerwall
{
	public class DummyOfferWallProvider : IOfferWallProvider
	{
		public OfferWallProviderID providerID
		{
			get
			{
				return OfferWallProviderID.TapJoy;
			}
		}

		public event Action<List<IDisplayableOffer>, bool> onOffersReceived;

		public event Action onOffersRequestFailed;

		public event Action<int> onPendingRewardsReceived;

		public event Action onPendingRewardsRequestFailed;

		public event Action<OffersResponseData> notifyOffersReceivedVSOffersRequested;

		public void InitProvider(IOfferWallDataManager dataManager)
		{
		}

		public void RefreshOffers(bool bypassTimeLimitsEnforcedByOutside = false)
		{
			List<IDisplayableOffer> list = new List<IDisplayableOffer>();
			for (int i = 1; i <= 4; i++)
			{
				list.Add(new TapjoyOffer("Dummy offer " + i, "500", "Free", "500", string.Empty, string.Empty, string.Empty, string.Empty, "App"));
			}
			if (onOffersReceived != null)
			{
				onOffersReceived(list, false);
			}
		}

		public void GetPendingRewards(bool bypassTimeLimitsEnforcedByOutside = false)
		{
			if (onPendingRewardsReceived != null)
			{
				onPendingRewardsReceived(0);
			}
		}

		public void UnloadProvider()
		{
		}
	}
}
