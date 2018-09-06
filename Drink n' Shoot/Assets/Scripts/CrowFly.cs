using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowFly : MonoBehaviour {
	[Tooltip("The zone in which the crow moves arround.")]
	[SerializeField] MeshRenderer m_walkable;
	[SerializeField] LayerMask m_crowsLayer;
	[SerializeField] LayerMask m_sceneLayer;
	[SerializeField] float m_height;
	[SerializeField] float MIN_INTERVAL;
	[SerializeField] float m_speed;
	[SerializeField] float m_rotationSpeed;
	CrowMovement m_crowMovement;
	Vector3 m_targetPosition;
	Vector3 m_startingPosition;
	Quaternion m_targetRotation;
	Quaternion m_startingRotation;
	float m_interval;
	bool m_flying;
	float m_accu;
	float m_initHeight;
	float m_quadraticA;
	float m_quadraticB;
	float m_halfDistance;
	RaycastHit m_heightDelta;

	private void Awake() {
		m_flying = false;
		m_interval = MIN_INTERVAL;
		m_accu = 0f;
		m_targetPosition = transform.position;
		m_targetRotation = transform.rotation;
		m_crowMovement = GetComponent<CrowMovement>();
	}

	private void Update() {
		m_interval -= Time.deltaTime;
		if (m_interval <= 0f) {
			if ( /*!m_crowMovement.IsMoving()*/ true && !m_flying) {
				StartFly();
			}
			if (m_flying) {
				Fly();
			}
		}
	}

	void StartFly() {
		if (GetLandingPosition(ref m_targetPosition)) {
			m_initHeight = transform.position.y;
			m_startingPosition = transform.position;
			m_startingRotation = transform.rotation;
			CalculateQuadratic(ref m_quadraticA, ref m_quadraticB, 
				ref m_halfDistance, ref m_heightDelta);
			m_targetRotation = Quaternion.LookRotation(
				m_targetPosition - transform.position);
			m_flying = true;
		}
	}

	bool GetLandingPosition(ref Vector3 landingPosition) {
		Vector3 landing = m_walkable.transform.position;
		landing.x += Random.Range(-m_walkable.bounds.extents.x,
			m_walkable.bounds.extents.x);
		landing.z += Random.Range(-m_walkable.bounds.extents.z,
			m_walkable.bounds.extents.z);
		bool isFreeToLand = !Physics.Raycast(landing + Vector3.up * m_height,
			Vector3.down, m_height + m_walkable.bounds.max.y, m_crowsLayer);
		if (isFreeToLand) {
			landingPosition = landing;
		}
		return isFreeToLand;
	}
	
	void Fly() {
		// m_targetPosition.y = (Mathf.Sin(m_accu) * m_height) + m_initHeight;
		m_accu += Time.deltaTime * m_speed;
		m_accu = Mathf.Clamp01(m_accu);
		float x = m_accu * m_halfDistance * 2 - m_halfDistance;
		Vector3 newPos = Vector3.Lerp(m_startingPosition,
			m_targetPosition, m_accu);
		newPos.y = -m_quadraticB * (x - m_quadraticA) * 
			(x - m_quadraticA)	+ m_initHeight;
		transform.position = newPos;
		transform.rotation = Quaternion.Lerp(m_startingRotation,
			m_targetRotation, Time.deltaTime * m_rotationSpeed);
		RaycastHit hit;
		bool touchedRoof = Physics.Raycast(transform.position,
			-transform.up, out hit, 0.5f, m_sceneLayer);
		if (HasLanded() || touchedRoof) {
			m_flying = false;
			m_accu = 0f;
			m_interval = MIN_INTERVAL;
		}
	}

	bool HasLanded() {
		return ((transform.position == m_targetPosition) &&
			(transform.rotation == m_targetRotation));
	}

	public bool IsFlying() {
		return m_flying;
	}

	void CalculateQuadratic (ref float cuadraticA, ref float cuadraticB,
		ref float halfDistance, ref RaycastHit heightDelta) {

		Vector3 diff = m_targetPosition - transform.position;
		diff.y = 0;
		m_halfDistance = Vector3.Magnitude(diff) / 2f;
		Vector3 heightChecker = m_targetPosition;
		heightChecker.y = m_height + m_initHeight;
		if(!Physics.Raycast(heightChecker, Vector3.down,
			out heightDelta, 100f, m_sceneLayer)){
				Debug.Log("Help!");
			}
		cuadraticB = (m_height - heightDelta.distance) / (m_halfDistance * 2f);
		cuadraticA = m_halfDistance - 
			(m_height + heightDelta.distance)/(m_height - heightDelta.distance);
	}
}