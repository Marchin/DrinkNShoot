using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using TMPro;

public class MainMenu : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI appVersionText;
    [SerializeField] UnityEvent onRequestPlay;
    

	void Start()
	{
		GameManager.Instance.ShowCursor();
		appVersionText.text = "Application Version: " + Application.version;
	}

	public void Play()
	{
		GameManager.Instance.HideCursor();
        GameManager.Instance.FadeToScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	public void Quit()
	{
		GameManager.Instance.QuitApplication();
	}
}
