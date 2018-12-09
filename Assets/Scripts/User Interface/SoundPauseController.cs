using UnityEngine;

public class SoundPauseController : MonoBehaviour {
    [SerializeField] AudioSource m_audioToPause;

    private void Start() {
        PauseMenu pauseMenu  = FindObjectOfType<PauseMenu>();
        pauseMenu.OnPause.AddListener(Pause);
        pauseMenu.OnResume.AddListener(Resume);
    }

    void Pause() {
        m_audioToPause.Pause();
    }

    void Resume() {
        m_audioToPause.Play();
    }
}
