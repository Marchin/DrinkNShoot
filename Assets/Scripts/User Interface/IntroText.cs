using UnityEngine;

public class IntroText : MonoBehaviour
{
	[SerializeField] AnimationClip fadeOutAnimation;
	
	Animator animator;
	PauseMenu pauseMenu;

	void Awake()
	{
		animator = GetComponent<Animator>();
		pauseMenu = transform.GetComponentInParent<PauseMenu>();
		pauseMenu.enabled = false;
	}

	void Start()
	{
		Time.timeScale = 0f;
		GameManager.Instance.ShowCursor();
		PlayerManager.Instance.DisablePlayerComponent(PlayerManager.PlayerComponent.GunComp);
		PlayerManager.Instance.DisablePlayerComponent(PlayerManager.PlayerComponent.CameraRotationComp);
	}

	void DeactivateObject()
	{
		gameObject.SetActive(false);
		PlayerManager.Instance.EnablePlayerComponent(PlayerManager.PlayerComponent.GunComp);
		PlayerManager.Instance.EnablePlayerComponent(PlayerManager.PlayerComponent.CameraRotationComp);
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