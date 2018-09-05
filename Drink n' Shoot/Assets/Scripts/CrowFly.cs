using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowFly : MonoBehaviour {
	[Tooltip("The zone in which the crow moves arround.")]
	[SerializeField] MeshRenderer m_walkable;
	[SerializeField] LayerMask m_crowsLayer;
	[SerializeField] float m_height;
	[SerializeField] float m_distance;
	[SerializeField] readonly float MIN_INTERVAL;
	Vector3 m_targetPosition;
	Quaternion m_targetRotation;
	float m_interval;
	bool m_flying;

	private void Awake() {
		m_flying = false;
		m_interval = MIN_INTERVAL;
		m_targetPosition = transform.position;
		m_targetRotation = transform.rotation;
	}

	private void Update() {
		m_interval -= Time.deltaTime;
		if (m_interval <= 0f){
			if (true /*is not moving and not flying*/) {
				Fly();
			}
		}
	}

	void Fly() {
		if (GetLandingPosition(ref m_targetPosition)){
		}
	}

	bool GetLandingPosition(ref Vector3 landingPosition){
		Vector3 landing = m_walkable.transform.position;
		landing.x += Random.Range(
			-m_walkable.bounds.extents.x, m_walkable.bounds.extents.x);
		landing.z += Random.Range
		(-m_walkable.bounds.extents.z, m_walkable.bounds.extents.z);
		bool isFreeToLand = 
			!Physics.Raycast(landing + Vector3.up * m_height,
				Vector3.down, m_height + m_walkable.bounds.max.y, m_crowsLayer); 
		if (isFreeToLand) {
			landingPosition = landing;
		}
		return isFreeToLand;
	}
}
