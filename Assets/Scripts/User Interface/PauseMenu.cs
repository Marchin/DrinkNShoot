using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PauseMenu : MonoBehaviour 
{
	[SerializeField] GameObject pauseMenuUI;
	[SerializeField] GameObject hudUI;
	[SerializeField] GameObject mainScreenUI;
	[SerializeField] GameObject settingsMenuUI;
	[SerializeField] GameObject returnButtonUI;
	[SerializeField] Button[] mainScreenButtons;
	[SerializeField] Animator pauseMenuAnimator;
	[SerializeField] AnimationClip slideInAnimation;
	[SerializeField] AnimationClip slideOutAnimation;
	[SerializeField] AudioClip pauseSound;
	
	static bool isPaused;
	
	AudioSource audioSource;
	float timeScaleBeforePause;
	bool canResume;

    UnityEvent onPause = new UnityEvent();
    UnityEvent onResume = new UnityEvent();

	void Awake()
	{   
		isPaused = false;
		canResume = true;
		audioSource = GetComponentInParent<AudioSource>();
    }

	void Update()
	{
		if (InputManager.Instance.GetPauseButton() && !LevelManager.Instance.GameInStandBy && !isPaused)
			Pause();
	}

	IEnumerator WaitToResume(float timeToWait)
	{
        canResume = false;
		foreach (Button button in mainScreenButtons)
			button.interactable = false;
        yield return new WaitForSecondsRealtime(timeToWait);
		canResume = true;
        foreach (Button button in mainScreenButtons)
            button.interactable = true;
	}

    IEnumerator OnPauseMenuFadeOutFinish()
    {
		yield return new WaitForSecondsRealtime(slideOutAnimation.length);
        Time.timeScale = timeScaleBeforePause;
        pauseMenuUI.SetActive(false);
        isPaused = false;
        hudUI.SetActive(true);
        onResume.Invoke();
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
		StartCoroutine(WaitToResume(slideInAnimation.length));
		onPause.Invoke();
	}

	public void Resume()
	{
		if (canResume)
		{
			GameManager.Instance.HideCursor();
			if (settingsMenuUI.activeInHierarchy && returnButtonUI.activeInHierarchy)
			{
				settingsMenuUI.SetActive(false);
				returnButtonUI.SetActive(false);
				mainScreenUI.SetActive(true);
			}
			pauseMenuAnimator.SetTrigger("Fade Out");
			StartCoroutine(OnPauseMenuFadeOutFinish());
        }
	}

	public void Quit()
	{
		Time.timeScale = 1f;
		LevelManager.Instance.QuitLevel();
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