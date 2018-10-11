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
	Vector3 m_direction;
	const float m_NEGLIGIBLE = 0.01f;
	float m_footOffset;
	bool m_rotCalculated;
	float m_targetY;

	private void Awake() {
		m_crow = GetComponent<Crow>();
		m_footOffset = GetComponent<BoxCollider>().size.y * 0.5f;
	}

	private void OnEnable() {
		m_targetPosition = m_crow.GetLandingZone(out m_direction);
		m_targetY = m_targetPosition.y;
		m_targetPosition.y += 1f;
		m_rotCalculated = false;
	}

	public void StateUpdate(out IState nextState) {
		if (Vector3.Distance(transform.position, m_targetPosition) > m_NEGLIGIBLE) {
			if (Vector3.Distance(transform.position, m_targetPosition) < 3f) {
				if(!m_rotCalculated) {
					m_targetPosition.y = m_targetY;
				}
				transform.position = Vector3.Lerp(
					transform.position, m_targetPosition, 2f * Time.deltaTime);
			} else if ((Vector3.Distance(transform.position, m_targetPosition) < m_landingRadius)) {
				Vector3 diff = m_targetPosition - transform.position;
				diff = transform.InverseTransformDirection(diff);
				diff.x = 0f;
				diff = diff.normalized;
				diff = transform.TransformDirection(diff);
				transform.position += diff * m_flightSpeed * Time.deltaTime;
			} else {
				transform.position += transform.forward * m_flightSpeed * Time.deltaTime;
			}
			RaycastHit hit;
			// Debug.DrawRay(m_targetPosition, Vector3.up, Color.magenta, 1f);
			if ((transform.position.y - m_targetPosition.y) > 0f) {
				if (Physics.Raycast(transform.position, m_targetPosition - transform.position,
						out hit, 5f, m_landingZonesLayer)) {

					// Debug.DrawRay(transform.position, Vector3.down, Color.magenta, 10f);
					if (!m_rotCalculated && transform.position.y > m_targetY) {
						m_targetRotation = Quaternion.LookRotation(
							m_direction, hit.normal);
						m_targetY = hit.transform.position.y;
						m_targetPosition.y = m_targetY;
						Debug.DrawRay(hit.transform.position, hit.normal, Color.magenta, 10f);
						m_rotCalculated = true;
						m_turnSpeed *= 3f;
					}
					if (hit.distance <= m_footOffset) {
						m_targetPosition = transform.position;
						transform.rotation = m_targetRotation;
					}
				} else {
					m_targetRotation = Quaternion.LookRotation(m_targetPosition - transform.position);
				}
			}
		} else {
			if (m_targetPosition.y == m_targetY) {
				transform.position = m_targetPosition;
			}
			transform.rotation = m_targetRotation;
		}
		if (Vector3.Distance(transform.eulerAngles, m_targetRotation.eulerAngles) > m_NEGLIGIBLE) {
			transform.rotation = Quaternion.RotateTowards(
				transform.rotation, m_targetRotation, m_turnSpeed * Time.deltaTime);
		} else {
			transform.rotation = m_targetRotation;
		}
		if ((transform.position == m_targetPosition) &&
			(transform.rotation == m_targetRotation)) {

			m_turnSpeed *= 0.5f;
			nextState = GetComponent<CrowMovement>();
		} else {
			nextState = this;
		}
	}

	public void StateFixedUpdate() { }

	public void SetRotationToDestination() {
		transform.rotation = Quaternion.LookRotation(m_targetPosition - transform.position, Vector3.up);
	}
}