using System;
using System.Collections;
using System.Collections.Generic;
using Boo.Lang;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(PerFrameRaycast))]
public class LaserScope : MonoBehaviour
{
	[Serializable]
	internal sealed class _0024ChoseNewAnimationTargetCoroutine_002483 : GenericGenerator<WaitForSeconds>
	{
		[Serializable]
		internal sealed class _0024 : GenericGeneratorEnumerator<WaitForSeconds>, IEnumerator
		{
			internal LaserScope _0024self__002484;

			public _0024(LaserScope self_)
			{
				_0024self__002484 = self_;
			}

			public override bool MoveNext()
			{
				int result;
				switch (_state)
				{
				default:
					_0024self__002484.aniDir = _0024self__002484.aniDir * 0.9f + UnityEngine.Random.Range(0.5f, 1.5f) * 0.1f;
					result = (YieldDefault(2) ? 1 : 0);
					break;
				case 2:
					_0024self__002484.minWidth = _0024self__002484.minWidth * 0.8f + UnityEngine.Random.Range(0.1f, 1f) * 0.2f;
					result = (Yield(3, new WaitForSeconds(1f + UnityEngine.Random.value * 2f - 1f)) ? 1 : 0);
					break;
				case 1:
					result = 0;
					break;
				}
				return (byte)result != 0;
			}
		}

		internal LaserScope _0024self__002485;

		public _0024ChoseNewAnimationTargetCoroutine_002483(LaserScope self_)
		{
			_0024self__002485 = self_;
		}

		public override IEnumerator<WaitForSeconds> GetEnumerator()
		{
			return new _0024(_0024self__002485);
		}
	}

	public float scrollSpeed;

	public float pulseSpeed;

	public float noiseSize;

	public float maxWidth;

	public float minWidth;

	public GameObject pointer;

	private LineRenderer lRenderer;

	private float aniTime;

	private float aniDir;

	private PerFrameRaycast raycast;

	public LaserScope()
	{
		scrollSpeed = 0.5f;
		pulseSpeed = 1.5f;
		noiseSize = 1f;
		maxWidth = 0.5f;
		minWidth = 0.2f;
		aniDir = 1f;
	}

	public virtual void Start()
	{
		lRenderer = ((LineRenderer)gameObject.GetComponent(typeof(LineRenderer))) as LineRenderer;
		aniTime = 0f;
		StartCoroutine_Auto(ChoseNewAnimationTargetCoroutine());
		raycast = GetComponent<PerFrameRaycast>();
	}

	public virtual IEnumerator ChoseNewAnimationTargetCoroutine()
	{
		return new _0024ChoseNewAnimationTargetCoroutine_002483(this).GetEnumerator();
	}

	public virtual void Update()
	{
		float x = GetComponent<Renderer>().material.mainTextureOffset.x + Time.deltaTime * aniDir * scrollSpeed;
		Vector2 mainTextureOffset = GetComponent<Renderer>().material.mainTextureOffset;
		float num = (mainTextureOffset.x = x);
		Vector2 vector = (GetComponent<Renderer>().material.mainTextureOffset = mainTextureOffset);
		GetComponent<Renderer>().material.SetTextureOffset("_NoiseTex", new Vector2((0f - Time.time) * aniDir * scrollSpeed, 0f));
		float b = Mathf.PingPong(Time.time * pulseSpeed, 1f);
		b = Mathf.Max(minWidth, b) * maxWidth;
		lRenderer.SetWidth(b, b);
		RaycastHit hitInfo = raycast.GetHitInfo();
		if ((bool)hitInfo.transform)
		{
			lRenderer.SetPosition(1, hitInfo.distance * Vector3.forward);
			float x2 = 0.1f * hitInfo.distance;
			Vector2 mainTextureScale = GetComponent<Renderer>().material.mainTextureScale;
			float num2 = (mainTextureScale.x = x2);
			Vector2 vector3 = (GetComponent<Renderer>().material.mainTextureScale = mainTextureScale);
			GetComponent<Renderer>().material.SetTextureScale("_NoiseTex", new Vector2(0.1f * hitInfo.distance * noiseSize, noiseSize));
			if ((bool)pointer)
			{
				pointer.GetComponent<Renderer>().enabled = true;
				pointer.transform.position = hitInfo.point + (transform.position - hitInfo.point) * 0.01f;
				pointer.transform.rotation = Quaternion.LookRotation(hitInfo.normal, transform.up);
				float x3 = 90f;
				Vector3 eulerAngles = pointer.transform.eulerAngles;
				float num3 = (eulerAngles.x = x3);
				Vector3 vector5 = (pointer.transform.eulerAngles = eulerAngles);
			}
		}
		else
		{
			if ((bool)pointer)
			{
				pointer.GetComponent<Renderer>().enabled = false;
			}
			float num4 = 200f;
			lRenderer.SetPosition(1, num4 * Vector3.forward);
			float x4 = 0.1f * num4;
			Vector2 mainTextureScale2 = GetComponent<Renderer>().material.mainTextureScale;
			float num5 = (mainTextureScale2.x = x4);
			Vector2 vector7 = (GetComponent<Renderer>().material.mainTextureScale = mainTextureScale2);
			GetComponent<Renderer>().material.SetTextureScale("_NoiseTex", new Vector2(0.1f * num4 * noiseSize, noiseSize));
		}
	}

	public virtual void Main()
	{
	}
}
