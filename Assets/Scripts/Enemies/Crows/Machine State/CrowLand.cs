using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Crow))]
public class CrowLand : MonoBehaviour, IState {
	[SerializeField] LayerMask m_landingZonesLayer;
	[SerializeField] LayerMask m_buildingsLayer;
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
		m_targetPosition.y += m_footOffset;
		m_targetY = m_targetPosition.y;
		m_targetPosition.y += 2f;
		m_rotCalculated = false;
	}

	public void StateUpdate(out IState nextState) {
		RaycastHit buildHit;
		Physics.Raycast(transform.position, transform.forward,
			out buildHit, 5f, m_buildingsLayer);
		RaycastHit landingHit;
		Physics.Raycast(transform.position, m_targetPosition - transform.position,
			out landingHit, 10f, m_landingZonesLayer);
		if (Vector3.Distance(transform.position, m_targetPosition) > m_NEGLIGIBLE) {
			if (Vector3.Distance(transform.position, m_targetPosition) < 1.5f) {
				if (!m_rotCalculated) {
					m_targetPosition.y = m_targetY;
				}
				transform.position = Vector3.Lerp(
					transform.position, m_targetPosition, 2.25f * Time.deltaTime);
			} else if (IsToCrash(landingHit, buildHit)) {
				Vector3 diff = m_targetPosition - transform.position;
				Vector3 projection = diff - buildHit.normal * Vector3.Dot(diff, buildHit.normal);
				diff = transform.InverseTransformDirection(diff);
				diff.x = 0f;
				diff.z = Mathf.Abs(diff.z);
				diff = transform.TransformDirection(diff);
				diff = diff.normalized;
				if (Vector3.SignedAngle(transform.forward, projection, Vector3.up) > 90f) {
					projection.x *= -1f;
					projection.z *= -1f;
				}
				m_targetRotation = Quaternion.LookRotation(projection);
				transform.position += transform.forward * 0.5f * m_flightSpeed * Time.deltaTime;
				m_rotCalculated = false;
			} else {
				Vector3 diff = m_targetPosition - transform.position;
				diff = transform.InverseTransformDirection(diff);
				diff.x = 0f;
				diff.z = Mathf.Abs(diff.z);
				diff = transform.TransformDirection(diff);
				diff = diff.normalized;
				transform.position += diff * m_flightSpeed * Time.deltaTime;
				if (!m_rotCalculated) {
					m_targetRotation = Quaternion.LookRotation(m_targetPosition - transform.position);
				}
			}
			if (transform.position.y > m_targetY) {
				if (landingHit.collider != null) {
					if (buildHit.collider == null || landingHit.distance < buildHit.distance) {
						// Debug.DrawRay(transform.position, Vector3.down, Color.magenta, 10f);
						if (!m_rotCalculated) {
							if (Vector3.Angle(transform.forward, m_direction) > 90f) {
								m_direction = -m_direction;
							}
							m_targetRotation = Quaternion.LookRotation(
								m_direction, landingHit.normal);
							m_targetY = landingHit.transform.position.y + m_footOffset;
							m_targetPosition.y = m_targetY;
							Debug.DrawRay(landingHit.transform.position, landingHit.normal, Color.magenta, 10f);
							m_rotCalculated = true;
							if (landingHit.distance <= m_footOffset) {
								m_targetPosition = transform.position;
								transform.rotation = m_targetRotation;
							}
						}
					}
				}
			}
		} else {
			transform.position = m_targetPosition;
			transform.rotation = m_targetRotation;
		}
		if (Vector3.Distance(transform.eulerAngles, m_targetRotation.eulerAngles) > m_NEGLIGIBLE) {
			float avoidingBuild = (buildHit.collider != null) ? 2f : 1f;
			transform.rotation = Quaternion.RotateTowards(
				transform.rotation, m_targetRotation, m_turnSpeed * avoidingBuild * Time.deltaTime);
		} else {
			transform.rotation = m_targetRotation;
		}
		if ((transform.position == m_targetPosition) &&
			(transform.rotation == m_targetRotation)) {

			CancelInvoke();
			nextState = GetComponent<CrowMovement>();
		} else {
			nextState = this;
		}
	}

	public void StateFixedUpdate() { }

	public void SetRotationToDestination() {
		transform.rotation = Quaternion.LookRotation(
			m_targetPosition - transform.position, Vector3.up);
	}

	bool IsToCrash(RaycastHit landingHit, RaycastHit buildingHit) {
		return ((buildingHit.collider != null && landingHit.collider == null) ||
			(buildingHit.collider != null && landingHit.collider != null &&
				buildingHit.distance < landingHit.distance));
	}
}