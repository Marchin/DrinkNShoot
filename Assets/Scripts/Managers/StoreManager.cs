using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreManager : MonoBehaviour
{
	[Header("Items")]
	[SerializeField] Gun[] gunsComponentsPrefabs;
	[SerializeField] int[] itemsPrices;
	
	Dictionary<IItem, int> itemsStock = new Dictionary<IItem, int>();
	
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
		int i = 0;
		foreach (IItem item in gunsComponentsPrefabs)
		{
			itemsStock[item] = itemsPrices[i];
			i++;
		}
	}

	public bool PurchaseItem(string itemName)
	{
		bool wasPurchased = false;

		foreach (IItem item in itemsStock.Keys)
		{
			if (item.GetName() == itemName)
			{
				switch (item.GetItemType())
				{
					case ItemType.Gun:
						if (!PlayerManager.Instance.HasGun((Gun)item) && PlayerManager.Instance.Currency >= itemsStock[item])
						{
							wasPurchased = true;
							PlayerManager.Instance.Currency -= itemsStock[item];
							PlayerManager.Instance.AddGun((Gun)item);
						}
						break;
					case ItemType.Consumable:
						break;
				}

				break;
			}
		}

		return wasPurchased;
	}

	public static StoreManager Instance
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
