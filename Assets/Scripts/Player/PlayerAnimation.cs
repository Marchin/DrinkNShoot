﻿using UnityEngine;
using UnityEngine.Animations;

[RequireComponent(typeof(Animator))]
public class PlayerAnimation : MonoBehaviour
{
	[SerializeField] AnimatorOverrideController animatorOverrideController;
	[SerializeField] AnimationClip[] sippingAnimations;
	[SerializeField] Avatar[] possibleAvatars;
	
	const float SIPPING_DELAY = 0.75f;
	
	Animator animator;
	WeaponHolder weaponHolder;

	void Awake()
	{
		animator = GetComponent<Animator>();
		weaponHolder = GetComponentInChildren<WeaponHolder>();
	}

	void Start() 
	{
		ChangeGunAnimations();
		ChangeConsumableAnimations();
		
		weaponHolder.OnGunSwap.AddListener(ChangeGunAnimations);	
		weaponHolder.OnConsumableSwap.AddListener(ChangeConsumableAnimations);	
		LevelManager.Instance.OnStartNextStage.AddListener(InvokeSipTaking);
	}

	void ResetTriggers() 
	{
		foreach (AnimatorControllerParameter parameter in animator.parameters)
			if (parameter.type == AnimatorControllerParameterType.Trigger)
				animator.ResetTrigger(parameter.name);
	}

	void ShootAnimation() 
	{
		animator.SetTrigger("Has Shot");
	}

	void ReloadStartAnimation() 
	{
		animator.SetTrigger("Has Started Reloading");
	}

	void ReloadAnimation() 
	{
		animator.SetTrigger("Has Reloaded");
	}

	void FinishReloadingAnimation() 
	{
		animator.SetTrigger("Has Finished Reloading");
	}

	void SwapWeaponStartAnimation()
	{
		animator.SetTrigger("Has Started to Swap Weapon");
	}

	void UseItemAnimation()
	{
		animator.SetTrigger("Has Used Item");
	}

	void InvokeSipTaking()
	{
		PlayerManager.Instance.DisablePlayerComponent(PlayerManager.PlayerComponent.GunComp);
		Invoke("TakeASip", SIPPING_DELAY);
	}

	void TakeASip()
	{
		animator.SetTrigger("Has Taken a Sip");
		Invoke("ReEnableGunComponent", sippingAnimations[(int)weaponHolder.EquippedGun.TypeOfGun].length);
	}

	void ReEnableGunComponent()
	{
		PlayerManager.Instance.EnablePlayerComponent(PlayerManager.PlayerComponent.GunComp);
	}

	void ChangeGunAnimations() 
	{
		Gun currentGun = weaponHolder.EquippedGun;
		
		animator.runtimeAnimatorController = animatorOverrideController;
		animator.avatar = possibleAvatars[(int)currentGun.TypeOfGun];

		animatorOverrideController["DEFAULT IDLE"] = currentGun.IdleAnimation;;
		animatorOverrideController["DEFAULT SHOOT"] = currentGun.ShootAnimation;
		animatorOverrideController["DEFAULT RELOAD START"] = currentGun.ReloadStartAnimation;
		animatorOverrideController["DEFAULT RELOAD"] = currentGun.ReloadAnimation;
		animatorOverrideController["DEFAULT RELOAD FINISH"] = currentGun.ReloadFinishAnimation;
		animatorOverrideController["DEFAULT WEAPON SWAP OUT"] = currentGun.SwapGunOutAnimation;
		animatorOverrideController["DEFAULT WEAPON SWAP IN"] = currentGun.SwapGunInAnimation;
		animatorOverrideController["DEFAULT TAKE A SIP"] = currentGun.TypeOfGun == Gun.GunType.Handgun ? sippingAnimations[0] : 
																										sippingAnimations[1];
		
		ChangeConsumableAnimations();

        currentGun.OnBackToIdle.AddListener(ResetTriggers);
        currentGun.OnShot.AddListener(ShootAnimation);
        currentGun.OnReloadStart.AddListener(ReloadStartAnimation);
        currentGun.OnReload.AddListener(ReloadAnimation);
        currentGun.OnReloadFinish.AddListener(FinishReloadingAnimation);
        weaponHolder.OnGunSwapStart.AddListener(SwapWeaponStartAnimation);
	}

	void ChangeConsumableAnimations()
	{
		Consumable currentConsumable = weaponHolder.EquippedConsumable;
		if (currentConsumable)
		{
			animatorOverrideController["DEFAULT USE ITEM"] = currentConsumable.UseAnimation;
			currentConsumable.OnUse.AddListener(UseItemAnimation);
		}
	}
}