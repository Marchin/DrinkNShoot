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
	}

	void Update()
	{
		float horizontalRotation = InputManager.Instance.GetHorizontalViewAxis() * rotationSpeed * Time.unscaledDeltaTime;
		float verticalRotation = InputManager.Instance.GetVerticalViewAxis() * rotationSpeed * Time.unscaledDeltaTime;

		horizontalAngle += horizontalRotation;
		verticalAngle -= verticalRotation;

		verticalAngle = Mathf.Clamp(verticalAngle, -verticalRange, verticalRange);
		horizontalAngle = Mathf.Clamp(horizontalAngle, minHorizontalAngle, maxHorizontalAngle);

		transform.eulerAngles = new Vector3(transform.eulerAngles.x, horizontalAngle, transform.eulerAngles.z);
		fpsCamera.eulerAngles = new Vector3(verticalAngle, fpsCamera.eulerAngles.y, fpsCamera.eulerAngles.z);
	}

	void ChangeHorizontalClamping()
	{
		Vector3 targetDir = (LevelManager.Instance.CurrentStagePosition - LevelManager.Instance.CurrentSpawnPointPosition).normalized;
		Quaternion targetCentralRotation = Quaternion.LookRotation(targetDir);

		Debug.Log(targetCentralRotation.eulerAngles);

		minHorizontalAngle = targetCentralRotation.eulerAngles.y - horizontalRange / 2f;
		maxHorizontalAngle = targetCentralRotation.eulerAngles.y + horizontalRange / 2f;
		Debug.DrawRay(LevelManager.Instance.CurrentSpawnPointPosition, targetDir * 100f, Color.green, 60f);
		Debug.Log(minHorizontalAngle);
		Debug.Log(maxHorizontalAngle);
	}
}