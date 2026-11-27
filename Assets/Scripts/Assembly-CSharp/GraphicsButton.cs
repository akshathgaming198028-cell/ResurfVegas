using UnityEngine;

public class GraphicsButton : MonoBehaviour
{
	private static readonly string[] MODE_NAMES = new string[3] { "Auto", "High", "Low" };

	private UILabel _label;

	public void Setup(UILabel label)
	{
		_label = label;
		_Refresh();
	}

	public void Click()
	{
		Settings.optionGraphics = (Settings.optionGraphics + 1) % 3;
		_Refresh();
	}

	private void _Refresh()
	{
		if (_label != null)
		{
			_label.text = "Graphics: " + MODE_NAMES[Settings.optionGraphics] + " (restart to apply)";
		}
	}
}
