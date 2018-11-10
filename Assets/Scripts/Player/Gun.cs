using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Gun : MonoBehaviour, IItem
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
	[SerializeField] [Tooltip("Name of the gun.")]
	string gunName;
	[SerializeField] [Range(1, 100)] [Tooltip("Amount of bullets that can be inside the gun.")] 
	int gunCapacity;
	[SerializeField] [Range(0f, 12f)] [Tooltip("Bullets fired per second.")] 
	float fireRate;
	[SerializeField] [Range(0f, 1000f)] [Tooltip("Maximum gun range.")] 
	float range;
	[SerializeField] [Range(0f, 1250f)] [Tooltip("Maximum force applied to a shot target.")] 
	float impactForce;
	[SerializeField] [Range(1f, 100f)] [Tooltip("Gun sway after being fired; affects accuracy.")]
	float recoilSwayLevel;
	
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
	AnimationClip swapGunAnimation;
	
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
	
	// Computing Fields
	Camera fpsCamera;
	Coroutine reloadRoutine;
	GunState currentState;
	int bulletsInGun = 0;
	float lastFireTime = 0f;
	float timeToShootSingleBullet = 0f;
	int consecutiveShots = 0;
	int consecutiveShotsAtShot = 0;
	int shootingLayerMask = 0;

	// Unity Methods
	void Awake()
	{
		fpsCamera = GetComponentInParent<Camera>();
		currentState = GunState.Idle;
		bulletsInGun = gunCapacity;
		timeToShootSingleBullet = 1f / fireRate;
		shootingLayerMask = ~LayerMask.GetMask(layersToIgnore);
	}

	void Start()
	{
		LevelManager.Instance.OnStartNextStage.AddListener(StopReloading);
	}

	void Update()
	{
		CheckConsecutiveShots();

		switch (currentState)
		{
			case GunState.Idle:
				if (InputManager.Instance.GetFireButton())
				{
					if (CanShoot())
					{
						Shoot();
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
				if (Time.unscaledTime - lastFireTime >= timeToShootSingleBullet)
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

		lastFireTime = Time.unscaledTime;
		bulletsInGun--;
		if (bullets.GetLength(0) > 0)
			bullets[bulletsInGun].SetActive(false);
		
		onShot.Invoke();

		float hitProbability = DrunkCrosshair.GetHitProbability();
		Vector3 direction = (fpsCamera.ScreenToWorldPoint(DrunkCrosshair.Position) - fpsCamera.transform.position).normalized;
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

	void CheckConsecutiveShots()
	{
        if (lastFireTime < Time.unscaledTime - timeToShootSingleBullet)
            if (consecutiveShots != 0)
                consecutiveShots = 0;
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
		return Time.unscaledTime - lastFireTime >= 1 / fireRate && bulletsInGun > 0;
	}

	bool CanReload()
	{
		return  bulletsInGun < gunCapacity && Time.timeScale == 1f;
	}

	bool ShouldPlayEmptyMagSound()
	{
		return bulletsInGun == 0;
	}

   // Public Methods
	public GameObject ObjectOnSight()
	{
        Vector3 direction = (fpsCamera.ScreenToWorldPoint(DrunkCrosshair.Position) - fpsCamera.transform.position).normalized;
        RaycastHit hit;

		return Physics.Raycast(fpsCamera.transform.position, direction, out hit, range, shootingLayerMask) ? hit.transform.gameObject : null;
	}

	public bool CanShootAtObject(GameObject target)
	{
		return shootingLayers.Contains(LayerMask.LayerToName(target.layer));
	}

	// Item Interface Methods
	public string GetName()
	{
		return gunName;
	}

	public int GetAmount()
	{
		return 1;
	}
	
	public int GetMaxAmount()
	{
		return 1;
	}

	public ItemType GetItemType()
	{
		return ItemType.Gun;
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

	public int GunCapacity
	{
		get { return gunCapacity; }
	}

	public int BulletsInGun
	{
		get { return bulletsInGun; }
	}

	public float RecoilSwayLevel
	{
		get { return recoilSwayLevel; }
	}

	public int ConsecutiveShots
	{
		get { return consecutiveShots; }
	}

	public int ConsecutiveShotsAtShot
	{
		get { return consecutiveShotsAtShot; }
	}

	public float TimeToShootSingleBullet
	{
		get { return timeToShootSingleBullet; }
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
	
	public AnimationClip SwapGunAnimation
	{
		get { return swapGunAnimation;}
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
}