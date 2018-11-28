using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class MainMenu : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI appVersionText;
	[SerializeField] AudioSource themeSong;

	[SerializeField] float musicStartDelay = 5f;
    
	float musicTimer = 0f;

	void Start()
	{
		GameManager.Instance.ShowCursor();
		appVersionText.text = "Application Version: " + Application.version;
	}

	void Update()
	{
		if (!themeSong.isPlaying)
		{
			musicTimer += Time.deltaTime;
			if (musicTimer >= musicStartDelay)
			{
				musicTimer = 0f;
				themeSong.Play();
			}
		}
	}

	public void Play(int level)
	{
		GameManager.Instance.HideCursor();
        GameManager.Instance.FadeToScene(GameManager.Instance.GetLevelSceneName(level));
	}

	public void EnterStore()
	{
		GameManager.Instance.FadeToScene(GameManager.Instance.StoreScene);
	}

	public void Quit()
	{
		GameManager.Instance.QuitApplication();
	}
}