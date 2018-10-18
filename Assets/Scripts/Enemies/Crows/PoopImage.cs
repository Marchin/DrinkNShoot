using UnityEngine;
using UnityEngine.UI;

public class PoopImage : MonoBehaviour {
    [SerializeField] float m_maxAlpha = 0.5f;
    [SerializeField] float m_duration;
    [SerializeField] float m_fadeDuration;
    FadePoop[] m_images;
    AudioSource m_soundFX;
    public float MaxAlpha {get{return m_maxAlpha;}}
    public float Duration {get{return m_duration;}}
    public float FadeDuration {get{return m_fadeDuration;}}

    private void Awake() {
        m_images = GetComponentsInChildren<FadePoop>();
        m_soundFX = GetComponent<AudioSource>();
    }

    private void Update() {
    }

    [ContextMenu("Poop")]
    public void Poop() {
        m_soundFX.Play();
        m_images[Random.Range(0, m_images.Length)].Poop();
    }
}