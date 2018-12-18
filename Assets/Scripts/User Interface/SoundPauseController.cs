using UnityEngine;

public class SoundPauseController : MonoBehaviour {
    [SerializeField] AudioSource[] m_audioToPause;
    [SerializeField] bool m_stopOnPauseMenu = true;
    [SerializeField] bool m_resumeAfterStageCompletion = true;

    private void Start() {
        if (m_stopOnPauseMenu)
        {
            PauseMenu pauseMenu  = FindObjectOfType<PauseMenu>();
            pauseMenu.OnPause.AddListener(Pause);
            pauseMenu.OnResume.AddListener(Resume);
        }      

        LevelManager.Instance.OnGameOver.AddListener(Pause);
        if (m_resumeAfterStageCompletion)
            LevelManager.Instance.OnStartNextStage.AddListener(Resume);
    }

    private void Pause() {
        foreach (AudioSource audio in m_audioToPause)
            if (audio.isPlaying)
                audio.Pause();
    }

    private void Resume() {
        foreach (AudioSource audio in m_audioToPause)
            audio.UnPause();
    }
}
