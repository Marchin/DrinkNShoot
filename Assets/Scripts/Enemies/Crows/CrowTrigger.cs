using UnityEngine;

public class CrowTrigger : MonoBehaviour {
	[Header("Stage Properties")]
	[SerializeField] [Range(1, 50)]
	int m_requiredKills;
	[SerializeField] [Range(0, 10)]
	int m_difficultyLevel;
	[SerializeField] [Range(0, 600)]
	float m_completionTime;
	[SerializeField] GameObject m_stage;
	BoxCollider[] m_landingZones;
	CrowSpawner m_crowSpawner;

	private void Awake() {
		m_landingZones = m_stage.GetComponentsInChildren<BoxCollider>();
		m_crowSpawner = GetComponentInParent<CrowSpawner>();
	}

	private void OnTriggerEnter(Collider other) {
		m_stage.SetActive(true);
		LevelManager.Instance.EnterShootingStage();
		m_crowSpawner.SetLandingZones(m_landingZones);
	}

	private void OnTriggerExit() {
		m_stage.SetActive(false);
	}

	public int RequiredKills {
		get {
			return m_requiredKills;
		}
	}

	public int DifficultyLevel {
		get {
			return m_difficultyLevel;
		}
	}

	public float CompletionTime {
		get {
			return m_completionTime;
		}
	}

	public Vector3 StagePosition {
		get { 
			return m_stage.transform.position;
		}
	}
}