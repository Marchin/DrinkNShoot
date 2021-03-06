﻿using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PostProcessing;

public class SnakeOil : Consumable 
{
	[Header("Snake Oil Properties")]
	[SerializeField] float deadEyeDuration = 10f;
	[SerializeField] float deadEyeFactor = 0.1f;
	[SerializeField] AudioClip deadEyeExitSound;

	PostProcessingBehaviour postProcessingBehaviour;
	float deadEyeTimer = 0f;
	float previousFixedDeltaTime = 0f;
    float transitionDuration = 1f;
	bool isApplyingEffect = false;
	
	UnityEvent onBackToNormalTime = new UnityEvent();

	void Awake()
	{
		transitionDuration = deadEyeExitSound.length;
	}

	protected override void Start()
	{
		base.Start();
        postProcessingBehaviour = FindObjectOfType<Camera>().GetComponent<PostProcessingBehaviour>();
    }

	protected override void Update()
	{
		base.Update();
		if (isApplyingEffect)
		{				
			deadEyeTimer += Time.unscaledDeltaTime;
			if (HasToStopEffect() || WantsToStopEffect())
			{
				deadEyeTimer = 0f;
				isInUse = false;
				isApplyingEffect = false;
				StartCoroutine(GoBackToNormalTime());
			}
		}
	}

	protected override void ApplyConsumableEffect()
	{
		postProcessingBehaviour.profile.colorGrading.enabled = true;
		Time.timeScale = deadEyeFactor;
		previousFixedDeltaTime = Time.fixedDeltaTime;
		Time.fixedDeltaTime *= deadEyeFactor;
		isApplyingEffect = true;
	}

	bool HasToStopEffect()
	{
		return (deadEyeTimer >= deadEyeDuration);
	}

	bool WantsToStopEffect()
	{
		return (InputManager.Instance.GetUseItemButton() && isApplyingEffect);
	}

	IEnumerator GoBackToNormalTime()
	{
		onBackToNormalTime.Invoke();

		while (Time.timeScale < 1f)
		{
			Time.timeScale += (1f / transitionDuration) * Time.unscaledDeltaTime;
			
			yield return null;
		}
		
		StopEffect();
	}

	public void StopEffect()
	{
		postProcessingBehaviour.profile.colorGrading.enabled = false;
		Time.timeScale = 1f;
		Time.fixedDeltaTime = previousFixedDeltaTime;
	}

	public bool IsApplyingEffect
	{
		get { return isApplyingEffect; }
	}

	public UnityEvent OnBackToNormalTime
	{
		get { return onBackToNormalTime; }
	}
}