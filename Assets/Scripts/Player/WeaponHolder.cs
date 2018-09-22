using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : MonoBehaviour 
{
	[SerializeField] Gun.GunType initialGun;
	Gun equippedGun;
	Gun.GunType equippedGunType;

	void Awake()
	{
		equippedGunType = initialGun;
		SetEquippedGun();
	}
	
	void SetEquippedGun()
	{
		int i = 0;
		foreach (Transform gun in transform)
		{
			gun.gameObject.SetActive(i == (int)equippedGunType);
			if (i == (int)equippedGunType)
				equippedGun = gun.GetComponent<Gun>();
			i++;
		}
	}

	public Gun EquippedGun
	{
		get { return equippedGun; }
	}
}
