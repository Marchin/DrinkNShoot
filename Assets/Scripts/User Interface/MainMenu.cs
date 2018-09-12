using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	void Start()
	{
		Cursor.visible = true;
	}

	public void Play()
	{
		Cursor.visible = false;
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	public void Exit()
	{
		Application.Quit();
	}
}
