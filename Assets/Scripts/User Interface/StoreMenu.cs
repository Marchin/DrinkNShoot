using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoreMenu : MonoBehaviour
{
	enum ItemOnSale
	{
		Winchester
	}

	[SerializeField] TextMeshProUGUI currencyText;
	[SerializeField] TextMeshProUGUI itemPurchasingText;
	[SerializeField] Button[] gunPurchaseButtons;
	[SerializeField] string[] purchasingPanelTexts;

	void Start()
	{
		GameManager.Instance.ShowCursor();
		currencyText.text = PlayerManager.Instance.Currency.ToString();
	}

	public void PurchaseItem(string itemName)
	{
		bool wasPurchased = false;
		int purchasedItemIndex = -1;

		switch (itemName)
		{
			case "Winchester":
				wasPurchased = StoreManager.Instance.PurchaseGun(itemName);
				purchasedItemIndex = (int)ItemOnSale.Winchester;
				break;
		}

		if (wasPurchased)
		{
			currencyText.text = PlayerManager.Instance.Currency.ToString();
			gunPurchaseButtons[purchasedItemIndex].interactable = false;
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
