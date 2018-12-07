using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoreMenu : MonoBehaviour
{
	enum ItemOnSale
	{
        None = -1, Winchester, SnakeOil, Bait
	}

	[SerializeField] TextMeshProUGUI currencyText;
	[SerializeField] TextMeshProUGUI itemPurchasingText;
	[SerializeField] TextMeshProUGUI[] consumablesAmountText;
	[SerializeField] Button[] itemPurchaseButtons;
	[SerializeField] GameObject[] itemPrices;
	[SerializeField] string[] purchasingPanelTexts;
	[SerializeField] AudioSource hoverOverItemSound;
	[SerializeField] AudioSource purchaseItemSound;

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
}
