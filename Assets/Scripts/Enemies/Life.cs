using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Life : MonoBehaviour 
{
	[SerializeField] int hitPoints;
	[SerializeField] float deathLength;
	[SerializeField] UnityEvent onDeath;

	void Die()
	{
		Destroy(gameObject, deathLength);
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

	public UnityEvent OnDeath
	{
		get { return onDeath; }
	}
}
