using Kiloo.AdConstants.Internal;
using Kiloo.Plugins.DeviceUtility;
using UnityEngine;

public class AdConstants : IAdConstants
{
	private IAdConstants _constants;

	private static AdConstants _instance;

	public static AdConstants instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new AdConstants();
			}
			return _instance;
		}
	}

	public string FlurryApiKey
	{
		get
		{
			return _constants.FlurryApiKey;
		}
	}

	public string FlurryAdSpace
	{
		get
		{
			return _constants.FlurryAdSpace;
		}
	}

	public string AdColonyAppId
	{
		get
		{
			return _constants.AdColonyAppId;
		}
	}

	public string AdColonyZoneId
	{
		get
		{
			return _constants.AdColonyZoneId;
		}
	}

	public string TapJoyAppId
	{
		get
		{
			return _constants.TapJoyAppId;
		}
	}

	public string TapJoyAppKey
	{
		get
		{
			return _constants.TapJoyAppKey;
		}
	}

	public string TapJoyEventName
	{
		get
		{
			return _constants.TapJoyEventName;
		}
	}

	public string ChartboostAppId
	{
		get
		{
			return _constants.ChartboostAppId;
		}
	}

	public string ChartboostAppSignature
	{
		get
		{
			return _constants.ChartboostAppSignature;
		}
	}

	public string InmobiAppIdInterstitial
	{
		get
		{
			return _constants.InmobiAppIdInterstitial;
		}
	}

	public string InmobiAppIdRewardedVideo
	{
		get
		{
			return _constants.InmobiAppIdRewardedVideo;
		}
	}

	public string VungleAppId
	{
		get
		{
			return _constants.VungleAppId;
		}
	}

	public string MdotmAppId
	{
		get
		{
			return _constants.MdotmAppId;
		}
	}

	private AdConstants()
	{
		SetPlatformDependentConstants();
	}

	private void SetPlatformDependentConstants()
	{
		switch (Application.platform)
		{
		case RuntimePlatform.IPhonePlayer:
			if (Globals.DEBUG_USE_TEST_AD_CONSTANTS)
			{
				_constants = new AdConstantsIOS_TEST();
			}
			else
			{
				_constants = new AdConstantsIOS();
			}
			break;
		case RuntimePlatform.Android:
			if (DeviceUtility.isKindleDevice)
			{
				if (Globals.DEBUG_USE_TEST_AD_CONSTANTS)
				{
					_constants = new AdConstantsKindle_TEST();
				}
				else
				{
					_constants = new AdConstantsKindle();
				}
			}
			else if (DeviceUtility.isFlexionDevice)
			{
				if (Globals.DEBUG_USE_TEST_AD_CONSTANTS)
				{
					_constants = new AdConstantsFlexion_TEST();
				}
				else
				{
					_constants = new AdConstantsFlexion();
				}
			}
			else if (Globals.DEBUG_USE_TEST_AD_CONSTANTS)
			{
				_constants = new AdConstantsAndroid_TEST();
			}
			else
			{
				_constants = new AdConstantsAndroid();
			}
			break;
		case RuntimePlatform.WP8Player:
			if (Globals.DEBUG_USE_TEST_AD_CONSTANTS)
			{
				_constants = new AdConstantsWP_TEST();
			}
			else
			{
				_constants = new AdConstantsWP();
			}
			break;
		default:
			_constants = new AdConstantsEditor();
			break;
		}
	}
}
