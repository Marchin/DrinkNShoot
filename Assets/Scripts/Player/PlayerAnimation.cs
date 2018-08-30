using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimation : MonoBehaviour
{
	[SerializeField] Gun currentGun;
	Animator animator;

	void Awake()
	{
		animator = GetComponent<Animator>();
		currentGun = GetComponentInChildren<Gun>(); 
	}

	void Start()
	{
		currentGun.OnShot.AddListener(HasShot);
		currentGun.OnReloadStart.AddListener(HasStartedReloading);
		currentGun.OnReload.AddListener(HasReloaded);
		currentGun.OnReloadFinish.AddListener(HasFinishedReloading);
	}

	void Update()
	{

	}

	void HasShot()
	{
		animator.SetTrigger("Has Shot");
	}

	void HasStartedReloading()
	{
		animator.SetBool("Has Started Reloading", true);
	}
	void HasReloaded()
	{
		animator.SetTrigger("Has Reloaded");
	}

	void HasFinishedReloading()
	{
		animator.SetTrigger("Has Finished Reloading");
	}
}
