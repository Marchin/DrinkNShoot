using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
	[Header("UI Refreneces")]
	[SerializeField] GameObject completeLevelUI;
	[SerializeField] GameObject failLevelUI;
	[SerializeField] GameObject hudUI;
	[Header("Level Properties")]
	[SerializeField] Life[] enemiesLife;
	[SerializeField] [Range(20, 600)] float completionTime;
	[SerializeField] [Range(0, 25)] int difficultyLevel;
	[SerializeField] [Range(1, 200)] int requiredKills;
	[SerializeField] UnityEvent onEnemyKill;
	[SerializeField] UnityEvent onGameOver;
	const float QUIT_DELAY = 0.5f;
	static LevelManager instance;
	bool gameOver = false;
	float timeLeft = 0;
	int targetsKilled = 0;

	void Awake()
	{
		foreach (Life life in enemiesLife)
			life.OnDeath.AddListener(IncreaseKillCounter);
	}

	void Start()
	{
		timeLeft = completionTime;
	}

	void Update()
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

	void CompleteLevel()
	{
		gameOver = true;
		completeLevelUI.SetActive(true);
		hudUI.SetActive(false);
		onGameOver.Invoke();
	}

	void FailLevel()
	{
        gameOver = true;
        failLevelUI.SetActive(true);
        hudUI.SetActive(false);
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

	public void RestartLevel()
	{
		Invoke("Restart", QUIT_DELAY);
	}

	public void QuitLevel()
	{
		Invoke("Quit", QUIT_DELAY);
	}

	public UnityEvent OnEnemyKill
	{
		get { return onEnemyKill; }
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

	public UnityEvent OnGameOver
	{
		get { return onGameOver; }
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
