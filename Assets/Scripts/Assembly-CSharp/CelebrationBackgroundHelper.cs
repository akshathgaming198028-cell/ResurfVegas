using UnityEngine;

public class CelebrationBackgroundHelper : MonoBehaviour
{
	public Transform backgroundRoot;

	public Transform celebrationCloner;

	private bool shouldRotate;

	[SerializeField]
	private AnimationCurve _bragCelebrationAnimationCurve;

	public bool visible
	{
		get
		{
			return base.gameObject.activeSelf;
		}
		set
		{
			base.gameObject.SetActive(value);
		}
	}

	public void Hide()
	{
		visible = false;
	}

	public void Show(Quaternion rotation, bool shouldAnimateStripes, bool shouldRotateBackground)
	{
		if (!visible)
		{
			visible = true;
		}
		if (celebrationCloner.gameObject.activeSelf != shouldAnimateStripes)
		{
			celebrationCloner.gameObject.SetActive(shouldAnimateStripes);
		}
		shouldRotate = shouldRotateBackground;
		backgroundRoot.rotation = rotation;
		InvertParticlesDirection(false);
	}

	public void SetParticleAnimationSpeed(float speed)
	{
		ParticleSystem[] componentsInChildren = base.gameObject.GetComponentsInChildren<ParticleSystem>(false);
		foreach (ParticleSystem particleSystem in componentsInChildren)
		{
			particleSystem.playbackSpeed = speed;
		}
	}

	public void InvertParticlesDirection(bool invert)
	{
		if (invert)
		{
			celebrationCloner.localPosition = new Vector3(0f, 100f, 0f);
			celebrationCloner.localRotation = new Quaternion(0f, 0f, 180f, 0f);
		}
		else
		{
			celebrationCloner.localPosition = new Vector3(0f, -20f, 0f);
			celebrationCloner.localRotation = new Quaternion(0f, 0f, 0f, 0f);
		}
	}

	public float GetBragCelebrationAnimationCurveValue(float pos)
	{
		return _bragCelebrationAnimationCurve.Evaluate(pos);
	}

	private void Update()
	{
		if (shouldRotate)
		{
			backgroundRoot.Rotate(0f, 0f, 45f * Time.deltaTime);
		}
		_readoutTimer -= Time.deltaTime;
		if (_readoutTimer <= 0f)
		{
			_readoutTimer = 0.5f;
			RefreshReadout();
		}
	}

	// TEMPORARY on-screen readout for the new-highscore stripes (device check). Remove when fixed.
	private float _readoutTimer;

	private string _readout = "";

	private void RefreshReadout()
	{
		if (!Globals.DEBUG_CELEBRATION_READOUT || !base.gameObject.activeInHierarchy)
		{
			_readout = "";
			return;
		}
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		sb.AppendLine("GPU: " + SystemInfo.graphicsDeviceName + " | " + SystemInfo.graphicsDeviceType + " | " + SystemInfo.graphicsDeviceVersion);
		sb.AppendLine("Screen: " + Screen.width + "x" + Screen.height + " dpi " + Mathf.RoundToInt(Screen.dpi) + " | level " + DeviceInfo.Instance.performanceLevel + " | gpuCategory " + DeviceInfo.Instance.gpuCategory);
		sb.AppendLine("helper active " + base.gameObject.activeInHierarchy + " | cloner active " + (celebrationCloner != null && celebrationCloner.gameObject.activeInHierarchy));
		ParticleSystem[] systems = GetComponentsInChildren<ParticleSystem>(true);
		int playing = 0;
		foreach (ParticleSystem ps in systems)
		{
			if (ps.isPlaying)
			{
				playing++;
			}
		}
		sb.AppendLine("particle systems: " + systems.Length + " | playing: " + playing);
		Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
		int shown = 0;
		foreach (Renderer r in renderers)
		{
			if (shown >= 6)
			{
				break;
			}
			shown++;
			Material m = r.sharedMaterial;
			Shader sh = (m != null) ? m.shader : null;
			string matName = (m != null) ? m.name : "null";
			string shName = (sh != null) ? sh.name : "null";
			bool supported = sh != null && sh.isSupported;
			sb.AppendLine(" " + r.name + " | on " + r.gameObject.activeInHierarchy + " | enabled " + r.enabled + " | visible " + r.isVisible + " | mat " + matName + " | shader " + shName + " | supported " + supported);
		}
		_readout = sb.ToString();
	}

	private void OnGUI()
	{
		if (!Globals.DEBUG_CELEBRATION_READOUT || string.IsNullOrEmpty(_readout))
		{
			return;
		}
		GUI.Box(new Rect(10f, 120f, 1200f, 320f), _readout);
	}
}
