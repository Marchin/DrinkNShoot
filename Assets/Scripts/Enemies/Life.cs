using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Life : MonoBehaviour 
{
	[SerializeField] float health;
	[SerializeField] float deathLength;
	[SerializeField] UnityEvent onDeath;

	void Die()
	{
		Destroy(gameObject, deathLength);
	}
	
	public void TakeDamage(float amount)
	{
		health -= amount;
		
		if (health <= 0)
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
