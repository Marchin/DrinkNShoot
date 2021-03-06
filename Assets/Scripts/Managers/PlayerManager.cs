﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : MonoBehaviour
{
	public enum PlayerComponent
	{
		CameraRotationComp, DrunkCameraComp, GunComp, WeaponHolderComp, AnimatorComp
	}

	[SerializeField] GameObject weaponHolderPrefab;

	static PlayerManager instance;
	
	List<Gun> activeGuns;
	List<Consumable> activeConsumables;
	CameraRotation cameraRotation;
	DrunkCamera drunkCamera;
	WeaponHolder weaponHolder;
	Animator playerAnimator;
	int currency;
	int totalKills;
	bool[] hasGun = { true, false };
	int[] consumablesAmount = { 0, 0 };

    UnityEvent onGunEnable = new UnityEvent();
    UnityEvent onGunDisable = new UnityEvent();

	void Awake() 
	{
		if (Instance == this)
		{
			DontDestroyOnLoad(gameObject);

            activeGuns = new List<Gun>();
            activeConsumables = new List<Consumable>();

			currency = PlayerPrefs.GetInt("Currency", 0);

			hasGun[1] = (PlayerPrefs.GetInt("Has Winchester", 0) == 1);

			int i = 0;

            foreach (Transform gunObject in weaponHolderPrefab.GetComponent<WeaponHolder>().GunHolder)
            {
                if (hasGun[i])
                {
					gunObject.gameObject.SetActive(true);
                    Gun gun = gunObject.GetComponent<Gun>();
                    if (gun)
                        activeGuns.Add(gun);
                }
				i++;
            }

			i = 0;
            
			foreach (Transform consumableObject in weaponHolderPrefab.GetComponent<WeaponHolder>().ConsumableHolder)
            {
                Consumable consumable = consumableObject.GetComponent<Consumable>();
                if (consumable)
				{
					string consName = consumable.GetName();
                    activeConsumables.Add(consumable);
					consumablesAmount[i] = PlayerPrefs.GetInt(consName + " Amount", 0);
					consumable.IncreaseAmount(consumablesAmount[i]);
				}
				i++;
            }
		}
		else
			Destroy(gameObject);
	}

	void EnablePlayer()
	{
		cameraRotation.enabled = true;
		drunkCamera.enabled = true;
		weaponHolder.enabled = true;
		weaponHolder.EquippedGun.enabled = true;
		if (weaponHolder.EquippedConsumable)
			weaponHolder.EquippedConsumable.enabled = true;
		playerAnimator.enabled = true;

		onGunEnable.Invoke();
	}

	void DisablePlayer()
	{
		cameraRotation.enabled = false;
		drunkCamera.enabled = false;
		weaponHolder.enabled = false ;
        weaponHolder.EquippedGun.enabled = false;
		if (weaponHolder.EquippedConsumable)
			weaponHolder.EquippedConsumable.enabled = false;
		playerAnimator.enabled = false;

		onGunDisable.Invoke();
	}

    public void SetComponentReferencesForLevel()
    {
        cameraRotation = FindObjectOfType<CameraRotation>();
        drunkCamera = FindObjectOfType<DrunkCamera>();
        weaponHolder = FindObjectOfType<WeaponHolder>();
        playerAnimator = cameraRotation.gameObject.GetComponentInChildren<Animator>();

        LevelManager.Instance.OnGameOver.AddListener(DisablePlayer);
        FindObjectOfType<PauseMenu>().OnPause.AddListener(DisablePlayer);
        FindObjectOfType<PauseMenu>().OnResume.AddListener(EnablePlayer);
        FindObjectOfType<EndLevelMenu>().OnContinue.AddListener(EnablePlayer);
    }

	public bool DisablePlayerComponent(PlayerComponent component)
	{
		bool wasDisabled = false;

		switch (component)
		{
			case PlayerComponent.CameraRotationComp:
				cameraRotation.enabled = false;
				wasDisabled = true;
				break;			
			case PlayerComponent.DrunkCameraComp:
				drunkCamera.enabled = false;
				wasDisabled = true;
				break;
			case PlayerComponent.GunComp:
				weaponHolder.EquippedGun.enabled = false;
                wasDisabled = true;
				onGunDisable.Invoke();
                break;
			case PlayerComponent.WeaponHolderComp:
				weaponHolder.enabled = false;
                wasDisabled = true;
                break;
			case PlayerComponent.AnimatorComp:
				playerAnimator.enabled = false;
                wasDisabled = true;
                break;
		}

		return wasDisabled;
	}

	public bool EnablePlayerComponent(PlayerComponent component)
	{
        bool wasEnabled = false;

        switch (component)
        {
            case PlayerComponent.CameraRotationComp:
                cameraRotation.enabled = true;
                wasEnabled = true;
                break;
            case PlayerComponent.DrunkCameraComp:
                drunkCamera.enabled = true;
                wasEnabled = true;
                break;
            case PlayerComponent.GunComp:
                weaponHolder.EquippedGun.enabled = true;
                wasEnabled = true;
				onGunEnable.Invoke();
                break;
            case PlayerComponent.WeaponHolderComp:
                weaponHolder.enabled = true;
                wasEnabled = true;
                break;
            case PlayerComponent.AnimatorComp:
                playerAnimator.enabled = true;
                wasEnabled = true;
                break;
        }

        return wasEnabled;
	}

	public void StopDeadEyeEffect()
	{
		Consumable consumable = weaponHolder.EquippedConsumable;

		if (consumable && consumable.IsInUse)
		{
			consumable.CancelUse();
			if (consumable.GetName() == "Snake Oil")
			{
				SnakeOil snakeOil = (SnakeOil)consumable;
				if (snakeOil.IsApplyingEffect)
					snakeOil.StopEffect();
			}
		}
	}

	public static PlayerManager Instance
	{
		get 
		{
			if (!instance)
			{
				instance = FindObjectOfType<PlayerManager>();
				
				if (!instance)
				{
					GameObject gameObj = new GameObject("Player Manager");
					instance = gameObj.AddComponent<PlayerManager>();
				}
			}
			
			return instance;
		}
	}

	public void AddGun(Gun gun)
	{
		activeGuns.Add(gun);
		switch (gun.TypeOfGun)
		{
			case Gun.GunType.Rifle:
				PlayerPrefs.SetInt("Has Winchester", 1);
				break;
			default:
				break;
		}
	}

	public void AddConsumableStock(string consumableName, int amount)
	{
		foreach (Consumable consumable in activeConsumables)
			if (consumable.GetName() == consumableName)
			{
				consumable.IncreaseAmount(amount);

				int amountToSave = consumable.GetAmount();

				switch (consumableName)
				{
					case "Snake Oil":
						PlayerPrefs.SetInt("Snake Oil Amount", amountToSave);
						break;
					case "Bait":
						PlayerPrefs.SetInt("Bait Amount", amountToSave);
						break;
				}
			}
	}

	public bool HasGun(Gun gun)
	{
		return activeGuns.Contains(gun);
	}

	public bool HasGunOfType(Gun.GunType type)
	{
		bool hasGunOfType = false;

		foreach (Gun gun in activeGuns)
			if (gun.TypeOfGun == type)
			{
				hasGunOfType = true;
				break;
			}

		return hasGunOfType;
	}

	public bool HasItem(IItem it)
	{
        foreach (IItem item in activeGuns)
            if (item.GetName() == it.GetName())
                return true;

        foreach (IItem item in activeConsumables)
            if (item.GetName() == it.GetName())
                return true;

		return false;
	}

	public int GetItemAmount(IItem it)
	{
		foreach (IItem item in activeGuns)
			if (item.GetName() == it.GetName())
				return item.GetAmount();
		
		foreach (IItem item in activeConsumables)
			if (item.GetName() == it.GetName())
				return item.GetAmount();
		
		return 0;
	}

	public int GetItemAmount(string itemName)
	{
		foreach (IItem item in activeGuns)
			if (item.GetName() == itemName)
				return item.GetAmount();

		foreach (IItem item in activeConsumables)
			if (item.GetName() == itemName)
				return item.GetAmount();
		
		return 0;
	}

	public int GetItemMaxAmount(string itemName)
	{
        foreach (IItem item in activeGuns)
            if (item.GetName() == itemName)
                return item.GetMaxAmount();

        foreach (IItem item in activeConsumables)
            if (item.GetName() == itemName)
                return item.GetMaxAmount();

        return 0;
	}

	public void DecreaseConsumableAmount(Consumable cons)
	{
		foreach (Consumable consumable in activeConsumables)
			if (consumable.GetName() == cons.GetName())
			{
				consumable.ReduceAmount();
                
				int amountToSave = consumable.GetAmount();

                switch (consumable.GetName())
                {
                    case "Snake Oil":
                        PlayerPrefs.SetInt("Snake Oil Amount", amountToSave);
                        break;
                    case "Bait":
                        PlayerPrefs.SetInt("Bait Amount", amountToSave);
                        break;
                }
			}
	}

	public int Currency
	{
		get { return currency; }
		set 
		{ 
			currency = value; 
			PlayerPrefs.SetInt("Currency", currency);
		}
	}

	public int TotalKills
	{
		get { return totalKills; }
		set { totalKills = value; }
	}

	public UnityEvent OnGunEnable
	{
		get { return onGunEnable; }
	}

	public UnityEvent OnGunDisable
	{
		get { return onGunDisable; }
	}
}