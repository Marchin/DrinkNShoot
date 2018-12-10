using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelectionScreen : MonoBehaviour
{
	[Header("Texts")]
	[SerializeField] TextMeshProUGUI pageTitleText;

	[Header("Game Objects")]
	[SerializeField] GameObject previousPageButton;
	[SerializeField] GameObject nextPageButton;

	[Header("Sprites")]
	[SerializeField] Sprite[] firstPageSprites;
	[SerializeField] Sprite[] firstPageHlSprites;
	[SerializeField] Sprite[] firstPageDisSprites;
	[SerializeField] Sprite[] secondPageSprites;
	[SerializeField] Sprite[] secondPageHlSprites;
	[SerializeField] Sprite[] secondPageDisSprites;

	[Header("Images")]
	[SerializeField] Image[] pageLevels;

	[Header("Properties")]
	[SerializeField] [Range(1, 10)] int pages;
	[SerializeField] [Range(1, 5)] int levelsPerPage;
	[SerializeField] string[] pageTitles;

	int currentPageIndex;

	void OnEnable()
	{
        currentPageIndex = 0;
		SetUpSprites();
		previousPageButton.SetActive(false);
		nextPageButton.SetActive(true);
    }

	void SetUpSprites()
	{
		Sprite[] targetSprites;
		Sprite[] targetHlSprites;
		Sprite[] targetDisSprites;
		int latestLevel = GameManager.Instance.LastLevelUnlocked;

		switch (currentPageIndex)
		{
			case 0:
				targetSprites = firstPageSprites;
				targetHlSprites = firstPageHlSprites;
				targetDisSprites = firstPageDisSprites;
				break;
			case 1:
                targetSprites = secondPageSprites;
                targetHlSprites = secondPageHlSprites;
                targetDisSprites = secondPageDisSprites;
                break;
			default:
                targetSprites = firstPageSprites;
                targetHlSprites = firstPageHlSprites;
                targetDisSprites = firstPageDisSprites;
				break;
        }

        int i = 0;

        foreach (Image image in pageLevels)
        {
			SpriteState spriteState = new SpriteState();
			Button button = image.GetComponent<Button>();
			
			image.sprite = targetSprites[i];
            spriteState.highlightedSprite = targetHlSprites[i];
            spriteState.disabledSprite = targetDisSprites[i];
			button.spriteState = spriteState;
			button.interactable = (++i + levelsPerPage * currentPageIndex <= latestLevel);
        }
	}

    public void Play(int levelInPage)
    {
        GameManager.Instance.HideCursor();
        GameManager.Instance.FadeToScene(GameManager.Instance.GetLevelSceneName(levelInPage + levelsPerPage * currentPageIndex));
    }

	public void MoveToNextPage()
	{
		if (currentPageIndex < pages)
		{
			currentPageIndex++;
			pageTitleText.text = pageTitles[currentPageIndex];
			SetUpSprites();
            if (currentPageIndex == pages - 1)
                nextPageButton.SetActive(false);
			if (!previousPageButton.activeInHierarchy)
				previousPageButton.SetActive(true);
		}
	}

    public void MoveToPreviousPage()
    {
		if (currentPageIndex > 0)
		{
			currentPageIndex--;
            pageTitleText.text = pageTitles[currentPageIndex];
            SetUpSprites();
			if (currentPageIndex == 0)
				previousPageButton.SetActive(false);
			if (!nextPageButton.activeInHierarchy)
				nextPageButton.SetActive(true);
		}
    }
}