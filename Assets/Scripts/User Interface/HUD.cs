using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour 
{
	[Header("HUD Elements")]
	[SerializeField] Image crosshair;
	[SerializeField] Sprite[] gunSprites;
	[SerializeField] Sprite[] consumableSprites;
	[SerializeField] GameObject ammoHUD;
	[SerializeField] GameObject crowHUD;
	[SerializeField] GameObject timerHUD;
	[SerializeField] GameObject currencyHUD;
	[SerializeField] GameObject consumablesHUD;
	[SerializeField] GameObject rankBanner;
	[SerializeField] Image rankImage;
	[SerializeField] Sprite[] rankSprites;
	
	[Header("Audio Sources")]
	[SerializeField] AudioSource slideInBannerSound;
	[SerializeField] AudioSource slideOutBannerSound;
	[SerializeField] AudioSource rankBannerSound;
	[SerializeField] AudioSource[] clockTickSounds;
	
	[Header("Animations")]
	[SerializeField] AnimationClip slidingAnimation;
	
	[Header("Other Properties")]
	[SerializeField] float rankBannerDuration = 3f;
	
	const int CRITICAL_AMMO_IN_GUN = 1;
	const int CRITICAL_TIME_LEFT = 10;

	WeaponHolder weaponHolder;
    TextMeshProUGUI ammoText;
    TextMeshProUGUI crowsText;
    TextMeshProUGUI timerText;
    TextMeshProUGUI currencyText;
    TextMeshProUGUI consumablesText;
	TextMeshProUGUI rankBannerText;
	Animator rankBannerAnimator;
	Animator ammoHUDAnimator;
	Animator crowHUDAnimator;
	Animator timerHUDAnimator;
	Animator consumablesHUDAnimator;
	Color darkRedColor;
	Color bronzeColor;
	Color silverColor;
	Color goldColor;
	float objectiveBannerTimer;
	float clockTickTimer;
	bool objectiveBannerWasJustDisabled;

	void Awake()
	{
		ammoText = ammoHUD.GetComponentInChildren<TextMeshProUGUI>();
		crowsText = crowHUD.GetComponentInChildren<TextMeshProUGUI>();
		timerText = timerHUD.GetComponentInChildren<TextMeshProUGUI>();
		currencyText = currencyHUD.GetComponentInChildren<TextMeshProUGUI>();
		consumablesText = consumablesHUD.GetComponentInChildren<TextMeshProUGUI>();
		rankBannerText = rankBanner.GetComponentInChildren<TextMeshProUGUI>();

		ammoHUDAnimator = ammoHUD.GetComponent<Animator>();
		crowHUDAnimator = crowHUD.GetComponent<Animator>();
		timerHUDAnimator = timerHUD.GetComponent<Animator>();
		consumablesHUDAnimator = consumablesHUD.GetComponent<Animator>();
		rankBannerAnimator = rankBanner.GetComponent<Animator>();

		
		darkRedColor = new Color(0.5f, 0.1f, 0.1f);
		bronzeColor = new Color(0.6f, 0.3f, 0.1f);
		silverColor = new Color(0.4f, 0.4f, 0.3f);
		goldColor = new Color(0.6f, 0.5f, 0f);
	}

    void Start()
    {
		weaponHolder = FindObjectOfType<WeaponHolder>();
		
		ChangeGunInDisplay();
		ChangeConsumableInDisplay();
		
		weaponHolder.OnGunSwap.AddListener(ChangeGunInDisplay);
		weaponHolder.OnConsumableSwap.AddListener(ChangeConsumableInDisplay);
		LevelManager.Instance.OnEnemyKill.AddListener(ChangeKillsDisplay);
		LevelManager.Instance.OnStartNextStage.AddListener(ChangeKillsDisplay);
		PlayerManager.Instance.OnGunEnable.AddListener(ToggleCrosshair);
		PlayerManager.Instance.OnGunDisable.AddListener(ToggleCrosshair);

		objectiveBannerTimer = 0f;
		objectiveBannerWasJustDisabled = false;
		clockTickTimer = 0f;

		ChangeAmmoDisplay();
		if (weaponHolder.EquippedConsumable)
			ChangeConsumablesDisplay();
		ChangeKillsDisplay();
    }

	void Update()
	{
		ChangeTimerDisplay();
		ComputeRankBannerDisplay();
	}

    void ScaleCrosshair()
    {
        float newScaleValue = DrunkCrosshair.Scale;

        crosshair.transform.localScale = new Vector2(newScaleValue, newScaleValue);
    }

	void ChangeCrosshairColor()
	{
		crosshair.color = DrunkCrosshair.TargetOnClearSight ? darkRedColor : Color.white;
	}

	void MoveCrosshair()
	{
		crosshair.rectTransform.position = DrunkCrosshair.Position;
	}

	void ToggleCrosshair()
	{
		crosshair.enabled = !crosshair.enabled;
	}

    void ChangeAmmoDisplay()
    {
        int bulletsInCylinder = weaponHolder.EquippedGun.BulletsInGun;
        int cylinderCapacity = weaponHolder.EquippedGun.GunCapacity;

		if (bulletsInCylinder <= CRITICAL_AMMO_IN_GUN)
		{
			if (bulletsInCylinder == 0)
				ammoHUDAnimator.SetTrigger("Has to Pop");
			ammoText.color = darkRedColor;
		}
		else
			ammoText.color = Color.white;

        ammoText.text = bulletsInCylinder.ToString() + "/" + cylinderCapacity.ToString();
    }

    void ChangeConsumablesDisplay()
    {
        int amount = PlayerManager.Instance.GetItemAmount(weaponHolder.EquippedConsumable.GetName());
        int maxAmount = PlayerManager.Instance.GetItemMaxAmount(weaponHolder.EquippedConsumable.GetName());

		
		if (amount == 0)
			consumablesHUD.SetActive(false);
		else
		{
			consumablesHUDAnimator.SetTrigger("Has to Pop");
			consumablesText.color = Color.white;
		}

        consumablesText.text = amount.ToString() + "/" + maxAmount.ToString();
    }

	void ChangeKillsDisplay()
	{
		int targetsKilled = LevelManager.Instance.TargetsKilledInStage;
		int minimumRequiredKills = LevelManager.Instance.MinimumRequiredKills;
		int maximumRequiredKills = LevelManager.Instance.MaximumRequiredKills;
		int requiredKillsForNextTier = LevelManager.Instance.RequiredKillsForNextTier;
		int requiredKillsForCurrentTier = LevelManager.Instance.RequiredKillsForCurrentTier;

		crowsText.text = targetsKilled < maximumRequiredKills ? targetsKilled.ToString() + "/" + requiredKillsForNextTier.ToString() :
																targetsKilled.ToString();
		
		if (targetsKilled == requiredKillsForCurrentTier)
		{
			crowHUDAnimator.SetTrigger("Has to Pop");
			if (targetsKilled == minimumRequiredKills)
			{
				crowsText.color = bronzeColor;
				rankBannerText.text = "Bronze Rank Achieved!";
				rankImage.sprite = rankSprites[0];
			}
			else
			{
				if (targetsKilled == maximumRequiredKills)
				{
					crowsText.color = goldColor;
					rankBannerText.text = "Gold Rank Achieved!";
					rankImage.sprite = rankSprites[2];
				}
				else
				{
					crowsText.color = silverColor;
					rankBannerText.text = "Silver Rank Achieved!";
					rankImage.sprite = rankSprites[1];					
				}
			}
			rankBanner.SetActive(true);
			rankBannerAnimator.SetTrigger("Start");
			rankBannerSound.Play();
		}
		else
			if (crowsText.color != Color.white && targetsKilled < minimumRequiredKills)
				crowsText.color = Color.white;
	}

	void ChangeTimerDisplay()
	{
		int timeLeft = (int)LevelManager.Instance.TimeLeft;
		timerText.text = timeLeft.ToString() + "\"";

		if (timeLeft <= CRITICAL_TIME_LEFT)
		{
			timerText.color = darkRedColor;
			clockTickTimer += Time.deltaTime;
			if (clockTickTimer >= 1f)
			{
				timerHUDAnimator.SetTrigger("Has to Pop");
				clockTickTimer = 0f;
				int randomIndex = Random.Range(0, clockTickSounds.Length);
				clockTickSounds[randomIndex].Play();
			}
		}
		else
			timerText.color = Color.white;
	}

	void ComputeRankBannerDisplay()
	{
		if (rankBanner.activeInHierarchy && !objectiveBannerWasJustDisabled)
		{
			if (objectiveBannerTimer >= rankBannerDuration)
			{
				objectiveBannerWasJustDisabled = true;
				objectiveBannerTimer = 0f;
				rankBannerAnimator.SetTrigger("Exit");
				slideOutBannerSound.Play();
				Invoke("DisableRankBanner", slidingAnimation.length);
			}
			else
				objectiveBannerTimer += Time.deltaTime;
		}
	}

	void PopAmmoHUD()
	{
		ammoHUDAnimator.SetTrigger("Has to Pop");
	}

	void DisableRankBanner()
	{
		objectiveBannerWasJustDisabled = false;
		rankBanner.SetActive(false);
	}

	void ChangeGunInDisplay()
	{
        weaponHolder.EquippedGun.OnShot.AddListener(ChangeAmmoDisplay);
        weaponHolder.EquippedGun.OnIncreaseBulletCount.AddListener(ChangeAmmoDisplay);
        weaponHolder.EquippedGun.OnEmptyGun.AddListener(PopAmmoHUD);
        weaponHolder.CurrentCrosshair.OnScale.AddListener(ScaleCrosshair);
        weaponHolder.CurrentCrosshair.OnColorChange.AddListener(ChangeCrosshairColor);
        weaponHolder.CurrentCrosshair.OnMove.AddListener(MoveCrosshair);

		ammoHUD.GetComponent<Image>().sprite = gunSprites[(int)weaponHolder.EquippedGun.TypeOfGun];

		ChangeCrosshairColor();
		ChangeAmmoDisplay();
	}

	void ChangeConsumableInDisplay()
	{
		if (weaponHolder.EquippedConsumable)
		{
			consumablesHUD.SetActive(true);
			weaponHolder.EquippedConsumable.OnUse.AddListener(ChangeConsumablesDisplay);

            consumablesHUD.GetComponent<Image>().sprite = consumableSprites[weaponHolder.EquippedConsumableIndex];

            ChangeConsumablesDisplay();
		}
		else
			consumablesHUD.SetActive(false);
	}

	public void ChangeCurrencyDisplay(int currency)
	{
		currencyText.text = currency.ToString();
	}
}