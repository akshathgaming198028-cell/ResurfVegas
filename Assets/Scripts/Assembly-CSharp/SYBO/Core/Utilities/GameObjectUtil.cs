using System;
using UnityEngine;

namespace SYBO.Core.Utilities
{
	public static class GameObjectUtil
	{
		public static string GetLongName(this Component comp)
		{
			return Utils.GetLongName(comp.transform);
		}

		public static string GetLongName(this GameObject obj)
		{
			return Utils.GetLongName(obj.transform);
		}

		public static T AddComponentIfMissing<T>(this Component comp) where T : Component
		{
			return comp.gameObject.AddComponentIfMissing<T>();
		}

		public static T AddComponentIfMissing<T>(this GameObject go) where T : Component
		{
			T val = go.GetComponent<T>();
			if (val == null)
			{
				val = go.AddComponent<T>();
			}
			return val;
		}

		public static Component AddComponentIfMissing(this Component comp, Type type)
		{
			return comp.gameObject.AddComponentIfMissing(type);
		}

		public static Component AddComponentIfMissing(this GameObject go, Type type)
		{
			Component component = go.GetComponent(type);
			if (component == null)
			{
				component = go.AddComponent(type);
			}
			return component;
		}

		public static void ResetLocal(this Transform transform)
		{
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;
		}

		public static void ResetLocalPosition(this Transform transform)
		{
			transform.localPosition = Vector3.zero;
		}

		public static void ResetLocalRotation(this Transform transform)
		{
			transform.localRotation = Quaternion.identity;
		}

		public static void ResetLocalScale(this Transform transform)
		{
			transform.localScale = Vector3.one;
		}

		public static void UnparentAndDestroy(this GameObject go)
		{
			go.transform.parent = null;
			UnityEngine.Object.Destroy(go);
		}

		public static void SortTransformChildren(this Transform parentTransform, Comparison<Transform> cmp)
		{
			Transform[] array = new Transform[parentTransform.childCount];
			for (int i = 0; i < parentTransform.childCount; i++)
			{
				array[i] = parentTransform.GetChild(i);
			}
			Array.Sort(array, cmp);
			for (int j = 0; j < parentTransform.childCount; j++)
			{
				Transform transform = array[j];
				transform.SetSiblingIndex(j);
				transform.SortTransformChildren(cmp);
			}
		}

		public static bool IsPrefab(this GameObject go)
		{
			return false;
		}
	}
}
