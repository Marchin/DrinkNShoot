using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Life : MonoBehaviour 
{
	[SerializeField] int hitPoints;
	[SerializeField] float deathLength;
	[SerializeField] UnityEvent onDeath;
	ParticleSystem featherExplosion;
	SkinnedMeshRenderer skinnedMeshRenderer;
	int totalHitPoints;

	private void Awake() 
	{
		featherExplosion = GetComponentInChildren<ParticleSystem>();
		skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
		totalHitPoints = hitPoints;	
	}

	private void OnEnable() 
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
		gameObject.SetActive(false);
	}

	public UnityEvent OnDeath
	{
		get { return onDeath; }
	}
}
