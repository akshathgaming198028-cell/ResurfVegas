using UnityEngine;

public class AssignTextureAndMaterial : MonoBehaviour
{
	[SerializeField]
	private string _textureName;

	private Material _material;

	private void Awake()
	{
		CharacterMaterial.Request(_textureName, CharacterMaterial.CharacterMaterialShaderType.Default, false, DeviceInfo.PerformanceLevel.Low, delegate(Material onSucess)
		{
			_material = onSucess;
			Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				renderer.material = _material;
			}
		});
	}

	private void OnDestroy()
	{
		Texture mainTexture = _material.mainTexture;
		if (mainTexture != null)
		{
			Object.Destroy(mainTexture);
		}
		Object.Destroy(_material);
	}
}
