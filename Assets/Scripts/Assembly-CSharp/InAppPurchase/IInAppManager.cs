namespace InAppPurchase
{
	public interface IInAppManager
	{
		void InitFinishedCallback(bool success);

		string getInAppAPIKey();

		IsIAPConsumableValue IsProductConsumable(string productID);

		void ProductInfoRequestCallback(bool success, ProductRequestResultInfo[] productInfo);

		void PurchaseRequestCallback(string productId, string transactionId, string transactionIdBase64, string cookie, PurchaseResult result, ValidationResult validationResult);

		void ItemRestoredCallback(string productId, string transactionId, string cookie, ValidationResult validationResult);

		void RestoreRequestFinishedCallback(RestoreFinishedResult result);

		void ItemRefundedCallback(string productid, string transactionid, string cookie, ValidationResult validationResult);
	}
}
