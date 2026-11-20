using System;
using System.Collections;
using System.Collections.Generic;
using Boo.Lang;
using UnityEngine;

[Serializable]
public class ReceiverItem
{
	[Serializable]
	internal sealed class _0024SendWithDelay_002486 : GenericGenerator<WaitForSeconds>
	{
		[Serializable]
		internal sealed class _0024 : GenericGeneratorEnumerator<WaitForSeconds>, IEnumerator
		{
			internal MonoBehaviour _0024sender_002487;

			internal ReceiverItem _0024self__002488;

			public _0024(MonoBehaviour sender, ReceiverItem self_)
			{
				_0024sender_002487 = sender;
				_0024self__002488 = self_;
			}

			public override bool MoveNext()
			{
				int result;
				switch (_state)
				{
				default:
					result = (Yield(2, new WaitForSeconds(_0024self__002488.delay)) ? 1 : 0);
					break;
				case 2:
					if ((bool)_0024self__002488.receiver)
					{
						_0024self__002488.receiver.SendMessage(_0024self__002488.action);
					}
					else
					{
						Debug.LogWarning("No receiver of signal \"" + _0024self__002488.action + "\" on object " + _0024sender_002487.name + " (" + _0024sender_002487.GetType().Name + ")", _0024sender_002487);
					}
					YieldDefault(1);
					goto case 1;
				case 1:
					result = 0;
					break;
				}
				return (byte)result != 0;
			}
		}

		internal MonoBehaviour _0024sender_002489;

		internal ReceiverItem _0024self__002490;

		public _0024SendWithDelay_002486(MonoBehaviour sender, ReceiverItem self_)
		{
			_0024sender_002489 = sender;
			_0024self__002490 = self_;
		}

		public override IEnumerator<WaitForSeconds> GetEnumerator()
		{
			return new _0024(_0024sender_002489, _0024self__002490);
		}
	}

	public GameObject receiver;

	public string action;

	public float delay;

	public ReceiverItem()
	{
		action = "OnSignal";
	}

	public virtual IEnumerator SendWithDelay(MonoBehaviour sender)
	{
		return new _0024SendWithDelay_002486(sender, this).GetEnumerator();
	}
}
