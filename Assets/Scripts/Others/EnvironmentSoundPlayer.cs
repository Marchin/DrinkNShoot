using UnityEngine;

public class EnvironmentSoundPlayer : MonoBehaviour
{
	[SerializeField] AudioSource audio;

	void Start()
	{
		LevelManager.Instance.OnStartPlayingEnvironmentSounds.AddListener(PlaySound);
	}

	void PlaySound()
	{
		audio.Play();
	}
}