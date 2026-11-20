using UnityEngine;

public class TopRunState : MonoBehaviour
{
	[SerializeField]
	private GameObject _normalStateParent;

	private TopRunNormalStateHelper _normalStateHelper;

	[SerializeField]
	private GameObject _resultsStateParent;

	[SerializeField]
	private GameObject _offlineStateParent;

	private void Awake()
	{
		_normalStateHelper = _normalStateParent.GetComponent<TopRunNormalStateHelper>();
	}

	public void SetState(TopRunStates newState)
	{
		ResetStates();
		switch (newState)
		{
		case TopRunStates.NORMAL:
			SetNormalState();
			break;
		case TopRunStates.RESULTS:
			SetResultsState();
			break;
		case TopRunStates.OFFLINE:
			SetOfflineState();
			break;
		default:
			Debug.LogWarning("The following state was not found: " + newState.ToString() + ". Button set to offline mode.");
			SetOfflineState();
			break;
		}
	}

	private void ResetStates()
	{
		_normalStateParent.SetActive(false);
		_resultsStateParent.SetActive(false);
		_offlineStateParent.SetActive(false);
	}

	private void SetNormalState()
	{
		_normalStateParent.SetActive(true);
		_normalStateHelper.InitNormalState();
	}

	private void SetResultsState()
	{
		_resultsStateParent.SetActive(true);
	}

	private void SetOfflineState()
	{
		_offlineStateParent.SetActive(true);
	}
}
