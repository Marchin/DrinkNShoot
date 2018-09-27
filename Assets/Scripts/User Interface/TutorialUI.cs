﻿using System.Collections;
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
	[SerializeField] float instructionDisplayDuration = 8f;
	[SerializeField] float initialBannerDelay = 3f;
	[SerializeField] string[] initialInstructions;
	[SerializeField] string reloadInstruction;
	BannerType activeBannerType;
	int initialInstructionIndex = 0;
	float timer = 0f;
	bool initialBannerEnabled = false;
	bool reloadBannerEnabled = false;
	bool hasPressedReloadButton = false;

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
				if (timer >= instructionDisplayDuration)
				{
					timer = 0;
					initialInstructionIndex++;
					
					if (initialInstructionIndex != initialInstructions.Length)
						tutorialText.text = initialInstructions[initialInstructionIndex];
					else
						DisableBanner();
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
						DisableBanner();
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
	}

	void EnableReloadBanner()
	{
		timer = 0;
		banner.SetActive(true);
		tutorialText.text = reloadInstruction;
		activeBannerType = BannerType.Reload;
	}

	void DisableBanner()
	{
		activeBannerType = BannerType.None;
        banner.SetActive(false);
	}
}
