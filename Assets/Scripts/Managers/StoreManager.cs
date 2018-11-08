using System.Collections.Generic;
using UnityEngine;

public class StoreManager : MonoBehaviour
{
	[Header("Items")]
	[SerializeField] Gun[] gunsComponentsPrefabs;
	[SerializeField] Consumable[] consumableComponentsPrefabs;
	[SerializeField] int[] itemsPrices;
	
	Dictionary<IItem, int> itemsStock = new Dictionary<IItem, int>();
	
	static StoreManager instance;

	void Awake()
	{
		if (Instance == this)
		{
			DontDestroyOnLoad(gameObject);
            
			IItem[] itemComponentsPrefabs = new IItem[gunsComponentsPrefabs.GetLength(0) + consumableComponentsPrefabs.GetLength(0)];
            gunsComponentsPrefabs.CopyTo(itemComponentsPrefabs, 0);
            consumableComponentsPrefabs.CopyTo(itemComponentsPrefabs, gunsComponentsPrefabs.GetLength(0));
            int i = 0;
            foreach (IItem item in itemComponentsPrefabs)
            {
                itemsStock[item] = itemsPrices[i];
                i++;
            }
		}
		else
			Destroy(gameObject);
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
							itemsStock[item] = -1;
						}
						break;
					case ItemType.Consumable:
						if (PlayerManager.Instance.GetItemAmount(item) < item.GetMaxAmount() && PlayerManager.Instance.Currency >= itemsStock[item])
						{
							wasPurchased = true;
							PlayerManager.Instance.Currency -= itemsStock[item];
							PlayerManager.Instance.AddConsumableStock(item.GetName(), 1);
						}
						break;
				}

				break;
			}
		}

		return wasPurchased;
	}

	public Dictionary<IItem, int> ItemsStock
	{
		get { return itemsStock; }
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
