using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[RequireComponent(typeof(Animator))]
public class PlayerAnimation : MonoBehaviour
{
	[SerializeField] AnimatorOverrideController animatorOverrideController;
	[SerializeField] AnimationClip[] handgunAnimations;
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
		weaponHolder.EquippedGun.OnShot.AddListener(HasShot);
		weaponHolder.EquippedGun.OnReloadStart.AddListener(HasStartedReloading);
		weaponHolder.EquippedGun.OnReload.AddListener(HasReloaded);
		weaponHolder.EquippedGun.OnReloadFinish.AddListener(HasFinishedReloading);
	}

	void ResetTriggers() 
	{
		foreach (AnimatorControllerParameter parameter in animator.parameters)
			if (parameter.type == AnimatorControllerParameterType.Trigger && parameter.name != "Has Finished Reloading")
				animator.ResetTrigger(parameter.name);
	}

	void HasShot() 
	{
		animator.SetTrigger("Has Shot");
	}

	void HasStartedReloading() 
	{
		animator.SetTrigger("Has Started Reloading");
	}

	void HasReloaded() 
	{
		animator.SetTrigger("Has Reloaded");
	}

	void HasFinishedReloading() 
	{
		animator.SetTrigger("Has Finished Reloading");
	}

	void ChangeGunAnimations() 
	{
		animator.runtimeAnimatorController = animatorOverrideController;

		animatorOverrideController["DEFAULT IDLE"] = handgunAnimations[0];
		animatorOverrideController["DEFAULT SHOOTING"] = weaponHolder.EquippedGun.ShootAnimation;
		animatorOverrideController["DEFAULT RELOADING START"] = weaponHolder.EquippedGun.ReloadStartAnimation;
		animatorOverrideController["DEFAULT RELOADING"] = weaponHolder.EquippedGun.ReloadAnimation;
		animatorOverrideController["DEFAULT RELOADING FINISH"] = weaponHolder.EquippedGun.ReloadFinishAnimation;
	}
}