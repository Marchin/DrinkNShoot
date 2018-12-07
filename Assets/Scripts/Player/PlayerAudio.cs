using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerAudio : MonoBehaviour 
{
	[SerializeField] AudioSource drinkingSound;
	[SerializeField] AudioSource burpingSound;
	[SerializeField] AudioSource deadEyeEnterSound;
	[SerializeField] AudioSource deadEyeExitSound;
	[SerializeField] AudioSource cowboyYellSound;
	[SerializeField] AudioSource bePoopedShoutSound;
	[SerializeField] PoopImage poopImage;
	[SerializeField] [Range(0f, 100f)]
	float yellProbability = 15f;
	[SerializeField] [Range(0f, 100f)]
	float bePoopedShoutProbability = 15f;
	
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
		poopImage.OnPoopAppear.AddListener(PlayBePoopedShoutSound);
	}

	void PlayShootSound()
	{
		weaponHolder.EquippedGun.ShootSound.Play();
	}

	void PlayReloadStartSound()
	{
		weaponHolder.EquippedGun.ReloadStartSound.Play();
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

	void PlayCowboyYellSound()
	{
		if (Random.Range(0f, 100f) < yellProbability)
			cowboyYellSound.Play();
	}

	void PlayBePoopedShoutSound()
	{
		if (Random.Range(0f, 100f) < bePoopedShoutProbability)
			bePoopedShoutSound.Play();
	}

	void ChangeGunSounds()
	{
        weaponHolder.EquippedGun.OnShot.AddListener(PlayShootSound);
        weaponHolder.EquippedGun.OnReloadStart.AddListener(InvokeReloadSound);
        weaponHolder.EquippedGun.OnReload.AddListener(InvokeReloadSound);
        weaponHolder.EquippedGun.OnEmptyGun.AddListener(PlayEmptyGunSound);
        weaponHolder.EquippedGun.OnReloadCancel.AddListener(CancelInvokeReloadSound);
        weaponHolder.EquippedGun.OnShotTarget.AddListener(PlayCowboyYellSound);
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