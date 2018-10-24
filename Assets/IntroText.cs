using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroText : MonoBehaviour
{
	[SerializeField] AnimationClip fadeOutAnimation;
	Animator animator;
	PauseMenu pauseMenu;

	void Start()
	{
		Time.timeScale = 0f;
		GameManager.Instance.ShowCursor();
		PlayerManager.Instance.DisablePlayerComponent(PlayerManager.PlayerComponent.GunComp);
		animator = GetComponent<Animator>();
		pauseMenu = transform.GetComponentInParent<PauseMenu>();
		pauseMenu.enabled = false;
	}

	void DeactivateObject()
	{
		gameObject.SetActive(false);
		PlayerManager.Instance.EnablePlayerComponent(PlayerManager.PlayerComponent.GunComp);
		pauseMenu.enabled = true;
	}

	public void Continue()
	{
		Time.timeScale = 1f;
		GameManager.Instance.HideCursor();
		animator.SetTrigger("Fade Out");
		Invoke("DeactivateObject", fadeOutAnimation.length);
	}
}
