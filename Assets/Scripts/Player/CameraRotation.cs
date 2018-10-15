﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour {
	[SerializeField] float rotationSpeed;
	[SerializeField] float horizontalRange;
	[SerializeField] float verticalRange;
	Transform fpsCamera;
	float horizontalAngle = 0f;
	float verticalAngle = 0f;
	float minHorizontalAngle = 0f;
	float maxHorizontalAngle = 0f;
	
	void Awake()
	{
		Cursor.lockState = CursorLockMode.Locked;
		fpsCamera = GetComponentInChildren<Camera>().transform;
		horizontalAngle = transform.localEulerAngles.y;
	}

	void Start()
	{
		LevelManager.Instance.OnShootingStageEnter.AddListener(ChangeHorizontalClamping);
		ChangeHorizontalClamping();
	}

	void Update()
	{
		float horizontalRotation = InputManager.Instance.GetHorizontalViewAxis() * rotationSpeed * Time.deltaTime;
		float verticalRotation = InputManager.Instance.GetVerticalViewAxis() * rotationSpeed * Time.deltaTime;

		horizontalAngle += horizontalRotation;
		verticalAngle -= verticalRotation;

		verticalAngle = Mathf.Clamp(verticalAngle, -verticalRange, verticalRange);
		horizontalAngle = Mathf.Clamp(horizontalAngle, minHorizontalAngle, maxHorizontalAngle);

		transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, horizontalAngle, transform.localEulerAngles.z);
		fpsCamera.localEulerAngles = new Vector3(verticalAngle, fpsCamera.localEulerAngles.y, fpsCamera.localEulerAngles.z);
	}

	void ChangeHorizontalClamping()
	{
		Vector3 targetDir = (LevelManager.Instance.CurrentStagePosition - transform.position).normalized;
		Quaternion targetCentralRotation = Quaternion.LookRotation(targetDir);

		minHorizontalAngle = targetCentralRotation.eulerAngles.y - horizontalRange / 2f;
		maxHorizontalAngle = targetCentralRotation.eulerAngles.y + horizontalRange / 2f;
	}
}