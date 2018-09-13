using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Gun : MonoBehaviour
{
	public enum GunType
	{
		Handgun
	}

	[Header("Gun Stats")]
	[SerializeField] [Tooltip("Type of gun.")]
	GunType gunType;
	[SerializeField] [Range(0, 12)] [Tooltip("Bullets fired per second.")] 
	float fireRate;
	[SerializeField] [Range(0, 1000)] [Tooltip("Maximum gun range.")] 
	float range;
	[SerializeField] [Range(0, 1250)] [Tooltip("Maximum force applied to a shot target.")] 
	float impactForce;
	[SerializeField] [Range(0, 1000)] [Tooltip("Maximum amount of ammo that can be carried.")] 
	int maxAmmo;
	[SerializeField] [Range(0, 100)] [Tooltip("Amount of bullets that can be inside the gun.")] 
	int cylinderCapacity;
	[SerializeField] [Range(0, 10)] [Tooltip("Regular sway of the gun; affects accuracy.")]
	float regularSwayLevel;
	[SerializeField] [Range(1, 10)] [Tooltip("Gun sway after being fired; affects accuracy.")]
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
	[SerializeField] UnityEvent onCrosshairScale;

	// Computing Fields
	const float baseSway = 0.003f;
	const float swayApproximation = 0.0001f;
	const float interpPerc = 0.1f;
	const int crosshairScaleMultiplier = 20;
	Transform fpsCamera;
	int shootingLayerMask;
	int ammoLeft;
	float lastFireTime = 0;
	int bulletsInCylinder = 0;
	bool isReloading = false;
	float regularSway = 0;
	float recoilSway = 0;
	float drunkSway = 0;
	float crosshairScale = 1;
	int consecutiveShots = 0;
	bool isIncreasingDrunkSway = true;

	void Awake()
	{
		fpsCamera = GetComponentInParent<Camera>().transform;
		bulletsInCylinder = cylinderCapacity;
		ammoLeft = maxAmmo;
		shootingLayerMask = LayerMask.GetMask(shootingLayers);
		regularSway = baseSway * regularSwayLevel;
		recoilSway = regularSway + baseSway * recoilSwayLevel;
	}

	void Update()
	{
		if (!PauseMenu.IsPaused)
		{
			ComputeDrunkSway();
			ComputeConsecutiveShots();
		}

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
		float swayDir;

		if (lastFireTime < Time.time - recoilDuration - 1 / fireRate)
		{
			float minSway = -regularSway - drunkSway;
			float maxSway = regularSway + drunkSway;
			crosshairScale += regularSway * crosshairScaleMultiplier;
			swayDir = Random.Range(0, 2);
			horSway = swayDir == 0 ? Random.Range(minSway, -baseSway) : Random.Range(baseSway, maxSway);
			swayDir = Random.Range(0, 2);
			verSway = swayDir == 0 ? Random.Range(minSway, -baseSway) : Random.Range(baseSway, maxSway);
		}
		else
		{
			consecutiveShots++;
			float minAddedRecoilSway = -recoilSway - baseSway * consecutiveShots - drunkSway;
			float maxAddedRecoilSway = recoilSway + baseSway * consecutiveShots + drunkSway;
			crosshairScale += recoilSway * consecutiveShots * crosshairScaleMultiplier;
			swayDir = Random.Range(0, 2);
			horSway = swayDir == 0 ? Random.Range(minAddedRecoilSway, -regularSway) : Random.Range(regularSway, maxAddedRecoilSway);
			swayDir = Random.Range(0, 2);
			verSway = swayDir == 0 ? Random.Range(minAddedRecoilSway, -regularSway): Random.Range(regularSway, maxAddedRecoilSway);
		}
		
		OnCrosshairScale.Invoke();

		shotSway = new Vector3(horSway, verSway, 0);
		lastFireTime = Time.time;
		bulletsInCylinder--;

		Debug.DrawRay(fpsCamera.position, (fpsCamera.forward + shotSway) * range, Color.red, 3);
		
		RaycastHit hit;

		if (Physics.Raycast(fpsCamera.position, (fpsCamera.forward + shotSway).normalized, 
			out hit, range, shootingLayerMask))
		{
			Life targetLife = hit.transform.GetComponent<Life>();
			Rigidbody targetRigidbody = hit.transform.GetComponent<Rigidbody>();

			if (targetLife)
				targetLife.TakeDamage();

			if (targetRigidbody)
			{
				float forcePercentage = 1 - (transform.position - hit.transform.position).sqrMagnitude / (range * range);
				targetRigidbody.AddForceAtPosition(-hit.normal * impactForce * forcePercentage, hit.point);
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

    void ComputeDrunkSway()
    {
        if (isIncreasingDrunkSway)
        {
            drunkSway = Mathf.Lerp(drunkSway, regularSway * LevelManager.Instance.DifficultyLevel, interpPerc);
            if (drunkSway >= regularSway * LevelManager.Instance.DifficultyLevel - swayApproximation)
                isIncreasingDrunkSway = false;
        }
        else
        {
            drunkSway = Mathf.Lerp(drunkSway, 0, interpPerc);
            if (drunkSway <= swayApproximation)
                isIncreasingDrunkSway = true;
        }

        if (crosshairScale != 1 + drunkSway)
        {
            crosshairScale = Mathf.Lerp(crosshairScale, 1 + drunkSway * crosshairScaleMultiplier, interpPerc);
            onCrosshairScale.Invoke();
        }
    }

	void ComputeConsecutiveShots()
	{
        if (lastFireTime < Time.time - recoilDuration - 1 / fireRate)
        {
            if (consecutiveShots != 0)
                consecutiveShots = 0;
        }
	}

	bool CanShoot()
	{
		return !PauseMenu.IsPaused &&  !LevelManager.Instance.GameOver && !isReloading && 
		Time.time - lastFireTime >= 1 / fireRate && bulletsInCylinder > 0;
	}

	bool CanReload()
	{
		return !PauseMenu.IsPaused && !LevelManager.Instance.GameOver && !isReloading &&
		Time.time - lastFireTime >= 1 / fireRate && ammoLeft > 0 && bulletsInCylinder < cylinderCapacity;
	}

	// Public Methods
	public GunType TypeOfGun
	{
		get { return gunType; }
	}

	public int CylinderCapacity
	{
		get { return cylinderCapacity; }
	}
	
	public int MaxAmmo
	{
		get { return maxAmmo; }
	}

	public int BulletsInCylinder
	{
		get { return bulletsInCylinder; }
	}

	public int AmmoLeft
	{
		get { return ammoLeft; }
	}

	public float CrossshairScale
	{
		get { return crosshairScale; }
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

	public UnityEvent OnCrosshairScale
	{
		get { return onCrosshairScale; }
	}
}