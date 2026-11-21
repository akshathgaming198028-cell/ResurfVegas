#define ENABLE_ERROR_LOGS
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Kiloo.Plugins.InappPurchase;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace InAppPurchase.Internal
{
	public class WP8PurchaseProvider : MonoBehaviour, IPurchaseProvider
	{
		private const string CALLBACK_RECEIVER_NAME = "WP8PurchaseBridgeReceiver";

		private IInAppManager _inAppManager;

		private PurchaseDelegate _purchaseDelegate;

		private ItemRestoredDelegate _itemRestoredDelegate;

		private ItemRefundedDelegate _itemRefundedDelegate;

		private ProductInfoRequestDelegate _productRequestDelegate;

		private RestorePurchaseFinishedDelegate _restorePurchaseFinishedDelegate;

		private ValidationMode _validationMode;

		private string _validationServerAppId;

		private string _validationServerAppSecret;

		private GameObject _callbackGameObject;

		public GameObject CreateCallbackGameObject()
		{
			GameObject gameObject = new GameObject("WP8PurchaseBridgeReceiver");
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			gameObject.AddComponent<WP8PurchaseProvider>();
			return gameObject;
		}

		public void Init(IInAppManager inAppManager, ValidationMode validationMode, string validationAppId, string validationAppSecret)
		{
			_inAppManager = inAppManager;
			_purchaseDelegate = inAppManager.PurchaseRequestCallback;
			_productRequestDelegate = inAppManager.ProductInfoRequestCallback;
			_itemRestoredDelegate = inAppManager.ItemRestoredCallback;
			_itemRefundedDelegate = inAppManager.ItemRefundedCallback;
			_restorePurchaseFinishedDelegate = inAppManager.RestoreRequestFinishedCallback;
			_validationMode = validationMode;
			_validationServerAppId = validationAppId;
			_validationServerAppSecret = validationAppSecret;
			if (validationMode == ValidationMode.Validate && (string.IsNullOrEmpty(_validationServerAppId) || string.IsNullOrEmpty(_validationServerAppSecret)))
			{
				LogError("WP8PurchaseProvider ERROR: If validation is enabled, appId and secret must be provided");
			}
			if (_callbackGameObject == null)
			{
				_callbackGameObject = CreateCallbackGameObject();
			}
			InappPurchaseManagerWP8.Instance.SetDebugLogsEnabled(false);
			InappPurchaseManagerWP8.OnWP8ConsumablePurchaseSuccessful += callback_onWP8ConsumablePurchaseSuccessful;
			InappPurchaseManagerWP8.OnWP8PurchaseLicenseActive += callback_onWP8PurchaseLicenseActive;
			InappPurchaseManagerWP8.OnWP8PurchaseLicenseInactive += callback_onWP8PurchaseLicenseInactive;
			InappPurchaseManagerWP8.OnWP8PurchaseCancelled += callback_onWP8PurchaseCancelled;
			InappPurchaseManagerWP8.OnWP8PurchaseFailed += callback_onWP8PurchaseFailed;
			InappPurchaseManagerWP8.OnWP8PurchaseFinished += callback_onWP8PurchaseFinished;
			InappPurchaseManagerWP8.OnWP8QuerySuccessful += callback_onWP8QuerySuccessful;
			InappPurchaseManagerWP8.OnWP8QueryFailed += callback_onWP8QueryFailed;
			InappPurchaseManagerWP8.Instance.Init();
			_inAppManager.InitFinishedCallback(true);
		}

		public bool CanMakePurchases()
		{
			return InappPurchaseManagerWP8.Instance.IsInitialized;
		}

		public void RequestProductInfo(string[] productIds)
		{
			InappPurchaseManagerWP8.Instance.QueryListProducts(null);
		}

		public bool StartPurchase(string productId)
		{
			return StartPurchase(productId, null);
		}

		public bool StartPurchase(string productId, string cookie)
		{
			if (string.IsNullOrEmpty(cookie))
			{
				LogWarning("Cookies for the purchase is not supported for WP8.");
			}
			InappPurchaseManagerWP8.Instance.BuyItem(productId);
			return true;
		}

		public void FinishPurchase(string productId, string transactionId)
		{
		}

		public void FinishPurchase(PurchasableProduct product)
		{
			if (_inAppManager.IsProductConsumable(product.ProductId) == IsIAPConsumableValue.Consumable)
			{
				InappPurchaseManagerWP8.Instance.ReportProductFulfillment(product);
			}
		}

		public bool RestoreNonConsumablePurchases(string[] productIds)
		{
			RestoreLicenses(RestoreLicensesFinished);
			return true;
		}

		private void RestoreLicenses(Action<bool> onFinishHandler)
		{
			InappPurchaseManagerWP8.Instance.RestoreProductLicenses(onFinishHandler);
		}

		private void callback_onWP8ConsumablePurchaseSuccessful(PurchasableProduct product)
		{
			HandlePurchaseEnded(product, InternalPurchaseResult.Completed);
		}

		private void callback_onWP8PurchaseLicenseActive(PurchasableProduct product)
		{
			HandlePurchaseEnded(product, InternalPurchaseResult.Restored);
		}

		private void callback_onWP8PurchaseLicenseInactive(PurchasableProduct product)
		{
			HandlePurchaseEnded(product, InternalPurchaseResult.Refunded);
		}

		private void callback_onWP8PurchaseCancelled(string productId, string error)
		{
			PurchasableProduct product = new PurchasableProduct(productId, null, null, null, null, PurchasableProduct.PurchasableProductType.Unknown, false);
			HandlePurchaseEnded(product, InternalPurchaseResult.Canceled);
		}

		private void callback_onWP8PurchaseFailed(string productId, string error)
		{
			PurchasableProduct product = new PurchasableProduct(productId, null, null, null, null, PurchasableProduct.PurchasableProductType.Unknown, false);
			HandlePurchaseEnded(product, InternalPurchaseResult.Failed);
		}

		private void callback_onWP8PurchaseFinished()
		{
			RestoreLicenses(RestoreLicensesFinished);
		}

		private void callback_onWP8QuerySuccessful(List<PurchasableProduct> listProducts)
		{
			ProductRequestResultInfo[] array = new ProductRequestResultInfo[listProducts.Count];
			for (int i = 0; i < listProducts.Count; i++)
			{
				PurchasableProduct purchasableProduct = listProducts[i];
				array[i].Init(purchasableProduct.ProductId, purchasableProduct.FormattedPrice, purchasableProduct.Name, purchasableProduct.Description);
			}
			StringBuilder stringBuilder = new StringBuilder("WP8PurchaseProvider ProductInfo:\n");
			ProductRequestResultInfo[] array2 = array;
			for (int j = 0; j < array2.Length; j++)
			{
				ProductRequestResultInfo productRequestResultInfo = array2[j];
				stringBuilder.AppendLine("    productId: " + productRequestResultInfo.productId + "  localized price: " + productRequestResultInfo.localizedPrice);
			}
			ProductInfoRequestDelegate productRequestDelegate = _productRequestDelegate;
			if (productRequestDelegate != null)
			{
				productRequestDelegate(true, array);
			}
		}

		private void callback_onWP8QueryFailed(string error)
		{
			ProductInfoRequestDelegate productRequestDelegate = _productRequestDelegate;
			if (productRequestDelegate != null)
			{
				productRequestDelegate(false, new ProductRequestResultInfo[0]);
			}
		}

		private void FireEventWithMessage(Action<string> handler, string message)
		{
			if (handler != null)
			{
				handler(message);
			}
		}

		private void FireEmptyEvent(Action handler)
		{
			if (handler != null)
			{
				handler();
			}
		}

		private void HandlePurchaseEnded(PurchasableProduct product, InternalPurchaseResult internalPurchaseResult)
		{
			string productId = product.ProductId;
			string empty = string.Empty;
			string empty2 = string.Empty;
			if (_validationMode == ValidationMode.Validate && internalPurchaseResult == InternalPurchaseResult.Completed)
			{
				ValidateTransaction(productId, empty, empty2, internalPurchaseResult);
				return;
			}
			switch (internalPurchaseResult)
			{
			case InternalPurchaseResult.Restored:
			{
				ItemRestoredDelegate itemRestoredDelegate = _itemRestoredDelegate;
				if (itemRestoredDelegate != null)
				{
					itemRestoredDelegate(productId, empty, null, ValidationResult.DidNotValidate);
				}
				return;
			}
			case InternalPurchaseResult.Refunded:
			{
				ItemRefundedDelegate itemRefundedDelegate = _itemRefundedDelegate;
				if (itemRefundedDelegate != null)
				{
					itemRefundedDelegate(productId, empty, null, ValidationResult.DidNotValidate);
				}
				return;
			}
			}
			PurchaseDelegate purchaseDelegate = _purchaseDelegate;
			if (purchaseDelegate != null)
			{
				purchaseDelegate(productId, empty, empty2, null, internalPurchaseResult.ToPurchaseResult(), ValidationResult.DidNotValidate);
			}
			if (_inAppManager.IsProductConsumable(productId) == IsIAPConsumableValue.Consumable && internalPurchaseResult == InternalPurchaseResult.Completed)
			{
				FinishPurchase(product);
			}
		}

		private void RestoreLicensesFinished(bool success)
		{
			RestoreFinishedResult restoreFinishedResult = RestoreFinishedResult.Failed;
			restoreFinishedResult = (success ? RestoreFinishedResult.Completed : RestoreFinishedResult.Failed);
			RestorePurchaseFinishedDelegate restorePurchaseFinishedDelegate = _restorePurchaseFinishedDelegate;
			if (restorePurchaseFinishedDelegate != null)
			{
				restorePurchaseFinishedDelegate(restoreFinishedResult);
			}
		}

		private void ValidateTransaction(string productId, string transactionId, string transactionRecieptBase64, InternalPurchaseResult purchaseResult)
		{
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogError(string msg, UnityEngine.Object context = null)
		{
			Debug.LogError("WP8PurchaseProvider: " + msg, context);
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogWarning(string msg, UnityEngine.Object context = null)
		{
			Debug.LogWarning("WP8PurchaseProvider: " + msg, context);
		}

		[Conditional("ENABLE_PURCHASE_PROVIDER_LOGS")]
		private static void Log(string msg, UnityEngine.Object context = null)
		{
			Debug.Log("WP8PurchaseProvider: " + msg, context);
		}
	}
}
