using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Gun : MonoBehaviour
{
	// Enumerators
	public enum GunType
	{
		Handgun
	}

	enum GunState
	{
		Idle, Shooting, Reloading
	}
	
	// Serialized Fields
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
	[SerializeField] [Range(1, 10)] [Tooltip("Gun sway after being fired; affects accuracy.")]
	float recoilSwayLevel;
	
	[Header("Shooting Layers")] 
	[SerializeField] [Tooltip("The names of the layers that the gun can make damage to.")]
	List<string> shootingLayers;
	[SerializeField] [Tooltip("The names of the layers the gun will ignore when using raycasts.")]
	string[] layersToIgnore;
	
	[Header("Gun Animations")]
	[SerializeField] [Tooltip("The 'shoot' animation associated to the gun.")]
	AnimationClip shootAnimation;
	[SerializeField] [Tooltip("The 'start to reload' animation associated to the gun.")]
	AnimationClip reloadStartAnimation;
	[SerializeField] [Tooltip("The 'reload' animation associated to the gun.")]
	AnimationClip reloadAnimation;
	[SerializeField] [Tooltip("The 'finish reloading' animation associated to the gun.")]
	AnimationClip reloadFinishAnimation;
	
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
	[SerializeField] UnityEvent onEmptyGun;
	[SerializeField] UnityEvent onCrosshairScale;
	[SerializeField] UnityEvent onCrosshairColorChange;
	[SerializeField] UnityEvent onCrosshairMove;
	
	// Constants
	const float DRUNK_SWAY_MULT = 10f;
	const float RECOIL_SWAY_MULT = 0.5f;
	const float MAX_SWAY_ALLOWED = 10f;
	const float CROSSHAIR_SCALE_MULT = 0.05f;
	const float MAX_DRUNK_CROSSHAIR_SPEED = 7f;
	
	// Computing Fields
	Camera fpsCamera;
	Coroutine reloadRoutine;
	GunState currentState;
	Vector3 crosshairPosition;
	int ammoLeft;
	int bulletsInCylinder = 0;
	float lastFireTime = 0f;
	float recoilDuration = 0f;
	float drunkSwayPercentage = 0f;
	float drunkSwayInterpTime = 0f;
	float crosshairScaleInterpTime = 0f;
	float crosshairScaleAtShotInterpTime = 0f;
	float crosshairScale = 1f;
	int consecutiveShots = 0;
	float crosshairScaleBeforeShot = 1f;
	float crosshairScaleAfterShot = 1f;
	bool isIncreasingDrunkSway = true;
	bool targetOnClearSight = false;
	float drunkCrosshairRadius = 25f;
	float drunkCrosshairAngle = 0f;
	float drunkCrosshairSpeed = 0f;
	int shootingLayerMask = 0;

	// Unity Methods
	void Awake()
	{
		fpsCamera = GetComponentInParent<Camera>();
		currentState = GunState.Idle;
		crosshairPosition = new Vector3(Screen.width / 2f, Screen.height / 2f, 1f);
		bulletsInCylinder = cylinderCapacity;
		ammoLeft = maxAmmo;
		recoilDuration = 1f / fireRate;
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
				if (Time.time - lastFireTime >= recoilDuration)
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
		crosshairScaleBeforeShot = crosshairScale;
		crosshairScale += recoilSwayLevel * consecutiveShots * CROSSHAIR_SCALE_MULT;
		crosshairScaleAfterShot = crosshairScale;
		crosshairScaleAtShotInterpTime = 0f;
		OnCrosshairScale.Invoke();

		lastFireTime = Time.time;
		bulletsInCylinder--;

		Vector3 direction = (fpsCamera.ScreenToWorldPoint(crosshairPosition) - fpsCamera.transform.position).normalized;
		RaycastHit hit;
		
		Debug.DrawRay(fpsCamera.transform.position, direction * range, Color.red, 3);

		if (Physics.Raycast(fpsCamera.transform.position, direction, out hit, range, shootingLayerMask) && targetOnClearSight)
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

		for (int i = bulletsInCylinder; i < cylinderCapacity; i++)
		{
			onReload.Invoke();
			yield return new WaitForSeconds(reloadAnimation.length);
			bulletsInCylinder++;
			ammoLeft--;
        }

		onReloadFinish.Invoke();
		yield return new WaitForSeconds(reloadFinishAnimation.length);
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

		if (isIncreasingDrunkSway)
		{
			drunkSwayPercentage = Mathf.Lerp(0f, LevelManager.Instance.DifficultyLevel * DRUNK_SWAY_MULT, drunkSwayInterpTime);
			drunkSwayInterpTime += Time.deltaTime;
			if (consecutiveShots == 0)
				crosshairScale = Mathf.Lerp(1f, 1f + drunkSwayPercentage * CROSSHAIR_SCALE_MULT, crosshairScaleInterpTime);
			else
			{
				crosshairScale = Mathf.Lerp(crosshairScaleAfterShot, crosshairScaleBeforeShot, crosshairScaleAtShotInterpTime);
				crosshairScaleAtShotInterpTime += Time.deltaTime / recoilDuration;
				if (crosshairScaleAtShotInterpTime >= 1f)
					crosshairScaleAtShotInterpTime = 0f;
			}
			crosshairScaleInterpTime += Time.deltaTime;
			if (drunkSwayInterpTime >= 1f)
			{
				drunkSwayInterpTime = 0f;
				crosshairScaleInterpTime = 0f;
				isIncreasingDrunkSway = false;
			}
		}
		else
		{
			drunkSwayPercentage = Mathf.Lerp(LevelManager.Instance.DifficultyLevel * DRUNK_SWAY_MULT, 0f, drunkSwayInterpTime);
			drunkSwayInterpTime += Time.deltaTime;
			if (consecutiveShots == 0)
				crosshairScale = Mathf.Lerp(1f + drunkSwayPercentage * CROSSHAIR_SCALE_MULT, 1f, crosshairScaleInterpTime);
			else
			{
				crosshairScale = Mathf.Lerp(crosshairScaleAfterShot, crosshairScaleBeforeShot, crosshairScaleAtShotInterpTime);
				crosshairScaleAtShotInterpTime += Time.deltaTime / recoilDuration;
				if (crosshairScaleAtShotInterpTime >= 1f)
					crosshairScaleAtShotInterpTime = 0f;
			}
			crosshairScaleInterpTime += Time.deltaTime;
			if (drunkSwayInterpTime >= 1f)
			{
				drunkSwayInterpTime = 0f;
				crosshairScaleInterpTime = 0f;
				isIncreasingDrunkSway = true;
			}
		}		
		
		if (crosshairScale != previousCrosshairScale)
			onCrosshairScale.Invoke();
    }

	void CheckConsecutiveShots()
	{
        if (lastFireTime < Time.time - recoilDuration)
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
		currentState = GunState.Idle;
		onBackToIdle.Invoke();
	}

	bool CanShoot()
	{
		return Time.time - lastFireTime >= 1 / fireRate && bulletsInCylinder > 0;
	}

	bool CanReload()
	{
		return ammoLeft > 0 && bulletsInCylinder < cylinderCapacity;
	}

	bool ShouldPlayEmptyMagSound()
	{
		return bulletsInCylinder == 0;
	}

	// Getters & Setters
	public GunType TypeOfGun
	{
		get { return gunType; }
	}

	public Vector3 CrosshairPosition
	{
		get { return crosshairPosition; }
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

	public bool TargetOnClearSight
	{
		get { return targetOnClearSight; }
	}

	public float DrunkCrosshairSpeed
	{
		get { return drunkCrosshairSpeed; }
		set { drunkCrosshairSpeed = value < MAX_DRUNK_CROSSHAIR_SPEED ? value : MAX_DRUNK_CROSSHAIR_SPEED;}
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