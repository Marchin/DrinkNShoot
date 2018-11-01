using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StoreMenu : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI currencyText;

	void Start()
	{
		currencyText.text = PlayerManager.Instance.Currency.ToString();
	}

	public void PurchaseWinchester()
	{
		// StoreManager.Instance.PurchaseGun();
	}

	public void QuitStore()
	{
		GameManager.Instance.FadeToScene(GameManager.Instance.MainMenuScene);
	}
}
