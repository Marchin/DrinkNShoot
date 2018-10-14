using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
	[Header("Entities References")]
	[SerializeField] NavMeshAgent playersWagon;
	[SerializeField] CrowSpawner crowSpawner;
	[Header("UI References")]
	[SerializeField] GameObject completeLevelUI;
	[SerializeField] GameObject completeStageUI;
	[SerializeField] GameObject failLevelUI;
	[SerializeField] GameObject hudUI;
	[SerializeField] GameObject tutorialUI;
	[Header("Level Properties")]
	[SerializeField] int cashEarnedPerKill = 5;
	[SerializeField] bool tutorialEnabled = true;
	[Header("Sounds")]
	[SerializeField] AudioSource completeLevelSound;
	[SerializeField] AudioSource failLevelSound;
	[Header("Events")]
	[SerializeField] UnityEvent onEnemyKill;
	[SerializeField] UnityEvent onGameOver;
	[SerializeField] UnityEvent onStartNextStage;
	[SerializeField] UnityEvent onFirstEmptyGun;
	[SerializeField] UnityEvent onClearFirstStage;
	List<Transform> enemySpawnPoints;
	CrowTrigger currentSpawnPoint;
	EndLevelMenu endLevelMenu;
	HUD hud;
	const float QUIT_DELAY = 0.5f;
	static LevelManager instance;
	int currentStageIndex;
	int maxStageIndex;
	bool gameOver = false;
	float timeLeft = 0;
	int targetsKilledInStage = 0;
	int totalKills = 0;
	int cashEarnedInStage = 0;
	int totalIncome = 0;
	bool hasNotifiedEmptyGun = false;
	bool inShootingStage = true;

	void Awake()
	{
		enemySpawnPoints = new List<Transform>();
		currentStageIndex = 0;
	}

	void Start()
	{
		foreach (Transform spawnPoint in crowSpawner.transform)
			if (spawnPoint.gameObject.activeInHierarchy && spawnPoint.GetComponent<CrowTrigger>())
				enemySpawnPoints.Add(spawnPoint);
		maxStageIndex = enemySpawnPoints.Count - 1;
		
		currentSpawnPoint = enemySpawnPoints[currentStageIndex].GetComponent<CrowTrigger>();
		endLevelMenu = FindObjectOfType<EndLevelMenu>();
		hud = FindObjectOfType<HUD>();

		timeLeft = currentSpawnPoint.CompletionTime;

		if (tutorialEnabled)
		{
			playersWagon.gameObject.GetComponentInChildren<WeaponHolder>().EquippedGun.OnEmptyGun.AddListener(FirstEmptyGunNotice);
			tutorialUI.SetActive(true);
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
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		
		totalKills += targetsKilledInStage;
		cashEarnedInStage = targetsKilledInStage * cashEarnedPerKill;
		totalIncome += cashEarnedInStage;
		PlayerManager.Instance.Currency += cashEarnedInStage;
		PlayerManager.Instance.TotalKills += targetsKilledInStage;
		
		endLevelMenu.ChangeEndScreenText(cashEarnedInStage, totalIncome, targetsKilledInStage, totalKills);
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
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		failLevelSound.Play();
		onGameOver.Invoke();
	}

	void IncreaseKillCounter()
	{
		targetsKilledInStage++;
		onEnemyKill.Invoke();
	}

	void Restart()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	void Quit()
	{
		SceneManager.LoadScene("Main Menu");
	}

	void FirstEmptyGunNotice()
	{
		if (!hasNotifiedEmptyGun)
		{
			hasNotifiedEmptyGun = true;
			onFirstEmptyGun.Invoke();
		}
	}

	public void RestartLevel()
	{
		Invoke("Restart", QUIT_DELAY);
	}

	public void QuitLevel()
	{
		Invoke("Quit", QUIT_DELAY);
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
		playersWagon.gameObject.GetComponentInChildren<WeaponHolder>().EquippedGun.DrunkCrosshairSpeed++;
		targetsKilledInStage = 0;
		timeLeft = currentSpawnPoint.CompletionTime;
		onStartNextStage.Invoke();
	}

	public void EnterShootingStage()
	{
		crowSpawner.enabled = true;
		inShootingStage = true;
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
}
