using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Life : MonoBehaviour 
{
	[SerializeField] int hitPoints;
	
	ParticleSystem featherExplosion;
	SkinnedMeshRenderer skinnedMeshRenderer;
	int totalHitPoints;
	float deathLength;

	UnityEvent onDeath = new UnityEvent();
	
	void Awake() 
	{
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
		Invoke("Disable", deathLength * Time.timeScale);
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

	public float DeathLength
	{
		get { return deathLength; }
		set { deathLength = value; }
	}

	public UnityEvent OnDeath
	{
		get { return onDeath; }
	}
}