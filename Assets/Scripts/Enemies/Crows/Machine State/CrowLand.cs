using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Crow))]
public class CrowLand : MonoBehaviour, IState {
	[SerializeField] LayerMask m_landingZonesLayer;
	[SerializeField] float m_flightSpeed;
	[SerializeField] float m_turnSpeed;
	Crow m_crow;
	LayerMask m_obstaclesLayer;
	Vector3 m_targetPosition;
	Quaternion m_targetRotation;
	Vector3 m_direction;
	Vector3 m_lastMovement;
	BoxCollider m_LZCollider;
	const float m_NEGLIGIBLE = 0.01f;
	float m_footOffset;
	bool m_rotCalculated;

	private void Awake() {
		m_crow = GetComponent<Crow>();
		m_obstaclesLayer = m_crow.ObstaclesLayer;
		m_footOffset = GetComponent<BoxCollider>().size.y * 0.5f;
	}

	private void OnEnable() {
		m_targetPosition = m_crow.GetLandingZone(out m_direction);
		m_LZCollider = m_crow.GetLZCollider();
		m_targetPosition.y += m_footOffset;
		m_rotCalculated = false;
		m_lastMovement = transform.forward;
	}

	public void StateUpdate(out IState nextState) {
		RaycastHit buildHit;
		Physics.Raycast(transform.position, m_lastMovement,
			out buildHit, 8f, m_obstaclesLayer);
		RaycastHit landingHit;
		Physics.Raycast(transform.position, m_targetPosition - transform.position,
			out landingHit, 10f, m_landingZonesLayer);
		Debug.DrawRay(buildHit.point, buildHit.normal, Color.cyan, 0.5f);
		if (Vector3.Distance(transform.position, m_targetPosition) > m_NEGLIGIBLE) {
			if (Vector3.Distance(transform.position, m_targetPosition) < 1.5f &&
				!IsToCrash(landingHit, buildHit)) {

				if (!m_rotCalculated) {
					RaycastHit hit;
					if (Physics.Raycast(transform.position, Vector3.down,
						out hit, 2f, m_landingZonesLayer)) {

						m_targetRotation = Quaternion.LookRotation(m_direction, hit.normal);
						m_targetPosition = hit.point + hit.normal * m_footOffset;
					} else {
						m_targetRotation = Quaternion.LookRotation(m_direction);
					}
					if (Vector3.Angle(transform.forward, m_direction) > 90f) {
						m_direction = -m_direction;
					}
				}
				transform.position = Vector3.Lerp(
					transform.position, m_targetPosition, 2f * Time.deltaTime);
			} else if (IsToCrash(landingHit, buildHit)) {
				Vector3 diff = m_targetPosition - transform.position;
				Vector3 projection = diff - buildHit.normal *
					Vector3.Dot(diff, buildHit.normal);
				if (Vector3.Angle(projection, Vector3.down) < 45f) {
					projection = Vector3.Cross(projection, buildHit.normal);
				}
				diff = transform.InverseTransformDirection(diff);
				diff.x = 0f;
				diff.z = Mathf.Abs(diff.z);
				diff = transform.TransformDirection(diff);
				diff = diff.normalized;
				if (Vector3.SignedAngle(transform.forward, projection, Vector3.up) > 90f) {
					projection.x *= -1f;
					projection.z *= -1f;
				}
				Debug.DrawRay(buildHit.point, projection, Color.yellow, 1f);
				m_targetRotation = Quaternion.LookRotation(projection);
				if (buildHit.distance < 0.25f) {
					transform.rotation = m_targetRotation;
				}
				m_lastMovement = transform.forward * 0.1f *
					m_flightSpeed * Time.deltaTime;
				transform.position += m_lastMovement;
				m_rotCalculated = false;
			} else {
				Vector3 diff = m_targetPosition - transform.position;
				diff = transform.InverseTransformDirection(diff);
				diff = diff.normalized;
				diff.x = 0f;
				diff.z = Mathf.Abs(diff.z);
				diff = transform.TransformDirection(diff);
				float angle = Vector3.SignedAngle(
					diff, m_targetPosition - transform.position, Vector3.up);
				if (angle == 180f || angle == 90f) {
					m_targetRotation = Quaternion.LookRotation(
						transform.eulerAngles + 5f * Vector3.up);
					transform.rotation = m_targetRotation;
					m_rotCalculated = false;
				}
				m_lastMovement = diff.normalized * m_flightSpeed * Time.deltaTime;
				transform.position += m_lastMovement;
				if (!m_rotCalculated) {
					m_targetRotation = Quaternion.LookRotation(
						m_targetPosition - transform.position);
				}
			}
			if (landingHit.collider != null &&
				Vector3.Angle(landingHit.normal, Vector3.up) < 45f) {

				if (buildHit.collider == null || landingHit.distance < buildHit.distance) {
					// Debug.DrawRay(transform.position, Vector3.down, Color.magenta, 10f);
					if (!m_rotCalculated) {
						if (Vector3.Angle(transform.forward, m_direction) > 90f) {
							m_direction = -m_direction;
						}
						m_targetRotation = Quaternion.LookRotation(
							m_direction, landingHit.normal);
						Debug.DrawRay(landingHit.transform.position,
							landingHit.normal, Color.magenta, 10f);
						m_rotCalculated = true;
					}
				}
			} else {
				RaycastHit hit;
				if (Physics.Raycast(transform.position,
					Vector3.down, out hit, 2f , m_landingZonesLayer)) {
						
					if (Vector3.Angle(transform.forward, m_direction) > 90f) {
						m_direction = -m_direction;
					}

					m_targetPosition = hit.point + hit.normal * m_footOffset;
					m_targetRotation = Quaternion.LookRotation(m_direction, hit.normal);
					
					transform.rotation = m_targetRotation;
					transform.position = m_targetPosition;
				}
			}
		} else {
			transform.rotation = m_targetRotation;
			transform.position = m_targetPosition;
		}
		if (Vector3.Distance(transform.eulerAngles,
				m_targetRotation.eulerAngles) > m_NEGLIGIBLE) {

			float avoidingBuild = (buildHit.collider != null) ? 2.5f : 1f;
			transform.rotation = Quaternion.RotateTowards(
				transform.rotation, m_targetRotation, m_turnSpeed *
				avoidingBuild * Time.deltaTime);
		} else {
			transform.rotation = m_targetRotation;
		}
		if ((transform.position == m_targetPosition) &&
			(transform.rotation == m_targetRotation)) {

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