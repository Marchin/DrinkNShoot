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
	[SerializeField] GameObject failLevelUI;
	[SerializeField] GameObject hudUI;
	[SerializeField] GameObject tutorialUI;
	[Header("Level Properties")]
	[SerializeField] [Range(20, 600)] float startCompletionTime;
	[SerializeField] [Range(0, 25)] int startDifficultyLevel;
	[SerializeField] [Range(1, 200)] int startRequiredKills;
	[SerializeField] bool tutorialEnabled = true;
	[Header("Events")]
	[SerializeField] UnityEvent onEnemyKill;
	[SerializeField] UnityEvent onGameOver;
	[SerializeField] UnityEvent onStartNextStage;
	[SerializeField] UnityEvent onFirstEmptyGun;
	List<Transform> enemySpawnPoints;
	const float QUIT_DELAY = 0.5f;
	const int DIFFICULTY_INCREASE = 2;
	static LevelManager instance;
	int currentStage;
	float completionTime;
	int difficultyLevel;
	int requiredKills;
	bool gameOver = false;
	float timeLeft = 0;
	int targetsKilled = 0;
	bool hasNotifiedEmptyGun = false;

	void Awake()
	{
		enemySpawnPoints = new List<Transform>();
		completionTime = startCompletionTime;
		timeLeft = completionTime;
		difficultyLevel = startDifficultyLevel;
		requiredKills = startRequiredKills;
		currentStage = 0;
	}

	void Start()
	{
		foreach (Transform spawnPoint in crowSpawner.transform)
			enemySpawnPoints.Add(spawnPoint);

		if (tutorialEnabled)
		{
			playersWagon.gameObject.GetComponentInChildren<WeaponHolder>().EquippedGun.OnEmptyGun.AddListener(FirstEmptyGunNotice);
			tutorialUI.SetActive(true);
		}
	}

	void Update()
	{
		if (!gameOver && !PauseMenu.IsPaused)
		{
			timeLeft -= Time.deltaTime;

			if (timeLeft <= 0)
			{
				if (targetsKilled >= requiredKills)
					CompleteLevel();
				else
					FailLevel();
			}
		}
	}

	void CompleteLevel()
	{
		gameOver = true;
		completeLevelUI.SetActive(true);
		hudUI.SetActive(false);
		Time.timeScale = 0;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
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
		onGameOver.Invoke();
	}

	void IncreaseKillCounter()
	{
		targetsKilled++;
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
		crowSpawner.DisableCrows();
		gameOver = false;
		currentStage++;
		playersWagon.destination = enemySpawnPoints[currentStage].position;
		completionTime = startCompletionTime + 10;
		targetsKilled = 0;
		timeLeft = completionTime;
		difficultyLevel += DIFFICULTY_INCREASE * currentStage;
		requiredKills += DIFFICULTY_INCREASE * currentStage;
		onStartNextStage.Invoke();
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
		get { return difficultyLevel; }
	}

	public int TargetsKilled
	{
		get { return targetsKilled; }
	}

	public int RequiredKills
	{
		get { return requiredKills; }
	}
}
