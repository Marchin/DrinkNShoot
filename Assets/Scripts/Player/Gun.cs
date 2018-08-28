using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
	[SerializeField] float damage;
	[SerializeField] float fireRate;
	[SerializeField] float range;
	[SerializeField] float impactForce;
	[SerializeField] int ammoLeft;
	[SerializeField] int cylinderCapacity;
	[SerializeField] AnimationClip shootAnimation;
	[SerializeField] AnimationClip reloadAnimation;
	float lastFireTime = 0;
	int bulletsInCylinder = 0;
	bool isReloading = false;

	void Start()
	{
		bulletsInCylinder = cylinderCapacity; 
	}

	void Update()
	{
		if (Input.GetButtonDown("Fire1") && CanShoot())
			Shoot();

		if (Input.GetButton("Reload") && CanReload())
			StartCoroutine(Reload());
	}

	void Shoot()
	{
		lastFireTime = Time.time;
		bulletsInCylinder--;

		RaycastHit hit;

		if (Physics.Raycast(transform.position, transform.forward, out hit, range))
		{
			Rigidbody targetRigidbody = hit.transform.GetComponent<Rigidbody>();

			if (targetRigidbody)
				targetRigidbody.AddForce(-hit.normal * impactForce);
		}
	}

	IEnumerator Reload()
	{
		isReloading = true;

		for (int i = bulletsInCylinder; i < cylinderCapacity; i++)
		{
			yield return new WaitForSeconds(2);
			bulletsInCylinder++;
			ammoLeft--;
		}

		isReloading = false;
	}

	bool CanShoot()
	{
		return (!isReloading && Time.time - lastFireTime >= 1 / fireRate && bulletsInCylinder > 0);
	}

	bool CanReload()
	{
		return (!isReloading && Time.time - lastFireTime >= 1 / fireRate && ammoLeft > 0 && bulletsInCylinder < cylinderCapacity);
	}

	public float FireRate
	{
		get { return fireRate;}
	}
}