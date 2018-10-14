using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CrowAnimation : MonoBehaviour
{
	Animator animator;
	Crow crow;

	void Awake()
	{
		animator = GetComponent<Animator>();
		crow = GetComponentInParent<Crow>();
	}

	void Start()
	{
		crow.OnLand.AddListener(HasLanded);
		crow.OnFly.AddListener(HasFlown);
	}

	void HasLanded()
	{
		animator.SetTrigger("Has Landed");
	}

	void HasFlown()
	{
		animator.SetTrigger("Has Flown");
	}
}
