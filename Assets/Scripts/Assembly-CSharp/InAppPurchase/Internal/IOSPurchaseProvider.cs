#define ENABLE_ERROR_LOGS
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace InAppPurchase.Internal
{
	public class IOSPurchaseProvider : IPurchaseProvider
	{
		private struct ValidationCookie
		{
			public string productId;

			public string transactionId;

			public string transactionIdBase64;

			public InternalPurchaseResult purchaseResult;
		}

		private const string BRIDGE_DELEGATE_GAMEOBJECT_NAME = "IOSPurchaseBridge_callback";

		private const char SEPARATOR_CHAR = ';';

		private static readonly char[] SEPARATOR_FOR_STRING_SPLIT = new char[1] { ';' };

		private IInAppManager _inAppManager;

		private PurchaseDelegate _purchaseDelegate;

		private ItemRestoredDelegate _itemRestoredDelegate;

		private ProductInfoRequestDelegate _productRequestDelegate;

		private RestorePurchaseFinishedDelegate _restorePurchaseFinishedDelegate;

		private ValidationMode _validationMode;

		private string _validationServerAppId;

		private string _validationServerAppSecret;

		private GameObject _callbackGameObject;

		public void Init(IInAppManager inAppManager, ValidationMode validationMode, string validationAppId, string validationAppSecret)
		{
			_inAppManager = inAppManager;
			_purchaseDelegate = inAppManager.PurchaseRequestCallback;
			_productRequestDelegate = inAppManager.ProductInfoRequestCallback;
			_itemRestoredDelegate = inAppManager.ItemRestoredCallback;
			_restorePurchaseFinishedDelegate = inAppManager.RestoreRequestFinishedCallback;
			_validationMode = validationMode;
			_validationServerAppId = validationAppId;
			_validationServerAppSecret = validationAppSecret;
			if (validationMode == ValidationMode.Validate && (string.IsNullOrEmpty(_validationServerAppId) || string.IsNullOrEmpty(_validationServerAppSecret)))
			{
				LogError("IOSPurchaseProvider ERROR: If validation is enabled, appId and secret must be provided");
			}
			IOSPurchaseBridge_EnableDebugLogs("false");
			if (_callbackGameObject == null)
			{
				_callbackGameObject = new GameObject("IOSPurchaseBridge_callback");
				Object.DontDestroyOnLoad(_callbackGameObject);
				IOSPurchaseProviderCallbackListener iOSPurchaseProviderCallbackListener = _callbackGameObject.AddComponent<IOSPurchaseProviderCallbackListener>();
				iOSPurchaseProviderCallbackListener.Setup(ProductRequestSuccessful, ProductRequestFailed, PurchasePurchased, PurchaseRestored, PurchaseFailed, RestoreFinished);
			}
			IOSPurchaseBridge_Init();
			_inAppManager.InitFinishedCallback(true);
		}

		public bool CanMakePurchases()
		{
			return IOSPurchaseBridge_CanMakePayments();
		}

		public void RequestProductInfo(string[] productIds)
		{
			IOSPurchaseBridge_RunProductsRequest(ArrayToString(productIds));
		}

		public bool StartPurchase(string productId)
		{
			return StartPurchase(productId, null);
		}

		public bool StartPurchase(string productId, string cookie)
		{
			if (string.IsNullOrEmpty(cookie))
			{
				LogWarning("Cookies for the purchase is not supported for iOS.");
			}
			return IOSPurchaseBridge_StartPurchase(productId);
		}

		public void FinishPurchase(string productId, string transactionId)
		{
			IOSPurchaseBridge_FinishTransaction(productId, transactionId);
		}

		public bool RestoreNonConsumablePurchases(string[] productIds)
		{
			return IOSPurchaseBridge_RestoreNonConsumablePurchases();
		}

		private string ArrayToString(string[] array)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i]);
				if (i < array.Length - 1)
				{
					stringBuilder.Append(';');
				}
			}
			return stringBuilder.ToString();
		}

		private void HandlePurchaseEnded(string message, InternalPurchaseResult internalPurchaseResult)
		{
			string productId;
			string transactionId;
			string transactionRecieptBase;
			if (TryParsePurchaseEndedMessage(message, out productId, out transactionId, out transactionRecieptBase))
			{
				if (_validationMode == ValidationMode.Validate && internalPurchaseResult == InternalPurchaseResult.Completed)
				{
					ValidateTransaction(productId, transactionId, transactionRecieptBase, internalPurchaseResult);
				}
				else if (internalPurchaseResult == InternalPurchaseResult.Restored)
				{
					ItemRestoredDelegate itemRestoredDelegate = _itemRestoredDelegate;
					if (itemRestoredDelegate != null)
					{
						itemRestoredDelegate(productId, transactionId, null, ValidationResult.DidNotValidate);
					}
				}
				else
				{
					PurchaseDelegate purchaseDelegate = _purchaseDelegate;
					if (purchaseDelegate != null)
					{
						purchaseDelegate(productId, transactionId, transactionRecieptBase, null, internalPurchaseResult.ToPurchaseResult(), ValidationResult.DidNotValidate);
					}
				}
			}
			else
			{
				LogError("IOSPurchaseProvider ERROR: Malformed message from native code: " + message);
			}
		}

		private bool TryParsePurchaseEndedMessage(string message, out string productId, out string transactionId, out string transactionRecieptBase64)
		{
			string[] array = message.Split(SEPARATOR_FOR_STRING_SPLIT);
			if (array.Length == 3)
			{
				productId = array[0];
				transactionId = array[1];
				transactionRecieptBase64 = array[2];
				return true;
			}
			productId = string.Empty;
			transactionId = string.Empty;
			transactionRecieptBase64 = string.Empty;
			return false;
		}

		private bool TryParseProductInfo(string message, out ProductRequestResultInfo[] productInfo)
		{
			string[] array = message.Split(SEPARATOR_FOR_STRING_SPLIT);
			int result;
			if (array.Length >= 1 && int.TryParse(array[0], out result))
			{
				int num = 1 + result * 4;
				if (array.Length == num)
				{
					ProductRequestResultInfo[] array2 = new ProductRequestResultInfo[result];
					for (int i = 0; i < result; i++)
					{
						int num2 = 1 + i * 4;
						string productId = array[num2];
						string localizedPrice = array[num2 + 1];
						string title = array[num2 + 2];
						string description = array[num2 + 3];
						array2[i].Init(productId, localizedPrice, title, description);
					}
					productInfo = array2;
					return true;
				}
			}
			productInfo = new ProductRequestResultInfo[0];
			return false;
		}

		private void ValidateTransaction(string productId, string transactionId, string transactionReceiptBase64, InternalPurchaseResult purchaseResult)
		{
			IOSPurchaseValidator.Validate(cookie: new ValidationCookie
			{
				productId = productId,
				transactionId = transactionId,
				purchaseResult = purchaseResult,
				transactionIdBase64 = transactionReceiptBase64
			}, appId: _validationServerAppId, appSecret: _validationServerAppSecret, completionDelegate: OnValidationCompleted);
		}

		private void OnValidationCompleted(ValidationResult result, object cookie)
		{
			ValidationCookie validationCookie = (ValidationCookie)cookie;
			PurchaseDelegate purchaseDelegate = _purchaseDelegate;
			if (purchaseDelegate != null)
			{
				purchaseDelegate(validationCookie.productId, validationCookie.transactionId, validationCookie.transactionIdBase64, null, validationCookie.purchaseResult.ToPurchaseResult(), result);
			}
		}

		private InternalPurchaseResult GetProperPurchaseResult(string errorCode)
		{
			int result = -1;
			if (int.TryParse(errorCode, out result))
			{
				switch (result)
				{
				case 0:
					return InternalPurchaseResult.Completed;
				case 1:
					return InternalPurchaseResult.Failed;
				case 2:
					return InternalPurchaseResult.Canceled;
				case 3:
					return InternalPurchaseResult.AlreadyOwned;
				case 4:
					return InternalPurchaseResult.ItemUnavailable;
				default:
					return InternalPurchaseResult.Failed;
				}
			}
			return InternalPurchaseResult.Failed;
		}

		private void ProductRequestSuccessful(string message)
		{
			ProductRequestResultInfo[] productInfo;
			if (TryParseProductInfo(message, out productInfo))
			{
				StringBuilder stringBuilder = new StringBuilder("IOSPurchaseProvider ProductInfo:\n");
				ProductRequestResultInfo[] array = productInfo;
				for (int i = 0; i < array.Length; i++)
				{
					ProductRequestResultInfo productRequestResultInfo = array[i];
					stringBuilder.AppendLine("    productId: " + productRequestResultInfo.productId + "  localized price: " + productRequestResultInfo.localizedPrice);
				}
				ProductInfoRequestDelegate productRequestDelegate = _productRequestDelegate;
				if (productRequestDelegate != null)
				{
					productRequestDelegate(true, productInfo);
				}
			}
			else
			{
				LogError("IOSPurchaseProvider ERROR: Malformed message from native code: " + message);
			}
		}

		private void ProductRequestFailed()
		{
			ProductInfoRequestDelegate productRequestDelegate = _productRequestDelegate;
			if (productRequestDelegate != null)
			{
				productRequestDelegate(false, new ProductRequestResultInfo[0]);
			}
		}

		private void PurchasePurchased(string message)
		{
			HandlePurchaseEnded(message, InternalPurchaseResult.Completed);
		}

		private void PurchaseRestored(string message)
		{
			HandlePurchaseEnded(message, InternalPurchaseResult.Restored);
		}

		private void PurchaseFailed(string message)
		{
			string[] array = message.Split(SEPARATOR_FOR_STRING_SPLIT);
			InternalPurchaseResult internalPurchaseResult = InternalPurchaseResult.Failed;
			if (array.Length == 4)
			{
				internalPurchaseResult = GetProperPurchaseResult(array[3]);
				message = array[0] + ';' + array[1] + ';' + array[2];
			}
			HandlePurchaseEnded(message, internalPurchaseResult);
		}

		private void RestoreFinished(string message)
		{
			RestoreFinishedResult restoreFinishedResult = RestoreFinishedResult.Failed;
			int result = -1;
			int.TryParse(message, out result);
			switch (result)
			{
			case 0:
				restoreFinishedResult = RestoreFinishedResult.Failed;
				break;
			case 1:
				restoreFinishedResult = RestoreFinishedResult.NothingToRestore;
				break;
			case 2:
				restoreFinishedResult = RestoreFinishedResult.Completed;
				break;
			case 3:
				restoreFinishedResult = RestoreFinishedResult.Canceled;
				break;
			default:
				restoreFinishedResult = RestoreFinishedResult.Failed;
				break;
			}
			RestorePurchaseFinishedDelegate restorePurchaseFinishedDelegate = _restorePurchaseFinishedDelegate;
			if (restorePurchaseFinishedDelegate != null)
			{
				restorePurchaseFinishedDelegate(restoreFinishedResult);
			}
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogError(string msg, Object context = null)
		{
			Debug.LogError("IOSPurchaseProvider: " + msg, context);
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogWarning(string msg, Object context = null)
		{
			Debug.LogWarning("IOSPurchaseProvider: " + msg, context);
		}

		[Conditional("ENABLE_PURCHASE_PROVIDER_LOGS")]
		private static void Log(string msg, Object context = null)
		{
			Debug.Log("IOSPurchaseProvider: " + msg, context);
		}

		private static void IOSPurchaseBridge_Init() { }

		private static bool IOSPurchaseBridge_CanMakePayments() { return false; }

		private static void IOSPurchaseBridge_RunProductsRequest(string productIdsList) { }

		private static bool IOSPurchaseBridge_StartPurchase(string productId) { return false; }

		private static void IOSPurchaseBridge_FinishTransaction(string productId, string transactionId) { }

		private static bool IOSPurchaseBridge_RestoreNonConsumablePurchases() { return false; }

		private static bool IOSPurchaseBridge_EnableDebugLogs(string enabledBoolString) { return false; }
	}
}
