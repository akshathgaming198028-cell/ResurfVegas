using System;
using System.Collections;
using System.Collections.Generic;
using Boo.Lang;
using UnityEngine;

[Serializable]
public class Health : MonoBehaviour
{
	[Serializable]
	internal sealed class _0024Regenerate_002491 : GenericGenerator<WaitForSeconds>
	{
		[Serializable]
		internal sealed class _0024 : GenericGeneratorEnumerator<WaitForSeconds>, IEnumerator
		{
			internal Health _0024self__002492;

			public _0024(Health self_)
			{
				_0024self__002492 = self_;
			}

			public override bool MoveNext()
			{
				int result;
				switch (_state)
				{
				default:
					if (!(_0024self__002492.regenerateSpeed <= 0f))
					{
						goto case 3;
					}
					goto IL_00db;
				case 2:
					if (!(_0024self__002492.health < _0024self__002492.maxHealth))
					{
						_0024self__002492.health = _0024self__002492.maxHealth;
						_0024self__002492.enabled = false;
					}
					goto IL_00b5;
				case 3:
					if (_0024self__002492.enabled)
					{
						if (!(Time.time <= _0024self__002492.lastDamageTime + 3f))
						{
							_0024self__002492.health += _0024self__002492.regenerateSpeed;
							result = (YieldDefault(2) ? 1 : 0);
							break;
						}
						goto IL_00b5;
					}
					goto IL_00db;
				case 1:
					{
						result = 0;
						break;
					}
					IL_00b5:
					result = (Yield(3, new WaitForSeconds(1f)) ? 1 : 0);
					break;
					IL_00db:
					YieldDefault(1);
					goto case 1;
				}
				return (byte)result != 0;
			}
		}

		internal Health _0024self__002493;

		public _0024Regenerate_002491(Health self_)
		{
			_0024self__002493 = self_;
		}

		public override IEnumerator<WaitForSeconds> GetEnumerator()
		{
			return new _0024(_0024self__002493);
		}
	}

	public float maxHealth;

	public float health;

	public float regenerateSpeed;

	public bool invincible;

	public bool dead;

	public GameObject damagePrefab;

	public Transform damageEffectTransform;

	public float damageEffectMultiplier;

	public bool damageEffectCentered;

	public GameObject scorchMarkPrefab;

	private GameObject scorchMark;

	public SignalSender damageSignals;

	public SignalSender dieSignals;

	private float lastDamageTime;

	private ParticleEmitter damageEffect;

	private float damageEffectCenterYOffset;

	private float colliderRadiusHeuristic;

	public Health()
	{
		maxHealth = 100f;
		health = 100f;
		damageEffectMultiplier = 1f;
		damageEffectCentered = true;
		colliderRadiusHeuristic = 1f;
	}

	public virtual void Awake()
	{
		enabled = false;
		if ((bool)damagePrefab)
		{
			if (damageEffectTransform == null)
			{
				damageEffectTransform = transform;
			}
			GameObject gameObject = Spawner.Spawn(damagePrefab, Vector3.zero, Quaternion.identity);
			gameObject.transform.parent = damageEffectTransform;
			gameObject.transform.localPosition = Vector3.zero;
			damageEffect = gameObject.GetComponent<ParticleEmitter>();
			colliderRadiusHeuristic = new Vector2(GetComponent<Collider>().bounds.extents.x, GetComponent<Collider>().bounds.extents.z).magnitude * 0.5f;
			damageEffectCenterYOffset = GetComponent<Collider>().bounds.extents.y;
		}
		if ((bool)scorchMarkPrefab)
		{
			scorchMark = (GameObject)UnityEngine.Object.Instantiate(scorchMarkPrefab, Vector3.zero, Quaternion.identity);
			scorchMark.SetActive(false);
		}
	}

	public virtual void OnDamage(float amount, Vector3 fromDirection)
	{
		if (invincible || dead || !(amount > 0f))
		{
			return;
		}
		health -= amount;
		damageSignals.SendSignals(this);
		lastDamageTime = Time.time;
		if (!(regenerateSpeed <= 0f))
		{
			enabled = true;
		}
		if ((bool)damageEffect)
		{
			damageEffect.transform.rotation = Quaternion.LookRotation(fromDirection, Vector3.up);
			if (!damageEffectCentered)
			{
				Vector3 vector = fromDirection;
				vector.y = 0f;
				damageEffect.transform.position = transform.position + Vector3.up * damageEffectCenterYOffset + colliderRadiusHeuristic * vector;
			}
			damageEffect.Emit();
		}
		if (!(health > 0f))
		{
			health = 0f;
			dead = true;
			dieSignals.SendSignals(this);
			enabled = false;
			if ((bool)scorchMark)
			{
				scorchMark.SetActive(true);
				Vector3 vector2 = GetComponent<Collider>().ClosestPointOnBounds(transform.position - Vector3.up * 100f);
				scorchMark.transform.position = vector2 + Vector3.up * 0.1f;
				float y = UnityEngine.Random.Range(0f, 90f);
				Vector3 eulerAngles = scorchMark.transform.eulerAngles;
				float num = (eulerAngles.y = y);
				Vector3 vector3 = (scorchMark.transform.eulerAngles = eulerAngles);
			}
		}
	}

	public virtual void OnEnable()
	{
		StartCoroutine_Auto(Regenerate());
	}

	public virtual IEnumerator Regenerate()
	{
		return new _0024Regenerate_002491(this).GetEnumerator();
	}

	public virtual void Main()
	{
	}
}
