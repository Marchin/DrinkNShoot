using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class EndLevelMenu : MonoBehaviour 
{
	[SerializeField] Button continueButton;
	[SerializeField] TextMeshProUGUI titleText;
	[SerializeField] TextMeshProUGUI cashText;
	[SerializeField] TextMeshProUGUI cashAmountText;
	[SerializeField] TextMeshProUGUI killsText;
	[SerializeField] TextMeshProUGUI killsAmountText;
	[SerializeField] TextMeshProUGUI firstButtonText;
	[SerializeField] GameObject stageCompleteUI;
	[SerializeField] GameObject bronzeTierUI;
	[SerializeField] GameObject silverTierUI;
	[SerializeField] GameObject goldTierUI;
	[SerializeField] string[] possibleTitles;
	[SerializeField] string[] possibleCashLegends;
	[SerializeField] string[] possibleKillsLegends;
	[SerializeField] string[] possibleButtonNames;
    [SerializeField] Animator levelFailedAnimator;
    [SerializeField] Animator stageCompleteAnimator;
    [SerializeField] AnimationClip levelFailedFadeOutAnimation;
    [SerializeField] AnimationClip stageCompleteFadeOutAnimation;

	TextMeshProUGUI[] cashBonusesTexts = {null, null, null};
	bool lastStageOfLevel = false;

	UnityEvent onContinue = new UnityEvent();
	
	void Start()
	{
		titleText.text = possibleTitles[0];
		cashText.text = possibleCashLegends[0];
		killsText.text = possibleKillsLegends[0];
		firstButtonText.text = possibleButtonNames[0];

        cashBonusesTexts[0] = bronzeTierUI.GetComponentInChildren<TextMeshProUGUI>();
        cashBonusesTexts[1] = silverTierUI.GetComponentInChildren<TextMeshProUGUI>();
        cashBonusesTexts[2] = goldTierUI.GetComponentInChildren<TextMeshProUGUI>();

        cashBonusesTexts[0].text = "+ $" + LevelManager.Instance.CashEarnedBronze.ToString();
        cashBonusesTexts[1].text = "+ $" + LevelManager.Instance.CashEarnedSilver.ToString();
        cashBonusesTexts[2].text = "+ $" + LevelManager.Instance.CashEarnedGold.ToString();
	}

	void PlayNextLevel()
	{
		GameManager.Instance.FadeToScene(LevelManager.Instance.NextLevelName);
	}

	void Continue()
	{
		stageCompleteUI.SetActive(false);
        GameManager.Instance.HideCursor();
        onContinue.Invoke();
        LevelManager.Instance.MoveToNextStage();
	}
	
	void Restart()
	{
        GameManager.Instance.HideCursor();
        LevelManager.Instance.RestartLevel();
	}

	public void RestartLevel()
	{
        Time.timeScale = 1f;
		levelFailedAnimator.SetTrigger("Fade Out");
		Invoke("Restart", levelFailedFadeOutAnimation.length);
	}

	public void PlayNextStage()
	{
		Time.timeScale = 1f;
		stageCompleteAnimator.SetTrigger("Fade Out");
		if (!lastStageOfLevel)
			Invoke("Continue", stageCompleteFadeOutAnimation.length);
		else
			Invoke("PlayNextLevel", stageCompleteFadeOutAnimation.length);
	}

	public void Quit()
	{
		Time.timeScale = 1f;
		LevelManager.Instance.QuitLevel();
	}

	public void ChangeEndScreenText(int cash, int kills, LevelManager.StageCompletionTier tier, bool levelCompleted = false, bool lastLevel = false)
	{
		if (levelCompleted)
		{
			titleText.text = possibleTitles[1];
            cashText.text = possibleCashLegends[1];
            killsText.text = possibleKillsLegends[1];
			if (!lastLevel)
            	firstButtonText.text = possibleButtonNames[1];
			else
				continueButton.gameObject.SetActive(false);
			
			lastStageOfLevel = true;
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
				cashBonusesTexts[0].gameObject.SetActive(true);
				cashBonusesTexts[1].gameObject.SetActive(true);
				cashBonusesTexts[2].gameObject.SetActive(true);
				break;

			case LevelManager.StageCompletionTier.Silver:

                foreach (Image image in goldImages)
                    if (image.gameObject.name != goldTierUI.name)
                        image.gameObject.SetActive(image.gameObject.name == "Failed");
				cashBonusesTexts[0].gameObject.SetActive(true);
				cashBonusesTexts[1].gameObject.SetActive(true);
				cashBonusesTexts[2].gameObject.SetActive(false);
				break;

			case LevelManager.StageCompletionTier.Bronze:
                
                foreach (Image image in goldImages)
                    if (image.gameObject.name != goldTierUI.name)
                        image.gameObject.SetActive(image.gameObject.name == "Failed");	
                foreach (Image image in silverImages)
                    if (image.gameObject.name != silverTierUI.name)
                        image.gameObject.SetActive(image.gameObject.name == "Failed");
                cashBonusesTexts[0].gameObject.SetActive(true);
                cashBonusesTexts[1].gameObject.SetActive(false);
                cashBonusesTexts[2].gameObject.SetActive(false);
                break;
		}
	}

	public UnityEvent OnContinue
	{
		get { return onContinue; }
	}
}