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
    
    const float START_BACKGROUND_LOAD_DELAY = 1.5f;

	bool requestedPlay = false;

	void Start()
	{
		GameManager.Instance.ShowCursor();
        GameManager.Instance.StartLoadingFirstLevel(this);
		appVersionText.text = "Application Version: " + Application.version;
	}

	public void Play()
	{
		GameManager.Instance.HideCursor();
        requestedPlay = true;
	}

	public void Quit()
	{
		GameManager.Instance.QuitApplication();
	}

    public bool RequestedPlay
    {
        get { return requestedPlay; }
    }
}
