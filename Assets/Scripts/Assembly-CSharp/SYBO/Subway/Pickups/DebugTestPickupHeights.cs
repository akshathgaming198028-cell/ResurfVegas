using System.Collections.Generic;
using UnityEngine;

namespace SYBO.Subway.Pickups
{
	public class DebugTestPickupHeights : MonoBehaviour
	{
		[SerializeField]
		private GameObject _placeholderPrefab;

		private List<GameObject> _placeholderList;

		private bool willPreview;

		protected void Awake()
		{
		}

		public void TogglePreviews()
		{
			willPreview = !willPreview;
			if (_placeholderList == null)
			{
				_placeholderList = new List<GameObject>();
			}
			foreach (GameObject placeholder in _placeholderList)
			{
				Object.DestroyImmediate(placeholder);
			}
			_placeholderList.Clear();
			if (willPreview)
			{
			}
			Debug.Log("TogglePreviews() willPreview: " + willPreview);
		}

		private GameObject InstantiatePlaceHolderPrefab(Vector3 newLocalPosition)
		{
			GameObject gameObject = Object.Instantiate(_placeholderPrefab);
			gameObject.transform.SetParent(base.transform, false);
			gameObject.transform.localPosition = newLocalPosition;
			GameObject gameObject2 = new GameObject("TextMesh");
			gameObject2.transform.SetParent(gameObject.transform, false);
			gameObject2.transform.localPosition = new Vector3(7f, 0f, 0f);
			TextMesh textMesh = gameObject2.AddComponent<TextMesh>();
			textMesh.fontSize = 50;
			textMesh.text = string.Empty + newLocalPosition.y;
			return gameObject;
		}
	}
}
