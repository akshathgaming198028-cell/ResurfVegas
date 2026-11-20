using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SYBO.Core;
using SYBO.Core.Components.Singleton;
using SYBO.Core.Utilities;
using UnityEngine;

public class TrackChunk : MonoBehaviour
{
	[Serializable]
	public class TrackCheckPoint
	{
		public int TrackNumber;

		public float Z;

		public TrackCheckPoint(int trackNumber, float z)
		{
			TrackNumber = trackNumber;
			Z = z;
		}
	}

	public const int JetpackZMin = 1000000;

	public const int JetpackZMax = -1;

	private Transform _transform;

	private List<TrackObject> _trackObjects = new List<TrackObject>();

	private Vector3 _initLocalPosition = Vector3.zero;

	private bool _active;

	private bool _deactivating;

	private int _numDeactivatePerFrame = 15;

	public float zSize = 90f;

	public int probability = 1;

	public float zMinimum;

	public bool zMaximumActive;

	public float zMaximum;

	public List<TrackCheckPoint> CheckPoints;

	public bool wasDisabledDueToHoverBoard;

	public bool isTutorial;

	public int TurboHeadstart;

	private Dictionary<Transform, Vector3> hiddenObstacles = new Dictionary<Transform, Vector3>();

	[Header("Runtime fields (unset)")]
	private TrackChunk _previous;

	private TrackChunk _next;

	public Transform Transform
	{
		get
		{
			return _transform;
		}
	}

	public bool Active
	{
		get
		{
			return base.gameObject.activeSelf && _active;
		}
	}

	public bool Deactivating
	{
		get
		{
			return _deactivating;
		}
	}

	public float ZWorldStart
	{
		get
		{
			return _transform.position.z;
		}
	}

	public float ZWorldEnd
	{
		get
		{
			return _transform.position.z + zSize;
		}
	}

	public TrackChunk Previous
	{
		get
		{
			return _previous;
		}
		set
		{
			if (value != null && _previous != null)
			{
				Debug.LogErrorFormat(this, "Chunk '{0}' - Trying to set previous chunk as '{1}', but it's alread set for to '{2}'", base.name, value.name, _previous.name);
			}
			_previous = value;
		}
	}

	public TrackChunk Next
	{
		get
		{
			return _next;
		}
		set
		{
			if (value != null && _next != null)
			{
				Debug.LogErrorFormat(this, "Chunk '{0}' - Trying to set next chunk as '{1}', but it's alread set for to '{2}'", base.name, value.name, _next.name);
			}
			_next = value;
		}
	}

	public event Action OnActivate;

	public event Action OnDeactivate;

	private void Awake()
	{
		_transform = base.transform;
		_initLocalPosition = _transform.localPosition;
		GetComponentsInChildren(true, _trackObjects);
		if (!zMaximumActive)
		{
			zMaximum = float.MaxValue;
		}
		if (TurboHeadstart == 0)
		{
			TrackChunkCollection.AddToChunks(this);
		}
		else if (TurboHeadstart == 1)
		{
			TrackChunkCollection.AddToTurboHeadstart1Chunks(this);
		}
		else if (TurboHeadstart == 2)
		{
			TrackChunkCollection.AddToTurboHeadstart2Chunks(this);
		}
	}

	private void Start()
	{
		base.gameObject.SetActive(false);
	}

	public IEnumerator Activate(int numActivatedPerFrame, ChunkStateChangePositionProvider activationPosProvider = null)
	{
		if (Active)
		{
			if (Application.isEditor)
			{
				Debug.LogWarning("TrackChunk repeated activation at " + Utils.GetLongName(_transform));
			}
			yield break;
		}
		_DoRemoveNullValuesFromTrackObjectArray();
		float reqActivationZ = 0f;
		if (activationPosProvider != null)
		{
			reqActivationZ = activationPosProvider.RequiredActivationZ;
		}
		int i = 0;
		foreach (TrackObject o in _trackObjects)
		{
			if (o.gameObject.activeInHierarchy)
			{
				o.Activate();
				i++;
			}
			int numActivatePerFrameDynamic = numActivatedPerFrame;
			if (activationPosProvider != null)
			{
				float distToRequriedActivationPoint = o.Transform.position.z - reqActivationZ;
				if (distToRequriedActivationPoint < 0f)
				{
					numActivatePerFrameDynamic = int.MaxValue;
				}
				else if (distToRequriedActivationPoint < 100f)
				{
					numActivatePerFrameDynamic = 2 * numActivatedPerFrame;
				}
			}
			if (numActivatePerFrameDynamic <= i)
			{
				yield return null;
				i = 0;
				if (activationPosProvider != null)
				{
					reqActivationZ = activationPosProvider.RequiredActivationZ;
				}
			}
		}
		if (this.OnActivate != null)
		{
			this.OnActivate();
		}
		_active = true;
	}

	public void Deactivate()
	{
		_deactivating = true;
		_transform.localPosition = _initLocalPosition;
		SingletonMonobehavior<WorkLoadQueue>.Instance.EnqueueWork(DeactivateAsync());
	}

	public IEnumerator DeactivateAsync(ChunkStateChangePositionProvider deactivationPosProvider = null)
	{
		if (!Active)
		{
			if (Application.isEditor)
			{
				Debug.LogWarningFormat(this, "TrackChunk repeated deactivation at '{0}'", Utils.GetLongName(_transform));
			}
			_deactivating = false;
			yield break;
		}
		_deactivating = true;
		if (!isTutorial)
		{
			_transform.localPosition = _initLocalPosition;
		}
		_DoRemoveNullValuesFromTrackObjectArray();
		if (this.OnDeactivate != null)
		{
			this.OnDeactivate();
		}
		float reqDeactivationZ = 0f;
		if (deactivationPosProvider != null)
		{
			reqDeactivationZ = deactivationPosProvider.RequiredDeactivationZ;
		}
		int i = 0;
		foreach (TrackObject o in _trackObjects)
		{
			if (!o.gameObject.activeInHierarchy)
			{
				continue;
			}
			i++;
			o.Deactivate();
			int numDeactivatePerFrameDynamic = _numDeactivatePerFrame;
			if (deactivationPosProvider != null)
			{
				float distToRequriedActivationPoint = o.Transform.position.z - reqDeactivationZ;
				if (distToRequriedActivationPoint < -1000f)
				{
					numDeactivatePerFrameDynamic = int.MaxValue;
				}
				else if (distToRequriedActivationPoint < 0f)
				{
					numDeactivatePerFrameDynamic = 2 * numDeactivatePerFrameDynamic;
				}
			}
			if (numDeactivatePerFrameDynamic <= i)
			{
				yield return null;
				i = 0;
			}
		}
		base.gameObject.SetActive(false);
		if (Next != null)
		{
			Next.Previous = null;
		}
		Next = null;
		Previous = null;
		_active = false;
		_deactivating = false;
	}

	public void DeactivateObstacles(float maxZ)
	{
		wasDisabledDueToHoverBoard = true;
		foreach (Transform item in base.transform)
		{
			DeactiveObstaclesRecursive(item, maxZ);
		}
	}

	private void DeactiveObstaclesRecursive(Transform target, float maxZ)
	{
		float num = ((!(target.GetComponent<Collider>() != null)) ? target.transform.position.z : target.GetComponent<Collider>().bounds.min.z);
		if (target.GetComponent<SnapObject>() == null)
		{
			foreach (Transform item in target)
			{
				DeactiveObstaclesRecursive(item, maxZ);
			}
			return;
		}
		if (num < maxZ && target.gameObject.layer != Layers.Instance.KeepOnHoverboard)
		{
			Vector3 localPosition = target.localPosition;
			if (!hiddenObstacles.ContainsKey(target))
			{
				hiddenObstacles.Add(target, localPosition);
			}
			target.localPosition = new Vector3(localPosition.x, -1000f, localPosition.z);
		}
	}

	public void RestoreHiddenObstacles()
	{
		foreach (KeyValuePair<Transform, Vector3> hiddenObstacle in hiddenObstacles)
		{
			if (hiddenObstacle.Key != null)
			{
				hiddenObstacle.Key.localPosition = hiddenObstacle.Value;
			}
		}
		hiddenObstacles.Clear();
	}

	public float GetLastCheckPoint(float characterZ)
	{
		TrackCheckPoint trackCheckPoint = (from c in CheckPoints
			orderby c.Z
			where c.Z <= characterZ
			select c).LastOrDefault();
		if (trackCheckPoint == null)
		{
			Debug.Log(" No checkpoint found in track chunk");
			return 0f;
		}
		return trackCheckPoint.Z;
	}

	private void DrawCheckPointGizmos()
	{
		if (CheckPoints == null)
		{
			return;
		}
		foreach (TrackCheckPoint checkPoint in CheckPoints)
		{
			Vector3 position = base.transform.position;
			position.z = checkPoint.Z;
			Gizmos.DrawSphere(position + Vector3.up * 5f, 5f);
		}
	}

	public void OnDrawGizmos()
	{
		DrawCheckPointGizmos();
		float num = 0f;
		Vector3 size = new Vector3(60f, 0f, 180f);
		for (; num < zSize; num += 180f)
		{
			Vector3 center = base.transform.position + new Vector3(0f, 0f, num + 90f);
			Gizmos.DrawWireCube(center, size);
		}
	}

	private void _DoRemoveNullValuesFromTrackObjectArray()
	{
		List<TrackObject> list = new List<TrackObject>();
		foreach (TrackObject trackObject in _trackObjects)
		{
			if (trackObject != null)
			{
				list.Add(trackObject);
			}
		}
		_trackObjects = list;
	}

	private void OnValidate()
	{
		Comparison<Transform> cmp = delegate(Transform t1, Transform t2)
		{
			int num = t1.localPosition.z.CompareTo(t2.localPosition.z);
			return (num == 0) ? t1.GetSiblingIndex().CompareTo(t2.GetSiblingIndex()) : num;
		};
		base.transform.SortTransformChildren(cmp);
	}

	public static bool IsJetpackChunk(float zMinimum, float zMaximum)
	{
		return 1000000f <= zMinimum && zMaximum <= -1f;
	}

	public static bool IsJetpackChunk(TrackChunk chunk)
	{
		return IsJetpackChunk(chunk.zMinimum, chunk.zMaximum);
	}
}
