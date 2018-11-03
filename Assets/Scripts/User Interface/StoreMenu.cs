using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoreMenu : MonoBehaviour
{
	enum ItemOnSale
	{
		Winchester, SnakeOil, Bait
	}

	[SerializeField] TextMeshProUGUI currencyText;
	[SerializeField] TextMeshProUGUI itemPurchasingText;
	[SerializeField] Button[] itemPurchaseButtons;
	[SerializeField] string[] purchasingPanelTexts;

	void Start()
	{
		GameManager.Instance.ShowCursor();
		currencyText.text = PlayerManager.Instance.Currency.ToString();
	}

	public void PurchaseItem(string itemName)
	{
		bool wasPurchased = StoreManager.Instance.PurchaseItem(itemName);
		int purchasedItemIndex = -1;

		if (wasPurchased)
		{
			switch (itemName)
			{
				case "Winchester":
					purchasedItemIndex = (int)ItemOnSale.Winchester;
					itemPurchaseButtons[purchasedItemIndex].interactable = false;
					break;
				case "Snake Oil":
					purchasedItemIndex = (int)ItemOnSale.SnakeOil;
					break;
				case "Bait":
					purchasedItemIndex = (int)ItemOnSale.Bait;
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
