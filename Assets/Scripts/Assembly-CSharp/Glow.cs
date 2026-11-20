using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Glow : MonoBehaviour
{
	private MeshRenderer meshRenderer;

	private bool _isDebugGlowEnabled;

	public bool IsDebugGlowEnabled
	{
		get
		{
			return _isDebugGlowEnabled;
		}
		internal set
		{
			_isDebugGlowEnabled = value;
			List<Transform> list = GetComponentsInChildren<Transform>().ToList();
			foreach (Transform item in list)
			{
				if (_isDebugGlowEnabled)
				{
					item.localScale = new Vector3(1f, 1f, 1f);
				}
				else
				{
					item.localScale = new Vector3(0f, 0f, 0f);
				}
			}
		}
	}

	public void Awake()
	{
		Transform transform = base.transform;
		Transform parent = transform.parent;
		Transform transform2 = ((!(parent == null)) ? parent.parent : null);
		if (DeviceInfo.Instance.performanceLevel == DeviceInfo.PerformanceLevel.Low && (parent.gameObject.name.Contains("coin") || (transform2 != null && transform2.gameObject.name.Contains("coin"))))
		{
			foreach (Transform item in base.transform)
			{
				Object.Destroy(item.gameObject);
			}
			meshRenderer = GetComponentInChildren<MeshRenderer>();
			if (meshRenderer != null)
			{
				meshRenderer.enabled = false;
				base.enabled = false;
				Object.Destroy(meshRenderer);
				meshRenderer = null;
			}
		}
		else
		{
			meshRenderer = GetComponentInChildren<MeshRenderer>();
		}
	}

	public void SetVisible(bool visible)
	{
		if (meshRenderer != null)
		{
			meshRenderer.enabled = visible;
			base.enabled = visible;
		}
	}
}
