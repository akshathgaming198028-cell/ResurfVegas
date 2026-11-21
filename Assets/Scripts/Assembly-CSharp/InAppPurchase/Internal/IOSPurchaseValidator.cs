#define ENABLE_ERROR_LOGS
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace InAppPurchase.Internal
{
	public class IOSPurchaseValidator
	{
		public delegate void ValidationCompleteDelegate(ValidationResult result, object cookie);

		private const string VALIDATION_URL = "http://iap.kiloo.com/validateinapp.php";

		private const float TIMEOUT = 10f;

		private static MonoBehaviour coroutineBehaviour;

		public static void Validate(string appId, string appSecret, ValidationCompleteDelegate completionDelegate, object cookie)
		{
			if (completionDelegate != null)
			{
				completionDelegate(ValidationResult.DidNotValidate, cookie);
			}
			LogError("Validation currently not implemented for iOS");
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogError(string msg, Object context = null)
		{
			Debug.LogError("IOSPurchaseValidator: " + msg, context);
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogWarning(string msg, Object context = null)
		{
			Debug.LogWarning("IOSPurchaseValidator: " + msg, context);
		}

		[Conditional("ENABLE_PURCHASE_PROVIDER_LOGS")]
		private static void Log(string msg, Object context = null)
		{
			Debug.Log("IOSPurchaseValidator: " + msg, context);
		}
	}
}
