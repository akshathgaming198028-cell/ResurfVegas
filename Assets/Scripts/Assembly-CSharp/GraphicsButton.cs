using Kiloo.Common.Performance;
using UnityEngine;

public class GraphicsButton : MonoBehaviour
{
	private UILabel _desc;

	// Debug readout, visible on screen so no logcat is needed. Shows whether taps reach the button.
	private static int _tapCount;

	private static string _lastEvent = "none";

	// Only High or Low is ever stored. Until the player picks one, the row shows the level this device starts with.
	public void Setup(UILabel desc)
	{
		_desc = desc;
		_Refresh();
	}

	public void Click()
	{
		_tapCount++;
		_lastEvent = "click " + _tapCount;
		int current = Settings.optionGraphics == 0 ? _DeviceDefault() : Settings.optionGraphics;
		Settings.optionGraphics = current == 1 ? 2 : 1;
		_Refresh();
	}

	private static int _DeviceDefault()
	{
		return DeviceInfo.Instance.gpuCategory == GPUCategory.High ? 1 : 2;
	}

	private void OnGUI()
	{
		if (!Globals.DEBUG_GRAPHICS_READOUT)
		{
			return;
		}
		string stored = Settings.optionGraphics.ToString();
		string applied = DeviceInfo.Instance.performanceLevel.ToString();
		string device = DeviceInfo.Instance.gpuCategory.ToString();
		GUI.Label(new Rect(10f, 10f, 1000f, 300f), "GFX taps=" + _tapCount + " last=" + _lastEvent + " stored=" + stored + " applied=" + applied + " device=" + device + " quality=" + QualitySettings.GetQualityLevel() + " fps=" + Application.targetFrameRate);
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
