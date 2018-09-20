using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Life : MonoBehaviour 
{
	[SerializeField] int hitPoints;
	[SerializeField] float deathLength;
	[SerializeField] UnityEvent onDeath;
	int totalHitPoints;

	private void Awake() 
	{
		totalHitPoints = hitPoints;	
	}

	private void OnEnable() 
	{
		hitPoints = totalHitPoints;
	}

	void Die()
	{
		Invoke("Disable", deathLength);
	}
	
	public void TakeDamage()
	{
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
