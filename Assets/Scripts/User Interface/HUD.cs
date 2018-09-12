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
		// EnemyManager.Instance.OnCrowKill.AddListener(ChangeCrowsDisplay);

		criticalAmmoLeft = weaponHolder.EquippedGun.MaxAmmo / criticalAmmoLeftPortion ;
		criticalAmmoInGun = weaponHolder.EquippedGun.CylinderCapacity / criticalAmmoInGunPortion;
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
        string bulletsInCylinder = weaponHolder.EquippedGun.BulletsInCylinder.ToString();
        string ammoLeft = weaponHolder.EquippedGun.AmmoLeft.ToString();

		if (weaponHolder.EquippedGun.AmmoLeft <= criticalAmmoLeft)
			ammoText.color = Color.red;
		else
		{
			if (weaponHolder.EquippedGun.BulletsInCylinder <= criticalAmmoInGun)
				ammoText.color = Color.yellow;
			else
				ammoText.color = Color.white;
		}

        ammoText.text = bulletsInCylinder + "/" + ammoLeft;
    }

	void ChangeCrowsDisplay()
	{
		string crowsKilled = LevelManager.Instance.CrowsKilled.ToString();
		string targetKills =  LevelManager.Instance.TargetKills.ToString();

		crowsText.text = crowsKilled + "/" + targetKills;
	}

	void ChangeTimerDisplay()
	{
		timerText.text = ((int)LevelManager.Instance.TimeLeft).ToString() + "'";

		if (LevelManager.Instance.TimeLeft <= criticalTimeLeft)
			timerText.color = Color.red;
	}
}
