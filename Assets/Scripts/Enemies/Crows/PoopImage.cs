using UnityEngine;
using UnityEngine.UI;

public class PoopImage : MonoBehaviour {
    [SerializeField] float m_maxAlpha = 0.5f;
    [SerializeField] float m_duration;
    [SerializeField] float m_fadeDuration;
    Image m_image;
    float m_fadingRate;
    bool m_fading;

    private void Awake() {
        m_fading = false;
        m_image = GetComponent<Image>();
        m_fadingRate = m_maxAlpha / m_fadeDuration;
        SetAlpha(0f);
    }

    private void Update() {
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

    [ContextMenu("Poop")]
    public void Poop() {
        SetAlpha(m_maxAlpha);
        Invoke("StartToFade", m_duration);
    }

    void StartToFade() {
        m_fading = true;
    }
}