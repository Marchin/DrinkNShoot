using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoreMenu : MonoBehaviour
{
	public enum ItemOnSale
	{
        None = -1, Winchester, SnakeOil, Bait
	}

	[Header("Texts")]
	[SerializeField] TextMeshProUGUI currencyText;
	[SerializeField] TextMeshProUGUI itemPurchasingText;
	[SerializeField] TextMeshProUGUI itemDescriptionText;
	[SerializeField] TextMeshProUGUI[] consumablesAmountText;

	[Header("Buttons")]
	[SerializeField] Button[] itemPurchaseButtons;
	[SerializeField] Button[] itemDescriptionButtons;

    [Header("Game Objects")]
    [SerializeField] GameObject[] itemPrices;
	[SerializeField] GameObject descriptionPanel;

	[Header("Texts' Contents")]
	[SerializeField] string[] purchasingPanelTexts;
	[SerializeField] string[] descriptionPanelTexts;

    [Header("Animations")]
    [SerializeField] AnimationClip descriptionPopOutAnimation;

    [Header("Audio Sources")]
    [SerializeField] AudioSource hoverOverItemSound;
	[SerializeField] AudioSource purchaseItemSound;
	[SerializeField] AudioSource notPurchaseItemSound;
	[SerializeField] AudioSource descriptionPopInSound;
	[SerializeField] AudioSource descriptionPopOutSound;
	[SerializeField] AudioSource[] descriptionVoiceActing;

	Animator descriptionAnimator;
	MenuMusicPlayer musicPlayer;

	void Awake()
	{
		descriptionAnimator = descriptionPanel.GetComponent<Animator>();
		musicPlayer = GetComponent<MenuMusicPlayer>();
	}

	void Start()
	{
		GameManager.Instance.ShowCursor();
		currencyText.text = PlayerManager.Instance.Currency.ToString();

		int i = 0;
		foreach (IItem item in StoreManager.Instance.ItemsStock.Keys)
		{
			itemPrices[i].GetComponentInChildren<TextMeshProUGUI>().text = StoreManager.Instance.ItemsStock[item].ToString();
			if (item.GetItemType() != ItemType.Gun)
			{
				string currentAmount = item.GetAmount().ToString();
				string maxAmount = item.GetMaxAmount().ToString();
				
				consumablesAmountText[i].text = currentAmount + "/" + maxAmount;
			}
			if (PlayerManager.Instance.HasItem(item) && PlayerManager.Instance.GetItemAmount(item) == item.GetMaxAmount())
			{
				itemPurchaseButtons[i].interactable = false;
				itemPrices[i].SetActive(false);
			}
			i++;
		}
	}

	public void PurchaseItem(string itemName)
	{
		bool wasPurchased = StoreManager.Instance.PurchaseItem(itemName);
		ItemOnSale purchasedItem = ItemOnSale.None;

		if (wasPurchased)
		{
			purchaseItemSound.Play();
			
			switch (itemName)
			{
				case "Winchester":
					purchasedItem = ItemOnSale.Winchester;
					itemPurchaseButtons[(int)purchasedItem].interactable = false;
					itemPrices[(int)purchasedItem].SetActive(false);
					break;
				case "Snake Oil":
					purchasedItem = ItemOnSale.SnakeOil;
					break;
				case "Bait":
					purchasedItem = ItemOnSale.Bait;
					break;
			}

			switch (purchasedItem)
			{
				case ItemOnSale.SnakeOil:
				case ItemOnSale.Bait:
                    int currentAmount = PlayerManager.Instance.GetItemAmount(itemName);
                    int maxAmount = PlayerManager.Instance.GetItemMaxAmount(itemName);
					
					consumablesAmountText[(int)purchasedItem].text = currentAmount.ToString() + "/" + maxAmount.ToString();
					
					if (currentAmount == maxAmount)
					{
						itemPurchaseButtons[(int)purchasedItem].interactable = false;
						itemPrices[(int)purchasedItem].SetActive(false);
					}
					break;
				default:
					break;
			}

			currencyText.text = PlayerManager.Instance.Currency.ToString();
			itemPurchasingText.text = purchasingPanelTexts[0];
		}
		else
		{
			notPurchaseItemSound.Play();

			if (PlayerManager.Instance.Currency >= StoreManager.Instance.GetItemPrice(itemName))
            	itemPurchasingText.text = purchasingPanelTexts[2];
			else
            	itemPurchasingText.text = purchasingPanelTexts[1];
		}
    }

	public void PlayHoverOverItemSound(Button button)
	{
		if (button.IsInteractable())
			hoverOverItemSound.Play();
	}

    public void QuitStore()
    {
        GameManager.Instance.FadeToScene(GameManager.Instance.MainMenuScene);
    }

	public void PopInDescription(int itemIndex)
	{	
		musicPlayer.ThemeSong.volume = 0.25f;
		
		foreach (Button button in itemDescriptionButtons)
			button.interactable = false;	
		
		itemDescriptionText.text = descriptionPanelTexts[itemIndex];
		descriptionPanel.SetActive(true);
		descriptionPopInSound.Play();
		descriptionVoiceActing[itemIndex].PlayDelayed(0.1f);
		StartCoroutine(PopOutDescription(itemIndex));
	}

    IEnumerator PopOutDescription(int itemIndex)
    {
		float timer = 0f;
		
		while (timer < descriptionVoiceActing[itemIndex].clip.length)
		{
			timer += Time.deltaTime;
			if (InputManager.Instance.GetFireButton())
				timer = descriptionVoiceActing[itemIndex].clip.length;

			yield return null;
		}

        descriptionAnimator.SetTrigger("Pop Out");
        descriptionPopOutSound.Play();
		if (descriptionVoiceActing[itemIndex].isPlaying)
			descriptionVoiceActing[itemIndex].Stop();
		Invoke("DisableDescription", descriptionPopOutAnimation.length);
    }

	public void DisableDescription()
	{
		musicPlayer.ThemeSong.volume = 1f;

		descriptionPanel.SetActive(false);
        foreach (Button button in itemDescriptionButtons)
            button.interactable = true;
	}
}