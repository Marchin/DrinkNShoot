using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI appVersionText;
    
	bool playRequested;

	void Start()
	{
		GameManager.Instance.ShowCursor();
        playRequested = false;
        StartCoroutine("LoadAsyncScene");
		appVersionText.text = "Application Version: " + Application.version;
	}


	public void Play()
	{
		GameManager.Instance.HideCursor();
        playRequested = true;
	}

	public void Quit()
	{
		GameManager.Instance.QuitApplication();
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
