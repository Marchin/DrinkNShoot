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
    [SerializeField] GameObject completeStageUI;
    [SerializeField] GameObject failLevelUI;
    [SerializeField] GameObject hudUI;
    [SerializeField] GameObject tutorialUI;
    [SerializeField] GameObject introTextUI;

    [Header("Level Properties")]
    [SerializeField] float stageDrunkSpeedIncrease = 0.75f;
    [SerializeField] float stageDrunkRadiusIncrease = 10f;
    [SerializeField] int cashEarnedPerKill = 5;
	[SerializeField] int cashEarnedBronze = 100;
	[SerializeField] int cashEarnedSilver = 150;
	[SerializeField] int cashEarnedGold = 175;

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
    bool gameInStandBy = false;
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
        PlayerManager.Instance.SetComponentReferencesForLevel();

        endLevelMenu = FindObjectOfType<EndLevelMenu>();
        hud = FindObjectOfType<HUD>();

        crowSpawner.enabled = false;
        playersWagon.destination = currentSpawnPoint.transform.position;
        timeLeft = currentSpawnPoint.CompletionTime;

        currentSpawnPoint.EnableStage();

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
        if (!gameInStandBy && !PauseMenu.IsPaused && inShootingStage)
        {
            timeLeft -= Time.deltaTime;

            if (timeLeft <= 0)
            {
                if (targetsKilledInStage >= currentSpawnPoint.BronzeTierKills)
                    CompleteStage();
                else
                    FailLevel();
            }
        }
    }

    void CompleteStage()
    {
        gameInStandBy = true;
		completeStageUI.SetActive(true);

        hudUI.SetActive(false);
        Time.timeScale = 0;
        GameManager.Instance.ShowCursor();

        cashEarnedInStage = targetsKilledInStage * cashEarnedPerKill;

		if (targetsKilledInStage >= currentSpawnPoint.GoldTierKills)
		{
			stageCompletionTier = StageCompletionTier.Gold;
			cashEarnedInStage += cashEarnedBronze + cashEarnedSilver + cashEarnedGold;
		}
		else
		{
			if (targetsKilledInStage >= currentSpawnPoint.SilverTierKills)
			{
				stageCompletionTier = StageCompletionTier.Silver;
				cashEarnedInStage += cashEarnedBronze + cashEarnedSilver;
			}
			else
			{
				stageCompletionTier = StageCompletionTier.Bronze;
				cashEarnedInStage += cashEarnedBronze;
			}
		}
        
		totalKills += targetsKilledInStage;
        totalIncome += cashEarnedInStage;

        PlayerManager.Instance.Currency += cashEarnedInStage;
        PlayerManager.Instance.TotalKills += targetsKilledInStage;

		if (currentStageIndex != maxStageIndex)
        	endLevelMenu.ChangeEndScreenText(cashEarnedInStage, targetsKilledInStage, stageCompletionTier);
		else
			endLevelMenu.ChangeEndScreenText(totalIncome, totalKills, stageCompletionTier, true);
        
		hud.ChangeCurrencyDisplay(totalIncome);
        completeLevelSound.Play();
        onGameOver.Invoke();
    }

    void FailLevel()
    {
        gameInStandBy = true;
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
        GameManager.Instance.FadeToScene(SceneManager.GetActiveScene().name);
    }

    public void QuitLevel()
    {
        onQuitLevel.Invoke();
        GameManager.Instance.TutorialEnabled = true;
        GameManager.Instance.FadeToScene(GameManager.Instance.MainMenuScene);
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
        currentSpawnPoint.DisableStage();
        
        gameInStandBy = false;
        inShootingStage = false;
        currentStageIndex++;

        playersWagon.destination = enemySpawnPoints[currentStageIndex].position;
        currentSpawnPoint = enemySpawnPoints[currentStageIndex].GetComponent<CrowTrigger>();
        currentSpawnPoint.EnableStage();

        Gun[] currentGuns = playersWagon.gameObject.GetComponentInChildren<WeaponHolder>(true).GetComponentsInChildren<Gun>();

        foreach (Gun gun in currentGuns)
        {
            gun.DrunkCrosshairSpeed += stageDrunkSpeedIncrease;
            gun.DrunkCrosshairRadius += stageDrunkRadiusIncrease;
        }
        
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

    public bool GameInStandBy
    {
        get { return gameInStandBy; }
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

    public int MinimumRequiredKills
    {
        get { return currentSpawnPoint.BronzeTierKills; }
    }

    public int MaximumRequiredKills
    {
        get { return currentSpawnPoint.GoldTierKills; }
    }

    public int RequiredKillsForNextTier
    {
        get
        { 
            int requiredKills = currentSpawnPoint.BronzeTierKills;

            if (targetsKilledInStage >= currentSpawnPoint.SilverTierKills)
                requiredKills = currentSpawnPoint.GoldTierKills;
            else
                if (targetsKilledInStage >= currentSpawnPoint.BronzeTierKills)
                    requiredKills = currentSpawnPoint.SilverTierKills;

            return requiredKills;
        }
    }

    public int CashEarnedBronze
    {
        get { return cashEarnedBronze; }
    }

    public int CashEarnedSilver
    {
        get { return cashEarnedSilver; }
    }

    public int CashEarnedGold
    {
        get { return cashEarnedGold; }
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
