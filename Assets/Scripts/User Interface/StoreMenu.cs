using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoreMenu : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI currencyText;
	[SerializeField] Button purchaseButton;

	void Start()
	{
		GameManager.Instance.ShowCursor();
		currencyText.text = PlayerManager.Instance.Currency.ToString();
	}

	public void PurchaseItem(string itemName)
	{
		
		bool wasPurchased = false;

		switch (itemName)
		{
			case "Winchester":
				wasPurchased = StoreManager.Instance.PurchaseGun(itemName);
				break;
		}

		if (wasPurchased)
		{
			currencyText.text = PlayerManager.Instance.Currency.ToString();
			purchaseButton.interactable = false;
		}
	}

	public void QuitStore()
	{
		GameManager.Instance.FadeToScene(GameManager.Instance.MainMenuScene);
	}
}
