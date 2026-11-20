using System;
using System.Collections;
using System.Collections.Generic;
using Boo.Lang;
using Boo.Lang.Runtime;
using UnityEngine;

[Serializable]
public class EffectSequencer : MonoBehaviour
{
	[Serializable]
	internal sealed class _0024Start_002469 : GenericGenerator<object>
	{
		[Serializable]
		internal sealed class _0024 : GenericGeneratorEnumerator<object>, IEnumerator
		{
			internal ExplosionPart _0024go_002470;

			internal float _0024maxTime_002471;

			internal IEnumerator _0024_0024iterator_002465_002472;

			internal IEnumerator _0024_0024iterator_002466_002473;

			internal IEnumerator _0024_0024iterator_002467_002474;

			internal IEnumerator _0024_0024iterator_002468_002475;

			internal EffectSequencer _0024self__002476;

			public _0024(EffectSequencer self_)
			{
				_0024self__002476 = self_;
			}

			public override bool MoveNext()
			{
				int result;
				switch (_state)
				{
				default:
					_0024go_002470 = null;
					_0024maxTime_002471 = 0f;
					_0024_0024iterator_002465_002472 = _0024self__002476.ambientEmitters.GetEnumerator();
					while (_0024_0024iterator_002465_002472.MoveNext())
					{
						object obj2 = _0024_0024iterator_002465_002472.Current;
						if (!(obj2 is ExplosionPart))
						{
							obj2 = RuntimeServices.Coerce(obj2, typeof(ExplosionPart));
						}
						_0024go_002470 = (ExplosionPart)obj2;
						_0024self__002476.StartCoroutine_Auto(_0024self__002476.InstantiateDelayed(_0024go_002470));
						if ((bool)_0024go_002470.gameObject.GetComponent<ParticleEmitter>())
						{
							_0024maxTime_002471 = Mathf.Max(_0024maxTime_002471, _0024go_002470.delay + _0024go_002470.gameObject.GetComponent<ParticleEmitter>().maxEnergy);
						}
					}
					_0024_0024iterator_002466_002473 = _0024self__002476.explosionEmitters.GetEnumerator();
					while (_0024_0024iterator_002466_002473.MoveNext())
					{
						object obj3 = _0024_0024iterator_002466_002473.Current;
						if (!(obj3 is ExplosionPart))
						{
							obj3 = RuntimeServices.Coerce(obj3, typeof(ExplosionPart));
						}
						_0024go_002470 = (ExplosionPart)obj3;
						_0024self__002476.StartCoroutine_Auto(_0024self__002476.InstantiateDelayed(_0024go_002470));
						if ((bool)_0024go_002470.gameObject.GetComponent<ParticleEmitter>())
						{
							_0024maxTime_002471 = Mathf.Max(_0024maxTime_002471, _0024go_002470.delay + _0024go_002470.gameObject.GetComponent<ParticleEmitter>().maxEnergy);
						}
					}
					_0024_0024iterator_002467_002474 = _0024self__002476.smokeEmitters.GetEnumerator();
					while (_0024_0024iterator_002467_002474.MoveNext())
					{
						object obj4 = _0024_0024iterator_002467_002474.Current;
						if (!(obj4 is ExplosionPart))
						{
							obj4 = RuntimeServices.Coerce(obj4, typeof(ExplosionPart));
						}
						_0024go_002470 = (ExplosionPart)obj4;
						_0024self__002476.StartCoroutine_Auto(_0024self__002476.InstantiateDelayed(_0024go_002470));
						if ((bool)_0024go_002470.gameObject.GetComponent<ParticleEmitter>())
						{
							_0024maxTime_002471 = Mathf.Max(_0024maxTime_002471, _0024go_002470.delay + _0024go_002470.gameObject.GetComponent<ParticleEmitter>().maxEnergy);
						}
					}
					if ((bool)_0024self__002476.GetComponent<AudioSource>() && (bool)_0024self__002476.GetComponent<AudioSource>().clip)
					{
						_0024maxTime_002471 = Mathf.Max(_0024maxTime_002471, _0024self__002476.GetComponent<AudioSource>().clip.length);
					}
					result = (YieldDefault(2) ? 1 : 0);
					break;
				case 2:
					_0024_0024iterator_002468_002475 = _0024self__002476.miscSpecialEffects.GetEnumerator();
					while (_0024_0024iterator_002468_002475.MoveNext())
					{
						object obj = _0024_0024iterator_002468_002475.Current;
						if (!(obj is ExplosionPart))
						{
							obj = RuntimeServices.Coerce(obj, typeof(ExplosionPart));
						}
						_0024go_002470 = (ExplosionPart)obj;
						_0024self__002476.StartCoroutine_Auto(_0024self__002476.InstantiateDelayed(_0024go_002470));
						if ((bool)_0024go_002470.gameObject.GetComponent<ParticleEmitter>())
						{
							_0024maxTime_002471 = Mathf.Max(_0024maxTime_002471, _0024go_002470.delay + _0024go_002470.gameObject.GetComponent<ParticleEmitter>().maxEnergy);
						}
					}
					UnityEngine.Object.Destroy(_0024self__002476.gameObject, _0024maxTime_002471 + 0.5f);
					YieldDefault(1);
					goto case 1;
				case 1:
					result = 0;
					break;
				}
				return (byte)result != 0;
			}
		}

		internal EffectSequencer _0024self__002477;

		public _0024Start_002469(EffectSequencer self_)
		{
			_0024self__002477 = self_;
		}

		public override IEnumerator<object> GetEnumerator()
		{
			return new _0024(_0024self__002477);
		}
	}

	[Serializable]
	internal sealed class _0024InstantiateDelayed_002478 : GenericGenerator<WaitForSeconds>
	{
		[Serializable]
		internal sealed class _0024 : GenericGeneratorEnumerator<WaitForSeconds>, IEnumerator
		{
			internal ExplosionPart _0024go_002479;

			internal EffectSequencer _0024self__002480;

			public _0024(ExplosionPart go, EffectSequencer self_)
			{
				_0024go_002479 = go;
				_0024self__002480 = self_;
			}

			public override bool MoveNext()
			{
				int result;
				switch (_state)
				{
				default:
					result = (Yield(2, new WaitForSeconds(_0024go_002479.delay)) ? 1 : 0);
					break;
				case 2:
					UnityEngine.Object.Instantiate(_0024go_002479.gameObject, _0024self__002480.transform.position + Vector3.up * _0024go_002479.yOffset, _0024self__002480.transform.rotation);
					YieldDefault(1);
					goto case 1;
				case 1:
					result = 0;
					break;
				}
				return (byte)result != 0;
			}
		}

		internal ExplosionPart _0024go_002481;

		internal EffectSequencer _0024self__002482;

		public _0024InstantiateDelayed_002478(ExplosionPart go, EffectSequencer self_)
		{
			_0024go_002481 = go;
			_0024self__002482 = self_;
		}

		public override IEnumerator<WaitForSeconds> GetEnumerator()
		{
			return new _0024(_0024go_002481, _0024self__002482);
		}
	}

	public ExplosionPart[] ambientEmitters;

	public ExplosionPart[] explosionEmitters;

	public ExplosionPart[] smokeEmitters;

	public ExplosionPart[] miscSpecialEffects;

	public virtual IEnumerator Start()
	{
		return new _0024Start_002469(this).GetEnumerator();
	}

	public virtual IEnumerator InstantiateDelayed(ExplosionPart go)
	{
		return new _0024InstantiateDelayed_002478(go, this).GetEnumerator();
	}

	public virtual void Main()
	{
	}
}
