using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IntroText : MonoBehaviour
{
	[SerializeField] string[] slidesText;
	[SerializeField] TextMeshProUGUI bodyText;
	[SerializeField] Button continueButton;
	[SerializeField] Button skipButton;
	[SerializeField] AnimationClip fadeOutAnimation;
	[SerializeField] AnimationClip textOutAnimation;
	
	Animator animator;
	PauseMenu pauseMenu;
	int currentSlideIndex = 0;

	void Awake()
	{
		animator = GetComponent<Animator>();
		pauseMenu = transform.GetComponentInParent<PauseMenu>();
		pauseMenu.enabled = false;
		bodyText.text = slidesText[currentSlideIndex];
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
		pauseMenu.enabled = true;
	}

	IEnumerator SwapSlideText()
	{
		yield return new WaitForSecondsRealtime(textOutAnimation.length);

        bodyText.text = slidesText[currentSlideIndex];

        continueButton.interactable = true;
        skipButton.interactable = true;
	}

	public void Continue()
	{
		currentSlideIndex++;
		
		if (currentSlideIndex < slidesText.GetLength(0))
		{
            continueButton.interactable = false;
            skipButton.interactable = false;
			animator.SetTrigger("Text Out");
			StartCoroutine(SwapSlideText());
		}
		else
			FinishPresentation();
	}

	public void FinishPresentation()
	{
		continueButton.gameObject.SetActive(false);
		skipButton.gameObject.SetActive(false);
		
		Time.timeScale = 1f;
		GameManager.Instance.HideCursor();
		animator.SetTrigger("Fade Out");
        
		PlayerManager.Instance.EnablePlayerComponent(PlayerManager.PlayerComponent.GunComp);
		PlayerManager.Instance.EnablePlayerComponent(PlayerManager.PlayerComponent.CameraRotationComp);
		
		Invoke("DeactivateObject", fadeOutAnimation.length);
	}
}