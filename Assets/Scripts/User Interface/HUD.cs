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
	const int criticalAmmoLeftPortion = 5;
	const int criticalAmmoInGunPortion = 3;
	const int criticalTimeLeft = 15;
	int criticalAmmoLeft;
	int criticalAmmoInGun;

    void Start()
    {
        weaponHolder.EquippedGun.OnShot.AddListener(ChangeAmmoDisplay);
        weaponHolder.EquippedGun.OnReload.AddListener(ChangeAmmoDisplay);
        weaponHolder.EquippedGun.OnReloadFinish.AddListener(ChangeAmmoDisplay);
        weaponHolder.EquippedGun.OnCrosshairScale.AddListener(ScaleCrosshair);
		LevelManager.Instance.OnEnemyKill.AddListener(ChangeKillsDisplay);

		criticalAmmoLeft = weaponHolder.EquippedGun.MaxAmmo / criticalAmmoLeftPortion ;
		criticalAmmoInGun = weaponHolder.EquippedGun.CylinderCapacity / criticalAmmoInGunPortion;

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
			ammoText.color = Color.red;
		else
		{
			if (bulletsInCylinder <= criticalAmmoInGun)
				ammoText.color = Color.yellow;
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
		crowsText.color = targetsKilled < requiredKills ? Color.red : Color.green;
	}

	void ChangeTimerDisplay()
	{
		int timeLeft = (int)LevelManager.Instance.TimeLeft;
		timerText.text = timeLeft.ToString() + "'";

		if (timeLeft <= criticalTimeLeft)
			timerText.color = Color.red;
	}
}
