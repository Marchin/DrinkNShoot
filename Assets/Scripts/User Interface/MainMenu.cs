using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI appVersionText;
	[SerializeField] Button[] levelsButtons;

	void Start()
	{
		GameManager.Instance.ShowCursor();
		appVersionText.text = "Application Version: " + Application.version;
	}

	public void Play(int level)
	{
		GameManager.Instance.HideCursor();
        GameManager.Instance.FadeToScene(GameManager.Instance.GetLevelSceneName(level));
	}

	public void ShowUnlockedLevels()
	{
		int i = 1;
		
		foreach (Button levelButton in levelsButtons)
		{
			levelButton.interactable = (i <= GameManager.Instance.LastLevelUnlocked);
			levelButton.GetComponent<EventTrigger>().enabled = levelButton.IsInteractable();
			i++;
		}
	}

	public void EnterStore()
	{
		GameManager.Instance.FadeToScene(GameManager.Instance.StoreScene);
	}

	public void Quit()
	{
		GameManager.Instance.QuitApplication();
	}
}