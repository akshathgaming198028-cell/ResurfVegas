namespace InAppPurchase.Internal
{
	public interface IPurchaseProvider
	{
		void Init(IInAppManager inApManager, ValidationMode validation, string validationAppId, string validationAppSecret);

		bool CanMakePurchases();

		void RequestProductInfo(string[] productIds);

		bool StartPurchase(string productId);

		bool StartPurchase(string productId, string cookie);

		void FinishPurchase(string productId, string transactionId);

		bool RestoreNonConsumablePurchases(string[] productIds);
	}
}
