using System;

public abstract class ScreenAdProvider
{
	public abstract bool isInitialized { get; }

	public abstract bool HasCachedInterstials { get; }

	public abstract bool isScreenAdProviderSupported { get; }

	public abstract string id { get; }

	public event Action<ScreenAdProvider> onDidCacheInterstitial;

	public event Action<ScreenAdProvider> onDidFailToLoadInterstitial;

	public event Action<ScreenAdProvider> onDidDismissInterstitial;

	public event Action<ScreenAdProvider> onDidShowInterstitial;

	public event Action<ScreenAdProvider> onDidFailToShowInterstitial;

	public event Action<ScreenAdProvider> onDidClickInterstitial;

	public abstract void CacheNowInterstitials();

	public abstract void Init();

	public abstract void ShowInterstitial();

	public void OnDidCacheInterstitial(ScreenAdProvider provider)
	{
		Action<ScreenAdProvider> action = this.onDidCacheInterstitial;
		if (action != null)
		{
			action(provider);
		}
	}

	public void OnDidFailToLoadInterstitial(ScreenAdProvider provider)
	{
		Action<ScreenAdProvider> action = this.onDidFailToLoadInterstitial;
		if (action != null)
		{
			action(provider);
		}
	}

	public void OnDidDismissInterstitial(ScreenAdProvider provider)
	{
		Action<ScreenAdProvider> action = this.onDidDismissInterstitial;
		if (action != null)
		{
			action(provider);
		}
	}

	public void OnDidShowInterstitial(ScreenAdProvider provider)
	{
		Action<ScreenAdProvider> action = this.onDidShowInterstitial;
		if (action != null)
		{
			action(provider);
		}
	}

	public void OnDidClickInterstitial(ScreenAdProvider provider)
	{
		Action<ScreenAdProvider> action = this.onDidClickInterstitial;
		if (action != null)
		{
			action(provider);
		}
	}

	public void OnDidFailToShowInterstitial(ScreenAdProvider provider)
	{
		Action<ScreenAdProvider> action = this.onDidFailToShowInterstitial;
		if (action != null)
		{
			action(provider);
		}
	}
}
