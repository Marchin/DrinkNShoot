﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : MonoBehaviour
{
	static PlayerManager instance;
	CameraRotation cameraRotation;
	DrunkCamera drunkCamera;
	Gun equippedGun;
	Animator playerAnimator; 
	uint m_crowCurrency;

	void Awake() 
	{
		if (Instance == this)
			DontDestroyOnLoad(gameObject);
	}

	void Start()
	{
		if (Instance == this)
			SetLevelReferences();
		else
		{
			Instance.SetLevelReferences();
			Destroy(gameObject);
		}
	}

	void SetLevelReferences()
	{
        cameraRotation = FindObjectOfType<CameraRotation>();
        drunkCamera = FindObjectOfType<DrunkCamera>();
        equippedGun = FindObjectOfType<WeaponHolder>().EquippedGun;
        playerAnimator = cameraRotation.gameObject.GetComponentInChildren<Animator>();
		FindObjectOfType<PauseMenu>().OnPauseToggle.AddListener(TogglePlayerAvailability);
        LevelManager.Instance.OnGameOver.AddListener(TogglePlayerAvailability);
	}

	void TogglePlayerAvailability()
	{
		cameraRotation.enabled = !cameraRotation.enabled;
		drunkCamera.enabled = !drunkCamera.enabled;
		equippedGun.enabled = !equippedGun.enabled;
		playerAnimator.enabled = !playerAnimator.enabled;
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

	public uint CrowCurrency
	{
		get { return m_crowCurrency; }
		set { m_crowCurrency = value; }
	}
}
