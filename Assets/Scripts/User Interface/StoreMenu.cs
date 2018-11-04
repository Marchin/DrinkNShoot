using System.Collections;
using System.Collections.Generic;
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
	[SerializeField] Button[] itemPurchaseButtons;
	[SerializeField] GameObject[] itemPrices;
	[SerializeField] string[] purchasingPanelTexts;

	void Start()
	{
		GameManager.Instance.ShowCursor();
		currencyText.text = PlayerManager.Instance.Currency.ToString();

		int i = 0;
		foreach (IItem item in StoreManager.Instance.ItemsStock.Keys)
		{
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
					if (PlayerManager.Instance.GetItemAmount(itemName) == PlayerManager.Instance.GetItemMaxAmount(itemName))
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
            itemPurchasingText.text = purchasingPanelTexts[1];
    }

	public void QuitStore()
	{
		GameManager.Instance.FadeToScene(GameManager.Instance.MainMenuScene);
	}
}
