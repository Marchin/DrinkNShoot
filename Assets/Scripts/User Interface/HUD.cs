using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour 
{
	[Header("HUD Elements")]
	[SerializeField] Image crosshair;
	[SerializeField] TextMeshProUGUI ammoText;
	[SerializeField] TextMeshProUGUI crowsText;
	[SerializeField] TextMeshProUGUI timerText;
	[SerializeField] TextMeshProUGUI currencyText;
	[SerializeField] GameObject objectiveBanner;
	[Header("Audio Sources")]
	[SerializeField] AudioSource slidingBannerSound;
	[Header("Animations")]
	[SerializeField] AnimationClip slidingAnimation;
	[Header("References")]
	[SerializeField] WeaponHolder weaponHolder;
	const int CRITICAL_AMMO_LEFT_FRACT = 5;
	const int CRITICAL_AMMO_IN_GUN_FRAC = 3;
	const int WARNING_TIME_LEFT = 20;
	const int CRITICAL_TIME_LEFT = 10;
	const float OBJECTIVE_BANNER_DURATION = 1.5f;
	int criticalAmmoLeft;
	int criticalAmmoInGun;
	float objectiveBannerTimer;
	bool objectiveBannerWasDisabled;
	Animator objectiveBannerAnimator;
	Color darkGreen;
	Color darkRed;
	Color yellow;

	void Awake()
	{
		objectiveBannerAnimator = objectiveBanner.GetComponent<Animator>();
	}

    void Start()
    {
        weaponHolder.EquippedGun.OnShot.AddListener(ChangeAmmoDisplay);
        weaponHolder.EquippedGun.OnReload.AddListener(ChangeAmmoDisplay);
        weaponHolder.EquippedGun.OnReloadFinish.AddListener(ChangeAmmoDisplay);
        weaponHolder.EquippedGun.OnCrosshairScale.AddListener(ScaleCrosshair);
		LevelManager.Instance.OnEnemyKill.AddListener(ChangeKillsDisplay);
		LevelManager.Instance.OnStartNextStage.AddListener(ChangeKillsDisplay);

		criticalAmmoLeft = weaponHolder.EquippedGun.MaxAmmo / CRITICAL_AMMO_LEFT_FRACT ;
		criticalAmmoInGun = weaponHolder.EquippedGun.CylinderCapacity / CRITICAL_AMMO_IN_GUN_FRAC;

		objectiveBannerTimer = 0f;

		darkGreen = new Color(0.1f, 0.5f, 0.1f);
		darkRed = new Color(0.5f, 0.1f, 0.1f);
		yellow = new Color(0.8f, 0.6f, 0.1f);

		ChangeAmmoDisplay();
		ChangeKillsDisplay();
    }

	void Update()
	{
		ChangeTimerDisplay();
		ComputeObjectiveBannerDisplay();
	}

    void ScaleCrosshair()
    {
        float newScale = weaponHolder.EquippedGun.CrossshairScale;

        crosshair.transform.localScale = new Vector2(newScale, newScale);
    }

    void ChangeAmmoDisplay()
    {
        int bulletsInCylinder = weaponHolder.EquippedGun.BulletsInCylinder;
        int ammoLeft = weaponHolder.EquippedGun.AmmoLeft;

		if (ammoLeft <= criticalAmmoLeft)
			ammoText.color = darkRed;
		else
		{
			if (bulletsInCylinder <= criticalAmmoInGun)
				ammoText.color = yellow;
			else
				ammoText.color = Color.white;
		}

        ammoText.text = bulletsInCylinder.ToString() + "/" + ammoLeft.ToString();
    }

	void ChangeKillsDisplay()
	{
		int targetsKilled =  LevelManager.Instance.TargetsKilled;
		int requiredKills = LevelManager.Instance.RequiredKills;

		if (targetsKilled < requiredKills)
		{
			crowsText.text = targetsKilled.ToString() + "/" + requiredKills.ToString();
			crowsText.color = Color.white;
		}
		else
		{
			crowsText.text = targetsKilled.ToString();
			crowsText.color = darkGreen;
			if (targetsKilled == requiredKills)
			{
				objectiveBanner.SetActive(true);
				objectiveBannerAnimator.SetTrigger("Start");
				slidingBannerSound.Play();
			}
		}
	}

	void ChangeTimerDisplay()
	{
		int timeLeft = (int)LevelManager.Instance.TimeLeft;
		timerText.text = timeLeft.ToString() + "\"";

		if (timeLeft <= CRITICAL_TIME_LEFT)
			timerText.color = darkRed;
		else
		{
			if (timeLeft <= WARNING_TIME_LEFT)
				timerText.color = yellow;
			else
				timerText.color = Color.white;
		}
	}

	void ComputeObjectiveBannerDisplay()
	{
		if (objectiveBannerWasDisabled)
		{
			if (objectiveBannerTimer >= OBJECTIVE_BANNER_DURATION)
			{
				objectiveBannerWasDisabled = true;
				objectiveBannerTimer = 0f;
				objectiveBannerAnimator.SetTrigger("Exit");
				slidingBannerSound.Play();
				Invoke("DisableBanner", slidingAnimation.length);
			}
			else
				objectiveBannerTimer += Time.deltaTime;
		}
	}

	void DisableBanner()
	{
		objectiveBanner.SetActive(false);
	}

	public void ChangeCurrencyDisplay(int currency)
	{
		currencyText.text = "$" + currency.ToString();
	}
}
