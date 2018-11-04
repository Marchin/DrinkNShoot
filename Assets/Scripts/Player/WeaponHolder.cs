using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponHolder : MonoBehaviour 
{
	[SerializeField] Transform gunHolder;
	[SerializeField] Transform consumableHolder;
	[SerializeField] Gun.GunType initialGun;
	[SerializeField] UnityEvent onGunSwapStart;
	[SerializeField] UnityEvent onGunSwap;
	[SerializeField] UnityEvent onConsumableSwap;
	
	Gun equippedGun;
	Consumable equippedConsumable;
	Gun.GunType equippedGunType;
	int equippedConsumableIndex;
	bool isSwappingGun = false;

	void Awake()
	{
		equippedGunType = initialGun;
		equippedConsumableIndex = 0;
		SetEquippedGun();
		SetEquippedConsumable();
	}

	void Update()
	{
		if (CanSwapWeapon())
		{
			if (InputManager.Instance.GetSwapItemAxis() > 0f)
				StartCoroutine(SwapGun());
			if (InputManager.Instance.GetSwapItemAxis() < 0f)
				SwapConsumable();
		}
	}
	
	void SetEquippedGun()
	{
		int i = 0;
		foreach (Transform gun in gunHolder)
		{
			gun.gameObject.SetActive(i == (int)equippedGunType);
			if (i == (int)equippedGunType)
				equippedGun = gun.GetComponent<Gun>();
			i++;
		}
		onGunSwap.Invoke();
	}

	void SetEquippedConsumable()
	{
		int i = 0;
		foreach (Transform consumable in consumableHolder)
		{
			consumable.gameObject.SetActive(i == equippedConsumableIndex);
			if (i == equippedConsumableIndex)
			{
				Consumable cons = consumable.gameObject.GetComponent<Consumable>();
				if (PlayerManager.Instance.GetItemAmount(cons) > 0)
				{
					equippedConsumable = cons;
					cons.OnEmpty.AddListener(UnequipConsumable);
				}
			}
			i++;
		}
		onConsumableSwap.Invoke();
	}

	IEnumerator SwapGun()
	{
		Gun.GunType previousGunType = equippedGunType;

		if ((int)equippedGunType < gunHolder.childCount - 1)
		{
			for (int i = 1; i <= gunHolder.childCount - 1; i++)
				if (PlayerManager.Instance.HasGunOfType(equippedGunType + i))
					equippedGunType += i;
		}
		else
			equippedGunType = Gun.GunType.Handgun;

		if (equippedGunType != previousGunType)
		{
			isSwappingGun = true;
			onGunSwapStart.Invoke();
			yield return new WaitForSeconds(equippedGun.SwapGunAnimation.length);
			SetEquippedGun();
			isSwappingGun = false;
		}
	}

    void SwapConsumable()
    {
        int previousConsumableIndex = equippedConsumableIndex;

        if (equippedConsumableIndex < consumableHolder.childCount - 1)
        {
            for (int i = 1; i <= consumableHolder.childCount - 1; i++)
			{
				Consumable consumable = consumableHolder.GetChild(equippedConsumableIndex + i).GetComponent<Consumable>();
                if (consumable.GetAmount() > 0)
                    equippedConsumableIndex += i;
			}
        }
        else
        {
            for (int i = consumableHolder.childCount -1; i >= 1; i--)
            {
                Consumable consumable = consumableHolder.GetChild(equippedConsumableIndex - i).GetComponent<Consumable>();
                if (consumable.GetAmount() > 0)
                    equippedConsumableIndex -= i;
            }
		}

        if (equippedConsumableIndex != previousConsumableIndex)
            SetEquippedConsumable();
    }

	void UnequipConsumable()
	{
		equippedConsumable = null;
	}

	bool CanSwapWeapon()
	{
		return !isSwappingGun && equippedGun.CurrentState == Gun.GunState.Idle && Time.timeScale == 1f;
	}

	public Transform GunHolder
	{
		get { return gunHolder; }
	}

	public Transform ConsumableHolder
	{
		get { return consumableHolder; }
	}

	public Gun EquippedGun
	{
		get { return equippedGun; }
	}

	public Consumable EquippedConsumable
	{
		get { return equippedConsumable; }
	}

	public UnityEvent OnGunSwapStart
	{
		get { return onGunSwapStart; }
	}

	public UnityEvent OnGunSwap
	{
		get { return onGunSwap; }
	}

	public UnityEvent OnConsumableSwap
	{
		get { return onConsumableSwap; }
	}
}
