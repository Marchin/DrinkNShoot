using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class IntroText : MonoBehaviour
{
	[Header("Slides' Text")]
	[SerializeField] string[] slidesText;
	[SerializeField] TextMeshProUGUI bodyText;
	
	[Header("Slides' Images")]
	[SerializeField] Image[] firstSlideImages;
	[SerializeField] Image[] secondSlideImages;
	[SerializeField] Image[] thirdSlideImages;
	[SerializeField] Image[] fourthSlideImages;
	[SerializeField] Image[] fifthSlideImages;

	[Header("Interface Buttons")]
	[SerializeField] Button continueButton;
	[SerializeField] Button skipButton;

	[Header("Intro Animations")]
	[SerializeField] AnimationClip fadeOutAnimation;
	[SerializeField] AnimationClip textOutAnimation;
	
	Animator animator;
	AudioSource introTheme;
	PauseMenu pauseMenu;
	int currentSlideIndex = 0;

	void Awake()
	{
		animator = GetComponent<Animator>();
		introTheme = GetComponent<AudioSource>();
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
		introTheme.Stop();
		gameObject.SetActive(false);
		pauseMenu.enabled = true;
	}

	void SwapSlideImages(Image[] previousImages, Image[] nextImages)
	{
        foreach (Image image in previousImages)
			image.gameObject.SetActive(false);
        
		foreach (Image image in nextImages)
			image.gameObject.SetActive(true);
	}

	IEnumerator SwapSlide()
	{
		yield return new WaitForSecondsRealtime(textOutAnimation.length);

        bodyText.text = slidesText[currentSlideIndex];

		switch (currentSlideIndex)
		{
			case 1:
				SwapSlideImages(firstSlideImages, secondSlideImages);
				break;
			case 2:
				SwapSlideImages(secondSlideImages, thirdSlideImages);
				break;
			case 3:
				SwapSlideImages(thirdSlideImages, fourthSlideImages);
				break;
			case 4:
				SwapSlideImages(fourthSlideImages, fifthSlideImages);
				break;
			default:
				break;
		}

        continueButton.interactable = true;
		if (currentSlideIndex < slidesText.GetLength(0) - 1) 
        	skipButton.interactable = true;
		else
			skipButton.gameObject.SetActive(false);
	}

	public void Continue()
	{
		currentSlideIndex++;
		
		if (currentSlideIndex < slidesText.GetLength(0))
		{
            continueButton.interactable = false;
            skipButton.interactable = false;
			animator.SetTrigger("Text Out");
			StartCoroutine(SwapSlide());
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