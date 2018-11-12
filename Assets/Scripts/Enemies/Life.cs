using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Life : MonoBehaviour 
{
	[SerializeField] int hitPoints;
	[SerializeField] float deathLength;
	
	ParticleSystem featherExplosion;
	SkinnedMeshRenderer skinnedMeshRenderer;
	int totalHitPoints;

	UnityEvent onDeath;
	
	void Awake() 
	{
		onDeath = new UnityEvent();
		featherExplosion = GetComponentInChildren<ParticleSystem>();
		skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
		totalHitPoints = hitPoints;	
	}

	void OnEnable() 
	{
		featherExplosion.Stop();
		skinnedMeshRenderer.enabled = true;
		hitPoints = totalHitPoints;
	}

	void Die()
	{
		skinnedMeshRenderer.enabled = false;
		Invoke("Disable", deathLength);
	}
	
	public void TakeDamage()
	{
		featherExplosion.Play();
		hitPoints--;
		
		if (hitPoints <= 0)
		{
			onDeath.Invoke();
			Die();
		}		
	}

	void Disable()
	{
		GetComponent<Crow>().Die();
	}

	public UnityEvent OnDeath
	{
		get { return onDeath; }
	}
}
