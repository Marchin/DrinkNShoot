using UnityEngine;

public class CrowTrigger : MonoBehaviour {
	[SerializeField] GameObject m_stage;
	Collider[] m_landingZones;
	CrowSpawner m_crowSpawner;

	private void Awake() {
		m_landingZones = m_stage.GetComponentsInChildren<Collider>();
		m_crowSpawner = GetComponentInParent<CrowSpawner>();
	}

	private void OnTriggerEnter(Collider other) {
		m_crowSpawner.enabled = true;
		m_crowSpawner.SetLandingZones(m_landingZones);
		LevelManager.Instance.EnterShootingStage();
	}

	private void OnTriggerExit(Collider other) {
		m_crowSpawner.enabled = false;
	}
}