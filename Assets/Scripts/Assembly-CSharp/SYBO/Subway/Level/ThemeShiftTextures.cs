using UnityEngine;

namespace SYBO.Subway.Level
{
	public class ThemeShiftTextures : MonoBehaviour
	{
		private ThemeManager _themeManager;

		private Renderer[] _renderers;

		private void Start()
		{
			_themeManager = ThemeManager.Instance;
			RelinkMaterialWithThemeAssets();
		}

		private void RelinkMaterialWithThemeAssets()
		{
			_renderers = base.gameObject.GetComponentsInChildren<Renderer>(true);
			Theme theme = _themeManager.Theme;
			for (int i = 0; i < _renderers.Length; i++)
			{
				Material[] sharedMaterials = _renderers[i].sharedMaterials;
				for (int j = 0; j < sharedMaterials.Length; j++)
				{
					if (sharedMaterials[j] != null && theme.MaterialReplacementTextureMap.ContainsKey(sharedMaterials[j]))
					{
						sharedMaterials[j] = _themeManager.ReplaceMaterial(sharedMaterials[j]);
					}
				}
				_renderers[i].materials = sharedMaterials;
			}
		}
	}
}
