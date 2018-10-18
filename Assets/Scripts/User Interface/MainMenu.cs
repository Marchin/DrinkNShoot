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
    
	bool playRequested;

	void Start()
	{
		GameManager.Instance.ShowCursor();
        playRequested = false;
        StartCoroutine("LoadAsyncScene");
		appVersionText.text = "Application Version: " + Application.version;
	}

	void QuitApplication()
	{
		Application.Quit();
	}

	public void Play()
	{
		GameManager.Instance.HideCursor();
        playRequested = true;
	}

	public void Exit()
	{
		Invoke("QuitApplication", QUIT_DELAY);
	}

    IEnumerator LoadAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Town Level");
        asyncLoad.allowSceneActivation = false;
        while (!asyncLoad.isDone)
        {
            if (playRequested)
            {
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
