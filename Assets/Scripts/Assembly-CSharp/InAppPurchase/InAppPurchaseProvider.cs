#define ENABLE_ERROR_LOGS
using System.Diagnostics;
using System.Linq;
using InAppPurchase.Internal;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace InAppPurchase
{
	public class InAppPurchaseProvider : IInAppManager, IPurchaseProvider
	{
		private IPurchaseProvider _provider;

		private IAPPluginInitializationState _currentInitializationState;

		private IInAppManager _inAppManager;

		private ProductRequestResultInfo[] _latestProductInfo = new ProductRequestResultInfo[0];

		private static InAppPurchaseProvider _instance;

		public static InAppPurchaseProvider instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new InAppPurchaseProvider();
				}
				return _instance;
			}
		}

		public bool isInstanced
		{
			get
			{
				return _instance != null;
			}
		}

		private InAppPurchaseProvider()
		{
			switch (Application.platform)
			{
			case RuntimePlatform.IPhonePlayer:
				_provider = new IOSPurchaseProvider();
				break;
			case RuntimePlatform.Android:
				_provider = new AndroidPurchaseProvider();
				break;
			case RuntimePlatform.WP8Player:
				_provider = new WP8PurchaseProvider();
				break;
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.WindowsEditor:
				_provider = new EditorPurchaseProvider();
				break;
			default:
				LogError("There's no provider for the current platform.");
				break;
			}
		}

		public ProductRequestResultInfo[] GetLatestRequestedProductInfo()
		{
			return (ProductRequestResultInfo[])_latestProductInfo.Clone();
		}

		public bool TryGetLatestRequestedProductInfoForProductId(string productId, out ProductRequestResultInfo productInfo)
		{
			ProductRequestResultInfo[] latestProductInfo = _latestProductInfo;
			for (int i = 0; i < latestProductInfo.Length; i++)
			{
				ProductRequestResultInfo productRequestResultInfo = latestProductInfo[i];
				if (productRequestResultInfo.productId.Equals(productId))
				{
					productInfo = productRequestResultInfo;
					return true;
				}
			}
			productInfo = default(ProductRequestResultInfo);
			return false;
		}

		public void Init(IInAppManager inAppManager)
		{
			Init(inAppManager, ValidationMode.DontValidate, string.Empty, string.Empty);
		}

		public void Init(IInAppManager inAppManager, ValidationMode validationMode, string validationAppId, string validationAppSecret)
		{
			if (_currentInitializationState == IAPPluginInitializationState.NotInitialized)
			{
				_inAppManager = inAppManager;
				_currentInitializationState = IAPPluginInitializationState.Initializing;
				_provider.Init(this, validationMode, validationAppId, validationAppSecret);
			}
			else if (_currentInitializationState != IAPPluginInitializationState.Initializing && _currentInitializationState != IAPPluginInitializationState.InitializedSuccessfully)
			{
			}
		}

		public bool CanMakePurchases()
		{
			if (_currentInitializationState != IAPPluginInitializationState.InitializedSuccessfully)
			{
				return false;
			}
			return _provider.CanMakePurchases();
		}

		public void RequestProductInfo(string[] productIds)
		{
			if (CanMakePurchases())
			{
				string[] productIds2 = productIds.Distinct().ToArray();
				_provider.RequestProductInfo(productIds2);
			}
		}

		public bool StartPurchase(string productId)
		{
			if (!CanMakePurchases())
			{
				_inAppManager.PurchaseRequestCallback(productId, string.Empty, string.Empty, string.Empty, PurchaseResult.Failed, ValidationResult.DidNotValidate);
				return false;
			}
			return _provider.StartPurchase(productId, null);
		}

		public bool StartPurchase(string productId, string cookie)
		{
			if (!CanMakePurchases())
			{
				_inAppManager.PurchaseRequestCallback(productId, string.Empty, string.Empty, cookie, PurchaseResult.Failed, ValidationResult.DidNotValidate);
				return false;
			}
			return _provider.StartPurchase(productId, cookie);
		}

		public bool RestoreNonConsumablePurchases(string[] productIds)
		{
			if (!CanMakePurchases())
			{
				_inAppManager.RestoreRequestFinishedCallback(RestoreFinishedResult.Failed);
				return false;
			}
			return _provider.RestoreNonConsumablePurchases(productIds);
		}

		public void FinishPurchase(string productId, string transactionId)
		{
			if (CanMakePurchases())
			{
				_provider.FinishPurchase(productId, transactionId);
			}
		}

		public void InitFinishedCallback(bool success)
		{
			if (!success)
			{
				_currentInitializationState = IAPPluginInitializationState.NotInitialized;
			}
			else
			{
				_currentInitializationState = IAPPluginInitializationState.InitializedSuccessfully;
			}
			_inAppManager.InitFinishedCallback(success);
		}

		public string getInAppAPIKey()
		{
			return _inAppManager.getInAppAPIKey();
		}

		public IsIAPConsumableValue IsProductConsumable(string productID)
		{
			return _inAppManager.IsProductConsumable(productID);
		}

		public void ProductInfoRequestCallback(bool success, ProductRequestResultInfo[] productInfo)
		{
			_latestProductInfo = ((!success) ? new ProductRequestResultInfo[0] : ((ProductRequestResultInfo[])productInfo.Clone()));
			_inAppManager.ProductInfoRequestCallback(success, productInfo);
		}

		public void PurchaseRequestCallback(string productId, string transactionId, string transationIdBase64, string cookie, PurchaseResult result, ValidationResult validationResult)
		{
			_inAppManager.PurchaseRequestCallback(productId, transactionId, transationIdBase64, cookie, result, validationResult);
		}

		public void ItemRestoredCallback(string productId, string transactionId, string cookie, ValidationResult validationResult)
		{
			_inAppManager.ItemRestoredCallback(productId, transactionId, cookie, validationResult);
		}

		public void ItemRefundedCallback(string productid, string transactionid, string cookie, ValidationResult validationResult)
		{
			_inAppManager.ItemRefundedCallback(productid, transactionid, cookie, validationResult);
		}

		public void RestoreRequestFinishedCallback(RestoreFinishedResult result)
		{
			_inAppManager.RestoreRequestFinishedCallback(result);
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogError(string msg, Object context = null)
		{
			Debug.LogError("InAppPurchaseProvider: " + msg, context);
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogWarning(string msg, Object context = null)
		{
			Debug.LogWarning("InAppPurchaseProvider: " + msg, context);
		}

		[Conditional("ENABLE_PURCHASE_PROVIDER_LOGS")]
		private static void Log(string msg, Object context = null)
		{
			Debug.Log("InAppPurchaseProvider: " + msg, context);
		}
	}
}
