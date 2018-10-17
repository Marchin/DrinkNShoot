using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour 
{
	[SerializeField] AudioSource drinkingSound;
	[SerializeField] AudioSource burpingSound;
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

	// Animation Events Methods
	public void PlayDrinkingSound()
	{
		drinkingSound.Play();
	}

	public void PlayBurpingSound()
	{
		burpingSound.Play();
	}
}
