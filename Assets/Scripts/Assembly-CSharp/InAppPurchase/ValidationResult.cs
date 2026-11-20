namespace InAppPurchase
{
	public enum ValidationResult
	{
		DidNotValidate = 0,
		Ok = 1,
		FailedValidation = 2,
		ServerUnreachable = 3,
		ServerError = 4,
		InvalidServerResponse = 5
	}
}
