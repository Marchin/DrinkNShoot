using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI appVersionText;
	const float QUIT_DELAY = 0.5f;

	void Start()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		appVersionText.text = "Application Version: " + Application.version;
	}

	void QuitApplication()
	{
		Application.Quit();
	}

	public void Play()
	{
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	public void Exit()
	{
		Invoke("QuitApplication", QUIT_DELAY);
	}
}
