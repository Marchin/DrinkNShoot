using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialUI : MonoBehaviour
{
	enum BannerType
	{
		None, Initial, Reload
	}

	[SerializeField] GameObject banner;
	[SerializeField] TextMeshProUGUI tutorialText;
	[SerializeField] float instructionDisplayDuration = 5f;
	[SerializeField] float initialBannerDelay = 3f;
	[SerializeField] string[] initialInstructions;
	[SerializeField] string[] reloadInstructions;
	[SerializeField] AnimationClip slidingAnimation;
	BannerType activeBannerType;
	Animator bannerAnimator;
	int initialInstructionsIndex = 0;
	int reloadInstructionsIndex = 0;
	float timer = 0f;
	bool hasPressedReloadButton = false;

	void Awake()
	{
		bannerAnimator = banner.GetComponent<Animator>();
	}

	void Start()
	{
		Invoke("EnableInitialBanner", initialBannerDelay);
		LevelManager.Instance.OnFirstEmptyGun.AddListener(EnableReloadBanner);
	}

	void Update()
	{
		switch (activeBannerType)
		{
			case BannerType.Initial:
				if (timer >= instructionDisplayDuration + initialInstructionsIndex * slidingAnimation.length)
				{
					timer = 0;
					initialInstructionsIndex++;
					bannerAnimator.SetTrigger("Exit");			
					if (initialInstructionsIndex < initialInstructions.Length)
						Invoke("ShowNextBanner", slidingAnimation.length);
					else
					{
						activeBannerType = BannerType.None;
						initialInstructionsIndex = 0;
						Invoke("DisableBanner", slidingAnimation.length);
					}
				}
				else
					timer += Time.deltaTime;
				break;
			
			case BannerType.Reload:
				if (!hasPressedReloadButton && InputManager.Instance.GetReloadButton())
					hasPressedReloadButton = true;			
				if (timer >= instructionDisplayDuration && hasPressedReloadButton)
				{
					timer = 0;
					reloadInstructionsIndex++;
					bannerAnimator.SetTrigger("Exit");
					if (reloadInstructionsIndex < reloadInstructions.Length)
						Invoke("ShowNextBanner", slidingAnimation.length);
					else
					{
						activeBannerType = BannerType.None;
						reloadInstructionsIndex = 0;
						Invoke("DisableBanner",slidingAnimation.length);
					}
				}
				else
					timer += Time.deltaTime;
				break;
			
			default:
				break;
		}
	}

	void EnableInitialBanner()
	{
		timer = 0;
		banner.SetActive(true);
		activeBannerType = BannerType.Initial;
		bannerAnimator.SetTrigger("Start");
	}

	void EnableReloadBanner()
	{
		timer = 0;
		banner.SetActive(true);
		tutorialText.text = reloadInstructions[0];
		activeBannerType = BannerType.Reload;
		bannerAnimator.SetTrigger("Start");
	}

	void DisableBanner()
	{
        banner.SetActive(false);
	}

	void ShowNextBanner()
	{
		switch (activeBannerType)
		{
			case BannerType.Initial:
				tutorialText.text = initialInstructions[initialInstructionsIndex];
				break;
			
			case BannerType.Reload:
				tutorialText.text = reloadInstructions[reloadInstructionsIndex];
				break;
		}
		bannerAnimator.SetTrigger("Start");
	}
}
