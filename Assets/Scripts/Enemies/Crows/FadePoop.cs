using UnityEngine;
using UnityEngine.UI;

public class FadePoop : MonoBehaviour {
    float m_maxAlpha;
    float m_duration;
    float m_fadeDuration;
    float m_fallSpeed;
	Image m_image;
    float m_fadingRate;
    bool m_fading;

	private void Awake() {
		PoopImage poopImage = GetComponentInParent<PoopImage>();
		m_maxAlpha = poopImage.MaxAlpha;
		m_duration = poopImage.Duration;
		m_fadeDuration = poopImage.FadeDuration;
        m_fallSpeed = poopImage.FallSpeed;
		m_image = GetComponent<Image>();
        m_fadingRate = m_maxAlpha / m_fadeDuration;
        m_fading = false;
        SetAlpha(0f);
	}
	
	void Update () {
        if (m_fading) {
            SetAlpha(m_image.color.a - (m_fadingRate * Time.deltaTime));
            if (m_image.color.a <= 0f) {
                enabled = false;
            }
        }
        Vector2 pos = m_image.rectTransform.position;
        pos.y -= m_fallSpeed * (m_image.color.a) * (m_image.color.a) * Time.deltaTime;
        m_image.rectTransform.position = pos;
	}
    void SetAlpha(float alpha) {
        Color color = m_image.color;
        color.a = alpha;
        m_image.color = color;
    }

	
    public void Poop() {
        enabled = true;
        m_image.rectTransform.position = new Vector2(
            Random.Range(0.2f, 0.8f) * Screen.width, Random.Range(0.2f, 0.8f) * Screen.height);
        SetAlpha(m_maxAlpha);
        Invoke("StartToFade", m_duration);
    }

    void StartToFade() {
        m_fading = true;
    }

}
