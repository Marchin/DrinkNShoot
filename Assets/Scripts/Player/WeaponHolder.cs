using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponHolder : MonoBehaviour 
{
	enum ScrollWheelDir
	{
		Up, Down
	}

	[SerializeField] Gun.GunType initialGun;
	[SerializeField] UnityEvent onWeaponSwapStart;
	[SerializeField] UnityEvent onWeaponSwap;
	
	Gun equippedGun;
	Gun.GunType equippedGunType;

	void Awake()
	{
		equippedGunType = initialGun;
		SetEquippedGun();
	}

	void Update()
	{
		if (InputManager.Instance.GetSwapWeaponAxis() > 0f)
		{
			StartCoroutine(SwapWeapon(ScrollWheelDir.Up));
			onWeaponSwapStart.Invoke();
		}
		if (InputManager.Instance.GetSwapWeaponAxis() < 0f)
		{
			StartCoroutine(SwapWeapon(ScrollWheelDir.Down));
			onWeaponSwapStart.Invoke();
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

	IEnumerator SwapWeapon(ScrollWheelDir direction)
	{
		yield return new WaitForSeconds(0.2f);

		Gun.GunType previousGunType = equippedGun.TypeOfGun;

		if (direction == ScrollWheelDir.Up)
		{
			if ((int)equippedGunType < transform.childCount - 1)
				equippedGunType++;
			else
				equippedGunType = Gun.GunType.Rifle;
		}
		else
		{
			if ((int)equippedGunType > 0)
				equippedGunType--;
			else
				equippedGunType = Gun.GunType.Handgun;
		}

		if (equippedGunType != previousGunType)
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
