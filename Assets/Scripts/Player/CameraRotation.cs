using System.Collections;
using UnityEngine;

public class CameraRotation : MonoBehaviour 
{
	[SerializeField] float rotationSpeed;
	[SerializeField] float horizontalRange;
	[SerializeField] float verticalRange;
	
	Transform fpsCamera;
	float horizontalAngle = 0f;
	float verticalAngle = 0f;
	float minHorizontalAngle;
	float maxHorizontalAngle;
	bool horizontalRotationClamped = false;
	bool horizontalRotationBeingClamped = false;
	
	void Awake()
	{
		GameManager.Instance.HideCursor();
		fpsCamera = GetComponentInChildren<Camera>().transform;
		horizontalAngle = transform.localEulerAngles.y;
		minHorizontalAngle = horizontalAngle - horizontalRange / 2f;
		maxHorizontalAngle = horizontalAngle + horizontalRange / 2f;
	}

	void Start()
	{
		LevelManager.Instance.OnShootingStageEnter.AddListener(ChangeHorizontalClamping);
		LevelManager.Instance.OnStartNextStage.AddListener(UnclampHorizontalRotation);
	}

	void Update()
	{
		float horizontalRotation = InputManager.Instance.GetHorizontalViewAxis() * rotationSpeed * Time.unscaledDeltaTime;
		float verticalRotation = InputManager.Instance.GetVerticalViewAxis() * rotationSpeed * Time.unscaledDeltaTime;

		horizontalAngle += horizontalRotation;
		verticalAngle -= verticalRotation;

		verticalAngle = Mathf.Clamp(verticalAngle, -verticalRange, verticalRange);
		if (horizontalRotationClamped)
			horizontalAngle = Mathf.Clamp(horizontalAngle, minHorizontalAngle, maxHorizontalAngle);

		if (!horizontalRotationBeingClamped)
			transform.eulerAngles = new Vector3(transform.eulerAngles.x, horizontalAngle, transform.eulerAngles.z);
		fpsCamera.eulerAngles = new Vector3(verticalAngle, fpsCamera.eulerAngles.y, fpsCamera.eulerAngles.z);
	}

	void ChangeHorizontalClamping()
	{
		StartCoroutine(FocusOnNextStage());
	}

	void UnclampHorizontalRotation()
	{
		horizontalRotationClamped = false;
		minHorizontalAngle = 0f;
		maxHorizontalAngle = 0f;
	}

	IEnumerator FocusOnNextStage()
	{
        horizontalRotationBeingClamped = true;
        
		float timer = 0f;
		Vector3 targetDir = (LevelManager.Instance.CurrentStagePosition - LevelManager.Instance.CurrentSpawnPointPosition).normalized;
        Quaternion targetCentralRotation = Quaternion.LookRotation(targetDir);
		Quaternion fromRotation = transform.rotation;

		while (transform.rotation.eulerAngles.y != targetCentralRotation.eulerAngles.y)
		{
            transform.rotation = Quaternion.Slerp(fromRotation, targetCentralRotation, timer); 
			timer += Time.unscaledDeltaTime / 3f;
			yield return null;
		}

		horizontalAngle = targetCentralRotation.eulerAngles.y;
        minHorizontalAngle = targetCentralRotation.eulerAngles.y - horizontalRange / 2f;
        maxHorizontalAngle = targetCentralRotation.eulerAngles.y + horizontalRange / 2f;
        
        horizontalRotationClamped = true;
		horizontalRotationBeingClamped = false;
	}
}