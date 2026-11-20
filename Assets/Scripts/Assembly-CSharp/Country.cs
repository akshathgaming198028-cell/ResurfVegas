public class Country
{
	public string name { get; set; }

	public string flag { get; set; }

	public string countryCode { get; set; }

	public Country(string nameT, string flagT, string countryCodeT)
	{
		name = nameT;
		flag = flagT;
		countryCode = countryCodeT;
	}

	public Country()
	{
	}
}
