namespace Kiloo.Monetization.Flurry.Internal
{
	public interface IFlurryVideoAds
	{
		event FlurryAdsCallback onAdClosedEvent;

		event FlurryAdsCallback onApplicationExitEvent;

		event FlurryAdsCallback onRenderFailedEvent;

		event FlurryAdsCallback onSpaceDidFailToReceiveAdEvent;

		event FlurryAdsCallback onSpaceDidReceiveAdEvent;

		event FlurryAdsCallback onAdClickedEvent;

		event FlurryAdsCallback onAdOpenedEvent;

		event FlurryAdsCallback onVideoCompletedEvent;

		void Initialize(string adSpace);

		bool IsReady();

		void FetchAd();

		void DisplayAd();

		void EnableDebugLogs();

		void EnableTestAds();
	}
}
