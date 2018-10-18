using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectPool))]
public class CrowSpawner : MonoBehaviour {
	[SerializeField] float m_spawnInterval = 2f;
	[SerializeField] float m_poopInterval;
	BoxCollider[] m_landingZones;
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
				crow.SetLandingZones(m_landingZones);
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
			Invoke("OrderPoop", m_poopInterval);
		} else {
			Invoke("OrderPoop", 0.1f);
		}
	}
}