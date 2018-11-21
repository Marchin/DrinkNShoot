using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CrowAnimation : MonoBehaviour
{
	Animator animator;
	Crow crow;
	CrowMovement crowMovement;

	void Awake()
	{
		animator = GetComponent<Animator>();
		crow = GetComponentInParent<Crow>();
		crowMovement = GetComponentInParent<CrowMovement>();
	}

	void Start()
	{
		crow.OnLand.AddListener(WalkAnimation);
		crow.OnFly.AddListener(FlyAnimation);
		crowMovement.OnStartWalking.AddListener(WalkAnimation);
		crowMovement.OnStopWalking.AddListener(IdleAnimation);
	}

	void IdleAnimation()
	{
		animator.SetTrigger("Has Stopped");
	}

	void WalkAnimation()
	{
		animator.SetTrigger("Has Started Walking");
	}

	void FlyAnimation()
	{
		animator.SetTrigger("Has Flown");
	}
}
