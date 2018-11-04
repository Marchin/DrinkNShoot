﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectPool))]
public class CrowSpawner : MonoBehaviour {
	[SerializeField] float m_spawnInterval = 2f;
	[SerializeField] float m_poopInterval;
	BoxCollider[] m_landingZones;
	sbyte[] m_LZOccupation;
	ObjectPool m_pool;
	float m_counter;

	private void Awake() {
		m_pool = GetComponent<ObjectPool>();
		m_counter = m_spawnInterval;
	}

	private void OnEnable() {
		Invoke("OrderPoop", m_poopInterval);
	}

	void Update() {
		if (m_counter <= 0f) {
			GameObject go;
			if (m_pool.Request(out go)) {
				go.transform.position = new Vector3(180f, 80f, 150f);
				Crow crow = go.GetComponent<Crow>();
				go.GetComponent<CrowLand>().SetRotationToDestination();
				crow.Init();
				m_counter = m_spawnInterval;
			}
		} else {
			m_counter -= Time.deltaTime;
		}
	}

	public void SetLandingZones(BoxCollider[] landingZones) {
		m_landingZones = landingZones;
		m_LZOccupation = new sbyte[m_landingZones.Length];
		for (sbyte i = 0; i < m_LZOccupation.Length; i++) {
			m_LZOccupation[i] = 0;
		}
	}

	public void DisableCrows() {
		m_counter = m_spawnInterval;
		CancelInvoke();
		m_pool.DisableAll();
	}

	void OrderPoop() {
		GameObject go;
		if (m_pool.RequestActive(out go)) {
			go.GetComponent<Crow>().Poop();
			CancelInvoke();
			Invoke("OrderPoop", m_poopInterval);
		} else {
			CancelInvoke();
			Invoke("OrderPoop", 0.1f);
		}
	}

	public BoxCollider PickOneOfLessOccupiedLZ(out int indexLZ) {
		int leastBy2 = Random.Range(0, m_LZOccupation.Length);
		for (int i = 1; i < m_LZOccupation.Length; i++) {
			int index;
			if (i >= leastBy2) {
				index = i + 1;
			} else {
				index = i;
			}
			index %= m_LZOccupation.Length;

			if (m_LZOccupation[index] - m_LZOccupation[leastBy2] <= -2) {
				leastBy2 = index;
			}
		}
		m_LZOccupation[leastBy2]++;
		indexLZ = leastBy2;
		return m_landingZones[leastBy2];
	}

	public void FreeLZ(ref int indexLZ) {
		if (indexLZ >= 0) {
			m_LZOccupation[indexLZ]--;
		}
		indexLZ = -1;
	}
}