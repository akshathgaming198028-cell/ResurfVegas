using System;
using UnityEngine;

[Serializable]
public class PlayerMoveController : MonoBehaviour
{
	public MovementMotor motor;

	public Transform character;

	public GameObject cursorPrefab;

	public GameObject joystickPrefab;

	public float cameraSmoothing;

	public float cameraPreview;

	public float cursorPlaneHeight;

	public float cursorFacingCamera;

	public float cursorSmallerWithDistance;

	public float cursorSmallerWhenClose;

	private Camera mainCamera;

	private Transform cursorObject;

	private Joystick joystickLeft;

	private Joystick joystickRight;

	private Transform mainCameraTransform;

	private Vector3 cameraVelocity;

	private Vector3 cameraOffset;

	private Vector3 initOffsetToPlayer;

	private Vector3 cursorScreenPosition;

	private Plane playerMovementPlane;

	private GameObject joystickRightGO;

	private Quaternion screenMovementSpace;

	private Vector3 screenMovementForward;

	private Vector3 screenMovementRight;

	public PlayerMoveController()
	{
		cameraSmoothing = 0.01f;
		cameraPreview = 2f;
		cursorSmallerWhenClose = 1f;
		cameraVelocity = Vector3.zero;
		cameraOffset = Vector3.zero;
	}

	public virtual void Awake()
	{
		motor.movementDirection = Vector2.zero;
		motor.facingDirection = Vector2.zero;
		mainCamera = Camera.main;
		mainCameraTransform = mainCamera.transform;
		if (!character)
		{
			character = transform;
		}
		initOffsetToPlayer = mainCameraTransform.position - character.position;
		if ((bool)joystickPrefab)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(joystickPrefab) as GameObject;
			gameObject.name = "Joystick Left";
			joystickLeft = gameObject.GetComponent<Joystick>();
			joystickRightGO = UnityEngine.Object.Instantiate(joystickPrefab) as GameObject;
			joystickRightGO.name = "Joystick Right";
			joystickRight = joystickRightGO.GetComponent<Joystick>();
		}
		cameraOffset = mainCameraTransform.position - character.position;
		cursorScreenPosition = new Vector3(0.5f * (float)Screen.width, 0.5f * (float)Screen.height, 0f);
		playerMovementPlane = new Plane(character.up, character.position + character.up * cursorPlaneHeight);
	}

	public virtual void Start()
	{
		GUITexture component = joystickRightGO.GetComponent<GUITexture>();
		float num = (float)Screen.width - component.pixelInset.x - component.pixelInset.width;
		Rect pixelInset = component.pixelInset;
		float num2 = (pixelInset.x = num);
		Rect rect = (component.pixelInset = pixelInset);
		screenMovementSpace = Quaternion.Euler(0f, mainCameraTransform.eulerAngles.y, 0f);
		screenMovementForward = screenMovementSpace * Vector3.forward;
		screenMovementRight = screenMovementSpace * Vector3.right;
	}

	public virtual void OnDisable()
	{
		if ((bool)joystickLeft)
		{
			joystickLeft.enabled = false;
		}
		if ((bool)joystickRight)
		{
			joystickRight.enabled = false;
		}
	}

	public virtual void OnEnable()
	{
		if ((bool)joystickLeft)
		{
			joystickLeft.enabled = true;
		}
		if ((bool)joystickRight)
		{
			joystickRight.enabled = true;
		}
	}

	public virtual void Update()
	{
		motor.movementDirection = joystickLeft.position.x * screenMovementRight + joystickLeft.position.y * screenMovementForward;
		if (!(motor.movementDirection.sqrMagnitude <= 1f))
		{
			motor.movementDirection.Normalize();
		}
		playerMovementPlane.normal = character.up;
		playerMovementPlane.distance = 0f - character.position.y + cursorPlaneHeight;
		Vector3 zero = Vector3.zero;
		motor.facingDirection = joystickRight.position.x * screenMovementRight + joystickRight.position.y * screenMovementForward;
		zero = motor.facingDirection;
		Vector3 target = character.position + initOffsetToPlayer + zero * cameraPreview;
		mainCameraTransform.position = Vector3.SmoothDamp(mainCameraTransform.position, target, ref cameraVelocity, cameraSmoothing);
		cameraOffset = mainCameraTransform.position - character.position;
	}

	public static Vector3 PlaneRayIntersection(Plane plane, Ray ray)
	{
		float enter = default(float);
		plane.Raycast(ray, out enter);
		return ray.GetPoint(enter);
	}

	public static Vector3 ScreenPointToWorldPointOnPlane(Vector3 screenPoint, Plane plane, Camera camera)
	{
		Ray ray = camera.ScreenPointToRay(screenPoint);
		return PlaneRayIntersection(plane, ray);
	}

	public virtual void HandleCursorAlignment(Vector3 cursorWorldPosition)
	{
		if ((bool)cursorObject)
		{
			cursorObject.position = cursorWorldPosition;
			bool num = Input.mousePosition.x < 0f;
			if (!num)
			{
				num = Input.mousePosition.x > (float)Screen.width;
			}
			if (!num)
			{
				num = Input.mousePosition.y < 0f;
			}
			if (!num)
			{
				num = Input.mousePosition.y > (float)Screen.height;
			}
			Cursor.visible = num;
			Quaternion a = cursorObject.rotation;
			if (motor.facingDirection != Vector3.zero)
			{
				a = Quaternion.LookRotation(motor.facingDirection);
			}
			Vector3 forward = Input.mousePosition - mainCamera.WorldToScreenPoint(transform.position + character.up * cursorPlaneHeight);
			forward.z = 0f;
			Quaternion b = mainCameraTransform.rotation * Quaternion.LookRotation(forward, -Vector3.forward);
			cursorObject.rotation = Quaternion.Slerp(a, b, cursorFacingCamera);
			float a2 = 0.1f * Vector3.Dot(cursorWorldPosition - mainCameraTransform.position, mainCameraTransform.forward);
			float num2 = Mathf.Lerp(0.7f, 1f, Mathf.InverseLerp(0.5f, 4f, motor.facingDirection.magnitude));
			cursorObject.localScale = Vector3.one * Mathf.Lerp(a2, 1f, cursorSmallerWithDistance) * num2;
			if (Input.GetKey(KeyCode.O))
			{
				cursorFacingCamera += Time.deltaTime * 0.5f;
			}
			if (Input.GetKey(KeyCode.P))
			{
				cursorFacingCamera -= Time.deltaTime * 0.5f;
			}
			cursorFacingCamera = Mathf.Clamp01(cursorFacingCamera);
			if (Input.GetKey(KeyCode.K))
			{
				cursorSmallerWithDistance += Time.deltaTime * 0.5f;
			}
			if (Input.GetKey(KeyCode.L))
			{
				cursorSmallerWithDistance -= Time.deltaTime * 0.5f;
			}
			cursorSmallerWithDistance = Mathf.Clamp01(cursorSmallerWithDistance);
		}
	}

	public virtual void Main()
	{
	}
}
