using UnityEngine;
using UnityEngine.UI;

public class FadePoop : MonoBehaviour {
    float m_maxAlpha;
    float m_duration;
    float m_fadeDuration;
	Image m_image;
    float m_fadingRate;
    bool m_fading;

	private void Awake() {
		PoopImage poopImage = GetComponentInParent<PoopImage>();
		m_maxAlpha = poopImage.MaxAlpha;
		m_duration = poopImage.Duration;
		m_fadeDuration = poopImage.FadeDuration;
		m_image = GetComponent<Image>();
        m_fadingRate = m_maxAlpha / m_fadeDuration;
        m_fading = false;
        SetAlpha(0f);
	}
	
	void Update () {
        if (m_fading) {
            SetAlpha(m_image.color.a - (m_fadingRate * Time.deltaTime));
            if (m_image.color.a <= 0f) {
                m_fading = false;
            }
        }
	}
    void SetAlpha(float alpha) {
        Color color = m_image.color;
        color.a = alpha;
        m_image.color = color;
    }

	
    public void Poop() {
        SetAlpha(m_maxAlpha);
        Invoke("StartToFade", m_duration);
    }

    void StartToFade() {
        m_fading = true;
    }

}
