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
	[SerializeField] string reloadInstruction;
	[SerializeField] AnimationClip slidingAnimation;
	BannerType activeBannerType;
	Animator bannerAnimator;
	int initialInstructionIndex = 0;
	float timer = 0f;
	bool initialBannerEnabled = false;
	bool reloadBannerEnabled = false;
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
				if (timer >= instructionDisplayDuration + initialInstructionIndex * slidingAnimation.length)
				{
					timer = 0;
					initialInstructionIndex++;
					bannerAnimator.SetTrigger("Exit");
					
					if (initialInstructionIndex != initialInstructions.Length)
						Invoke("ShowNextBanner", slidingAnimation.length);
					else
					{
						activeBannerType = BannerType.None;
						bannerAnimator.SetTrigger("Exit");
						Invoke("DisableBanner", slidingAnimation.length);
					}
				}
				else
					timer += Time.deltaTime;
				break;
			
			case BannerType.Reload:
				if (!hasPressedReloadButton && InputManager.Instance.GetReloadButton())
					hasPressedReloadButton = true;
				if (timer >= instructionDisplayDuration)
				{
					if (hasPressedReloadButton)
					{
						timer = 0;
						activeBannerType = BannerType.None;
						bannerAnimator.SetTrigger("Exit");
						Invoke("DisableBanner", 0.33f);
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
		tutorialText.text = reloadInstruction;
		activeBannerType = BannerType.Reload;
		bannerAnimator.SetTrigger("Start");
	}

	void DisableBanner()
	{
        banner.SetActive(false);
	}

	void ShowNextBanner()
	{
		tutorialText.text = initialInstructions[initialInstructionIndex];
		bannerAnimator.SetTrigger("Start");
	}
}
