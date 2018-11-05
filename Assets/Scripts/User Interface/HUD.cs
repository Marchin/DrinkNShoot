using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class HUD : MonoBehaviour 
{
	[Header("HUD Elements")]
	[SerializeField] Image crosshair;
	[SerializeField] GameObject ammoHUD;
	[SerializeField] GameObject crowHUD;
	[SerializeField] GameObject timerHUD;
	[SerializeField] GameObject currencyHUD;
	[SerializeField] GameObject consumablesHUD;
	[SerializeField] GameObject rankBanner;
	
	[Header("Audio Sources")]
	[SerializeField] AudioSource slideInBannerSound;
	[SerializeField] AudioSource slideOutBannerSound;
	[SerializeField] AudioSource clockTickSound;
	
	[Header("Animations")]
	[SerializeField] AnimationClip slidingAnimation;
	
	[Header("References")]
	[SerializeField] WeaponHolder weaponHolder;
	
	[Header("Other Properties")]
	[SerializeField] float rankBannerDuration = 3f;
	
	const int CRITICAL_AMMO_IN_GUN = 1;
	const int CRITICAL_TIME_LEFT = 10;
	const int SECOND = 1;

    TextMeshProUGUI ammoText;
    TextMeshProUGUI crowsText;
    TextMeshProUGUI timerText;
    TextMeshProUGUI currencyText;
    TextMeshProUGUI consumablesText;
	TextMeshProUGUI rankBannerText;
	Animator rankBannerAnimator;
	Animator ammoHUDAnimator;
	Animator crowHUDAnimator;
	Animator timerHUDAnimator;
	Animator consumablesHUDAnimator;
	Color darkGreen;
	Color darkRed;
	Color yellow;
	float objectiveBannerTimer;
	float clockTickTimer;
	bool objectiveBannerWasJustDisabled;

	void Awake()
	{
		ammoText = ammoHUD.GetComponentInChildren<TextMeshProUGUI>();
		crowsText = crowHUD.GetComponentInChildren<TextMeshProUGUI>();
		timerText = timerHUD.GetComponentInChildren<TextMeshProUGUI>();
		currencyText = currencyHUD.GetComponentInChildren<TextMeshProUGUI>();
		consumablesText = consumablesHUD.GetComponentInChildren<TextMeshProUGUI>();
		rankBannerText = rankBanner.GetComponentInChildren<TextMeshProUGUI>();

		ammoHUDAnimator = ammoHUD.GetComponent<Animator>();
		crowHUDAnimator = crowHUD.GetComponent<Animator>();
		timerHUDAnimator = timerHUD.GetComponent<Animator>();
		consumablesHUDAnimator = consumablesHUD.GetComponent<Animator>();
		rankBannerAnimator = rankBanner.GetComponent<Animator>();
	}

    void Start()
    {
		ChangeWeaponInDisplay();
		ChangeConsumableInDisplay();
		
		weaponHolder.OnGunSwap.AddListener(ChangeWeaponInDisplay);
		weaponHolder.OnConsumableSwap.AddListener(ChangeConsumableInDisplay);
		LevelManager.Instance.OnEnemyKill.AddListener(ChangeKillsDisplay);
		LevelManager.Instance.OnStartNextStage.AddListener(ChangeKillsDisplay);
		PlayerManager.Instance.OnGunEnable.AddListener(ToggleCrosshair);
		PlayerManager.Instance.OnGunDisable.AddListener(ToggleCrosshair);

		objectiveBannerTimer = 0f;
		objectiveBannerWasJustDisabled = false;
		clockTickTimer = 0f;

		darkGreen = new Color(0f, 0.3f, 0f);
		darkRed = new Color(0.5f, 0.1f, 0.1f);

		ChangeAmmoDisplay();
		if (weaponHolder.EquippedConsumable)
			ChangeConsumablesDisplay();
		ChangeKillsDisplay();
    }

	void Update()
	{
		ChangeTimerDisplay();
		ComputeRankBannerDisplay();
	}

    void ScaleCrosshair()
    {
        float newScale = weaponHolder.EquippedGun.CrossshairScale;

        crosshair.transform.localScale = new Vector2(newScale, newScale);
    }

	void ChangeCrosshairColor()
	{
		crosshair.color = weaponHolder.EquippedGun.TargetOnClearSight ? darkRed : Color.white;
	}

	void MoveCrosshair()
	{
		crosshair.rectTransform.position = weaponHolder.EquippedGun.CrosshairPosition;
	}

	void ToggleCrosshair()
	{
		crosshair.enabled = !crosshair.enabled;
	}

    void ChangeAmmoDisplay()
    {
        int bulletsInCylinder = weaponHolder.EquippedGun.BulletsInGun;
        int cylinderCapacity = weaponHolder.EquippedGun.GunCapacity;

		if (bulletsInCylinder <= CRITICAL_AMMO_IN_GUN)
		{
			if (bulletsInCylinder == 0)
				ammoHUDAnimator.SetTrigger("Has to Pop");
			ammoText.color = darkRed;
		}
		else
			ammoText.color = Color.white;

        ammoText.text = bulletsInCylinder.ToString() + "/" + cylinderCapacity.ToString();
    }

    void ChangeConsumablesDisplay()
    {
        int amount = PlayerManager.Instance.GetItemAmount(weaponHolder.EquippedConsumable.GetName());
        int maxAmount = PlayerManager.Instance.GetItemMaxAmount(weaponHolder.EquippedConsumable.GetName());

		
		if (amount == 0)
			consumablesHUD.SetActive(false);
		else
		{
			consumablesHUDAnimator.SetTrigger("Has to Pop");
			consumablesText.color = Color.white;
		}

        consumablesText.text = amount.ToString() + "/" + maxAmount.ToString();
    }

	void ChangeKillsDisplay()
	{
		int targetsKilled = LevelManager.Instance.TargetsKilledInStage;
		int minimumRequiredKills = LevelManager.Instance.MinimumRequiredKills;
		int maximumRequiredKills = LevelManager.Instance.MaximumRequiredKills;
		int requiredKillsForNextTier = LevelManager.Instance.RequiredKillsForNextTier;

		crowsText.text = targetsKilled < maximumRequiredKills ? targetsKilled.ToString() + "/" + requiredKillsForNextTier.ToString() :
																targetsKilled.ToString();

		crowsText.color = targetsKilled >= minimumRequiredKills ? darkGreen : Color.white;
		
		if (targetsKilled + 1 == requiredKillsForNextTier + 1)
		{
			crowHUDAnimator.SetTrigger("Has to Pop");
			if (targetsKilled == minimumRequiredKills)
				rankBannerText.text = "Bronze Rank Achieved!";
			else
			{
				if (targetsKilled == maximumRequiredKills)
					rankBannerText.text = "Gold Rank Achieved!";
				else
					rankBannerText.text = "Silver Rank Achieved!";
			}
			rankBanner.SetActive(true);
			rankBannerAnimator.SetTrigger("Start");
			slideInBannerSound.Play();
		}
	}

	void ChangeTimerDisplay()
	{
		int timeLeft = (int)LevelManager.Instance.TimeLeft;
		timerText.text = timeLeft.ToString() + "\"";

		if (timeLeft <= CRITICAL_TIME_LEFT)
		{
			timerText.color = darkRed;
			clockTickTimer += Time.deltaTime;
			if (clockTickTimer >= SECOND)
			{
				timerHUDAnimator.SetTrigger("Has to Pop");
				clockTickTimer = 0f;
				clockTickSound.Play();
			}
		}
		else
			timerText.color = Color.white;
	}

	void ComputeRankBannerDisplay()
	{
		if (rankBanner.activeInHierarchy && !objectiveBannerWasJustDisabled)
		{
			if (objectiveBannerTimer >= rankBannerDuration)
			{
				objectiveBannerWasJustDisabled = true;
				objectiveBannerTimer = 0f;
				rankBannerAnimator.SetTrigger("Exit");
				slideOutBannerSound.Play();
				Invoke("DisableRankBanner", slidingAnimation.length);
			}
			else
				objectiveBannerTimer += Time.deltaTime;
		}
	}

	void PopAmmoHUD()
	{
		ammoHUDAnimator.SetTrigger("Has to Pop");
	}

	void DisableRankBanner()
	{
		objectiveBannerWasJustDisabled = false;
		rankBanner.SetActive(false);
	}

	void ChangeWeaponInDisplay()
	{
        weaponHolder.EquippedGun.OnShot.AddListener(ChangeAmmoDisplay);
        weaponHolder.EquippedGun.OnIncreaseBulletCount.AddListener(ChangeAmmoDisplay);
        weaponHolder.EquippedGun.OnEmptyGun.AddListener(PopAmmoHUD);
        weaponHolder.EquippedGun.OnCrosshairScale.AddListener(ScaleCrosshair);
        weaponHolder.EquippedGun.OnCrosshairColorChange.AddListener(ChangeCrosshairColor);
        weaponHolder.EquippedGun.OnCrosshairMove.AddListener(MoveCrosshair);

		ChangeCrosshairColor();
		ChangeAmmoDisplay();
	}

	void ChangeConsumableInDisplay()
	{
		if (weaponHolder.EquippedConsumable)
		{
			consumablesHUD.SetActive(true);
			weaponHolder.EquippedConsumable.OnUse.AddListener(ChangeConsumablesDisplay);

			ChangeConsumablesDisplay();
		}
		else
			consumablesHUD.SetActive(false);
	}

	public void ChangeCurrencyDisplay(int currency)
	{
		currencyText.text = currency.ToString();
	}

}