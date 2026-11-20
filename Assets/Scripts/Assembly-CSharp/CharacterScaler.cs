using UnityEngine;

public class CharacterScaler : MonoBehaviour
{
	public enum ScaleAnchorType
	{
		CharacterAnchor = 0,
		GameOverAnchor = 1,
		TutorialPupupAnchor = 2,
		CelebrationPopupAnchor = 3,
		LimitedCharacterPopupAnchor = 4
	}

	[SerializeField]
	public bool lookAtCamera;

	[SerializeField]
	private Camera _camera;

	[SerializeField]
	private ScaleAnchorType _anchorType;

	private float _posX = 90f;

	private float _posY = 225f;

	private static float _scaleMultiplierForRotation = 56f;

	private static float _scaleDelta = 450f;

	private UIRoot _root;

	private void Start()
	{
		if (_camera == null)
		{
			_camera = NGUITools.FindInParents<Camera>(base.gameObject);
		}
		_root = UIScreenController.Instance.root;
		SetScreenRelatedSettings();
		if (_root == null)
		{
			Debug.LogWarning("Root not set in the UIScreenController prefab");
		}
		ScaleCharacter();
		PositionCharacter();
		RotateCharacter();
	}

	private void Update()
	{
		if (lookAtCamera)
		{
			RotateCharacter();
			lookAtCamera = false;
		}
	}

	private void SetScreenRelatedSettings()
	{
		if (_anchorType == ScaleAnchorType.CharacterAnchor)
		{
			_posX = -75f;
			_posY = -45f;
			_scaleMultiplierForRotation = 56f;
			_scaleDelta = 450f;
		}
		else if (_anchorType == ScaleAnchorType.GameOverAnchor)
		{
			_posX = -70f;
			_posY = (float)(_root.manualHeight / 2) - 230f;
			_scaleMultiplierForRotation = 32f;
			_scaleDelta = 450f;
		}
		else if (_anchorType == ScaleAnchorType.TutorialPupupAnchor)
		{
			_posX = 92f;
			_posY = 40f;
			_scaleMultiplierForRotation = 0f;
			_scaleDelta = 380f;
		}
		else if (_anchorType == ScaleAnchorType.CelebrationPopupAnchor)
		{
			_posX = 0f;
			_posY = 0f;
			_scaleMultiplierForRotation = 0f;
			_scaleDelta = _root.manualHeight;
		}
		else if (_anchorType == ScaleAnchorType.LimitedCharacterPopupAnchor)
		{
			_posX = -75f;
			_posY = (float)_root.manualHeight * 0.5f - 45f;
			_scaleMultiplierForRotation = 56f;
			_scaleDelta = -400f;
		}
	}

	private void ScaleCharacter()
	{
		float num = _root.manualHeight;
		Transform transform = base.transform;
		Vector3 localPosition = transform.localPosition;
		localPosition.z = ((_scaleDelta == 0f) ? 0f : (num - _scaleDelta));
		transform.transform.localPosition = localPosition;
	}

	private void RotateCharacter()
	{
		if (_camera != null)
		{
			Vector3 zero = Vector3.zero;
			float num = (float)_root.manualHeight - 480f;
			float num2 = ((_scaleMultiplierForRotation == 0f) ? 0f : (num / _scaleMultiplierForRotation + 5f));
			zero.x = 0f - num2;
			zero.y = 0f;
			zero.z = 0f;
			base.gameObject.transform.localRotation = Quaternion.Euler(zero);
		}
		else
		{
			Debug.LogWarning("Camera not set in the CharacterModel prefab");
		}
	}

	private void PositionCharacter()
	{
		float posX = _posX;
		float posY = _posY;
		Transform transform = base.transform;
		Vector3 localPosition = transform.localPosition;
		localPosition.x = posX;
		localPosition.y = posY;
		transform.localPosition = localPosition;
	}
}
