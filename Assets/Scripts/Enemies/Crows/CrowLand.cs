﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowLand : MonoBehaviour {
	[SerializeField] Collider[] m_landingZones;
	[SerializeField] LayerMask m_landingZonesLayer;
	[SerializeField] float m_flightSpeed;
	[SerializeField] float m_turnSpeed;
	Vector3 m_targetPosition;
	Quaternion m_targetRotation;
	const float m_neglible = 0.1f;
	float m_footOffset;
	bool m_rotCalculated;

	private void Awake() {
		m_targetPosition = GetLandingZone();
		m_targetRotation = transform.rotation;
		m_footOffset = GetComponent<Collider>().bounds.extents.y;
		m_rotCalculated = false;
	}

	void Update() {
		if (Vector3.Distance(transform.position, m_targetPosition) > m_neglible) {
			transform.position = Vector3.Lerp(
				transform.position, m_targetPosition, m_flightSpeed * Time.deltaTime);
			RaycastHit hit;
			if (Physics.Raycast(transform.position, Vector3.down, out hit, 10f, m_landingZonesLayer)) {
				Debug.DrawRay(m_targetPosition, hit.normal, Color.magenta, 10f);
				Debug.DrawRay(transform.position, transform.forward, Color.magenta, 10f);
				if (hit.distance <= m_footOffset) {
					m_targetPosition = transform.position;
					transform.rotation = m_targetRotation;
				} else if (!m_rotCalculated) {
					m_targetRotation = Quaternion.LookRotation(Vector3.Cross(-hit.normal, transform.forward), hit.normal);
					m_rotCalculated = true;
				}
			}
		} else {
			transform.position = m_targetPosition;
		}
		if (Vector3.Distance(transform.eulerAngles, m_targetRotation.eulerAngles) > m_neglible) {
			transform.rotation = Quaternion.RotateTowards(
				transform.rotation, m_targetRotation, m_turnSpeed * Time.deltaTime);
		} else {
			transform.rotation = m_targetRotation;
		}
		if ((transform.position == m_targetPosition) && (transform.rotation == m_targetRotation)) {
			gameObject.GetComponent<CrowMovement>().enabled = true;
			enabled = false;
		}
	}

	Vector3 GetLandingZone() {
		Collider landingZone = m_landingZones[Random.Range(0, m_landingZones.Length)];
		Vector3 offSet = new Vector3(
			Random.Range(-landingZone.bounds.extents.x, landingZone.bounds.extents.x),
			0f,
			Random.Range(-landingZone.bounds.extents.z, landingZone.bounds.extents.z)
		);
		return (landingZone.transform.position + offSet);
	}
}