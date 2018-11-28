using UnityEngine;
using UnityEngine.Events;

public class PauseMenu : MonoBehaviour 
{
	[SerializeField] GameObject pauseMenuUI;
	[SerializeField] GameObject hudUI;
	[SerializeField] GameObject mainScreenUI;
	[SerializeField] GameObject settingsMenuUI;
	[SerializeField] GameObject returnButtonUI;
	[SerializeField] Animator pauseMenuAnimator;
	[SerializeField] AnimationClip fadeOutAnimation;
	[SerializeField] AudioClip pauseSound;
	
	static bool isPaused;
	
	AudioSource audioSource;
	float timeScaleBeforePause;

    UnityEvent onPause = new UnityEvent();
    UnityEvent onResume = new UnityEvent();

	void Awake()
	{   
		isPaused = false;
		audioSource = GetComponentInParent<AudioSource>();
    }

	void Update()
	{
		if (InputManager.Instance.GetPauseButton() && !LevelManager.Instance.GameInStandBy)
		{
			if (!isPaused)
				Pause();
			else
				Resume();
		}
	}

	public void Pause()
	{
		GameManager.Instance.ShowCursor();
		audioSource.PlayOneShot(pauseSound);
		timeScaleBeforePause = Time.timeScale;
		Time.timeScale = 0f;
		pauseMenuUI.SetActive(true);
		hudUI.SetActive(false);
		isPaused = true;
		onPause.Invoke();
	}

	public void Resume()
	{
		GameManager.Instance.HideCursor();
		if (settingsMenuUI.activeInHierarchy && returnButtonUI.activeInHierarchy)
		{
			settingsMenuUI.SetActive(false);
			returnButtonUI.SetActive(false);
			mainScreenUI.SetActive(true);
		}
		Time.timeScale = timeScaleBeforePause;
		pauseMenuAnimator.SetTrigger("Fade Out");
		Invoke("OnFadeOutFinish", fadeOutAnimation.length);
		hudUI.SetActive(true);
		isPaused = false;
		onResume.Invoke();
	}

	public void Quit()
	{
		Time.timeScale = 1f;
		LevelManager.Instance.QuitLevel();
	}

	void OnFadeOutFinish()
	{
		pauseMenuUI.SetActive(false);
	}
	
	public static bool IsPaused
	{
		get { return isPaused;}
	}
	
	public UnityEvent OnPause
	{
		get { return onPause; }
	}
	public UnityEvent OnResume
	{
		get { return onResume; }
	}
}