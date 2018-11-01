﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : MonoBehaviour
{
	public enum PlayerComponent
	{
		CameraRotationComp, DrunkCameraComp, GunComp, WeaponHolderComp, AnimatorComp
	}

	public enum Item
	{
		Revolver, Winchester, SnakeOil, Bait
	}

	[SerializeField] UnityEvent onGunAvailabilityToggle;

	static PlayerManager instance;
	
	CameraRotation cameraRotation;
	DrunkCamera drunkCamera;
	WeaponHolder weaponHolder;
	Animator playerAnimator;
	List<Item> equippedItems;
	int currency;
	int totalKills;

	void Awake() 
	{
		if (Instance == this)
			DontDestroyOnLoad(gameObject);
	}

	void Start()
	{
		if (Instance == this)
		{
			SetLevelReferences();
			equippedItems.Add(Item.Revolver);
		}
		else
			Destroy(gameObject);
	}

	void SetLevelReferences()
	{
        cameraRotation = FindObjectOfType<CameraRotation>();
        drunkCamera = FindObjectOfType<DrunkCamera>();
        weaponHolder = FindObjectOfType<WeaponHolder>();
        playerAnimator = cameraRotation.gameObject.GetComponentInChildren<Animator>();
		
		FindObjectOfType<PauseMenu>().OnPauseToggle.AddListener(TogglePlayerAvailability);
		FindObjectOfType<EndLevelMenu>().OnContinue.AddListener(TogglePlayerAvailability);
        LevelManager.Instance.OnGameOver.AddListener(TogglePlayerAvailability);
	}

	void TogglePlayerAvailability()
	{
		cameraRotation.enabled = !cameraRotation.enabled;
		drunkCamera.enabled = !drunkCamera.enabled;
		weaponHolder.enabled = !weaponHolder.enabled;
		weaponHolder.EquippedGun.enabled = !weaponHolder.EquippedGun.enabled;
		playerAnimator.enabled = !playerAnimator.enabled;

		onGunAvailabilityToggle.Invoke();
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
				onGunAvailabilityToggle.Invoke();
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
				onGunAvailabilityToggle.Invoke();
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

	public void AddItem(Item item)
	{
		equippedItems.Add(item);
	}

	public bool HasItem(Item item)
	{
		return equippedItems.Contains(item);
	}

	public int Currency
	{
		get { return currency; }
		set { currency = value; }
	}

	public int TotalKills
	{
		get { return totalKills; }
		set { totalKills = value; }
	}

	public UnityEvent OnGunDisabled
	{
		get { return onGunAvailabilityToggle; }
	}
}
