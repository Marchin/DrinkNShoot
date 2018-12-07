using System.Collections;
using UnityEngine;

public class Wagon : MonoBehaviour
{
	[SerializeField] Animation horseAnimation;
	[SerializeField] AudioSource horseSound;
	[SerializeField] AudioSource wagonSound;
	[SerializeField] [Range(0.1f, 0.5f)]
	float decelarationRate = 0.3f;

	string horseClipName;
	
	void Awake()
	{
		horseClipName = horseAnimation.clip.name;
	}

	public void Move()
	{
		horseAnimation.Play();
		horseSound.Play();
		wagonSound.Play();
	}

	public void Stop()
	{
		StartCoroutine(Decelerate());
	}

	IEnumerator Decelerate()
	{
		while (horseAnimation[horseClipName].speed > 0f)
		{
			horseAnimation[horseClipName].speed -= decelarationRate * Time.deltaTime;
			yield return null;
		}
		
		horseAnimation[horseClipName].speed = 1f;
		horseAnimation.Stop();
		horseSound.Stop();
		wagonSound.Stop();
	}
}
