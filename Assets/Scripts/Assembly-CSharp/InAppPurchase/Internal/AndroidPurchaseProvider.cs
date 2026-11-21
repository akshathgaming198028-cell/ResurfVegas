#define ENABLE_ERROR_LOGS
using System.Diagnostics;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace InAppPurchase.Internal
{
	public class AndroidPurchaseProvider : IPurchaseProvider
	{
		private const string BRIDGE_DELEGATE_GAMEOBJECT_NAME = "AndroidPurchaseBridge_callback";

		private const string FINISHED_TRANSACTION_ID_PREFIX = "SHOULD_CONSUME_";

		private const string SEPARATOR_CHAR = ";";

		private static readonly char[] SEPARATOR_FOR_STRING_SPLIT = new char[1] { ';' };

		private InitDelegate _initDelegate;

		private PurchaseDelegate _purchaseDelegate;

		private ItemRestoredDelegate _itemRestoredDelegate;

		private ItemRefundedDelegate _itemRefundedDelegate;

		private ProductInfoRequestDelegate _productRequestDelegate;

		private RestorePurchaseFinishedDelegate _restorePurchaseFinishedDelegate;

		private IInAppManager _inAppManager;

		private string _validationServerAppId;

		private string _validationServerAppSecret;

		private string _iapAPIKey;

		private AndroidJavaObject _iabInstance;

		private GameObject _callbackGameObject;

		public void Init(IInAppManager inAppManager, ValidationMode validation, string validationAppId, string validationAppSecret)
		{
			_inAppManager = inAppManager;
			_initDelegate = inAppManager.InitFinishedCallback;
			_purchaseDelegate = inAppManager.PurchaseRequestCallback;
			_productRequestDelegate = inAppManager.ProductInfoRequestCallback;
			_itemRestoredDelegate = inAppManager.ItemRestoredCallback;
			_itemRefundedDelegate = inAppManager.ItemRefundedCallback;
			_restorePurchaseFinishedDelegate = inAppManager.RestoreRequestFinishedCallback;
			_validationServerAppId = validationAppId;
			_validationServerAppSecret = validationAppSecret;
			_iapAPIKey = inAppManager.getInAppAPIKey();
			if (_iabInstance == null)
			{
				_iabInstance = new AndroidJavaObject("com.kiloo.androidiablibrary.IABBridge");
			}
			_iabInstance.Set("debugLogsEnabled", false);
			_iabInstance.Set("shouldAutorestoreOnProductRequest", false);
			if (_callbackGameObject == null)
			{
				_callbackGameObject = new GameObject("AndroidPurchaseBridge_callback");
				Object.DontDestroyOnLoad(_callbackGameObject);
				AndroidPurchaseProviderCallbackListener androidPurchaseProviderCallbackListener = _callbackGameObject.AddComponent<AndroidPurchaseProviderCallbackListener>();
				androidPurchaseProviderCallbackListener.Setup(InitSuccessful, InitFailed, ProductRequestSuccessful, ProductRequestFailed, PurchasePurchased, PurchaseRestored, PurchaseRefunded, PurchaseFailed, RestoreFinished, ConsumeFinished);
			}
			_iabInstance.Call("init", _iapAPIKey);
			if (validation == ValidationMode.Validate && (string.IsNullOrEmpty(_validationServerAppId) || string.IsNullOrEmpty(_validationServerAppSecret)))
			{
				LogError("AndroidPurchaseProvider ERROR: This is not supported for the Android part of the plugin yet!");
			}
		}

		public bool CanMakePurchases()
		{
			return _iabInstance.Call<bool>("isIabPossible", new object[0]);
		}

		public void RequestProductInfo(string[] productIds)
		{
			string text = string.Join(";", productIds);
			string text2 = string.Empty;
			foreach (string text3 in productIds)
			{
				text2 = text2 + (_inAppManager.IsProductConsumable(text3) == IsIAPConsumableValue.Consumable) + ";";
				if (!text3.Equals(text3.ToLower()))
				{
					LogError("The product id: " + text3 + " contains uppercase letters. This is an error, as it will not be accepted by Google Play!");
				}
			}
			_iabInstance.Call("requestProductInfo", text, text2);
		}

		public bool StartPurchase(string productId)
		{
			return _iabInstance.Call<bool>("startPurchase", new object[1] { productId });
		}

		public bool StartPurchase(string productId, string cookie)
		{
			return _iabInstance.Call<bool>("startPurchase", new object[2] { productId, cookie });
		}

		public void FinishPurchase(string productId, string transactionId)
		{
			if (_inAppManager.IsProductConsumable(productId) == IsIAPConsumableValue.Consumable && !string.IsNullOrEmpty(transactionId))
			{
				PlayerPrefs.SetString("SHOULD_CONSUME_" + transactionId, productId);
				PlayerPrefs.Save();
			}
			_iabInstance.Call("finishPurchase", productId, transactionId);
		}

		public bool RestoreNonConsumablePurchases(string[] productIds)
		{
			string text = string.Join(";", productIds);
			string text2 = string.Empty;
			int num = 0;
			foreach (string productID in productIds)
			{
				text2 = text2 + (_inAppManager.IsProductConsumable(productID) == IsIAPConsumableValue.Consumable) + ";";
				num++;
			}
			return _iabInstance.Call<bool>("restoreNonConsumablePurchases", new object[2] { text, text2 });
		}

		private void InitSuccessful()
		{
			InitDelegate initDelegate = _initDelegate;
			if (initDelegate != null)
			{
				initDelegate(true);
			}
		}

		private void InitFailed()
		{
			InitDelegate initDelegate = _initDelegate;
			if (initDelegate != null)
			{
				initDelegate(false);
			}
		}

		private void ProductRequestSuccessful(string message)
		{
			ProductRequestResultInfo[] productInfo;
			if (TryParseProductInfo(message, out productInfo))
			{
				StringBuilder stringBuilder = new StringBuilder("AndroidPurchaseProvider ProductInfo:\n");
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
				LogError("AndroidPurchaseProvider ERROR: Malformed message from native code: " + message);
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

		private void PurchaseRefunded(string message)
		{
			HandlePurchaseEnded(message, InternalPurchaseResult.Refunded);
		}

		private void PurchaseFailed(string message)
		{
			string[] array = message.Split(SEPARATOR_FOR_STRING_SPLIT);
			InternalPurchaseResult internalPurchaseResult = InternalPurchaseResult.Failed;
			if (array.Length == 5)
			{
				internalPurchaseResult = GetProperPurchaseResult(array[4]);
				message = array[0] + ";" + array[1] + ";" + array[2] + ";" + array[3];
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

		private void ConsumeFinished(string transactionId)
		{
			string value = PlayerPrefs.GetString("SHOULD_CONSUME_" + transactionId);
			if (!string.IsNullOrEmpty(value))
			{
				PlayerPrefs.DeleteKey("SHOULD_CONSUME_" + transactionId);
				PlayerPrefs.Save();
			}
		}

		private void ValidateTransaction(string productId, string transactionId, string transactionRecieptBase64, InternalPurchaseResult purchaseResult)
		{
		}

		private void HandlePurchaseEnded(string message, InternalPurchaseResult internalPurchaseResult)
		{
			string productId;
			string transactionId;
			string transactionRecieptBase;
			string cookie;
			if (TryParsePurchaseEndedMessage(message, out productId, out transactionId, out transactionRecieptBase, out cookie))
			{
				switch (internalPurchaseResult)
				{
				case InternalPurchaseResult.Restored:
				{
					ItemRestoredDelegate itemRestoredDelegate = _itemRestoredDelegate;
					if (itemRestoredDelegate != null)
					{
						itemRestoredDelegate(productId, transactionId, cookie, ValidationResult.DidNotValidate);
					}
					return;
				}
				case InternalPurchaseResult.Refunded:
				{
					ItemRefundedDelegate itemRefundedDelegate = _itemRefundedDelegate;
					if (itemRefundedDelegate != null)
					{
						itemRefundedDelegate(productId, transactionId, cookie, ValidationResult.DidNotValidate);
					}
					return;
				}
				}
				string value = PlayerPrefs.GetString("SHOULD_CONSUME_" + transactionId);
				if (_inAppManager.IsProductConsumable(productId) == IsIAPConsumableValue.NotConsumable || string.IsNullOrEmpty(value))
				{
					PurchaseDelegate purchaseDelegate = _purchaseDelegate;
					if (purchaseDelegate != null)
					{
						purchaseDelegate(productId, transactionId, transactionRecieptBase, cookie, internalPurchaseResult.ToPurchaseResult(), ValidationResult.DidNotValidate);
					}
				}
				else
				{
					FinishPurchase(productId, transactionId);
				}
			}
			else
			{
				LogError("AndroidPurchaseProvider ERROR: Malformed message from native code: " + message);
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

		private bool TryParsePurchaseEndedMessage(string message, out string productId, out string transactionId, out string transactionRecieptBase64, out string cookie)
		{
			string[] array = message.Split(SEPARATOR_FOR_STRING_SPLIT);
			if (array.Length == 4)
			{
				productId = array[0];
				transactionId = array[1];
				transactionRecieptBase64 = array[2];
				cookie = array[3];
				return true;
			}
			productId = string.Empty;
			transactionId = string.Empty;
			transactionRecieptBase64 = string.Empty;
			cookie = string.Empty;
			return false;
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogError(string msg, Object context = null)
		{
			Debug.LogError("AndroidPurchaseProvider: " + msg, context);
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogWarning(string msg, Object context = null)
		{
			Debug.LogWarning("AndroidPurchaseProvider: " + msg, context);
		}

		[Conditional("ENABLE_PURCHASE_PROVIDER_LOGS")]
		private static void Log(string msg, Object context = null)
		{
			Debug.Log("AndroidPurchaseProvider: " + msg, context);
		}
	}
}
