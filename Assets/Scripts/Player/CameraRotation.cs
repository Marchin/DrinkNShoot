using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour {
	[SerializeField] float rotationSpeed;
	[SerializeField] float horizontalRange;
	[SerializeField] float verticalRange;
	Transform fpsCamera;
	float horizontalAngle = 0;
	float verticalAngle = 0;
	float minHorizontalAngle;
	float maxHorizontalAngle;
	
	void Awake()
	{
		Cursor.lockState = CursorLockMode.Locked;
		fpsCamera = GetComponentInChildren<Camera>().transform;
		horizontalAngle = transform.localEulerAngles.y;
		minHorizontalAngle = horizontalAngle - verticalRange;
		maxHorizontalAngle = horizontalAngle + verticalRange;
	}

	void Update() {
		float horizontalRotation = InputManager.Instance.GetHorizontalViewAxis() * rotationSpeed * Time.deltaTime;
		float verticalRotation = InputManager.Instance.GetVerticalViewAxis() * rotationSpeed * Time.deltaTime;

		horizontalAngle += horizontalRotation;
		verticalAngle -= verticalRotation;

		horizontalAngle = Mathf.Clamp(horizontalAngle, minHorizontalAngle, maxHorizontalAngle);
		verticalAngle = Mathf.Clamp(verticalAngle, -verticalRange, verticalRange);

		transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, horizontalAngle, transform.localEulerAngles.z);
		fpsCamera.localEulerAngles = new Vector3(verticalAngle, fpsCamera.localEulerAngles.y, fpsCamera.localEulerAngles.z);
	}
}