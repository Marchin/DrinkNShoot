using System.Collections;
using System.Collections.Generic;
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

		transform.eulerAngles = new Vector3(transform.eulerAngles.x, horizontalAngle, transform.eulerAngles.z);
		fpsCamera.eulerAngles = new Vector3(verticalAngle, fpsCamera.eulerAngles.y, fpsCamera.eulerAngles.z);
	}

	void ChangeHorizontalClamping()
	{
		horizontalRotationClamped = true;
		Vector3 targetDir = (LevelManager.Instance.CurrentStagePosition - LevelManager.Instance.CurrentSpawnPointPosition).normalized;
		Quaternion targetCentralRotation = Quaternion.LookRotation(targetDir);

		minHorizontalAngle = targetCentralRotation.eulerAngles.y - horizontalRange / 2f;
		maxHorizontalAngle = targetCentralRotation.eulerAngles.y + horizontalRange / 2f;
	}

	void UnclampHorizontalRotation()
	{
		horizontalRotationClamped = false;
		minHorizontalAngle = 0f;
		maxHorizontalAngle = 0f;
	}
}