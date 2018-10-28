using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Gun : MonoBehaviour
{
	// Enumerators
	public enum GunType
	{
		Handgun, Rifle
	}

	public enum GunState
	{
		Idle, Shooting, Reloading
	}
	
	// Serialized Fields
	[Header("Gun Stats")]
	[SerializeField] [Tooltip("Type of gun.")]
	GunType gunType;
	[SerializeField] [Range(1, 100)] [Tooltip("Amount of bullets that can be inside the gun.")] 
	int gunCapacity;
	[SerializeField] [Range(0f, 12f)] [Tooltip("Bullets fired per second.")] 
	float fireRate;
	[SerializeField] [Range(0f, 1000f)] [Tooltip("Maximum gun range.")] 
	float range;
	[SerializeField] [Range(0f, 1250f)] [Tooltip("Maximum force applied to a shot target.")] 
	float impactForce;
	[SerializeField] [Range(1f, 20f)] [Tooltip("Gun sway after being fired; affects accuracy.")]
	float recoilSwayLevel;
	[SerializeField] [Range(0f, 0.5f)] [Tooltip("Number of seconds that the recoil affects the accuracy.")]
	float recoilDuration;

	[Header("Drunkness Fields")]
	[SerializeField] [Range(0f, 50f)] [Tooltip("Maximum radius of the crosshair circular motion.")]
	float maxCrosshairRadius = 30f;
	[SerializeField] [Range(1, 10)] [Tooltip("Maximum crosshair movement speed.")]
	float maxCrosshairSpeed = 7f;
	
	[Header("Shooting Layers")] 
	[SerializeField] [Tooltip("The names of the layers that the gun can make damage to.")]
	List<string> shootingLayers;
	[SerializeField] [Tooltip("The names of the layers the gun will ignore when using raycasts.")]
	string[] layersToIgnore;

	[Header("Bullet Objects")]
	[SerializeField] [Tooltip("The bullet game objects inside the gun.")]
	GameObject[] bullets;
	
	[Header("Gun Animations")]
	[SerializeField] [Tooltip("The 'shoot' animation associated to the gun.")]
	AnimationClip shootAnimation;
	[SerializeField] [Tooltip("The 'start to reload' animation associated to the gun.")]
	AnimationClip reloadStartAnimation;
	[SerializeField] [Tooltip("The 'reload' animation associated to the gun.")]
	AnimationClip reloadAnimation;
	[SerializeField] [Tooltip("The 'finish reloading' animation associated to the gun.")]
	AnimationClip reloadFinishAnimation;
	[SerializeField] [Tooltip("The 'swap weapon' animation associated to the gun.")]
	AnimationClip swapWeaponAnimation;
	
	[Header("Gun Audio Sources")]
	[SerializeField] [Tooltip("The'shoot' sound associated to the gun.")]
	AudioSource shootSound;
	[SerializeField] [Tooltip("The 'reload' sound associated to the gun.")]
	AudioSource reloadSound;
	[SerializeField] [Tooltip("The sound the gun makes when it is fired while being empty.")]
	AudioSource emptyGunSound;
	
	[Header("Events")]
	[SerializeField] UnityEvent onBackToIdle;
	[SerializeField] UnityEvent onShot;
	[SerializeField] UnityEvent onReloadStart;
	[SerializeField] UnityEvent onReload;
	[SerializeField] UnityEvent onReloadFinish;
	[SerializeField] UnityEvent onReloadCancel;
	[SerializeField] UnityEvent onIncreaseBulletCount;
	[SerializeField] UnityEvent onEmptyGun;
	[SerializeField] UnityEvent onCrosshairScale;
	[SerializeField] UnityEvent onCrosshairColorChange;
	[SerializeField] UnityEvent onCrosshairMove;
	
	// Constants
	const float DRUNK_SWAY_MULT = 10f;
	const float MAX_SWAY_ALLOWED = 10f;
	const float CROSSHAIR_SCALE_MULT = 0.05f;
	
	// Computing Fields
	Camera fpsCamera;
	Coroutine reloadRoutine;
	GunState currentState;
	Vector3 crosshairPosition;
	int bulletsInGun = 0;
	float lastFireTime = 0f;
	float timeToShootSingleBullet = 0f;
	float drunkSwayPercentage = 0f;
	float drunkSwayInterpTime = 0f;
	float crosshairScaleInterpTime = 0f;
	float crosshairScaleAtShotInterpTime = 0f;
	float crosshairScale = 1f;
	int consecutiveShots = 0;
	int consecutiveShotsAtShot = 0;
	float crosshairScaleBeforeShot = 1f;
	float crosshairScaleAfterShot = 1f;
	bool isIncreasingDrunkSway = true;
	bool targetOnClearSight = false;
	float drunkCrosshairRadius = 0f;
	float drunkCrosshairAngle = 0f;
	float drunkCrosshairSpeed = 0f;
	int shootingLayerMask = 0;

	// Unity Methods
	void Awake()
	{
		fpsCamera = GetComponentInParent<Camera>();
		currentState = GunState.Idle;
		crosshairPosition = new Vector3(Screen.width / 2, Screen.height / 2, 1f);
		bulletsInGun = gunCapacity;
		timeToShootSingleBullet = 1f / fireRate;
		recoilDuration += timeToShootSingleBullet;
		shootingLayerMask = ~LayerMask.GetMask(layersToIgnore);
	}

	void Start()
	{
		LevelManager.Instance.OnStartNextStage.AddListener(StopReloading);
	}

	void Update()
	{
		MoveCrosshairAround();
		ScaleCrosshairAccordingToDrukness();
		CheckConsecutiveShots();
		IndicateShotAccuracy();

		switch (currentState)
		{
			case GunState.Idle:
				if (InputManager.Instance.GetFireButton())
				{
					if (CanShoot())
					{
						Shoot();
						onShot.Invoke();
						currentState = GunState.Shooting;
					}
					else
						if (ShouldPlayEmptyMagSound())
							onEmptyGun.Invoke();
				}
				else
					if (InputManager.Instance.GetReloadButton() && CanReload())
					{
						reloadRoutine = StartCoroutine(Reload());
						currentState = GunState.Reloading;
					}
				break;
			
			case GunState.Shooting:
				if (Time.time - lastFireTime >= timeToShootSingleBullet)
					ReturnToIdle();
				break;
			
			case GunState.Reloading:
				if (InputManager.Instance.GetFireButton())
					StopReloading();
				break;
			default:
				break;
		}
	}

	// Private Methods
	void Shoot()
	{	
		consecutiveShots++;
		consecutiveShotsAtShot = consecutiveShots;
		crosshairScaleBeforeShot = crosshairScale;
		crosshairScale += recoilSwayLevel * consecutiveShots * CROSSHAIR_SCALE_MULT;
		crosshairScaleAfterShot = crosshairScale;
		crosshairScaleAtShotInterpTime = 0f;
		OnCrosshairScale.Invoke();

		lastFireTime = Time.time;
		bulletsInGun--;
		if (bullets.GetLength(0) > 0)
			bullets[bulletsInGun].SetActive(false);

		float hitProbability = targetOnClearSight ? 100f : Random.Range(0f, 100f - drunkSwayPercentage);
		Vector3 direction = (fpsCamera.ScreenToWorldPoint(crosshairPosition) - fpsCamera.transform.position).normalized;
		RaycastHit hit;
		
		Debug.DrawRay(fpsCamera.transform.position, direction * range, Color.red, 3);

		if (Physics.Raycast(fpsCamera.transform.position, direction, out hit, range, shootingLayerMask) && hitProbability > 50f)
		{
			Life targetLife = hit.transform.GetComponent<Life>();
			Rigidbody targetRigidbody = hit.transform.GetComponent<Rigidbody>();

			if (targetLife)
				targetLife.TakeDamage();

			if (targetRigidbody)
			{
				float forcePercentage = 1f - (transform.position - hit.transform.position).sqrMagnitude / (range * range);
				targetRigidbody.AddForceAtPosition(-hit.normal * impactForce * forcePercentage, hit.point);
			}
		}
	}

	IEnumerator Reload()
	{
		onReloadStart.Invoke();
		yield return new WaitForSeconds(reloadStartAnimation.length);

		for (int i = bulletsInGun; i < gunCapacity; i++)
		{
			onReload.Invoke();
			yield return new WaitForSeconds(reloadAnimation.length);
			if (bullets.GetLength(0) > 0)
				bullets[bulletsInGun].SetActive(true);
			bulletsInGun++;
			onIncreaseBulletCount.Invoke();
        }

		onReloadFinish.Invoke();
		yield return new WaitForSeconds(reloadFinishAnimation.length);
		reloadRoutine = null;
		ReturnToIdle();
	}

	void MoveCrosshairAround()
	{
		Vector3 previousCrosshairPosition = crosshairPosition;

		crosshairPosition.x = Screen.width / 2 + Mathf.Cos(drunkCrosshairAngle) * drunkCrosshairRadius;
		crosshairPosition.y = Screen.height / 2 + Mathf.Sin(drunkCrosshairAngle) * drunkCrosshairRadius;
		drunkCrosshairAngle += drunkCrosshairSpeed * Time.deltaTime;
		if (drunkCrosshairAngle >= 2 * Mathf.PI)
			drunkCrosshairAngle -= 2 * Mathf.PI;
		
		if (crosshairPosition != previousCrosshairPosition)
			onCrosshairMove.Invoke();
	}

    void ScaleCrosshairAccordingToDrukness()
    {   
		float previousCrosshairScale = crosshairScale;

		if (consecutiveShots == 0)
		{
			if (isIncreasingDrunkSway)
			{
				drunkSwayPercentage = Mathf.Lerp(0f, LevelManager.Instance.DifficultyLevel * DRUNK_SWAY_MULT, drunkSwayInterpTime);
				crosshairScale = Mathf.Lerp(1f, 1f + drunkSwayPercentage * CROSSHAIR_SCALE_MULT, crosshairScaleInterpTime);
			}
			else
			{
                drunkSwayPercentage = Mathf.Lerp(LevelManager.Instance.DifficultyLevel * DRUNK_SWAY_MULT, 0f, drunkSwayInterpTime);
                crosshairScale = Mathf.Lerp(1f + drunkSwayPercentage * CROSSHAIR_SCALE_MULT, 1f, crosshairScaleInterpTime);
			}
			drunkSwayInterpTime += Time.deltaTime;
			crosshairScaleInterpTime += Time.deltaTime;
			if (drunkSwayInterpTime >= 1f)
			{
				drunkSwayInterpTime = 0f;
				crosshairScaleInterpTime = 0f;
				isIncreasingDrunkSway = !isIncreasingDrunkSway;
			}
		}
		else
		{
			crosshairScale = Mathf.Lerp(crosshairScaleAfterShot, crosshairScaleBeforeShot, crosshairScaleAtShotInterpTime);
			crosshairScaleAtShotInterpTime += Time.deltaTime / (recoilDuration * consecutiveShotsAtShot);
        }	
		
		if (crosshairScale != previousCrosshairScale)
			onCrosshairScale.Invoke();
    }

	void CheckConsecutiveShots()
	{
        if (lastFireTime < Time.time - timeToShootSingleBullet)
            if (consecutiveShots != 0)
                consecutiveShots = 0;
	}

	void IndicateShotAccuracy()
	{
		Vector3 direction = (fpsCamera.ScreenToWorldPoint(crosshairPosition) - fpsCamera.transform.position).normalized;
		RaycastHit hit;

        if (Physics.Raycast(fpsCamera.transform.position, direction, out hit, range, shootingLayerMask) &&
			drunkSwayPercentage + recoilSwayLevel * consecutiveShots < MAX_SWAY_ALLOWED)
		{
            if (!targetOnClearSight && shootingLayers.Contains(LayerMask.LayerToName(hit.transform.gameObject.layer)))
			{
				targetOnClearSight = true;
				onCrosshairColorChange.Invoke();
			}
		}
		else
		{
			if (targetOnClearSight)
			{
				targetOnClearSight = false;
				onCrosshairColorChange.Invoke();
			}
		}
	}

	void StopReloading()
	{
		if (reloadRoutine != null)
		{
			StopCoroutine(reloadRoutine);
			reloadRoutine = null;
			onReloadCancel.Invoke();
			onReloadFinish.Invoke();
			Invoke("ReturnToIdle", reloadFinishAnimation.length);
		}
	}

	void ReturnToIdle()
	{
		onBackToIdle.Invoke();
		currentState = GunState.Idle;
	}

	bool CanShoot()
	{
		return Time.time - lastFireTime >= 1 / fireRate && bulletsInGun > 0;
	}

	bool CanReload()
	{
		return  bulletsInGun < gunCapacity;
	}

	bool ShouldPlayEmptyMagSound()
	{
		return bulletsInGun == 0;
	}

	// Getters & Setters
	public GunType TypeOfGun
	{
		get { return gunType; }
	}

	public GunState CurrentState
	{
		get { return currentState; }
	}

	public Vector3 CrosshairPosition
	{
		get { return crosshairPosition; }
	}
	public int GunCapacity
	{
		get { return gunCapacity; }
	}

	public int BulletsInGun
	{
		get { return bulletsInGun; }
	}

	public float CrossshairScale
	{
		get { return crosshairScale; }
	}

	public bool TargetOnClearSight
	{
		get { return targetOnClearSight; }
	}

	public float DrunkCrosshairSpeed
	{
		get { return drunkCrosshairSpeed; }
		set { drunkCrosshairSpeed = value < maxCrosshairSpeed ? value : maxCrosshairSpeed;}
	}
	public float DrunkCrosshairRadius
	{
		get { return drunkCrosshairRadius; }
		set { drunkCrosshairRadius = value < maxCrosshairRadius ? value : maxCrosshairRadius;}
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
	
	public AnimationClip SwapWeaponAnimation
	{
		get { return swapWeaponAnimation;}
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

	public UnityEvent OnBackToIdle
	{
		get { return onBackToIdle; }
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

	public UnityEvent OnReloadCancel
	{
		get { return onReloadCancel; }
	}

	public UnityEvent OnIncreaseBulletCount
	{
		get { return onIncreaseBulletCount; }
	}

	public UnityEvent OnEmptyGun
	{
		get { return onEmptyGun; }
	}

	public UnityEvent OnCrosshairScale
	{
		get { return onCrosshairScale; }
	}

	public UnityEvent OnCrosshairColorChange
	{
		get { return onCrosshairColorChange; }
	}
	
	public UnityEvent OnCrosshairMove
	{
		get { return onCrosshairMove; }
	}
}