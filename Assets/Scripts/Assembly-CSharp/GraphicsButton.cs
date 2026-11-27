using Kiloo.Common.Performance;
using UnityEngine;

public class GraphicsButton : MonoBehaviour
{
	private UILabel _title;

	private UILabel _desc;

	private UIWidget[] _hitAreas = new UIWidget[0];

	// Only High (1) or Low (2) is ever stored. 0 means the player has not picked yet, so the device default is shown.
	public void Setup(UILabel title, UILabel desc)
	{
		_title = title;
		_desc = desc;
		_Refresh();
	}

	public void SetHitAreas(UIWidget[] areas)
	{
		_hitAreas = areas;
	}

	private void Update()
	{
		if (!Input.GetMouseButtonUp(0) || !gameObject.activeInHierarchy)
		{
			return;
		}
		Camera cam = UICamera.currentCamera != null ? UICamera.currentCamera : Camera.main;
		if (cam == null)
		{
			return;
		}
		Vector3 p = Input.mousePosition;
		foreach (UIWidget area in _hitAreas)
		{
			if (area == null || !area.gameObject.activeInHierarchy)
			{
				continue;
			}
			Vector3[] corners = area.worldCorners;
			float minX = float.MaxValue;
			float minY = float.MaxValue;
			float maxX = float.MinValue;
			float maxY = float.MinValue;
			foreach (Vector3 corner in corners)
			{
				Vector3 sp = cam.WorldToScreenPoint(corner);
				minX = Mathf.Min(minX, sp.x);
				maxX = Mathf.Max(maxX, sp.x);
				minY = Mathf.Min(minY, sp.y);
				maxY = Mathf.Max(maxY, sp.y);
			}
			if (p.x >= minX && p.x <= maxX && p.y >= minY && p.y <= maxY)
			{
				Click();
				return;
			}
		}
	}

	public void Click()
	{
		int current = _Current();
		Settings.optionGraphics = current == 1 ? 2 : 1;
		_Refresh();
	}

	private static int _Current()
	{
		if (Settings.optionGraphics != 0)
		{
			return Settings.optionGraphics;
		}
		return DeviceInfo.Instance.gpuCategory == GPUCategory.High ? 1 : 2;
	}

	private void _Refresh()
	{
		if (_title != null)
		{
			_title.text = "Graphics: " + (_Current() == 1 ? "High" : "Low");
		}
		if (_desc != null)
		{
			_desc.text = "Restart to apply";
		}
	}
}
