using System.Collections;
using SYBO.Subway;
using SYBO.Subway.Characters;
using UnityEngine;

public class TutorialEvent : MonoBehaviour
{
	private Game game;

	[SerializeField]
	private bool displayText;

	[SerializeField]
	private string text;

	[SerializeField]
	private bool displayMesh;

	[SerializeField]
	private float direction;

	[SerializeField]
	private float time = 1f;

	[SerializeField]
	private bool endTutorial;

	[SerializeField]
	private bool allowHoverboard;

	private Hoverboard hoverboard;

	private Character character;

	private Track track;

	private bool _initialized;

	private GameObject _mesh;

	private GameObject mesh
	{
		get
		{
			if (_mesh == null)
			{
				_mesh = Camera.main.gameObject.transform.FindChild("arrow").gameObject;
			}
			return _mesh;
		}
	}

	private void Awake()
	{
		game = Game.Instance;
		hoverboard = Hoverboard.Instance;
	}

	private void Start()
	{
		if (!(game == null) && !_initialized)
		{
			character = game.character;
			track = game.track;
			_initialized = true;
		}
	}

	private IEnumerator ShowArrow()
	{
		mesh.transform.rotation = Quaternion.AngleAxis(direction, new Vector3(0f, 0f, 1f)) * Quaternion.Euler(0f, 180f, 0f);
		mesh.SetActive(true);
		Vector3 pos = new Vector3(0f, 0f, 20f);
		yield return StartCoroutine(pTween.To(time, delegate(float t)
		{
			mesh.transform.localPosition = Vector3.Lerp(pos - mesh.transform.up * 5f, pos + mesh.transform.up * 5f, t);
			mesh.GetComponent<Renderer>().material.mainTextureOffset = Vector2.Lerp(Vector2.zero, new Vector2(0f, -0.035f), t);
			if (!game.IsInGame.Value)
			{
				mesh.transform.localPosition = new Vector3(2000f, 2000f, 0f);
			}
		}));
		mesh.SetActive(false);
	}

	private void OnDisable()
	{
		if (_mesh != null)
		{
			_mesh.SetActive(false);
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		if (!character.stopColliding && collider.gameObject.name.Equals("Character"))
		{
			if (displayText)
			{
				UIScreenController.Instance.QueueMessage(text);
			}
			if (displayMesh)
			{
				StartCoroutine(ShowArrow());
			}
			if (allowHoverboard)
			{
				hoverboard.IsAllowed = true;
			}
			if (endTutorial)
			{
				PlayerInfo instance = PlayerInfo.Instance;
				track.IsRunningOnTutorialTrack = false;
				instance.IsTutorialCompleted = true;
				track.tutorial = false;
			}
		}
	}
}
