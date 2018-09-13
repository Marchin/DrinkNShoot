using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevelMenu : MonoBehaviour 
{
	void Start()
	{
		Time.timeScale = 0;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	public void Restart()
	{
		Time.timeScale = 1;
		LevelManager.Instance.RestartLevel();
	}

	public void Quit()
	{
		Time.timeScale = 1;
		LevelManager.Instance.QuitLevel();
	}
}
