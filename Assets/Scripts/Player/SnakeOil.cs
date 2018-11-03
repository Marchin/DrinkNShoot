using System.Collections;
using UnityEngine;

public class SnakeOil : Consumable 
{
	[SerializeField] float deadEyeDuration = 4f;
	[SerializeField] float transitionDuration = 0.5f;
	[SerializeField] float deadEyeFactor = 0.1f;

	float deadEyeTimer;
	float previousFixedDeltaTime;
	
	void Update()
	{
		if (isConsuming)
		{
			deadEyeTimer += Time.unscaledDeltaTime;
			if (deadEyeTimer >= deadEyeDuration)
			{
				deadEyeTimer = 0f;
				isConsuming = false;
				StartCoroutine(GoBackToNormalTime());
			}
		}
	}

	protected override void ApplyConsumableEffect()
	{
		PlayerManager.Instance.DisablePlayerComponent(PlayerManager.PlayerComponent.DrunkCameraComp);
		Time.timeScale = deadEyeFactor;
		previousFixedDeltaTime = Time.fixedDeltaTime;
		Time.fixedDeltaTime *= deadEyeFactor;
	}

	IEnumerator GoBackToNormalTime()
	{
		while (Time.timeScale < 1f)
		{
			Time.timeScale += Mathf.Clamp01((1f / transitionDuration) * Time.unscaledDeltaTime);
			
			yield return null;
		}
		
		Time.fixedDeltaTime = previousFixedDeltaTime;
	}
}
