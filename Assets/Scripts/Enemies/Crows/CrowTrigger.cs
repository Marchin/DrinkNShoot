using UnityEngine;

public class CrowTrigger : MonoBehaviour {
	[SerializeField] GameObject m_stage;
	BoxCollider[] m_landingZones;
	CrowSpawner m_crowSpawner;

	private void Awake() {
		m_landingZones = m_stage.GetComponentsInChildren<BoxCollider>();
		m_crowSpawner = GetComponentInParent<CrowSpawner>();
	}

	private void OnTriggerEnter(Collider other) {
		LevelManager.Instance.EnterShootingStage();
		m_crowSpawner.SetLandingZones(m_landingZones);
	}
}