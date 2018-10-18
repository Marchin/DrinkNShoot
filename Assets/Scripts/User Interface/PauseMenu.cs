using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PauseMenu : MonoBehaviour 
{
	[SerializeField] GameObject pauseMenuUI;
	[SerializeField] GameObject hudUI;
	[SerializeField] UnityEvent onPauseToggle;
	
	static bool isPaused;

	void Start()
	{
		isPaused = false;
	}

	void Update()
	{
		if (InputManager.Instance.GetPauseButton() && !LevelManager.Instance.GameOver)
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
		Time.timeScale = 0;
		pauseMenuUI.SetActive(true);
		hudUI.SetActive(false);
		isPaused = true;
		onPauseToggle.Invoke();
	}

	public void Resume()
	{
		GameManager.Instance.HideCursor();
		Time.timeScale = 1;
		pauseMenuUI.SetActive(false);
		hudUI.SetActive(true);
		isPaused = false;
		onPauseToggle.Invoke();
	}

	public void Quit()
	{
		Time.timeScale = 1;
		LevelManager.Instance.QuitLevel();
	}
	
	public static bool IsPaused
	{
		get { return isPaused;}
	}
	
	public UnityEvent OnPauseToggle
	{
		get { return onPauseToggle; }
	}
}
