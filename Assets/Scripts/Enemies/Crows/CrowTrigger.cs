using UnityEngine;

public class CrowTrigger : MonoBehaviour {
	[Header("Stage Properties")]
	[SerializeField] GameObject m_stage;
	[SerializeField] [Range(0, 10)]
	int m_difficultyLevel;
	[SerializeField] [Range(0f, 600f)]
	float m_completionTime;
	[SerializeField] [Range(1, 50)]
	int m_bronzeTierKills;
	[SerializeField] [Range(1, 50)]
	int m_silverTierKills;
	[SerializeField] [Range(1, 50)]
	int m_goldTierKills;
	
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

	public void EnableStage() {
		m_stage.SetActive(true);
	}

	public void DisableStage() {
		m_stage.SetActive(false);
	}

	public int BronzeTierKills {
		get {
			return m_bronzeTierKills;
		}
	}

	public int SilverTierKills {
		get {
			return m_silverTierKills;
		}
	}

	public int GoldTierKills {
		get {
			return m_goldTierKills;
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