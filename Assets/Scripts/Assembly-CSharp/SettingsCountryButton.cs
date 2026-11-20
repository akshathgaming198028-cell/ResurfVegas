using UnityEngine;

public class SettingsCountryButton : MonoBehaviour
{
	[SerializeField]
	private UILabel currentCountryLabel;

	private void OnEnable()
	{
		currentCountryLabel.text = CountryManager.instance.GetCurrentTournamentCountry().name;
	}
}
