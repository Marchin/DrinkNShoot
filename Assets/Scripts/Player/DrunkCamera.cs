using UnityEngine;

[RequireComponent(typeof(Camera))]
public class DrunkCamera : MonoBehaviour {
    [SerializeField] float m_speed;
    [SerializeField] float m_shakeness;
    [Range(0f, 100f)]
    [Tooltip("How much the total shakness varies (percentage)")]
    [SerializeField] float m_variance = 0.3f;
    [Tooltip("Temporal Dificult Level")]
    [Range(0, 25)]
    int m_level;
	const float m_HALF_PI = Mathf.PI / 2f;
    float m_min = 0.3f;
    float m_shakenessMult;
	float m_currRotation;
    float m_prevRot; 

	private void Awake() {
        m_level = LevelManager.Instance.DifficultyLevel;
		m_currRotation = transform.eulerAngles.z;
        m_prevRot = m_currRotation;
		m_shakenessMult = Random.Range(m_min, 1f);
	}

	private void Update() {
        if (!PauseMenu.IsPaused && !LevelManager.Instance.GameOver) {
            float deltaMin = 0.05f;
            m_min = 1f - deltaMin * m_level;
            if (m_min < deltaMin) {
                m_min = 0.1f;
            }
            if (m_level > 12) {
                m_speed = 1.5f;
            } else {
                m_speed = 1f;
            }
            float deltaShake = 0.025f;
            m_shakeness = deltaShake * m_level + 0.075f;
            m_min = 1f - (m_variance / 100f);

            Vector3 newRotation = transform.eulerAngles;
            float oscillation = Mathf.Sin(f: Time.time * m_speed + m_HALF_PI);
            m_currRotation += oscillation * m_shakeness * m_shakenessMult;
            if (Mathf.Sign(m_prevRot) != Mathf.Sign(m_currRotation)) {
                m_shakenessMult = Random.Range(m_min, 1f);
            }
            newRotation.z = m_currRotation;
            m_prevRot = m_currRotation;
            transform.eulerAngles = newRotation;
        }
	}
}