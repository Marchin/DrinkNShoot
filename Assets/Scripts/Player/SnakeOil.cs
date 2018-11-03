using System.Collections;
using UnityEngine;

public class SnakeOil : Consumable 
{
	[Header("Snake Oil Properties")]
	[SerializeField] float deadEyeDuration = 10f;
	[SerializeField] float transitionDuration = 1f;
	[SerializeField] float deadEyeFactor = 0.1f;

	float deadEyeTimer = 0f;
	float previousFixedDeltaTime = 0f;
	
	protected override void Update()
	{
		base.Update();

		if (isUsing)
		{
			deadEyeTimer += Time.unscaledDeltaTime;
			if (HasToStopEffect())		
			{
				deadEyeTimer = 0f;
				isUsing = false;
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
		
		Time.timeScale = 1f;
		Time.fixedDeltaTime = previousFixedDeltaTime;
	}
}