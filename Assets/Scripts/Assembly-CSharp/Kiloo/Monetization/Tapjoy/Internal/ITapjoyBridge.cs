namespace Kiloo.Monetization.Tapjoy.Internal
{
	public interface ITapjoyBridge
	{
		event TapjoyConnectEvent onConnect;

		event TapjoyGetFullScreenAdResponseEvent onGetFullScreenAdResponse;

		event TapjoyViewDidAppearEvent onViewDidAppear;

		event TapjoyViewDidDisappearEvent onViewDidDisappear;

		event TapjoyViewWillAppearEvent onViewWillAppear;

		event TapjoyViewWillDisappearEvent onViewWillDisappear;

		event TapjoyVideoStartEvent onVideoStart;

		event TapjoyVideoErrorEvent onVideoError;

		event TapjoyVideoCompleteEvent onVideoComplete;

		event TapjoyEarnedPointsEvent onEarnedPoints;

		event TapjoyGetPointsEvent onGetPoints;

		event TapjoyGetPointsErrorEvent onGetPointsError;

		event TapjoySpendPointsEvent onSpendPoints;

		event TapjoySpendPointsErrorEvent onSpendPointsError;

		void Initialize(string appID, string appKey, string eventName);

		void GetFullScreenAd();

		void ShowFullScreenAd();

		void GetPoints();

		void SpendPoints(int amount);
	}
}
