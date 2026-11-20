using System;
using System.Collections;
using UnityEngine;

public class SSEventsLabelTimer : MonoBehaviour
{
	[SerializeField]
	private float _timespanToRefreshTimeLabel;

	private UILabel _thisLabel;

	private void Awake()
	{
		StartCoroutine("ChangeEventBtnLabelText");
	}

	private void OnDisable()
	{
		StopCoroutine("ChangeEventBtnLabelText");
	}

	private void OnEnable()
	{
		StartCoroutine("ChangeEventBtnLabelText");
	}

	private IEnumerator ChangeEventBtnLabelText()
	{
		if (_thisLabel == null)
		{
			_thisLabel = base.gameObject.GetComponent<UILabel>();
			if (_thisLabel == null)
			{
				Debug.LogError("This MonoBehavior should be put only on UILabels!");
			}
		}
		TimeSpan ts = TimeSpan.FromSeconds(SSEventsManagerController.instance.TimeLeftInSeconds());
		string _eventBtnText = Strings.Get(StringID.EVENTS_POPUP_BTN_TIMELEFT);
		int totalHours = (int)ts.TotalHours;
		int totalMinutes = (int)ts.TotalMinutes;
		if (totalHours > 0)
		{
			if (totalHours > 1)
			{
				_thisLabel.text = string.Format(_eventBtnText, totalHours, Strings.Get(StringID.HOUR_ABREVIATED_PLURAL));
			}
			else
			{
				_thisLabel.text = string.Format(_eventBtnText, totalHours, Strings.Get(StringID.HOUR_ABREVIATED_SINGULAR));
			}
		}
		else if (totalMinutes > 1)
		{
			_thisLabel.text = string.Format(_eventBtnText, totalMinutes, Strings.Get(StringID.MINUTE_ABREVIATED_PLURAL));
		}
		else
		{
			_thisLabel.text = string.Format(_eventBtnText, totalMinutes, Strings.Get(StringID.MINUTE_ABREVIATED_SINGULAR));
		}
		yield return new WaitForSeconds(_timespanToRefreshTimeLabel);
		StartCoroutine(ChangeEventBtnLabelText());
	}
}
