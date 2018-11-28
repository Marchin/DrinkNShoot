using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Game Properties")]
    [SerializeField] GameObject loadingScreen;
    [SerializeField] SettingsMenu.GfxSetting currentGfxSetting = SettingsMenu.GfxSetting.Wild;
    [SerializeField] float currentSfxVolume = 0.75f;
    [SerializeField] float currentMusicVolume = 0.75f;

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
    string nextSceneToLoad;
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
        set { currentGfxSetting = value; }
    }

    public float CurrentSfxVolume
    {
        get { return currentSfxVolume; }
        set { currentSfxVolume = value; }
    }

    public float CurrentMusicVolume
    {
        get { return currentSfxVolume; }
        set { currentSfxVolume = value; }
    }

    public string MainMenuScene
    {
        get { return mainMenuScene; }
    }

    public string StoreScene
    {
        get { return storeScene; }
    }

    public bool TutorialEnabled
    {
        get { return tutorialEnabled; }
        set { tutorialEnabled = value; }
    }
}