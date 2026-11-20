using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Kiloo.Common;
using Kiloo.HTTP;
using Kiloo.Monetization.Offerwall.Internal;
using Kiloo.Monetization.Tapjoy;
using Kiloo.Plugins.DeviceUtility;
using MiniJSON;
using UnityEngine;

namespace Kiloo.Monetization.Offerwall.TapjoyOfferwall
{
	public class TapJoyOfferWallProvider : IOfferWallProvider
	{
		private enum TapJoyOfferwallConnectionState
		{
			Disconnected = 0,
			Connecting = 1,
			Connected = 2
		}

		private enum RequestStates
		{
			NotStarted = 0,
			InProgress = 1,
			Succeeded = 2,
			Failed = 3
		}

		private DateTime _lastTimeOffersWereRequested = DateTime.MinValue;

		private DateTime _lastTimeRewardsWereRequested = DateTime.MinValue;

		private TapJoyOfferwallConnectionState _offerWallConnectionState;

		private List<IDisplayableOffer> _downloadedOffers = new List<IDisplayableOffer>();

		private int _amountPendingApprovalToBeRewarded;

		private int _amountApprovedToBeRewarded;

		private string _deviceExternalIP = string.Empty;

		private IOfferWallDataManager _dataManager;

		private RequestStates _requestOffersState;

		private RequestStates _requestRewardState;

		private RequestStates _spendRewardState;

		private RequestStates _requestExternalIpState;

		public OfferWallProviderID providerID
		{
			get
			{
				return OfferWallProviderID.TapJoy;
			}
		}

		private bool allowsSDKInitialize
		{
			get
			{
				return isManagerSetUp && !isSDKInitialized && !isSDKInitializing && IsSupportedOnPlatform();
			}
		}

		private bool isSDKInitialized
		{
			get
			{
				return _offerWallConnectionState == TapJoyOfferwallConnectionState.Connected;
			}
		}

		private bool isSDKInitializing
		{
			get
			{
				return _offerWallConnectionState == TapJoyOfferwallConnectionState.Connecting;
			}
		}

		private bool isManagerSetUp
		{
			get
			{
				return _dataManager != null;
			}
		}

		public event Action<List<IDisplayableOffer>, bool> onOffersReceived;

		public event Action onOffersRequestFailed;

		public event Action<int> onPendingRewardsReceived;

		public event Action onPendingRewardsRequestFailed;

		public event Action<OffersResponseData> notifyOffersReceivedVSOffersRequested;

		public bool IsSupportedOnPlatform()
		{
			return Application.platform == RuntimePlatform.Android && !DeviceUtility.isKindleDevice;
		}

		public void InitProvider(IOfferWallDataManager dataManager)
		{
			_dataManager = dataManager;
		}

		public void RefreshOffers(bool bypassTimeLimitsEnforcedByOutside = false)
		{
			if (isManagerSetUp)
			{
				bool flag = bypassTimeLimitsEnforcedByOutside || _offerWallConnectionState == TapJoyOfferwallConnectionState.Disconnected;
				int timeInterval = ((_requestOffersState != RequestStates.Succeeded) ? _dataManager.offersRefreshIntervalAfterFail : _dataManager.offersRefreshIntervalAfterSuccess);
				if (flag || HasTimeIntervalPassedSinceDate(_lastTimeOffersWereRequested, timeInterval))
				{
					CoroutineUtil.StartCoroutine(RefreshOffersCoroutine(_dataManager.GetNumberOfOffersToRequest()));
				}
				else if (_downloadedOffers != null && _downloadedOffers.Count > 0)
				{
					Action<List<IDisplayableOffer>, bool> action = this.onOffersReceived;
					if (action != null)
					{
						action(_downloadedOffers, true);
					}
				}
				else
				{
					Action action2 = this.onOffersRequestFailed;
					if (action2 != null)
					{
						action2();
					}
				}
			}
			else
			{
				LogError("RefreshOffers should not be called before setting up the Offerwall");
				_downloadedOffers.Clear();
				Action action3 = this.onOffersRequestFailed;
				if (action3 != null)
				{
					action3();
				}
			}
		}

		public void GetPendingRewards(bool bypassTimeLimitsEnforcedByOutside = false)
		{
			if (isManagerSetUp)
			{
				bool flag = bypassTimeLimitsEnforcedByOutside || _offerWallConnectionState == TapJoyOfferwallConnectionState.Disconnected;
				int timeInterval = ((_requestRewardState != RequestStates.Succeeded || _spendRewardState != RequestStates.Succeeded) ? _dataManager.rewardsRefreshIntervalAfterFail : _dataManager.rewardsRefreshIntervalAfterSuccess);
				if (flag || HasTimeIntervalPassedSinceDate(_lastTimeRewardsWereRequested, timeInterval))
				{
					CoroutineUtil.StartCoroutine(GetPendingRewardsCoroutine());
				}
				else if (_amountApprovedToBeRewarded >= 0)
				{
					Action<int> action = this.onPendingRewardsReceived;
					if (action != null)
					{
						action(_amountApprovedToBeRewarded);
					}
					_amountApprovedToBeRewarded = 0;
				}
				else
				{
					LogError("The amount approved to be rewarded to the user is: " + _amountApprovedToBeRewarded + ". It should never be below 0!");
					Action action2 = this.onPendingRewardsRequestFailed;
					if (action2 != null)
					{
						action2();
					}
				}
			}
			else
			{
				LogError("GetPendingRewards should not be called before setting up the Offerwall");
				Action action3 = this.onPendingRewardsRequestFailed;
				if (action3 != null)
				{
					action3();
				}
			}
		}

		public void UnloadProvider()
		{
			TapJoy.onInitializeFinished -= InitializeFinished;
			TapJoy.onEarnPoints -= EarnedPoints;
			TapJoy.onGetPoints -= GotPoints;
			TapJoy.onGetPointsError -= GetPointsError;
			TapJoy.onSpendPoints -= SpentPoints;
			TapJoy.onSpendPointsError -= SpendPointsError;
		}

		private void InitializeFinished(string result)
		{
			if (result == "success")
			{
				_offerWallConnectionState = TapJoyOfferwallConnectionState.Connected;
			}
			else
			{
				_offerWallConnectionState = TapJoyOfferwallConnectionState.Disconnected;
			}
		}

		private void EarnedPoints(string amount)
		{
		}

		private void GotPoints(string amount)
		{
			int result = 0;
			if (int.TryParse(amount, out result) && result > 0)
			{
				_amountPendingApprovalToBeRewarded = result;
			}
			_requestRewardState = RequestStates.Succeeded;
		}

		private void GetPointsError()
		{
			_requestRewardState = RequestStates.Failed;
		}

		private void SpentPoints(string amountOfPointsLeftInUsersBalance)
		{
			_amountApprovedToBeRewarded += _amountPendingApprovalToBeRewarded;
			_amountPendingApprovalToBeRewarded = 0;
			_spendRewardState = RequestStates.Succeeded;
		}

		private void SpendPointsError()
		{
			_spendRewardState = RequestStates.Failed;
		}

		private void InitializeSDK()
		{
			if (allowsSDKInitialize)
			{
				_offerWallConnectionState = TapJoyOfferwallConnectionState.Connecting;
				TapJoy.onInitializeFinished += InitializeFinished;
				TapJoy.onEarnPoints += EarnedPoints;
				TapJoy.onGetPoints += GotPoints;
				TapJoy.onGetPointsError += GetPointsError;
				TapJoy.onSpendPoints += SpentPoints;
				TapJoy.onSpendPointsError += SpendPointsError;
				if (TapJoy.isInitialized)
				{
					InitializeFinished("success");
				}
				else
				{
					TapJoy.Initialize(_dataManager.GetSDKAppID(providerID), _dataManager.GetSDKAppKey(providerID), _dataManager.GetSDKEventName(providerID));
				}
			}
		}

		private IEnumerator RefreshOffersCoroutine(int amountOfOffersToRequest)
		{
			if (_requestOffersState == RequestStates.InProgress)
			{
				yield break;
			}
			_downloadedOffers.Clear();
			_requestOffersState = RequestStates.InProgress;
			InitializeSDK();
			while (!isSDKInitialized)
			{
				yield return null;
				if (_offerWallConnectionState == TapJoyOfferwallConnectionState.Disconnected)
				{
					_downloadedOffers.Clear();
					Action handler = this.onOffersRequestFailed;
					if (handler != null)
					{
						handler();
					}
					_requestOffersState = RequestStates.Failed;
					yield break;
				}
			}
			PostExternalIpRequest();
			while (_requestExternalIpState != RequestStates.Succeeded || string.IsNullOrEmpty(_deviceExternalIP))
			{
				yield return null;
				if (_requestExternalIpState == RequestStates.Failed || (_requestExternalIpState == RequestStates.Succeeded && string.IsNullOrEmpty(_deviceExternalIP)))
				{
					_downloadedOffers.Clear();
					Action handler2 = this.onOffersRequestFailed;
					if (handler2 != null)
					{
						handler2();
					}
					_requestOffersState = RequestStates.Failed;
					yield break;
				}
			}
			MakeOfferwallRequest(amountOfOffersToRequest);
			while (_requestOffersState != RequestStates.Succeeded)
			{
				yield return null;
				if (_requestOffersState == RequestStates.Failed)
				{
					_downloadedOffers.Clear();
					Action handler3 = this.onOffersRequestFailed;
					if (handler3 != null)
					{
						handler3();
					}
					_requestOffersState = RequestStates.Failed;
					yield break;
				}
			}
			if (_downloadedOffers != null && _downloadedOffers.Count > 0)
			{
				OfferWallImagesDownloader<IDisplayableOffer>.DownloadImagesForOffers(_downloadedOffers);
				Action<List<IDisplayableOffer>, bool> offersReceivedHandler = this.onOffersReceived;
				if (offersReceivedHandler != null)
				{
					offersReceivedHandler(_downloadedOffers, false);
				}
			}
			else
			{
				Action handler4 = this.onOffersRequestFailed;
				if (handler4 != null)
				{
					handler4();
				}
				_requestOffersState = RequestStates.Failed;
			}
		}

		private IEnumerator GetPendingRewardsCoroutine()
		{
			if (_requestRewardState == RequestStates.InProgress || _spendRewardState == RequestStates.InProgress)
			{
				yield break;
			}
			_requestRewardState = RequestStates.InProgress;
			InitializeSDK();
			while (!isSDKInitialized)
			{
				yield return null;
				if (_offerWallConnectionState == TapJoyOfferwallConnectionState.Disconnected)
				{
					Action handler = this.onPendingRewardsRequestFailed;
					if (handler != null)
					{
						handler();
					}
					_requestRewardState = RequestStates.Failed;
					yield break;
				}
			}
			_lastTimeRewardsWereRequested = DateTime.Now;
			TapJoy.GetPoints();
			while (_requestRewardState != RequestStates.Succeeded)
			{
				yield return null;
				if (_requestRewardState == RequestStates.Failed)
				{
					Action handler2 = this.onPendingRewardsRequestFailed;
					if (handler2 != null)
					{
						handler2();
					}
					yield break;
				}
			}
			if (_amountPendingApprovalToBeRewarded > 0)
			{
				_spendRewardState = RequestStates.InProgress;
				TapJoy.SpendPoints(_amountPendingApprovalToBeRewarded);
				while (_spendRewardState != RequestStates.Succeeded)
				{
					yield return null;
					if (_spendRewardState == RequestStates.Failed)
					{
						Action handler3 = this.onPendingRewardsRequestFailed;
						if (handler3 != null)
						{
							handler3();
						}
						_requestRewardState = RequestStates.Failed;
						yield break;
					}
				}
				if (_amountApprovedToBeRewarded >= 0)
				{
					Action<int> handler4 = this.onPendingRewardsReceived;
					if (handler4 != null)
					{
						handler4(_amountApprovedToBeRewarded);
					}
					_amountApprovedToBeRewarded = 0;
					yield break;
				}
				Action handler5 = this.onPendingRewardsRequestFailed;
				if (handler5 != null)
				{
					handler5();
				}
				_requestRewardState = RequestStates.Failed;
				LogError("The amount approved to be rewarded to the user is: " + _amountApprovedToBeRewarded + ". It should never be below 0!");
			}
			else if (_amountPendingApprovalToBeRewarded == 0)
			{
				Action<int> handler6 = this.onPendingRewardsReceived;
				if (handler6 != null)
				{
					handler6(0);
				}
			}
			else
			{
				Action handler7 = this.onPendingRewardsRequestFailed;
				if (handler7 != null)
				{
					handler7();
				}
				_requestRewardState = RequestStates.Failed;
				LogError("The amount offered to be rewarded to the user is: " + _amountPendingApprovalToBeRewarded + ". It should never be below 0!");
			}
		}

		private bool HasTimeIntervalPassedSinceDate(DateTime lastRequestDateTime, int timeInterval)
		{
			double totalSeconds = DateTime.Now.Subtract(lastRequestDateTime).TotalSeconds;
			return (double)timeInterval < totalSeconds;
		}

		private List<TapjoyOffer> FilterOffers(List<TapjoyOffer> unfilteredOffers)
		{
			List<TapjoyOffer> list = new List<TapjoyOffer>();
			Dictionary<OfferWallFilterType, string> filters = _dataManager.GetFilters();
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			if (unfilteredOffers != null && filters != null)
			{
				for (int i = 0; i < unfilteredOffers.Count; i++)
				{
					TapjoyOffer tapjoyOffer = unfilteredOffers[i];
					int result;
					int result2;
					if (filters.ContainsKey(OfferWallFilterType.minReward) && int.TryParse(filters[OfferWallFilterType.minReward], out result) && int.TryParse(tapjoyOffer.payoutAmount, out result2) && result2 >= result)
					{
						flag = true;
					}
					int result3;
					int result4;
					if (filters.ContainsKey(OfferWallFilterType.maxReward) && int.TryParse(filters[OfferWallFilterType.maxReward], out result3) && int.TryParse(tapjoyOffer.payoutAmount, out result4) && result3 >= result4)
					{
						flag2 = true;
					}
					if (filters.ContainsKey(OfferWallFilterType.costForInstall) && tapjoyOffer.costForInstall.Equals(filters[OfferWallFilterType.costForInstall], StringComparison.InvariantCultureIgnoreCase))
					{
						flag3 = true;
					}
					if (filters.ContainsKey(OfferWallFilterType.offerType) && tapjoyOffer.offerType.Equals(filters[OfferWallFilterType.offerType], StringComparison.InvariantCultureIgnoreCase))
					{
						flag4 = true;
					}
					if (flag && flag2 && flag3 && flag4)
					{
						list.Add(tapjoyOffer);
					}
				}
			}
			return list;
		}

		private void MakeOfferwallRequest(int amountOfOffersToRequest)
		{
			string sDKAppID = _dataManager.GetSDKAppID(providerID);
			string deviceIdentifier = DeviceUtility.GetDeviceIdentifier();
			string androidAdvertisingID = DeviceUtility.GetAndroidAdvertisingID();
			string deviceExternalIP = _deviceExternalIP;
			string text = DateTime.UtcNow.ToUnixTime().ToString();
			string data = string.Format("{0}:{1}:{2}:{3}", sDKAppID, androidAdvertisingID, text, _dataManager.GetSDKAppKey(providerID));
			string verifier = StringUtility.ToHexString(Checksum.ComputeSHA256(data));
			string bundleVersion = DeviceUtility.GetBundleVersion();
			string oSVersion = DeviceUtility.GetOSVersion();
			PostOfferwallRequest(new OfferwallRequest(sDKAppID, deviceIdentifier, androidAdvertisingID, deviceExternalIP, text, verifier, bundleVersion, deviceIdentifier, oSVersion, amountOfOffersToRequest));
		}

		private void PostOfferwallRequest(OfferwallRequest requestData)
		{
			_lastTimeOffersWereRequested = DateTime.Now;
			IRequest request = WebClient.CreateGet(new Uri(requestData.FormatToString())).WithIgnoreCertificateErrors(true).WithTimeout(_dataManager.requestTimeout);
			request.acceptGzip = false;
			WebClient.SendRequestAsync(request, delegate(IRequest request2, bool isFromCache)
			{
				IResponse response = request2.response;
				string jsonstring = ((response != null) ? response.ToString() : null);
				if (request2.isSuccessful && response != null)
				{
					List<TapjoyOffer> list = DeserializeJsonOffers(jsonstring);
					List<TapjoyOffer> list2 = FilterOffers(list);
					if (this.notifyOffersReceivedVSOffersRequested != null)
					{
						this.notifyOffersReceivedVSOffersRequested(new OffersResponseData
						{
							offersRequested = requestData.max,
							offersReceived = list.Count,
							offersWeCanUse = list2.Count
						});
					}
					if (list2.Count > 0)
					{
						List<IDisplayableOffer> list3 = new List<IDisplayableOffer>();
						for (int i = 0; i < list2.Count; i++)
						{
							list3.Add(list2[i]);
						}
						_downloadedOffers = list3;
						_requestOffersState = RequestStates.Succeeded;
						{
							foreach (TapjoyOffer item in list2)
							{
							}
							return;
						}
					}
					_requestOffersState = RequestStates.Failed;
				}
				else
				{
					LogError("OfferWall request failed: " + request2.statusMessage);
					_requestOffersState = RequestStates.Failed;
				}
			});
		}

		private void PostExternalIpRequest()
		{
			IRequest request = KilooWebClient.CreateGet(new Uri(_dataManager.externalIpUrl)).WithIgnoreCertificateErrors(true).WithTimeout(_dataManager.requestTimeout);
			request.acceptGzip = false;
			_requestExternalIpState = RequestStates.InProgress;
			WebClient.SendRequestAsync(request, delegate(IRequest request2, bool isFromCache)
			{
				IResponse response = request2.response;
				string text = ((response != null) ? response.ToString() : null);
				if (request2.isSuccessful && response != null && !string.IsNullOrEmpty(text))
				{
					_deviceExternalIP = text;
					_requestExternalIpState = RequestStates.Succeeded;
				}
				else
				{
					LogError("Request external IP failed: " + request2.statusMessage);
					_requestExternalIpState = RequestStates.Failed;
				}
			});
		}

		private static List<TapjoyOffer> DeserializeJsonOffers(string jsonstring)
		{
			Dictionary<string, object> dictionary = Json.Deserialize(jsonstring) as Dictionary<string, object>;
			List<TapjoyOffer> result = new List<TapjoyOffer>();
			if (dictionary != null)
			{
				object value;
				if (dictionary.TryGetValue("OfferArray", out value))
				{
					List<object> resultObject = (List<object>)value;
					result = ParseOffersFromList(resultObject);
				}
				else
				{
					LogError("OfferWallResponse dictionary does not contain OfferArray field");
				}
			}
			return result;
		}

		private static List<TapjoyOffer> ParseOffersFromList(List<object> resultObject)
		{
			List<TapjoyOffer> list = null;
			if (resultObject != null)
			{
				list = new List<TapjoyOffer>();
				foreach (object item2 in resultObject)
				{
					if (!(item2 is Dictionary<string, object>))
					{
						continue;
					}
					string empty = string.Empty;
					string empty2 = string.Empty;
					string empty3 = string.Empty;
					string empty4 = string.Empty;
					string empty5 = string.Empty;
					string empty6 = string.Empty;
					string empty7 = string.Empty;
					string storeID = string.Empty;
					int result = 0;
					Dictionary<string, object> dictionary = item2 as Dictionary<string, object>;
					if (dictionary == null || !dictionary.ContainsKey("Name"))
					{
						continue;
					}
					empty = (string)dictionary["Name"];
					if (string.IsNullOrEmpty(empty) || !dictionary.ContainsKey("Cost"))
					{
						continue;
					}
					empty2 = (string)dictionary["Cost"];
					if (string.IsNullOrEmpty(empty2) || !dictionary.ContainsKey("Amount"))
					{
						continue;
					}
					empty3 = (string)dictionary["Amount"];
					if (string.IsNullOrEmpty(empty3) || !dictionary.ContainsKey("RedirectURL"))
					{
						continue;
					}
					empty4 = (string)dictionary["RedirectURL"];
					if (string.IsNullOrEmpty(empty4) || !dictionary.ContainsKey("IconURL"))
					{
						continue;
					}
					empty5 = (string)dictionary["IconURL"];
					if (string.IsNullOrEmpty(empty5) || !dictionary.ContainsKey("IconURLMedium"))
					{
						continue;
					}
					empty6 = (string)dictionary["IconURLMedium"];
					if (string.IsNullOrEmpty(empty6) || !dictionary.ContainsKey("Type"))
					{
						continue;
					}
					empty7 = (string)dictionary["Type"];
					if (!string.IsNullOrEmpty(empty7))
					{
						if (dictionary.ContainsKey("Payout"))
						{
							int.TryParse(dictionary["Payout"].ToString(), out result);
						}
						if (dictionary.ContainsKey("StoreID"))
						{
							storeID = (string)dictionary["StoreID"];
						}
						TapjoyOffer item = new TapjoyOffer(empty, empty3, empty2, result.ToString(), empty5, empty6, empty4, storeID, empty7);
						list.Add(item);
					}
				}
			}
			return list;
		}

		private static void LogError(string msg, UnityEngine.Object context = null)
		{
			Debug.LogError("TapJoyOfferWallProvider: " + msg, context);
		}

		private static void LogWarning(string msg, UnityEngine.Object context = null)
		{
			Debug.LogWarning("TapJoyOfferWallProvider: " + msg, context);
		}

		[Conditional("OFFERWALL_DEBUG_LOGS")]
		private static void Log(string msg, UnityEngine.Object context = null)
		{
			Debug.Log("TapJoyOfferWallProvider: " + msg, context);
		}

		[Conditional("OFFERWALL_DEBUG_LOGS")]
		public static void PrintRequestResponse(string response)
		{
			int num = 1000;
			int num2 = Mathf.CeilToInt(response.Length / num);
			for (int i = 0; i <= num2; i++)
			{
				int num3 = i * num;
				int num4 = (i + 1) * num;
				if (num4 < response.Length)
				{
				}
			}
		}
	}
}
