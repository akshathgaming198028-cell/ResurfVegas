using UnityEngine;

namespace Kiloo.Common.Extensions
{
	public static class TransformExtensions
	{
		public static void Reset(this Transform t)
		{
			t.localPosition = Vector3.zero;
			t.localRotation = Quaternion.identity;
			t.localScale = Vector3.one;
		}

		public static void SetLocalScaleX(this Transform transform, float scaleX)
		{
			transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
		}

		public static void SetLocalScaleY(this Transform transform, float scaleY)
		{
			transform.localScale = new Vector3(transform.localScale.x, scaleY, transform.localScale.z);
		}

		public static void SetLocalScaleZ(this Transform transform, float scaleZ)
		{
			transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, scaleZ);
		}

		public static void SetLocalPosX(this Transform transform, float posX)
		{
			transform.localPosition = new Vector3(posX, transform.localPosition.y, transform.localPosition.z);
		}

		public static void SetLocalPosY(this Transform transform, float posY)
		{
			transform.localPosition = new Vector3(transform.localPosition.x, posY, transform.localPosition.z);
		}

		public static void SetLocalPosZ(this Transform transform, float posZ)
		{
			transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, posZ);
		}

		public static void SetPosX(this Transform transform, float posX)
		{
			transform.position = new Vector3(posX, transform.position.y, transform.position.z);
		}

		public static void SetPosY(this Transform transform, float posY)
		{
			transform.position = new Vector3(transform.position.x, posY, transform.position.z);
		}

		public static void SetPosZ(this Transform transform, float posZ)
		{
			transform.position = new Vector3(transform.position.x, transform.position.y, posZ);
		}
	}
}
