﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Gun : MonoBehaviour
{
	[Header("Gun Stats")] 
	[SerializeField] [Range(0, 100)] [Tooltip("Maximum gun damage.")] 
	float damage;
	[SerializeField] [Range(0, 12)] [Tooltip("Bullets fired per second.")] 
	float fireRate;
	[SerializeField] [Range(0, 1000)] [Tooltip("Maximum gun range.")] 
	float range;
	[SerializeField] [Range(0, 1250)] [Tooltip("Maximum force applied to a shot target.")] 
	float impactForce;
	[SerializeField] [Range(0, 1000)] [Tooltip("Maximum amount of ammo that can be carried.")] 
	int ammoLeft;
	[SerializeField] [Range(0, 100)] [Tooltip("Amount of bullets that can be inside the gun.")] 
	int cylinderCapacity;
	[SerializeField] [Range(0, 10)] [Tooltip("Regular sway of the gun; affects accuracy.")]
	float regularSwayLevel;
	[SerializeField] [Range(0, 10)] [Tooltip("Gun sway after being fired; affects accuracy.")]
	float recoilSwayLevel;
	[SerializeField] [Range(0, 5)] [Tooltip("The number of seconds that the recoil affects the sway.")]
	float recoilDuration;
	[Header("Gun Animations")]
	[SerializeField] [Tooltip("The 'shoot' animation associated to the gun.")]
	AnimationClip shootAnimation;
	[SerializeField] [Tooltip("The 'start to reload' animation associated to the gun.")]
	AnimationClip reloadStartAnimation;
	[SerializeField] [Tooltip("The 'reload' animation associated to the gun.")]
	AnimationClip reloadAnimation;
	[SerializeField] [Tooltip("The 'finish reloading' animation associated to the gun.")]
	AnimationClip reloadFinishAnimation;
	[Header("Gun Audio Souces")]
	[SerializeField] [Tooltip("The'shoot' sound associated to the gun.")]
	AudioSource shootSound;
	[SerializeField] [Tooltip("The 'reload' sound associated to the gun.")]
	AudioSource reloadSound;
	[SerializeField] [Tooltip("The sound the gun makes when it is fired while being empty.")]
	AudioSource emptyGunSound;
	[Header("Layers Masks")]
	[SerializeField] [Tooltip("The name of layers that contain the possibe shooting targets.")]
	string[] shootingLayers;
	[Header("Events")]
	[SerializeField] UnityEvent onShot;
	[SerializeField] UnityEvent onReloadStart;
	[SerializeField] UnityEvent onReload;
	[SerializeField] UnityEvent onReloadFinish;
	[SerializeField] UnityEvent onEmptyGun;
	// Computing Fields
	const float baseSway = 0.005f;
	Transform fpsCamera;
	int shootingLayerMask;
	float lastFireTime = 0;
	int bulletsInCylinder = 0;
	bool isReloading = false;
	float regularSway;
	float recoilSway;
	int consecutiveShots;

	void Awake()
	{
		fpsCamera = GetComponentInParent<Camera>().transform;
	}

	void Start()
	{
		bulletsInCylinder = cylinderCapacity;
		shootingLayerMask = LayerMask.GetMask(shootingLayers);
		regularSway = baseSway * regularSwayLevel;
		recoilSway = baseSway * recoilSwayLevel;
		recoilDuration += 1 / fireRate;
	}

	void Update()
	{
		if (lastFireTime < Time.time - recoilDuration)
			if (consecutiveShots != 0)
				consecutiveShots = 0;

		if (InputManager.Instance.GetFireButton())
		{
			if (CanShoot())
			{
				Shoot();
				onShot.Invoke();
			}
			else
				if (bulletsInCylinder == 0)
					onEmptyGun.Invoke();
		}

		if (InputManager.Instance.GetReloadButton() && CanReload())
			StartCoroutine(Reload());
	}
	// Private Methods
	void Shoot()
	{
		Vector3 shotSway;
		float horSway;
		float verSway;

		if (lastFireTime < Time.time - recoilDuration)
		{
			horSway = Random.Range(-regularSway, regularSway);
			verSway = Random.Range(-regularSway, regularSway);	
		}
		else
		{
			float additionalSway = baseSway * consecutiveShots;
			horSway = Random.Range(-recoilSway - additionalSway, recoilSway + additionalSway);
			verSway = Random.Range(-recoilSway - additionalSway, recoilSway + additionalSway);
			consecutiveShots++;
		}

		shotSway = new Vector3(horSway, verSway, 0);
		lastFireTime = Time.time;
		bulletsInCylinder--;
		
		RaycastHit hit;

		if (Physics.Raycast(fpsCamera.position, (fpsCamera.forward + shotSway).normalized, 
			out hit, range, shootingLayerMask))
		{
			Life targetLife = hit.transform.GetComponent<Life>();
			Rigidbody targetRigidbody = hit.transform.GetComponent<Rigidbody>();

			if (targetLife)
			{
				float dmgPercentage = 1 - (transform.position - hit.transform.position).sqrMagnitude / (range * range);
				targetLife.TakeDamage(damage * dmgPercentage);
			}

			if (targetRigidbody)
			{
				float forcePercentage = 1 - (transform.position - hit.transform.position).sqrMagnitude / (range * range);
				targetRigidbody.AddForceAtPosition(-hit.normal * impactForce, hit.point);
			}
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
			ammoLeft--;
		}

		onReloadFinish.Invoke();
		isReloading = false;
	}

	bool CanShoot()
	{
		return (!isReloading && Time.time - lastFireTime >= 1 / fireRate && 
				bulletsInCylinder > 0);
	}

	bool CanReload()
	{
		return (!isReloading && Time.time - lastFireTime >= 1 / fireRate && 
				ammoLeft > 0 && bulletsInCylinder < cylinderCapacity);
	}
	// Public Methods
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

	public AudioSource ShootSound
	{
		get { return shootSound; }
	}

	public AudioSource ReloadSound
	{
		get { return reloadSound; }
	}
	public AudioSource EmptyGunSound
	{
		get { return emptyGunSound; }
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