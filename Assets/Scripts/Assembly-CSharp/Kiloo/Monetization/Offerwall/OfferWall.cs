using System;
using System.Collections.Generic;
using System.Diagnostics;
using Kiloo.Plugins.DeviceUtility;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kiloo.Monetization.Offerwall
{
	public class OfferWall
	{
		private List<IOfferWallProvider> _providers = new List<IOfferWallProvider>();

		private IOfferWallProvider _activeProvider;

		private IOfferWallDataManager _dataManager;

		public event Action<List<IDisplayableOffer>, bool> onOffersReceived;

		public event Action onOffersRequestFailed;

		public event Action<int> onPendingRewardsReceived;

		public event Action onPendingRewardsRequestFailed;

		public event Action<OffersResponseData> notifyOffersReceivedVSOffersRequested;

		public bool AreOffersAllowedOnDevice()
		{
			return IsSupportedOnPlatform() && !IsOfferWallDisabled();
		}

		public void Initialize(IOfferWallDataManager dataManager, List<IOfferWallProvider> possibleProviders)
		{
			_dataManager = dataManager;
			_providers = possibleProviders;
			if (AreOffersAllowedOnDevice())
			{
				if (dataManager != null && possibleProviders != null)
				{
					ChangeActiveProvider();
				}
				else
				{
					LogError("The OfferWall cannot be initialized due to faulty parameters!");
				}
			}
		}

		public void RefreshOffers(bool bypassTimeLimitsEnforcedByOutside = false)
		{
			if (AreOffersAllowedOnDevice())
			{
				if (IsInitialized())
				{
					_activeProvider.RefreshOffers(bypassTimeLimitsEnforcedByOutside);
				}
				else
				{
					LogError("The OfferWall needs initializing before it can RefreshOffers.");
				}
			}
		}

		public void GetPendingRewards(bool bypassTimeLimitsEnforcedByOutside = false)
		{
			if (AreOffersAllowedOnDevice())
			{
				if (IsInitialized())
				{
					_activeProvider.GetPendingRewards(bypassTimeLimitsEnforcedByOutside);
				}
				else
				{
					LogError("The OfferWall needs initializing before it can GetPendingRewards.");
				}
			}
		}

		private bool IsInitialized()
		{
			return _dataManager != null && _activeProvider != null;
		}

		private bool IsOfferWallDisabled()
		{
			bool result = false;
			if (_dataManager != null)
			{
				result = _dataManager.isOfferWallDisabled;
			}
			else
			{
				LogError("The OfferWall needs to be initialized first!");
			}
			return result;
		}

		private bool IsSupportedOnPlatform()
		{
			return (Application.platform == RuntimePlatform.Android && !DeviceUtility.isKindleDevice) || Application.isEditor;
		}

		private void ChangeActiveProvider()
		{
			IOfferWallProvider offerWallProvider = null;
			if (_providers.Count > 0)
			{
				offerWallProvider = _providers[0];
			}
			if (offerWallProvider != null)
			{
				SetUpActiveProvider(offerWallProvider);
			}
		}

		private void SetUpActiveProvider(IOfferWallProvider newProvider)
		{
			if (_activeProvider != null && (newProvider == null || _activeProvider.providerID != newProvider.providerID))
			{
				_activeProvider.onOffersReceived -= this.onOffersReceived;
				_activeProvider.onOffersRequestFailed -= this.onOffersRequestFailed;
				_activeProvider.onPendingRewardsReceived -= this.onPendingRewardsReceived;
				_activeProvider.onPendingRewardsRequestFailed -= this.onPendingRewardsRequestFailed;
				_activeProvider.notifyOffersReceivedVSOffersRequested -= this.notifyOffersReceivedVSOffersRequested;
				_activeProvider.UnloadProvider();
			}
			_activeProvider = newProvider;
			if (_activeProvider != null)
			{
				_activeProvider.onOffersReceived += OnOffersReceived;
				_activeProvider.onOffersRequestFailed += this.onOffersRequestFailed;
				_activeProvider.onPendingRewardsReceived += this.onPendingRewardsReceived;
				_activeProvider.onPendingRewardsRequestFailed += this.onPendingRewardsRequestFailed;
				_activeProvider.notifyOffersReceivedVSOffersRequested += this.notifyOffersReceivedVSOffersRequested;
				_activeProvider.InitProvider(_dataManager);
			}
		}

		private void OnOffersReceived(List<IDisplayableOffer> offers, bool cached)
		{
			if (this.onOffersReceived != null)
			{
				this.onOffersReceived(offers, cached);
			}
		}

		private static void LogError(string msg, UnityEngine.Object context = null)
		{
			Debug.LogError("OfferWall: " + msg, context);
		}

		private static void LogWarning(string msg, UnityEngine.Object context = null)
		{
			Debug.LogWarning("OfferWall: " + msg, context);
		}

		[Conditional("OFFERWALL_DEBUG_LOGS")]
		private static void Log(string msg, UnityEngine.Object context = null)
		{
			Debug.Log("OfferWall: " + msg, context);
		}
	}
}
