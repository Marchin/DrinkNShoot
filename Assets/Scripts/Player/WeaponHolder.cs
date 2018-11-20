using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class WeaponHolder : MonoBehaviour 
{
	[SerializeField] Transform gunHolder;
	[SerializeField] Transform consumableHolder;
	[SerializeField] Gun.GunType initialGun;
	
	Gun equippedGun;
	Consumable equippedConsumable;
	Gun.GunType equippedGunType;
	DrunkCrosshair currentCrosshair;
	int equippedConsumableIndex;
	bool isSwappingGun = false;

    UnityEvent onGunSwapStart = new UnityEvent();
    UnityEvent onGunSwap = new UnityEvent();
    UnityEvent onConsumableSwap = new UnityEvent();

	void Awake()
	{
		equippedGunType = initialGun;
		equippedConsumableIndex = 0;
	}

	void OnEnable()
	{
		SetEquippedGun();
	}
	
	void Start()
	{
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
			{
				equippedGun = gun.GetComponent<Gun>();
				currentCrosshair = gun.GetComponent<DrunkCrosshair>();
			}
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
		int totalGuns = gunHolder.childCount;

		for (int i = 1; i < totalGuns; i++)
			if (PlayerManager.Instance.HasGunOfType((Gun.GunType)(((int)equippedGunType + i) % totalGuns)))
				equippedGunType = (Gun.GunType)((int)(equippedGunType + i) % totalGuns);

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
		int totalConsumables = consumableHolder.childCount;

		for (int i = 1; i < totalConsumables; i++)
		{
			Consumable consumable = consumableHolder.GetChild((equippedConsumableIndex + i) % totalConsumables).GetComponent<Consumable>();
			if (PlayerManager.Instance.GetItemAmount(consumable) > 0)
				equippedConsumableIndex = (equippedConsumableIndex + i) % totalConsumables;
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

	public DrunkCrosshair CurrentCrosshair
	{
		get { return currentCrosshair; }
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