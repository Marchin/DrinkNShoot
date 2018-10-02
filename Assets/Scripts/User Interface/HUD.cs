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
	[Header("References")]
	[SerializeField] WeaponHolder weaponHolder;
	const int CRITICAL_AMMO_LEFT_FRACT = 5;
	const int CRITICAL_AMMO_IN_GUN_FRAC = 3;
	const int WARNING_TIME_LEFT = 20;
	const int CRITICAL_TIME_LEFT = 10;
	int criticalAmmoLeft;
	int criticalAmmoInGun;
	Color darkGreen;
	Color darkRed;
	Color yellow;

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

		darkGreen = new Color(0.1f, 0.5f, 0.1f);
		darkRed = new Color(0.5f, 0.1f, 0.1f);
		yellow = new Color(0.8f, 0.6f, 0.1f);

		ChangeAmmoDisplay();
		ChangeKillsDisplay();
    }

	void Update()
	{
		ChangeTimerDisplay();
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

        ammoText.text = bulletsInCylinder + "/" + ammoLeft;
    }

	void ChangeKillsDisplay()
	{
		int targetsKilled =  LevelManager.Instance.TargetsKilled;
		int requiredKills = LevelManager.Instance.RequiredKills;

		crowsText.text = targetsKilled + "/" + requiredKills;
		crowsText.color = targetsKilled < requiredKills ? darkRed : darkGreen;
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
}
