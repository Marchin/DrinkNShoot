using UnityEngine;
using UnityEngine.Video;

public class SplashPlayer : MonoBehaviour
{
	[SerializeField] GameObject mainMenuUI;
	[SerializeField] GameObject splashVideo;
	[SerializeField] VideoClip video;

	void Awake()
	{

		mainMenuUI.SetActive(!GameManager.Instance.ShouldPlaySplashVideo);
		splashVideo.SetActive(GameManager.Instance.ShouldPlaySplashVideo);

		if (GameManager.Instance.ShouldPlaySplashVideo)
		{
			GameManager.Instance.HideCursor();	
			Invoke("EnableMenu", (float)video.length);
		}
	}

	void EnableMenu()
	{
		mainMenuUI.SetActive(true);
		GameManager.Instance.ShouldPlaySplashVideo = false;
		GameManager.Instance.ShowCursor();
	}
}