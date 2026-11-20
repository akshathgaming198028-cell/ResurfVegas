using UnityEngine;

namespace SYBO.Subway.Level
{
	public class ThemeShiftMeshes : MonoBehaviour
	{
		private MeshFilter _meshFilter;

		private SkinnedMeshRenderer _skinnedMeshRenderer;

		private void Awake()
		{
			_meshFilter = GetComponent<MeshFilter>();
			_skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
		}

		private void Start()
		{
			ThemeManager.Instance.ThemeChanged += ThemeManager_ChangeTheme;
		}

		private void OnDestroy()
		{
			if (ThemeManager.Instance != null)
			{
				ThemeManager.Instance.ThemeChanged -= ThemeManager_ChangeTheme;
			}
		}

		public void ThemeManager_ChangeTheme(Theme theme)
		{
			string text = theme.name.ToLower();
			if (_meshFilter != null && !(_meshFilter.sharedMesh == null) && _meshFilter.sharedMesh.name != string.Empty)
			{
				string text2 = _meshFilter.sharedMesh.name;
				string text3 = text2.Substring(text2.IndexOf("_") + 1);
				string key = text + "_" + text3;
				Mesh value;
				if (theme.EnvironmentModelMeshes.TryGetValue(key, out value))
				{
					_meshFilter.mesh = value;
				}
			}
			if (_skinnedMeshRenderer != null)
			{
				string text4 = _skinnedMeshRenderer.sharedMesh.name;
				string text5 = text4.Substring(text4.IndexOf("_") + 1);
				string key2 = text + "_" + text5;
				Mesh value2;
				if (theme.CharacterModelMeshes.TryGetValue(key2, out value2))
				{
					_skinnedMeshRenderer.sharedMesh = value2;
				}
			}
		}
	}
}
