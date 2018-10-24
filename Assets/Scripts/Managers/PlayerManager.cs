using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : MonoBehaviour
{
	public enum PlayerComponent
	{
		CameraRotationComp, DrunkCameraComp, GunComp, AnimatorComp
	}

	[SerializeField] UnityEvent onGunAvailabilityToggle;

	static PlayerManager instance;
	
	CameraRotation cameraRotation;
	DrunkCamera drunkCamera;
	Gun equippedGun;
	Animator playerAnimator; 
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
		FindObjectOfType<EndLevelMenu>().OnContinue.AddListener(TogglePlayerAvailability);
        LevelManager.Instance.OnGameOver.AddListener(TogglePlayerAvailability);
	}

	void TogglePlayerAvailability()
	{
		cameraRotation.enabled = !cameraRotation.enabled;
		drunkCamera.enabled = !drunkCamera.enabled;
		equippedGun.enabled = !equippedGun.enabled;
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
				equippedGun.enabled = false;
                wasDisabled = true;
				onGunAvailabilityToggle.Invoke();
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
                equippedGun.enabled = true;
                wasEnabled = true;
				onGunAvailabilityToggle.Invoke();
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
