using System.Collections;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class TableElementAlwaysVisible : MonoBehaviour
{
	[SerializeField]
	private UIPanel _scrollPanel;

	[SerializeField]
	private TweenHeight _largestTweenHeight;

	private float _scrollPanelStartY;

	private UpgradeScreen _parrent;

	private int _index;

	public float ScrollPanelStartY
	{
		set
		{
			_scrollPanelStartY = value;
		}
	}

	public bool IsExpanded
	{
		get
		{
			if (_largestTweenHeight.cachedWidget.height == _largestTweenHeight.from)
			{
				return false;
			}
			return true;
		}
	}

	public int Index
	{
		get
		{
			return _index;
		}
		set
		{
			_index = value;
		}
	}

	public UpgradeScreen Parrent
	{
		set
		{
			_parrent = value;
		}
	}

	private void Start()
	{
		if (_scrollPanel == null)
		{
			_scrollPanel = NGUITools.FindInParents<UIPanel>(base.gameObject);
		}
	}

	private void TheElementIsOnBottom(float bottomPointOnScreen, float bottomPointOfTable)
	{
		if (AreAllClosed())
		{
			if (WillExpand())
			{
				float noOfPixels = bottomPointOfTable - bottomPointOnScreen;
				StartCoroutine(MovePanelAfterFrames(noOfPixels, 3));
			}
		}
		else if (hasAnotherElementOpenOnBottom())
		{
			if (WillExpand())
			{
				float noOfPixels2 = bottomPointOfTable - bottomPointOnScreen;
				MovePanelBy(noOfPixels2);
			}
		}
		else if (hasAnotherElementOpenOnTop() && WillExpand())
		{
			float num = bottomPointOfTable - bottomPointOnScreen;
			MovePanelBy(num - (float)(_largestTweenHeight.to - _largestTweenHeight.from));
		}
	}

	private void TheElementIsOnTop(float topPointOnScreen, float topPointOfTable)
	{
		float num = 10f;
		if (AreAllClosed())
		{
			if (WillExpand())
			{
				float noOfPixels = topPointOfTable - topPointOnScreen - num;
				MovePanelBy(noOfPixels);
			}
		}
		else if (hasAnotherElementOpenOnTop())
		{
			if (WillExpand())
			{
				float num2 = 0f - (topPointOnScreen - topPointOfTable) - num;
				MovePanelBy((float)(-(_largestTweenHeight.to - _largestTweenHeight.from)) + num2);
			}
		}
		else if (WillExpand())
		{
			float noOfPixels2 = 0f - (topPointOnScreen - topPointOfTable) - num;
			MovePanelBy(noOfPixels2);
		}
	}

	private void OnClick()
	{
		ToggleButtonScale();
	}

	public void ToggleButtonScale()
	{
		float num = (float)(_largestTweenHeight.to - _largestTweenHeight.from) / 2f;
		float num2 = ((!WillExpand()) ? (base.gameObject.transform.localPosition.y + num) : (base.gameObject.transform.localPosition.y - num));
		float y = _scrollPanel.transform.localPosition.y;
		float num3 = ((!WillExpand()) ? _largestTweenHeight.from : _largestTweenHeight.to);
		float num4 = num2 - num3 / 2f;
		float num5 = num4 + y;
		float num6 = 0f - _scrollPanel.baseClipRegion.w + _scrollPanelStartY;
		float num7 = num2 + num3 / 2f;
		float num8 = num7 + y;
		float scrollPanelStartY = _scrollPanelStartY;
		if (num5 < num6)
		{
			TheElementIsOnBottom(num5, num6);
		}
		bool flag = false;
		if (hasAnotherElementOpenOnTop())
		{
			float num9 = num8 + (float)(_largestTweenHeight.to - _largestTweenHeight.from);
			if (num9 > scrollPanelStartY)
			{
				TheElementIsOnTop(num8, scrollPanelStartY);
				flag = true;
			}
		}
		if (num8 > scrollPanelStartY && !flag)
		{
			TheElementIsOnTop(num8, scrollPanelStartY);
		}
		if (!WillExpand())
		{
			_parrent.IndexExpanded = _index;
		}
	}

	private IEnumerator MovePanelAfterFrames(float noOfPixels, int frames)
	{
		int numberOfFrames = 0;
		while (numberOfFrames < frames)
		{
			numberOfFrames++;
			yield return null;
		}
		MovePanelBy(noOfPixels);
	}

	private void MovePanelBy(float noOfPixels)
	{
		Vector3 localPosition = _scrollPanel.transform.localPosition;
		localPosition.y += noOfPixels;
		_scrollPanel.transform.localPosition = localPosition;
		Vector2 zero = Vector2.zero;
		zero.y = noOfPixels;
		_scrollPanel.clipOffset -= zero;
	}

	private bool hasAnotherElementOpenOnTop()
	{
		return _parrent.IndexExpanded < _index && _parrent.IndexExpanded != -1;
	}

	private bool hasAnotherElementOpenOnBottom()
	{
		return _parrent.IndexExpanded > _index && _parrent.IndexExpanded != -1;
	}

	private bool WillExpand()
	{
		if (_largestTweenHeight.cachedWidget.height == _largestTweenHeight.from)
		{
			return true;
		}
		return false;
	}

	private bool AreAllClosed()
	{
		return !_parrent.IsLastElementOpen();
	}

	private void OnDestroy()
	{
		_parrent = null;
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	private static void LogError(string msg, Object context = null)
	{
		Debug.LogError(msg, context);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	private static void LogWarning(string msg, Object context = null)
	{
		Debug.LogWarning(msg, context);
	}

	[Conditional("ENABLE_TABLE_ELEMENT_ALWAYS_VISIBLE_LOGS")]
	private static void Log(string msg, Object context = null)
	{
		Debug.Log(msg, context);
	}
}
