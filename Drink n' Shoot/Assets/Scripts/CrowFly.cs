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
	Quaternion m_targetRotation;
	float m_interval;
	bool m_flying;
	float m_accu;
	float m_initHeight;

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
		}
		if (m_flying) {
			Fly();
		}
	}

	void StartFly() {
		if (GetLandingPosition(ref m_targetPosition)) {
			m_targetRotation = Quaternion.LookRotation(
				m_targetPosition - transform.position);
			m_initHeight = transform.position.y;
			m_flying = true;
		}
	}
	//Hacer una cuadratica entre altura inicial, altura final, y altura de vuelo
	void Fly() {
		m_targetPosition.y = (Mathf.Sin(m_accu) * m_height) + m_initHeight;
		m_accu += Time.deltaTime * m_speed;
		transform.position = Vector3.Lerp(transform.position,
			m_targetPosition, Time.deltaTime * m_speed);
		transform.rotation = Quaternion.Lerp(transform.rotation,
			m_targetRotation, Time.deltaTime * m_rotationSpeed);
		RaycastHit hit;
		bool touchedRoof = Physics.Raycast(transform.position,
			Vector3.down, out hit, 0.5f, m_sceneLayer);
		if (HasLanded() || touchedRoof) {
			m_flying = false;
			m_accu = 0f;
			m_interval = MIN_INTERVAL;
			transform.rotation = Quaternion.LookRotation(transform.forward, hit.normal);
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

	bool HasLanded() {
		return ((transform.position == m_targetPosition) &&
			(transform.rotation == m_targetRotation));
	}

	public bool IsFlying() {
		return m_flying;
	}
}