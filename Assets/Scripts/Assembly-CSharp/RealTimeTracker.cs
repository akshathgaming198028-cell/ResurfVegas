using UnityEngine;

public class RealTimeTracker : MonoBehaviour
{
	private static RealTimeTracker mInst;

	private float mRealTime;

	private float mRealDelta;

	public static float time
	{
		get
		{
			if (mInst == null)
			{
				Spawn();
			}
			return mInst.mRealTime;
		}
	}

	public static float deltaTime
	{
		get
		{
			if (mInst == null)
			{
				Spawn();
			}
			return mInst.mRealDelta;
		}
	}

	private static void Spawn()
	{
		GameObject gameObject = new GameObject("_RealTimeTracker");
		Object.DontDestroyOnLoad(gameObject);
		mInst = gameObject.AddComponent<RealTimeTracker>();
		mInst.mRealTime = Time.realtimeSinceStartup;
	}

	private void Update()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		mRealDelta = Mathf.Clamp01(realtimeSinceStartup - mRealTime);
		mRealTime = realtimeSinceStartup;
	}
}
