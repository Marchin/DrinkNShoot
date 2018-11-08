using UnityEngine;
using UnityEngine.Events;

public class PauseMenu : MonoBehaviour 
{
	[SerializeField] GameObject pauseMenuUI;
	[SerializeField] GameObject hudUI;
	[SerializeField] UnityEvent onPause;
	[SerializeField] UnityEvent onResume;
	
	static bool isPaused;
	float timeScaleBeforePause;

	void Start()
	{
		isPaused = false;
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
		Time.timeScale = timeScaleBeforePause;
		pauseMenuUI.SetActive(false);
		hudUI.SetActive(true);
		isPaused = false;
		onResume.Invoke();
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