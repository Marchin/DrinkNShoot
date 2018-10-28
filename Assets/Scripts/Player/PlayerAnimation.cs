using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[RequireComponent(typeof(Animator))]
public class PlayerAnimation : MonoBehaviour
{
	[SerializeField] AnimatorOverrideController animatorOverrideController;
	[SerializeField] AnimationClip[] handgunAnimations;
	[SerializeField] AnimationClip[] rifleAnimations;
	[SerializeField] AnimationClip sippingAnimation;
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
		weaponHolder.OnWeaponSwap.AddListener(ChangeGunAnimations);	
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

	void SwapWeaponAnimation()
	{
		animator.SetTrigger("Has Swapped Weapon");
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
		Gun currentGun = weaponHolder.EquippedGun;
		
		animator.runtimeAnimatorController = animatorOverrideController;
		animator.avatar = possibleAvatars[(int)currentGun.TypeOfGun];

		animatorOverrideController["DEFAULT IDLE"] = currentGun.TypeOfGun == Gun.GunType.Handgun ? handgunAnimations[0] : rifleAnimations[0];
		animatorOverrideController["DEFAULT SHOOT"] = currentGun.ShootAnimation;
		animatorOverrideController["DEFAULT RELOAD START"] = currentGun.ReloadStartAnimation;
		animatorOverrideController["DEFAULT RELOAD"] = currentGun.ReloadAnimation;
		animatorOverrideController["DEFAULT RELOAD FINISH"] = currentGun.ReloadFinishAnimation;
		animatorOverrideController["DEFAULT WEAPON SWAP"] = currentGun.SwapWeaponAnimation;

        currentGun.OnBackToIdle.AddListener(ResetTriggers);
        currentGun.OnShot.AddListener(ShootAnimation);
        currentGun.OnReloadStart.AddListener(ReloadStartAnimation);
        currentGun.OnReload.AddListener(ReloadAnimation);
        currentGun.OnReloadFinish.AddListener(FinishReloadingAnimation);
        weaponHolder.OnWeaponSwapStart.AddListener(SwapWeaponAnimation);
	}
}