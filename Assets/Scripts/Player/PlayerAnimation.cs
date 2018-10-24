﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[RequireComponent(typeof(Animator))]
public class PlayerAnimation : MonoBehaviour
{
	[SerializeField] AnimatorOverrideController animatorOverrideController;
	[SerializeField] AnimationClip[] handgunAnimations;
	[SerializeField] AnimationClip sippingAnimation;
	
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

		weaponHolder.EquippedGun.OnBackToIdle.AddListener(ResetTriggers);
		weaponHolder.EquippedGun.OnShot.AddListener(ShootAnimation);
		weaponHolder.EquippedGun.OnReloadStart.AddListener(ReloadStartAnimation);
		weaponHolder.EquippedGun.OnReload.AddListener(ReloadAnimation);
		weaponHolder.EquippedGun.OnReloadFinish.AddListener(FinishReloadingAnimation);

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

	void InvokeSipTaking()
	{
		PlayerManager.Instance.DisablePlayerComponent(PlayerManager.PlayerComponent.GunComp);
		Invoke("TakeASip", SIPPING_DELAY);
	}

	void TakeASip()
	{
		animator.SetTrigger("Has Taken a Sip");
		Invoke("ReEnableGunComponent", sippingAnimation.length);
	}

	void ReEnableGunComponent()
	{
		PlayerManager.Instance.EnablePlayerComponent(PlayerManager.PlayerComponent.GunComp);
	}

	void ChangeGunAnimations() 
	{
		animator.runtimeAnimatorController = animatorOverrideController;

		animatorOverrideController["DEFAULT IDLE"] = handgunAnimations[0];
		animatorOverrideController["DEFAULT SHOOT"] = weaponHolder.EquippedGun.ShootAnimation;
		animatorOverrideController["DEFAULT RELOAD START"] = weaponHolder.EquippedGun.ReloadStartAnimation;
		animatorOverrideController["DEFAULT RELOAD"] = weaponHolder.EquippedGun.ReloadAnimation;
		animatorOverrideController["DEFAULT RELOAD FINISH"] = weaponHolder.EquippedGun.ReloadFinishAnimation;
	}
}