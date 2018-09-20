using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour 
{
	[SerializeField]
	GameObject pauseMenuUI;
	[SerializeField]
	GameObject hudUI;
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
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		Time.timeScale = 0;
		pauseMenuUI.SetActive(true);
		hudUI.SetActive(false);
		isPaused = true;
	}

	public void Resume()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		Time.timeScale = 1;
		pauseMenuUI.SetActive(false);
		hudUI.SetActive(true);
		isPaused = false;
	}

	public void Quit()
	{
		Time.timeScale = 1;
		LevelManager.Instance.QuitLevel();
	}

	public static bool IsPaused
	{
		get { return isPaused; }
	}
}
