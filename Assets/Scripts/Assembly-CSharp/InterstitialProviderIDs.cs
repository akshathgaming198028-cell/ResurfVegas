using System.Collections.Generic;

public static class InterstitialProviderIDs
{
	public const string INMOBI = "inmobi";

	public const string CHARTBOOST = "chartboost";

	public const string MDOTM = "mdotm";

	public static List<string> allValidProviderIDs = new List<string> { "inmobi", "chartboost", "mdotm" };

	private static List<string> debugProviderIDs = new List<string>();

	public static List<string> GetDebugProviderIDs()
	{
		debugProviderIDs.Clear();
		if (Globals.DEBUG_INTERSTITIAL_PROVIDERS)
		{
			if (Globals.DEBUG_INTERSTITIAL_PROVIDER_INMOBI && !debugProviderIDs.Contains("inmobi"))
			{
				debugProviderIDs.Add("inmobi");
			}
			if (Globals.DEBUG_INTERSTITIAL_PROVIDER_CHARTBOOST && !debugProviderIDs.Contains("chartboost"))
			{
				debugProviderIDs.Add("chartboost");
			}
			if (Globals.DEBUG_INTERSTITIAL_PROVIDER_MDOTM && !debugProviderIDs.Contains("mdotm"))
			{
				debugProviderIDs.Add("mdotm");
			}
		}
		return debugProviderIDs;
	}
}
