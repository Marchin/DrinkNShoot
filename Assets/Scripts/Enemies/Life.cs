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
	MeshRenderer meshRenderer;
	int totalHitPoints;

	private void Awake() 
	{
		featherExplosion = GetComponentInChildren<ParticleSystem>();
		meshRenderer = GetComponentInChildren<MeshRenderer>();
		totalHitPoints = hitPoints;	
	}

	private void OnEnable() 
	{
		featherExplosion.Stop();
		meshRenderer.enabled = true;
		hitPoints = totalHitPoints;
	}

	void Die()
	{
		meshRenderer.enabled = false;
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
