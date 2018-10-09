using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour 
{
	WeaponHolder weaponHolder;

	void Awake()
	{
		weaponHolder = GetComponentInChildren<WeaponHolder>();
	}

	void Start()
	{
		weaponHolder.EquippedGun.OnShot.AddListener(PlayShootSound);
		weaponHolder.EquippedGun.OnReload.AddListener(InvokeReloadSound);
		weaponHolder.EquippedGun.OnEmptyGun.AddListener(PlayEmptyGunSound);
		weaponHolder.EquippedGun.OnReloadCancel.AddListener(CancelInvokeReloadSound);
	}

	void PlayShootSound()
	{
		weaponHolder.EquippedGun.ShootSound.Play();
	}
	
	void InvokeReloadSound()
	{
		Invoke("PlayReloadSound", weaponHolder.EquippedGun.ReloadAnimation.length * 0.75f);
	}

	void CancelInvokeReloadSound()
	{
		CancelInvoke("PlayReloadSound");
	}

	void PlayReloadSound()
	{
		weaponHolder.EquippedGun.ReloadSound.Play();
	}

	void PlayEmptyGunSound()
	{
		weaponHolder.EquippedGun.EmptyGunSound.Play();
	}
}
