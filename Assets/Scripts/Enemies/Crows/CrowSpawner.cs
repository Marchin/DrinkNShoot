using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectPool))]
public class CrowSpawner : MonoBehaviour {
	[SerializeField] float m_spawnInterval = 2f;
	Collider[] m_landingZones;
	ObjectPool m_pool;
	float m_counter;
	private void Awake() {
		m_pool = GetComponent<ObjectPool>();
		m_counter = m_spawnInterval;
	}

	void Update() {
		if (m_counter <= 0f) {
			GameObject go;
			if (m_pool.Request(out go)) {
				go.transform.position = Vector3.up * 50f;
				Crow crow = go.GetComponent<Crow>();
				crow.SetLandingZones(m_landingZones);
				crow.Init();
				m_counter = m_spawnInterval;
			}
		} else {
			m_counter -= Time.deltaTime;
		}
	}

	public void SetLandingZones(Collider[] landingZones) {
		m_landingZones = landingZones;
	}

	public void DisableCrows() {
		m_counter = m_spawnInterval;
		m_pool.DisableAll();
	}
}