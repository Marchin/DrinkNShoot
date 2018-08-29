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
		currentGun.OnReload.AddListener(IsReloading);
	}

	void Update()
	{

	}

	void HasShot()
	{
		animator.SetTrigger("Has Shot");
	}

	void IsReloading()
	{
		animator.SetBool("Is Reloading", true);
		Invoke("HasFinishedReloading", currentGun.GetReloadTime());
	}

	void HasFinishedReloading()
	{
		animator.SetBool("Is Reloading", false);
	}
}
