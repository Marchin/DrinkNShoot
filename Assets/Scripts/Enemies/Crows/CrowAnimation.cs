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
		crow.OnLand.AddListener(PlayMovementAnimation);
		crow.OnFly.AddListener(PlayFlyAnimation);
		crow.OnAttack.AddListener(PlayAttackAnimation);
	}

	void Update()
	{
		if ((Object)crow.CurrentState == crowMovement)
			animator.SetFloat("Velocity", crowMovement.Velocity);
	}

	void PlayFlyAnimation()
	{
		animator.SetBool("Flying", true);
		animator.SetBool("Attacking", false);
	}

	void PlayMovementAnimation()
	{
		animator.SetBool("Flying", false);
		animator.SetBool("Attacking", false);
	}

	void PlayAttackAnimation() 
	{
		animator.SetBool("Attacking", true);
	}
}