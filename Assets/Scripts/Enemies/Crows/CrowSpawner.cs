using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectPool))]
public class CrowSpawner : MonoBehaviour {
	[SerializeField] float m_spawnInterval;
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
				m_counter = m_spawnInterval;
			}
		} else {
			m_counter -= Time.deltaTime;
		}

	}
}