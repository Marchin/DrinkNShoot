using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponHolder : MonoBehaviour 
{
	[SerializeField] Gun.GunType initialGun;
	[SerializeField] UnityEvent onWeaponSwapStart;
	[SerializeField] UnityEvent onWeaponSwap;
	
	Gun equippedGun;
	Gun.GunType equippedGunType;
	bool isSwappingWeapon;


	void Awake()
	{
		equippedGunType = initialGun;
		SetEquippedGun();
	}

	void Update()
	{
		if (CanSwapWeapon())
		{
			if (InputManager.Instance.GetSwapWeaponAxis() > 0f)
				StartCoroutine(SwapWeapon());
		}
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
		onWeaponSwap.Invoke();
	}

	IEnumerator SwapWeapon()
	{
		Gun.GunType previousGunType = equippedGun.TypeOfGun;

		if ((int)equippedGunType < transform.childCount - 1)
		{
			for (int i = 1; i <= transform.childCount - 1; i++)
				if (PlayerManager.Instance.HasGunOfType(equippedGunType + i))
					equippedGunType += i;
		}
		else
			equippedGunType = Gun.GunType.Handgun;

		if (equippedGunType != previousGunType)
		{
			isSwappingWeapon = true;
			onWeaponSwapStart.Invoke();
			yield return new WaitForSeconds(equippedGun.SwapWeaponAnimation.length);
			SetEquippedGun();
			isSwappingWeapon = false;
		}
	}

	bool CanSwapWeapon()
	{
		return !isSwappingWeapon && equippedGun.CurrentState == Gun.GunState.Idle;
	}

	public void SetEquippedGunType(Gun.GunType gunType)
	{
		equippedGunType = gunType;
		SetEquippedGun();
	}

	public Gun EquippedGun
	{
		get { return equippedGun; }
	}

	public UnityEvent OnWeaponSwapStart
	{
		get { return onWeaponSwapStart; }
	}

	public UnityEvent OnWeaponSwap
	{
		get { return onWeaponSwap; }
	}
}
