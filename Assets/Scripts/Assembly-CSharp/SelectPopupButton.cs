using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(UIEventTrigger), typeof(BoxCollider))]
public class SelectPopupButton : MonoBehaviour
{
	[SerializeField]
	private List<string> _options = new List<string>();

	private Dictionary<string, string> _actualOptions;

	public bool multiSelect;

	[SerializeField]
	private UILabel _currentSelectedLabel;

	private List<string> _selectedItems;

	public List<EventDelegate> onChange = new List<EventDelegate>();

	public string value { get; private set; }

	public Dictionary<string, string> multipleValues { get; private set; }

	private void Awake()
	{
		multipleValues = new Dictionary<string, string>();
	}

	private void Start()
	{
		_actualOptions = _options.ToDictionary((string x) => x, (string x) => x);
		EventDelegate.Add(GetComponent<UIEventTrigger>().onClick, OnButtonClick);
		if (_currentSelectedLabel == null)
		{
			_currentSelectedLabel = GetComponentInChildren<UILabel>();
		}
		if (!multiSelect && string.IsNullOrEmpty(value))
		{
			value = _options[0];
		}
		NotifyOnChange();
	}

	public void SetItems(List<string> items, int selectedIndex = 0)
	{
		_options = items;
		_actualOptions = _options.ToDictionary((string x) => x, (string x) => x);
		if (!multiSelect)
		{
			value = _options[selectedIndex];
		}
		NotifyOnChange();
	}

	public void SetItems(Dictionary<string, string> items)
	{
		_options = items.Values.ToList();
		_actualOptions = items;
		if (!multiSelect)
		{
			value = _options[0];
		}
		NotifyOnChange();
	}

	private void OnButtonClick()
	{
		UIScreenController.Instance.PushPopup("ScrollListSelectPopup");
		ScrollListSelectPopup.instance.Initialize(_actualOptions, OnSelectPopupClosed, (!multiSelect) ? new List<string> { _currentSelectedLabel.text } : multipleValues.Keys.ToList(), multiSelect);
	}

	private void OnSelectPopupClosed(Dictionary<string, string> selectedItems)
	{
		_currentSelectedLabel.text = ((selectedItems.Count != 1) ? string.Format("{0} selected", selectedItems.Count) : selectedItems.Values.ElementAt(0));
		if (selectedItems.Count >= 1)
		{
			multipleValues = selectedItems;
			value = selectedItems.Values.ElementAt(0);
			NotifyOnChange();
		}
	}

	private void NotifyOnChange()
	{
		foreach (EventDelegate item in onChange)
		{
			item.Execute();
		}
	}
}
