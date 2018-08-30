using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Gun : MonoBehaviour
{
	[Header("Gun Stats")] 
	[SerializeField] float damage;
	[SerializeField] float fireRate;
	[SerializeField] float range;
	[SerializeField] float impactForce;
	[SerializeField] int ammoLeft;
	[SerializeField] int cylinderCapacity;
	[Header("Gun Animations")]
	[SerializeField] AnimationClip shootAnimation;
	[SerializeField] AnimationClip reloadStartAnimation;
	[SerializeField] AnimationClip reloadAnimation;
	[SerializeField] AnimationClip reloadFinishAnimation;
	[Header("Gun Audio Souces")]
	[SerializeField] AudioSource shootSound;
	[SerializeField] AudioSource reloadSound;
	[SerializeField] AudioSource emptyGunSound;
	[Header("Layers Masks")]
	[SerializeField] string[] shootingLayers;
	[Header("Events")]
	[SerializeField] UnityEvent onShot;
	[SerializeField] UnityEvent onReloadStart;
	[SerializeField] UnityEvent onReload;
	[SerializeField] UnityEvent onReloadFinish;
	[SerializeField] UnityEvent onEmptyGun;
	// Computing Fields
	float lastFireTime = 0;
	int bulletsInCylinder = 0;
	bool isReloading = false;
	int shootingLayerMask;

	void Start()
	{
		bulletsInCylinder = cylinderCapacity;
		shootingLayerMask = LayerMask.GetMask(shootingLayers);
	}

	void Update()
	{
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
		lastFireTime = Time.time;
		bulletsInCylinder--;
		
		RaycastHit hit;

		if (Physics.Raycast(transform.position, transform.forward, out hit, range, shootingLayerMask))
		{
			Life targetLife = hit.transform.GetComponent<Life>();
			Rigidbody targetRigidbody = hit.transform.GetComponent<Rigidbody>();

			if (targetLife)
			{
				float dmgPercentage = 1 - (transform.position - hit.transform.position).sqrMagnitude / (range * range);
				targetLife.TakeDamage(damage * dmgPercentage);
			}

			if (targetRigidbody)
				targetRigidbody.AddForceAtPosition(-hit.normal * impactForce, hit.point);
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