using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

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
    [SerializeField] float drunkSpeedMultiplier = 0.25f;
    [SerializeField] float drunkRadiusMultiplier = 10f;
    [SerializeField] int cashEarnedPerKill = 5;
	[SerializeField] int cashEarnedBronze = 100;
	[SerializeField] int cashEarnedSilver = 150;
	[SerializeField] int cashEarnedGold = 175;
	[SerializeField] bool showIntroText;
	[SerializeField] bool showTutorial;
	[SerializeField] int levelNumber;
	[SerializeField] string levelName;
	[SerializeField] string nextLevelName;

    [Header("Sounds")]
    [SerializeField] AudioSource completeLevelSound;
    [SerializeField] AudioSource failLevelSound;

    static LevelManager instance;

    List<Transform> enemySpawnPoints;
    Wagon wagon;
    CrowTrigger currentSpawnPoint;
    EndLevelMenu endLevelMenu;
	StageCompletionTier stageCompletionTier;
    int currentStageIndex;
    int maxStageIndex;
    bool gameInStandBy = false;
    float timeLeft = 0;
    int targetsKilledInStage = 0;
    int totalKills = 0;
    int cashEarnedInStage = 0;
    int totalIncome = 0;
    bool inShootingStage = false;

    // Events
    UnityEvent onEnemyKill = new UnityEvent();
    UnityEvent onGameOver = new UnityEvent();
    UnityEvent onQuitLevel = new UnityEvent();
    UnityEvent onStartNextStage = new UnityEvent();
    UnityEvent onFirstEmptyGun = new UnityEvent();
    UnityEvent onClearFirstStage = new UnityEvent();
    UnityEvent onShootingStageEnter = new UnityEvent();

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

    void OnEnable()
    {
        PlayerManager.Instance.SetComponentReferencesForLevel();
    }

    void Start()
    {
        endLevelMenu = FindObjectOfType<EndLevelMenu>();
        wagon = playersWagon.gameObject.GetComponent<Wagon>();

        crowSpawner.enabled = false;
        playersWagon.destination = currentSpawnPoint.transform.position;
        timeLeft = currentSpawnPoint.CompletionTime;

        currentSpawnPoint.EnableStage();
        SetGunsCrosshairsParameters();

        if (showTutorial && GameManager.Instance.TutorialEnabled)
        {
            playersWagon.gameObject.GetComponentInChildren<WeaponHolder>().EquippedGun.OnEmptyGun.AddListener(FirstEmptyGunNotice);
            tutorialUI.SetActive(true);
            if (showIntroText)
            {
                introTextUI.SetActive(true);
                introTextUI.GetComponent<IntroText>().OnPresentationFinished.AddListener(StartMovingWagon);
            }
        }
        else
            wagon.Move();
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

    void StartMovingWagon()
    {
        wagon.Move();
    }

    void CompleteStage()
    {
        gameInStandBy = true;

        PlayerManager.Instance.StopDeadEyeEffect();

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

        Time.timeScale = 0f;
        hudUI.SetActive(false);
        completeStageUI.SetActive(true);
        GameManager.Instance.ShowCursor();

        if (currentStageIndex != maxStageIndex)
        	endLevelMenu.ChangeEndScreenText(cashEarnedInStage, targetsKilledInStage, stageCompletionTier);
		else
        {
            GameManager.Instance.LastLevelUnlocked = levelNumber + 1;
			endLevelMenu.ChangeEndScreenText(totalIncome, totalKills, stageCompletionTier, true, nextLevelName == "");
        }
        
        completeLevelSound.Play();
        onGameOver.Invoke();
    }

    void FailLevel()
    {
        gameInStandBy = true;
        PlayerManager.Instance.StopDeadEyeEffect();
        Time.timeScale = 0f;
        hudUI.SetActive(false);
        failLevelUI.SetActive(true);
        GameManager.Instance.ShowCursor();
        if (currentStageIndex == maxStageIndex)
            GameManager.Instance.TutorialEnabled = false;
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
        playersWagon.GetComponentInChildren<WeaponHolder>().EquippedGun.StopReloadingImmediately();
        GameManager.Instance.FadeToScene(levelName);
    }

    public void QuitLevel()
    {
        onQuitLevel.Invoke();
        playersWagon.GetComponentInChildren<WeaponHolder>().EquippedGun.StopReloadingImmediately();
        if (currentStageIndex == maxStageIndex)
            GameManager.Instance.TutorialEnabled = false;
        GameManager.Instance.FadeToScene(GameManager.Instance.MainMenuScene);
    }

    void FirstEmptyGunNotice()
    {
        if (timeLeft > 10f)
        {
            playersWagon.GetComponentInChildren<WeaponHolder>().EquippedGun.OnEmptyGun.RemoveListener(FirstEmptyGunNotice);
            onFirstEmptyGun.Invoke();
        }
    }

    void SetGunsCrosshairsParameters()
    {
        DrunkCrosshair[] crosshairs = playersWagon.GetComponentInChildren<WeaponHolder>().GetComponentsInChildren<DrunkCrosshair>(true);

        foreach (DrunkCrosshair crosshair in crosshairs)
        {
            crosshair.BaseSpeed = currentSpawnPoint.DifficultyLevel * drunkSpeedMultiplier;
            crosshair.BaseRadius = currentSpawnPoint.DifficultyLevel * drunkRadiusMultiplier;
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
        wagon.Move();
        currentSpawnPoint = enemySpawnPoints[currentStageIndex].GetComponent<CrowTrigger>();
        currentSpawnPoint.EnableStage();

        SetGunsCrosshairsParameters();
        
        targetsKilledInStage = 0;
        timeLeft = currentSpawnPoint.CompletionTime;
        onStartNextStage.Invoke();    
    }

    public void EnterShootingStage()
    {
        crowSpawner.enabled = true;
        inShootingStage = true;
        wagon.Stop();
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

    public int RequiredKillsForCurrentTier
    {
        get
        { 
            int requiredKills = currentSpawnPoint.BronzeTierKills;

            if (targetsKilledInStage > currentSpawnPoint.SilverTierKills)
                requiredKills = currentSpawnPoint.GoldTierKills;
            else
                if (targetsKilledInStage > currentSpawnPoint.BronzeTierKills)
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

    public string NextLevelName
    {
        get { return nextLevelName; }
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
