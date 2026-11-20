using UnityEngine;

public class PickupPoint : MonoBehaviour
{
	private TrackObject _trackObject;

	private PickupManager _pickupManager;

	private void Awake()
	{
		_trackObject = base.gameObject.GetComponent<TrackObject>();
		_trackObject.OnActivate += OnActivate;
		_trackObject.OnDeactivate += OnDeactivate;
		_pickupManager = PickupManager.Instance;
	}

	private void OnActivate()
	{
		_pickupManager.AddPoint(base.transform);
	}

	private void OnDeactivate()
	{
		_pickupManager.RemovePoint(base.transform);
	}
}
