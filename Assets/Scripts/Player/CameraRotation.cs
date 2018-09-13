using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
	[SerializeField] float rotationSpeed;
	[SerializeField] float horizontalRange;
	[SerializeField] float verticalRange;
	Transform fpsCamera;
	float horizontalAngle = 0;
	float verticalAngle = 0;
	
	void Awake()
	{
		Cursor.lockState = CursorLockMode.Locked;
		fpsCamera = GetComponentInChildren<Camera>().transform;
	}

	void Update()
	{
		float horizontalRotation = InputManager.Instance.GetHorizontalViewAxis() * rotationSpeed * Time.deltaTime;
		float verticalRotation = InputManager.Instance.GetVerticalViewAxis() * rotationSpeed * Time.deltaTime;

		horizontalAngle += horizontalRotation;
		verticalAngle -= verticalRotation;

		horizontalAngle = Mathf.Clamp(horizontalAngle, -horizontalRange, horizontalRange);
		verticalAngle = Mathf.Clamp(verticalAngle, -verticalRange, verticalRange);

		transform.localEulerAngles = new Vector3(0, horizontalAngle, 0);
		fpsCamera.localEulerAngles = new Vector3(verticalAngle, fpsCamera.localEulerAngles.y, fpsCamera.localEulerAngles.z);
	}
}