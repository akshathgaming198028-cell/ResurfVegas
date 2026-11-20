using System;
using System.Collections.Generic;

public class DebugSettingsCell
{
	public DebugSettingsCellType cellType;

	public DebugSetting settingType;

	public string cellTitle;

	public string cellDescription;

	public string cellFunctionName;

	public ButtonIcon cellButtonIcon;

	public bool multiSelect;

	public Action callback;

	public Dictionary<string, string> _items { get; private set; }
}
