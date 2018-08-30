using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour 
{
	[SerializeField] Gun currentGun;

	void Start()
	{
		currentGun.OnShot.AddListener(PlayShootSound);
		currentGun.OnReload.AddListener(PlayReloadSound);
		currentGun.OnEmptyGun.AddListener(PlayEmptyGunSound);
	}

	void PlayShootSound()
	{
		currentGun.ShootSound.Play();
	}

	void PlayReloadSound()
	{
		currentGun.ReloadSound.Play();
	}

	void PlayEmptyGunSound()
	{
		currentGun.EmptyGunSound.Play();
	}
}
