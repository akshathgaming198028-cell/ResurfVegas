using UnityEngine;

public class GraphicsButton : MonoBehaviour
{
	private UILabel _desc;

	// Only High or Low is ever stored. Until the player picks one, the row shows the level this device starts with.
	public void Setup(UILabel desc)
	{
		_desc = desc;
		_Refresh();
	}

	public void Click()
	{
		int current = Settings.optionGraphics == 0 ? _DeviceDefault() : Settings.optionGraphics;
		Settings.optionGraphics = current == 1 ? 2 : 1;
		_Refresh();
	}

	private static int _DeviceDefault()
	{
		return DeviceInfo.Instance.gpuCategory == GPUCategory.High ? 1 : 2;
	}

	private void _Refresh()
	{
		if (_desc != null)
		{
			int current = Settings.optionGraphics == 0 ? _DeviceDefault() : Settings.optionGraphics;
			_desc.text = (current == 1 ? "High" : "Low") + ", restart to apply";
		}
	}
}
