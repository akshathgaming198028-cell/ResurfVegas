using UnityEngine;

// Drag-to-scroll for the settings rows. Works from raw touch/mouse positions (like the Graphics tap),
// so it does not depend on NGUI's event routing. The rows are moved inside the clipped panel.
public class SettingsListScroll : MonoBehaviour
{
	// Half a row: the grid's top row starts at its localPosition.y and extends this far below.
	private const float ROW_HALF_HEIGHT = 24f;

	private Transform _grid;

	private UIGrid _uiGrid;

	private UIPanel _panel;

	private float _gridTopY;

	private bool _dragging;

	private float _lastY;

	public void Setup(UIGrid uiGrid, UIPanel panel)
	{
		_uiGrid = uiGrid;
		_grid = uiGrid.transform;
		_panel = panel;
		_gridTopY = _grid.localPosition.y;
	}

	private void OnEnable()
	{
		ResetOffset();
	}

	public void ResetOffset()
	{
		if (_grid != null)
		{
			Vector3 p = _grid.localPosition;
			p.y = _gridTopY;
			_grid.localPosition = p;
		}
	}

	private void Update()
	{
		if (_panel == null || _grid == null)
		{
			return;
		}
		Camera cam = UICamera.currentCamera != null ? UICamera.currentCamera : Camera.main;
		if (cam == null)
		{
			return;
		}
		Vector3 mouse = Input.mousePosition;
		if (Input.GetMouseButtonDown(0))
		{
			_dragging = IsInsideWindow(cam, mouse);
			_lastY = mouse.y;
		}
		if (Input.GetMouseButtonUp(0))
		{
			_dragging = false;
		}
		if (!_dragging || !Input.GetMouseButton(0))
		{
			return;
		}
		float pxPerUnit = PixelsPerLocalUnit(cam);
		if (pxPerUnit <= 0f)
		{
			return;
		}
		float dy = (mouse.y - _lastY) / pxPerUnit;
		_lastY = mouse.y;
		Vector3 p = _grid.localPosition;
		p.y = Mathf.Clamp(p.y + dy, _gridTopY, _gridTopY + MaxScroll());
		_grid.localPosition = p;
	}

	private bool IsInsideWindow(Camera cam, Vector3 mouse)
	{
		Vector4 clip = _panel.baseClipRegion;
		Transform tr = _panel.transform;
		float minX = float.MaxValue;
		float minY = float.MaxValue;
		float maxX = float.MinValue;
		float maxY = float.MinValue;
		for (int i = 0; i < 4; i++)
		{
			float x = clip.x + ((i % 2 == 0) ? -0.5f : 0.5f) * clip.z;
			float y = clip.y + ((i < 2) ? -0.5f : 0.5f) * clip.w;
			Vector3 sp = cam.WorldToScreenPoint(tr.TransformPoint(new Vector3(x, y, 0f)));
			minX = Mathf.Min(minX, sp.x);
			maxX = Mathf.Max(maxX, sp.x);
			minY = Mathf.Min(minY, sp.y);
			maxY = Mathf.Max(maxY, sp.y);
		}
		return mouse.x >= minX && mouse.x <= maxX && mouse.y >= minY && mouse.y <= maxY;
	}

	private float PixelsPerLocalUnit(Camera cam)
	{
		Transform tr = _panel.transform;
		Vector3 a = cam.WorldToScreenPoint(tr.TransformPoint(Vector3.zero));
		Vector3 b = cam.WorldToScreenPoint(tr.TransformPoint(Vector3.up));
		return Vector3.Distance(a, b);
	}

	// How far the rows may move up so the last active row can reach the bottom of the window.
	private float MaxScroll()
	{
		int rows = 0;
		foreach (Transform child in _grid)
		{
			if (child.gameObject.activeInHierarchy)
			{
				rows++;
			}
		}
		if (rows <= 0)
		{
			return 0f;
		}
		float cell = _uiGrid != null ? _uiGrid.cellHeight : 60f;
		Vector4 clip = _panel.baseClipRegion;
		float windowBottom = clip.y - clip.w * 0.5f;
		float contentBottom = _gridTopY - (rows - 1) * cell - ROW_HALF_HEIGHT;
		return Mathf.Max(0f, windowBottom - contentBottom);
	}
}
