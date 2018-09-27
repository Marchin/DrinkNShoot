using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectPool))]
public class CrowSpawner : MonoBehaviour {
	[SerializeField] float m_spawnInterval = 2f;
	[SerializeField] float m_spawnDistance;
	[SerializeField] float m_spawnHeight = 50f;
	Transform m_player;
	Collider[] m_landingZones;
	ObjectPool m_pool;
	
	float m_counter;
	private void Awake() {
		m_player = FindObjectOfType<CameraRotation>().transform;
		m_pool = GetComponent<ObjectPool>();
		m_counter = m_spawnInterval;
	}

	void Update() {
		if (m_counter <= 0f) {
			GameObject go;
			if (m_pool.Request(out go)) {
				float angle = Random.Range(0f, 360f);
				Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * m_spawnDistance;
				offset.y = m_spawnHeight;
				go.transform.position = m_player.position + offset ;
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