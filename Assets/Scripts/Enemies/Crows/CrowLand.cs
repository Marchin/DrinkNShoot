using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Crow))]
public class CrowLand : MonoBehaviour, IState {
	[SerializeField] LayerMask m_landingZonesLayer;
	[SerializeField] float m_flightSpeed;
	[SerializeField] float m_turnSpeed;
	[SerializeField] float m_landingRadius;
	Crow m_crow;
	Vector3 m_targetPosition;
	Quaternion m_targetRotation;
	const float m_neglible = 0.1f;
	float m_footOffset;
	bool m_rotCalculated;
	float m_targetY;

	private void Awake() {
		m_crow = GetComponent<Crow>();
		m_targetRotation = transform.rotation;
		m_footOffset = GetComponent<Collider>().bounds.extents.y;
	}

	private void OnEnable() {
		m_targetPosition = m_crow.GetLandingZone();
		m_targetY = m_targetPosition.y;
		m_targetPosition.y = transform.position.y;
		m_rotCalculated = false;
	}

	public void StateUpdate(out IState nextState) {
		if (Vector3.Distance(transform.position, m_targetPosition) > m_neglible) {
			if (Vector3.Distance(transform.position, m_targetPosition) < 15f) {
				transform.position = Vector3.Lerp(
					transform.position, m_targetPosition, 2f * Time.deltaTime);
			} else if ((Vector3.Distance(transform.position, m_targetPosition) < m_landingRadius)) {
				m_targetPosition.y = m_targetY;
				transform.position += (transform.forward - transform.up * .1f) * m_flightSpeed * Time.deltaTime;
			} else {
				transform.position += transform.forward * m_flightSpeed * Time.deltaTime;
			}
			RaycastHit hit;
			if (Physics.Raycast(transform.position, m_targetPosition - transform.position,
					out hit, 20f, m_landingZonesLayer)) {

				Debug.DrawRay(m_targetPosition, hit.normal, Color.magenta, 10f);
				Debug.DrawRay(transform.position, Vector3.down, Color.magenta, 10f);
				if (!m_rotCalculated) {
					m_targetRotation = Quaternion.LookRotation(
						Vector3.right, hit.normal);
					m_rotCalculated = true;
					m_turnSpeed *= 2f;
				}
				if (hit.distance <= m_footOffset) {
					m_targetPosition = transform.position;
					transform.rotation = m_targetRotation;
				}
			} else {
				m_targetRotation = Quaternion.LookRotation(m_targetPosition - transform.position);
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
		if ((transform.position == m_targetPosition) &&
			(transform.rotation == m_targetRotation)) {

			m_turnSpeed *= 0.25f;
			nextState = GetComponent<CrowMovement>();
		} else {
			nextState = this;
		}
	}

	public void StateFixedUpdate() { }

}