namespace Kiloo.Plugins.MdotM.Internal
{
	public interface IMdotMPlatformBridge
	{
		event MdotMLocationCallback didCacheInterstitial;

		event MdotMLocationCallback didFailToLoadInterstitial;

		event MdotMLocationCallback didDismissInterstitial;

		event MdotMLocationCallback didCloseInterstitial;

		event MdotMLocationCallback didClickInterstitial;

		event MdotMLocationCallback didShowInterstitial;

		void StartSession(string appId);

		void CacheInterstitial();

		void ShowInterstitial();

		bool HasCachedInterstitial();

		void SetShouldRequestInterstitial(bool val);

		void SetShouldDisplayInterstitial(bool val);

		void SetShouldRequestInterstitialsInFirstSession(bool val);
	}
}
