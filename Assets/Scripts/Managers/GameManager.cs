using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	[SerializeField] SettingsMenu.GfxSetting currentGfxSetting = SettingsMenu.GfxSetting.Wild;
	[SerializeField] float currentSfxVolume = 0.75f;
	[SerializeField] AnimationClip fadeOutAnimation;
	
	static GameManager instance;

	Animator animator;
	int nextSceneToLoad = 0;

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

	void LoadNextScene()
	{
		StartCoroutine("LoadNextSceneAsynchronously");
	}

	IEnumerator LoadNextSceneAsynchronously()
	{
		AsyncOperation operation = SceneManager.LoadSceneAsync(nextSceneToLoad);
		
		while (!operation.isDone)
			yield return null;
		
		nextSceneToLoad = 0;
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
		Invoke("LoadNextScene", fadeOutAnimation.length);
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
}
