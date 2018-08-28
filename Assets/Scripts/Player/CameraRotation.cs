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
	
	void Start()
	{
		fpsCamera = GetComponentInChildren<Camera>().transform;
	}

	void Update()
	{
		float horizontalRotation = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
		float verticalRotation = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

		horizontalAngle += horizontalRotation;
		verticalAngle -= verticalRotation;

		horizontalAngle = Mathf.Clamp(horizontalAngle, -horizontalRange, horizontalRange);
		verticalAngle = Mathf.Clamp(verticalAngle, -verticalRange, verticalRange);

		transform.localEulerAngles = new Vector3(0, horizontalAngle, 0);
		fpsCamera.localEulerAngles = new Vector3(verticalAngle, 0, 0);
	}
}