using System.Collections;
using UnityEngine;
using UnityEngine.PostProcessing;

public class SnakeOil : Consumable 
{
	[Header("Snake Oil Properties")]
	[SerializeField] float deadEyeDuration = 10f;
	[SerializeField] float transitionDuration = 1f;
	[SerializeField] float deadEyeFactor = 0.1f;

	PostProcessingBehaviour postProcessingBehaviour;
	float deadEyeTimer = 0f;
	float previousFixedDeltaTime = 0f;
	bool isApplyingEffect = false;
	
	protected override void Update()
	{
		base.Update();

		if (isApplyingEffect)
		{
			deadEyeTimer += Time.unscaledDeltaTime;
			if (HasToStopEffect())		
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
		PlayerManager.Instance.DisablePlayerComponent(PlayerManager.PlayerComponent.DrunkCameraComp);
		postProcessingBehaviour = PlayerManager.Instance.FPSCamera.GetComponent<PostProcessingBehaviour>();
		postProcessingBehaviour.profile.colorGrading.enabled = true;
		Time.timeScale = deadEyeFactor;
		previousFixedDeltaTime = Time.fixedDeltaTime;
		Time.fixedDeltaTime *= deadEyeFactor;
		isApplyingEffect = true;
	}

	bool HasToStopEffect()
	{
		return (deadEyeTimer >= deadEyeDuration || PlayerManager.Instance.CurrentGun.BulletsInGun == 0);
	}

	IEnumerator GoBackToNormalTime()
	{
		while (Time.timeScale < 1f)
		{
			Time.timeScale += (1f / transitionDuration) * Time.unscaledDeltaTime;
			
			yield return null;
		}
		
		postProcessingBehaviour.profile.colorGrading.enabled = false;
		Time.timeScale = 1f;
		Time.fixedDeltaTime = previousFixedDeltaTime;
	}
}