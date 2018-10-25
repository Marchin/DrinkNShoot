using UnityEngine;

[RequireComponent(typeof(Camera))]
public class DrunkCamera : MonoBehaviour {
    const float m_HALF_PI = Mathf.PI / 2f;
    float m_speed;
    float m_shakeness;
    int m_level;
    float m_min = 0.3f;
    float m_shakenessMult;
    float m_currRotation;
    float m_prevRot;
    float m_oscillation;

    private void Awake() {
        m_currRotation = transform.eulerAngles.z;
        m_prevRot = m_currRotation;
        m_shakenessMult = Random.Range(m_min, 1f);
    }

    private void Start() {
        LevelManager.Instance.OnStartNextStage.AddListener(IncreaseDrunkLevel);
        m_level = LevelManager.Instance.DifficultyLevel;
    }

    private void Update() {
        float deltaMin = 0.05f;
        m_min = 1f - deltaMin * m_level;
        if (m_min < deltaMin) {
            m_min = 0.1f;
        }
        if (m_level > 2) {
            m_speed = 1.5f;
        } else {
            m_speed = 1f;
        }
        float deltaShake = 5f;
        m_shakeness = deltaShake * m_level + 5f;

        Vector3 newRotation = transform.eulerAngles;
        m_oscillation = Mathf.Sin(f: Time.time * m_speed + m_HALF_PI);
        m_currRotation = (m_oscillation * m_shakeness * m_shakenessMult);
        if (Mathf.Sign(m_prevRot) != Mathf.Sign(m_currRotation)) {
            m_shakenessMult = Random.Range(m_min, 1f);
        }
        newRotation.z = m_currRotation;
        m_prevRot = m_currRotation;
        transform.eulerAngles = newRotation;
    }

    private void IncreaseDrunkLevel() {
        m_level = LevelManager.Instance.DifficultyLevel;
    }

    public float GetTrembleSpeed01() {
        return Mathf.Abs(m_oscillation);
    }
}