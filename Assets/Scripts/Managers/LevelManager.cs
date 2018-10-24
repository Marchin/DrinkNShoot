using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
	public enum StageCompletionTier
	{
		Bronze, Silver, Gold
	}

    [Header("Entities References")]
    [SerializeField] NavMeshAgent playersWagon;
    [SerializeField] CrowSpawner crowSpawner;

    [Header("UI References")]
    [SerializeField] GameObject completeLevelUI;
    [SerializeField] GameObject completeStageUI;
    [SerializeField] GameObject failLevelUI;
    [SerializeField] GameObject hudUI;
    [SerializeField] GameObject tutorialUI;
    [SerializeField] GameObject introTextUI;

    [Header("Level Properties")]
    [SerializeField] float stageDrunkSpeedIncrease = 0.75f;
    [SerializeField] float stageDrunkRadiusIncrease = 10f;
    [SerializeField] int cashEarnedPerKill = 5;

    [Header("Sounds")]
    [SerializeField] AudioSource completeLevelSound;
    [SerializeField] AudioSource failLevelSound;

    [Header("Events")]
    [SerializeField] UnityEvent onEnemyKill;
    [SerializeField] UnityEvent onGameOver;
    [SerializeField] UnityEvent onQuitLevel;
    [SerializeField] UnityEvent onStartNextStage;
    [SerializeField] UnityEvent onFirstEmptyGun;
    [SerializeField] UnityEvent onClearFirstStage;
    [SerializeField] UnityEvent onShootingStageEnter;

    static LevelManager instance;

    List<Transform> enemySpawnPoints;
    CrowTrigger currentSpawnPoint;
    EndLevelMenu endLevelMenu;
    HUD hud;
	StageCompletionTier stageCompletionTier;
    int currentStageIndex;
    int maxStageIndex;
    bool gameOver = false;
    float timeLeft = 0;
    int targetsKilledInStage = 0;
    int totalKills = 0;
    int cashEarnedInStage = 0;
    int totalIncome = 0;
    bool hasNotifiedEmptyGun = false;
    bool inShootingStage = false;

    void Awake()
    {
        enemySpawnPoints = new List<Transform>();
        currentStageIndex = 0;
        foreach (Transform spawnPoint in crowSpawner.transform)
            if (spawnPoint.gameObject.activeInHierarchy && spawnPoint.GetComponent<CrowTrigger>())
                enemySpawnPoints.Add(spawnPoint);
        maxStageIndex = enemySpawnPoints.Count - 1;
        currentSpawnPoint = enemySpawnPoints[currentStageIndex].GetComponent<CrowTrigger>();
    }

    void Start()
    {
        endLevelMenu = FindObjectOfType<EndLevelMenu>();
        hud = FindObjectOfType<HUD>();

        crowSpawner.enabled = false;
        playersWagon.destination = currentSpawnPoint.transform.position;
        timeLeft = currentSpawnPoint.CompletionTime;

        if (GameManager.Instance.TutorialEnabled)
        {
            playersWagon.gameObject.GetComponentInChildren<WeaponHolder>().EquippedGun.OnEmptyGun.AddListener(FirstEmptyGunNotice);
            tutorialUI.SetActive(true);
            introTextUI.SetActive(true);
            GameManager.Instance.TutorialEnabled = false;
        }
    }

    void Update()
    {
        if (!gameOver && !PauseMenu.IsPaused && inShootingStage)
        {
            timeLeft -= Time.deltaTime;

            if (timeLeft <= 0)
            {
                if (targetsKilledInStage >= currentSpawnPoint.RequiredKills)
                    CompleteStage();
                else
                    FailLevel();
            }
        }
    }

    void CompleteStage()
    {
        gameOver = true;
        if (currentStageIndex != maxStageIndex)
            completeStageUI.SetActive(true);
        else
            completeLevelUI.SetActive(true);

        hudUI.SetActive(false);
        Time.timeScale = 0;
        GameManager.Instance.ShowCursor();

		if (targetsKilledInStage >= currentSpawnPoint.GoldTierKills)
			stageCompletionTier = StageCompletionTier.Gold;
		else
		{
			if (targetsKilledInStage >= currentSpawnPoint.SilverTierKills)
				stageCompletionTier = StageCompletionTier.Silver;
			else
				stageCompletionTier = StageCompletionTier.Bronze;
		}

        totalKills += targetsKilledInStage;
        cashEarnedInStage = targetsKilledInStage * cashEarnedPerKill;
        totalIncome += cashEarnedInStage;
        PlayerManager.Instance.Currency += cashEarnedInStage;
        PlayerManager.Instance.TotalKills += targetsKilledInStage;

        endLevelMenu.ChangeEndScreenText(cashEarnedInStage, totalIncome, targetsKilledInStage, totalKills, stageCompletionTier);
        hud.ChangeCurrencyDisplay(totalIncome);
        completeLevelSound.Play();
        onGameOver.Invoke();
    }

    void FailLevel()
    {
        gameOver = true;
        failLevelUI.SetActive(true);
        hudUI.SetActive(false);
        Time.timeScale = 0;
        GameManager.Instance.ShowCursor();
        failLevelSound.Play();
        onGameOver.Invoke();
    }

    void IncreaseKillCounter()
    {
        targetsKilledInStage++;
        onEnemyKill.Invoke();
    }

    public void RestartLevel()
    {
        onQuitLevel.Invoke();
        GameManager.Instance.FadeToScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitLevel()
    {
        onQuitLevel.Invoke();
        GameManager.Instance.TutorialEnabled = true;
        GameManager.Instance.FadeToScene(0);
    }

    void FirstEmptyGunNotice()
    {
        if (!hasNotifiedEmptyGun)
        {
            hasNotifiedEmptyGun = true;
            onFirstEmptyGun.Invoke();
        }
    }

    public void AddEnemyLife(Life enemyLife)
    {
        enemyLife.OnDeath.AddListener(IncreaseKillCounter);
    }

    public void MoveToNextStage()
    {
        if (currentStageIndex == 0)
            onClearFirstStage.Invoke();
        crowSpawner.DisableCrows();
        crowSpawner.enabled = false;

        gameOver = false;
        inShootingStage = false;
        currentStageIndex++;

        playersWagon.destination = enemySpawnPoints[currentStageIndex].position;
        currentSpawnPoint = enemySpawnPoints[currentStageIndex].GetComponent<CrowTrigger>();

        Gun currentGun = playersWagon.gameObject.GetComponentInChildren<WeaponHolder>().EquippedGun;
        currentGun.DrunkCrosshairSpeed += stageDrunkSpeedIncrease;
        currentGun.DrunkCrosshairRadius += stageDrunkRadiusIncrease; ;
        targetsKilledInStage = 0;
        timeLeft = currentSpawnPoint.CompletionTime;
        onStartNextStage.Invoke();
    }

    public void EnterShootingStage()
    {
        crowSpawner.enabled = true;
        inShootingStage = true;
        onShootingStageEnter.Invoke();
    }

    public static LevelManager Instance
    {
        get
        {
            if (!instance)
                instance = FindObjectOfType<LevelManager>();
            if (!instance)
            {
                GameObject gameObj = new GameObject("Level Manager");
                instance = gameObj.AddComponent<LevelManager>();
            }

            return instance;
        }
    }

    public UnityEvent OnEnemyKill
    {
        get { return onEnemyKill; }
    }

    public UnityEvent OnGameOver
    {
        get { return onGameOver; }
    }

    public UnityEvent OnQuitLevel
    {
        get { return onQuitLevel; }
    }

    public UnityEvent OnStartNextStage
    {
        get { return onStartNextStage; }
    }

    public UnityEvent OnFirstEmptyGun
    {
        get { return onFirstEmptyGun; }
    }

    public UnityEvent OnClearFirstStage
    {
        get { return onClearFirstStage; }
    }

    public UnityEvent OnShootingStageEnter
    {
        get { return onShootingStageEnter; }
    }

    public bool GameOver
    {
        get { return gameOver; }
    }

    public float TimeLeft
    {
        get { return timeLeft; }
    }

    public int DifficultyLevel
    {
        get { return currentSpawnPoint.DifficultyLevel; }
    }

    public int TargetsKilledInStage
    {
        get { return targetsKilledInStage; }
    }

    public int RequiredKills
    {
        get { return currentSpawnPoint.RequiredKills; }
    }

    public Vector3 CurrentStagePosition
    {
        get { return currentSpawnPoint.StagePosition; }
    }

    public Vector3 CurrentSpawnPointPosition
    {
        get { return currentSpawnPoint.transform.position; }
    }
}
