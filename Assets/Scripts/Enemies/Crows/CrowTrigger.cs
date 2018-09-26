using UnityEngine;

public class CrowTrigger : MonoBehaviour {
	[SerializeField] Collider[] m_landingZones;
	CrowSpawner m_crowSpawner;

	private void Awake() {
		m_crowSpawner = GetComponentInParent<CrowSpawner>();
	}

	private void OnTriggerEnter(Collider other) {
		m_crowSpawner.enabled = true;
		m_crowSpawner.SetLandingZones(m_landingZones);
	}

	private void OnTriggerExit(Collider other) {
		m_crowSpawner.enabled = false;
		LevelManager.Instance.IncreaseStageLevel();
	}
}