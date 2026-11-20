using UnityEngine;

public class ResolutionSetup : MonoBehaviour
{
	private const string PATH_TO_ATLASES = "Prefabs/Atalses/";

	private const string HIGH_RES_MENU_ATLAS_NAME = "HighResAtlas";

	private const string HIGH_RES_COUNTRIES_ATLAS_NAME = "HighResCountriesAtlas";

	private const string LOW_RES_MENU_ATLAS_NAME = "LowResAtlas";

	private const string LOW_RES_COUNTRIES_ATLAS_NAME = "LowResCountriesAtlas";

	[SerializeField]
	private UIAtlas menuAtlas;

	[SerializeField]
	private UIAtlas countriesAtlas;

	private UIAtlas _lowResMenuAtlas;

	private UIAtlas _lowResCountriesAtlas;

	private UIAtlas _highResMenuAtlas;

	private UIAtlas _highResCountriesAtlas;

	private UIAtlas _editorMenuAtlase;

	private UIAtlas _editorCountriesAtlase;

	private void Awake()
	{
		_editorMenuAtlase = menuAtlas.replacement;
		_editorCountriesAtlase = countriesAtlas.replacement;
		SetupMenuAtlasReference();
		SetupCountriesAtlasReference();
	}

	private void SetupCountriesAtlasReference()
	{
		if (DeviceInfo.Instance.isHighres)
		{
			LoadAtlas("HighResCountriesAtlas", ref _highResCountriesAtlas);
			SetAtlasReferenceTo(countriesAtlas, _highResCountriesAtlas);
		}
		else
		{
			LoadAtlas("HighResCountriesAtlas", ref _highResCountriesAtlas);
			SetAtlasReferenceTo(countriesAtlas, _highResCountriesAtlas);
		}
	}

	private void SetupMenuAtlasReference()
	{
		if (DeviceInfo.Instance.isHighres)
		{
			LoadAtlas("HighResAtlas", ref _highResMenuAtlas);
			SetAtlasReferenceTo(menuAtlas, _highResMenuAtlas);
		}
		else
		{
			LoadAtlas("LowResAtlas", ref _lowResMenuAtlas);
			SetAtlasReferenceTo(menuAtlas, _lowResMenuAtlas);
		}
	}

	private void LoadAtlas(string atlasName, ref UIAtlas cachedAtlas)
	{
		if (cachedAtlas == null)
		{
			cachedAtlas = Resources.Load("Prefabs/Atalses/" + atlasName, typeof(UIAtlas)) as UIAtlas;
		}
	}

	private void SetAtlasReferenceTo(UIAtlas referenceAtlas, UIAtlas newAtlas)
	{
		if (referenceAtlas != null && newAtlas != null)
		{
			referenceAtlas.replacement = newAtlas;
		}
		else
		{
			Debug.LogError("ResolutionSetup: Failed to set the reference to the new atlas, one of the atlases is null!");
		}
	}

	private void OnDisable()
	{
		if (menuAtlas != null && countriesAtlas != null)
		{
			menuAtlas.replacement = _editorMenuAtlase;
			countriesAtlas.replacement = _editorCountriesAtlase;
		}
	}
}
