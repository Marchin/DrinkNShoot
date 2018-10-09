using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class EndLevelMenu : MonoBehaviour 
{
	[SerializeField] UnityEvent onContinue;
	[SerializeField] TextMeshProUGUI cashEarnedText;
	[SerializeField] TextMeshProUGUI totalIncomeText;
	[SerializeField] TextMeshProUGUI killsText;
	[SerializeField] TextMeshProUGUI totalKillsText;


	public void Restart()
	{
		Time.timeScale = 1;
		LevelManager.Instance.RestartLevel();
	}

	public void PlayNextStage()
	{
		Time.timeScale = 1;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		LevelManager.Instance.MoveToNextStage();
		onContinue.Invoke();
	}

	public void Quit()
	{
		Time.timeScale = 1;
		LevelManager.Instance.QuitLevel();
	}

	public void ChangeEndScreenText(int cashEarned, int totalIncome, int kills, int totalKills)
	{
        cashEarnedText.text = "$" + cashEarned.ToString();
        totalIncomeText.text = "$" + totalIncome.ToString();
        killsText.text = kills.ToString();
        totalKillsText.text = totalKills.ToString();
	}

	public UnityEvent OnContinue
	{
		get { return onContinue; }
	}
}
