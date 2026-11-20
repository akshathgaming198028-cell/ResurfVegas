using System;
using System.Collections.Generic;
using UnityEngine;

namespace SYBO.Subway.Level
{
	public class ThemeManager : MonoBehaviour
	{
		private static bool _applicationQuitting;

		private static ThemeManager _instance;

		[SerializeField]
		private Vector4 _distortion = new Vector4(-0.0005f, -0.0003f, 0f, 0f);

		[SerializeField]
		private Theme _theme;

		private Theme _themeRefDuplicate;

		public Dictionary<Material, Material> _original2CloneMaterials = new Dictionary<Material, Material>();

		public Dictionary<Material, List<Material>> _original2DynamicCloneMaterials = new Dictionary<Material, List<Material>>();

		private CameraCulling cameraCulling;

		public static ThemeManager Instance
		{
			get
			{
				if (_applicationQuitting && Application.isPlaying)
				{
					return null;
				}
				if (_instance == null)
				{
					_instance = Utils.FindObject<ThemeManager>();
				}
				return _instance;
			}
		}

		public Theme Theme
		{
			get
			{
				return _theme;
			}
		}

		public event Action<Theme> ThemeChanged;

		private void Awake()
		{
			if (_instance != null && _instance != this)
			{
				Debug.LogError("Instatiated multiple ThemeManagers!");
			}
			_applicationQuitting = false;
			_instance = this;
			UIScreenController.OnApplicationResumed += UIScreenController_OnApplicationResumed;
			Material[] dynamicChangingMaterials = _theme.DynamicChangingMaterials;
			foreach (Material key in dynamicChangingMaterials)
			{
				_original2DynamicCloneMaterials[key] = new List<Material>();
			}
			foreach (KeyValuePair<Material, Texture> item in _theme.MaterialReplacementTextureMap)
			{
				Material key2 = item.Key;
				if (!_original2CloneMaterials.ContainsKey(key2))
				{
					Material value = new Material(key2);
					_original2CloneMaterials.Add(key2, value);
				}
			}
			Material glowGoldMatrial = _theme.GlowGoldMatrial;
			if (!_original2CloneMaterials.ContainsKey(glowGoldMatrial))
			{
				Material value2 = new Material(glowGoldMatrial);
				_original2CloneMaterials.Add(glowGoldMatrial, value2);
			}
		}

		public void PrepoolThemePrefabs()
		{
		}

		private void Update()
		{
			if (_theme != _themeRefDuplicate && _theme != null)
			{
				_themeRefDuplicate = _theme;
				ResetTheme();
			}
		}

		private void OnApplicationQuit()
		{
			_applicationQuitting = true;
		}

		private void OnValidate()
		{
			ResetTheme();
		}

		private void ResetTheme()
		{
			SetShaderStates();
			RenderSettings.fog = false;
			if (Camera.main != null)
			{
				Camera.main.backgroundColor = _theme.FogGradientBottom;
			}
			foreach (KeyValuePair<Material, Texture> item in _theme.MaterialReplacementTextureMap)
			{
				Material value;
				if (_original2CloneMaterials.TryGetValue(item.Key, out value))
				{
					value.SetTexture("_MainTex", item.Value);
				}
			}
			Material value2;
			if (_original2CloneMaterials.TryGetValue(_theme.GlowGoldMatrial, out value2))
			{
				value2.SetColor("_MainColor", _theme.GlowGoldColor);
				value2.SetFloat("_Falloff", _theme.GlowGoldFalloff);
				if (cameraCulling == null)
				{
					cameraCulling = UnityEngine.Object.FindObjectOfType(typeof(CameraCulling)) as CameraCulling;
				}
				cameraCulling.TransparentFXCullingDistance = _theme.GlowGoldFalloff;
			}
			Resources.UnloadUnusedAssets();
			if (this.ThemeChanged != null)
			{
				this.ThemeChanged(_theme);
			}
		}

		public Material ReplaceMaterial(Material sharedMaterial)
		{
			Material result = _original2CloneMaterials[sharedMaterial];
			List<Material> value;
			if (_original2DynamicCloneMaterials.TryGetValue(sharedMaterial, out value))
			{
				Material material = new Material(sharedMaterial);
				material.name = "replaced_" + sharedMaterial.name;
				value.Add(material);
				result = material;
			}
			return result;
		}

		public void ForceRefresh()
		{
			ResetTheme();
		}

		private void OnDestroy()
		{
			foreach (KeyValuePair<Material, Material> original2CloneMaterial in _original2CloneMaterials)
			{
				UnityEngine.Object.Destroy(original2CloneMaterial.Value);
			}
			_original2CloneMaterials.Clear();
			UIScreenController.OnApplicationResumed -= UIScreenController_OnApplicationResumed;
			if (_instance == this)
			{
				_applicationQuitting = true;
			}
		}

		private void SetShaderStates()
		{
			Shader.SetGlobalVector("_Distort", _distortion);
			Shader.SetGlobalColor("_SkyGradientTopColor", _theme.FogGradientTop);
			Shader.SetGlobalColor("_SkyGradientBottomColor", _theme.FogGradientBottom);
			Shader.SetGlobalColor("_FogSilhouetteColor", _theme.FogSilhouetteColor);
			Shader.SetGlobalFloat("_SkyGradientOffset", _theme.FogGradientOffset);
		}

		private void UIScreenController_OnApplicationResumed()
		{
			SetShaderStates();
		}
	}
}
