using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreManager : MonoBehaviour
{
	[Header("Items")]
	[SerializeField] PlayerManager.Item[] items;
	[SerializeField] int[] prices;
	
	Dictionary<PlayerManager.Item, int> itemPrices = new Dictionary<PlayerManager.Item, int>();
	
	static StoreManager instance;

	void Awake()
	{
		if (Instance == this)
			DontDestroyOnLoad(gameObject);
		else
			Destroy(gameObject);
	}

	void Start()
	{
		GameManager.Instance.ShowCursor();
		int i = 0;
		foreach (PlayerManager.Item item in items)
		{
			itemPrices[item] = prices[i];
			i++;
		}
	}

	public void PurchaseItem(int itemIndex)
	{
		PlayerManager.Item item = (PlayerManager.Item)itemIndex;

		if (PlayerManager.Instance.HasItem(item) && PlayerManager.Instance.Currency >= itemPrices[item])
		{
			PlayerManager.Instance.Currency -= itemPrices[item];
			PlayerManager.Instance.AddItem(item);
		}
	}

	public StoreManager Instance
	{
		get
		{
			if (!instance)
				instance = FindObjectOfType<StoreManager>();
			if (!instance)
			{
				GameObject gameObj = new GameObject("Store Manager");
				instance = gameObj.AddComponent<StoreManager>();
			}

			return instance;
		}
	}
}
