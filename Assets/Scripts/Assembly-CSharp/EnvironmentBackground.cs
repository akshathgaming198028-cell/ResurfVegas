using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SYBO.Subway.Cameras;
using SYBO.Subway.Level;
using SYBO.Subway.Managers;
using UnityEngine;

public class EnvironmentBackground : MonoBehaviour
{
	private Transform _characterCameraTransform;

	private Transform _transform;

	private ThemeManager _themeManager;

	[SerializeField]
	private float _leftLimit;

	[SerializeField]
	private float _rightLimit;

	[SerializeField]
	private GameObject _plane;

	private List<BackgroundObject> _backgroundObjects = new List<BackgroundObject>();

	private int _nextMonumentSpawnIndex;

	private List<BackgroundObject> _changingMonumentElements = new List<BackgroundObject>();

	private int _nextBackgroundSpawnIndex;

	private List<BackgroundObject> _changingBackgroundElements = new List<BackgroundObject>();

	private void Awake()
	{
		_transform = base.transform;
		_plane.SetActive(true);
		if (DemoVideoModeManager.IsEnabled)
		{
			Vector3 localScale = _plane.transform.localScale;
			localScale.x = 5f;
			_plane.transform.localScale = localScale;
		}
	}

	private void OnDestroy()
	{
		if (_themeManager != null)
		{
			_themeManager.ThemeChanged -= ThemeManager_ThemeChanged;
		}
	}

	private IEnumerator Start()
	{
		_characterCameraTransform = CharacterCamera.Instance.transform;
		_themeManager = ThemeManager.Instance;
		_themeManager.ThemeChanged += ThemeManager_ThemeChanged;
		ResetBackground(_themeManager.Theme);
		while (true)
		{
			_nextMonumentSpawnIndex = UpdateElementList(_changingMonumentElements, _nextMonumentSpawnIndex);
			_nextBackgroundSpawnIndex = UpdateElementList(_changingBackgroundElements, _nextBackgroundSpawnIndex);
			yield return new WaitForSeconds(2f);
		}
	}

	private void Update()
	{
		_transform.position = new Vector3(0f, 0f, _characterCameraTransform.position.z + 900f);
	}

	private void ThemeManager_ThemeChanged(Theme theme)
	{
		ResetBackground(theme);
	}

	public void ResetBackground(Theme theme)
	{
		ReadOnlyCollection<BackgroundObject> monumentPrefabs = theme.MonumentPrefabs;
		ReadOnlyCollection<BackgroundObject> backgroundPrefabs = theme.BackgroundPrefabs;
		foreach (BackgroundObject backgroundObject in _backgroundObjects)
		{
			Object.Destroy(backgroundObject.gameObject);
		}
		_backgroundObjects.Clear();
		_changingBackgroundElements.Clear();
		_changingMonumentElements.Clear();
		if (monumentPrefabs.Count != 0)
		{
			foreach (BackgroundObject item in monumentPrefabs)
			{
				GameObject gameObject = Object.Instantiate(item.gameObject);
				BackgroundObject component = gameObject.GetComponent<BackgroundObject>();
				_backgroundObjects.Add(component);
				Transform transform = gameObject.transform;
				transform.parent = _transform;
				transform.localPosition = new Vector3(_leftLimit, 0f, 0f);
				transform.localRotation = Quaternion.identity;
				ToggleElement(component, false);
				if (component.Movable)
				{
					_changingMonumentElements.Add(component);
				}
			}
			int num = Random.Range(0, _changingMonumentElements.Count - 1);
			if (num == _nextMonumentSpawnIndex)
			{
				if (num + 1 > _changingMonumentElements.Count - 1)
				{
					_nextMonumentSpawnIndex = 0;
				}
				else
				{
					_nextMonumentSpawnIndex = num + 1;
				}
			}
			else
			{
				_nextMonumentSpawnIndex = num;
			}
			if (_changingMonumentElements.Count > 0)
			{
				ToggleElement(_changingMonumentElements[_nextMonumentSpawnIndex], true);
			}
		}
		if (backgroundPrefabs.Count > 2)
		{
			foreach (BackgroundObject item2 in backgroundPrefabs)
			{
				GameObject gameObject2 = Object.Instantiate(item2.gameObject);
				BackgroundObject component2 = gameObject2.GetComponent<BackgroundObject>();
				_backgroundObjects.Add(component2);
				Transform transform2 = gameObject2.transform;
				transform2.parent = _transform;
				transform2.localPosition = new Vector3(_leftLimit, 0f, 0f);
				transform2.localRotation = Quaternion.identity;
				ToggleElement(component2, false);
				if (component2.Movable)
				{
					_changingBackgroundElements.Add(component2);
				}
			}
			int nextBackgroundSpawnIndex = Random.Range(0, _changingBackgroundElements.Count - 1);
			_nextBackgroundSpawnIndex = nextBackgroundSpawnIndex;
			_nextBackgroundSpawnIndex = PlaceElement(_nextBackgroundSpawnIndex, _changingBackgroundElements, _leftLimit * 0.33f);
			_nextBackgroundSpawnIndex = PlaceElement(_nextBackgroundSpawnIndex, _changingBackgroundElements, 0f);
			_nextBackgroundSpawnIndex = PlaceElement(_nextBackgroundSpawnIndex, _changingBackgroundElements, _rightLimit * 0.5f);
		}
		else
		{
			Debug.LogWarning("You need to have more then two background element to make it work!");
		}
	}

	private int PlaceElement(int spawnPosition, List<BackgroundObject> spawnList, float position)
	{
		spawnPosition = ((spawnPosition != spawnList.Count - 1) ? (spawnPosition + 1) : 0);
		ToggleElement(spawnList[spawnPosition], true);
		spawnList[spawnPosition].transform.localPosition = new Vector3(position, 0f, 0f);
		return spawnPosition;
	}

	private int UpdateElementList(List<BackgroundObject> bakcgroundObjects, int spawnElementIndex)
	{
		foreach (BackgroundObject bakcgroundObject in bakcgroundObjects)
		{
			if (bakcgroundObject.transform.localPosition.x > _rightLimit)
			{
				ToggleElement(bakcgroundObject, false);
				bakcgroundObject.transform.localPosition = new Vector3(_leftLimit, 0f, 0f);
				bakcgroundObject.GetComponent<MoveGameObject>().model.localScale = Vector3.zero;
				ToggleElement(bakcgroundObjects[spawnElementIndex], true);
				spawnElementIndex = ((spawnElementIndex != bakcgroundObjects.Count - 1) ? (spawnElementIndex + 1) : 0);
			}
		}
		return spawnElementIndex;
	}

	private void ToggleElement(BackgroundObject bo, bool on)
	{
		if (!bo.AlwaysVisible)
		{
			bo.gameObject.SetActive(on);
		}
	}
}
