using SYBO.Core.Components.Debugging;
using UnityEngine;

namespace SYBO.Core.Utilities
{
	public class HierarchyUtility : MonoBehaviour
	{
		public static GameObject GetGameObjectByHierarchyPathType(HierarchyPathType hierarchyPathType)
		{
			GameObject gameObject = null;
			switch (hierarchyPathType)
			{
			case HierarchyPathType.Null:
				gameObject = null;
				break;
			case HierarchyPathType.Root:
				if (!_HasGameObjectByHierarchyPathString("_Root"))
				{
					gameObject = _GetGameObjectByHierarchyPathString("_Root");
					CommentComponent commentComponent = _GetComponentOrAddComponent<CommentComponent>(gameObject);
					commentComponent.Comment = "The '_Root' GameObject is created on-demand by HierarchyUtility.";
					commentComponent.IsCommentEditableInInspector = false;
				}
				else
				{
					gameObject = _GetGameObjectByHierarchyPathString("_Root");
				}
				gameObject.transform.SetAsFirstSibling();
				break;
			case HierarchyPathType.Dynamic:
			{
				gameObject = _GetGameObjectByHierarchyPathString("_Root/Dynamic");
				gameObject.transform.parent = GetGameObjectByHierarchyPathType(HierarchyPathType.Root).transform;
				gameObject.transform.SetAsFirstSibling();
				CommentComponent commentComponent = _GetComponentOrAddComponent<CommentComponent>(gameObject);
				commentComponent.Comment = "The 'Dynamic' GameObject is created on-demand by HierarchyUtility. It holds managers and other dynamic instances.\n Please do not remove it.";
				commentComponent.IsCommentEditableInInspector = false;
				break;
			}
			case HierarchyPathType.DynamicManagers:
			{
				gameObject = _GetGameObjectByHierarchyPathString("_Root/Dynamic/Managers");
				gameObject.transform.parent = GetGameObjectByHierarchyPathType(HierarchyPathType.Dynamic).transform;
				CommentComponent commentComponent = _GetComponentOrAddComponent<CommentComponent>(gameObject);
				commentComponent.Comment = "The 'Managers' GameObject is created on-demand by HierarchyUtility. It holds managers and other dynamic instances.\n Please do not remove it.";
				commentComponent.IsCommentEditableInInspector = false;
				break;
			}
			case HierarchyPathType.Manual:
			{
				gameObject = _GetGameObjectByHierarchyPathString("_Root/Manual");
				gameObject.transform.parent = GetGameObjectByHierarchyPathType(HierarchyPathType.Root).transform;
				gameObject.transform.SetSiblingIndex(1);
				CommentComponent commentComponent = _GetComponentOrAddComponent<CommentComponent>(gameObject);
				commentComponent.Comment = "The 'Manual' GameObject is created on-demand by HierarchyUtility. Manually drag in children as you like. Please do not remove it.";
				commentComponent.IsCommentEditableInInspector = false;
				break;
			}
			case HierarchyPathType.ManualManagers:
			{
				gameObject = _GetGameObjectByHierarchyPathString("_Root/Manual/Managers");
				gameObject.transform.parent = GetGameObjectByHierarchyPathType(HierarchyPathType.Manual).transform;
				CommentComponent commentComponent = _GetComponentOrAddComponent<CommentComponent>(gameObject);
				commentComponent.Comment = "The 'Managers' GameObject is created on-demand by HierarchyUtility. Manually drag in children as you like. Please do not remove it.";
				commentComponent.IsCommentEditableInInspector = false;
				break;
			}
			}
			return gameObject;
		}

		public static void ReparentByHierarchyPathType(Transform childTransform, HierarchyPathType hierarchyPathType, bool worldPositionStays = false)
		{
			GameObject gameObjectByHierarchyPathType = GetGameObjectByHierarchyPathType(hierarchyPathType);
			if (gameObjectByHierarchyPathType == null)
			{
				childTransform.SetParent(null, worldPositionStays);
			}
			else
			{
				childTransform.SetParent(gameObjectByHierarchyPathType.transform, worldPositionStays);
			}
		}

		public static void ReparentByHierarchyPathType(Transform childTransform, HierarchyPathType hierarchyPathType, Vector3 newWorldPosition)
		{
			ReparentByHierarchyPathType(childTransform, hierarchyPathType);
			childTransform.position = newWorldPosition;
		}

		public static void Reparent(Transform childTransform, Transform parentTransform, bool worldPositionStays = false)
		{
			childTransform.SetParent(parentTransform, worldPositionStays);
		}

		public static void Reparent(Transform childTransform, Transform parentTransform, Vector3 newLocalPosition)
		{
			childTransform.SetParent(parentTransform);
			childTransform.localPosition = newLocalPosition;
		}

		private static GameObject _GetGameObjectByHierarchyPathString(string hierarchyPathString)
		{
			GameObject gameObject = GameObject.Find(hierarchyPathString);
			if (gameObject == null)
			{
				gameObject = new GameObject(hierarchyPathString);
			}
			return gameObject;
		}

		private static T _GetComponentOrAddComponent<T>(GameObject go) where T : Component
		{
			T val = go.GetComponent<T>();
			if (val == null)
			{
				val = go.AddComponent<T>();
			}
			return val;
		}

		private static bool _HasGameObjectByHierarchyPathString(string hierarchyPathString)
		{
			GameObject gameObject = GameObject.Find(hierarchyPathString);
			if (gameObject == null)
			{
				return false;
			}
			return true;
		}
	}
}
