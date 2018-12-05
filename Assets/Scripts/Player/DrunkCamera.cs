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
    float m_initHeight;

    private void Awake() {
        m_currRotation = transform.eulerAngles.z;
        m_prevRot = m_currRotation;
        m_shakenessMult = Random.Range(m_min, 1f);
        m_initHeight = transform.localPosition.y;
    }

    private void Start() {
        LevelManager.Instance.OnStartNextStage.AddListener(ChangeDrunkLevel);
        m_level = LevelManager.Instance.DifficultyLevel;
    }

    private void Update() {
        float deltaMin = 0.05f;
        m_min = 1f - deltaMin * m_level;
        if (m_min < deltaMin) {
            m_min = 0.1f;
        }
        // if (m_level > 2) {
        //     m_speed = 1.5f;
        // } else {
        //     m_speed = 1f;
        // }
        m_speed = 1f + m_level * 0.15f;
        float deltaShake = 5f;
        m_shakeness = deltaShake * m_level + 5f;

        Vector3 newRotation = transform.eulerAngles;
        m_oscillation = Mathf.Sin(f: Time.time * m_speed + m_HALF_PI);
        float a = Mathf.Abs(Mathf.Cos(f: Time.time * m_speed + m_HALF_PI));
        float intensity = m_oscillation * m_shakeness * m_shakenessMult;
        transform.localPosition = new Vector3 (m_oscillation * m_oscillation * intensity * 0.01f, a * 0.07f, 0f);
        transform.localPosition += m_initHeight * Vector3.up;
        m_currRotation = -(intensity) * 0.75f;
        if (Mathf.Sign(m_prevRot) != Mathf.Sign(m_currRotation)) {
            m_shakenessMult = Random.Range(m_min, 1f);
        }
        newRotation.z = m_currRotation * 0.25f;
        m_prevRot = m_currRotation;
        transform.eulerAngles = newRotation;
    }

    private void ChangeDrunkLevel() {
        m_level = LevelManager.Instance.DifficultyLevel;
    }

    public float GetTrembleSpeed01() {
        return Mathf.Abs(m_oscillation);
    }
}