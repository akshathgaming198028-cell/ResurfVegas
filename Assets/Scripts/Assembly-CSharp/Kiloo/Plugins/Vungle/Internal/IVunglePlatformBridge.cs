using System.Collections;

namespace Kiloo.Plugins.Vungle.Internal
{
	public interface IVunglePlatformBridge
	{
		event VungleAdStartEvent onAdStart;

		event VungleCachedAdAvailableEvent onCachedAdAvailable;

		event VungleVideoViewFinishedEvent onVideoViewFinished;

		event VungleAdUnavailableEvent onAdUnavailable;

		event VungleWillCloseProductSheet onWillCloseProductSheet;

		bool InitVungle(string appId);

		void PlayAd();

		void PlayAdWithOptions(IDictionary optionsData);

		bool IsCachedAdAvailable();

		void OnPause();

		void OnResume();
	}
}
