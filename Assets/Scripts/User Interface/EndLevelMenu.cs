using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class EndLevelMenu : MonoBehaviour 
{
	[SerializeField] UnityEvent onContinue;
	[SerializeField] TextMeshProUGUI cashEarnedText;
	[SerializeField] TextMeshProUGUI totalIncomeText;
	[SerializeField] TextMeshProUGUI killsText;
	[SerializeField] TextMeshProUGUI totalKillsText;
	[SerializeField] GameObject bronzeTierUI;
	[SerializeField] GameObject silverTierUI;
	[SerializeField] GameObject goldTierUI;

	public void Restart()
	{
		Time.timeScale = 1;
		GameManager.Instance.HideCursor();
		LevelManager.Instance.RestartLevel();
	}

	public void PlayNextStage()
	{
		Time.timeScale = 1;
		GameManager.Instance.HideCursor();
		onContinue.Invoke();
		LevelManager.Instance.MoveToNextStage();
	}

	public void Quit()
	{
		Time.timeScale = 1;
		LevelManager.Instance.QuitLevel();
	}

	public void ChangeEndScreenText(int cashEarned, int totalIncome, int kills, int totalKills, LevelManager.StageCompletionTier tier)
	{
        cashEarnedText.text = "$" + cashEarned.ToString();
        totalIncomeText.text = "$" + totalIncome.ToString();
        killsText.text = kills.ToString();
        totalKillsText.text = totalKills.ToString();

		Image[] goldImages = goldTierUI.GetComponentsInChildren<Image>(true);		
		Image[] silverImages = silverTierUI.GetComponentsInChildren<Image>(true);
		Image[] bronzeImages = bronzeTierUI.GetComponentsInChildren<Image>(true);

		foreach (Image image in goldImages)
			if (image.gameObject.name != goldTierUI.name)
				image.gameObject.SetActive(image.gameObject.name == "Completed");
		foreach (Image image in silverImages)
            if (image.gameObject.name != silverTierUI.name)
                image.gameObject.SetActive(image.gameObject.name == "Completed");
		foreach (Image image in bronzeImages)
            if (image.gameObject.name != bronzeTierUI.name)
                image.gameObject.SetActive(image.gameObject.name == "Completed");

		switch (tier)
		{
			case LevelManager.StageCompletionTier.Gold:
				break;

			case LevelManager.StageCompletionTier.Silver:

                foreach (Image image in goldImages)
                    if (image.gameObject.name != goldTierUI.name)
                        image.gameObject.SetActive(image.gameObject.name == "Failed");
				break;

			case LevelManager.StageCompletionTier.Bronze:
                
                foreach (Image image in goldImages)
                    if (image.gameObject.name != goldTierUI.name)
                        image.gameObject.SetActive(image.gameObject.name == "Failed");	
                foreach (Image image in silverImages)
                    if (image.gameObject.name != silverTierUI.name)
                        image.gameObject.SetActive(image.gameObject.name == "Failed");	
				break;
		}
	}

	public UnityEvent OnContinue
	{
		get { return onContinue; }
	}
}
