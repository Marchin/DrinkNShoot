using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	[SerializeField] SettingsMenu.GfxSetting currentGfxSetting = SettingsMenu.GfxSetting.Wild;
	[SerializeField] float currentSfxVolume = 0.75f;
	[SerializeField] AnimationClip fadeInAnimation;
	[SerializeField] AnimationClip fadeOutAnimation;
	
	static GameManager instance;

	Animator animator;
	AsyncOperation firstLevelLoadOperation;
	int nextSceneToLoad = -1;
	bool tutorialEnabled = true;

	void Awake()
	{
		if (Instance == this)
		{
			animator = GetComponent<Animator>();
			DontDestroyOnLoad(gameObject);
		}
		else
			Destroy(gameObject);

	}

	void Start()
	{
		QualitySettings.SetQualityLevel((int)currentGfxSetting);
	}

	IEnumerator LoadFirstLevelInBackground(MainMenu mainMenu)
    {
        firstLevelLoadOperation = SceneManager.LoadSceneAsync(1);
        firstLevelLoadOperation.allowSceneActivation = false;

        while (!firstLevelLoadOperation.isDone)
        {
            if (mainMenu.RequestedPlay)
				animator.SetTrigger("Fade Out");
            
            yield return null;
        }
    }

	public void OnFadeOutComplete()
	{
		if (nextSceneToLoad >= 0)
			SceneManager.LoadScene(nextSceneToLoad);
		else
		{
			animator.ResetTrigger("Fade Out");
			firstLevelLoadOperation.allowSceneActivation = true;
		}

		nextSceneToLoad = -1;
		animator.SetTrigger("Fade In");
	}

	public void HideCursor()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	public void ShowCursor()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	public void FadeToScene(int sceneIndex)
	{
		nextSceneToLoad = sceneIndex;
		animator.SetTrigger("Fade Out");
	}

	public void StartLoadingFirstLevel(MainMenu mainMenu)
	{
		StartCoroutine(LoadFirstLevelInBackground(mainMenu));
	}

	public void QuitApplication()
	{
		Application.Quit();
	}

	public static GameManager Instance
	{
		get
		{
			if (!instance)
			{
				instance = FindObjectOfType<GameManager>();

				if (!instance)
				{
					GameObject gameObj = new GameObject("Game Manager");
					instance = gameObj.AddComponent<GameManager>();
				}
			}
			
			return instance;
		}
	}

	public SettingsMenu.GfxSetting CurrentGfxSetting
	{
		get { return currentGfxSetting; }
		set { currentGfxSetting = value; }
	}

	public float CurrentSfxVolume
	{
		get { return currentSfxVolume; }
		set { currentSfxVolume = value; }
	}

	public bool TutorialEnabled
	{
		get { return tutorialEnabled; }
		set { tutorialEnabled = value; }
	}
}
