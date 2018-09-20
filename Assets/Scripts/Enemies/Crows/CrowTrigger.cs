using UnityEngine;

public class CrowTrigger : MonoBehaviour {
	[SerializeField] Collider[] m_landingZones;

	private void Awake() {
	}

	private void OnTriggerEnter(Collider other) {
		CrowSpawner.Instance.gameObject.SetActive(true);
		CrowSpawner.Instance.SetLandingZones(m_landingZones);
	}

	private void OnTriggerExit(Collider other) {
		CrowSpawner.Instance.gameObject.SetActive(false);		
	}
}
