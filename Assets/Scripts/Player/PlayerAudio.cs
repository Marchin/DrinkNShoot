using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerAudio : MonoBehaviour 
{
	[SerializeField] AudioSource drinkingSound;
	[SerializeField] AudioSource burpingSound;
	[SerializeField] AudioSource deadEyeEnterSound;
	[SerializeField] AudioSource deadEyeExitSound;
	
	WeaponHolder weaponHolder;

	void Awake()
	{
		weaponHolder = GetComponentInChildren<WeaponHolder>();
	}

	void Start()
	{
		ChangeGunSounds();
		ChangeConsumableSounds();
		weaponHolder.OnGunSwap.AddListener(ChangeGunSounds);
		weaponHolder.OnConsumableSwap.AddListener(ChangeConsumableSounds);
	}

	void PlayShootSound()
	{
		weaponHolder.EquippedGun.ShootSound.Play();
	}
	
	void InvokeReloadSound()
	{
		Invoke("PlayReloadSound", weaponHolder.EquippedGun.ReloadAnimation.length * 0.75f * Time.timeScale);
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

	void PlayUseItemSound()
	{
		weaponHolder.EquippedConsumable.UseSound.Play();
	}

	void PlayDeadEyeExitSound()
	{
		deadEyeExitSound.Play();
	}

	void ChangeGunSounds()
	{
        weaponHolder.EquippedGun.OnShot.AddListener(PlayShootSound);
        weaponHolder.EquippedGun.OnReload.AddListener(InvokeReloadSound);
        weaponHolder.EquippedGun.OnEmptyGun.AddListener(PlayEmptyGunSound);
        weaponHolder.EquippedGun.OnReloadCancel.AddListener(CancelInvokeReloadSound);
	}

	void ChangeConsumableSounds()
	{
		if (weaponHolder.EquippedConsumable)
		{
			weaponHolder.EquippedConsumable.OnUse.AddListener(PlayUseItemSound);
			if (weaponHolder.EquippedConsumable.GetName() == "Snake Oil")
			{
				SnakeOil snakeOil = (SnakeOil)weaponHolder.EquippedConsumable;
				snakeOil.OnBackToNormalTime.AddListener(PlayDeadEyeExitSound);
			}
		}
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

	public void PlayDeadEyeEnterSound()
	{
		deadEyeEnterSound.Play();
	}
}