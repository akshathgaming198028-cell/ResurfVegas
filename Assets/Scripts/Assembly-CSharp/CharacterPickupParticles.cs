using System;
using System.Collections;
using SYBO.Subway.Characters.States;
using UnityEngine;

public class CharacterPickupParticles : MonoBehaviour
{
	[Serializable]
	private class EffectDetails
	{
		public Transform target;

		public float duration = 0.05f;

		public AnimationCurve scaleCurve;
	}

	public static Vector3 coinEfxOffset = 1.2f * Vector3.forward;

	public GameObject CoinEFX;

	public GameObject PowerUpEFX;

	[OptionalField]
	public Transform master;

	public AudioClipInfo CoinPickup;

	public AudioClipInfo PowerUpPickup;

	public float CoinDistanceForStairway;

	private int coinStairway;

	private int flyWay;

	private int[] pentatonicScale = new int[17]
	{
		12, 13, 14, 15, 16, 17, 18, 19, 20, 21,
		22, 23, 24, 25, 26, 27, 28
	};

	private JetpackState jetpack;

	public AnimationCurve compressCurve;

	[SerializeField]
	private EffectDetails coinEffect;

	[SerializeField]
	private EffectDetails powerupEffect;

	private void Start()
	{
		jetpack = JetpackState.Instance;
		JetpackState jetpackState = jetpack;
		jetpackState.OnStop = (JetpackState.OnStopDelegate)Delegate.Combine(jetpackState.OnStop, new JetpackState.OnStopDelegate(OnJetpackStop));
	}

	private void OnJetpackStop()
	{
		flyWay = 0;
	}

	private void OnDestroy()
	{
		if (jetpack != null)
		{
			JetpackState jetpackState = jetpack;
			jetpackState.OnStop = (JetpackState.OnStopDelegate)Delegate.Remove(jetpackState.OnStop, new JetpackState.OnStopDelegate(OnJetpackStop));
		}
	}

	public void PickedUpCoin(Pickup pickup)
	{
		if (80f < pickup.transform.position.y)
		{
			coinStairway = 0;
			CoinPickup.maxPitch = Mathf.Pow(2f, compressCurve.Evaluate((float)flyWay / 48f));
			CoinPickup.minPitch = Mathf.Pow(2f, compressCurve.Evaluate((float)flyWay / 48f));
			flyWay++;
		}
		else if (pickup.transform.position.y < 0.1f || (8.795f < pickup.transform.position.y && pickup.transform.position.y < 8.805f) || (9.95f < pickup.transform.position.y && pickup.transform.position.y < 10.05f) || (28.95f < pickup.transform.position.y && pickup.transform.position.y < 29.05f) || (34.95f < pickup.transform.position.y && pickup.transform.position.y < 35.05f))
		{
			flyWay = 0;
			coinStairway = 0;
			CoinPickup.maxPitch = Mathf.Pow(2f, (float)pentatonicScale[coinStairway % pentatonicScale.Length] / 12f) * 0.5f;
			CoinPickup.minPitch = Mathf.Pow(2f, (float)pentatonicScale[coinStairway % pentatonicScale.Length] / 12f) * 0.5f;
		}
		else
		{
			flyWay = 0;
			if (coinStairway < pentatonicScale.Length - 1)
			{
				coinStairway++;
			}
			CoinPickup.maxPitch = Mathf.Pow(2f, (float)pentatonicScale[coinStairway % pentatonicScale.Length] / 12f) * 0.5f;
			CoinPickup.minPitch = Mathf.Pow(2f, (float)pentatonicScale[coinStairway % pentatonicScale.Length] / 12f) * 0.5f;
		}
		So.Instance.playSound(CoinPickup);
		StartCoroutine(EffectCoroutine(coinEffect));
	}

	private IEnumerator EffectCoroutine(EffectDetails details)
	{
		details.target.Rotate(0f, 0f, UnityEngine.Random.Range(0f, 360f));
		Material material = details.target.GetComponent<Renderer>().sharedMaterial;
		EffectDetails details2 = default(EffectDetails);
		yield return StartCoroutine(pTween.To(details.duration, delegate(float t)
		{
			details2.target.GetComponent<Renderer>().enabled = true;
			material.SetColor("_MainColor", Color.Lerp(Color.white, Color.black, t));
			details2.target.localScale = Vector3.one * details2.scaleCurve.Evaluate(t);
		}));
		details.target.GetComponent<Renderer>().enabled = false;
	}

	public void PickedUpPowerUp()
	{
		So.Instance.playSound(PowerUpPickup);
		PickedUpDefaultPowerUp();
	}

	public void PickedUpDefaultPowerUp()
	{
		StartCoroutine(EffectCoroutine(powerupEffect));
	}
}
