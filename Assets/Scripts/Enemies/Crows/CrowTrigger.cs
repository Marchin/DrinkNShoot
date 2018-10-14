using UnityEngine;

public class CrowTrigger : MonoBehaviour {
	[Header("Stage Properties")]
	[SerializeField] [Range(1, 50)]
	int requiredKills;
	[SerializeField] [Range(0, 10)]
	int difficultyLevel;
	[SerializeField] [Range(0, 600)]
	float completionTime;
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

	public int RequiredKills {
		get {
			return requiredKills;
		}
	}

	public int DifficultyLevel {
		get {
			return difficultyLevel;
		}
	}

	public float CompletionTime {
		get {
			return completionTime;
		}
	}

}