using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Loading Screen")]
    [SerializeField] GameObject loadingScreen;
    
    [Header("Settings Menu")]
    [SerializeField] SettingsMenu settingsMenu;

    [Header("Scene Names")]
    [SerializeField] string mainMenuScene;
    [SerializeField] string storeScene;
    [SerializeField] string[] levelScenes;

    [Header("Animations")]
    [SerializeField] AnimationClip fadeInAnimation;
    [SerializeField] AnimationClip fadeOutAnimation;

    const float MIN_LOAD_TIME = 1.5f;

    static GameManager instance;
    
    Animator animator;
    Slider loadingBarSlider;
    TextMeshProUGUI loadingText;
    SettingsMenu.GfxSetting currentGfxSetting;
    float currentSfxVolume;
    float currentMusicVolume;
    float currentMouseSensitivity;
    bool tutorialEnabled;
    int lastLevelUnlocked;
    int totalLevels = 2;
    bool shouldPlaySplashVideo = true;
    string nextSceneToLoad;

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
        currentGfxSetting = (SettingsMenu.GfxSetting)PlayerPrefs.GetInt("GfxSetting", (int)SettingsMenu.GfxSetting.Wild);
        currentSfxVolume = PlayerPrefs.GetFloat("SfxVolume", 0.75f);
        currentMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        currentMouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 0.75f);
        tutorialEnabled = (PlayerPrefs.GetInt("TutorialEnabled", 1) == 1);
        lastLevelUnlocked = PlayerPrefs.GetInt("LatestLevel", 1);
        
        if (settingsMenu)
        {
            settingsMenu.UpdateSfxVolume();
            settingsMenu.UpdateMusicVolume();
            settingsMenu.UpdateGraphicsSetting();
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        QualitySettings.SetQualityLevel((int)currentGfxSetting);
        loadingBarSlider = loadingScreen.GetComponentInChildren<Slider>();
        loadingText = loadingScreen.GetComponentInChildren<TextMeshProUGUI>();
    }

    IEnumerator LoadSceneAsynchronously(string nextSceneToLoad)
    {
        if (nextSceneToLoad != null)
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
                loadTimer += Time.deltaTime;
                loadingBarSlider.value = progress;
                loadingText.text = "Loading: " + (int)(progress * 100) + "%";

                if (progress == 1f)
                    operation.allowSceneActivation = true;

                yield return null;
            }
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        animator.SetTrigger("Fade In");
        loadingScreen.SetActive(false);
        nextSceneToLoad = null;
    }

    public void FadeToScene(string sceneName)
    {
        nextSceneToLoad = sceneName;
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

    public void QuitApplication()
    {
        Application.Quit();
    }

    public string GetLevelSceneName(int level)
    {
        return levelScenes[level - 1];
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
        set
        { 
            currentGfxSetting = value;
            PlayerPrefs.SetInt("GfxSetting", (int)currentGfxSetting);
        }
    }

    public float CurrentSfxVolume
    {
        get { return currentSfxVolume; }
        set
        {
            currentSfxVolume = value;
            PlayerPrefs.SetFloat("SfxVolume", currentSfxVolume);
        }
    }

    public float CurrentMusicVolume
    {
        get { return currentSfxVolume; }
        set 
        {
            currentSfxVolume = value;
            PlayerPrefs.SetFloat("MusicVolume", currentMusicVolume);
        }
    }

    public float CurrentMouseSensitivity
    {
        get { return currentMouseSensitivity; }
        set 
        {
            currentMouseSensitivity = value;
            PlayerPrefs.SetFloat("MouseSensitivity", currentMouseSensitivity);
        }
    }

    public bool TutorialEnabled
    {
        get { return tutorialEnabled; }
        set 
        {
            tutorialEnabled = value;
            if (tutorialEnabled)
                PlayerPrefs.SetInt("TutorialEnabled", 1);
            else
                PlayerPrefs.SetInt("TutorialEnabled", 0);
        }
    }

    public int LastLevelUnlocked
    {
        get { return lastLevelUnlocked; }
        set 
        {
            if (value <= totalLevels)
            {
                lastLevelUnlocked = value;
                PlayerPrefs.SetInt("LatestLevel", lastLevelUnlocked);
            }
        }
    }

    public bool ShouldPlaySplashVideo
    {
        get { return shouldPlaySplashVideo; }
        set { shouldPlaySplashVideo = value; }
    }

    public string MainMenuScene
    {
        get { return mainMenuScene; }
    }

    public string StoreScene
    {
        get { return storeScene; }
    }

}