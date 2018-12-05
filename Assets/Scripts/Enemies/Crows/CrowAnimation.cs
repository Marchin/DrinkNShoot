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
	}

	void Update()
	{
		if ((Object)crow.CurrentState == crowMovement)
			animator.SetFloat("Velocity", crowMovement.Velocity);
	}

	void PlayFlyAnimation()
	{
		animator.SetTrigger("Has Flown");
	}

	void PlayMovementAnimation()
	{
		animator.SetTrigger("Has Landed");
	}
}