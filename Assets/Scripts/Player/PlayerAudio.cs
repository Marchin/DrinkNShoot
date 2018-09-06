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
		weaponHolder.EquippedGun.OnReload.AddListener(PlayReloadSound);
		weaponHolder.EquippedGun.OnEmptyGun.AddListener(PlayEmptyGunSound);
	}

	void PlayShootSound()
	{
		weaponHolder.EquippedGun.ShootSound.Play();
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
