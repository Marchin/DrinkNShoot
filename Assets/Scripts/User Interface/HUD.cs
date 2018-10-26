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
	[SerializeField] GameObject objectiveBanner;
	
	[Header("Audio Sources")]
	[SerializeField] AudioSource slideInBannerSound;
	[SerializeField] AudioSource slideOutBannerSound;
	[SerializeField] AudioSource clockTickSound;
	
	[Header("Animations")]
	[SerializeField] AnimationClip slidingAnimation;
	
	[Header("References")]
	[SerializeField] WeaponHolder weaponHolder;
	
	[Header("Other Properties")]
	[SerializeField] float objectiveBannerDuration = 3.0f;
	
	const int CRITICAL_AMMO_IN_GUN = 1;
	const int CRITICAL_TIME_LEFT = 10;
	const int SECOND = 1;

    TextMeshProUGUI ammoText;
    TextMeshProUGUI crowsText;
    TextMeshProUGUI timerText;
    TextMeshProUGUI currencyText;
	Animator objectiveBannerAnimator;
	Animator ammoHUDAnimator;
	Animator crowHUDAnimator;
	Animator timerHUDAnimator;
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

		ammoHUDAnimator = ammoHUD.GetComponent<Animator>();
		crowHUDAnimator = crowHUD.GetComponent<Animator>();
		timerHUDAnimator = timerHUD.GetComponent<Animator>();
		objectiveBannerAnimator = objectiveBanner.GetComponent<Animator>();
	}

    void Start()
    {
        weaponHolder.EquippedGun.OnShot.AddListener(ChangeAmmoDisplay);
        weaponHolder.EquippedGun.OnIncreaseBulletCount.AddListener(ChangeAmmoDisplay);
        weaponHolder.EquippedGun.OnEmptyGun.AddListener(PopAmmoHUD);
        weaponHolder.EquippedGun.OnCrosshairScale.AddListener(ScaleCrosshair);
        weaponHolder.EquippedGun.OnCrosshairColorChange.AddListener(ChangeCrosshairColor);
        weaponHolder.EquippedGun.OnCrosshairMove.AddListener(MoveCrosshair);
		
		LevelManager.Instance.OnEnemyKill.AddListener(ChangeKillsDisplay);
		LevelManager.Instance.OnStartNextStage.AddListener(ChangeKillsDisplay);
		PlayerManager.Instance.OnGunDisabled.AddListener(ToggleCrosshair);

		objectiveBannerTimer = 0f;
		objectiveBannerWasJustDisabled = false;
		clockTickTimer = 0f;

		darkGreen = new Color(0f, 0.3f, 0f);
		darkRed = new Color(0.5f, 0.1f, 0.1f);

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
        int bulletsInCylinder = weaponHolder.EquippedGun.BulletsInCylinder;
        int cylinderCapacity = weaponHolder.EquippedGun.CylinderCapacity;

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

	void ChangeKillsDisplay()
	{
		int targetsKilled =  LevelManager.Instance.TargetsKilledInStage;
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
				crowHUDAnimator.SetTrigger("Has to Pop");
				objectiveBanner.SetActive(true);
				objectiveBannerAnimator.SetTrigger("Start");
				slideInBannerSound.Play();
			}
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

	void ComputeObjectiveBannerDisplay()
	{
		if (objectiveBanner.activeInHierarchy && !objectiveBannerWasJustDisabled)
		{
			if (objectiveBannerTimer >= objectiveBannerDuration)
			{
				objectiveBannerWasJustDisabled = true;
				objectiveBannerTimer = 0f;
				objectiveBannerAnimator.SetTrigger("Exit");
				slideOutBannerSound.Play();
				Invoke("DisableBanner", slidingAnimation.length);
			}
			else
				objectiveBannerTimer += Time.deltaTime;
		}
	}

	void PopAmmoHUD()
	{
		ammoHUDAnimator.SetTrigger("Has to Pop");
	}

	void DisableBanner()
	{
		objectiveBannerWasJustDisabled = false;
		objectiveBanner.SetActive(false);
	}

	public void ChangeCurrencyDisplay(int currency)
	{
		currencyText.text = currency.ToString();
	}
}
