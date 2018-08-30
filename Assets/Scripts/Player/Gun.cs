using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Gun : MonoBehaviour
{
	[SerializeField] float damage;
	[SerializeField] float fireRate;
	[SerializeField] float range;
	[SerializeField] float impactForce;
	[SerializeField] int ammoLeft;
	[SerializeField] int cylinderCapacity;
	[SerializeField] AnimationClip shootAnimation;
	[SerializeField] AnimationClip reloadStartAnimation;
	[SerializeField] AnimationClip reloadAnimation;
	[SerializeField] AnimationClip reloadFinishAnimation;
	[SerializeField] UnityEvent onShot;
	[SerializeField] UnityEvent onReloadStart;
	[SerializeField] UnityEvent onReload;
	[SerializeField] UnityEvent onReloadFinish;
	[SerializeField] UnityEvent onEmptyGun;

	float lastFireTime = 0;
	int bulletsInCylinder = 0;
	bool isReloading = false;

	void Start()
	{
		bulletsInCylinder = cylinderCapacity; 
	}

	void Update()
	{
		if (Input.GetButtonDown("Fire1"))
		{
			if (CanShoot())
			{
				Shoot();
				onShot.Invoke();
			}
			else
				onEmptyGun.Invoke();
		}

		if (Input.GetButton("Reload") && CanReload())
			StartCoroutine(Reload());
	}

	void Shoot()
	{
		lastFireTime = Time.time;
		bulletsInCylinder--;
		Debug.Log(bulletsInCylinder);
		

		RaycastHit hit;

		if (Physics.Raycast(transform.position, transform.forward, out hit, range, 
			LayerMask.GetMask("Dynamic Decoration, Shootables")))
		{
			Rigidbody targetRigidbody = hit.transform.GetComponent<Rigidbody>();

			if (targetRigidbody)
				targetRigidbody.AddForce(-hit.normal * impactForce);
		}
	}

	IEnumerator Reload()
	{
		isReloading = true;
		onReloadStart.Invoke();

		for (int i = bulletsInCylinder; i < cylinderCapacity; i++)
		{
			onReload.Invoke();
			yield return new WaitForSeconds(reloadAnimation.length);
			bulletsInCylinder++;
			Debug.Log(bulletsInCylinder);
			ammoLeft--;
		}

		onReloadFinish.Invoke();
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

	public float GetReloadTime()
	{
		return reloadAnimation.length;
	}

	public AnimationClip ShootAnimation
	{
		get { return shootAnimation;}
	}

	public AnimationClip ReloadStartAnimation
	{
		get { return reloadStartAnimation;}
	}

	public AnimationClip ReloadAnimation
	{
		get { return reloadAnimation;}
	}

	public AnimationClip ReloadFinishAnimation
	{
		get { return reloadFinishAnimation;}
	}

	public UnityEvent OnShot
	{
		get { return onShot; }
	}

	public UnityEvent OnReloadStart
	{
		get { return onReloadStart; }
	}

	public UnityEvent OnReload
	{
		get { return onReload; }
	}

	public UnityEvent OnReloadFinish
	{
		get { return onReloadFinish; }
	}
	public UnityEvent OnEmptyGun
	{
		get { return onEmptyGun; }
	}
}