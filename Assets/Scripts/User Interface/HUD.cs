using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour 
{
	[SerializeField] Image crosshair;
	[SerializeField] TextMeshProUGUI ammoText;
	[SerializeField] WeaponHolder weaponHolder;

	void Start()
	{
		weaponHolder.EquippedGun.OnShot.AddListener(ChangeAmmoDisplay);
		weaponHolder.EquippedGun.OnReload.AddListener(ChangeAmmoDisplay);
		weaponHolder.EquippedGun.OnReloadFinish.AddListener(ChangeAmmoDisplay);
		weaponHolder.EquippedGun.OnCrosshairScale.AddListener(ScaleCrosshair);
	}

	void ChangeAmmoDisplay()
	{
		string bulletsInCylinder = weaponHolder.EquippedGun.BulletsInCylinder.ToString();
		string ammoLeft = weaponHolder.EquippedGun.AmmoLeft.ToString();

		ammoText.text = bulletsInCylinder + "/" + ammoLeft;
	}

	void ScaleCrosshair()
	{
		float newScale = weaponHolder.EquippedGun.CrossshairScale;
		crosshair.transform.localScale = new Vector2(newScale, newScale);
	}
}
