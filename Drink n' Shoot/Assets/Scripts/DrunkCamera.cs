using UnityEngine;

public class DrunkCamera : MonoBehaviour {
	[SerializeField] Transform m_camera;
	[SerializeField] float m_speed;
	[SerializeField] float m_shakeness;
	const float m_HALF_PI = Mathf.PI / 2f;
	const float MIN = 0.6f;
	float m_shakenessMult;
	float m_currRotation;
	bool m_rotatingRight;

	private void Awake() {
		m_currRotation = m_camera.eulerAngles.z;
		m_shakenessMult = Random.Range(MIN, 1f);
		if (Mathf.Sin(f: Time.time * m_speed + m_HALF_PI) > 0f) {
			m_rotatingRight = true;
		} else {
			m_rotatingRight = false;
		}
	}

	private void Update() {
		Vector3 newRotation = m_camera.eulerAngles;
		float oscillation = Mathf.Sin(f: Time.time * m_speed + m_HALF_PI);
		if (((oscillation < 0f) && m_rotatingRight) ||
			(oscillation > 0f) && !m_rotatingRight) {
			m_shakenessMult = Random.Range(MIN, 1f);
		}
		m_currRotation += oscillation * m_shakeness * m_shakenessMult;
		newRotation.z = m_currRotation;
		m_camera.eulerAngles = newRotation;
	}
}