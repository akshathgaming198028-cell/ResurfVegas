namespace Kiloo.Monetization.Offerwall.TapjoyOfferwall
{
	public class OfferwallRequest
	{
		public enum DeviceType
		{
			iphone = 0,
			itouch = 1,
			ipad = 2,
			android = 3,
			windows = 4
		}

		private const string LIBRARY_VERSION = "server";

		private const string SDK_TYPE = "offers";

		private const string REQUEST_PREFIX = "https://ws.tapjoyads.com/get_offers?";

		public string appID { get; private set; }

		public string udid { get; private set; }

		public string advertisingID { get; private set; }

		public string libraryVersion { get; private set; }

		public string deviceIP { get; private set; }

		public string timestamp { get; private set; }

		public string verifier { get; private set; }

		public string deviceType { get; private set; }

		public string sdkType { get; private set; }

		public string appVersion { get; private set; }

		public string publisherUserID { get; private set; }

		public string osVersion { get; private set; }

		public int max { get; private set; }

		public OfferwallRequest(string appID, string udid, string advertisingID, string deviceIP, string timestamp, string verifier, string appVersion, string publisherUserID, string osVersion, int max)
		{
			this.appID = appID;
			this.udid = udid;
			this.advertisingID = advertisingID;
			this.deviceIP = deviceIP;
			this.timestamp = timestamp;
			this.verifier = verifier;
			this.appVersion = appVersion;
			this.publisherUserID = publisherUserID;
			this.osVersion = osVersion;
			this.max = max;
			libraryVersion = "server";
			sdkType = "offers";
			deviceType = DeviceType.android.ToString();
		}

		public string FormatToString()
		{
			return string.Format("{0}app_id={1}&udid={2}&advertising_id={3}&library_version={4}&device_ip={5}&timestamp={6}&verifier={7}&device_type={8}&sdk_type={9}&app_version={10}&publisher_user_id={11}&os_version={12}&max={13}&json={14}", "https://ws.tapjoyads.com/get_offers?", appID, udid, advertisingID, libraryVersion, deviceIP, timestamp, verifier, deviceType, sdkType, appVersion, publisherUserID, osVersion, max, 1);
		}
	}
}
