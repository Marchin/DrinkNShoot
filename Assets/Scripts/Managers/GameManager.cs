using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	[SerializeField] GameObject loadingScreen;
	[SerializeField] SettingsMenu.GfxSetting currentGfxSetting = SettingsMenu.GfxSetting.Wild;
	[SerializeField] float currentSfxVolume = 0.75f;
	[SerializeField] AnimationClip fadeInAnimation;
	[SerializeField] AnimationClip fadeOutAnimation;
	
	const float MIN_LOAD_TIME = 1.5f;

	static GameManager instance;

	Animator animator;
	Slider loadingBarSlider;
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
		loadingBarSlider = loadingScreen.GetComponentInChildren<Slider>();
	}

	// IEnumerator LoadFirstLevelInBackground(MainMenu mainMenu)
    // {
    //     firstLevelLoadOperation = SceneManager.LoadSceneAsync(1);
    //     firstLevelLoadOperation.allowSceneActivation = false;

    //     while (!firstLevelLoadOperation.isDone)
    //     {
    //         if (mainMenu.RequestedPlay)
	// 			animator.SetTrigger("Fade Out");
            
    //         yield return null;
    //     }
    // }

	IEnumerator LoadSceneAsynchronously(int nextSceneToLoad)
	{
		if (nextSceneToLoad >= 0)
		{
			HideCursor();

			loadingScreen.SetActive(true);

			float loadTimer = 0f;
			float maxProgressValue = 0.9f + MIN_LOAD_TIME;
			AsyncOperation operation = SceneManager.LoadSceneAsync(nextSceneToLoad);
			operation.allowSceneActivation = false;

			while (!operation.isDone)
			{
				float progress = Mathf.Clamp01((operation.progress + loadTimer) / maxProgressValue);
				loadingBarSlider.value = progress;
				loadTimer += Time.deltaTime;

				if (progress == 1f && loadTimer >= MIN_LOAD_TIME)		
					operation.allowSceneActivation = true;
				
				yield return null;
			}

			nextSceneToLoad = -1;
			
			loadingScreen.SetActive(false);
			animator.SetTrigger("Fade In");
		}
	}

	public void FadeToScene(int sceneIndex)
	{
		nextSceneToLoad = sceneIndex;
		animator.SetTrigger("Fade Out");
	}

	public void OnFadeOutComplete()
	{
		StartCoroutine(LoadSceneAsynchronously(nextSceneToLoad));
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

	// public void StartLoadingFirstLevel(MainMenu mainMenu)
	// {
	// 	StartCoroutine(LoadFirstLevelInBackground(mainMenu));
	// }

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
