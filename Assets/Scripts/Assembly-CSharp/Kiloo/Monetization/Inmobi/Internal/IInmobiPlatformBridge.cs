namespace Kiloo.Monetization.Inmobi.Internal
{
	public interface IInmobiPlatformBridge
	{
		event InmobiCallback didCacheAd;

		event InmobiCallback didFailToLoadAd;

		event InmobiCallback didDismissAd;

		event InmobiCallback willShowAd;

		event InmobiCallback didCompleteIncentive;

		void Init(string appId);

		void CacheAd(string key, string appId);

		void ShowAd(string key, string appId);

		bool HasCachedAd(string key, string appId);
	}
}
