using UnityEngine;

public class DrunkCamera : MonoBehaviour {
	[SerializeField] Transform m_camera;
	[SerializeField] float m_speed;
	[SerializeField] float m_shakeness;
	[SerializeField] float m_min = 0.3f;
	const float m_HALF_PI = Mathf.PI / 2f;
	float m_shakenessMult;
	float m_currRotation;
    float m_prevRot; 

	private void Awake() {
		m_currRotation = m_camera.eulerAngles.z;
        m_prevRot = m_currRotation;
		m_shakenessMult = Random.Range(m_min, 1f);
	}

	private void Update() {
		Vector3 newRotation = m_camera.eulerAngles;
		float oscillation = Mathf.Sin(f: Time.time * m_speed + m_HALF_PI);
		m_currRotation += oscillation * m_shakeness * m_shakenessMult;
        if (Mathf.Sign(m_prevRot) != Mathf.Sign(m_currRotation)) {
            m_shakenessMult = Random.Range(m_min, 1f);
        }
        newRotation.z = m_currRotation;
        m_prevRot = m_currRotation;
		m_camera.eulerAngles = newRotation;
	}
}