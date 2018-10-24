using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class EndLevelMenu : MonoBehaviour 
{
	[SerializeField] TextMeshProUGUI titleText;
	[SerializeField] TextMeshProUGUI cashText;
	[SerializeField] TextMeshProUGUI cashAmountText;
	[SerializeField] TextMeshProUGUI killsText;
	[SerializeField] TextMeshProUGUI killsAmountText;
	[SerializeField] TextMeshProUGUI firstButtonText;
	[SerializeField] GameObject bronzeTierUI;
	[SerializeField] GameObject silverTierUI;
	[SerializeField] GameObject goldTierUI;
	[SerializeField] string[] possibleTitles;
	[SerializeField] string[] possibleCashLegends;
	[SerializeField] string[] possibleKillsLegends;
	[SerializeField] string[] possibleButtonNames;
	[SerializeField] UnityEvent onContinue;

	void Start()
	{
		titleText.text = possibleTitles[0];
		cashText.text = possibleCashLegends[0];
		killsText.text = possibleKillsLegends[0];
		firstButtonText.text = possibleButtonNames[0];
	}

	public void Restart()
	{
		Time.timeScale = 1f;
		GameManager.Instance.HideCursor();
		LevelManager.Instance.RestartLevel();
	}

	public void PlayNextStage()
	{
		Time.timeScale = 1f;
		GameManager.Instance.HideCursor();
		onContinue.Invoke();
		LevelManager.Instance.MoveToNextStage();
	}

	public void Quit()
	{
		Time.timeScale = 1f;
		LevelManager.Instance.QuitLevel();
	}

	public void ChangeEndScreenText(int cash, int kills, LevelManager.StageCompletionTier tier, bool levelCompleted = false)
	{
		if (levelCompleted)
		{
			titleText.text = possibleTitles[1];
            cashText.text = possibleCashLegends[1];
            killsText.text = possibleKillsLegends[1];
            firstButtonText.text = possibleButtonNames[1];
		}

        cashAmountText.text = "$" + cash.ToString();
        killsAmountText.text = kills.ToString();

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
