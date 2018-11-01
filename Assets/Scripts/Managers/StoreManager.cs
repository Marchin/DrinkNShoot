using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreManager : MonoBehaviour
{
	[Header("Items")]
	[SerializeField] Gun[] gunsComponentsPrefabs;
	[SerializeField] int[] gunsPrices;
	
	Dictionary<Gun, int> gunsStock = new Dictionary<Gun, int>();
	
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
		foreach (Gun gun in gunsComponentsPrefabs)
		{
			gunsStock[gun] = gunsPrices[i];
			i++;
		}
	}

	public bool PurchaseGun(string gunName)
	{
		bool wasPurchased = false;

		foreach (Gun gun in gunsStock.Keys)
		{
			if (gun.GunName == gunName)
			{
				if (!PlayerManager.Instance.HasGun(gun) && PlayerManager.Instance.Currency >= gunsStock[gun])
				{
					wasPurchased = true;
					PlayerManager.Instance.Currency -= gunsStock[gun];
					PlayerManager.Instance.AddGun(gun);
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
