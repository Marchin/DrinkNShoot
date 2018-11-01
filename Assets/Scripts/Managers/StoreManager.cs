using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreManager : MonoBehaviour
{
	[Header("Items")]
	[SerializeField] Gun[] guns;
	[SerializeField] int[] gunsPrices;
	
	Dictionary<Gun, int> gunsPricesDict = new Dictionary<Gun, int>();
	
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
		foreach (Gun gun in guns)
		{
			gunsPricesDict[gun] = gunsPrices[i];
			i++;
		}
	}

	public void PurchaseGun(Gun gun)
	{
		if (!PlayerManager.Instance.HasGun(gun) && PlayerManager.Instance.Currency >= gunsPricesDict[gun])
		{
			PlayerManager.Instance.Currency -= gunsPricesDict[gun];
			PlayerManager.Instance.AddGun(gun);
		}
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
